using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyBitzApiLib.SkyBitzApi
{
    
    public class SkyBitzApiException : Exception
    {
        public int ErrorCode { get; set; }

        public SkyBitzApiException(int errorCode) : this(errorCode, null, null) { }
        public SkyBitzApiException(int errorCode, string message) : this(errorCode, message, null) { }
        public SkyBitzApiException(int errorCode, string message, Exception ex) : base(message, ex)
        {
            ErrorCode = errorCode;
        }
    }

}
