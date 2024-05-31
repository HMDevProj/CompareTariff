namespace Exceptions
{
    public class TariffIdMismatchException : CustomException
    {
        public TariffIdMismatchException(string message) : base(message) { }
    }
}
