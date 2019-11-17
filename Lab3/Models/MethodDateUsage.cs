using System;

namespace Lab3.Models
{
    public class MethodDateUsage
    {
        public MethodDateUsage(DateTime from, DateTime to)
        {
            DateFrom = from.ToString();
            DateTo = to.ToString();
        }

        public string DateFrom { get; set; }
        public string DateTo { get; set; }
    }
}
