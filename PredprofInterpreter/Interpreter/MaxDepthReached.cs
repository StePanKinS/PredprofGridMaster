namespace GridMasterPredprof
{
    public class MaxDepthReached : CodeException
    {
        public MaxDepthReached(int line, string? message = null)
            : base(line, message) { }
    }
}
