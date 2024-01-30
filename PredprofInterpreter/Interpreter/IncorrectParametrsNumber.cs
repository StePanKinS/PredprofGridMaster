namespace GridMasterPredprof
{
    public class IncorrectParametrsNumber : CodeException
    {
        public IncorrectParametrsNumber(
            int line,
            string? message = "Неправильное количество параметров"
            ) : base(line, message) { }
    }
}
