using System;
using System.Collections.Generic;
using System.Linq;
using Models;
using Server.General;
using Server.Modules;

namespace Server.Communication
{
    public class RequestHandler
    {
        public static List<Module> Modules = new List<Module>();

        public Response HandleRequest(byte[] request)
        {
            Request requestObject = Converter.ConvertJsonToObject<Request>(Converter.ConvertBytesToString(request));
            Response responseObject = new Response();
            Log.Add($"Received {Converter.ConvertObjectToJson(requestObject)} from {requestObject.Header.User}", requestObject.Header.User, MessageType.Normal);

            Module module = Modules.FirstOrDefault(x => x.Name == requestObject.Header.Identifier.Module);
            if (module is null)
            {
                responseObject = GetErrorResponse($"Failed to call function {requestObject.Header.Identifier.Function} at unkown module {requestObject.Header.Identifier.Module}", new List<User>{requestObject.Header.User});
            }
            else
            {
                try
                {
                    responseObject = module.ProcessRequest(requestObject);
                }
                catch (Exception ex)
                {
                    responseObject = GetErrorResponse($"Failed to process request for function {requestObject.Header.Identifier.Function} at module {requestObject.Header.Identifier.Module}: {Environment.NewLine} {ex}",new List<User> { requestObject.Header.User });
                }
            }

            Log.Add($"Sending {Converter.ConvertObjectToJson(responseObject)} to {string.Join(",", responseObject.Header.Targets.Select(x => x.ToString()))}", requestObject.Header.User, MessageType.Normal);
            return responseObject;
        }

        private Response GetErrorResponse(string message, List<User> targets)
        {
            ResponseHeader header = new ResponseHeader { Code = ResponseCode.UnplannedError, Message = message, Targets = targets };
            return new Response { Header = header, Body = new Document() };
        }
    }
}
