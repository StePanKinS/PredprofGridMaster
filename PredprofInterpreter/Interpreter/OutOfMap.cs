namespace GridMasterPredprof
{
    public class OutOfMap : CodeException
    {
        public OutOfMap(int line, string? message = null)
            : base(line, message) { }
    }
}
