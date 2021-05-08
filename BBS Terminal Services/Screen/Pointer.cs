using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHX.BBS.TS.Screen
{
    class Pointer
    {
        public string ClearScreen() => "\u001b[2J" + Start();
        public string Move(int col, int row) => string.Format("\u001b[{0};{1}f", row, col);
        public string Move(string col, string row) => string.Format("\u001b[{0};{1}f", row, col);
        public string Start() => Move(0, 0);
        public string SavePosition { get => "\u001b[s"; }
        public string RestorePosition { get => "\u001b[u"; }
        public string ClearLine { get => "\u001b[2K"; }
    }
}
