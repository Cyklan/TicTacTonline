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
using System.Windows.Threading;

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

        protected RoomDocument Room
        {
            get
            {
                return GetMain.CurrentGame;
            }
            set
            {
                GetMain.CurrentGame = value;
            }
        }

        protected void ChangeControl(MainWindow.Controls control)
        {
            GetMain.ChangeControl(control);
        }

        protected Response Exchange(Document body, string module, string function)
        {
            return Exchange(new Request()
            {
                Header = new RequestHeader()
                {
                    User = User,
                    Identifier = new Identifier()
                    {
                        Module = module,
                        Function = function
                    }
                },
                Body = body
            });
        }

        protected Response Exchange(Request request)
        {
            CheckConnectionAndSwitchToConnectionScreen();
            return Client.Exchange(request);
        }

        protected void Send(Document body, string module, string function )
        {
            Send(new Request()
            {
                Header = new RequestHeader()
                {
                    User = User,
                    Identifier = new Identifier()
                    {
                        Module = module,
                        Function = function
                    }
                },
                Body = body
            });
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

        /// <summary>
        /// Application.DoEvents gibts nicht in WPF, daher so -> Fenster kann noch bewegt, geschlossen, ... werden, während auf Verbindung gewartet wird.
        /// </summary>
        protected void DoEvents()
        {
            try
            {
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
            }
            catch
            {
                // Wenn die Form geschlossen wird, wird manchmal eine NullReference Exception geworfen
            }

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
