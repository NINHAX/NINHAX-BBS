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
            Authenticating = 1,
            LoggedIn = 2
        }
        public enum Mode
        {
            LineMode,
            CharMode,
            PasswordMode
        }
        public Guid nvtId { get; private set; }
        public IPEndPoint remoteEndPoint { get; private set; }
        private DateTime connectedAt { get; set; }
        public DateTime lastActionAt { get; set; }
        public int screenW { get; set; }
        public int screenH { get; set; }
        public bool BinaryMode { get; set; }
        public Status status { get; set; }
        public Mode mode { get; set; }
        private Guid user { get; set; }
        public string inputData { get; set; }

        public NVT(Guid nvtId, IPEndPoint remoteEndPoint)
        {
            this.nvtId = nvtId;
            this.remoteEndPoint = remoteEndPoint;
            connectedAt = DateTime.Now;
            status = Status.Guest;
            mode = Mode.LineMode;
            user = Guid.Empty;
            lastActionAt = DateTime.Now;
            screenW = 80;
            screenH = 24;
            BinaryMode = false;
        }

        public void ResetInputData()
        {
            this.inputData = string.Empty;
        }

        public void AppendInputData(string data)
        {
            this.inputData += data;
        }

        public void RemoveLastChar()
        {
            this.inputData = this.inputData.Substring(0, this.inputData.Length - 1);
        }
    }
}
