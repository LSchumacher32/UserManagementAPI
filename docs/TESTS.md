# Testdokumentation – UserManagementAPI

## 1. Ziel der Tests

Die manuellen Tests überprüfen die CRUD-Endpunkte der
Benutzerverwaltungs-API sowie die Eingabevalidierung und die zentrale
Behandlung unerwarteter Ausnahmen.

Geprüft werden sowohl erfolgreiche Anfragen als auch Fehlerfälle,
beispielsweise ungültige Eingaben und nicht vorhandene Benutzer.

## 2. Testumgebung

| Merkmal | Beschreibung |
|---|---|
| Testdatum | 02.11.2026 |
| Anwendung | UserManagementAPI |
| Ausführung | Lokal in der Entwicklungsumgebung |
| Testwerkzeug | Swagger UI |
| Datenformat | JSON |
| Speicherung | Statische In-Memory-Liste |

Die Basisadresse lautet:

```text
[https://localhost](https://localhost):<Port>/api/users
```

Der Port richtet sich nach dem verwendeten Startprofil.
Swagger UI ist unter `/swagger` erreichbar.

## 3. Vorbereitung und Durchführung

1. Anwendung starten.
2. Swagger UI öffnen.
3. Den jeweiligen Endpunkt auswählen.
4. Über „Try it out“ die benötigten Parameter und gegebenenfalls
   einen JSON-Body eingeben.
5. Die Anfrage über „Execute“ ausführen.
6. Statuscode und Response-Body mit dem erwarteten Ergebnis vergleichen.
7. Änderungen zusätzlich über einen GET-Aufruf überprüfen.

Nach einem Neustart ist die Benutzerliste leer und der ID-Zähler
beginnt erneut bei 1. Für Tests eines vorhandenen Benutzers wird
deshalb zuerst ein Benutzer über POST angelegt.

Für nachfolgende Anfragen wird die tatsächlich zurückgegebene ID verwendet.
Zusammengehörige Tests werden ohne Neustart durchgeführt.

Es werden ausschließlich fiktive Benutzerdaten verwendet.

## 4. Verwendete Testdaten

### Gültige Benutzerdaten

```json
{
  "firstName": "Max",
  "lastName": "Mustermann",
  "email": "max@example.com"
}
```

### Gültige Daten für eine Aktualisierung

```json
{
  "firstName": "Max",
  "lastName": "Muster",
  "email": "max.muster@example.com"
}
```

### Ungültige Benutzerdaten

```json
{
  "firstName": "",
  "lastName": "Mustermann",
  "email": "keine-email"
}
```

Für Tests mit einer nicht vorhandenen ID wird eine Kennung verwendet,
die aktuell nicht in der Benutzerliste vorhanden ist, beispielsweise 999.

## 5. Testübersicht

| ID | Testfall | Erwartetes Ergebnis | Tatsächliches Ergebnis | Status |
|---|---|---|---|---|
| T01 | GET /api/users | 200 mit JSON-Liste | 200 OK; Benutzerliste als JSON zurückgegeben | Bestanden |
| T02 | POST mit gültigen Daten | 201 mit Benutzer und vergebener ID | 201 Created; Benutzer zurückgegeben und ID automatisch vergeben | Bestanden |
| T03 | GET mit vorhandener ID | 200 mit passendem Benutzer | 200 OK; passender Benutzer zurückgegeben | Bestanden |
| T04 | GET mit nicht vorhandener ID | 404 | 404 Not Found | Bestanden |
| T05 | POST mit leerem Vornamen und ungültiger E-Mail | 400 mit beiden Feldfehlern | 400 Bad Request; Fehlermeldungen für FirstName und Email ausgegeben | Bestanden |
| T06 | PUT mit vorhandener ID und gültigen Daten | 200; anschließendes GET zeigt Änderungen | 200 OK; Änderungen durch anschließendes GET bestätigt | Bestanden |
| T07 | PUT mit nicht vorhandener ID und gültigen Daten | 404 | 404 Not Found | Bestanden |
| T08 | PUT mit vorhandener ID und ungültigen Daten | 400; gespeicherte Daten unverändert | 400 Bad Request; gespeicherte Daten unverändert | Bestanden |
| T09 | DELETE mit vorhandener ID | 204 ohne Response-Body | 204 No Content; kein Response-Body | Bestanden |
| T10 | GET nach DELETE | 404; Benutzer fehlt auch in der Liste | 404 Not Found; Benutzer auch in der Gesamtliste nicht mehr vorhanden | Bestanden |
| T11 | DELETE mit nicht vorhandener ID | 404 | 404 Not Found | Bestanden |
| T12 | Absichtlich ausgelöste Exception | 500 mit allgemeiner Fehlerantwort | 500 Internal Server Error; allgemeine Fehlermeldung ohne interne Exception-Details | Bestanden |

## 6. Eingabevalidierung

Die Validierungsregeln sind über Data Annotations im User-Modell definiert:

- FirstName: Pflichtfeld
- LastName: Pflichtfeld
- Email: Pflichtfeld und Prüfung des E-Mail-Formats

Der UserController verwendet das Attribut `[ApiController]`.
Ungültige Modelldaten werden automatisch abgelehnt, bevor die
Controller-Methode ausgeführt wird.

### Dokumentierter Validierungstest

Bei einer Anfrage mit leerem Vornamen und ungültiger E-Mail-Adresse
wurde folgende Antwort zurückgegeben:

```json
{
  "type": "[https://tools.ietf.org/html/rfc9110#section-15.5.1](https://tools.ietf.org/html/rfc9110#section-15.5.1)",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Email": [
      "Die E-Mail-Adresse ist ungültig."
    ],
    "FirstName": [
      "Der Vorname ist erforderlich."
    ]
  },
  "traceId": "00-9ec9e8c1a15e354af51d38e8ec95f1de-deba8ba486fb8cf5-00"
}
```

Die Antwort enthält die erwarteten feldbezogenen Fehlermeldungen.
Die traceId gehört zu dieser konkreten Anfrage und kann bei
weiteren Aufrufen abweichen.

Die Prüfung wurde außerdem für PUT durchgeführt. Ungültige Eingaben
wurden abgelehnt; die zuvor gespeicherten Benutzerdaten blieben unverändert.

## 7. Zentrale Fehlerbehandlung

Die zentrale Fehlerbehandlung verwendet:

- `AddProblemDetails(...)` zur Konfiguration strukturierter Fehlerantworten
- `UseExceptionHandler()` zur Behandlung unbehandelter Ausnahmen

Für den Test wurde vorübergehend folgende Methode im UserController ergänzt:

```csharp
[HttpGet("test-error")]
public IActionResult TestError()
{
    throw new InvalidOperationException(
        "Absichtlicher Fehler zum Testen der Middleware.");
}
```

Die Methode löste absichtlich eine Ausnahme aus, ohne diese lokal
durch einen try/catch-Block abzufangen.

### Tatsächliche Antwort

```json
{
  "type": "[https://tools.ietf.org/html/rfc9110#section-15.6.1](https://tools.ietf.org/html/rfc9110#section-15.6.1)",
  "title": "Interner Serverfehler",
  "status": 500,
  "detail": "Ein unerwarteter Fehler ist aufgetreten. Bitte versuche es später erneut.",
  "traceId": "00-ac43ad436bd16bd961d25f09c4561155-2fe07699d8cf41a9-00"
}
```

Die Antwort enthielt die konfigurierte allgemeine Fehlermeldung.
Die interne Exception-Nachricht und ein Stacktrace wurden nicht
im Response-Body ausgegeben.

Der temporäre Test-Endpunkt gehört nicht zur regulären API und ist
vor der Veröffentlichung zu entfernen.

## 8. Grenzen der Tests

Die Tests wurden manuell über Swagger UI durchgeführt.

Nicht Bestandteil dieser Testreihe waren:

- Automatisierte Unit- oder Integrationstests
- Lasttests
- Tests paralleler Schreibzugriffe
- Sicherheits- und Penetrationstests
- Authentifizierungs- oder Autorisierungstests, da kein Zugriffsschutz
  implementiert ist
- Prüfung einer dauerhaften Datenbankspeicherung, da die API
  ausschließlich In-Memory-Daten verwendet

Die erfolgreichen Tests belegen das erwartete Verhalten der beschriebenen
Testfälle. Sie stellen keinen Nachweis der Produktionsreife dar.

## 9. Fazit

Alle zwölf dokumentierten manuellen Testfälle wurden erfolgreich
über Swagger UI durchgeführt.

Die Tests bestätigten:

- Das Abrufen aller Benutzer und einzelner Benutzer
- Das Anlegen von Benutzern mit automatischer ID-Vergabe
- Das Aktualisieren vorhandener Benutzer
- Das Löschen vorhandener Benutzer
- Die erwarteten Fehlerantworten bei nicht vorhandenen Benutzern
- Die Ablehnung ungültiger Eingaben
- Die zentrale Behandlung unerwarteter Ausnahmen mit einer allgemeinen
  Fehlerantwort