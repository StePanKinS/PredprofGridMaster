namespace GridMasterPredprof
{
    public class InvalidSyntax : CodeException
    {
        public InvalidSyntax(int line, string? message = null)
            : base(line, message) { }
    }
}
