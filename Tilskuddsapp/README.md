# Tilskuddsapp

## Oppsett av applikasjonen

Teknologiene som brukes i applikasjonen:

- **Docker**: Bygger og kjører applikasjonen og databasen.
- **PostgreSQL**: Database for lagring av søknader og veiledningsattester som utkast.
- **ASP.NET Core MVC**: Applikasjonen er bygget med .NET 10 MVC.

## Slik kjører du applikasjonen

### Forutsetninger

1. **Installer og start Docker Desktop** ([Last ned Docker Desktop](https://www.docker.com/products/docker-desktop/)).
2. **Klon repositoryet** til maskinen din:

   ```sh
   git clone https://github.com/Karostudent/Tilskuddsapp.git
   cd Tilskuddsapp
   ```

### Starte applikasjonen

1. **Åpne en terminal** i prosjektets rotmappe, der `docker-compose.yml` ligger.
2. **Opprett databasekonfigurasjonen** første gang. Kjør i PowerShell:

   ```powershell
   ./scripts/Initialize-LocalDatabase.ps1
   ```

3. **Bygg og start containerne** med Docker Compose:

   ```sh
   docker compose up --build
   ```

   Kommandoen bygger Docker-bildene og starter både applikasjonen og databasen.

4. **Kjør i bakgrunnen** hvis ønskelig, ved å bruke denne kommandoen i stedet:

   ```sh
   docker compose up -d --build
   ```

5. **Åpne applikasjonen** i nettleseren:

   ```text
   http://localhost:5189
   ```

6. **Kontroller i Docker Desktop** at disse to containerne kjører:

   - `tilskuddsapp-app-1` – ASP.NET Core-applikasjonen.
   - `tilskuddsapp-postgres-1` – PostgreSQL-databasen.

### Bruke applikasjonen

Trykk **SØK HER** for å fylle ut søknaden. Trykk **Neste** for å fylle ut veiledningsattesten. **Lagre utkast** lagrer begge trinnene, og **Åpne lagrede utkast** lar deg fortsette senere.

Appen har foreløpig ikke innlogging eller innsending av søknader. Bruk testdata.

### Stoppe applikasjonen

Trykk `Ctrl+C` i terminalen der appen kjører, eller kjør:

```sh
docker compose down
```

Dataene beholdes i Docker-volumet `tilskuddsapp_postgres_data`.

For å fjerne containerne, nettverket og datavolumet:

```sh
docker compose down -v
```

**Dette sletter alle lagrede utkast.**
