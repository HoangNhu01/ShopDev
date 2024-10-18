namespace OpenXMLLibrary.Dtos
{
    public class InputReplaceDto
    {
        public string? FindText { get; set; }
        public string? ReplaceText { get; set; }

        /// <summary>
        /// Stream ảnh
        /// </summary>
        public Stream? ReplaceImage { get; set; }

        /// <summary>
        /// Định dạng ảnh
        /// </summary>
        public string? ReplaceImageExtension { get; set; }

        /// <summary>
        /// Chiều rộng ảnh tính bằng inch
        /// </summary>
        public double ReplaceImageWidth { get; set; }

        /// <summary>
        /// Chiều cao ảnh tính bằng inch
        /// </summary>
        public double ReplaceImageHeight { get; set; }
    }
}
