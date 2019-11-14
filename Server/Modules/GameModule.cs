﻿using Models;
using Server.Database;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Modules
{
    public class GameModule : Module
    {
        public GameModule() : base("GameModule") { }

        private Response StartGame(Request request)
        {
            ResponseHeader header = new ResponseHeader();
            RoomDocument document = (RoomDocument)request.Body;
            header.Targets = new List<User> { document.Game.Player1, document.Game.Player2 };

            using DatabaseQueries db = new DatabaseQueries(request.Header.User);
            if (!db.ChangeRoomStatus(document.Id, RoomStatus.Ongoing))
            {
                header.Code = ResponseCode.PlannedError;
                header.Message = "Game could not be started";
                return new Response() { Header = header, Body = document };
            }

            StartNewGame(document.Game);
            db.ChangeRoomStatus(document.Id, RoomStatus.Ongoing);

            header.Code = ResponseCode.GameStart;
            header.Message = $"Game started. It is {document.Game.CurrentPlayer.Name}'s turn";

            return new Response() { Header = header, Body = document };
        }

        private Response HandleTurn(Request request)
        {
            ResponseHeader header = new ResponseHeader();
            RoomDocument document = (RoomDocument)request.Body;

            header.Targets = new List<User>
            {
                document.Game.Player1 == document.Game.CurrentPlayer ? document.Game.Player2 : document.Game.Player1
            };

            User winner = PlayRound(document.Game);

            if (winner == null)
            {
                header.Code = ResponseCode.GameTurnProcessed;
                header.Message = $"Turn {document.Game.RoundsPlayed} processed - no winner.";
            }
            else
            {
                header.Targets.Add(document.Game.Player1 == document.Game.CurrentPlayer ? document.Game.Player1 : document.Game.Player2);

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

                using DatabaseQueries db = new DatabaseQueries(request.Header.User);
                SaveGame(document, winner, db);
            }

            return new Response() { Header = header, Body = document };
        }

        private Response SendMessage(Request request)
        {
            ResponseHeader header = new ResponseHeader();
            ChatDocument body = (ChatDocument)request.Body;
            header.Targets = new List<User> { body.Target };
            header.Code = ResponseCode.Message;

            using DatabaseQueries db = new DatabaseQueries(request.Header.User);
            db.SaveMessage(request.Header.User.Name, body.Message, body.RoomId);

            return new Response() { Header = header, Body = body };
        }

        private void SaveGame(RoomDocument document, User winner, DatabaseQueries db)
        {
            db.ChangeRoomStatus(document.Id, RoomStatus.Closed);

            db.RemoveUserFromRoom(document.Game.Player1);
            db.RemoveUserFromRoom(document.Game.Player2);

            db.InsertMatch(document, winner);
        }

        private void StartNewGame(Game game)
        {
            game.Fields = new FieldStatus[3, 3];
            for (int x = 0; x < 3; x++)
                for (int y = 0; x < 3; x++)
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
        }

        private User PlayRound(Game game)
        {
            FieldStatus status = game.CurrentPlayer == game.Player1 ? FieldStatus.Player1 : FieldStatus.Player2;
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

        private void UpdateField(int x, int y, FieldStatus status, Game game) => game.Fields[x, y] = status;

        private FieldStatus CheckForWinner(Game game)
        {
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
