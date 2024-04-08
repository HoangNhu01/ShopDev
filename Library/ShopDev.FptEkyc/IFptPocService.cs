using CR.FptEkyc.Dtos;
using CR.InfrastructureBase.Ekyc;
using CR.InfrastructureBase.Ekyc.Dtos;

namespace CR.FptEkyc
{
    public interface IFptPocService : IEkyc
    {
        /// <summary>
        /// Lấy mã giao dịch
        /// </summary>
        /// <returns></returns>
        Task<OcrPocCodeTransactionResponseDto> GetCodeTransaction();
    }
}
