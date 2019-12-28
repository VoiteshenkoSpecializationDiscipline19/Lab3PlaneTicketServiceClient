using System;

namespace Lab3.Models
{
    public class MethodUsageDates
    {
        public MethodUsageDates(DateTime from, DateTime to)
        {
            DateFrom = from.ToString("d");
            DateTo = to.ToString("d");
        }

        public string DateFrom { get; set; }
        public string DateTo { get; set; }
    }
}