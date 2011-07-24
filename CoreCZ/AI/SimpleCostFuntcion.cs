using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CoreCZ.AI
{
    public class SimpleCostFuntcion : MinMax.ICostFunction<GameState>
    {
        Random r = new Random(32);

        struct LineStatistics
        {
            public int cross;
            public int zero;
        };

        public int EvaluateState(GameState state)
        {
            int max_zero = int.MinValue;
            int max_cross = int.MinValue;

            foreach (Position p in state)
            {
                PositionInfo info = state[p];
                if (info == null || info.Side != Side.Nobody)
                {
                    continue;
                }
                //Debug.WriteLine("Checking position {0}, {1}", p.X, p.Y);

                LineStatistics st = EvaluatePosition(info);
                max_cross = Math.Max(st.cross, max_cross);
                max_zero = Math.Max(st.zero, max_zero);
            }

            //if (max_cross == WinValue) max_zero = 0;
            //if (max_zero == WinValue) max_cross = 0;

            //int res = state.Player == Side.Cross ? (int)(max_cross - 0.5 * max_zero) : (int)(max_zero - 0.5 * max_cross);

            //Debug.WriteLine(">>>>enter evaluate state");
            //Debug.WriteLine(state);
            //Debug.WriteLine("max_zero = {0}, max_cross = {1}", max_zero, max_cross);

            int res = max_zero - max_cross;
            //Debug.WriteLine("<<<<enter evaluate state");
            
            res = Math.Min(res, WinValue);
            res = Math.Max(res, LoseValue);

            //if (res == 0)
            //{
            //    Console.WriteLine("Zero state:");
            //    Console.WriteLine(state);
            //}

            return res;
        }

        private LineStatistics EvaluatePosition(PositionInfo positionInfo)
        {
            LineStatistics ls = new LineStatistics();
            ls.cross = ls.zero = 0;
            foreach (PositionInfo.Orientation o in PositionInfo.Orientations)
            {
                foreach (PositionInfo.Direction d in PositionInfo.Directions)
                {
                    PositionInfo.LineInfo li = positionInfo[o, d];
                    if (li.side == Side.Cross)
                    {
                        ls.cross = Math.Max(ls.cross, LineCost(li.amount, li.open));
                    }
                    else if (li.side == Side.Zero)
                    {
                        ls.zero = Math.Max(ls.zero, LineCost(li.amount, li.open));                    
                    }
                }
            }
            return ls;
        }

        private int LineCost(int size, bool isopen)
        {
            switch (size)
            {
                case 1:
                    return 1;
                case 2:
                    return isopen ? 4 : 2;
                case 3:
                    return isopen ? 200 : 4;
                case 4:
                    return isopen ? 4000 : 10;
                case 5:
                    return 100000;                        
            }
            //System.Diagnostics.Debug.Assert(false, "Length more than 5 detected!");
            return int.MaxValue;
        }

        public int WinValue
        {
            get {return 50000;}
        }

        public int LoseValue
        {
            get { return -50000; }
        }
    }
}
