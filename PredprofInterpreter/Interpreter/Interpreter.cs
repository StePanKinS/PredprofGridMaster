namespace GridMasterPredprof
{
    public class Interpreter
    {
        private string program;
        private List<(int x, int y)> positions = new List<(int x, int y)>();
        private Dictionary<string, int> vars = new Dictionary<string, int>();
        private Dictionary<string, int> procs = new Dictionary<string, int>();

        private int xMax = 21;
        private int yMax = 21;

        private int maxDepth = 3;

        private bool? isAlive = null;
        private bool isSteppingMode = true;
        private bool isStepAvailable = false;
        private Exception? threadException = null;

        private int returnedCount = 0;

        string availableChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIGKLMNOPQRSTUVWXYZ";

        public Interpreter(string program, bool isSteppingMode = true)
        {
            this.program = program;
            this.isSteppingMode = isSteppingMode;
            positions.Add((0, 0));
        }

        public void Start()
        {
            isAlive = true;

            setUpProcs();

            new Thread(() =>
            {
                try
                {
                    run(0, 0, BlockType.Main);
                }
                catch (Exception e)
                {
                    threadException = e;
                }
                finally
                {
                    isAlive = false;
                    isStepAvailable = false;
                }
            }).Start();
        }

        public (int x, int y)? Step()
        {
            if (isAlive == null)
            {
                throw new Exception();
            }

            if (!(bool)isAlive)
            {
                if (threadException != null)
                {
                    throw threadException;
                }

                return null;
            }

            isStepAvailable = true;

            while (isStepAvailable)
            { }

            if (returnedCount < positions.Count)
            {
                return positions[returnedCount++];
            }
            return null;
        }

        public (int x, int y)[] GetTrajectory()
        {
            if (isAlive == null)
            {
                throw new Exception();
            }

            if ((bool)isAlive && isSteppingMode)
            {
                throw new Exception();
            }

            while ((bool)isAlive)
            {
                Thread.Sleep(3);
            }

            if (threadException != null)
            {
                throw threadException;
            }

            (int x, int y)[] ret = new (int, int)[positions.Count];
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = positions[i];
            }
            return ret;
        }

        public bool IsRunning()
        {
            return isAlive == true;
        }

        private void setUpProcs()
        {
            int pos = 0;
            while (true)
            {
                string? line = readLine(pos);

                if (line == null) 
                    return;


                string[] words = line.Trim().Split();
                cutWords(ref words);

                if(words.Length == 0 || words[0] != "procedure")
                {
                    moveCursor(ref pos);
                    continue;
                }


                testCount2(words, pos);

                testName(words[1], pos);

                if (procs.ContainsKey(words[1]))
                {
                    throw new NameAlreadyExsists(getLine(pos));
                }

                moveCursor(ref pos);

                procs.Add(words[1], pos);

                findClose(ref pos, BlockType.Procedure);
            }
        }

        private void run(int pos, int depth, BlockType blockType)
        {
            if (depth > maxDepth)
            {
                throw new MaxDepthReached(getLine(pos));
            }

            while (true)
            {
                if (isSteppingMode)
                {
                    while (!isStepAvailable)
                    {
                        if (!(isAlive ?? true))
                        {
                            return;
                        }
                        Thread.Sleep(3);
                    }
                }

                int startPos = pos;
                string? line = readLine(pos);
                moveCursor(ref pos);

                if (line == null)
                {
                    if (blockType != BlockType.Main)
                    {
                        throw new IncorrectBlockClose(getLine(pos), blockType, BlockType.Main);
                    }
                    return;
                }

                line = line.Trim();

                if(line == "")
                {
                    continue;
                }

                string[] words = line.Split();
                cutWords(ref words);

                switch (words[0].ToLower())
                {
                    case "right":
                        {
                            testCount2(words, startPos);

                            int n = getInt(words[1], startPos);


                            (int x, int y) p = positions[positions.Count - 1];
                            p.x += n;
                            if (p.x >= xMax)
                            {
                                
                                throw new OutOfMap(getLine(startPos));
                            }

                            positions.Add(p);

                            break;
                        }
                    case "left":
                        {
                            testCount2(words, startPos);

                            int n = getInt(words[1], startPos);

                            (int x, int y) p = positions[positions.Count - 1];
                            p.x -= n;

                            if (p.x < 0)
                            {
                                throw new OutOfMap(getLine(startPos));
                            }

                            positions.Add(p);

                            break;
                        }
                    case "up":
                        {
                            testCount2(words, startPos);

                            int n = getInt(words[1], startPos);

                            (int x, int y) p = positions[positions.Count - 1];
                            p.y += n;

                            if (p.y >= yMax)
                            {
                                throw new OutOfMap(getLine(startPos));
                            }

                            positions.Add(p);

                            break;
                        }
                    case "down":
                        {
                            testCount2(words, startPos);

                            int n = getInt(words[1], startPos);

                            (int x, int y) p = positions[positions.Count - 1];
                            p.y -= n;

                            if (p.y < 0)
                            {
                                throw new OutOfMap(getLine(startPos));
                            }

                            positions.Add(p);

                            break;
                        }
                    case "ifblock":
                        {
                            int p = pos;
                            findClose(ref pos, BlockType.If);

                            testCount2(words, startPos);

                            switch (words[1].ToLower())
                            {
                                case "right":
                                    {
                                        if (positions[positions.Count - 1].x == 20)
                                            run(p, depth + 1, BlockType.If);

                                        break;
                                    }
                                case "left":
                                    {
                                        if (positions[positions.Count - 1].x == 0)
                                            run(p, depth + 1, BlockType.If);

                                        break;
                                    }
                                case "up":
                                    {
                                        if (positions[positions.Count - 1].y == 20)
                                            run(p, depth + 1, BlockType.If);

                                        break;
                                    }
                                case "down":
                                    {
                                        if (positions[positions.Count - 1].y == 0)
                                            run(p, depth + 1, BlockType.If);

                                        break;
                                    }
                                default:
                                    {
                                        throw new InvalidSyntax(getLine(startPos));
                                    }
                            }

                            break;
                        }
                    case "endif":
                        {
                            if (blockType != BlockType.If)
                            {
                                throw new IncorrectBlockClose(getLine(startPos), blockType, BlockType.If);
                            }

                            return;
                        }
                    case "repeat":
                        {
                            int p = pos;
                            findClose(ref pos, BlockType.Repeat);

                            testCount2(words, startPos);

                            int n = getInt(words[1], startPos);

                            for (int i = 0; i < n; i++)
                            {
                                run(p, depth + 1, BlockType.Repeat);
                            }

                            break;
                        }
                    case "endrepeat":
                        {
                            if (blockType != BlockType.Repeat)
                            {
                                throw new IncorrectBlockClose(getLine(startPos), blockType, BlockType.Repeat);
                            }

                            return;
                        }
                    case "set":
                        {
                            if (words.Length != 4)
                            {
                                throw new IncorrectParametrsNumber(getLine(startPos));
                            }

                            testName(words[1], startPos);

                            if (words[2] != "=")
                            {
                                throw new InvalidSyntax(getLine(startPos));
                            }

                            int n = getInt(words[3], startPos);
                            vars[words[1]] = n;

                            break;
                        }
                    case "procedure":
                        {
                            if (blockType != BlockType.Main)
                                throw new WrongPlaceForProc(getLine(startPos));

                            findClose(ref pos, BlockType.Procedure);

                            break;
                        }
                    case "endproc":
                        {
                            if (blockType != BlockType.Procedure)
                            {
                                throw new IncorrectBlockClose(getLine(startPos), BlockType.Procedure, blockType);
                            }

                            return;
                        }
                    case "call":
                        {
                            testCount2(words, startPos);

                            int p;
                            try
                            {
                                p = procs[words[1]];
                            }
                            catch (KeyNotFoundException)
                            {
                                throw new NameDoesNotExist(getLine(startPos));
                            }

                            run(p, depth + 1, BlockType.Procedure);

                            break;
                        }
                    default:
                        {
                            throw new InvalidSyntax(getLine(startPos));
                        }
                }

                isStepAvailable = false;
            }
        }

        private void cutWords(ref string[] words)
        {
            for (int i = 0; i < words.Length; i++)
            {
                while (true)
                {
                    if (i >= words.Length || words[i] != "")
                        break;

                    words = removeAt(words, i);
                }
            }
        }

        private string[] removeAt(string[] arr, int index)
        {
            string[] res = new string[arr.Length - 1];

            for(int i = 0; i < index; i++)
            {
                res[i] = arr[i];
            }
            for(int i = index; i < res.Length; i++)
            {
                res[i] = arr[i + 1];
            }

            return res;
        }

        private void findClose(ref int pos, BlockType searchFor)
        {
            List<(BlockType type, int pos)> ints = [];
            string? line;

            while (true)
            {
                line = readLine(pos);
                if (line == null)
                {
                    throw new IncorrectBlockClose(getLine(pos), searchFor, BlockType.Main);
                }

                string[] words = line.Trim().ToLower().Split();

                switch (words[0])
                {
                    case "ifblock":
                        {
                            ints.Add((BlockType.If, pos));

                            break;
                        }
                    case "repeat":
                        {
                            ints.Add((BlockType.Repeat, pos));

                            break;
                        }
                    case "procedure":
                        {
                            ints.Add((BlockType.Procedure, pos));

                            break;
                        }
                    case "endif":
                        {
                            if (ints.Count == 0)
                            {
                                if (searchFor == BlockType.If)
                                {
                                    moveCursor(ref pos);
                                    return;
                                }

                                throw new IncorrectBlockClose(getLine(pos), BlockType.If, searchFor);
                            }

                            if (ints[ints.Count - 1].type != BlockType.If)
                            {
                                throw new IncorrectBlockClose(getLine(pos), BlockType.If, ints[ints.Count - 1].type);
                            }

                            ints.RemoveAt(ints.Count - 1);

                            break;
                        }
                    case "endrepeat":
                        {
                            if (ints.Count == 0)
                            {
                                if (searchFor == BlockType.Repeat)
                                {
                                    moveCursor(ref pos);
                                    return;
                                }

                                throw new IncorrectBlockClose(getLine(pos), BlockType.Repeat, searchFor);
                            }

                            if (ints[ints.Count - 1].type != BlockType.Repeat)
                            {
                                throw new IncorrectBlockClose(getLine(pos), BlockType.Repeat, ints[ints.Count - 1].type);
                            }

                            ints.RemoveAt(ints.Count - 1);

                            break;
                        }
                    case "endproc":
                        {
                            if (ints.Count == 0)
                            {
                                if (searchFor == BlockType.Procedure)
                                {
                                    moveCursor(ref pos);
                                    return;
                                }

                                throw new IncorrectBlockClose(getLine(pos), BlockType.Procedure, searchFor);
                            }

                            if (ints[ints.Count - 1].type != BlockType.Procedure)
                            {
                                throw new IncorrectBlockClose(getLine(pos), BlockType.Procedure, ints[ints.Count - 1].type);
                            }

                            ints.RemoveAt(ints.Count - 1);

                            break;
                        }
                }

                moveCursor(ref pos);
            }
        }

        private int getInt(string str, int position)
        {
            if (!int.TryParse(str, out int n))
            {
                try
                {
                    n = vars[str];
                }
                catch (KeyNotFoundException)
                {
                    throw new NameDoesNotExist(getLine(position));
                }
            }

            if (n < 1 || n > 1000)
            {
                throw new ValueOutOfRange(getLine(position));
            }

            return n;
        }

        private int getLine(int pos)
        {
            int n = 1;
            for(int i = 0; i < pos; i++)
            {
                char c = program[i];
                if(c == '\n')
                {
                    n++;
                }
            }
            return n;
        }

        private void testCount2(string[] words, int position)
        {

            if (words.Length != 2)
            {
                throw new IncorrectParametrsNumber(getLine(position));
            }
        }

        private void testName(string word, int position)
        {
            foreach(char c in word)
            {
                if (!availableChars.Contains(c))
                {
                    throw new InvalidName(getLine(position));
                }
            }
        }
        
        private string? readLine(int pos)
        {
            string line = "";
            char c;

            int i = pos;
            while (i < program.Length)
            {
                c = program[i++];
                if(c == '\n' || c == '\r')
                {
                    return line;
                }
                line += c;
            }

            if(i > pos)
            {
                return line;
            }

            return null;
        }

        private void moveCursor(ref int pos)
        {
            while (pos < program.Length)
            {
                char c = program[pos++];

                if(c == '\n')
                {
                    return;
                }
            }
        }
    }
}
