namespace GridMasterPredprof
{
    public class NameDoesNotExist : CodeException
    {
        public NameDoesNotExist(int line, string? message = null)
            : base(line, message) { }
    }
}
