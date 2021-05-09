using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHX.BBS.TS.Services
{
    public static class TelnetIACHandler
    {
        public enum Command
        {
            SE = 240,
            NOP = 241,
            DataMark = 242,
            Break = 243,
            IP = 244,
            AO = 245,
            AYT = 246,
            EraseChar = 247,
            EraseLine = 248,
            GoAhead = 249,
            Subnegotiation = 250,
            WILL = 251,
            WONT = 252,
            DO = 253,
            DONT = 254,
            IAC = 255
        }

        public enum Option
        {
            BinaryTransmission = 0,
            Echo = 1,
            Reconnection = 2,
            SuppressGoAhead = 3,
            ApproxMessageSizeNegotiation = 4,
            Status = 5,
            TimingMark = 6,
            RemoteControlledTransAndEcho = 7,
            OutputLineWidth = 8,
            OutputPageSize = 9,
            OutputCarriageReturnDisposition = 10,
            OutputHorizontalTabStops = 11,
            OutputHorizontalTabDisposition = 12,
            OutputFormfeedDisposition = 13,
            OutputVerticalTabstops = 14,
            OutputVerticalTabDisposition = 15,
            OutputLinefeedDisposition = 16,
            ExtendedASCII = 17,
            Logout = 18,
            ByteMacro = 19,
            DataEntryTerminal = 20,
            SUPDUP = 21,
            SUPDUPOutput = 22,
            SendLocation = 23,
            TerminalType = 24,
            EndOfRecord = 25,
            TACACSUserIdentification = 26,
            OutputMarking = 27,
            TerminalLocationNumber = 28,
            Telnet3270Regime = 29,
            X3PAD = 30,
            NegotiateAboutWindowSize = 31,
            TerminalSpeed = 32,
            RemoteFlowControl = 33,
            Linemode = 34,
            XDisplayLocation = 35,
            EnvironmentOption = 36,
            AuthenticationOption = 37,
            EncryptionOption = 38,
            NewEnvironmentOption = 39,
            TN3270E = 40,
            XAUTH = 41,
            CHARSET = 42,
            TelnetRemoteSerialPort = 43,
            ComPortControlOption = 44,
            TelnetSuppressLocalEcho = 45,
            TelnetStartTLS = 46,
            KERMIT = 47,
            SEND_URL = 48,
            FORWARD_X = 49,
            TELOPT_PRAGMA_LOGON = 138,
            TELOPT_SSPI_LOGON = 139,
            TELOPT_PRAGMA_HEARTBEAT = 140,
            Extended_Options_List = 255,
        }

        public static byte[] Do(Option op) =>
            new byte[] { (byte)Command.IAC, (byte)Command.DO, (byte)op };

        public static byte[] Dont(Option op) =>
            new byte[] { (byte)Command.IAC, (byte)Command.DONT, (byte)op };

        public static byte[] Will(Option op) =>
            new byte[] { (byte)Command.IAC, (byte)Command.WILL, (byte)op };

        public static byte[] Wont(Option op) =>
            new byte[] { (byte)Command.IAC, (byte)Command.WONT, (byte)op };

        public static void WindowSize(byte[] data, int offset, NVT nvt)
        {
            if (data[offset + 1] == (byte)Command.Subnegotiation
                && data[offset + 2] == (byte)Option.NegotiateAboutWindowSize)
            {
                int w = data[offset + 3] * 256 + data[offset + 4];
                int h = data[offset + 5] * 256 + data[offset + 6];
                if (w != 0) nvt.ScreenW = w;
                if (h != 0) nvt.ScreenH = h;
            }
        }

        public static void Binary(byte[] data, int offset, NVT nvt)
        {
            if (data[offset + 1] == (byte)Command.DO
                && data[offset + 2] == (byte)Option.BinaryTransmission)
                nvt.BinaryMode = true;
        }
    }
}
