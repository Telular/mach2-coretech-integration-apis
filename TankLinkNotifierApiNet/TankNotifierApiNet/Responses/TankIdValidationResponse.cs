﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TankNotifierApiNet.Responses
{
    public class TankIdValidationResponse : ApiResponse
    {
        public string ShortTankId { get; set; }
        public string LongTankId { get; set;  }
    }
}
