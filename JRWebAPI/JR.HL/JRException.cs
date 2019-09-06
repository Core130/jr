using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JR.HL
{
    public class JRException : Exception
    {
        public JRException(string msg) : base(msg) { }
    }
}
