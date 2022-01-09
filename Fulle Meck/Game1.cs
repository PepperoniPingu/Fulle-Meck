/*
 * Fulle Meck by Hugo Frisk 2021
 * 
 * Wordlist:
 * - Stencils are the outlines for where the player can place components on the rocket
 * - Molijox a.k.a. component
 *
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Fulle_Meck
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public static GameWindow window;

        public static SpriteFont gameFont;

        public static MouseState mouse;
        public static KeyboardState keyboard;

        public static SoundEffect snapSound;
        public static SoundEffectInstance snapSoundInstance;
        public static SoundEffect motorSound;
        public static SoundEffectInstance motorSoundInstance;

        // Enumerate the game states/modes
        public enum GameStates
        {
            menu, build, launch, levelSelect, shootemUp, shootemUpResult
        }
        // Current game state
        public static GameStates gameState;

        // Array for which levels are unlocked
        public static Unlocked[] levelsUnlocked = { new Unlocked(true), new Unlocked(false), new Unlocked(false)};

        // List for all components
        public static List<Molijox> molijoxer = new List<Molijox>();

        // Variable for if the mouse buton is pressed
        private static bool pressed;
        // Method to identify when the left mousebutton was clicked, 
        public static bool mouseRealesedEvent(MouseState _mouse) {
            if (_mouse.LeftButton == ButtonState.Released)
            {
                if (pressed == true)
                {
                    pressed = false;
                    return true;
                } else
                {
                    return false;
                }
            } else
            {
                pressed = true;
                return false;
            }
        }

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            window = Window;

            // Configure screen, 1080p fullscreen
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();

            gameState = GameStates.menu;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            gameFont = Content.Load<SpriteFont>("Printing/gameFont");

            // Load the rocket motor
            molijoxer.Add(new Molijox(
                new Rectangle(100, 100, 32, 32),
                Content.Load<Texture2D>("Sprites/Raket2x"),
                Content.Load<Texture2D>("Sprites/Raket8x"),
                Molijox.Type.motor, 
                0.002f, 
                0.7f, 
                false
                ));

            // Load the bad rocket motor
            molijoxer.Add(new Molijox(
                new Rectangle(100, 100, 32, 32),
                Content.Load<Texture2D>("Sprites/TrattMotor2x"),
                Content.Load<Texture2D>("Sprites/TrattMotor8x"),
                Molijox.Type.motor,
                0.0035f,
                0.55f,
                true
                ));

            // Load the rusty tank
            molijoxer.Add(new Molijox(
                new Rectangle(400, 100, 32, 32),
                Content.Load<Texture2D>("Sprites/RostigTank2x"),
                Content.Load<Texture2D>("Sprites/RostigTank8x"),
                Molijox.Type.tank,
                0f, 
                1.8f, 
                false
                ));

            // Load the barrel tank
            molijoxer.Add(new Molijox(
                new Rectangle(400, 100, 32, 32),
                Content.Load<Texture2D>("Sprites/Barrel2x"),
                Content.Load<Texture2D>("Sprites/Barrel8x"),
                Molijox.Type.tank,
                0f,
                1f, 
                true
                ));

            // Load the top capsule
            molijoxer.Add(new Molijox(
                new Rectangle(800, 100, 32, 32),
                Content.Load<Texture2D>("Sprites/Capsule2x"), 
                Content.Load<Texture2D>("Sprites/CapsuleMulle2x"),
                Content.Load<Texture2D>("Sprites/Capsule8x"),
                Molijox.Type.top,
                0.001f,
                0.1f, 
                false
                ));

            // Load the top chair
            molijoxer.Add(new Molijox(
                new Rectangle(800, 100, 32, 32),
                Content.Load<Texture2D>("Sprites/Chair2x"), 
                Content.Load<Texture2D>("Sprites/ChairMulle2x"),
                Content.Load<Texture2D>("Sprites/Chair8x"),
                Molijox.Type.top,
                0.0007f,
                0.05f,
                true
                ));

            // Load and design the individual menus

            List<Menu.Element> _elements = new List<Menu.Element>();

            // Create the menu showed on start up
            _elements.Add(new Menu.Element(
                new Rectangle(window.ClientBounds.Width / 2, window.ClientBounds.Height / 2, 300, 100),
                "Start",
                Menu.Element.Type.button,
                new Menu.Element.handler(Building.initialize),
                Color.GreenYellow
                ));
            _elements.Add(new Menu.Element(
                new Rectangle(150, 100, 200, 100),
                "Exit",
                Menu.Element.Type.button,
                new Menu.Element.handler(Exit),
                Color.Red
                ));
            menus.createMenu("start", _elements);
            _elements.Clear();

            // Create the menu showed during building
            _elements.Add(new Menu.Element(
                new Rectangle(150, 100, 200, 100),
                "Menu",
                Menu.Element.Type.button,
                new Menu.Element.handler(goToStart),
                Color.Red
                ));
            _elements.Add(new Menu.Element(
                new Rectangle(window.ClientBounds.Width - 150, 100, 200, 100),
                "Launch",
                Menu.Element.Type.button,
                new Menu.Element.handler(launch),
                Color.GreenYellow
                ));
            menus.createMenu("building", _elements);
            _elements.Clear();

            // Create the menu showed during flying
            _elements.Add(new Menu.Element(
                new Rectangle(150, 100, 200, 100),
                "Menu",
                Menu.Element.Type.button,
                new Menu.Element.handler(goToStart),
                Color.Red
                ));
            _elements.Add(new Menu.Element(
                new Rectangle(window.ClientBounds.Width - 150, 100, 200, 100),
                "Build",
                Menu.Element.Type.button,
                new Menu.Element.handler(Building.initialize),
                Color.GreenYellow
                ));
            menus.createMenu("flying", _elements);
            _elements.Clear();

            // Create the menu showed during game over
            _elements.Add(new Menu.Element(
                new Rectangle(150, 100, 200, 100),
                "Menu",
                Menu.Element.Type.button,
                new Menu.Element.handler(goToStart),
                Color.Red
                ));
            _elements.Add(new Menu.Element(
                new Vector2(window.ClientBounds.Width / 2, window.ClientBounds.Height / 2 - 150),
                "Game Over :(",
                Menu.Element.Type.text
                ));
            _elements.Add(new Menu.Element(
                new Rectangle(window.ClientBounds.Width / 2, window.ClientBounds.Height / 2, 400, 100),
                "Return to building",
                Menu.Element.Type.button,
                new Menu.Element.handler(Building.initialize),
                Color.Red
                ));
            menus.createMenu("gameOver", _elements);
            _elements.Clear();

            // Create the menu showed when level was cleared
            _elements.Add(new Menu.Element(
                new Rectangle(150, 100, 200, 100),
                "Menu",
                Menu.Element.Type.button,
                new Menu.Element.handler(goToStart),
                Color.Red
                ));
            _elements.Add(new Menu.Element(
                new Vector2(window.ClientBounds.Width / 2, window.ClientBounds.Height / 2 - 150),
                "Level cleared!",
                Menu.Element.Type.text
                ));
            _elements.Add(new Menu.Element(
                new Rectangle(window.ClientBounds.Width / 2, window.ClientBounds.Height / 2, 400, 100),
                "Return to building",
                Menu.Element.Type.button,
                new Menu.Element.handler(Building.initialize),
                Color.GreenYellow
                ));
            menus.createMenu("levelCleared", _elements);
            _elements.Clear();

            // Create the menu showed when level was cleared and molijox unlocked
            _elements.Add(new Menu.Element(
                new Rectangle(150, 100, 200, 100),
                "Menu",
                Menu.Element.Type.button,
                new Menu.Element.handler(goToStart),
                Color.Red
                ));
            _elements.Add(new Menu.Element(
                new Vector2(window.ClientBounds.Width / 2, window.ClientBounds.Height / 2 - 200),
                "Level cleared! Unlocked:",
                Menu.Element.Type.text
                ));
            _elements.Add(new Menu.Element(
                new Rectangle(window.ClientBounds.Width / 2, window.ClientBounds.Height / 2 + 300, 400, 100),
                "Return to building",
                Menu.Element.Type.button,
                new Menu.Element.handler(Building.initialize),
                Color.GreenYellow
                ));
            menus.createMenu("levelClearedMoj", _elements);
            _elements.Clear();

            // Create the menu showed during level selection
            _elements.Add(new Menu.Element(
                new Vector2(window.ClientBounds.Width / 2, 200), 
                "Choose level:", 
                Menu.Element.Type.text
                ));
            _elements.Add(new Menu.Element(
                new Rectangle(150, 100, 200, 100),
                "Menu",
                Menu.Element.Type.button,
                new Menu.Element.handler(goToStart),
                Color.Red
                ));
            _elements.Add(new Menu.Element(
                new Rectangle(window.ClientBounds.Width / 2 - 250, 300, 200, 100),
                "Level 1",
                Menu.Element.Type.button,
                new Menu.Element.handler(goToLevel1),
                Color.GreenYellow, 
                levelsUnlocked[0]
                ));
            _elements.Add(new Menu.Element(
                new Rectangle(window.ClientBounds.Width / 2, 300, 200, 100),
                "Level 2",
                Menu.Element.Type.button,
                new Menu.Element.handler(goToLevel2),
                Color.GreenYellow, 
                levelsUnlocked[1]
                ));
            _elements.Add(new Menu.Element(
                new Rectangle(window.ClientBounds.Width / 2 + 250, 300, 200, 100),
                "Level 3",
                Menu.Element.Type.button,
                new Menu.Element.handler(goToLevel3),
                Color.GreenYellow, 
                levelsUnlocked[2]
                ));
            menus.createMenu("levelSelect", _elements);
            _elements.Clear();

            // Load the sprites for the shootemup game
            ShootemUp.stoneSprite = Content.Load<Texture2D>("Sprites/Stone");
            ShootemUp.stoneSprite2x = Content.Load<Texture2D>("Sprites/Stone2x");
            ShootemUp.stoneSprite4x = Content.Load<Texture2D>("Sprites/Stone4x");
            ShootemUp.stoneSprite8x = Content.Load<Texture2D>("Sprites/Stone8x");
            ShootemUp.heartSprite = Content.Load<Texture2D>("Sprites/Hjaerta");
            ShootemUp.fuelSprite2x = Content.Load<Texture2D>("Sprites/Hembraent2x");
            ShootemUp.rocketFire = Content.Load<Texture2D>("Sprites/Fire");
            ShootemUp.rocketFire2x = Content.Load<Texture2D>("Sprites/Fire2x");

            // Load the textures for the stencils
            Building.motorStencil = Content.Load<Texture2D>("Sprites/MotorStencil8x");
            Building.tankStencil = Content.Load<Texture2D>("Sprites/TankStencil8x");
            Building.topStencil = Content.Load<Texture2D>("Sprites/CapsuleStencil8x");

            // Load the sprites for launching
            Launching.launchBkg = Content.Load<Texture2D>("Sprites/MulleInDaWoods8x");
            Launching.rocketFire = Content.Load<Texture2D>("Sprites/Fire2x");

            // Load sound effects
            snapSound = Content.Load<SoundEffect>("Sound/Snap");
            snapSoundInstance = snapSound.CreateInstance();
            snapSoundInstance.Volume = 0.2f;
            motorSound = Content.Load<SoundEffect>("Sound/RaketSprak");
            motorSoundInstance = motorSound.CreateInstance();
            motorSoundInstance.Volume = 0.2f;
            motorSoundInstance.IsLooped = true; 
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            // Read mouse and keyboard
            mouse = Mouse.GetState();
            keyboard = Keyboard.GetState();

            // Determines what routine to follow based on set game mode
            switch (gameState)
            {
                case GameStates.build:
                    Building.update(mouse);
                    break;


                case GameStates.launch:
                    Launching.update(keyboard, mouse);
                    break;

                case GameStates.menu:
                    menus.update(mouse);
                    break;

                case GameStates.levelSelect:
                    LevelSelect.update(mouse);
                    break;

                case GameStates.shootemUp:
                    ShootemUp.update(keyboard, mouse);
                    break;

                case GameStates.shootemUpResult:
                    ShootemUpResult.update(mouse);
                    break;

                default:
                    Building.initialize();
                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            _spriteBatch.Begin();

            // Determines what drawing routine to follow based on set game mode
            switch (gameState) {
                case GameStates.build:
                    Building.draw(_spriteBatch, mouse);
                    break;

                case GameStates.launch:
                    Launching.draw(_spriteBatch, keyboard, mouse);
                    break;

                case GameStates.menu:
                    menus.draw(_spriteBatch, mouse);
                    break;

                case GameStates.levelSelect:
                    LevelSelect.draw(_spriteBatch, mouse);
                    break;

                case GameStates.shootemUp:
                    ShootemUp.draw(_spriteBatch, keyboard, mouse);
                    break;

                case GameStates.shootemUpResult:
                    ShootemUpResult.draw(_spriteBatch, mouse);
                    break;

                default:
                    Building.draw(_spriteBatch, mouse);
                    break;
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }


        // Handlers

        // Method for when to return to start menu
        private static void goToStart()
        {
            gameState = GameStates.menu;
            menus.currentMenu = "start";
        }

        // Method for when to try to launch
        private static void launch()
        {
            Launching.initialize();

            if (Launching.builtRocket.motor.onRocket == false || Launching.builtRocket.tank.onRocket == false || Launching.builtRocket.top.onRocket == false)
            {
                Building.initialize();
            }
        }

        // Methods for wich level to start
        private static void goToLevel1()
        {
            ShootemUp.initialize(1);
        }
        private static void goToLevel2()
        {
            ShootemUp.initialize(2);
        }
        private static void goToLevel3()
        {
            ShootemUp.initialize(3);
        }

    }

    // Hack to make it possible to pass a variable instead of a value
    public class Unlocked
    {
        public bool var;

        public Unlocked(bool _var)
        {
            var = _var;
        }
    }
}
