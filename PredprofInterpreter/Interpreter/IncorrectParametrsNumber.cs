namespace GridMasterPredprof
{
    public class IncorrectParametrsNumber : CodeException
    {
        public IncorrectParametrsNumber(int line, string? message = null)
            : base(line, message) { }
    }
}
