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

namespace xomango
{
    public partial class GamePage : PhoneApplicationPage
    {
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Set the sharing mode of the graphics device to turn on XNA rendering
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(true);            

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(SharedGraphicsDeviceManager.Current.GraphicsDevice);

            if (NavigationContext.QueryString["resume"] == "true")
            {
                gameControler = GameControler.Load();            
            }            
            if (gameControler==null)
            {
                gameControler = new GameControler();
                gameControler.SetUpGame(PlayerType.Human, PlayerType.Machine);
            }
            gameControler.OnTurn += NewTurnDone;
            // TODO: use this.content to load your game content here
            game = new XoGame(gameControler, new Microsoft.Xna.Framework.Rectangle(0, 0, 480, 720), content);

            game.LoadContent();

            // Start the timer
            timer.Start();                        

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Stop the timer
            timer.Stop();

            // Set the sharing mode of the graphics device to turn off XNA rendering
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(false);

            gameControler.Save();
            base.OnNavigatedFrom(e);
        }      

        /// <summary>
        /// Allows the page to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        private void OnUpdate(object sender, GameTimerEventArgs e)
        {
            game.Update(new GameTime(e.TotalTime, e.ElapsedTime));
            // TODO: Add your update logic here
        }

        /// <summary>
        /// Allows the page to draw itself.
        /// </summary>
        private void OnDraw(object sender, GameTimerEventArgs e)
        {
            SharedGraphicsDeviceManager.Current.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);
            
            game.Draw(/*new GameTime(e.TotalTime, e.ElapsedTime)*/);
            // TODO: Add your drawing code here
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

        private void NewTurnDone(object sender, TurnEventArgs args)
        {
            if (gameControler.GameBoard.Winner)
            {
                if (gameControler.CurrentPlayer.Type == PlayerType.Human)
                {
                    MessageBox.Show("Congratulations! You win!");
                }
                else
                {
                    MessageBox.Show("Sorry, you lose.");                
                }
            }
        }

        private void PageOrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            //game.Orientation = ConvertPageOrientation(e.Orientation);
            switch (e.Orientation)
            {
                case PageOrientation.Portrait:
                case PageOrientation.PortraitDown:
                case PageOrientation.PortraitUp:
                    game.ScreenRectangle = new Microsoft.Xna.Framework.Rectangle(0, 0, (int)this.ActualWidth, (int)this.ActualHeight - 80);
                    break;
                default:
                    game.ScreenRectangle = new Microsoft.Xna.Framework.Rectangle(0, 0, (int)this.ActualWidth - 80, (int)this.ActualHeight);
                    break;
            }

            game.Draw();
        }
    }
}