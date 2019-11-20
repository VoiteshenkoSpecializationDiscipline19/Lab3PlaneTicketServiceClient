namespace Lab3.Models
{
    public class Route
    {
        public string routeId { get; set; }
        public string routeFrom { get; set; }    
        public string routeWhere { get; set; }        
        public string routeDate { get; set; }      
        public string routeTime { get; set; }      
        public string routePrice { get; set; }

        public Route(string routeFrom, string routeWhere, string routeDate)
        {
            this.routeFrom = routeFrom;
            this.routeWhere = routeWhere;
            this.routeDate = routeDate;
        }

        public Route()
        {
            routeId = null; 
            routeFrom = null;
            routeWhere = null;
            routeDate = null;
            routeTime = null;
            routePrice = null;
        }
    }
}
