using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GameComponents;
using GameComponents.Control;

namespace xomango
{
    public partial class GamePage : PhoneApplicationPage
    {
        GameStatistics stat;
        ContentManager content;
        GameTimer timer;
        SpriteBatch spriteBatch;
        GameControler gameControler;
        XoGame game;


        public GamePage()
        {
            InitializeComponent();

            // Get the content manager from the application
            content = (Application.Current as App).Content;            

            // Create a timer for this page
            timer = new GameTimer();
            timer.UpdateInterval = TimeSpan.FromTicks(333333);
            timer.Update += OnUpdate;
            timer.Draw += OnDraw;
        }

        private DifficultyLevel getLevel()
        {
            if (NavigationContext.QueryString.ContainsKey("ai"))
            {
                if (NavigationContext.QueryString["ai"].ToLower() == "hard")
                {
                    return DifficultyLevel.HARD;
                }
                else if (NavigationContext.QueryString["ai"].ToLower() == "easy")
                {
                    return DifficultyLevel.EASY;
                }
                else
                {
                    System.Diagnostics.Debug.Assert(false, "Level parameter has wrong value");
                }
            }
            return DifficultyLevel.EASY;            
        }


        private void initGame()
        {
            if (gameControler == null)
            {
                gameControler = new GameControler();
                gameControler.aiLevel = getLevel();
                if (NavigationContext.QueryString["side"].ToLower() == "zero")
                {
                    gameControler.SetUpGame(PlayerType.Machine, PlayerType.Human);
                }
                else
                {
                    gameControler.SetUpGame(PlayerType.Human, PlayerType.Machine);
                }
            }

            // TODO: use this.content to load your game content here
            Microsoft.Xna.Framework.Rectangle gameRect = new Microsoft.Xna.Framework.Rectangle(0, 0, (int)ActualWidth, (int)ActualHeight );
            game = new XoGame(SharedGraphicsDeviceManager.Current.GraphicsDevice, gameControler, gameRect, content);

            game.LoadContent();
            game.TurnAnimationEvent += OnAnimationReady;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            stat = new GameStatistics();
            // Set the sharing mode of the graphics device to turn on XNA rendering
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(true);

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(SharedGraphicsDeviceManager.Current.GraphicsDevice);

            if (NavigationContext.QueryString["resume"].ToLower() == "true")
            {
                gameControler = GameControler.Load();            
            }

            initGame();

            // Start the timer
            timer.Start();

            base.OnNavigatedTo(e);

            undoButton = (Microsoft.Phone.Shell.ApplicationBarIconButton)ApplicationBar.Buttons[1];
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Stop the timer
            timer.Stop();

            // Set the sharing mode of the graphics device to turn off XNA rendering
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(false);

            gameControler.Save();
            stat.Flush();
            base.OnNavigatedFrom(e);
        }      

        /// <summary>
        /// Allows the page to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        private void OnUpdate(object sender, GameTimerEventArgs e)
        {
            game.Update(new GameTime(e.TotalTime, e.ElapsedTime));
        }

        /// <summary>
        /// Allows the page to draw itself.
        /// </summary>
        private void OnDraw(object sender, GameTimerEventArgs e)
        {
            SharedGraphicsDeviceManager.Current.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);
            
            game.Draw();
        }

        private DisplayOrientation ConvertPageOrientation(PageOrientation po)
        {
            switch (po)
            {
                case PageOrientation.Landscape:
                case PageOrientation.LandscapeLeft:
                    return DisplayOrientation.LandscapeLeft;
                case PageOrientation.LandscapeRight:
                    return DisplayOrientation.LandscapeRight;
                case PageOrientation.Portrait:
                case PageOrientation.PortraitUp:
                case PageOrientation.PortraitDown:
                    return DisplayOrientation.Portrait;
            }
            return DisplayOrientation.Default;
        }

        private void OnAnimationReady(object sender, EventArgs args)
        {
            undoButton.IsEnabled = true;
            if (gameControler.GameBoard.Winner)
            {
                bool winner = false;
                if (gameControler.CurrentPlayer.Type == PlayerType.Human)
                {
                    winner = true;
                    MessageBox.Show("Congratulations! You win!");
                }
                else
                {
                    MessageBox.Show("Sorry, you lose.");                
                }

                stat.AddGame(winner, gameControler.aiLevel, gameControler.Player1.Side, gameControler.GameBoard.Turns.Count() / 2);

                undoButton.IsEnabled = false;
            }
        }

        private void PageOrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            switch (e.Orientation)
            {
                case PageOrientation.Portrait:
                case PageOrientation.PortraitDown:
                case PageOrientation.PortraitUp:
                    game.ScreenRectangle = new Microsoft.Xna.Framework.Rectangle(0, 0, (int)this.ActualWidth, (int)this.ActualHeight);
                    break;
                default:
                    game.ScreenRectangle = new Microsoft.Xna.Framework.Rectangle(0, 0, (int)this.ActualWidth, (int)this.ActualHeight);
                    break;
            }

            game.Draw();
        }

        private void PhoneApplicationPage_LayoutUpdated(object sender, EventArgs e)
        {
            // Check for 0 because when we navigate away the LayoutUpdate event
            // is raised but ActualWidth and ActualHeight will be 0 in that case.            
            if (ActualWidth > 0 && ActualHeight > 0)
            {
                SharedGraphicsDeviceManager.Current.PreferredBackBufferWidth = (int)ActualWidth;
                SharedGraphicsDeviceManager.Current.PreferredBackBufferHeight = (int)ActualHeight;
            }
        }


        private void restartButton_Click(object sender, EventArgs e)
        {
            gameControler = null;
            initGame();
        }

        private void undoButton_Click(object sender, EventArgs e)
        {
            gameControler.Undo();            
            undoButton.IsEnabled = false;
        }
    }
}