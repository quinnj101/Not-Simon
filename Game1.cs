using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Timers;


namespace Simon
{
    public enum SimonColors {GREEN, RED, YELLOW, BLUE, NONE};
    public enum Turn { PLAYER, COMPUTER, PLAYBACK, GAMEOVER};

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D board;
        Texture2D simon;
        Texture2D cursor;
        Random rand;
        bool canclick = true;
        int level = 1;
        int a;
        Turn turn = Turn.COMPUTER;
        Timer t = new Timer(750);

        List<SimonColors> moves, player; // Hint
        int PlayBackIndex = 0;  // Index into moves list
        int PlayerTurnIndex = 0; // When it's the player's turn, you can use this to store what move the player is on

        SimonColors Lit = SimonColors.NONE;  // Which button is currently lit up?
        
        

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            graphics.ApplyChanges();

            Content.RootDirectory = "Content";

            rand = new Random(System.Environment.TickCount);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            board = Content.Load<Texture2D>("board");
            simon = Content.Load<Texture2D>("simon");
            cursor = Content.Load<Texture2D>("cursor");

            SoundManager.Initialize(Content);

            moves = new List<SimonColors>();
            player = new List<SimonColors>();

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            

            // TODO: Add your update logic here

            if (turn == Turn.COMPUTER)
            {
                /*for (int i = 0; true; i++)
                {
                    timer++;
                    if (timer == 45)
                    {
                        break;
                    }
                }
                 */

          
                while (moves.Count <= level) 
                {
                    
                    moves.Add((SimonColors)rand.Next(0, 4));
                    Lit = moves[PlayBackIndex];
                    PlayBackIndex++;
                        
                    
                }
                PlayBackIndex = 0;
                turn = Turn.PLAYBACK;
            }
            //TODO: After 1 second add a random move

            else if (turn == Turn.PLAYBACK)
            {
                Lit = SimonColors.NONE;
                while(PlayBackIndex < moves.Count)
                {
                   
                        Lit = moves[PlayBackIndex];
                        
                        PlayBackIndex++;
                        
                }
                // TODO: Play one move every 750ms.. 
                // DO NOT PLAY BACK ALL MOVES AT ONCE

                // If PlayBackIndex == moves.Count then turn = Turn.PLAYER (and set PlayerTurnIndex to 0)
                if (PlayBackIndex == moves.Count)
                {
                    turn = Turn.PLAYER;
                    PlayerTurnIndex = 0;
                }
            }
            else if (turn == Turn.PLAYER)
            {
                MouseState ms = Mouse.GetState();

                if (ms.LeftButton == ButtonState.Pressed && canclick)
                {
                    canclick = false;
                    
                    Lit = getPressed();
                    if (Lit == SimonColors.GREEN)
                    {
                        player.Add(SimonColors.GREEN);
                    }

                    if (Lit == SimonColors.RED)
                    {
                        player.Add(SimonColors.RED);
                    }

                    if (Lit == SimonColors.YELLOW)
                    {
                        player.Add(SimonColors.YELLOW);
                    }

                    if (Lit == SimonColors.BLUE)
                    {
                        player.Add(SimonColors.BLUE);
                    }

                  

                    if (Lit != SimonColors.NONE)
                    {
                        Lit = player[PlayerTurnIndex];
                        while (PlayerTurnIndex < (player.Count - 1))
                        {
                            PlayerTurnIndex++;
                        }
                    }


                    if (PlayerTurnIndex > moves.Count)
                    {
                        turn = Turn.GAMEOVER;
                    }
                   

                    // Check to see if green button is hit.. add code to make sure the mouse button is depressed so you
                    // don't respond to this buttonpress twice in a row

                    if (Lit != SimonColors.NONE)
                    {

                        if (Lit == moves[PlayerTurnIndex])
                        {

                            
                            PlayBackIndex += 1;
                            if (PlayerTurnIndex <= moves.Count)
                            {
                                level++;
                                player.Clear();
                                PlayerTurnIndex = 0;
                                moves.Clear();
                                PlayBackIndex = 0;
                                turn = Turn.COMPUTER;
                            }
                            else
                            {
                                turn = Turn.GAMEOVER;
                            }
 

                        }



                       
                        // do something here!  Maybe see if Lit was the correct button to press?

                        SoundManager.PlaySimonSound(Lit);
                    }
                }
                if (ms.LeftButton != ButtonState.Pressed)
                {
                    canclick = true;
                }
            }
            else if (turn == Turn.GAMEOVER)
            {
                SoundManager.PlayGameOver();

                moves.Clear();
                player.Clear();
                PlayerTurnIndex = 0;
                moves.Clear();
                PlayBackIndex = 0;
                turn = Turn.COMPUTER;
                Lit = SimonColors.NONE;
            }

            base.Update(gameTime);
        }


        // point is the on-screen mouse coordinate where a click occurred
        // destination is a rectangle where this sprite will be drawn
        // source is a rectangle from the simon spritesheet
        // 
        // Returns TRUE if the mouse click was on a non-transparent pixel
        public bool isPressed(Texture2D texture, Rectangle destination, Rectangle source)
        {
            MouseState ms = Mouse.GetState();
            Vector2 point = new Vector2(ms.X, ms.Y);
            uint[] PixelData = new uint[texture.Width * texture.Height];

            texture.GetData<uint>(0, new Rectangle(0,0,texture.Width,texture.Height), PixelData, 0, texture.Width*texture.Height);

            Vector2 point_translated = point - new Vector2(destination.X, destination.Y);

            if (point_translated.X >= 0 && point_translated.X < source.Width && point_translated.Y >= 0 && point_translated.Y < source.Height)
            {
                int offset = ((int)point_translated.Y + source.Y) * texture.Width + (int)point_translated.X + source.X;

                if (PixelData[offset] != 0)
                    return true;
            }

            return false;
        }

        public SimonColors getPressed()
        {
            if (isPressed(simon, new Rectangle(46, 40, 238, 243), new Rectangle(0, 0, 238, 243)))
            {
                return SimonColors.GREEN;
            }

            // RED
            else if (isPressed(simon, new Rectangle(46 + 277, 40, 238, 243), new Rectangle(277, 0, 238, 243)))
            {
                return SimonColors.RED;
            }


            // YELLOW
            else if (isPressed(simon, new Rectangle(46, 40 + 276, 238, 243), new Rectangle(0, 276, 238, 243)))
            {
                return SimonColors.YELLOW;
            }


            // BLUE
            else if (isPressed(simon, new Rectangle(46 + 277, 40 + 276, 238, 243), new Rectangle(277, 276, 238, 243)))
            {
                return SimonColors.BLUE;
            }
            else
            {
            return SimonColors.NONE;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            MouseState ms = Mouse.GetState();

            spriteBatch.Begin();

                spriteBatch.Draw(board, new Rectangle(0, 0, 800, 600), Color.White);

                // Maybe we shouldn't draw all the highlights?   Just the "Lit" one perhaps?   But here's the code if you want to..

                // Draw green hightlight (note that this shouldn't ALWAYS been drawn)
                if (Lit == SimonColors.GREEN)
                    spriteBatch.Draw(simon, new Rectangle(46, 40, 238, 243), new Rectangle(0, 0, 238, 243), Color.White);

                // Draw red hightlight (note that this shouldn't ALWAYS been drawn)
                if (Lit == SimonColors.RED)
                    spriteBatch.Draw(simon, new Rectangle(46 + 277, 40, 238, 243), new Rectangle(277, 0, 238, 243), Color.White);

                // Draw yellow hightlight (note that this shouldn't ALWAYS been drawn)
                if (Lit == SimonColors.YELLOW)
                    spriteBatch.Draw(simon, new Rectangle(46, 40 + 276, 238, 243), new Rectangle(0, 276, 238, 243), Color.White);

                // Draw blue hightlight (note that this shouldn't ALWAYS been drawn)
                if (Lit == SimonColors.BLUE)
                    spriteBatch.Draw(simon, new Rectangle(46 + 277, 40 + 276, 238, 243), new Rectangle(277, 276, 238, 243), Color.White);

                // Draw cursor
                spriteBatch.Draw(cursor, new Vector2(ms.X, ms.Y), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
   
}
