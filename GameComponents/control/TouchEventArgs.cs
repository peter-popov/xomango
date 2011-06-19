using System;
using Microsoft.Xna.Framework.Input.Touch;
using CoreCZ;

namespace GameComponents.Control
{
    public class TouchEventArgs : EventArgs
    {
        public TouchEventArgs(Position pos)
        {
            Position = pos;
        }
        public readonly Position Position;
    }
}
