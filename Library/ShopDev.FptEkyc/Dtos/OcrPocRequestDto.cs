using System.Text.Json.Serialization;

namespace CR.FptEkyc.Dtos
{
    public class OcrPocRequestDto
    {
        [JsonPropertyName("anhMatTruoc")]
        public required string AnhMatTruoc { get; set; }

        [JsonPropertyName("anhMatSau")]
        public string? AnhMatSau { get; set; }

        [JsonPropertyName("maGiayTo")]
        public string? MaGiayTo { get; set; }
    }

    public class OcrPocFaceMatchRequestDto
    {
        [JsonPropertyName("anhMatTruoc")]
        public required string AnhMatTruoc { get; set; }

        [JsonPropertyName("anhKhachHang")]
        public required string AnhKhachHang { get; set; }
    }

    public class OcrPocLivenessRequestDto
    {
        [JsonPropertyName("anhVideo")]
        public List<OcrPocLivenessImageVideoRequestDto> AnhVideo { get; set; } = new();

        [JsonPropertyName("anhMatTruoc")]
        public required string AnhMatTruoc { get; set; }

        [JsonPropertyName("hanhDong")]
        public OcrPocLivenessActionRequestDto? HanhDong { get; set; }
    }

    public class OcrPocLivenessImageVideoRequestDto
    {
        [JsonPropertyName("anh")]
        public required string Anh { get; set; }

        [JsonPropertyName("thoiGian")]
        public required string ThoiGian { get; set; }
    }

    public class OcrPocLivenessActionRequestDto
    {
        [JsonPropertyName("thoiGianQuayVideo")]
        public string? ThoiGianQuayVideo { get; set; }

        [JsonPropertyName("soAnhGuiLenTrong1s")]
        public string? SoAnhGuiLenTrong1s { get; set; }

        [JsonPropertyName("noiDungHanhDongs")]
        public List<OcrPocLivenessActionDetailRequestDto>? NoiDungHanhDongs { get; set; }
    }

    public class OcrPocLivenessActionDetailRequestDto
    {
        [JsonPropertyName("maHanhDong")]
        public string? MaHanhDong { get; set; }

        [JsonPropertyName("tenHanhDong")]
        public string? TenHanhDong { get; set; }

        [JsonPropertyName("thoiGianHanhDong")]
        public string? ThoiGianHanhDong { get; set; }
    }
}
