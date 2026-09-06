using Npgsql;
using Tilskuddsapp.Data;
var builder = WebApplication.CreateBuilder(args);

// Local credentials are ignored by Git. Environment variables take precedence.
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true)
    .AddEnvironmentVariables().AddCommandLine(args);
builder.Services.AddSingleton(_ => NpgsqlDataSource.Create(
    builder.Configuration.GetConnectionString("Postgres")
    ?? throw new InvalidOperationException("Configure ConnectionStrings:Postgres. See README.")));
builder.Services.AddSingleton<GrantDraftStore>();
builder.Services.AddAntiforgery(options => options.HeaderName = "X-CSRF-TOKEN");
builder.Services.AddControllersWithViews();

var app = builder.Build();
await app.Services.GetRequiredService<GrantDraftStore>().InitializeAsync();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
