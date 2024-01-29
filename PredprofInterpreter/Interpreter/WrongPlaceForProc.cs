namespace GridMasterPredprof
{
    public class WrongPlaceForProc : CodeException
    {
        public WrongPlaceForProc(int line, string? message = null)
            :base(line, message) { }
    }
}
