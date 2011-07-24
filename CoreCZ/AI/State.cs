using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreCZ.AI
{

    public sealed class PositionInfo
    {
        public struct LineInfo
        {
            public Side side;
            public byte amount;
            public bool open;
        }

        public enum Orientation : int
        {
            Vertical = 0,
            Horizontal = 1,
            DigonalSW = 2,
            DiagonalNW = 3
        }

        public enum Direction : int
        {
            Positive = 0,
            Negative = 1
        }

        public PositionInfo(Side s)
        {
            this.Side = s;
            for (int i = 0; i < env.Length; ++i)
            {
                env[i].side = Side.Nobody;
            }
        }

        public LineInfo this[Orientation o, Direction d]
        {
            get
            {
                return env[2*(int)o + (int)d];
            }
            set
            {
                env[2*(int)o + (int)d] = value;
            }
        }

        public static Position GetDirectionVector(Orientation o, Direction d)
        {
            Position p = new Position(0, 0);
            if (o == Orientation.Vertical) p.column = 1;
            if (o == Orientation.Horizontal) p.row = 1;
            if (o == Orientation.DigonalSW) p.row = p.column = 1;
            if (o == Orientation.DiagonalNW) { p.row = 1; p.column = -1; }

            if (d == Direction.Negative)
            {
                p.row = (short)(-p.row);
                p.column = (short)(-p.column);             
            }
            return p;
        }

        public static Direction Flip(Direction d)
        {
            if (d == Direction.Positive) return Direction.Negative;
            return Direction.Positive;
        }

        public PositionInfo Clone()
        {
            PositionInfo res = new PositionInfo(this.Side);
            for (int i = 0; i < 8; ++i)
            {
                res.env[i] = this.env[i];
            }
            res.Active = this.Active;
            return res;
        }

        private LineInfo[] env = new LineInfo[8]; 
        public bool Active {get; set;}
        public Side Side {get; set;}
        public static Orientation[] Orientations = new Orientation[4] { Orientation.Vertical, Orientation.Horizontal, Orientation.DigonalSW, Orientation.DiagonalNW };
        public static Direction[] Directions = new Direction[2] { Direction.Positive, Direction.Negative };    
    }


    public sealed partial class State : IEnumerable<Position>
    {
        #region Constraction
        public State(Position pos, Side s)
        {
            area = new Area(new Position(pos.X - 2, pos.Y - 2), 5, 5);
            data = new PositionInfo[area.Width, area.Height];
            this[pos] = new PositionInfo(s);
            this.side = Utils.FlipSide(s);
            PopulateAroundPosition(pos, true, 1);
            generation = 1;
        }

        private State(Area area)
        {
            this.area = area;
            data = new PositionInfo[area.Width, area.Height];
        }
        #endregion

        #region Public interface
        public Side Player
        {
            get { return side; }
        }                

        public IEnumerator<Position> GetEnumerator()
        {
            for (int i = 0; i <= data.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= data.GetUpperBound(1); j++)
                {
                    Position pos = new Position(area.sw.X + i, area.sw.Y + j);
                    if (this[pos] != null)
                    {
                        yield return pos;
                    }
                }
            }
        }
     
        public PositionInfo this[Position pos]
        {
            get
            {
                if (pos.X - area.sw.X < 0 || pos.X - area.sw.X >= area.Width) return null;
                if (pos.Y - area.sw.Y < 0 || pos.Y - area.sw.Y >= area.Height) return null;

                return data[pos.X - area.sw.X, pos.Y - area.sw.Y];
            }
            private set
            {
                data[pos.X - area.sw.X, pos.Y - area.sw.Y] = value;
            }
        }

        private int getMaxLine(PositionInfo info)
        {
            int max = int.MinValue;
                if (info == null || info.Side != Side.Nobody)
                {
                    return 0;
                }

                foreach (PositionInfo.Orientation o in PositionInfo.Orientations)
                {
                    foreach (PositionInfo.Direction d in PositionInfo.Directions)
                    {
                        PositionInfo.LineInfo li = info[o, d];
                        if (li.amount > max) max = li.amount;
                    }
                }
            return max;
        }

        public int Generation
        {
            get { return generation; }
        }

        public override string ToString()
        {
            string s = "";
            for (int i = 0; i <= data.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= data.GetUpperBound(1); j++)
                {
                    PositionInfo p = data[i, j];
                    if (p != null && p.Side == Side.Cross)
                    {
                        s += "x";
                    }
                    else if (p != null && p.Side == Side.Zero)
                    {
                        s += "o";
                    }
                    else
                    {
                        int k = getMaxLine(p);
                        if (k > 0)
                            s += string.Format("{0}", k);
                        else
                            s += "_";
                    }
                }
                s += "\n";
            }
            return s;
        }
        #endregion

        #region Derive state
        public State DeriveState(Position pos)
        {
            State result = new State(InflateArea(this.area, pos));
            result.generation = this.generation + 1;

            foreach (Position p in this)
            {
                result[p] = this[p].Clone();
            }

            result[pos] = new PositionInfo(this.side);
            result.side = Utils.FlipSide(this.side);

            result.UpdateLines(pos);
            result.PopulateAroundPosition(pos, true, 2);
                                                   
            return result;
        }
        #endregion

        #region Private helpers

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private Position Transalte(int i, int j)
        {
            return new Position(area.sw.X + i, area.sw.Y + j);
        }

        private Area InflateArea(Area a, Position p)
        {
            int x0 = Math.Min(a.sw.X, p.X - 2);
            int y0 = Math.Min(a.sw.Y, p.Y - 2);
            int x1 = Math.Max(a.ne.X, p.X + 2 + 1);
            int y1 = Math.Max(a.ne.Y, p.Y + 2 + 1);
            return new Area(new Position(x0, y0), new Position(x1, y1));
        }
        #endregion

        #region Serialization

        public void Serialize(BinaryWriter bw)
        {
            
        }

        public void Deserialize(BinaryReader bw)
        {

        }
        #endregion

        #region Fields
        private Side side;
        private Area area;
        private PositionInfo[,] data;
        private int generation;
        #endregion
    }


    partial class State
    {
        private void UpdateLines(Position pos)
        {
            System.Diagnostics.Debug.Assert(this[pos] != null, "Can't update from empty position");
            System.Diagnostics.Debug.Assert(this[pos].Side != Side.Nobody, "Can't update from empty position");
            // Move two steps to each duration and calculate info
            foreach (PositionInfo.Orientation o in PositionInfo.Orientations)
            {
                foreach (PositionInfo.Direction d in PositionInfo.Directions)
                {
                    Position delta = PositionInfo.GetDirectionVector(o, d);
                    Position p = pos;
                    Side prev = Side.Nobody;
                    for (int i = 0; i < 5; ++i)
                    {
                        p = p + delta;
                        if (this[p] == null || this[p].Side == Side.Nobody)
                        {
                            if (i >= 2)
                            {
                                PopulateTurnInfo(p);
                            }
                            break;
                        }

                        if (this[p].Side == prev || prev == Side.Nobody)
                        {
                            prev = this[p].Side;
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }

        private void PopulateAroundPosition(Position pos, bool force, int dist)
        {
            System.Diagnostics.Debug.Assert(this[pos] != null, "Can't populate from empty position");            
            System.Diagnostics.Debug.Assert(this[pos].Side != Side.Nobody, "Can't populate from empty position");
            // Move two steps to each duration and calculate info
            foreach (PositionInfo.Orientation o in PositionInfo.Orientations)
            {
                foreach (PositionInfo.Direction d in PositionInfo.Directions)
                {
                    Position delta = PositionInfo.GetDirectionVector(o, d);
                    Position p = pos;

                    for (int i = 0; i < dist; ++i)
                    {
                        p = p + delta;
                        if (this[p] != null && ( this[p].Side != Side.Nobody || this[p].Active && !force ) )
                        {
                            continue;
                        }
                        PopulateTurnInfo(p);
                    }
                }
            }
        }

        private void PopulateTurnInfo(Position turnPos)
        {
            PositionInfo turnInfo = this[turnPos];
            if (turnInfo == null)
            {
                turnInfo = new PositionInfo(Side.Nobody);
            }

            System.Diagnostics.Debug.Assert(turnInfo.Side == Side.Nobody, "Trying to populate alredy occupied cell");
            
            foreach (PositionInfo.Orientation o in PositionInfo.Orientations)
            {
                foreach (PositionInfo.Direction d in PositionInfo.Directions)
                {
                    Position delta = PositionInfo.GetDirectionVector(o, d);
                    PositionInfo.LineInfo lineInfo = new PositionInfo.LineInfo();
                    lineInfo.open = true;
                    lineInfo.amount = 0;
                    lineInfo.side = Side.Nobody;
                    Position p = turnPos + delta;
                    if (this[p] != null) lineInfo.side = this[p].Side;

                    while (lineInfo.open)
                    {
                        if (this[p] == null || this[p].Side == Side.Nobody)
                        {
                            break;
                        }
                        if (this[p].Side == lineInfo.side)
                        {
                            lineInfo.amount++;
                        }
                        else
                        {
                            lineInfo.open = false;
                        }
                        p = p + delta;
                    }

                    System.Diagnostics.Debug.Assert(lineInfo.amount > 0 || lineInfo.side == Side.Nobody);
                    turnInfo[o, d] = lineInfo;                    
                }
            }

            turnInfo.Active = true;

            this[turnPos] = turnInfo;
        }
    }

    
}
