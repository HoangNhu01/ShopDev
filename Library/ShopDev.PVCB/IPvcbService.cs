using CR.InfrastructureBase.Bank;
using CR.PVCB.Dtos.BankInfoDto;
using CR.PVCB.Dtos.DepositDto;
using CR.PVCB.Dtos.InfoAccountDto;
using CR.PVCB.Dtos.InquiryDtos;
using CR.PVCB.Dtos.TokenDtos;
using CR.PVCB.Dtos.VirtualAccDtos;

namespace CR.PVCB
{
    public interface IPvcbService
        : IInquiry<Task<PvcbInquiryResponseDto>, PvcbInquiryRequestDto>,
            IDeposit<Task<PvcbDepositResponseDto>, PvcbDepositRequestDto>
    {
        /// <summary>
        /// Lấy token từ Pvcb
        /// </summary>
        /// <returns></returns>
        Task<PvcbGetTokenReponseDto> GetPvcbToken();

        /// <summary>
        /// Get danh sách ngân hàng từ Pvcb
        /// </summary>
        /// <returns></returns>
        Task<PvcbBankInfoResponseDto> GetListBankFromPvcb();

        /// <summary>
        /// Tạo tạo mới tài khoản ảo
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task OpenVirtualAcc(string vaName, string vaNumber);

        /// <summary>
        /// Xem chi tiết 1 vacc
        /// </summary>
        /// <param name="accNumber"></param>
        /// <returns></returns>
        Task<PvcVirtualResponeBaseDto<PvcVirtualAccDto>> GetDetailVirtualAcc(string accNumber);

        /// <summary>
        /// Lấy danh sach giao dịch từ vacc
        /// </summary>
        /// <param name="accNumber"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        Task<PvcVirtualResponeBaseDto<PvcVirtualAccTransactionDto>> GetListTransactionOfVirtualAcc(
            string accNumber,
            string dateTime
        );

        /// <summary>
        /// Khóa vacc
        /// </summary>
        /// <param name="accNumber"></param>
        /// <param name="lockReason"></param>
        /// <returns></returns>
        Task<PvcVirtualResponeBaseDto> LockVirtualAcc(string accNumber, string lockReason);

        /// <summary>
        /// Mở khóa vacc
        /// </summary>
        /// <param name="accNumber"></param>
        /// <returns></returns>
        Task<PvcVirtualResponeBaseDto> UnLockVirtualAcc(string accNumber);

        /// <summary>
        /// Đóng vacc
        /// </summary>
        /// <param name="accNumber"></param>
        /// <returns></returns>
        Task<PvcVirtualResponeBaseDto> CloseVirtualAcc(string accNumber);

        /// <summary>
        /// Truy vấn số dư tài khoản ngân hàng
        /// </summary>
        /// <returns></returns>
        Task<RepsonseInfoAcountDto> PvcbInfoAccount();

        /// Kiểm tra otp google auth
        /// </summary>
        /// <param name="otp"></param>
        /// <returns></returns>
        bool VerifyGoogleAuthOtp(string otp);

        /// <summary>
        /// Lấy ảnh qr base64 google auth
        /// </summary>
        /// <returns></returns>
        string GetGoogleAuthQrBase64();
    }
}
