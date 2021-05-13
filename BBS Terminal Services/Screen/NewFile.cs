using NHX.BBS.TS.Services;

namespace NHX.BBS.TS.Screen
{
    internal class NewFile : Screen
    {
        enum States
        {
            WatingConfirmation = 0,
            WatingFile = 1
        }

        States status;

        string text;
        string file;
        public NewFile(NVT nvt, TelnetServer server, Screen parent) : base(nvt, server, parent)
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
                    file = message;
                    LnWrite("Insert the text...");
                    ShowNext();
                    break;
                case States.WatingFile:
                    File.New(message, file);
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