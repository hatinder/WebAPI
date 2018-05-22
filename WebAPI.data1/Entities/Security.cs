using System;
using System.Collections.Generic;
using System.Text;

//TODO: symbol could be used as key between two tables, SecurityId isnt needed.
namespace WebAPI.data1.Entities
{
    public class Security
    {
        public Security()
        {
            PriceCollection=new List<Prices>();
        }
        public int SecurityId { get; set; }
        public string symbol { get; set; }
        public string SecurityName { get; set; }
        public string Sector { get; set; }
        public string SubIndustry { get; set; }
        public ICollection<Prices> PriceCollection { get; set; }
        
    }
}
