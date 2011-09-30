using System;

using System.IO;
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameComponents
{
    public class GameStatistics
    {        
        public struct Stat
        {
            public Stat(int c, int v, int t)
            {
                Count = c;
                Victory = v;
                TurnsAmount = t;
            }
            public readonly int Count;
            public readonly int Victory;
            public readonly int TurnsAmount;        
        }


        //will load statistics from LocalStorage
        public GameStatistics()
        {
            overall = read("Overall");
            other[index(DifficultyLevel.EASY, CoreCZ.Side.Cross)] = read(key(DifficultyLevel.EASY, CoreCZ.Side.Cross));
            other[index(DifficultyLevel.EASY, CoreCZ.Side.Zero )] =  read(key(DifficultyLevel.EASY, CoreCZ.Side.Zero));
            other[index(DifficultyLevel.HARD, CoreCZ.Side.Cross)] = read(key(DifficultyLevel.HARD, CoreCZ.Side.Cross));
            other[index(DifficultyLevel.HARD, CoreCZ.Side.Zero )] =  read(key(DifficultyLevel.HARD, CoreCZ.Side.Zero));
        }

        //Save to local storage
        public void Flush()
        {
            write(overall, "Overall");
            write(other[index(DifficultyLevel.EASY, CoreCZ.Side.Cross)], key(DifficultyLevel.EASY, CoreCZ.Side.Cross));
            write(other[index(DifficultyLevel.EASY, CoreCZ.Side.Zero)], key(DifficultyLevel.EASY, CoreCZ.Side.Zero));
            write(other[index(DifficultyLevel.HARD, CoreCZ.Side.Cross)], key(DifficultyLevel.HARD, CoreCZ.Side.Cross));
            write(other[index(DifficultyLevel.HARD, CoreCZ.Side.Zero)], key(DifficultyLevel.HARD, CoreCZ.Side.Zero));
        }

        public void AddGame(bool win, DifficultyLevel level, CoreCZ.Side side, int turnsAmount)
        {
            overall = increment(overall, win, turnsAmount);
            other[index(level, side)] = increment(other[index(level, side)], win, turnsAmount);
        }

        public Stat Overall
        {
            get
            {
                return overall;
            }
        }

        public Stat this[DifficultyLevel level, CoreCZ.Side side]
        {
            get
            {
                return other[index(level, side)];
            }
        }

        public Stat this[DifficultyLevel level]
        {
            get
            {
                Stat s1 = other[index(level, CoreCZ.Side.Cross)];
                Stat s2 = other[index(level, CoreCZ.Side.Zero)];
                return new Stat(s1.Count + s2.Count, s1.Victory + s2.Victory, s1.TurnsAmount + s2.TurnsAmount);
            }
        }

        public Stat this[CoreCZ.Side side]
        {
            get
            {
                Stat s1 = other[index(DifficultyLevel.EASY, side)];
                Stat s2 = other[index(DifficultyLevel.HARD, side)];
                return new Stat(s1.Count + s2.Count, s1.Victory + s2.Victory, s1.TurnsAmount + s2.TurnsAmount);
            }
        }

        private void write(Stat s, string prefix)
        {
            settings[prefix + "Count"] = s.Count;
            settings[prefix + "Victory"] = s.Victory;
            settings[prefix + "TurnsAmount"] = s.TurnsAmount;
        }

        private Stat read(string prefix)
        {
            string countStr = prefix + "Count";
            string victoryStr = prefix + "Victory";
            string turnsAmountStr = prefix + "TurnsAmount";

            if (settings.Contains(countStr) && settings.Contains(victoryStr) && settings.Contains(turnsAmountStr))
            {
                return new Stat((int)settings[countStr], (int)settings[victoryStr], (int)settings[turnsAmountStr]);
            }
            return new Stat();
        }

        private Stat increment(Stat s, bool win, int turns)
        {
            return new Stat( s.Count + 1, s.Victory + ( win ? 1 : 0 ), s.TurnsAmount + turns );
        }

        private int index(DifficultyLevel level, CoreCZ.Side side)
        {
            int off = side == CoreCZ.Side.Cross ? 0 : 1;
            return off + (level == DifficultyLevel.EASY ? 0 : 2);
        }

        private string key(DifficultyLevel level, CoreCZ.Side side)
        {
            string off = side == CoreCZ.Side.Cross ? "Cross" : "Zero";
            return off + (level == DifficultyLevel.EASY ? "Easy" : "Hard");
        }


        private IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;            
        private Stat overall;
        private Stat[] other = new Stat[4];                        
    }
}
