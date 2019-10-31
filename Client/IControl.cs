using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client
{
    public interface IControl
    {
        public void HandleSpontaneousResponse(Response response);

    }
}
