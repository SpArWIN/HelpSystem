﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpSystem.Domain.ViewModel.Product
{
    public class BindingProductViewModel
    {
        public Guid Id { get; set; }
        public string NameProduct { get; set; }
        public string  CodeProduct { get; set; }

        public string locationWarehouse { get; set; }
    }
}