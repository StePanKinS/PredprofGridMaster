namespace GridMasterPredprof
{
    public class IncorrectBlockClose : CodeException
    {
        public BlockType BlockType { get; }
        public BlockType TryToClose { get; }
        public IncorrectBlockClose(
            int line,
            BlockType blockType,
            BlockType tryToClose,
            string? message = "Неправильное закрытие блока"
            ) : base(line, message)
        {
            BlockType = blockType;
            TryToClose = tryToClose;
        }
    }
}
