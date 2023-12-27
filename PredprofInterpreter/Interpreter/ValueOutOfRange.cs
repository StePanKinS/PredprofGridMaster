namespace GridMasterPredprof
{
    public class ValueOutOfRange : CodeException
    {
        public ValueOutOfRange(int line, string? message = null)
            : base(line, message) { }
    }
}
