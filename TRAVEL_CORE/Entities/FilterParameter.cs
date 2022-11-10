using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TRAVEL_CORE.Entities
{
    public class FilterParameter
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int OrderStatus { get; set; }
        public List<BrowseColumnFilter>? Filters { get; set; }
    }
}
