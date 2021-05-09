using NHX.BBS.Logic.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NHX.BBS.TS.Services
{
    public class NVT
    {
        public enum Status
        {
            Guest = 0,
            LoggedIn = 1
        }
        public enum Mode
        {
            LineMode,
            CharMode,
            PasswordMode
        }
        public Guid NVTId { get; private set; }
        public IPEndPoint RemoteEndPoint { get; private set; }
        public Screen.Screen Screen { get; set; }
        public DateTime ConnectedAt { get; set; }
        public DateTime LastActionAt { get; set; }
        public int ScreenW { get; set; }
        public int ScreenH { get; set; }
        public bool BinaryMode { get; set; }
        public Status State { get; set; }
        public Mode CurrentMode { get; set; }
        public User User { get; set; }
        public string InputData { get; set; }

        public NVT(Guid nvtId, IPEndPoint remoteEndPoint)
        {
            this.NVTId = nvtId;
            this.RemoteEndPoint = remoteEndPoint;
            ConnectedAt = DateTime.Now;
            State = Status.Guest;
            CurrentMode = Mode.LineMode;
            LastActionAt = DateTime.Now;
            ScreenW = 80;
            ScreenH = 24;
            BinaryMode = false;
        }

        public void ResetInputData()
        {
            this.InputData = string.Empty;
        }

        public void AppendInputData(string data)
        {
            this.InputData += data;
        }

        public void RemoveLastChar()
        {
            this.InputData = this.InputData[0..^1];
        }
    }
}
