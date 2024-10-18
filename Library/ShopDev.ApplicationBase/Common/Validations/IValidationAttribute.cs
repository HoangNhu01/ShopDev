namespace ShopDev.ApplicationBase.Common.Validations
{
    public interface IValidationAttribute
    {
        string? ErrorMessageLocalization { get; set; }
    }
}
