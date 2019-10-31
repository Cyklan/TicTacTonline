using Models;
using System;
using System.Collections.Generic;
using System.Text;
using Server.Database;

namespace Server.Modules
{
    public enum RoomStatus
    {
        Open,
        Full,
        Ongoing,
        Closed
    }


    public class RoomModule : Module
    {
        public RoomModule() : base("roomModule") { }

        // Only returns rooms with unfinished games
        private Response GetRooms(Request request)
        {
            return new Response();
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
                if (db.AddUserToGame(request.Header.User, body.Id))
                {
                    header.Code = ResponseCode.Ok;
                    header.Message = "Room created successfully";
                }
                else
                {
                    header.Code = ResponseCode.PlannedError;
                    header.Message = "User could not be added to game";

                    if(DeleteRoom(db, body))
                    {
                        header.Message += Environment.NewLine + "Room deleted";
                    }
                    else
                    {
                        header.Message += Environment.NewLine + "Room could not be deleted";
                    }
                }
            }

            return new Response() { Header = header, Body = body };
        }

        private Response JoinRoom(Request request)
        {
            return new Response();
        }

        private Response LeaveRoom(Request request)
        {
            return new Response();
        }

        private bool DeleteRoom(DatabaseQueries db, RoomDocument room)
        {
            return db.ChangeRoomStatus(room.Id, RoomStatus.Closed);
        }

    }
}
