﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RadiosMotorola.Models
{
    public class Radio
    {
        public int id { get; set; }
        public string alias { get; set; }
        public string allowed_locations { get; set; } //public List<string> allowed_location { get; set; }, but Entity Frawork does not allow complex vars :(
        public string location { get; set; }
    }
}
