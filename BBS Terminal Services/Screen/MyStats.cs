using NHX.BBS.TS.Services;
using System.Threading.Tasks;

namespace NHX.BBS.TS.Screen
{
    internal class MyStats : Screen
    {
        enum States
        {
            WatingConfirmation = 0,
        }

        States status;

        string text;
        public MyStats(NVT nvt, TelnetServer server, Screen parent) : base(nvt, server, parent)
        {
            text = File.Read("MyStats");
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
            nvt.Screen = parentScreen;
            nvt.Screen.Show();
        }
    }
}