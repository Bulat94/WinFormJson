﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinJson
{    
        public class Rootobject
        {
            public DateTime Date { get; set; }
            public DateTime PreviousDate { get; set; }
            public string PreviousURL { get; set; }
            public DateTime Timestamp { get; set; }
            public Dictionary<string, Currency> Valute { get; set; }
        }

        public class Currency
        {
            public string ID { get; set; }
            public string NumCode { get; set; }
            public string CharCode { get; set; }
            public int Nominal { get; set; }
            public string Name { get; set; }
            public double Value { get; set; }
            public double Previous { get; set; }
        }
}