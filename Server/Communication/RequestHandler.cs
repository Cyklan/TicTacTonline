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
        private CommunicationConverter converter = new CommunicationConverter();
        private List<Module> Modules = new List<Module>();

        public RequestHandler()
        {
            Modules.Add(new LoginModule());
        }

        public CommunicationWrapper HandleRequest(byte[] request)
        {
            CommunicationWrapper communicationWrapper = new CommunicationWrapper();
            communicationWrapper.RequestData = request;
            communicationWrapper.Request = converter.ConvertJsonToObject<Request>(converter.ConvertBytesToString(request));
            communicationWrapper.Response = new Response();

            Log.Add($"Received {converter.ConvertObjectToJson(communicationWrapper.Request)} from {communicationWrapper.Request.Header.User}", communicationWrapper.Request.Header.User, MessageType.Normal);

            Module module = Modules.FirstOrDefault(x => x.Name == communicationWrapper.Request.Header.Identifier.Module);
            if (module is null)
            {
                communicationWrapper.Response = GetErrorResponse($"Failed to call function {communicationWrapper.Request.Header.Identifier.Function} at unkown module {communicationWrapper.Request.Header.Identifier.Module}");
                if (!communicationWrapper.Targets.Contains(communicationWrapper.Request.Header.User)) communicationWrapper.Targets.Add(communicationWrapper.Request.Header.User);
            }
            else
            {
                try
                {
                    communicationWrapper = module.ProcessRequest(communicationWrapper);
                }
                catch (Exception ex)
                {
                    communicationWrapper.Response = GetErrorResponse($"Failed to process request for function {communicationWrapper.Request.Header.Identifier.Function} at module {communicationWrapper.Request.Header.Identifier.Module}: {Environment.NewLine} {ex.ToString()}");
                    if (!communicationWrapper.Targets.Contains(communicationWrapper.Request.Header.User)) communicationWrapper.Targets.Add(communicationWrapper.Request.Header.User);
                }
            }

            communicationWrapper.ResponseData = converter.ConvertStringToBytes(converter.ConvertObjectToJson(communicationWrapper.Response));

            Log.Add($"Sending {converter.ConvertObjectToJson(communicationWrapper.Response)} to {string.Join(",", communicationWrapper.Targets.Select(x => x.ToString()))}", communicationWrapper.Request.Header.User, MessageType.Normal);
            return communicationWrapper;
        }

        private Response GetErrorResponse(string message)
        {
            ResponseHeader header = new ResponseHeader() { Code = 1, Message = message };
            Log.Add(message);
            return new Response() { Header = header, Body = new Body() };
        }
    }
}
