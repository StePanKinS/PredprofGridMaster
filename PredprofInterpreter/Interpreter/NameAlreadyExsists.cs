namespace GridMasterPredprof
{
    public class NameAlreadyExsists : CodeException
    {
        public NameAlreadyExsists(int line, string? message = null)
            : base(line, message) { }
    }
}
