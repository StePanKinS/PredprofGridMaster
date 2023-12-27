namespace GridMasterPredprof
{
    public class CodeException : Exception
    {
        public int line { get; }

        public CodeException(int line, string? message = null)
            : base(message)
        {
            this.line = line;
        }
    }
}
