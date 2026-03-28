namespace CR.DtoBase.Validations
{
    public interface IValidationAttribute
    {
        string? ErrorMessage { get; set; }
    }
}