using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankUtilityApiLib.TankUtilityApi
{
    
    public class TankUtilityApiException : Exception
    {
        public int ErrorCode { get; set; }

        public TankUtilityApiException(int errorCode) : this(errorCode, null, null) { }
        public TankUtilityApiException(int errorCode, string message) : this(errorCode, message, null) { }
        public TankUtilityApiException(int errorCode, string message, Exception ex) : base(message, ex)
        {
            ErrorCode = errorCode;
        }
    }

}
