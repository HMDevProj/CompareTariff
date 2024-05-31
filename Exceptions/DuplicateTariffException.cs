namespace Exceptions
{
    public class DuplicateTariffException : CustomException
    {
        public DuplicateTariffException(string message) : base(message) { }
    }
}
