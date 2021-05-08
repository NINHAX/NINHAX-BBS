using NHX.BBS.TS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHX.BBS.TS.Screen
{
    class Screen
    {
        protected NVT nvt;
        protected TelnetServer server;
        protected Pointer pointer;
        public Screen parentScreen { get; set; }
        public Screen(NVT nvt, TelnetServer server) : this(nvt, server, null) { }
        public Screen(NVT nvt, TelnetServer server, Screen parent)
        {
            this.nvt = nvt;
            this.server = server;
            this.parentScreen = parent;
            this.pointer = new Pointer();
        }
        public virtual void Show() { }
        public virtual void HandleMessage(string msg) { }
        public virtual void HandleChar(byte[] data, int bytesReceived)
        {
            switch (data[0])
            {
                case 0x03:
                    ControlC();
                    break;
                case 0x09:
                    Tab();
                    break;
                case 0x1b:
                    if (bytesReceived == 1) ESC();
                    else if (bytesReceived == 3 && data[1] == '[')
                    {
                        switch (data[2])
                        {
                            case 65:
                                Up();
                                break;
                            case 66:
                                Down();
                                break;
                            case 67:
                                Right();
                                break;
                            case 68:
                                Left();
                                break;
                            default:
                                break;
                        }
                    }
                    else if (bytesReceived == 4 && data[1] == '[' && data[3] == 126)
                    {
                        switch (data[2])
                        {
                            case 53:
                                PageUp();
                                break;
                            case 54:
                                PageDown();
                                break;
                            case 49:
                                Home();
                                break;
                            case 52:
                                End();
                                break;
                            default:
                                break;
                        }
                    }
                    else if (bytesReceived == 3 && data[1] == 'O')
                    {
                        switch (data[2])
                        {
                            case 80:
                                F1();
                                break;
                            case 81:
                                F2();
                                break;
                            case 82:
                                F3();
                                break;
                            case 83:
                                F4();
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public virtual void ShowNext() { }
        protected virtual void ControlC() { }
        protected virtual void Tab() { }
        protected virtual void ESC() { }
        protected virtual void Up() { }
        protected virtual void Down() { }
        protected virtual void Left() { }
        protected virtual void Right() { }
        protected virtual void PageUp() { }
        protected virtual void PageDown() { }
        protected virtual void Home() { }
        protected virtual void End() { }
        protected virtual void F1() { }
        protected virtual void F2() { }
        protected virtual void F3() { }
        protected virtual void F4() { }
        protected void Write(string message) => server.SendMessageToNVT(nvt, message);
        protected void Writeln(string s) => Write(server + "\r\n");
        protected void Writeln() => Write("\r\n");
        protected void LnWrite(string s) => Write("\r\n" + s);
        protected void MoveTo(int row, int col) => Write(pointer.Move(col, row));
        protected void ClearLine(int row) => Write(pointer.Move(1, row) + pointer.ClearLine);
    }
}
