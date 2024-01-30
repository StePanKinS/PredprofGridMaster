namespace GridMasterPredprof
{
    public class WrongPlaceForProc : CodeException
    {
        public WrongPlaceForProc(
            int line, 
            string? message = "Неправильное место объявления процедуры"
            ) : base(line, message) { }
    }
}
