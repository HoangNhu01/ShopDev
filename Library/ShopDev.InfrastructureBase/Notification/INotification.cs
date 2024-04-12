using ShopDev.InfrastructureBase.Notification.Dtos;

namespace ShopDev.InfrastructureBase.Notification
{
    public interface INotification
    {
        /// <summary>
        /// Send EMAIL, SMS, PUSH APP
        /// </summary>
        /// <typeparam name="TDto">DTO</typeparam>
        /// <param name="dto">Data</param>
        /// <param name="key">Key</param>
        /// <param name="receiver">Receiver thông tin người nhận</param>
        /// <param name="attachments">Danh sách file đính kèm dạng list url</param>
        /// <returns></returns>
        Task SendNotificationAsync<TDto>(
            TDto dto,
            string key,
            ReceiverDto receiver,
            List<string>? attachments
        )
            where TDto : class;
    }
}
