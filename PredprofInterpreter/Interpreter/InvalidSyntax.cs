namespace GridMasterPredprof
{
    public class InvalidSyntax : CodeException
    {
        public InvalidSyntax(
            int line,
            string? message = "Неправильная команда"
            ) : base(line, message) { }
    }
}
