using CR.PVCB.Dtos.EkycDtos;

namespace CR.PVCB
{
    public interface IPvcbEkycService
    {
        Task<PvcbEkycResponseInformationDto> GetCustomerInformation(string userIdPartner);
    }
}
