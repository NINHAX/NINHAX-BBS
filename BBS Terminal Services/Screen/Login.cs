using NHX.BBS.Logic;
using NHX.BBS.TS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NHX.BBS.TS.Screen
{
    public class Login : Screen
    {
        enum States
        {
            WatingOption = 0,
            WatingEmail = 1,
            WatingPassword = 2,
        }
        string text;
        States status;
        Regex emailValidator = new(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
        string email;
        string password;
        public Login(NVT nvt, TelnetServer server) : base(nvt, server, null)
        {
            text = File.Read("Login");
        }

        public override void Show()
        {
            Writeln();
            Write(text);
            status = States.WatingOption;
            LnWrite("Please select a valid option [Guest, Login, New]: ");
        }

        public override void HandleMessage(string message)
        {
            switch (status)
            {
                case States.WatingOption:
                    string option = message.ToLower();
                    switch (option)
                    {
                        case "guest":
                            Writeln("Welcome Guest...");
                            ShowNext();
                            break;
                        case "login":
                            status = States.WatingEmail;
                            LnWrite("Please input valid email: ");
                            break;
                        case "new":
                            nvt.Screen = new NewUser(nvt, server, this);
                            nvt.Screen.Show();
                            break;
                        default:
                            LnWrite("Please select a valid option [Guest, Login, New]: ");
                            break;
                    }
                    break;
                case States.WatingEmail:
                    if (emailValidator.IsMatch(message))
                    {
                        status = States.WatingPassword;
                        email = message;
                        LnWrite("Password: ");
                        nvt.CurrentMode = NVT.Mode.PasswordMode;
                    }
                    if (!emailValidator.IsMatch(message))
                    {
                        LnWrite("Please input valid email: ");
                    }
                    break;
                case States.WatingPassword:
                    password = message;
                    try
                    {
                        nvt.User = UserLogic.Auth(email, password);
                        nvt.State = NVT.Status.LoggedIn;
                        nvt.CurrentMode = NVT.Mode.LineMode;
                        ShowNext();
                    }
                    catch
                    {
                        LnWrite("Email or User failed...");
                        status = States.WatingOption;
                        nvt.CurrentMode = NVT.Mode.LineMode;
                        LnWrite("Please select a valid option [Guest, Login, New]: ");
                    }
                    break;
            }
        }

        public override void ShowNext()
        {
            nvt.Screen = new Menu(nvt, server);
            nvt.Screen.Show();
        }
    }
}
