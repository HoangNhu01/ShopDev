using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CR.Signature.Dtos
{
    public class DigitalSignatureResponse
    {
        public required string RequestID { get; set; }
        public required string ResponseID { get; set; }
        public required string ResponseCode { get; set; }
        public required string SignedFileData { get; set; }
        public required string ResponseMessage { get; set; }
    }
}
