namespace BankingApp.Exceptions
{
    public class AccountNotFoundException(string exceptionMessage) : Exception(exceptionMessage)
    {
    }
}