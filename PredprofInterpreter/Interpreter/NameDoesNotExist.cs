namespace GridMasterPredprof
{
    public class NameDoesNotExist : CodeException
    {
        public NameDoesNotExist(
            int line, 
            string? message = "Имени не существует"
            ) : base(line, message) { }
    }
}
