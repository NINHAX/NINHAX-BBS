using NHX.BBS.TS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHX.BBS.TS.Screen
{
    public class Login : Screen
    {
        enum States
        {
            WatingConfirmation = 0,
        }

        States status;
        public Login(NVT nvt, TelnetServer server) : base(nvt, server, null)
        {
        }

        public override void Show()
        {
            Write(pointer.ClearScreen());
        }

        public override void HandleMessage(string message)
        {
            switch (status)
            {
                case States.WatingConfirmation:
                    break;
            }
        }

        public override void ShowNext()
        {
            //nvt.screen = new Login(nvt, server);
            //nvt.screen.Show();
        }
    }
}
