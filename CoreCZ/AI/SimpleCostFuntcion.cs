using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;


namespace CoreCZ.AI
{
    public class SimpleCostFuntcion : MinMax.ICostFunction<GameState>
    {
        private Side winPlayer;

        public static Action startProfiling;
        public static Action endProfiling;
               
        private Dictionary<GameState, int> cache = new Dictionary<GameState, int>();
        public static int calls = 0;
        public static int cache_hits = 0;
        

        public SimpleCostFuntcion(Side winPlayer)
        {
            this.winPlayer = winPlayer;
        }

        class LineStatistics
        {
            public int cross;
            public int zero;

            public int this[Side side]
            {
                get
                {
                    return side == Side.Cross ? cross : zero;
                }
                set
                {
                    if (side == Side.Cross) cross = value;
                    else if (side == Side.Zero) zero = value;
                }
            }
        };

        public int EvaluateState(GameState state)
        {
            //startProfiling();
            int res = 0;
            //calls++;
            //if (cache.TryGetValue(state, out res))
            //{
            //    cache_hits++;
            //    //endProfiling();
            //    return res;
            //}
            res = EvaluateStateImpl(state);
            //cache[state] = res;
            //endProfiling();
            return res;
        }

        private LineStatistics st = new LineStatistics();

        private int EvaluateStateImpl(GameState state)
        {            
            int max_zero = 0; //int.MinValue;
            int max_cross = 0;// int.MinValue;
            
            foreach (var p in state)
            {
                PositionInfo info = p.Info;
                if (info == null || info.Side != Side.Nobody)
                {
                    continue;
                }

                EvaluatePosition(info, st);

                max_cross += st.cross;
                max_zero += st.zero;
            }
            var res = (winPlayer == Side.Zero) ? (max_zero - (int)(0.8 * max_cross)) : (max_cross - (int)(0.8 * max_zero));
            res = Math.Max(Math.Min(res, WinValue), LoseValue);            
            return res;
        }

        private LineStatistics EvaluatePosition(PositionInfo positionInfo, LineStatistics ls)
        {
            ls.cross = ls.zero = 0;
            foreach (var o in PositionInfo.Orientations)
            {

                var liPos = positionInfo[o, PositionInfo.Direction.Positive];
                var liNeg = positionInfo[o, PositionInfo.Direction.Negative];

                if (liPos.side == liNeg.side)
                {
                    ls[liPos.side] = PatternCost(liPos.amount, liPos.open, liNeg.amount, liNeg.open);
                }
                else
                {
                    ls[liPos.side] = LineCost(liPos.amount, liPos.open);
                    ls[liNeg.side] = LineCost(liNeg.amount, liNeg.open);
                }
            }
            return ls;
        }        

        static private int LineCost(int size, bool isopen)
        {
            switch (size)
            {
                case 1:
                    return 1;
                case 2:
                    return isopen ? 4 : 2;
                case 3:
                    return isopen ? 300 : 3;
                case 4:
                    return isopen ? 4000 : 10;
                case 5:
                    return 100000;                        
            }
            return int.MaxValue;
        }

        static private int PatternCost(int size1, bool isopen1, int size2, bool isopen2)
        {
            return LineCost(size1 + size2, isopen1 && isopen2);
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
