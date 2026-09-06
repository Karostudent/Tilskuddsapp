using System.Text.Json;
using Npgsql;
using NpgsqlTypes;
using Tilskuddsapp.ViewModels;

namespace Tilskuddsapp.Data;

// A versioned JSONB document atomically preserves the entire unfinished form.
public sealed class GrantDraftStore(NpgsqlDataSource source)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task InitializeAsync()
    {
        await using var command = source.CreateCommand("""
            CREATE TABLE IF NOT EXISTS grant_drafts (
                id integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
                version integer NOT NULL DEFAULT 1,
                schema_version integer NOT NULL DEFAULT 1,
                document jsonb NOT NULL,
                created_at timestamptz NOT NULL DEFAULT now(),
                updated_at timestamptz NOT NULL DEFAULT now()
            )
            """);
        await command.ExecuteNonQueryAsync();
    }

    public async Task<List<DraftSummary>> ListAsync(CancellationToken ct)
    {
        await using var command = source.CreateCommand(
            "SELECT id, updated_at FROM grant_drafts ORDER BY updated_at DESC");
        await using var reader = await command.ExecuteReaderAsync(ct);
        var drafts = new List<DraftSummary>();
        while (await reader.ReadAsync(ct))
            drafts.Add(new(reader.GetInt32(0), reader.GetDateTime(1)));
        return drafts;
    }

    public async Task<GrantApplicationFormViewModel?> GetAsync(int id, CancellationToken ct)
    {
        await using var command = source.CreateCommand(
            "SELECT document::text, version FROM grant_drafts WHERE id = $1");
        command.Parameters.AddWithValue(id);
        await using var reader = await command.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct)) return null;
        var model = JsonSerializer.Deserialize<GrantApplicationFormViewModel>(reader.GetString(0), JsonOptions)!;
        model.Id = id;
        model.Version = reader.GetInt32(1);
        return model;
    }

    public async Task<(int Id, int Version)?> SaveAsync(GrantApplicationFormViewModel model, CancellationToken ct)
    {
        var sql = model.Id.HasValue
            ? "UPDATE grant_drafts SET document = $1, version = version + 1, updated_at = now() WHERE id = $2 AND version = $3 RETURNING id, version"
            : "INSERT INTO grant_drafts(document) VALUES ($1) RETURNING id, version";
        await using var command = source.CreateCommand(sql);
        command.Parameters.Add(new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Jsonb,
            Value = JsonSerializer.Serialize(model, JsonOptions) });
        if (model.Id.HasValue)
        {
            command.Parameters.AddWithValue(model.Id.Value);
            command.Parameters.AddWithValue(model.Version);
        }
        await using var reader = await command.ExecuteReaderAsync(ct);
        return await reader.ReadAsync(ct) ? (reader.GetInt32(0), reader.GetInt32(1)) : null;
    }
}

public sealed record DraftSummary(int Id, DateTime UpdatedAt);
