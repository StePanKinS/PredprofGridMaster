namespace GridMasterPredprof
{
    public class InvalidName : CodeException
    {
        public InvalidName(
            int line,
            string? message = "Неправильное имя"
            ) : base(line, message) { }
    }
}
