using Models;
using System;
using System.Collections.Generic;
using System.Text;
using Server.Database;

namespace Server.Modules
{
    public enum RoomStaus
    {
        Open,
        Full,
        Ongoing,
        Closed
    }


    public class RoomModule : Module
    {
        public RoomModule() : base("roomModule") { }

        private Response GetRooms(Request request)
        {

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
                if(db.AddUserToGame(request.Header.User, body.Id))
                {
                    header.Code = ResponseCode.Ok;
                    header.Message = "User could not be added to game";
                }
                else
                {
                    header.Code = ResponseCode.PlannedError;
                    header.Message = "User could not be added to game";
                }
            }

            return new Response() { Header = header, Body = body };
        }

        private Response JoinRoom(Request request)
        {

        }

        private Response LeaveRoom(Request request)
        {

        }

        private void DeleteRoom(RoomDocument room)
        {

        }

    }
}
