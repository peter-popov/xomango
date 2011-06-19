using System;
using System.IO;
using System.Collections.Generic;

namespace CoreCZ
{
    /// <summary>
    /// 
    /// </summary>
    public class InvalidTurnException: Exception 
    {
        
    }
    
    /// <summary>
    /// 
    /// </summary>
    public struct Position
    {

        public Position(int r, int c)
        {
            row = (short)r;
            column = (short)c;
        }
     
        public Position(short r, short c)
        {
            row = r;
            column = c;
        }

        public static bool operator ==(Position a, Position b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(Position a, Position b)
        {
            return !(a == b);
        }

        public static Position operator +(Position a, Position b)
        {
            return new Position(a.row + b.row, a.column + b.column);
        }

        public static Position operator -(Position a, Position b)
        {
            return new Position(a.row - b.row, a.column - b.column);
        }


        // TODO: use byte    
        public short row;
        public short column;

        public int X { get { return row; } }
        public int Y { get { return column; } }    
    }

    /// <summary>
    /// 
    /// </summary>
    public enum Side:byte
	{
	    Cross,
        Zero,
        Nobody
	}

    public class Utils
    {
        public static Side FlipSide(Side s)
        {
            switch (s)
            {
                case Side.Nobody:
                    return Side.Nobody;
                case Side.Zero:
                    return Side.Cross;
                case Side.Cross:
                    return Side.Zero;
            }
            return Side.Nobody;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public struct Area
    {
        public Area(Position sw, int w, int h)
        {
            this.sw = sw;
            this.ne = new Position(sw.row + w, sw.column + h);
        }
        
        public Area(Position sw, Position ne)
        {
            this.sw = sw;
            this.ne = ne;
        }

        public int Width { get { return ne.X - sw.X; } }
        public int Height { get { return ne.Y - sw.Y; } }

        public readonly Position ne, sw;
    }

    /// <summary>
    /// 
    /// </summary>
    public struct Turn
    {
        public Turn(Position pos, Side s, int i)
        {
            position = pos;
            side = s;
            index = i;
        }

        public readonly Position position;
        public readonly Side side;
        public readonly int index;
    }


    /// <summary>
    /// 
    /// </summary>
    public partial class Board
    {
        /// <summary>
        /// 
        /// </summary>
        public Board()
        {
            mField = new Storage<Side>(100, Side.Nobody);
            mBoundingBox = new Area(new Position(short.MaxValue, short.MaxValue), new Position(short.MinValue, short.MinValue));
            CurrentPlayer = Side.Cross;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public Side this[Position pos]
        {
            set 
            {
                try
                {
                    Side s = mField[pos.row, pos.column];

                    if (s != Side.Nobody || value != CurrentPlayer)
                    {
                        throw new InvalidTurnException();
                    }

                    updatePosition(pos, value);
                }
                catch (IndexOutOfRangeException)
                {
                    //Special case, not a game error. reallocation may be needed
                    throw new InvalidTurnException();
                }
            }
            get 
            {
                try
                {
                    return mField[pos.row, pos.column];
                }
                catch (IndexOutOfRangeException)
                {
                    return Side.Nobody;
                }
            }
        }

        /// <summary>
        /// Removes last turn from the board.
        /// </summary>
        /// <returns>return true if unod was done properly</returns>
        public bool Undo()
        {
            if (mTurns.Count == 0)
            {
                return false;
            }

            Turn lastTurn = mTurns[mTurns.Count - 1];
            mField[lastTurn.position.row, lastTurn.position.column] = Side.Nobody;
            CurrentPlayer = lastTurn.side;

            mTurns.RemoveAt(mTurns.Count - 1);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public Area BoundingBox
        {
            get { return mBoundingBox; }
        }


        /// <summary>
        /// 
        /// </summary>
        public Side CurrentPlayer
        {
            get;
            private set;
        }


        /// <summary>
        /// 
        /// </summary>
        public Position LastPosition
        {
            get 
            {
                if (mTurns.Count > 0)
                {
                    return mTurns[mTurns.Count - 1].position;
                }
                else
                {
                    return new Position(0, 0);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Winner
        {
            get { return mWin; }
        }


        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<Turn> Turns
        {
            get { return mTurns; }
        }


        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            mTurns.Clear();
            mField.Fill(Side.Nobody);
            mBoundingBox = new Area(new Position(short.MaxValue, short.MaxValue), new Position(short.MinValue, short.MinValue));
            CurrentPlayer = Side.Cross;
            mWin = false;
        }

#region Serialization
        private void writePos(Position p, BinaryWriter bw)
        {
            bw.Write(p.row);
            bw.Write(p.column);            
        }

        private Position readPos(BinaryReader br)
        {
            short row = br.ReadInt16();
            short col = br.ReadInt16();
            return new Position(row, col);
        }


        public void Serialize(BinaryWriter bw)
        {
            //Simply write all turns
            bw.Write(mTurns.Count);
            foreach(Turn turn in mTurns)
            {
                writePos(turn.position, bw);
                bw.Write((int)turn.side);
            }
        }

        public void Deserialize(BinaryReader br)
        {
            Clear();
            // read and "make" all turns
            int count = br.ReadInt32();
            for (int i = 0; i < count; ++i)
            {
                Position p = readPos(br);
                Side s = (Side)(br.ReadInt32());
                this[p] = s;
            }
        }
#endregion
    }

    #region Board Implementations
    public partial class Board
    {
        private void updatePosition(Position pos, Side val)
        {
            mField[pos.row, pos.column] = val;
            CurrentPlayer = val == Side.Cross ? Side.Zero : Side.Cross;
            mTurns.Add(new Turn(pos, val, mTurns.Count));
            mBoundingBox = join(mBoundingBox, pos);
            mWin = checkTurnForVictory(pos);
        }

        private bool checkTurnForVictory(Position pos)
        {
            if (this[pos] == Side.Nobody)
            {
                return false;
            }

            Position[] dirs = { new Position(-1, 1), new Position(0, 1), new Position(1, 1), new Position(1, 0) };

            
            foreach (Position dp in dirs)
            {
                int count = 1;
                Position p = pos + dp;
                while (this[p] == this[pos] && count < WinCount)
                {
                    count++;
                    p += dp;
                }

                p = pos - dp;
                while (this[p] == this[pos] && count < WinCount)
                {
                    count++;
                    p -= dp;
                }

                if (count == WinCount) return true;
            }

            return false;
        }

        private static Area join(Area a, Position p)
        {
            return new Area( new Position( Math.Min( a.sw.row, p.row ), Math.Min( a.sw.column, p.column )),
                             new Position( Math.Max( a.ne.row, p.row ), Math.Max( a.ne.column, p.column )) );
        }        

        private Area mBoundingBox;
        private List<Turn> mTurns = new List<Turn>();
        private Storage<Side> mField;
        private bool mWin = false;
        private const int WinCount = 5;
    }
    #endregion
}
