using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CR.FptEkyc.Dtos
{
    public class OcrPocLivenessDto
    {
        /// <summary>
        /// Ảnh video
        /// </summary>
        public required List<OcrPocLivenessDetailDto> ImageVideo { get; set; }

        /// <summary>
        /// Ảnh mặt trước
        /// </summary>
        public required IFormFile FrontIdImage { get; set; }
    }

    public class OcrPocLivenessDetailDto
    {
        /// <summary>
        /// Ảnh
        /// </summary>
        public required IFormFile Image { get; set; }

        /// <summary>
        /// Thời gian cắt ở giấy bao nhiêu
        /// </summary>
        public required string Time { get; set; }
    }
}
