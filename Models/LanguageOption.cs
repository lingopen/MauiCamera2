using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiCamera2.Models
{
    public class LanguageOption
    {
        public string Name { get; set; }
        public string Code { get; set; }

        public LanguageOption(string name, string code)
        {
            Name = name;
            Code = code;
        }
    }
}
