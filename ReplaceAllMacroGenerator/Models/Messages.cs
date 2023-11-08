namespace ReplaceAllMacroGenerator.Models
{
    public record class POMessage(ReplacementInfo TheInfo);
    public record class OperationErrorMessage(string ErrorType, string ErrorMessage);
}
