using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameComponents
{
    public class GameOptions
    {
        private GameOptions()
        { }

        public int CellSize { get { return 60; } }


        // Singleton//
        static GameOptions()
        {
            instance = new GameOptions();
        }

        public static GameOptions Instanse
        {
            get { return instance; }
        }

        private static GameOptions instance;
    }
}
