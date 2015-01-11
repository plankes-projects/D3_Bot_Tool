using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D3_Bot_Tool
{
    class MyResult
    {
        public MyResult(bool error, String msg)
        {
            this.error = error;
            this.error_msg = msg;
        }

        public bool error;
        public String error_msg;
    }
}
