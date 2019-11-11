using Models;
using System;
using System.Collections.Generic;
using System.Text;
using Server.Database;
using System.Linq;

namespace Server.Modules
{

    public class RoomModule : Module
    {
        public RoomModule() : base("roomModule") { }

        private Response GetRooms(Request request)
        {
            ResponseHeader header = new ResponseHeader() { Targets = { request.Header.User } };
            RoomsDocument body = new RoomsDocument();

            using DatabaseQueries db = new DatabaseQueries(request.Header.User);
            body.Rooms = db.GetRooms();

            header.Code = ResponseCode.Ok;
            header.Message = $"Found '{body.Rooms.Count}' rooms";

            return new Response() { Header = header, Body = body };
        }

        private Response CreateRoom(Request request)
        {
            ResponseHeader header = new ResponseHeader() { Targets = { request.Header.User } };
            RoomDocument body = (RoomDocument)request.Body;
            body.Game = new Game() { Player1 = request.Header.User };

            using DatabaseQueries db = new DatabaseQueries(request.Header.User);
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

        private Response JoinRoom(Request request)
        {
            ResponseHeader header = new ResponseHeader();
            RoomDocument body = (RoomDocument)request.Body;

            header.Targets = new List<User>
            {
                request.Header.User
            };

            using DatabaseQueries db = new DatabaseQueries(request.Header.User);

            if (db.GetRooms().Where(x => x.Id == body.Id).Count() > 2)
            {
                header.Code = ResponseCode.PlannedError;
                header.Message = $"The room is full";

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

            if (!(body.Game.Player1 is null || body.Game.Player2 is null))
            {
                db.ChangeRoomStatus(body.Id, RoomStatus.Full);
                header.Message += Environment.NewLine + "Room full";
            }

            return new Response() { Header = header, Body = body };
        }

        private Response LeaveRoom(Request request)
        {
            ResponseHeader header = new ResponseHeader();
            RoomDocument body = (RoomDocument)request.Body;

            header.Targets = new List<User>
            {
                request.Header.User
            };

            using DatabaseQueries db = new DatabaseQueries(request.Header.User);
            if (!db.RemoveUserFromRoom(request.Header.User))
            {
                header.Code = ResponseCode.PlannedError;
                header.Message = $"Player {request.Header.User.Name} could not be removed from the room";
                return new Response() { Header = header, Body = body };
            }

            header.Code = ResponseCode.LeftRoom;
            header.Message = $"Player {request.Header.User.Name} left the room.";

            if (body.Game.Player1 is null || body.Game.Player2 is null)
            {
                db.ChangeRoomStatus(body.Id, RoomStatus.Closed);
                header.Message += Environment.NewLine + "Room deleted";
                return new Response() { Header = header, Body = body };
            }
            else
            {
                db.ChangeRoomStatus(body.Id, RoomStatus.Open);
                header.Message += Environment.NewLine + "Room open";
            }

            if (body.Game.Player1.Name == request.Header.User.Name)
            {
                body.Game.Player1 = null;
                header.Targets.Add(body.Game.Player2);
            }
            else if (body.Game.Player2.Name == request.Header.User.Name)
            {
                body.Game.Player2 = null;
                header.Targets.Add(body.Game.Player1);
            }
            else
            {
                throw new Exception($"The user {request.Header.User} has not joined the room");
            }

            return new Response() { Header = header, Body = body };
        }

    }
}
