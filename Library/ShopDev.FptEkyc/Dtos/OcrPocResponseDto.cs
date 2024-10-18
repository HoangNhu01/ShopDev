using System.Text.Json.Serialization;

namespace CR.FptEkyc.Dtos
{
    public class OcrChiTietNoiTru
    {
        [JsonPropertyName("province")]
        public string? Province { get; set; }

        [JsonPropertyName("district")]
        public string? District { get; set; }

        [JsonPropertyName("ward")]
        public string? Ward { get; set; }

        [JsonPropertyName("street")]
        public string? Street { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }
    }

    public class OcrData
    {
        [JsonPropertyName("soCmt")]
        public string? SoCmt { get; set; }

        [JsonPropertyName("hoVaTen")]
        public string? HoVaTen { get; set; }

        [JsonPropertyName("namSinh")]
        public string? NamSinh { get; set; }

        [JsonPropertyName("queQuan")]
        public string? QueQuan { get; set; }

        [JsonPropertyName("noiTru")]
        public string? NoiTru { get; set; }

        [JsonPropertyName("dacDiemNhanDang")]
        public string? DacDiemNhanDang { get; set; }

        [JsonPropertyName("ngayCap")]
        public string? NgayCap { get; set; }

        [JsonPropertyName("noiCap")]
        public string? NoiCap { get; set; }

        [JsonPropertyName("loaiCmtMatTruoc")]
        public string? LoaiCmtMatTruoc { get; set; }

        [JsonPropertyName("loaiCmtMatSau")]
        public string? LoaiCmtMatSau { get; set; }

        [JsonPropertyName("loaiCmtKhacMatTruoc")]
        public string? LoaiCmtKhacMatTruoc { get; set; }

        [JsonPropertyName("quocTich")]
        public string? QuocTich { get; set; }

        [JsonPropertyName("ngayHetHan")]
        public string? NgayHetHan { get; set; }

        [JsonPropertyName("gioiTinh")]
        public string? GioiTinh { get; set; }

        [JsonPropertyName("chiTietNoiTru")]
        public OcrChiTietNoiTru? ChiTietNoiTru { get; set; }

        [JsonPropertyName("kiemTraMatTruoc")]
        public OcrKiemTraMatTruoc? KiemTraMatTruoc { get; set; }

        [JsonPropertyName("kiemTraMatSau")]
        public OcrKiemTraMatSau? KiemTraMatSau { get; set; }

        [JsonPropertyName("score")]
        public string? Score { get; set; }

        [JsonPropertyName("soCmtScore")]
        public string? SoCmtScore { get; set; }

        [JsonPropertyName("hoVaTenScore")]
        public string? HoVaTenScore { get; set; }

        [JsonPropertyName("namSinhScore")]
        public string? NamSinhScore { get; set; }

        [JsonPropertyName("ngayHetHanScore")]
        public string? NgayHetHanScore { get; set; }

        [JsonPropertyName("queQuanScore")]
        public string? QueQuanScore { get; set; }

        [JsonPropertyName("noiTruScore")]
        public string? NoiTruScore { get; set; }

        [JsonPropertyName("danTocScore")]
        public string? DanTocScore { get; set; }

        [JsonPropertyName("tonGiaoScore")]
        public string? TonGiaoScore { get; set; }

        [JsonPropertyName("ngayCapScore")]
        public string? NgayCapScore { get; set; }

        [JsonPropertyName("noiCapScore")]
        public string? NoiCapScore { get; set; }

        [JsonPropertyName("gioiTinhScore")]
        public string? GioiTinhScore { get; set; }

        [JsonPropertyName("quocTichScore")]
        public string? QuocTichScore { get; set; }

        [JsonPropertyName("maHoa")]
        public OcrMaHoa? MaHoa { get; set; }

        [JsonPropertyName("maTinhQueQuan")]
        public string? MaTinhQueQuan { get; set; }

        [JsonPropertyName("maTinhDiaChi")]
        public string? MaTinhDiaChi { get; set; }

        [JsonPropertyName("maTinhNoiCap")]
        public string? MaTinhNoiCap { get; set; }
    }

    public class OcrPassportData
    {
        [JsonPropertyName("soHoChieu")]
        public string? SoHoChieu { get; set; }

        [JsonPropertyName("soCmt")]
        public string? SoCmt { get; set; }

        [JsonPropertyName("hoVaTen")]
        public string? HoVaTen { get; set; }

        [JsonPropertyName("namSinh")]
        public string? NamSinh { get; set; }

        [JsonPropertyName("diaChi")]
        public string? DiaChi { get; set; }

        [JsonPropertyName("ngayCap")]
        public string? NgayCap { get; set; }

        [JsonPropertyName("noiCap")]
        public string? NoiCap { get; set; }

        [JsonPropertyName("quocTich")]
        public string? QuocTich { get; set; }

        [JsonPropertyName("ngayHetHan")]
        public string? NgayHetHan { get; set; }

        [JsonPropertyName("gioiTinh")]
        public string? GioiTinh { get; set; }
    }

    public class OcrKiemTraMatSau
    {
        [JsonPropertyName("dauDo")]
        public string? DauDo { get; set; }

        [JsonPropertyName("dauDoScore")]
        public string? DauDoScore { get; set; }

        [JsonPropertyName("anhBiLoa")]
        public string? AnhBiLoa { get; set; }

        [JsonPropertyName("anhBiLoaScore")]
        public string? AnhBiLoaScore { get; set; }

        [JsonPropertyName("vanTayPhai")]
        public string? VanTayPhai { get; set; }

        [JsonPropertyName("vanTayPhaiScore")]
        public string? VanTayPhaiScore { get; set; }

        [JsonPropertyName("vanTayTrai")]
        public string? VanTayTrai { get; set; }

        [JsonPropertyName("vanTayTraiScore")]
        public string? VanTayTraiScore { get; set; }

        [JsonPropertyName("khungHinh")]
        public string? KhungHinh { get; set; }

        [JsonPropertyName("khungHinhScore")]
        public string? KhungHinhScore { get; set; }
    }

    public class OcrKiemTraMatTruoc
    {
        [JsonPropertyName("chupLaiTuManHinh")]
        public string? ChupLaiTuManHinh { get; set; }

        [JsonPropertyName("chupLaiTuManHinhScore")]
        public string? ChupLaiTuManHinhScore { get; set; }

        [JsonPropertyName("denTrang")]
        public string? DenTrang { get; set; }

        [JsonPropertyName("denTrangScore")]
        public string? DenTrangScore { get; set; }

        [JsonPropertyName("catGoc")]
        public string? CatGoc { get; set; }

        [JsonPropertyName("catGocScore")]
        public List<string>? CatGocScore { get; set; }

        [JsonPropertyName("dauNoi")]
        public string? DauNoi { get; set; }

        [JsonPropertyName("dauQuocHuy")]
        public string? DauQuocHuy { get; set; }

        [JsonPropertyName("dauQuocHuyScore")]
        public string? DauQuocHuyScore { get; set; }

        [JsonPropertyName("anhBiLoa")]
        public string? AnhBiLoa { get; set; }

        [JsonPropertyName("anhBiLoaScore")]
        public string? AnhBiLoaScore { get; set; }

        [JsonPropertyName("kiemTraAnh")]
        public string? KiemTraAnh { get; set; }

        [JsonPropertyName("kiemTraAnhScore")]
        public string? KiemTraAnhScore { get; set; }

        [JsonPropertyName("thayTheAnh")]
        public string? ThayTheAnh { get; set; }

        [JsonPropertyName("thayTheAnhScore")]
        public string? ThayTheAnhScore { get; set; }

        [JsonPropertyName("khungHinh")]
        public string? KhungHinh { get; set; }

        [JsonPropertyName("khungHinhScore")]
        public string? KhungHinhScore { get; set; }

        [JsonPropertyName("ngayHetHan")]
        public string? NgayHetHan { get; set; }

        [JsonPropertyName("quyLuatSo")]
        public string? QuyLuatSo { get; set; }
    }

    public class OcrMaHoa
    {
        [JsonPropertyName("noiDung")]
        public string? NoiDung { get; set; }

        [JsonPropertyName("soCmt")]
        public string? SoCmt { get; set; }

        [JsonPropertyName("hoVaTen")]
        public string? HoVaTen { get; set; }

        [JsonPropertyName("namSinh")]
        public string? NamSinh { get; set; }

        [JsonPropertyName("ngayHetHan")]
        public string? NgayHetHan { get; set; }

        [JsonPropertyName("gioiTinh")]
        public string? GioiTinh { get; set; }
    }

    public class OcrPocResponseDto
    {
        [JsonPropertyName("status")]
        public int? Status { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("included")]
        public object? Included { get; set; }

        [JsonPropertyName("codeMessage")]
        public object? CodeMessage { get; set; }
    }

    public class OcrPocIdenResponseDto : OcrPocResponseDto
    {
        [JsonPropertyName("data")]
        public OcrData? Data { get; set; }
    }

    public class OcrPocPassportResponseDto : OcrPocResponseDto
    {
        [JsonPropertyName("data")]
        public OcrPassportData? Data { get; set; }
    }

    public class OcrPocLivenessResponseDto : OcrPocResponseDto
    {
        [JsonPropertyName("data")]
        public double? Data { get; set; }
    }

    public class OcrPocCodeTransactionResponseDto : OcrPocResponseDto
    {
        [JsonPropertyName("data")]
        public string? Data { get; set; }
    }
}
