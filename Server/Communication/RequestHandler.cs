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
        public List<Module> Modules { get; set; }
        private readonly Log log = new Log();
        private Converter converter = new Converter();

        public RequestHandler()
        {
            Modules = new List<Module>();
        }

        public Response HandleRequest(Request request)
        {
            Response response = new Response();

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
            return response;
        }

        private Response GetErrorResponse(string message, List<User> targets)
        {
            ResponseHeader header = new ResponseHeader { Code = ResponseCode.UnplannedError, Message = message, Targets = targets };
            return new Response { Header = header, Body = new Document() };
        }

    }
}
