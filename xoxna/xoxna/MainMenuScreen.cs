//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO.IsolatedStorage;
using PlainGUI;
using System;
using GameComponents.Control;

namespace xo5
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class MainMenuScreen : MenuScreen
    {
        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen(Rectangle gameRect)
            : base("Crosses&Zeros")
        {
            this.gameRectangle = gameRect;
        }

        public override void LoadContent()
        {            
            Background = ScreenManager.Game.Content.Load<Texture2D>("textures/comicpanel10");
            TitleColor = Color.Black;

            SpriteFont menuFont = ScreenManager.Game.Content.Load<SpriteFont>("MenuFont");
            ScreenManager.Font = menuFont;

            // Create our menu entries.
            resumeGame = new MenuEntry(menuFont, "Resume");
            MenuEntries.Add(resumeGame);
            resumeGame.Selected += ResumeGame;
            
             // Create our menu entries.
            MenuEntry newGame = new MenuEntry(menuFont, "New game");       
            newGame.Selected += NewGame;
            MenuEntries.Add(newGame);

            // Create our menu entries.
            MenuEntry about = new MenuEntry(menuFont, "About");            
            MenuEntries.Add(about);

            ScreenManager.OnDeactivated += Deactivated;

            base.LoadContent();
        }

        /// <summary>
        /// Event handler for our New Game button.
        /// </summary>
        public void NewGame(object sender, PlayerIndexEventArgs e)
        {
            gameControler = new GameControler();
            gameControler.SetUpGame(PlayerType.Human, PlayerType.Machine);

            BattleScreen bs = new BattleScreen(this, ScreenManager.GraphicsDevice, gameControler, gameRectangle);
            LoadingScreen.Load(ScreenManager, true, null, this, bs);
        }

        /// <summary>
        /// Event handler for our Resume Game button.
        /// </summary>
        private void ResumeGame(object sender, PlayerIndexEventArgs e)
        {
            gameControler = GameControler.Load();
            if (gameControler != null)
            {                
                BattleScreen bs = new BattleScreen(this, ScreenManager.GraphicsDevice, gameControler, gameRectangle);                
                LoadingScreen.Load(ScreenManager, true, null, this, bs);
            }
            else
            {
                NewGame(sender, e);
            }
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            resumeGame.Enabled = settings.Contains("Saved") && settings["Saved"].ToString() == true.ToString();
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public void Deactivated(object sender, EventArgs args)
        {
            if (gameControler != null && !gameControler.GameBoard.Winner)
            {
                gameControler.Save();
            }
        }

        /// <summary>
        /// When the user cancels the main menu, we exit the game.
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            ScreenManager.Game.Exit();
        }

        Rectangle gameRectangle;
        GameControler gameControler = null;
        // Create our menu entries.
        MenuEntry resumeGame;
    }
}
