namespace GridMasterPredprof
{
    public class MaxDepthReached : CodeException
    {
        public MaxDepthReached(
            int line,
            string? message = "Превышена максимальная вложенность"
            ) : base(line, message) { }
    }
}
