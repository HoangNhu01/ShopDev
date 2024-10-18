namespace OpenXMLLibrary.Dtos
{
    /// <summary>
    /// Class result cho việc xuất file (tải file trên fe)
    /// </summary>
    public class ExportFileDto
    {
        public Stream? Stream { get; set; }
        public string? FileName { get; set; }
    }
}
