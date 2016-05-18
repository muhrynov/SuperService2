﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitMobile.DbEngine;

namespace Test.Catalog
{
    public class Client : DbEntity
    {
        public DbRef Id { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
    }
}
