using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHX.BBS.TS.Screen
{
    static class File
    {
        public static string Read(string name)
        {
            string path = @"./Screen/Assets/" + name+".txt";
            try
            {
                return System.IO.File.ReadAllText(path);
            }
            catch
            {
                return "Failed to read text op " + name;
            }
        }
    }
}
