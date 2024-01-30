namespace GridMasterPredprof
{
    public class CodeException(int line, string? message = null) : Exception(message)
    {
        public int line { get; } = line;
    }
}
