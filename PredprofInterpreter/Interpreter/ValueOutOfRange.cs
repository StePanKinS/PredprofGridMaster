namespace GridMasterPredprof
{
    public class ValueOutOfRange : CodeException
    {
        public ValueOutOfRange(
            int line, 
            string? message = "Значение вне допустимого диапазона"
            ) : base(line, message) { }
    }
}
