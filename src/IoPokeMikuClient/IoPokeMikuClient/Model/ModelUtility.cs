﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoPokeMikuClient.Model
{
    static class ModelUtility
    {
        public static bool StartsWith(this Guid fullUuid, string partUuidStr)
        {
            return fullUuid.ToString().ToLower().StartsWith(partUuidStr.ToLower());
        }

        public static bool EndsWith(this Guid fullUuid, string partUuidStr)
        {
            return fullUuid.ToString().ToLower().EndsWith(partUuidStr.ToLower());
        }
    }
}
