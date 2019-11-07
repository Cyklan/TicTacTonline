using Client.Communication;
using Client.General;
using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Client.Controls
{

    [TypeDescriptionProvider(typeof(AbstractControlDescriptionProvider<BaseControl, UserControl>))]
    public abstract class BaseControl : UserControl
    {
        public User currentUser { get; set; }
        protected WebsocketClient client { get; set; }

        public BaseControl(User user, WebsocketClient client)
        {
            currentUser = user;
            this.client = client;
        }

        protected MainWindow GetMain
        {
            get
            {
                return (MainWindow)Window.GetWindow(this);
            }
        }

        protected Response Exchange(Request request)
        {
            CheckConnection();
            return client.Exchange(request);
        }

        protected void Send(Request request)
        {
            CheckConnection();
            client.Send(request);
        }

        private void CheckConnection()
        {
            if (!client.IsConnected)
            {
                throw new Exception("Connection lost");
            }
        }

        public abstract void HandleSpontaneousResponse(Response response);
    }
}
