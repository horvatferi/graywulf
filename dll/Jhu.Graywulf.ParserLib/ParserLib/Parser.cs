﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.ParserLib
{
    public abstract class Parser
    {
        struct ParserCheckpoint
        {
            public int Pos;     // Position
            public int Count;   // Token count

            public ParserCheckpoint(int pos, int count)
            {
                Pos = pos;
                Count = count;
            }
        }

        private int pos;

        private Stack<ParserCheckpoint> checkpoints;
        private string code;
        private List<int> lineStarts;

        internal int Pos
        {
            get { return pos; }
        }

        internal string Code
        {
            get { return code; }
        }

        public abstract StringComparer Comparer { get; }

        public abstract HashSet<string> Keywords { get; }

        public abstract Token Execute(string code);

        public Token Execute(Token rootToken, string code)
        {
            this.pos = 0;
            this.checkpoints = new Stack<ParserCheckpoint>();
            this.code = code;
            FindLines();

            if (rootToken.Match(this) && pos == code.Length)
            {
                if (rootToken is Node)
                {
                    return ((Node)rootToken).Interpret();
                }
                else
                {
                    return rootToken;
                }
            }
            else
            {
                int p, line, col;
                GetLineCol(out p, out line, out col);
                var ex = new ParserException(String.Format(ExceptionMessages.NotUnderstandableToken, line + 1, col + 1));
                ex.Pos = p; ex.Line = line; ex.Col = col;
                throw ex;
            }
        }

        internal void Advance(int count)
        {
            pos += count;
        }

        /// <summary>
        /// Creates a checkpoint.
        /// </summary>
        /// <param name="position"></param>
        internal void Checkpoint(int count)
        {
            checkpoints.Push(new ParserCheckpoint(pos, count));
        }

        /// <summary>
        /// Rolls the parsing back to the last checkpoint
        /// </summary>
        /// <param name="position"></param>
        internal int Rollback()
        {
            var cp = checkpoints.Pop();
            pos = cp.Pos;
            return cp.Count;
        }

        /// <summary>
        /// Commits the last checkpoint
        /// </summary>
        /// <param name="position"></param>
        internal void Commit()
        {
            checkpoints.Pop();
        }

        /// <summary>
        /// Finds line starting positions in the code
        /// </summary>
        private void FindLines()
        {
            lineStarts = new List<int>();

            lineStarts.Add(0);

            int i = 0;
            while ((i = code.IndexOf(Environment.NewLine, i)) >= 0)
            {
                i += Environment.NewLine.Length;
                lineStarts.Add(i);
            }
        }

        /// <summary>
        /// Calculates the line and column corresponding to an absolute
        /// position within the code string
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="line"></param>
        /// <param name="col"></param>
        internal void GetLineCol(out int pos, out int line, out int col)
        {
            pos = this.pos;

            line = -1;
            foreach (var i in lineStarts)
            {
                if (i > pos)
                {
                    break;
                }
                line++;
            }

            col = pos - lineStarts[line];
        }
    }
}
