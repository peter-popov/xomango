using System;
using Microsoft.Xna.Framework.Input.Touch;

namespace xomango.control
{
    public class TouchEventArgs : EventArgs
    {
        public TouchEventArgs(GestureSample gs)
        {
            this.gs = gs;
        }
        public readonly GestureSample gs;
    }
}
