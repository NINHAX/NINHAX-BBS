using NHX.BBS.TS.Services;

namespace NHX.BBS.TS.Screen
{
    internal class NewPost : Screen
    {
        enum States
        {
            WatingConfirmation = 0,
            WatingFile = 1
        }

        States status;
        string title;
        public NewPost(NVT nvt, TelnetServer server, Screen parent) : base(nvt, server, parent)
        {
        }

        public override void Show()
        {
            Write(pointer.ClearScreen());
            LnWrite("Please insert the name of the text file and press enter");
            status = States.WatingConfirmation;
        }

        public override void HandleMessage(string message)
        {
            switch (status)
            {
                case States.WatingConfirmation:
                    LnWrite("Insert the text...");
                    ShowNext();
                    break;
                case States.WatingFile:
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