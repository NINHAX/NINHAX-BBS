using NHX.BBS.TS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHX.BBS.TS.Screen
{
    public class Menu : Screen
    {
        enum States
        {
            WatingConfirmation = 0,
        }

        States status;

        string text;
        public Menu(NVT nvt, TelnetServer server) : base(nvt, server, null)
        {
            text = File.Read("Welcome");
        }

        public override void Show()
        {
            Write(pointer.ClearScreen());
            Write(text);
            LnWrite("IP AND PORT: " + server.GetSocketByNVT(nvt).RemoteEndPoint);
            LnWrite("Client ID: " + nvt.NVTId);
            LnWrite("Screen: " + nvt.ScreenW + " x " + nvt.ScreenH);
            status = States.WatingConfirmation;
            LnWrite("press enter...");
        }

        public override void HandleMessage(string message)
        {
            switch (status)
            {
                case States.WatingConfirmation:
                    LnWrite("Ok...");
                    ShowNext();
                    break;
            }
        }

        public override void ShowNext()
        {
            //nvt.Screen = new Login(nvt, server);
            //nvt.Screen.Show();
        }
    }
}
