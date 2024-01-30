namespace GridMasterPredprof
{
    public class NameAlreadyExsists : CodeException
    {
        public NameAlreadyExsists(
            int line, 
            string? message = "Имя уже существует"
            ) : base(line, message) { }
    }
}
