using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerProject.Utilities
{
    public static class StringHelper
    {
        public static string RemoveSpecial(string s)
        {
            return s.Replace("\n", "").Replace("\r", "").Replace("\t", "");
        }
    }
}
