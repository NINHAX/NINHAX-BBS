using NHX.BBS.TS.Services;
using System.Threading.Tasks;

namespace NHX.BBS.TS.Screen
{
    internal class NewThread : Screen
    {
        public NewThread(NVT nvt, TelnetServer server, Screen parent) : base(nvt, server, parent)
        {
        }

        enum States
        {
            WatingCommand = 0,
        }

        States status;

        string text;

        public override void Show()
        {
            Write(pointer.ClearScreen());
            Write(text);
            LnWrite("Thread: ");
            LnWrite("\t NewThread");
            LnWrite("\t ViewThreads");
            LnWrite("User: ");
            LnWrite("\t MyStats");
            LnWrite("\t Exit");
            status = States.WatingCommand;
            if (nvt.State == NVT.Status.Guest)
            {
                LnWrite("Guest>");
            }
            else
            {
                LnWrite(nvt.User.Email + ">");
            }
        }

        public override void HandleMessage(string message)
        {
            switch (nvt.State)
            {
                case NVT.Status.LoggedIn:
                    switch (status)
                    {
                        case States.WatingCommand:
                            switch (message.ToLower())
                            {
                                case "newthread":
                                    nvt.Screen = new NewThread(nvt, server, this);
                                    nvt.Screen.Show();
                                    break;
                                case "viewthreads":
                                    nvt.Screen = new ViewThread(nvt, server, this);
                                    nvt.Screen.Show();
                                    break;
                                case "mystats":
                                    nvt.Screen = new MyStats(nvt, server, this);
                                    nvt.Screen.Show();
                                    break;
                                case "exit":
                                    server.CloseSocket(server.GetSocketByNVT(nvt));
                                    break;
                                default:
                                    Show();
                                    break;
                            }
                            break;
                    }
                    break;
                default:
                    switch (status)
                    {
                        case States.WatingCommand:
                            switch (message.ToLower())
                            {
                                case "newthread":
                                    LnWrite("You are a Guest...");
                                    Task.Delay(500);
                                    nvt.Screen.Show();
                                    break;
                                case "viewthreads":
                                    nvt.Screen = new ViewThread(nvt, server, this);
                                    nvt.Screen.Show();
                                    break;
                                case "mystats":
                                    LnWrite("You are a Guest...");
                                    Task.Delay(500);
                                    nvt.Screen.Show();
                                    break;
                                case "exit":
                                    server.CloseSocket(server.GetSocketByNVT(nvt));
                                    break;
                                default:
                                    Show();
                                    break;
                            }
                            break;
                    }
                    break;
            }
        }
    }
}