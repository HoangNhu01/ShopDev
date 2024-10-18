namespace ShopDev.Constants.RabbitMQ
{
    public static class RabbitQueues
    {
        public const string UpdateStock = "inventory.stock.update";
        public const string UpdateOrder = "inventory.order.update";

        public const string LogAuth = "log.auth";
        public const string LogCore = "log.core";
        public const string LogInvest = "log.invest";
        public const string LogMedia = "log.media";
        public const string LogPayment = "log.payment";
        public const string LogNoti = "log.noti";
    }
}
