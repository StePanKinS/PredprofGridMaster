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
                string? line = readLine(ref pos);

                if (line == null)
                {
                    if (blockType != BlockType.Main)
                    {
                        throw new IncorrectBlockClose(getLine(startPos), blockType, BlockType.Main);
                    }
                    return;
                }

                line = line.Trim();

                if(line == "")
                {
                    continue;
                }

                string[] words = line.Split();

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
                            int p = pos;
                            findClose(ref pos, BlockType.Procedure);

                            testCount2(words, startPos);

                            testName(words[1], getLine(startPos));

                            if (procs.ContainsKey(words[1]))
                            {
                                throw new NameAlreadyExsists(getLine(startPos));
                            }

                            procs.Add(words[1], p);

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

        private void findClose(ref int pos, BlockType searchFor)
        {
            List<(BlockType type, int pos)> ints = new List<(BlockType, int)>();
            string? line;

            while (true)
            {
                int p = pos;
                line = readLine(ref pos);
                if (line == null)
                {
                    throw new IncorrectBlockClose(getLine(p), searchFor, BlockType.Main);
                }

                string[] words = line.Trim().ToLower().Split();

                switch (words[0])
                {
                    case "ifblock":
                        {
                            ints.Add((BlockType.If, p));

                            break;
                        }
                    case "repeat":
                        {
                            ints.Add((BlockType.Repeat, p));

                            break;
                        }
                    case "procedure":
                        {
                            ints.Add((BlockType.Procedure, p));

                            break;
                        }
                    case "endif":
                        {
                            if (ints.Count == 0)
                            {
                                if (searchFor == BlockType.If)
                                {
                                    return;
                                }
                            }

                            if (ints[ints.Count - 1].type != BlockType.If)
                            {
                                throw new IncorrectBlockClose(getLine(p), BlockType.If, ints[ints.Count - 1].type);
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
                                    return;
                                }
                            }

                            if (ints[ints.Count - 1].type != BlockType.Repeat)
                            {
                                throw new IncorrectBlockClose(getLine(p), BlockType.Repeat, ints[ints.Count - 1].type);
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
                                    return;
                                }
                            }

                            if (ints[ints.Count - 1].type != BlockType.Procedure)
                            {
                                throw new IncorrectBlockClose(getLine(p), BlockType.Procedure, ints[ints.Count - 1].type);
                            }

                            ints.RemoveAt(ints.Count - 1);

                            break;
                        }
                }
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
        
        private string? readLine(ref int pos)
        {
            string line = "";
            char c;

            int i = pos;
            while (i < program.Length)
            {
                c = program[i++];
                if(c == '\n' || c == '\r')
                {
                    pos = i + 1;

                    if (c == '\r' && pos < program.Length && program[pos] == '\n')
                        pos++;

                    return line;
                }
                line += c;
            }

            if(i > pos)
            {
                pos = i;
                return line;
            }

            return null;
        }
    }
}
