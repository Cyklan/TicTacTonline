using System;
using System.Collections.Generic;
using System.Linq;
using Models;
using Server.General;
using Server.Modules;

namespace Server.Communication
{
    /// <summary>
    /// Beantwortet alle einkommenden Requests
    /// </summary>
    public class RequestHandler
    {
        public List<Module> Modules { get; set; }
        private readonly Log log = new Log();
        private Converter converter = new Converter();

        public RequestHandler()
        {
            Modules = new List<Module>();
        }

        /// <summary>
        /// Überprüft, ob das angesprochene Modul und die aufgerufene Methode verfügbar sind.
        /// Sind sie verfügbar, wird die passende Methode ausgeführt.
        /// Sind sie nicht verfügbar, wird ein Fehler an den Client gesendet, der die Request ursprünglich sendete
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public byte[] HandleRequest(byte[] requestData, out List<User> targets, out User requestUser)
        {
            Response response = new Response();
            Request request = converter.ConvertJsonToObject<Request>(converter.ConvertBytesToString(requestData));
            request.Body = GetBody(requestData);

            log.Add($"Received {converter.ConvertObjectToJson(request)} from {request.Header.User}", request.Header.User, MessageType.Normal);

            Module module = Modules.FirstOrDefault(x => x.Name.ToLower() == request.Header.Identifier.Module.ToLower());
            if (module is null)
            {
                response = GetErrorResponse($"Failed to call function {request.Header.Identifier.Function} at unkown module {request.Header.Identifier.Module}", new List<User> { request.Header.User });
                log.Add($"Failed to call function {request.Header.Identifier.Function} at unkown module {request.Header.Identifier.Module}", request.Header.User, MessageType.Error);
            }
            else
            {
                try
                {
                    response = module.ProcessRequest(request);
                }
                catch (Exception ex)
                {
                    response = GetErrorResponse($"Failed to process request for function {request.Header.Identifier.Function} at module {request.Header.Identifier.Module}: {Environment.NewLine} {ex.ToString()}", new List<User> { request.Header.User });
                    log.Add($"Failed to process request for function {request.Header.Identifier.Function} at module {request.Header.Identifier.Module}: {Environment.NewLine} {ex.ToString()}", request.Header.User, MessageType.Error);
                }
            }

            response.Header.MessageNumber = request.Header.MessageNumber;
            targets = response.Header.Targets;
            requestUser = request.Header.User;

            log.Add($"Sending {converter.ConvertObjectToJson(response)} to {string.Join(",", response.Header.Targets.Select(x => x.ToString()))}", request.Header.User, MessageType.Normal);

            return converter.ConvertStringToBytes(converter.ConvertObjectToJson(response));
        }

        /// <summary>
        /// Erstellt Fehlernachrichten
        /// </summary>
        /// <param name="message"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        private Response GetErrorResponse(string message, List<User> targets)
        {
            ResponseHeader header = new ResponseHeader { Code = ResponseCode.UnplannedError, Message = message, Targets = targets };
            return new Response { Header = header, Body = new Document() };
        }

        /// <summary>
        /// Findet die passende Klasse für den Body
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private Document GetBody(byte[] data)
        {
            string json = converter.ConvertBytesToString(data);

            string bodyJson = json.Split(new string[] { "\"Body\": " }, StringSplitOptions.None)[1];
            bodyJson = bodyJson.Trim();
            bodyJson = bodyJson.Remove(bodyJson.Length - 1);
            string[] bodySplit = bodyJson.Split(',');

            string type = bodySplit[bodySplit.Length - 1];

            if (type.Contains("RemovePlayerFromRoomDocument"))
            {
                return converter.ConvertJsonToObject<RemovePlayerFromRoomDocument>(bodyJson);
            }
            else if (type.Contains("RoomDocument"))
            {
                return converter.ConvertJsonToObject<RoomDocument>(bodyJson);
            }
            else if (type.Contains("RoomsDocument"))
            {
                return converter.ConvertJsonToObject<RoomsDocument>(bodyJson);
            }
            else if (type.Contains("ChatDocument"))
            {
                return converter.ConvertJsonToObject<ChatDocument>(bodyJson);
            }
            else if (type.Contains("MatchHistoryDocument"))
            {
                return null;
            }
            else if (type.Contains("LeaderboardDocument"))
            {
                return null;
            }

            return new Document();
        }
    }
}
