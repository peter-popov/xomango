using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using PlainGUI;
using PlainGUI.Controls;

namespace xo5
{
    class GameInfoPanel : PlainGUI.Controls.PanelControl
    {
        public GameInfoPanel(ScreenManager manager)
        {

            undoButton = new Button("Undo", manager.Font, Color.Black, new Vector2(15, 15));

            infoText = new TextControl("Info pannel", manager.Font, Color.Black, new Vector2(undoButton.Position.X + undoButton.Size.X + 20, 15));
            infoText.Scale = 0.65f;
            AddChild(undoButton);
            AddChild(infoText);
        }

        public event EventHandler<EventArgs> OnUndo
        {
            add
            {
                undoButton.Pressed += value;
            }
            remove
            {
                undoButton.Pressed -= value;            
            }
        }

        public bool UndoButtonEnabled
        {
            get;
            set;
        }

        public string Info
        {
            get;
            set;
        }

        public override void HandleInput(InputState input)
        {
            base.HandleInput(input);
        }

        private TextControl infoText;
        private Button undoButton;
    }
}
