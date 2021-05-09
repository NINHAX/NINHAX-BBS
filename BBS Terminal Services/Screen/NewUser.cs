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
    public class NewUser : Screen
    {
        enum States
        {
            WatingEmail = 0,
            WatingPassword = 1,
            WatingDisplayName = 2,
        }

        States status;
        Regex emailValidator = new(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
        string text;

        string email;
        string password;
        string displayName;
        public NewUser(NVT nvt, TelnetServer server, Screen parent) : base(nvt, server, parent)
        {
            text = File.Read("NewUser");
        }

        public override void Show()
        {
            Write(pointer.ClearScreen());
            Write(text);
            status = States.WatingEmail;
            LnWrite("Email: ");
        }

        public override void HandleMessage(string message)
        {
            switch (status)
            {
                case States.WatingEmail:
                    if (emailValidator.IsMatch(message))
                    {
                        email = message;
                        status = States.WatingPassword;
                        LnWrite("Password: ");
                    }
                    if (!emailValidator.IsMatch(message))
                    {
                        LnWrite("Please input valid email: ");
                    }
                    break;
                case States.WatingPassword:
                    password = message;
                    status = States.WatingDisplayName;
                    LnWrite("Please input your display name: ");
                    break;
                case States.WatingDisplayName:
                    displayName = message;
                    LnWrite("Creating user...");
                    try
                    {
                        UserLogic.CreateUser(email, password, displayName);
                        LnWrite("User created...");
                        nvt.Screen = parentScreen;
                        nvt.Screen.Show();
                    }
                    catch
                    {
                        LnWrite("Creating a new user is not available...");
                        nvt.Screen = parentScreen;
                        nvt.Screen.Show();
                    }
                    break;
            }
        }
    }
}
