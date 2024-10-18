using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CR.PVCB.Dtos.VirtualAccDtos
{
    public class PvcVirtualAccTransactionDto
    {
        public string? TransRef { get; set; }
        public string? BookingDate { get; set; }
        public string? ValueDate { get; set; }
        public string? VANum { get; set; }
        public string? VAName { get; set; }
        public string? CustAcNo { get; set; }
        public string? CustAcName { get; set; }
        public string? Currency { get; set; }
        public string? Amount { get; set; }
    }
}
