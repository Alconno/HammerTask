﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Services.Services
{
    public interface IEncrypt
    {
        public string Hash(string value);
    }
}
