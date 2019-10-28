using System;
using System.Collections.Generic;
using System.Linq;
using Server.Modules;
using Models;
using Server.General;

namespace Server.Communication
{
    public class RequestHandler
    {
        public static List<Module> Modules = new List<Module>();

        public Response HandleRequest(byte[] request, string ipPort)
        {
            Request requestObject = Converter.ConvertJsonToObject<Request>(Converter.ConvertBytesToString(request));
            requestObject.Header.User.IpPort = ipPort;
            Response responseObject = new Response();
            Log.Add($"Received {Converter.ConvertObjectToJson(requestObject)} from {requestObject.Header.User}", requestObject.Header.User, MessageType.Normal);

            Module module = Modules.FirstOrDefault(x => x.Name.ToLower() == requestObject.Header.Identifier.Module.ToLower());
            if (module is null)
            {
                responseObject = GetErrorResponse($"Failed to call function {requestObject.Header.Identifier.Function} at unkown module {requestObject.Header.Identifier.Module}", new List<User> { requestObject.Header.User });
                Log.Add($"Failed to call function {requestObject.Header.Identifier.Function} at unkown module {requestObject.Header.Identifier.Module}", requestObject.Header.User, MessageType.Error);
            }
            else
            {
                try
                {
                    responseObject = module.ProcessRequest(requestObject);
                }
                catch (Exception ex)
                {
                    responseObject = GetErrorResponse($"Failed to process request for function {requestObject.Header.Identifier.Function} at module {requestObject.Header.Identifier.Module}: {Environment.NewLine} {ex.ToString()}", new List<User> { requestObject.Header.User });
                    Log.Add($"Failed to process request for function {requestObject.Header.Identifier.Function} at module {requestObject.Header.Identifier.Module}: {Environment.NewLine} {ex.ToString()}", requestObject.Header.User, MessageType.Error);
                }
            }

            responseObject.Header.MessageNumber = requestObject.Header.MessageNumber;
            Log.Add($"Sending {Converter.ConvertObjectToJson(responseObject)} to {string.Join(",", responseObject.Header.Targets.Select(x => x.ToString()))}", requestObject.Header.User, MessageType.Normal);
            return responseObject;
        }

        private Response GetErrorResponse(string message, List<User> targets)
        {
            ResponseHeader header = new ResponseHeader() { Code = ResposneCode.UnplannedError, Message = message, Targets = targets };
            return new Response() { Header = header, Body = new Document() };
        }
    }
}
