namespace GridMasterPredprof
{
    public class InvalidName : CodeException
    {
        public InvalidName(int line, string? message = null)
            : base(line, message) { }
    }
}
