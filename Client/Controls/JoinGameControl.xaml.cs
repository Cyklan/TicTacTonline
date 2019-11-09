using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Client.Communication;

namespace Client.Controls
{
    /// <summary>
    /// Interaction logic for JoinGameControl.xaml
    /// </summary>
    public partial class JoinGameControl : BaseControl
    {
        public JoinGameControl()
        {
            InitializeComponent();
        }

        private void btJoinGame_Click(object sender, RoutedEventArgs e)
        {

        }

        public override void HandleSpontaneousResponse(Response response)
        {
            throw new NotImplementedException();
        }
    }
}
