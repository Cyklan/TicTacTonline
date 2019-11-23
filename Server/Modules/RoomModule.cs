using Models;
using System;
using System.Collections.Generic;
using System.Text;
using Server.Database;
using System.Linq;
using Server.Communication;

namespace Server.Modules
{
    /// <summary>
    /// Kümmert sich um alle Räume
    /// </summary>
    public class RoomModule : Module
    {
        public RoomModule() : base("RoomModule") { }

        /// <summary>
        /// Gibt alle Räume zurück, die nicht bereits geschlossen wurden
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Function("GetRooms")]
        public Response GetRooms(Request request)
        {
            ResponseHeader header = new ResponseHeader() { Targets = new List<User> { request.Header.User } };
            RoomsDocument body = new RoomsDocument
            {
                Rooms = db.GetRooms()
            };

            header.Code = ResponseCode.Ok;
            header.Message = $"Found '{body.Rooms.Count}' rooms";

            return new Response() { Header = header, Body = body };
        }

        /// <summary>
        /// Erstellt einen Raum, sodass andere Nutzer ihm beitreten können
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Function("CreateRoom")]
        public Response CreateRoom(Request request)
        {
            ResponseHeader header = new ResponseHeader() { Targets = new List<User> { request.Header.User } };
            RoomDocument body = (RoomDocument)request.Body;
            body.Game = new Game() { Player1 = request.Header.User };

            body.Id = db.CreateNewGame(body.Name, body.Password);

            if (body.Id < 0)
            {
                header.Code = ResponseCode.PlannedError;
                header.Message = "Room could not be created";
            }
            else
            {
                if (db.AddUserToRoom(request.Header.User, body.Id))
                {
                    header.Code = ResponseCode.Ok;
                    header.Message = "Room created successfully";
                }
                else
                {
                    header.Code = ResponseCode.PlannedError;
                    header.Message = "User could not be added to game";

                    db.ChangeRoomStatus(body.Id, RoomStatus.Closed);
                }
            }

            return new Response() { Header = header, Body = body };
        }

        /// <summary>
        /// Lässt einen Nutzer einem Raum beitreten
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Function("JoinRoom")]
        public Response JoinRoom(Request request)
        {
            ResponseHeader header = new ResponseHeader();
            RoomDocument body = (RoomDocument)request.Body;
            List<RoomDocument> rooms;

            header.Targets = new List<User>
            {
                request.Header.User
            };

            rooms = db.GetRooms().Where(x => x.Id == body.Id).ToList();
            
            // Wenn es keine Räume zu der Raum Id gibt
            if (rooms.Count() == 0)
            {
                header.Code = ResponseCode.PlannedError;
                header.Message = $"The room does not exist";

                return new Response() { Header = header, Body = body };
            }

            // Wenn es mehr als einen Raum zu der Id gibt
            if (rooms.Count() > 2)
            {
                header.Code = ResponseCode.PlannedError;
                header.Message = $"The room is full";

                return new Response() { Header = header, Body = body };
            }

            // Wenn der Raum nicht offen ist
            if (rooms.First(x => x.Id == body.Id).RoomStatus != RoomStatus.Open)
            {
                header.Code = ResponseCode.PlannedError;
                header.Message = $"The room is not open";

                return new Response() { Header = header, Body = body };
            }

            if (!db.AddUserToRoom(request.Header.User, body.Id))
            {
                header.Code = ResponseCode.PlannedError;
                header.Message = $"Player {request.Header.User.Name} could not join the room";
                return new Response() { Header = header, Body = body };
            }

            header.Code = ResponseCode.JoinedRoom;
            header.Message = $"Player {request.Header.User.Name} joined the room";
            
            // Den Spieler im Game-Objekt füllen, abhängig davon, welcher noch nicht gefüllt ist
            if (body.Game.Player1 is null)
            {
                body.Game.Player1 = request.Header.User;
                header.Targets.Add(body.Game.Player2);
            }
            else
            {
                body.Game.Player2 = request.Header.User;
                header.Targets.Add(body.Game.Player1);
            }

            // Wenn jetzt zwei Spieler im Raum sind wird dieser auf "Full" gesetzt
            if (!(body.Game.Player1 is null || body.Game.Player2 is null))
            {
                db.ChangeRoomStatus(body.Id, RoomStatus.Full);
                header.Message += Environment.NewLine + "Room full";
            }

            return new Response() { Header = header, Body = body };
        }

        /// <summary>
        /// Lässt einen Nutzer einen Raum verlassen
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Function("LeaveRoom")]
        public Response LeaveRoom(Request request)
        {
            ResponseHeader header = new ResponseHeader();
            RemovePlayerFromRoomDocument body = (RemovePlayerFromRoomDocument)request.Body;
            bool getTargetFromRoom = false;

            header.Targets = new List<User>
            {
                request.Header.User
            };

            if (!db.RemoveUserFromRoom(body.PlayerToRemove))
            {
                header.Code = ResponseCode.PlannedError;
                header.Message = $"Player {body.PlayerToRemove} could not be removed from the room";
                return new Response() { Header = header, Body = body };
            }

            header.Code = ResponseCode.LeftRoom;
            header.Message = $"Player {body.PlayerToRemove} left the room.";

            // Wenn der Raum jetzt leer ist, wird er gelöscht, ansonsten wieder offen
            if (body.Room.Game.Player1 is null || body.Room.Game.Player2 is null)
            {
                db.ChangeRoomStatus(body.Room.Id, RoomStatus.Closed);
                header.Message += Environment.NewLine + "Room deleted";
                return new Response() { Header = header, Body = body };
            }
            else
            {
                db.ChangeRoomStatus(body.Room.Id, RoomStatus.Open);
                header.Message += Environment.NewLine + "Room open";
            }

            // Wenn der Sender der Anfrage aus dem Raum entfernt werden soll
            // muss dieser aus dem Raum ermittelt werden, warum habe ich vergessen
            if(request.Header.User.Name.ToLower() != body.PlayerToRemove.Name.ToLower())
            {
                header.Targets.Add(body.PlayerToRemove);
            }
            else
            {
                getTargetFromRoom = true;
            }

            if (body.Room.Game.Player1.Name.ToLower() == body.PlayerToRemove.Name.ToLower())
            {
                body.Room.Game.Player1 = null;
                if (getTargetFromRoom) header.Targets.Add(body.Room.Game.Player2);
            }
            else if (body.Room.Game.Player2.Name.ToLower() == body.PlayerToRemove.Name.ToLower())
            {
                body.Room.Game.Player2 = null;
                if (getTargetFromRoom) header.Targets.Add(body.Room.Game.Player1);
            }
            else
            {
                throw new Exception($"The user {body.PlayerToRemove} has not joined the room");
            }

            return new Response() { Header = header, Body = body };
        }

    }
}
