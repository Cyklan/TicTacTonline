using Models;
using Server.Communication;
using Server.Database;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Modules
{
    /// <summary>
    /// Kümmert sich um alles Spielrelevante
    /// </summary>
    public class GameModule : Module
    {
        public GameModule() : base("GameModule") { }


        /// <summary>
        /// Startet ein Spiel und informiert beide Spieler darüber
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Function("StartGame")]
        public Response StartGame(Request request)
        {
            ResponseHeader header = new ResponseHeader();
            RoomDocument document = (RoomDocument)request.Body;
            header.Targets = new List<User> { document.Game.Player1, document.Game.Player2 };

            if (!db.ChangeRoomStatus(document.Id, RoomStatus.Ongoing))
            {
                header.Code = ResponseCode.PlannedError;
                header.Message = "Game could not be started";
                return new Response() { Header = header, Body = document };
            }

            StartNewGame(document.Game);

            header.Code = ResponseCode.GameStart;
            header.Message = $"Game started. It is {document.Game.CurrentPlayer.Name}'s turn";

            return new Response() { Header = header, Body = document };
        }

        /// <summary>
        /// Kümmert sich darum, dass ein Spielzug abgearbeitet wird.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Function("HandleTurn")]
        public Response HandleTurn(Request request)
        {
            ResponseHeader header = new ResponseHeader();
            RoomDocument document = (RoomDocument)request.Body;
            User winner;

            // Antwort an Spielpartner schicken
            header.Targets = new List<User> { document.Game.CurrentPlayer };

            // Wenn ein  Spieler das Spiel verlassen hat, hat der verbleibende Spieler gewonnen
            if (document.Game.Player1 is null || document.Game.Player2 is null)
            {
                if (document.Game.Player1 is null)
                {
                    document.Game.Player1 = document.Game.CurrentPlayer;
                    winner = document.Game.Player2;
                }
                else
                {
                    document.Game.Player2 = document.Game.CurrentPlayer;
                    winner = document.Game.Player2;
                }

            }
            else
            {
                winner = PlayRound(document.Game);
            }

            // Wenn kein gewinner ermittelt werden konnte geht das Spiel weiter
            if (winner == null)
            {
                header.Code = ResponseCode.GameTurnProcessed;
                header.Message = $"Turn {document.Game.RoundsPlayed} processed - no winner.";
                return new Response() { Header = header, Body = document };
            }

            // Wenn ein Gewinner ermittelt werden konnte wird der verbleibende Spieler zur Antwort hinzugefügt und es wir ermittelt,
            // ob es ein Unentschieden oder Sieg ist
            header.Targets.Add(header.Targets.Any(x => x.Name.ToLower() == document.Game.Player1.Name.ToLower()) ? document.Game.Player2 : document.Game.Player1);

            if (string.IsNullOrEmpty(winner.Name))
            {
                header.Code = ResponseCode.GameTie;
                header.Message = $"Turn {document.Game.RoundsPlayed} processed - tie.";
            }
            else
            {
                header.Code = ResponseCode.GameOver;
                header.Message = $"Turn {document.Game.RoundsPlayed} processed - {winner.Name} won.";
            }

            SaveGame(document, winner);

            return new Response() { Header = header, Body = document };
        }

        [Function("SendMessage")]
        public Response SendMessage(Request request)
        {
            ResponseHeader header = new ResponseHeader();
            ChatDocument body = (ChatDocument)request.Body;
            header.Targets = new List<User> { body.Target };
            header.Code = ResponseCode.Message;

            db.SaveMessage(request.Header.User.Name, body.Message, body.RoomId);

            return new Response() { Header = header, Body = body };
        }

        /// <summary>
        /// Wurde ein Spiel abgeschlossen, wird es und der Sieger des Spiels in der Datenbank gespeichert.
        /// Der Raum wird geschlossen.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="winner"></param>
        private void SaveGame(RoomDocument document, User winner)
        {
            db.ChangeRoomStatus(document.Id, RoomStatus.Closed);

            db.RemoveUserFromRoom(document.Game.Player1);
            db.RemoveUserFromRoom(document.Game.Player2);

            db.InsertMatch(document, winner);
        }

        /// <summary>
        /// Bereitet ein Spielfeld vor und entscheidet, welcher Spieler zuerst spielen darf.
        /// </summary>
        /// <param name="game"></param>
        private void StartNewGame(Game game)
        {
            game.Fields = new FieldStatus[3, 3];
            for (int x = 0; x < 3; x++)
                for (int y = 0; y < 3; y++)
                    game.Fields[x, y] = FieldStatus.Empty;
            game.RoundsPlayed = 0;

            if (new Random().Next(0, 2) == 1)
            {
                game.CurrentPlayer = game.Player1;
            }
            else
            {
                game.CurrentPlayer = game.Player2;
            }

            game.Turns = new List<Turn>();
        }

        /// <summary>
        /// Der aktuellste Spielzug wird verarbeitet und auf dem Feld eingetragen.
        /// Zusätzlich wird ermittelt, welcher Spieler das Spiel gewonnen hat.
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        private User PlayRound(Game game)
        {
            FieldStatus status = game.CurrentPlayer.Name.ToLower() == game.Player1.Name.ToLower() ? FieldStatus.Player2 : FieldStatus.Player1;
            game.RoundsPlayed++;
            UpdateField(game.Turns.Last().X, game.Turns.Last().Y, status, game);
            FieldStatus winner = CheckForWinner(game);
            if (winner == FieldStatus.Empty)
            {
                if (game.RoundsPlayed == 9) return new User();
                return null;
            }

            game.CurrentPlayer = game.CurrentPlayer == game.Player1 ? game.Player2 : game.Player1;
            if (winner == FieldStatus.Player1) return game.Player1;
            return game.Player2;
        }

        /// <summary>
        /// Das Spielfeld wird auf der passenden Koordinate angepasst.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="status"></param>
        /// <param name="game"></param>
        private void UpdateField(int x, int y, FieldStatus status, Game game) => game.Fields[x, y] = status;

        /// <summary>
        /// Algorithmus zum Überprüfen, ob ein spieler das Spiel gewonnen hat.
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        private FieldStatus CheckForWinner(Game game)
        {
            // Ein Tic Tac Toe Sieg ist erst nach 5 Runden möglich
            if (game.RoundsPlayed < 5) return FieldStatus.Empty;
            FieldStatus winner;

            // Reihen überprüfen
            // Links -> Rechts
            for (int y = 0; y < 3; y++)
            {
                winner = CompareRows(0, y, 1, 0, game);
                if (winner != FieldStatus.Empty) return winner;
            }

            // Zeilen überprüfen
            // Oben -> Unten
            for (int x = 0; x < 3; x++)
            {
                winner = CompareRows(x, 0, 0, 1, game);
                if (winner != FieldStatus.Empty) return winner;
            }

            // Diagonalen überprüfen
            // Oben links -> Unten rechts
            winner = CompareRows(0, 0, 1, 1, game);
            if (winner != FieldStatus.Empty) return winner;

            // Oben rechts -> Unten links
            winner = CompareRows(2, 0, -1, 1, game);

            return winner;

        }

        /// <summary>
        /// Vergleicht, ob eine Reihe die gleichen Felder enthält.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="dX"></param>
        /// <param name="dY"></param>
        /// <param name="game"></param>
        /// <returns></returns>
        private FieldStatus CompareRows(int x, int y, int dX, int dY, Game game)
        {
            FieldStatus first = game.Fields[y, x];
            if (first == FieldStatus.Empty) return FieldStatus.Empty;

            for (int i = 0; i < 3; i++)
            {
                int startX = x + dX * i;
                int startY = y + dY * i;
                if (game.Fields[startY, startX] != first) return FieldStatus.Empty;
            }

            return first;
        }
    }
}
