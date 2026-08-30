# Notater og tanker - Tilskuddsapp

## Oversikt
Dette er en ASP.NET Core MVC-applikasjon (.NET 10) for håndtering av tilskuddssaker for leger i spesialisering.

---

## Endringer og fikser (30. august 2026)

### 1. Fikset GrantCaseController struktur
**Problem:** GrantCaseIndex-viewet ble ikke funnet når løsningen ble kjørt.

**Årsak:** Controller-navnet matchet ikke view-mappen:
- Controller: `GrantCaseController` (uten "s")
- View-mappe: `Views\GrantCases\` (med "s")

**Løsning:**
- Endret controller-navnet til `GrantCasesController` for konsistens med andre controllere (DoctorsController, SupervisorsController)
- ASP.NET Core MVC følger konvensjonen: `ControllerName` → views i `Views\ControllerName\`

---

### 2. Implementert enum-baserte statuser
**Problem:** GrantCaseController brukte hardkodede strenger for status ("Under behandling", "Klar til kontroll").

**Løsning:**
- Endret til enum-verdier fra `Enums.cs`:
  - `GrantCaseStatus.Pending`
  - `GrantCaseStatus.Approved`
  - `GrantCaseStatus.Rejected`
- Lagt til `using Tilskuddsapp.ViewModels;` i controller
- Lagt til `using static Tilskuddsapp.Models.Enums;` for enklere enum-bruk
- Fikset view til å bruke `@grantCase.GrantCaseStatus` i stedet for `@grantCase.Status`

**Fordeler:**
- Type-sikkerhet
- Enklere vedlikehold
- Lettere å utvide med nye statuser senere

**Potensielt fremtidig forbedring:**
- Legge til Display-attributter på enum-verdiene for norske visningsnavn
- Eller lage en extension-metode for oversettelse til norsk

---

### 3. Fikset namespace i ViewModel
**Problem:** Build feilet fordi `GrantCaseFormViewModel` mangler namespace.

**Årsak:** Klassen var definert uten namespace, men views refererte til `Tilskuddsapp.ViewModels.GrantCaseFormViewModel`.

**Løsning:**
- Lagt til `namespace Tilskuddsapp.ViewModels { ... }` rundt klassen
- Lagt til `using Tilskuddsapp.ViewModels;` i GrantCasesController

---

### 4. Fikset viewfilnavn
**Problem:** GrantCaseCalculationResult-viewet ble ikke vist etter beregning.

**Årsak:** Viewfilen het `GrntCaseCalculationResult.cshtml` (mangler "a" i "Grnt").

**Løsning:**
- Omdøpt fil til `GrantCaseCalculationResult.cshtml`
- Nå matcher filnavnet det controlleren returnerer: `return View("GrantCaseCalculationResult", model);`

---

### 5. Forbedret navigasjon
**Endringer:**
- Lagt til "Opprett tilskuddssak" i hovednavigasjonsmenyen (`_Layout.cshtml`)
- Lagt til "Opprett ny tilskuddssak"-knapp på GrantCaseIndex-siden

**Resultat:** Enklere for brukere å opprette nye tilskuddssaker direkte fra navigasjonen eller oversiktssiden.

---

## Fremtidige oppgaver og ideer

### Database-implementering (utsatt)
**Mål:** Når en ny tilskuddssak opprettes, skal legen automatisk lagres i database og vises i DoctorIndex.

**Hva som må gjøres:**
1. Installere Entity Framework Core-pakker:
   - `Microsoft.EntityFrameworkCore.Sqlite` (eller SQL Server)
   - `Microsoft.EntityFrameworkCore.Design`

2. Implementere `ApplicationDbContext`:
   - DbSet for Doctor, GrantCase, Supervisor
   - Konfigurere relasjoner mellom entiteter

3. Registrere DbContext i `Program.cs`:
   ```csharp
   builder.Services.AddDbContext<ApplicationDbContext>(options =>
	   options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
   ```

4. Oppdatere controllere:
   - Injisere ApplicationDbContext via dependency injection
   - Implementere CRUD-operasjoner mot database

5. Migrasjoner:
   - `dotnet ef migrations add InitialCreate`
   - `dotnet ef database update`

**Viktige spørsmål:**
- Skal vi sjekke for duplikater før vi lagrer en Doctor?
- Skal Supervisor også lagres automatisk?
- Trenger vi validering for å unngå duplikate leger?

---

## Prosjektstruktur

### Controllers
- `HomeController.cs` - Hjemmeside
- `GrantCasesController.cs` - Håndterer tilskuddssaker (Create, Index, Details)
- `DoctorsController.cs` - Oversikt over leger
- `SupervisorsController.cs` - Oversikt over veiledere

### Models
- `Doctor.cs` - Lege-entitet med navn, type, og tilskuddssaker
- `GrantCase.cs` - Tilskuddssak med beregninger
- `Supervisor.cs` - Veileder
- `Enums.cs` - DoctorType, SupervisorType, GrantCaseStatus
- `Expense.cs` - Utgifter knyttet til tilskuddssaker
- `Allocation.cs` - Fordeling av midler

### ViewModels
- `GrantCaseFormViewModel.cs` - Skjema for opprettelse av tilskuddssak
- `GrantCaseDetailsViewModel.cs` - Detaljvisning (tom ennå)
- `GrantCalculationViewModel.cs` - Beregningsresultater (tom ennå)
- `ErrorViewModel.cs` - Feilhåndtering

### Views
- `Views/GrantCases/`
  - `GrantCaseIndex.cshtml` - Liste over tilskuddssaker
  - `GrantCaseCreate.cshtml` - Skjema for ny tilskuddssak
  - `GrantCaseCalculationResult.cshtml` - Viser beregningsresultater
  - `GrantCaseDetails.cshtml` - Detaljvisning av tilskuddssak
  - `GrantCaseEdit.cshtml` - Redigering (ikke implementert ennå)

### Services
- `IGrantCalculationService.cs` - Interface for beregningslogikk
- `GrantCalculationService.cs` - Implementering (ikke tatt i bruk ennå)

---

## Tekniske notater

### Nåværende begrensninger
- **Ingen database:** Alt bruker hardkodet testdata i controllerne
- **Ingen persistering:** Data går tapt ved omstart
- **Begrenset validering:** Kun grunnleggende validering på ViewModel-nivå

### Beregningslogikk (i GrantCasesController.Create POST)
1. **Veilederhonorar beregnes:**
   - `SupervisorAmount = SupervisionHours × SupervisorHourlyRate`
   - Kan ikke overstige innvilget tilskudd

2. **Resterende tilskudd fordeles:**
   - `RemainingGrant = ApprovedGrant - SupervisorAmount`

3. **For selvstendig næringsdrivende lege:**
   - `DoctorAmount = Min(DocumentedExpenses, RemainingGrant)`
   - `PracticeAmount = RemainingGrant - DoctorAmount`

4. **For legevakt:**
   - `DoctorAmount = 0`
   - `PracticeAmount = RemainingGrant`

**Fremtidig forbedring:** Flytte denne logikken til `GrantCalculationService`.

---

## Konvensjoner i prosjektet

### Naming
- Controllers: Flertallsform (DoctorsController, GrantCasesController)
- Views: Mappenavn matcher controller uten "Controller"-suffikset
- Enums: Definert i statisk klasse `Enums` med nested enums

### Prosjektoppsett
- Target Framework: .NET 10
- Nullable: Enabled
- Implicit Usings: Enabled
- Docker support: Linux

---

## Nyttige kommandoer

### Build og kjøring
```powershell
dotnet build
dotnet run
```

### Entity Framework (når implementert)
```powershell
dotnet ef migrations add <MigrationName>
dotnet ef database update
dotnet ef database drop --force
```

### Git
```powershell
git status
git add .
git commit -m "Beskrivelse"
git push origin <branch-navn>
```

Nåværende branch: `Struktur_fortsettelse_basert_på_forslag1`

---

## Referanser og lenker
- GitHub repo: https://github.com/Karostudent/Tilskuddsapp
- .NET 10 dokumentasjon: https://learn.microsoft.com/en-us/dotnet/
- ASP.NET Core MVC: https://learn.microsoft.com/en-us/aspnet/core/mvc/

---


•	✅ Leger registreres og vises i DoctorIndex
•	✅ Veiledere registreres og vises i SupervisorIndex med antall tilskuddssaker
•	✅ Tilskuddssaker registreres med koblinger til både leger og veiledere
•	✅ Alt fungerer både lokalt og i Docker


*Sist oppdatert: 30. august 2026*
