﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionGuard.Database
{
    public class User
    {
        public int Id { get; set; }
        public string Channel { get; set; } = string.Empty;
        public bool Active { get; set; }
        public ICollection<Panic>? Panics { get; set; }
    }
}
