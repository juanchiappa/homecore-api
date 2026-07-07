using System;
using System.Collections.Generic;
using System.Text;

namespace HomeCore.Entities
{
    public class ServicesConfig
    {
        public List<ServiceMonitorConfig> Services { get; set; } = new();
    }
}
