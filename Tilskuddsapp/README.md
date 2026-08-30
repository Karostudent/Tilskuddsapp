# Tilskuddsapp - ALIS Tilskuddsportal

> Praksisprosjekt i Kristiansand kommune H26

En webapplikasjon for administrasjon av ALIS-tilskudd (Allmennmedisinsk lisens) for leger i spesialisering. Systemet håndterer registrering av tilskuddssaker, automatisk beregning av tilskuddsfordeling, og oversikt over leger og veiledere.

## 📋 Innholdsfortegnelse

- [Funksjoner](#-funksjoner)
- [Teknologier](#-teknologier)
- [Kom i gang](#-kom-i-gang)
- [Database](#-database)
- [Docker](#-docker)
- [Prosjektstruktur](#-prosjektstruktur)
- [Bruk av systemet](#-bruk-av-systemet)
- [Feilsøking](#-feilsøking)

## ✨ Funksjoner

### Tilskuddssaker
- ✅ Opprett nye tilskuddssaker for leger
- ✅ Automatisk beregning av tilskuddsfordeling mellom:
  - Lege (dokumenterte kostnader)
  - Veileder (honorar basert på timer og timesats)
  - Praksissted (resterende midler)
- ✅ Advarsler for ugunstige konstellasjoner (f.eks. legevakt med ekstern veileder)
- ✅ Oversikt over alle tilskuddssaker med sortering
- ✅ Detaljvisning av enkeltsakar

### Leger
- ✅ Automatisk registrering av leger ved opprettelse av tilskuddssak
- ✅ Oversikt over alle registrerte leger
- ✅ Sporing av antall tilskuddssaker per lege
- ✅ Støtte for to legekategorier:
  - Selvstendig næringsdrivende
  - Legevakt

### Veiledere
- ✅ Automatisk registrering av veiledere ved opprettelse av tilskuddssak
- ✅ Oversikt over alle registrerte veiledere
- ✅ Sporing av antall tilskuddssaker per veileder
- ✅ Støtte for to veiledertyper:
  - Intern veileder
  - Ekstern veileder

## 🛠 Teknologier

### Backend
- **ASP.NET Core 10.0** - Web framework
- **Entity Framework Core** - ORM for database-tilgang
- **SQLite** - Lettvekt database

### Frontend
- **Razor Pages** - Server-side rendering
- **Bootstrap 5** - CSS framework
- **jQuery** - JavaScript-bibliotek

### DevOps
- **Docker** - Containerisering
- **Git** - Versjonskontroll

## 🚀 Kom i gang

### Forutsetninger

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (valgfritt, for containerisering)

### Lokal installasjon

1. **Klon repositoryet**
```bash
git clone <repository-url>
cd Tilskuddsapp
```

2. **Installer dependencies**
```bash
dotnet restore
```

3. **Opprett database**
```bash
dotnet ef database update
```

4. **Kjør applikasjonen**
```bash
dotnet run
```

Applikasjonen vil være tilgjengelig på `https://localhost:5001` eller `http://localhost:5000`.

### Docker-installasjon

1. **Bygg Docker-image**
```bash
docker build -t tilskuddsapp .
```

2. **Kjør container**
```bash
docker run -p 8080:8080 -p 8081:8081 tilskuddsapp
```

Applikasjonen vil være tilgjengelig på `http://localhost:8080`.

## 🗄 Database

Applikasjonen bruker SQLite som database. Det finnes to forskjellige connection strings:

### Lokal utvikling
```json
"DefaultConnection": "Data Source=Tilskuddsapp.db;Mode=ReadWriteCreate"
```
Database-filen opprettes i prosjektmappen.

### Docker/Production
```json
"DefaultConnection": "Data Source=/app/data/Tilskuddsapp.db;Mode=ReadWriteCreate"
```
Database-filen opprettes i `/app/data/` med korrekte tillatelser.

### Migrasjoner

Opprett ny migration:
```bash
dotnet ef migrations add <MigrationName>
```

Oppdater database:
```bash
dotnet ef database update
```

Tilbakestill database:
```bash
# Slett database-filene
rm Tilskuddsapp.db*

# Slett migrations-mappen
rm -rf Migrations/

# Opprett ny initial migration
dotnet ef migrations add InitialCreate

# Opprett database
dotnet ef database update
```

## 🐳 Docker

### Dockerfile-struktur

Applikasjonen bruker multi-stage build for optimal størrelse:

1. **Base stage** - Runtime-miljø med `/app/data` mappe for database
2. **Build stage** - Bygger applikasjonen
3. **Publish stage** - Publiserer built artifacts
4. **Final stage** - Minimalt runtime-image

### Viktig for SQLite i Docker

Dockerfile oppretter `/app/data` med riktige tillatelser:
```dockerfile
RUN mkdir -p /app/data && chmod 777 /app/data
```

Dette sikrer at den ikke-root brukeren som kjører applikasjonen har skrivetilgang til database-filen.

## 📁 Prosjektstruktur

```
Tilskuddsapp/
├── Controllers/          # MVC Controllers
│   ├── GrantCasesController.cs
│   ├── DoctorsController.cs
│   ├── SupervisorsController.cs
│   └── HomeController.cs
├── Models/              # Datamodeller
│   ├── GrantCase.cs
│   ├── Doctor.cs
│   ├── Supervisor.cs
│   ├── Expense.cs
│   └── Enums.cs
├── ViewModels/          # View-spesifikke modeller
│   └── GrantCaseFormViewModel.cs
├── Views/               # Razor views
│   ├── GrantCases/
│   ├── Doctors/
│   ├── Supervisors/
│   ├── Home/
│   └── Shared/
├── Data/                # Database context
│   └── ApplicationDbContext.cs
├── Migrations/          # EF Core migrations
├── wwwroot/            # Statiske filer
├── appsettings.json    # Produksjons-config
├── appsettings.Development.json  # Utviklings-config
└── Dockerfile
```

## 📖 Bruk av systemet

### 1. Opprett tilskuddssak

1. Klikk på **"Opprett tilskuddssak"** på forsiden eller i menyen
2. Fyll ut skjemaet:
   - Legens navn og type
   - Tilskuddsår og innvilget beløp
   - Dokumenterte kostnader (for selvstendig næringsdrivende)
   - Veilederens navn, type, timer og timesats
3. Klikk **"Beregn fordeling"**
4. Gjennomgå beregningen og eventuelle advarsler
5. Klikk **"Lagre tilskuddssak"**

### 2. Se oversikt over tilskuddssaker

- Klikk på **"Oversikt tilskuddssaker"**
- Listen viser alle saker sortert etter år (nyeste først) og legenavn
- Hver sak viser lege, år, og status

### 3. Se leger og veiledere

- **Oversikt leger**: Viser alle registrerte leger med antall tilskuddssaker
- **Oversikt veiledere**: Viser alle registrerte veiledere med antall tilskuddssaker og type

## 🔧 Feilsøking

### SQLite Error 10: 'disk I/O error'

**Problem:** Database-filen kan ikke skrives til.

**Løsning:**
- **Lokal utvikling:** Sjekk at du har skrivetilgang til prosjektmappen
- **Docker:** Sørg for at `/app/data` mappen har riktige tillatelser (chmod 777)

### SQLite Error 14: 'unable to open database file'

**Problem:** Database-filen finnes ikke eller stien er feil.

**Løsning:**
- Kjør `dotnet ef database update`
- Sjekk at connection string peker til riktig sti

### Veiledere/Leger vises ikke i oversikt

**Problem:** Navigation properties er ikke lastet inn.

**Løsning:**
- Sjekk at controller bruker `.Include()` for å laste relasjoner
- Eksempel: `.Include(s => s.GrantCases)`

### Antall tilskuddssaker viser 0

**Problem:** EF Core-relasjonen er ikke konfigurert korrekt.

**Løsning:**
- Sjekk `ApplicationDbContext.OnModelCreating`
- Sørg for at `.WithMany(x => x.Collection)` er spesifisert (ikke tom)

## 🤝 Bidrag

Dette er et praksisprosjekt for Kristiansand kommune. For spørsmål eller forslag, kontakt prosjektleder.

## 📄 Lisens

[Spesifiser lisens her]

## 📞 Kontakt

- **Prosjekt:** ALIS Tilskuddsportal
- **Organisasjon:** Kristiansand kommune
- **Periode:** Høst 2026

---

*Sist oppdatert: Januar 2026 ved hjelp av GitHub Copilot*