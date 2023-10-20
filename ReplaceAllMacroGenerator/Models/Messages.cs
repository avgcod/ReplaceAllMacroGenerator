namespace ReplaceAllMacroGenerator.Models
{
    public record class POMessage(POInfo TheInfo);
    public record class OperationErrorMessage(string ErrorType, string ErrorMessage);
}
