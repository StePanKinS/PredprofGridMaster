namespace GridMasterPredprof
{
    public class MaxDepthReachedexception : CodeException
    {
        public MaxDepthReachedexception(int line, string? message = null)
            : base(line, message) { }
    }
}
