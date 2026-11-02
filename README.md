# UserManagementAPI

Eine ASP.NET Core Web API zur Verwaltung von Benutzern.
Das Projekt entstand als Übungsaufgabe für das fiktive Unternehmen
TechHive Solutions.

Die API verwendet Controller und stellt CRUD-Endpunkte bereit.
Die Endpunkte können über Swagger UI aufgerufen und getestet werden.

## Funktionen

- Alle Benutzer abrufen
- Einzelne Benutzer anhand ihrer ID abrufen
- Benutzer anlegen
- Benutzerdaten aktualisieren
- Benutzer löschen
- Eingaben über Data Annotations validieren
- Unerwartete Ausnahmen zentral behandeln
- Allgemeine Fehlerantworten im ProblemDetails-Format ausgeben

## Verwendete Technologien

- C# und ASP.NET Core
- Controller-basierte Web API
- Swashbuckle.AspNetCore für Swagger UI
- Data Annotations zur Eingabevalidierung
- Exception-Handler-Middleware und ProblemDetails

Das Zielframework und die Paketversionen sind in der Projektdatei
UserManagementAPI.csproj hinterlegt.

## Projektaufbau

Wesentliche Dateien:

- Program.cs: Registrierung der Dienste und Konfiguration der Middleware
- Controllers/UserController.cs: CRUD-Endpunkte
- Models/User.cs: Benutzermodell und Validierungsregeln
- docs/TESTS.md: Testfälle und dokumentierte Ergebnisse

## Lokal starten

1. Repository klonen.
2. Die Solution oder UserManagementAPI.csproj in Visual Studio öffnen.
3. Sicherstellen, dass das zum Zielframework passende .NET SDK installiert ist.
4. NuGet-Pakete wiederherstellen und das Projekt erstellen.
5. Das Projekt mit dem HTTPS-Startprofil in der Entwicklungsumgebung starten.
6. Die Swagger-Oberfläche öffnen:

   https://localhost:<Port>/swagger

Den verwendeten Port findest du in Properties/launchSettings.json
beziehungsweise in der Startausgabe.

Swagger UI ist im aktuellen Aufbau nur in der Development-Umgebung aktiviert.

## Datenmodell

| Eigenschaft | Typ | Bedeutung |
|---|---|---|
| Id | int | Von der API vergebene Kennung |
| FirstName | string | Vorname, Pflichtfeld |
| LastName | string | Nachname, Pflichtfeld |
| Email | string | Pflichtfeld mit E-Mail-Formatprüfung |

Beispiel für POST und PUT:

```json
{
  "firstName": "Max",
  "lastName": "Mustermann",
  "email": "max@example.com"
}
```

Eine ID muss im Request-Body nicht angegeben werden.
POST vergibt sie automatisch. PUT verwendet die ID aus der URL.

## Endpunkte

| Methode | Route | Beschreibung | Vorgesehene Statuscodes |
|---|---|---|---|
| GET | /api/users | Alle Benutzer abrufen | 200 |
| GET | /api/users/{id} | Benutzer anhand der ID abrufen | 200, 404 |
| POST | /api/users | Benutzer anlegen | 201, 400 |
| PUT | /api/users/{id} | Benutzer aktualisieren | 200, 400, 404 |
| DELETE | /api/users/{id} | Benutzer löschen | 204, 404 |

Die Routenparameter für einzelne Benutzer sind auf Ganzzahlen beschränkt.

## Validierung

Die Validierung erfolgt am User-Modell:

- Required für Vorname, Nachname und E-Mail
- EmailAddress für die E-Mail-Adresse

Der Controller verwendet das Attribut ApiController.
Ungültige Modelldaten führen automatisch zu einer Antwort mit
400 Bad Request und feldbezogenen Fehlermeldungen.

Die E-Mail-Formatprüfung bestätigt nicht, dass eine Adresse tatsächlich existiert.

## Zentrale Fehlerbehandlung

Unerwartete Ausnahmen werden durch UseExceptionHandler zentral behandelt.
AddProblemDetails stellt strukturierte Fehlerantworten bereit.

Für Statuscode 500 werden ein allgemeiner Titel und eine allgemeine
Fehlermeldung ausgegeben. Interne Exception-Nachrichten und Stacktraces
werden nicht in diese Antwort übernommen.

Erwartete Fälle wie nicht vorhandene Benutzer werden weiterhin
direkt im Controller behandelt.

## Tests

Die manuellen Tests erfolgen über Swagger UI.

Testfälle, Erwartungen und bisher bestätigte Ergebnisse stehen in
[docs/TESTS.md](docs/TESTS.md).

## Grenzen des aktuellen Stands

- Benutzer werden ausschließlich in einer statischen In-Memory-Liste gespeichert.
- Nach einem Neustart gehen sämtliche Benutzerdaten verloren.
- Der ID-Zähler beginnt nach einem Neustart erneut bei 1.
- Liste und ID-Zähler sind nicht für parallele Schreibzugriffe abgesichert.
- Es gibt keine Datenbank.
- Es gibt keine Authentifizierung oder Autorisierung.
- Doppelte E-Mail-Adressen werden nicht verhindert.
- Automatisierte Tests sind noch nicht vorhanden.

Das Projekt ist eine Lernanwendung und nicht für den produktiven
Umgang mit personenbezogenen Daten vorgesehen.