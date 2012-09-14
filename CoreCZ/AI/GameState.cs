using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Diagnostics;

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
            this.Weight = int.MinValue;
            for (int i = 0; i < env.Length; ++i)
            {
                env[i].side = Side.Nobody;
            }
        }

        public LineInfo this[Orientation o, Direction d]
        {
            get
            {
                return env[2 * (int)o + (int)d];
            }
            set
            {
                env[2 * (int)o + (int)d] = value;
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
            this.env.CopyTo(res.env, 0);
            res.Active = this.Active;
            res.Weight = this.Weight;
            return res;
        }

        private LineInfo[] env = new LineInfo[8];
        public bool Active { get; set; }
        public Side Side { get; set; }
        public int Weight { get; set; }
        public static Orientation[] Orientations = new Orientation[4] { Orientation.Vertical, Orientation.Horizontal, Orientation.DigonalSW, Orientation.DiagonalNW };
        public static Direction[] Directions = new Direction[2] { Direction.Positive, Direction.Negative };
    }

    public class GameState : IEnumerable<Position>
    {
        #region State backup
        internal struct Change
        {
            internal Change(Position pos, PositionInfo info)
            {
                this.pos = pos;
                this.info = info;
            }
            internal readonly Position pos;
            internal readonly PositionInfo info;
        }

        public class ChangeSet
        {
            internal ChangeSet(Area a, Side player)
            {
                this.area = a;
                this.player = player;
            }

            internal void Add(Change change)
            {
                changes.Add(change);
            }

            internal void Add(Position pos, PositionInfo info)
            {
                changes.Add(new Change(pos, info));
            }

            internal IEnumerable<Change> Items
            {
                get { return changes; }
            }

            internal readonly Area area;
            internal readonly Side player;
            List<Change> changes = new List<Change>();
        }
        #endregion

        #region State mutation

        public ChangeSet Advance(Position pos, Side side)
        {
            ChangeSet backup = new ChangeSet(area, player);

            player = side;
            area = InflateArea(area, pos);

            //Update current position
            PositionInfo curentInfo = storage[pos.X, pos.Y];
            Debug.Assert(curentInfo == null || curentInfo.Side == Side.Nobody);
            backup.Add(pos, curentInfo != null ? curentInfo.Clone() : null );
            if (curentInfo == null)
            {
                curentInfo = new PositionInfo(side);
                storage[pos.X, pos.Y] = curentInfo;
            }
            else
            {
                curentInfo.Side = side;
            }
            //Update this info plus beighbiurs in all directions
            foreach (PositionInfo.Orientation o in PositionInfo.Orientations)
            {
                foreach (PositionInfo.Direction d in PositionInfo.Directions)
                {
                    updateOrientations(pos, curentInfo, o, d, backup);
                }
            }
            count++;
            return backup;
        }

        private PositionInfo.LineInfo getLineInfo(Position pos, PositionInfo.Orientation o, PositionInfo.Direction d)
        {
            PositionInfo info = storage[pos.X, pos.Y];
            if (info != null)
            {
                return info[o,d]; 
            }
            PositionInfo.LineInfo l = new PositionInfo.LineInfo();
            l.open = true;
            l.amount = 0;
            l.side = Side.Nobody;            
            return l;
        }

        private void updateOrientations(Position pos, PositionInfo info, PositionInfo.Orientation o, PositionInfo.Direction d, ChangeSet bu)
        {
            PositionInfo.LineInfo line = info[o, d];
            if (line.amount > 0 && !line.open) 
                return;
            Position direction = PositionInfo.GetDirectionVector(o, d);
            
            int shift = line.amount + 1;
            Position neighbourPos = pos + new Position(direction.X * shift, direction.Y * shift);

            PositionInfo neighbour = storage[neighbourPos.X, neighbourPos.Y];
            bu.Add(neighbourPos, neighbour != null ? neighbour.Clone() : null);

            if (neighbour == null)
            {
                neighbour = new PositionInfo(Side.Nobody);
                storage[neighbourPos.X, neighbourPos.Y] = neighbour;
            }
            Debug.Assert(neighbour.Side == Side.Nobody);
            PositionInfo.LineInfo neighbourLine = neighbour[o, PositionInfo.Flip(d)];
            Debug.Assert(neighbourLine.amount==0 || neighbourLine.open);

            if (neighbourLine.side == Side.Nobody)
            {
                PositionInfo.LineInfo opposite = info[o, PositionInfo.Flip(d)];
                neighbourLine.amount = (byte)(1 + (opposite.side == info.Side ? opposite.amount : (byte)(0)));
                neighbourLine.open = (opposite.side == info.Side && opposite.open) || opposite.side == Side.Nobody;
                neighbourLine.side = info.Side;
            }
            else if (line.side == info.Side)
            {
                //Merge
                PositionInfo.LineInfo opposite = info[o, PositionInfo.Flip(d)];
                neighbourLine.amount += (byte)(1 + (opposite.side == line.side ? opposite.amount : (byte)(0)));
                neighbourLine.open = (opposite.side == line.side && opposite.open) || opposite.side == Side.Nobody || opposite.amount == 0;
            }
            else
            {
                neighbourLine.open = false;
            }
            neighbour[o, PositionInfo.Flip(d)] = neighbourLine;
        }

        private Area InflateArea(Area a, Position p)
        {
            int x0 = Math.Min(a.sw.X, p.X - 1);
            int y0 = Math.Min(a.sw.Y, p.Y - 1);
            int x1 = Math.Max(a.ne.X, p.X + 1);
            int y1 = Math.Max(a.ne.Y, p.Y + 1);
            return new Area(new Position(x0, y0), new Position(x1, y1));
        }

        public void Undo(ChangeSet p)
        {
            foreach(Change c in p.Items)
            {                
                storage[c.pos.X, c.pos.Y] = c.info;            
            }
            area = p.area;
            player = p.player;
            count--;
        }

        public Side Player
        {
            get 
            {
                return player;
            }
        }

        public int Counter
        {
            get
            {
                return count;
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

        public override string ToString()
        {
            string s = "";
            for (int i = area.sw.X; i <= area.ne.X; i++)
            {
                for (int j = area.sw.Y; j <= area.ne.Y; j++)
                {
                    PositionInfo p = storage[i, j];
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

        #region Iteration
        public IEnumerator<Position> GetEnumerator()
        {
            for (int i = area.sw.X; i <= area.ne.X; i++)
            {
                for (int j = area.sw.Y; j <= area.ne.Y; j++)
                {
                    if (storage[i, j] != null)
                    {
                        yield return new Position(i, j);
                    }
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public PositionInfo this[Position pos]
        {
            get
            {
                return storage[pos.X, pos.Y];
            }
        }
        #endregion

        #region Private fields
        private Area area = new Area(new Position(short.MaxValue, short.MaxValue), new Position(short.MinValue, short.MinValue));
        private Storage<PositionInfo> storage = new Storage<PositionInfo>(20);
        private Side player = Side.Nobody;
        private int count = 0;        
        #endregion
    }
}
