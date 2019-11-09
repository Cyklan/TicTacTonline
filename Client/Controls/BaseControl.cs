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

        protected MainWindow GetMain
        {
            get
            {
                return (MainWindow)Window.GetWindow(this);
            }
        }

        protected User User
        {
            get
            {
                return GetMain.User;
            }
            set
            {
                GetMain.User = value;
            }
        }

        protected WebsocketClient Client
        {
            get
            {
                return GetMain.Client;
            }
        }

        protected void ChangeControl(MainWindow.Controls control)
        {
            GetMain.ChangeControl(control);
        }

        protected Response Exchange(Request request)
        {
            CheckConnectionAndSwitchToConnectionScreen();
            return Client.Exchange(request);
        }

        protected void Send(Request request)
        {
            CheckConnectionAndSwitchToConnectionScreen();
            Client.Send(request);
        }

        protected void Abort()
        {
            throw new SilentException();
        }

        private void CheckConnectionAndSwitchToConnectionScreen()
        {
            if (!Client.IsConnected)
            {
                GetMain.ChangeControl(MainWindow.Controls.Connection);
                Abort();
            }

        }

        public abstract void HandleSpontaneousResponse(Response response);
    }
}
