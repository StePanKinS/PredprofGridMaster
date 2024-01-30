namespace GridMasterPredprof
{
    public class OutOfMap : CodeException
    {
        public OutOfMap(
            int line, 
            string? message = "Выход за пределы карты"
            ) : base(line, message) { }
    }
}
