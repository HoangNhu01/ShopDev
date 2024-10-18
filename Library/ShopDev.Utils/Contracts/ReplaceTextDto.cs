using ShopDev.Utils.DataUtils;
using System.Globalization;

namespace ShopDev.Utils.Contracts
{
        public class ReplaceTextDto
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

                public ReplaceTextDto() { }

                /// <summary>
                /// Khởi tạo thường
                /// </summary>
                /// <param name="findText"></param>
                /// <param name="replateText"></param>
                public ReplaceTextDto(string findText, string replateText)
                {
                        FindText = findText;
                        ReplaceText = replateText;
                }

                /// <summary>
                /// Khởi tạo biến rỗng
                /// </summary>
                /// <param name="findText"></param>
                public ReplaceTextDto(string findText)
                {
                        FindText = findText;
                        ReplaceText = "";
                }

                /// <summary>
                /// Khởi tạo cho biến kiểu date
                /// </summary>
                /// <param name="findText"></param>
                /// <param name="date"></param>
                /// <param name="toStringPattern"></param>
                public ReplaceTextDto(string findText, DateTime? date, string toStringPattern)
                {
                        FindText = findText;
                        ReplaceText = date?.ToString(toStringPattern);
                }

                /// <summary>
                /// Format cho chữ
                /// </summary>
                /// <param name="findText"></param>
                /// <param name="replateText"></param>
                /// <param name="typeFormat"></param>
                public ReplaceTextDto(string findText, string replateText, EnumReplaceTextFormat typeFormat)
                {
                        FindText = findText;
                        //viết switch case kiểu mới
                        ReplaceText = typeFormat switch
                        {
                                EnumReplaceTextFormat.TitleCase
                                    => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(
                                        replateText?.ToLower() ?? " "
                                    ),
                                EnumReplaceTextFormat.UpperCase => replateText?.ToUpper(),
                                _ => ""
                        };
                }

                /// <summary>
                /// Format cho số
                /// </summary>
                /// <param name="findText"></param>
                /// <param name="replateNumber"></param>
                /// <param name="typeFormat"></param>
                public ReplaceTextDto(
                    string findText,
                    double? replateNumber,
                    EnumReplaceTextFormat typeFormat
                )
                {
                        FindText = findText;
                        //viết switch case kiểu mới
                        if (replateNumber != 0)
                        {
                                ReplaceText = typeFormat switch
                                {
                                        EnumReplaceTextFormat.NumberVietNam
                                            => replateNumber?.ToString("#,#.#", new CultureInfo("is-IS")) ?? "",
                                        EnumReplaceTextFormat.NumberToWord
                                            => NumberToText.ConvertNumberToText(replateNumber ?? 0) ?? "",
                                        _ => ""
                                };
                        }
                        else
                        {
                                ReplaceText = typeFormat switch
                                {
                                        EnumReplaceTextFormat.NumberVietNam => "0",
                                        EnumReplaceTextFormat.NumberToWord => "không",
                                        _ => ""
                                };
                        }
                }

                /// <summary>
                /// Hình ảnh có kích thước
                /// </summary>
                /// <param name="findText"></param>
                /// <param name="replaceImage"></param>
                /// <param name="replaceImageExtension"></param>
                /// <param name="replaceImageWidth"></param>
                /// <param name="replaceImageHeight"></param>
                public ReplaceTextDto(
                    string findText,
                    Stream replaceImage,
                    string replaceImageExtension,
                    double replaceImageWidth,
                    double replaceImageHeight
                )
                {
                        FindText = findText;
                        ReplaceImage = replaceImage;
                        ReplaceImageExtension = replaceImageExtension;
                        ReplaceImageWidth = replaceImageWidth;
                        ReplaceImageHeight = replaceImageHeight;
                }

                /// <summary>
                /// Hình ảnh không có kích thước
                /// </summary>
                /// <param name="findText"></param>
                /// <param name="replaceImage"></param>
                /// <param name="replaceImageExtension"></param>
                public ReplaceTextDto(string findText, Stream replaceImage, string replaceImageExtension)
                {
                        FindText = findText;
                        ReplaceImage = replaceImage;
                        ReplaceImageExtension = replaceImageExtension;
                }
        }
}
