using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model.Momo
{
    public class MomoExecuteResponse
    {
        public string OrderId { get; set; }
        public string Amount { get; set; }
        public string FullName { get; set; }
        public string OrderInfo { get; set; }

    }
}
