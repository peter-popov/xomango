//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PlainGUI;
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
        public MainMenuScreen(GameControler gameControler, Rectangle gameRect, BattleScreen screen)
            : base("Crosses&Zeros")
        {
            this.gameController = gameControler;
            this.gameRectangle = gameRect;
            this.battleScreen = screen;
        }

        public override void LoadContent()
        {
            SpriteFont menuFont = ScreenManager.Game.Content.Load<SpriteFont>("MenuFont");
            ScreenManager.Font = menuFont;

            // Create our menu entries.
            MenuEntry resumeGame = new MenuEntry(menuFont, "Resume");           
            MenuEntries.Add(resumeGame);
            resumeGame.Selected += ResumeGame;
            
             // Create our menu entries.
            MenuEntry newGame = new MenuEntry(menuFont, "New game");
            newGame.Selected += NewGame;
            MenuEntries.Add(newGame);

            // Create our menu entries.
            MenuEntry settings = new MenuEntry(menuFont, "Settings");
            MenuEntries.Add(settings);

            // Create our menu entries.
            MenuEntry about = new MenuEntry(menuFont, "About");            
            MenuEntries.Add(about);

            InitPlayers();
            

            base.LoadContent();
        }

        private void InitPlayers()
        {
            gameController.SetUpGame(PlayerType.Human, PlayerType.Machine);
        }

        /// <summary>
        /// Event handler for our New Game button.
        /// </summary>
        private void NewGame(object sender, PlayerIndexEventArgs e)
        {
            gameController.Restart();
            battleScreen.Reset();            
            LoadingScreen.Load(ScreenManager, false, null, this, battleScreen);
        }

        /// <summary>
        /// Event handler for our Resume Game button.
        /// </summary>
        private void ResumeGame(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, null, this, battleScreen);
        }

        /// <summary>
        /// When the user cancels the main menu, we exit the game.
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            ScreenManager.Game.Exit();
        }

        public override void Draw(GameTime gameTime)
        {                
            base.Draw(gameTime);
        }

        BattleScreen battleScreen;
        GameControler gameController;
        Rectangle gameRectangle;
    }
}
