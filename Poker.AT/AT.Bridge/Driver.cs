﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AT.Bridge
{
    public class Driver
    {
        public static TestsBridge getBridge()
        {
            ProxyBridge bridge = new ProxyBridge();

            bridge.setRealBridge(new Real()); // add real bridge here
            return bridge;
        }
    }
}
