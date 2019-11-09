using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.General
{
    /// <summary>
    /// Diese Exception wird vom Exception handler nicht in einer MsgBox angezeigt.
    /// </summary>
    public class SilentException : Exception
    {
        public SilentException()
        {

        }
    }
}
