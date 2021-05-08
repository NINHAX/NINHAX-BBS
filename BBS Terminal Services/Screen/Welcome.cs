using NHX.BBS.TS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHX.BBS.TS.Screen
{
    public class Welcome : Screen
    {
        string text;
        public Welcome(NVT nvt, TelnetServer server) : base(nvt, server, null)
        {
            text = File.Read("Welcome");
        }

        public override void Show()
        {
            pointer.ClearScreen();
            Write(text);
            LnWrite("IP AND PORT: " + server.GetSocketByNVT(nvt).RemoteEndPoint);
            LnWrite("Client ID: " + nvt.nvtId);
            LnWrite("Screen: " + nvt.screenW + " x " + nvt.screenH);

        }
    }
}
