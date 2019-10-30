# Berger Projekt

## Grundidee: Online Multiplayer Tic Tac Toe

### Tech Stack

.Net Core 3 (Server), .Net Framework 4.7 (Client), MySQL

### Features

- Tic Tac Toe online spielen
- Frontend
- .NET Core Server
- Raumsystem
- Simpler Nutzername durch Nutzereingabe

### Erweiterungen

- Register / Login
- Reconnect
- Chat
- Leaderboards
- Match History

### Erweiterte Erweiterungen

- Lokaler Multiplayer
- Spectating
- SSL Verschlüsselung

### Erweteiterte Erweiterungserweiterungen

- APM
- Matchmaking / Ranked

## Was müssen wir tuten?

- UML Diagramm
- 1 Anwendungsfalldiagramm
- 1 Sequenzdiagramm
- grober Zeitplan
- Testprotokoll (Zeitplanungsdingens)
- Präsentation _(reveal.js)_

## Termine

- 5.12.2019: Präsentation
- 19.12.2019: Projekt-/Dokumentationsabgabe

## Featuredefinitionen

### Raumsystem

- Neues Spiel erstellen
- Spiel beitreten
- Alle Spiele in Liste anzeigen
  - Filter: aktive Spiele zeigen, volle Spiele zeigen
- Raumname
- Raum UUID

### Register / Login

- Nutzername
- Passwort

### Chat

Chatten mit Gegner

#### Details:

- Chatlogs speichern
  - Nutzer
  - Nachricht
  - Timestamp
  - Zu Raum zuordbar

### Leaderboards

Ranglisten-Anzeige

#### Details:

- Nutzername
- Position
- Win/Lose/Tie
- Win/Lose%

### Match History

Letzte 10 Spiele einsehen

#### Details:

- Gegenspieler
- Sieg / Niederlage
