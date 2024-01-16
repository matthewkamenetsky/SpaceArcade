/* Author: Matthew Kamenetsky
 * File name: Game1.cs
 * Project name: PASS3_ICS4U
 * Creation date: May 11, 2023
 * Modified date: June 12, 2023
 * Description: the driver class is responsible for updating and drawing all gamestates of the space arcade. This includes an account sign-in system, a stats page sorted for each game type, an instructions page, a pre game phase with drag and drop features, the gameplay phase itself, and the results/endgame screens. The space arcade lets a player play 4 different types of space-themed games, in etiher a single, triple pack, or five pack format. The driver allows the gameplay elements of each game to be combined with other standard game elements into one coherent piece.
 */

using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Animation2D;

namespace SpaceArcade
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        //Store game states
        private const int TITLE = 0;
        private const int MENU = 1;
        private const int STATS = 2;
        private const int INFO = 3;
        private const int RESULTS = 4;
        private const int ENDGAME = 5;
        private const int PREGAME = 6;
        private const int GAMEPLAY = 7;

        //Store music indexes
        private const int MENU_MUSIC = 0;
        private const int PLAY_MUSIC = 1;

        //Store button indexes
        private const int PLAY_BTN = 0;
        private const int STAT_BTN = 1;
        private const int INFO_BTN = 2;
        private const int EXIT_BTN = 3;

        //Store validity indexes
        private const int VALID = 0;
        private const int INVALID = 1;
        private const int NEW = 2;

        //Store the max number of character's for a player's name
        private const int MAX_CHARS = 16;

        //Store the game types
        public const int SHOOTER = 0;
        public const int RUNNER = 1;
        public const int CATCHER = 2;
        public const int WHACKER = 3;
        public const int TRI = 4;
        public const int QUAD = 5;

        //Store the font types
        public const int BIG = 0;
        public const int BTN = 1;
        public const int REG = 2;
        public const int BOLD = 3;

        //Store offsets
        public const int TITLE_OFFSET = 48;
        public const int SIDE_OFFSET = 32;

        //Store the indexes of the username and password
        public const int USER_NAME = 0;
        public const int PASSWORD = 1;

        //Store the indexes of the stats screen
        private const int RANK = 0;
        private const int NAME = 1;
        private const int SCORE = 2;
        private const int DEFAULT = -1;

        //Store the indexes of the info screen
        private const int GOAL = 0;
        private const int MOVE = 1;
        private const int TIPS = 2;

        //Store the indexes of the pre game options
        private const int SINGLE = 0;
        private const int TRI_PACK = 1;
        private const int FOUR_PACK = 2;

        //Store the indexes of the pregame order
        private const int ONE = 0;
        private const int TWO = 1;
        private const int THREE = 2;
        private const int FOUR = 3;

        //Store the indexes of the results screen
        private const int LAST = 0;
        private const int NEXT = 1;

        //Store the screen dimensions
        private int screenWidth;
        private int screenHeight;

        //Store the mouse states
        private MouseState mouse;
        private MouseState prevMouse;

        //Store the keyboard states
        private KeyboardState kb;
        private KeyboardState prevKb;

        //Store the keys and letters array for user input
        private Keys[] keys = new Keys[] { Keys.A, Keys.B, Keys.C, Keys.D, Keys.E, Keys.F, Keys.G, Keys.H, Keys.I, Keys.J, Keys.K, Keys.L, Keys.M, Keys.N, Keys.O, Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T, Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y, Keys.Z, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9 };
        private string[] letters = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

        //Store the writer and reader of the user file
        private StreamWriter fileWriter;
        private StreamReader fileReader;

        //Store the fonts
        private SpriteFont[] fonts = new SpriteFont[4];

        //Store the music
        private Song[] songs = new Song[2];

        //Store the list of players and the current player
        private List<Player> players = new List<Player>();
        private Player player;

        //Store title images and rectangles
        private Texture2D titleImg;
        private Texture2D[] gameImgs = new Texture2D[6];
        private Rectangle titleRec;

        //Store the button image, rectangles, hover status, and sound effect
        private Texture2D btnImg;
        private Rectangle[] btnRecs = new Rectangle[4];
        private bool[] btnHovers = new bool[4];
        private SoundEffect btnSnd;

        //Store the game type titles
        private string[] gameBtnTexts = new string[] { "ALIEN SHOOTER", "SPACE RUN", "STAR CATCHER", "WHACK-A-'ROID", "TRIATHLON", "QUADRATHLON" };

        //Store title screen texts and locations
        private string titleBtnText = "MENU";
        private string[] promptPlayerTexts = new string[] { "Enter your Username", "Enter your Password" };
        private string[] playerInfoTexts = new string[2] { "", "" };
        private string resultText = "";
        private string continueText = "CONTINUE";
        private Vector2 titleBtnLoc;
        private Vector2[] promptLocs = new Vector2[2];
        private Vector2[] infoLocs = new Vector2[2];
        private Vector2 resultLoc;
        private Vector2 continueLoc;

        //Store the booleans related to the player entering their username/password
        private bool hasClickedMenu = false;
        private bool[] hasPutInfo = new bool[] { false, false };

        //Store menu screen texts and locations
        private string[] menuBtnTexts = new string[4] { "PLAY", "STATS", "INFO", "EXIT" };
        private Vector2[] menuBtnLocs = new Vector2[4];

        //Store stats screen texts and locations, rectangles, and hover statuses
        private string backText = "GO BACK";
        private string ptsText;
        private string[] statInfoTexts = new string[] { "RANK", "NAME", "SCORE" };
        private Vector2 playerNameLoc;
        private Vector2[] playerScoreLocs = new Vector2[6];
        private Vector2[,] statInfoLocs = new Vector2[1, 3];
        private Vector2[,] statLocs = new Vector2[10, 3];
        private Vector2[] statBtnLocs = new Vector2[7];
        private Rectangle statTitleRec;
        private Rectangle[] statRecs = new Rectangle[7];
        private bool[] statHovers = new bool[7];

        //Store the current stat page being viewed
        private int curStat = DEFAULT;

        //Store info screen texts, locations, button rectangles, and hover statuses
        private string descText = "The Space Arcade is Composod of 4 1-Minute Games:\n"
                                + "Alien Shooter-Space Run-Star Catcher-Whack-A-Roid\n"
                                + "              You can Select 3 Game Length Types:\n"
                                + "          Single Game, Triple Pack, and Quad Pack\n"
                                + "    Click Each Button to see more about Each Game";
        private string[] descTexts = new string[] { "GOAL", "MOVEMENT", "TIPS" };
        private string[] goalTexts = new string[] { "Shoot Aliens", "Dodge Non-Coin Objects", "Catch Stars", "Hit Asteroids" };
        private string[] moveTexts = new string[] { "AD or Side Arrow Keys to Move, Space to Shoot",
                                                    "Space to Move Up",
                                                    "Click the Selector to Choose Your Speed & Direction",
                                                    "Click to Destroy Asteroids" };
        private string[] tipsTexts = new string[] { "Watch Out for Unique Movements & Abilities",
                                                    "Dodge Fake Coins & Change Directions Often",
                                                    "Asteroids Cause Bounces, Star Patterns are Random",
                                                    "Watch The Clock!" };
        private Vector2 descLoc;
        private Vector2[] descTextLocs = new Vector2[6];
        private Vector2[] infoBtnLocs = new Vector2[5];
        private Rectangle[] infoRecs = new Rectangle[5];
        private bool[] infoHovers = new bool[5];

        //Store the current info page being viewed
        private int curInfo = DEFAULT;

        //Store pregame texts, locations, images, and rectangles
        private string[] optionTexts = new string[3] { "SINGLE GAME", "TRIPLE PACK", "FOUR PACK" };
        private string orderText = "Drag and Drop the Images to Create the Order" +
                                   "\n Press the X to Remove the Game from its Box";
        private string[] numberTexts = new string[] { "1", "2", "3", "4" };
        private Vector2[] optionLocs = new Vector2[3];
        private Vector2 orderLoc;
        private Vector2[] numberLocs = new Vector2[4];
        private Texture2D circleImg;
        private Texture2D removeImg;
        private Rectangle[] optionRecs = new Rectangle[3];
        private Rectangle[] numCircleRecs = new Rectangle[4];
        private Rectangle[] removeRecs = new Rectangle[4];
        private Rectangle[] removeHoverRecs = new Rectangle[4];
        private Rectangle[] dragRecs = new Rectangle[4];
        private Rectangle[] dropRecs = new Rectangle[4];
        private Rectangle[] ogDragRecs = new Rectangle[4];

        //Store the booleans related to the pregame button hovering, choosing of a game type, and the chosen game type
        private bool[] optionHovers = new bool[] { false, false, false };
        private bool hasChosenOption = false;
        private bool[] chosenGameType = new bool[] { false, false, false };

        //Store the index of the rectangle being dragged and the booleans related to the player's ordering, as well as integers representing the player's chosen games
        private int draggedRectangleIndex = DEFAULT;
        private bool[] isBoxFull = new bool[4] { false, false, false, false };
        private bool[] hasBeenDropped = new bool[4] { false, false, false, false };
        private bool[] isXHovered = new bool[] { false, false, false, false };
        private bool hasChosenOrder = false;
        private int[] chosenGames = new int[4];

        //Store results texts and locations
        private string[] resultsGameTexts = new string[] { "LAST GAME: ", "NEXT GAME: " };
        private string[] resultsPtsText = new string[] { "GAME POINTS: ", "TOTAL POINTS: " };
        private Vector2[] resultsGameLocs = new Vector2[2];
        private Vector2[] resultsPtsLocs = new Vector2[2];

        //Store endgame texts and locations
        private string totalText = "TOTAL POINTS: ";
        private string[] endGameTexts = new string[4];
        private Vector2 totalLoc;
        private Vector2[] endGameLocs = new Vector2[4];

        //Store the game background images, gamestate background images, and the background rectangle
        private Texture2D[] gameBgs = new Texture2D[4];
        private Texture2D[] bgs = new Texture2D[7];
        private Rectangle bgRec;

        //Store the player images and soundeffects
        private Texture2D[] playerImgs = new Texture2D[4];
        private SoundEffect[] playerSnds = new SoundEffect[4];

        //Store the blank texture image used in several games
        private Texture2D blankTexture;

        //Store the alien shooter images
        private Texture2D[] alienImgs = new Texture2D[3];
        private Texture2D[] bulletImgs = new Texture2D[2];
        private Texture2D[] splatImgs = new Texture2D[3];

        //Store the alien shooter sounds
        private SoundEffect shootSnd;
        private SoundEffect[] hitSnds = new SoundEffect[3];
        private SoundEffect poisonSnd;

        //Store the space run images
        private Texture2D[] flameImgs = new Texture2D[2];
        private Texture2D[] spaceObjImgs = new Texture2D[4];

        //Store the space run sounds
        private SoundEffect[] runnerSounds = new SoundEffect[4];

        //Store the star catcher images
        private Texture2D[] selectorImgs = new Texture2D[2];
        private Texture2D[] starImgs = new Texture2D[3];

        //Store the star catcher sounds
        private SoundEffect[] catcherSounds = new SoundEffect[3];

        //Store the whacker images
        private Texture2D[] whackerImgs = new Texture2D[5];

        //Store the whacker sounds
        private SoundEffect[] whackerSounds = new SoundEffect[4];

        //Store and set the initial game state, typically TITLE to start
        private int gameState = TITLE;

        //Store the minigame queue
        private GameQueue minigameQueue;

        //Store the current game and the current totPts
        private Minigame curGame;
        private int totPts = 0;

        //Store the random variable
        public static Random rng = new Random();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //Set the mouse to visible
            IsMouseVisible = true;

            //Set the screen dimensions and store them
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
            screenWidth = graphics.PreferredBackBufferWidth;
            screenHeight = graphics.PreferredBackBufferHeight;

            //Apply the graphical changes
            graphics.ApplyChanges();

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

            //Load fonts
            fonts[BIG] = Content.Load<SpriteFont>("Fonts/TitleFont");
            fonts[BTN] = Content.Load<SpriteFont>("Fonts/BigFont");
            fonts[REG] = Content.Load<SpriteFont>("Fonts/RegFont");
            fonts[BOLD] = Content.Load<SpriteFont>("Fonts/BoldFont");

            //Load music
            songs[MENU_MUSIC] = Content.Load<Song>("Sound/MenuMusic");
            songs[PLAY_MUSIC] = Content.Load<Song>("Sound/GameMusic");

            //Load background images
            bgs[TITLE] = Content.Load<Texture2D>("Images/TitleBg");
            bgs[MENU] = Content.Load<Texture2D>("Images/MenuBg1");
            bgs[STATS] = Content.Load<Texture2D>("Images/StatsBg1");
            bgs[INFO] = Content.Load<Texture2D>("Images/InfoBg");
            bgs[RESULTS] = Content.Load<Texture2D>("Images/ResultsBg");
            bgs[ENDGAME] = Content.Load<Texture2D>("Images/EndgameBg");
            bgs[PREGAME] = Content.Load<Texture2D>("Images/PregameBg");

            //Load title images
            titleImg = Content.Load<Texture2D>("Images/TitleImg");
            gameImgs[SHOOTER] = Content.Load<Texture2D>("Images/ShooterImg");
            gameImgs[RUNNER] = Content.Load<Texture2D>("Images/RunImg");
            gameImgs[CATCHER] = Content.Load<Texture2D>("Images/CatcherImg");
            gameImgs[WHACKER] = Content.Load<Texture2D>("Images/WhackerImg");
            gameImgs[TRI] = Content.Load<Texture2D>("Images/TriathlonImg");
            gameImgs[QUAD] = Content.Load<Texture2D>("Images/QuadrathlonImg");

            //Load the button image and soundeffect
            btnImg = Content.Load<Texture2D>("Images/Button");
            btnSnd = Content.Load<SoundEffect>("Sound/Btn1");

            //Load pregame images
            circleImg = Content.Load<Texture2D>("Images/NumCircle");
            removeImg = Content.Load<Texture2D>("Images/RemoveCircle");

            //Load the player images
            playerImgs[SHOOTER] = Content.Load<Texture2D>("Images/AlienShooter/Ship1");
            playerImgs[RUNNER] = Content.Load<Texture2D>("Images/SpaceRun/Astronaut");
            playerImgs[CATCHER] = Content.Load<Texture2D>("Images/StarCatchers/Net1");
            playerImgs[WHACKER] = Content.Load<Texture2D>("Images/WhackAnAsteroid/Hammer1");

            //Create the blank pixel texture and set its data
            blankTexture = new Texture2D(GraphicsDevice, 1, 1);
            blankTexture.SetData(new[] { Color.White });

            //Load the alien shooter images
            gameBgs[SHOOTER] = Content.Load<Texture2D>("Images/AlienShooter/SpaceBg");
            alienImgs[AlienShooter.SHOOT_ALIEN] = Content.Load<Texture2D>("Images/AlienShooter/ShipEnemy");
            alienImgs[AlienShooter.SPLASH_ALIEN] = Content.Load<Texture2D>("Images/AlienShooter/SplashEnemy");
            alienImgs[AlienShooter.POISON_ALIEN] = Content.Load<Texture2D>("Images/AlienShooter/PoisonEnemy");
            splatImgs[AlienShooter.SHOOT_ALIEN] = Content.Load<Texture2D>("Images/AlienShooter/Splat_64");
            splatImgs[AlienShooter.SPLASH_ALIEN] = Content.Load<Texture2D>("Images/AlienShooter/BlueSplat");
            splatImgs[AlienShooter.POISON_ALIEN] = Content.Load<Texture2D>("Images/AlienShooter/GreenSplat");
            bulletImgs[AlienShooter.PLAYER] = Content.Load<Texture2D>("Images/AlienShooter/Bullet1");
            bulletImgs[AlienShooter.ENEMY] = Content.Load<Texture2D>("Images/AlienShooter/Bullet2");

            //Load the alien shooter sounds
            shootSnd = Content.Load<SoundEffect>("Sound/ShooterSounds/ShootSnd");
            playerSnds[SHOOTER] = shootSnd;
            hitSnds[AlienShooter.SHOOT_ALIEN] = Content.Load<SoundEffect>("Sound/ShooterSounds/ShipSnd");
            hitSnds[AlienShooter.SPLASH_ALIEN] = Content.Load<SoundEffect>("Sound/ShooterSounds/SplashSnd");
            hitSnds[AlienShooter.POISON_ALIEN] = Content.Load<SoundEffect>("Sound/ShooterSounds/PoisonSnd");
            poisonSnd = Content.Load<SoundEffect>("Sound/ShooterSounds/PoisonAtkSnd");

            //Load the space run images
            gameBgs[RUNNER] = Content.Load<Texture2D>("Images/SpaceRun/RunBg1");
            flameImgs[Player.FLAME] = Content.Load<Texture2D>("Images/SpaceRun/JumpSheet1");
            flameImgs[Player.FIZZLE] = Content.Load<Texture2D>("Images/SpaceRun/FizzleImg");
            spaceObjImgs[SpaceRun.ASTEROID] = Content.Load<Texture2D>("Images/SpaceRun/AsteroidSheet");
            spaceObjImgs[SpaceRun.FIRE] = Content.Load<Texture2D>("Images/SpaceRun/FireSheet");
            spaceObjImgs[SpaceRun.FAKE_COIN] = Content.Load<Texture2D>("Images/SpaceRun/FakeCoinSheet");
            spaceObjImgs[SpaceRun.COIN] = Content.Load<Texture2D>("Images/SpaceRun/CoinSheet");

            //Load the space run sounds
            playerSnds[RUNNER] = Content.Load<SoundEffect>("Sound/RunnerSounds/RocketSnd");
            runnerSounds[SpaceRun.SCREAM] = Content.Load<SoundEffect>("Sound/RunnerSounds/ScreamSnd");
            runnerSounds[SpaceRun.BOOST] = Content.Load<SoundEffect>("Sound/RunnerSounds/CoinSnd");
            runnerSounds[SpaceRun.DODGE] = Content.Load<SoundEffect>("Sound/RunnerSounds/DodgeSnd");
            runnerSounds[SpaceRun.TICK] = Content.Load<SoundEffect>("Sound/RunnerSounds/TickSnd");

            //Load the star catchers images
            gameBgs[CATCHER] = Content.Load<Texture2D>("Images/StarCatchers/StarBg1");
            selectorImgs[Selector.BLANK] = blankTexture;
            selectorImgs[Selector.BTN] = Content.Load<Texture2D>("Images/StarCatchers/StopBtn");
            starImgs[StarCatcher.BIG] = Content.Load<Texture2D>("Images/StarCatchers/BigStar");
            starImgs[StarCatcher.MED] = Content.Load<Texture2D>("Images/StarCatchers/MedStar");
            starImgs[StarCatcher.SML] = Content.Load<Texture2D>("Images/StarCatchers/SmallStar");

            //Load the space run sounds
            catcherSounds[StarCatcher.BTN] = btnSnd;
            catcherSounds[StarCatcher.CATCH] = runnerSounds[SpaceRun.BOOST];
            catcherSounds[StarCatcher.BOUNCE] = Content.Load<SoundEffect>("Sound/CatcherSounds/BounceSnd");

            //Load the whacker images
            gameBgs[WHACKER] = Content.Load<Texture2D>("Images/WhackAnAsteroid/Bg1");
            whackerImgs[WhackAnAsteroid.CIRCLE] = Content.Load<Texture2D>("Images/WhackAnAsteroid/Circle1");
            whackerImgs[WhackAnAsteroid.ASTEROID] = Content.Load<Texture2D>("Images/WhackAnAsteroid/Asteroid1");
            whackerImgs[WhackAnAsteroid.POOF] = Content.Load<Texture2D>("Images/WhackAnAsteroid/PoofAnim");
            whackerImgs[WhackAnAsteroid.EXPLOSION] = Content.Load<Texture2D>("Images/WhackAnAsteroid/ExplosionAnim");

            //Load the whacker sounds
            whackerSounds[WhackAnAsteroid.MISS_CLICK] = Content.Load<SoundEffect>("Sound/WhackerSounds/MissSnd");
            whackerSounds[WhackerAsteroid.EXPLODE] = Content.Load<SoundEffect>("Sound/WhackerSounds/ExplodeSnd");
            whackerSounds[WhackAnAsteroid.POOF] = Content.Load<SoundEffect>("Sound/WhackerSounds/PoofSnd");
            whackerSounds[WhackAnAsteroid.SPAWN] = Content.Load<SoundEffect>("Sound/WhackerSounds/SpawnSnd");

            //Load the background rectangle
            bgRec = new Rectangle(0, 0, screenWidth, screenHeight);

            //Load the title rectangle
            titleRec = new Rectangle(screenWidth / 2 - (int)(titleImg.Width * 0.8) / 2, TITLE_OFFSET, (int)(titleImg.Width * 0.8), (int)(titleImg.Height * 0.8));

            //Load the play button's rectangle and its text location
            btnRecs[PLAY_BTN] = new Rectangle(screenWidth / 2 - btnImg.Width / 2, titleRec.Bottom + SIDE_OFFSET, btnImg.Width, btnImg.Height);
            menuBtnLocs[PLAY_BTN] = new Vector2(screenWidth / 2 - fonts[BTN].MeasureString(menuBtnTexts[PLAY_BTN]).X / 2, btnRecs[PLAY_BTN].Center.Y - fonts[BTN].MeasureString(menuBtnTexts[PLAY_BTN]).Y / 2);

            //Loop through the remaining button
            for (int i = STAT_BTN; i <= EXIT_BTN; i++)
            {
                //Create the button's rectangle and its text location
                btnRecs[i] = new Rectangle(btnRecs[PLAY_BTN].X, btnRecs[i - 1].Bottom + TITLE_OFFSET, btnImg.Width, btnImg.Height);
                menuBtnLocs[i] = new Vector2(screenWidth / 2 - fonts[BTN].MeasureString(menuBtnTexts[i]).X / 2, btnRecs[i].Center.Y - fonts[BTN].MeasureString(menuBtnTexts[i]).Y / 2);
            }

            //Create the title button text location
            titleBtnLoc = new Vector2(screenWidth / 2 - fonts[BTN].MeasureString(titleBtnText).X / 2, btnRecs[PLAY_BTN].Center.Y - fonts[BTN].MeasureString(titleBtnText).Y / 2);

            //Load the offscreen text locations
            for (int i = 0; i < promptLocs.Length; i++)
            {
                //Load the player prompt and player text locations
                promptLocs[i] = new Vector2(0, -200);
                infoLocs[i] = new Vector2(0, -200);
            }

            //Load the result of the player's input text location
            resultLoc = new Vector2(0, -200);

            //Load the continue button text location
            continueLoc = new Vector2(screenWidth / 2 - fonts[BTN].MeasureString(continueText).X / 2, btnRecs[EXIT_BTN].Center.Y - fonts[BTN].MeasureString(continueText).Y / 2);

            //Load the first stat button rectangle
            statRecs[SHOOTER] = new Rectangle(SIDE_OFFSET, screenHeight - TITLE_OFFSET * 3, (int)(btnImg.Width * 0.6), (int)(btnImg.Height * 0.4));

            //Loop through the first 2 stat buttons after the first
            for (int i = RUNNER; i < WHACKER; i++)
            {
                //Load the stat button rectangle
                statRecs[i] = new Rectangle(SIDE_OFFSET, statRecs[i - 1].Bottom + SIDE_OFFSET / 4, (int)(btnImg.Width * 0.6), (int)(btnImg.Height * 0.4));
            }

            //Loop through the remaining stat buttons except the last one
            for (int i = WHACKER; i <= QUAD; i++)
            {
                //Load the stat button rectangle
                statRecs[i] = new Rectangle(screenWidth - (int)(btnImg.Width * 0.6) - SIDE_OFFSET, statRecs[i - WHACKER].Y, (int)(btnImg.Width * 0.6), (int)(btnImg.Height * 0.4));
            }

            //Load the last stat button rectangle and text location
            statRecs[QUAD + 1] = new Rectangle(screenWidth / 2 - btnImg.Width / 2, screenHeight - btnImg.Height - SIDE_OFFSET, btnImg.Width, btnImg.Height);
            statBtnLocs[QUAD + 1] = new Vector2(statRecs[QUAD + 1].Center.X - fonts[BTN].MeasureString(backText).X / 2, statRecs[QUAD + 1].Center.Y - fonts[BTN].MeasureString(backText).Y / 2);

            //Loop through the other stat button text locations
            for (int i = 0; i <= QUAD; i++)
            {
                //Load the stat button text location
                statBtnLocs[i] = new Vector2(statRecs[i].Center.X - fonts[REG].MeasureString(gameBtnTexts[i]).X / 2, statRecs[i].Center.Y - fonts[REG].MeasureString(gameBtnTexts[i]).Y / 2);
            }

            //Load the stat info text locations
            statInfoLocs[ONE, ONE] = new Vector2(statRecs[SHOOTER].X, titleRec.Bottom);
            statInfoLocs[ONE, TWO] = new Vector2(screenWidth / 2 - fonts[BTN].MeasureString(statInfoTexts[TWO]).X / 2, statInfoLocs[ONE, ONE].Y);
            statInfoLocs[ONE, THREE] = new Vector2(statRecs[WHACKER].Left, statInfoLocs[ONE, ONE].Y);

            //Loop through the stat location rows
            for (int i = 0; i < statLocs.GetLength(0); i++)
            {
                //Loop through the stat location columns
                for (int j = 0; j < statLocs.GetLength(1); j++)
                {
                    //Check if the current row is the first
                    if (i == ONE)
                    {
                        //Load the first row of stat locations
                        statLocs[ONE, j] = new Vector2(statInfoLocs[ONE, j].X, statInfoLocs[ONE, ONE].Y + (int)(TITLE_OFFSET * 1.25));
                    }
                    else
                    {
                        //Load the stat locations
                        statLocs[i, j] = new Vector2(statInfoLocs[ONE, j].X, statLocs[i - 1, j].Y + (int)(TITLE_OFFSET * 0.9));
                    }

                    //Check if the current column is the name column
                    if (j == NAME)
                    {
                        //Set a new x value for the stat location
                        statLocs[i, j].X = btnRecs[EXIT_BTN].X;
                    }
                }
            }

            //Load the description txt location
            descLoc = new Vector2(screenWidth / 2 - fonts[BOLD].MeasureString("Alien Shooter-Space Run-Star Catcher-Whack-A-Roid").X / 2, titleRec.Bottom + SIDE_OFFSET * 3);

            //Load the info button rectangles
            infoRecs[SHOOTER] = new Rectangle(SIDE_OFFSET, screenHeight / 2 + TITLE_OFFSET + SIDE_OFFSET, (int)(btnImg.Width * 0.68), (int)(btnImg.Height * 0.68));
            infoRecs[RUNNER] = new Rectangle(infoRecs[SHOOTER].Right + SIDE_OFFSET, infoRecs[SHOOTER].Y, infoRecs[SHOOTER].Width, infoRecs[SHOOTER].Height);
            infoRecs[WHACKER] = new Rectangle(screenWidth - infoRecs[SHOOTER].Width - SIDE_OFFSET, infoRecs[SHOOTER].Y, infoRecs[SHOOTER].Width, infoRecs[SHOOTER].Height);
            infoRecs[CATCHER] = new Rectangle(infoRecs[WHACKER].Left - SIDE_OFFSET - infoRecs[SHOOTER].Width, infoRecs[SHOOTER].Y, infoRecs[SHOOTER].Width, infoRecs[SHOOTER].Height);
            infoRecs[TRI] = statRecs[QUAD + 1];

            //Loop through the info button locations except the last one
            for (int i = 0; i < infoBtnLocs.Length - 1; i++)
            {
                //Load the info button text location
                infoBtnLocs[i] = new Vector2(infoRecs[i].Center.X - fonts[REG].MeasureString(gameBtnTexts[i]).X / 2, infoRecs[i].Center.Y - fonts[REG].MeasureString(gameBtnTexts[i]).Y / 2);
            }

            //Load the last info button text
            infoBtnLocs[TRI] = new Vector2(infoRecs[TRI].Center.X - fonts[BTN].MeasureString(backText).X / 2, infoRecs[TRI].Center.Y - fonts[BTN].MeasureString(backText).Y / 2);

            //Load the description locations
            descTextLocs[GOAL] = new Vector2(screenWidth / 2 - fonts[BTN].MeasureString(descTexts[GOAL]).X / 2, titleRec.Bottom);
            descTextLocs[MOVE] = new Vector2(screenWidth / 2 - fonts[BTN].MeasureString(descTexts[MOVE]).X / 2, descTextLocs[GOAL].Y + TITLE_OFFSET * 2);
            descTextLocs[TIPS] = new Vector2(screenWidth / 2 - fonts[BTN].MeasureString(descTexts[TIPS]).X / 2, descTextLocs[MOVE].Y + TITLE_OFFSET * 2);

            //Loop through the remaining description locations
            for (int i = TIPS + 1; i < descTextLocs.Length; i++)
            {
                //Load the description locations
                descTextLocs[i] = new Vector2(descTextLocs[i - (TIPS + 1)].X, descTextLocs[i - (TIPS + 1)].Y + fonts[BTN].MeasureString(descTexts[i - (TIPS + 1)]).Y - SIDE_OFFSET / 4);
            }

            //Load the game type rectangles
            optionRecs[SINGLE] = new Rectangle(screenWidth / 2 - (int)(btnImg.Width * 0.85) / 2, screenHeight / 2 - (int)(btnImg.Height * 0.85) / 2, (int)(btnImg.Width * 0.85), (int)(btnImg.Height * 0.85));
            optionRecs[TRI_PACK] = new Rectangle(SIDE_OFFSET, optionRecs[SINGLE].Y, (int)(btnImg.Width * 0.85), (int)(btnImg.Height * 0.85));
            optionRecs[FOUR_PACK] = new Rectangle(screenWidth - SIDE_OFFSET - (int)(btnImg.Width * 0.85), optionRecs[TRI_PACK].Y, (int)(btnImg.Width * 0.85), (int)(btnImg.Height * 0.85));

            //Loop through the option text locations
            for (int i = 0; i < optionLocs.Length; i++)
            {
                //Load the option text locations
                optionLocs[i] = new Vector2(optionRecs[i].Center.X - fonts[BOLD].MeasureString(optionTexts[i]).X / 2, optionRecs[i].Center.Y - fonts[BOLD].MeasureString(optionTexts[i]).Y / 2);
            }

            //Load the initial drag and drop rectangles for the pregame
            dragRecs[ONE] = new Rectangle(SIDE_OFFSET, TITLE_OFFSET, (int)(gameImgs[SHOOTER].Width * 0.5), (int)(gameImgs[SHOOTER].Height * 0.5));
            dropRecs[ONE] = new Rectangle(screenWidth - TITLE_OFFSET * 2 - (int)(btnImg.Width * 1.5), SIDE_OFFSET / 2, (int)(btnImg.Width * 1.5), (int)(btnImg.Height * 1.2));

            //Loop through the remaining drag and drop rectangles of the pregame
            for (int i = TWO; i < dragRecs.Length; i++)
            {
                //Load the drag and drop rectangle
                dragRecs[i] = new Rectangle(SIDE_OFFSET, dragRecs[i - 1].Bottom + SIDE_OFFSET, (int)(gameImgs[i].Width * 0.5), (int)(gameImgs[i].Height * 0.5));
                dropRecs[i] = new Rectangle(dropRecs[i - 1].X, dropRecs[i - 1].Bottom + SIDE_OFFSET, (int)(btnImg.Width * 1.5), (int)(btnImg.Height * 1.2));
            }

            //Loop through the original drag rectangles
            for (int i = 0; i < ogDragRecs.Length; i++)
            {
                //Load the original drag rectangles
                ogDragRecs[i] = dragRecs[i];
            }

            //Loop through the number of order circles there are
            for (int i = 0; i < numCircleRecs.Length; i++)
            {
                //Load the number circle rectangles and their text locations
                numCircleRecs[i] = new Rectangle(dropRecs[i].X, dropRecs[i].Y, (int)(circleImg.Width * 1.2), (int)(circleImg.Height * 1.2));
                numberLocs[i] = new Vector2(numCircleRecs[i].Center.X - fonts[BIG].MeasureString(numberTexts[i]).X / 2, numCircleRecs[i].Center.Y - fonts[BIG].MeasureString(numberTexts[ONE]).Y / 2 + SIDE_OFFSET / 4);

                //Load the dropbox remover rectangles and their hover rectangles
                removeRecs[i] = new Rectangle(dropRecs[i].Right, dropRecs[i].Center.Y - removeImg.Height / 2, removeImg.Width, removeImg.Height);
                removeHoverRecs[i] = new Rectangle(removeRecs[i].X + SIDE_OFFSET / 2, removeRecs[i].Y + SIDE_OFFSET / 2, (int)(SIDE_OFFSET * 1.5), (int)(SIDE_OFFSET * 1.5));
            }

            //Load the location of the ordering instructions
            orderLoc = new Vector2(screenWidth / 2 - fonts[BOLD].MeasureString(orderText).X / 2, screenHeight - TITLE_OFFSET * 2);

            //Set volume for the song
            MediaPlayer.Volume = 0.8f;

            //Set the MediaPlayer to loop the song
            MediaPlayer.IsRepeating = true;

            //Play the menu song
            MediaPlayer.Play(songs[MENU_MUSIC]);

            //Read in the players file
            ReadStats();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //Setup the current mouse and previous mouse states
            prevMouse = mouse;
            mouse = Mouse.GetState();

            //Setup the current keyboard and previous keyboard states
            prevKb = kb;
            kb = Keyboard.GetState();

            //Hover buttons based on collision detection
            for (int i = 0; i < btnHovers.Length; i++)
            {
                //Hover the buttons
                btnHovers[i] = btnRecs[i].Contains(mouse.Position);
            }

            //Check the gamestate to determine how to update the game
            switch (gameState)
            {
                case TITLE:
                    //Check if the user has clicked to enter the menu
                    if (!hasClickedMenu)
                    {
                        //Check player input to determine how to proceed
                        if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
                        {
                            //Check fi the title button is pressed
                            if (btnRecs[TITLE].Contains(mouse.Position))
                            {
                                //Update the location of the prompt and user information text
                                promptLocs[USER_NAME].X = btnRecs[TITLE].Center.X - fonts[BOLD].MeasureString(promptPlayerTexts[USER_NAME]).X / 2;
                                promptLocs[USER_NAME].Y = btnRecs[TITLE].Bottom + TITLE_OFFSET / 2;
                                infoLocs[USER_NAME].X = btnRecs[MENU].X;
                                infoLocs[USER_NAME].Y = promptLocs[USER_NAME].Y + TITLE_OFFSET;

                                //Play the button sound
                                btnSnd.CreateInstance().Play();

                                //Set that the user clicked the menu
                                hasClickedMenu = true;
                            }
                        }
                    }
                    else
                    {
                        //Check if the user has not put in their username
                        if (!hasPutInfo[USER_NAME])
                        {
                            //Allow the user to input their username
                            playerInfoTexts[USER_NAME] = WriteName(playerInfoTexts[USER_NAME]);

                            //Check that their username is greater than 0 characters
                            if (playerInfoTexts[USER_NAME].Length > 0)
                            {
                                //Update the prompt and its location
                                promptPlayerTexts[USER_NAME] = "Press Enter to Continue";
                                promptLocs[USER_NAME].X = btnRecs[TITLE].Center.X - fonts[BOLD].MeasureString(promptPlayerTexts[USER_NAME]).X / 2;

                                //Check if the user pressed enter
                                if (kb.IsKeyDown(Keys.Enter) && !prevKb.IsKeyDown(Keys.Enter))
                                {
                                    //Update the location of the prompt and password text
                                    promptLocs[PASSWORD].X = btnRecs[TITLE].Center.X - fonts[BOLD].MeasureString(promptPlayerTexts[PASSWORD]).X / 2;
                                    promptLocs[PASSWORD].Y = infoLocs[USER_NAME].Y + TITLE_OFFSET;
                                    infoLocs[PASSWORD].X = infoLocs[USER_NAME].X;
                                    infoLocs[PASSWORD].Y = promptLocs[PASSWORD].Y + TITLE_OFFSET;

                                    //Set that the user put in their username
                                    hasPutInfo[USER_NAME] = true;
                                }
                            }
                            else
                            {
                                //Update the prompt text and its location
                                promptPlayerTexts[USER_NAME] = "Enter your Username";
                                promptLocs[USER_NAME].X = btnRecs[TITLE].Center.X - fonts[BOLD].MeasureString(promptPlayerTexts[USER_NAME]).X / 2;
                            }
                        }
                        else
                        {
                            //Check if the player has not put in their password
                            if (!hasPutInfo[PASSWORD])
                            {
                                //Allow the user to input their password
                                playerInfoTexts[PASSWORD] = WriteName(playerInfoTexts[PASSWORD]);

                                //Check that their password is greater than 0 characters
                                if (playerInfoTexts[PASSWORD].Length > 0)
                                {
                                    //Update the prompt and its location
                                    promptPlayerTexts[PASSWORD] = promptPlayerTexts[USER_NAME];
                                    promptLocs[PASSWORD].X = btnRecs[TITLE].Center.X - fonts[BOLD].MeasureString(promptPlayerTexts[PASSWORD]).X / 2;

                                    //Check if the user pressed enter
                                    if (kb.IsKeyDown(Keys.Enter) && !prevKb.IsKeyDown(Keys.Enter))
                                    {
                                        //Set that the user put in their password
                                        hasPutInfo[PASSWORD] = true;

                                        //Check the validity of the login
                                        CheckUserValidity();
                                    }
                                }
                                else
                                {
                                    //Reset the prompt text and its location
                                    promptPlayerTexts[PASSWORD] = "Enter your Password";
                                    promptLocs[PASSWORD].X = btnRecs[TITLE].Center.X - fonts[BOLD].MeasureString(promptPlayerTexts[PASSWORD]).X / 2;
                                }
                            }
                            else
                            {
                                //Check played input to determine how to proceed
                                if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
                                {
                                    //Check if the user pressed the button
                                    if (btnRecs[EXIT_BTN].Contains(mouse.Position))
                                    {
                                        //Play the button sound
                                        btnSnd.CreateInstance().Play();

                                        //Check the result of the user's login
                                        if (CheckUserValidity() == INVALID)
                                        {
                                            //Reset back to the title screen
                                            ResetTitle();
                                        }
                                        else
                                        {
                                            //Enter the menu
                                            gameState = MENU;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;
                case MENU:
                    //Loop through the buttons
                    for (int i = 0; i < btnHovers.Length; i++)
                    {
                        //Check if the user clicked
                        if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
                        {
                            //Check if the user clicked a button
                            if (btnRecs[i].Contains(mouse.Position))
                            {
                                //Play the button sound
                                btnSnd.CreateInstance().Play();

                                //Check which button was pressed
                                if (i == PLAY_BTN)
                                {
                                    //Reset the pregame phase and enter it
                                    ResetPreGame();
                                    gameState = PREGAME;
                                }
                                else if (i == STAT_BTN)
                                {
                                    //Reset the stats phase and enter it
                                    ResetStats();
                                    gameState = STATS;
                                }
                                else if (i == INFO_BTN)
                                {
                                    //Enter the info page
                                    gameState = INFO;
                                }
                                else
                                {
                                    //Exit the game
                                    Exit();
                                }
                            }
                        }
                    }
                    break;
                case STATS:
                    //Loop through the stat rectangles
                    for (int i = 0; i < statRecs.Length; i++)
                    {
                        //Check if the stat button is hovered
                        statHovers[i] = statRecs[i].Contains(mouse.Position);

                        //Check if the mouse is pressed
                        if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
                        {
                            //Check if a stat rectangle contains the mouse's position
                            if (statRecs[i].Contains(mouse.Position))
                            {
                                //Play the button sound
                                btnSnd.CreateInstance().Play();

                                //Check if the index is less than the last possible one
                                if (i <= QUAD)
                                {
                                    //Sort the player's by the game type
                                    players = MergeSort(players, 0, players.Count - 1, i);

                                    //Calculate the title rectangle
                                    statTitleRec.X = screenWidth / 2 - gameImgs[i].Width / 2;
                                    statTitleRec.Y = titleRec.Y;
                                    statTitleRec.Width = gameImgs[i].Width;
                                    statTitleRec.Height = gameImgs[i].Height;

                                    //Set the current page to the index
                                    curStat = i;
                                }
                                else
                                {
                                    //Check if the current page is not the default one
                                    if (curStat != DEFAULT)
                                    {
                                        //Go back to the default page
                                        curStat = DEFAULT;
                                    }
                                    else
                                    {
                                        //Go to the menu phase
                                        gameState = MENU;
                                    }
                                }
                            }
                        }
                    }
                    break;
                case INFO:
                    //Loop through the info rectangles
                    for (int i = 0; i < infoRecs.Length; i++)
                    {
                        //Check if the stat button is hovered
                        infoHovers[i] = infoRecs[i].Contains(mouse.Position);

                        //Check if the mouse is pressed
                        if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
                        {
                            //Check if a stat rectangle contains the mouse's position
                            if (infoRecs[i].Contains(mouse.Position))
                            {
                                //Play the button sound
                                btnSnd.CreateInstance().Play();

                                //Check if the index is less than the last possible one
                                if (i <= WHACKER)
                                {
                                    //Calculate the title rectangle
                                    statTitleRec.X = screenWidth / 2 - gameImgs[i].Width / 2;
                                    statTitleRec.Y = titleRec.Y;
                                    statTitleRec.Width = gameImgs[i].Width;
                                    statTitleRec.Height = gameImgs[i].Height;

                                    //Loop through the description text locations that are not the subtitles
                                    for (int j = TIPS + 1; j < descTextLocs.Length; j++)
                                    {
                                        //Check which subtitle the description text belongs to
                                        if (j - (TIPS + 1) == GOAL)
                                        {
                                            //Calculate the description text's x location
                                            descTextLocs[j].X = screenWidth / 2 - fonts[BOLD].MeasureString(goalTexts[i]).X / 2;
                                        }
                                        else if (j - (TIPS + 1) == MOVE)
                                        {
                                            //Calculate the description text's x location
                                            descTextLocs[j].X = screenWidth / 2 - fonts[BOLD].MeasureString(moveTexts[i]).X / 2;
                                        }
                                        else
                                        {
                                            //Calculate the description text's x location
                                            descTextLocs[j].X = screenWidth / 2 - fonts[BOLD].MeasureString(tipsTexts[i]).X / 2;
                                        }
                                    }

                                    //Set the current page to the index
                                    curInfo = i;
                                }
                                else
                                {
                                    //Check if the current page is not the default one
                                    if (curInfo != DEFAULT)
                                    {
                                        //Go back to the default page
                                        curInfo = DEFAULT;
                                    }
                                    else
                                    {
                                        //Go to the menu phase
                                        gameState = MENU;
                                    }
                                }
                            }
                        }
                    }
                    break;
                case RESULTS:
                    //Check if the user clicked
                    if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
                    {
                        //Check if the continue button was pressed
                        if (btnRecs[EXIT_BTN].Contains(mouse.Position))
                        {
                            //Play the button sound
                            btnSnd.CreateInstance().Play();

                            //Set the current game to the next one, set up the player, play the gameplay song, and enter the gameplay phase
                            curGame = minigameQueue.Dequeue();
                            player.Setup(curGame.GetType(), screenWidth, screenHeight);
                            MediaPlayer.Play(songs[PLAY_MUSIC]);
                            gameState = GAMEPLAY;
                        }
                    }
                    break;
                case ENDGAME:
                    //Check if the user clicked
                    if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
                    {
                        //Check if the continue button was pressed
                        if (btnRecs[EXIT_BTN].Contains(mouse.Position))
                        {
                            //Play the button sound
                            btnSnd.CreateInstance().Play();

                            //Save the players points, save the stats file, and enter the menu
                            player.SavePoints();
                            SaveStats();
                            gameState = MENU;
                        }
                    }
                    break;
                case PREGAME:
                    //Check if the user has not chosen an option
                    if (!hasChosenOption)
                    {
                        //Loop through the option button hovers
                        for (int i = 0; i < optionHovers.Length; i++)
                        {
                            //Hover the buttons
                            optionHovers[i] = optionRecs[i].Contains(mouse.Position);

                            //Check if the user clicked
                            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
                            {
                                //Check if the user clicked one of the options
                                if (optionRecs[i].Contains(mouse.Position))
                                {
                                    //Play the button sound
                                    btnSnd.CreateInstance().Play();

                                    //Check the clicked option
                                    if (i == SINGLE)
                                    {
                                        //Create a minigame queue with a size of one
                                        minigameQueue = new GameQueue(1);
                                    }
                                    else if (i == TRI_PACK)
                                    {
                                        //Create a minigame queue with a size of three
                                        minigameQueue = new GameQueue(3);
                                    }
                                    else
                                    {
                                        //Create a minigame queue with a size of four
                                        minigameQueue = new GameQueue(4);
                                    }

                                    //Set the chosen game type at the corresponding index to true, and set that the user chose a game type
                                    chosenGameType[i] = true;
                                    hasChosenOption = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        //Allow the player to drag and drop games into the order dropboxes
                        HandleDragAndDrop();

                        //Check the chosen game type 
                        if (chosenGameType[SINGLE])
                        {
                            //Check if the box is full at the first index
                            if (isBoxFull[ONE])
                            {
                                //Set that the user chose an order
                                hasChosenOrder = true;
                            }
                            else
                            {
                                //Set that the user did not choose an order
                                hasChosenOrder = false;
                            }
                        }
                        else if (chosenGameType[TRI_PACK])
                        {
                            //Loop through the number of drop boxes
                            for (int i = 0; i < FOUR; i++)
                            {
                                //Check if the box is not full
                                if (!isBoxFull[i])
                                {
                                    //Set that the player did not choose their order and break out of the loop
                                    hasChosenOrder = false;
                                    break;
                                }
                                else
                                {
                                    //Set that the user chose an order
                                    hasChosenOrder = true;
                                }
                            }
                        }
                        else
                        {
                            //Loop through the number of drop boxes
                            for (int i = 0; i < isBoxFull.Length; i++)
                            {
                                //Check if the box is not full
                                if (!isBoxFull[i])
                                {
                                    //Set that the player did not choose their order and break out of the loop
                                    hasChosenOrder = false;
                                    break;
                                }
                                else
                                {
                                    //Set that the user chose an order
                                    hasChosenOrder = true;
                                }
                            }
                        }

                        //Check if the user chose an order
                        if (hasChosenOrder)
                        {
                            //Check if the user clicked
                            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
                            {
                                //Check if the continue button was pressed
                                if (btnRecs[EXIT_BTN].Contains(mouse.Position))
                                {
                                    //Play the button sound
                                    btnSnd.CreateInstance().Play();

                                    //Loop through the chosen games
                                    for (int i = 0; i < chosenGames.Length; i++)
                                    {
                                        //Store a temporary minigame
                                        Minigame temp;

                                        //Check the chosen game
                                        if (chosenGames[i] == SHOOTER)
                                        {
                                            //Set the temp minigame to the alien shooter
                                            temp = new AlienShooter(fonts, gameBgs[SHOOTER], alienImgs, bulletImgs, splatImgs, hitSnds, poisonSnd, shootSnd, screenWidth, screenHeight);
                                        }
                                        else if (chosenGames[i] == RUNNER)
                                        {
                                            //Set the temp minigame to the space run
                                            temp = new SpaceRun(fonts, gameBgs[RUNNER], spaceObjImgs, blankTexture, runnerSounds, screenWidth, screenHeight);
                                        }
                                        else if (chosenGames[i] == CATCHER)
                                        {
                                            //Set the temp minigame to the star catcher
                                            temp = new StarCatcher(fonts, gameBgs[CATCHER], selectorImgs, starImgs, spaceObjImgs[SpaceRun.ASTEROID], catcherSounds, screenWidth, screenHeight);
                                        }
                                        else
                                        {
                                            //Set the temp minigame to the whack-a-'roid 
                                            temp = new WhackAnAsteroid(fonts, gameBgs[WHACKER], whackerImgs, whackerSounds, screenWidth, screenHeight);
                                        }

                                        //Enqueue the temp minigame into the queue
                                        minigameQueue.Enqueue(temp);
                                    }

                                    //Set the current minigame to the first one dequeued, setup the player, play the gameplay song, and go to the gameplay phase
                                    curGame = minigameQueue.Dequeue();
                                    player.Setup(curGame.GetType(), screenWidth, screenHeight);
                                    MediaPlayer.Play(songs[PLAY_MUSIC]);
                                    gameState = GAMEPLAY;
                                }
                            }
                        }
                    }
                    break;
                case GAMEPLAY:
                    //Check if the current game is active
                    if (curGame.IsActive())
                    {
                        //Check if the game is the whacker to determine mouse visibility
                        if (curGame.GetType() == WHACKER)
                        {
                            //Set the mouse to invisible
                            IsMouseVisible = false;
                        }

                        //Update the current minigame
                        curGame.UpdateGame(gameTime, player, kb, prevKb, mouse, prevMouse);
                    }
                    else
                    {
                        //Play the menu song
                        MediaPlayer.Play(songs[MENU_MUSIC]);

                        //Check if the mouse is not visible
                        if (!IsMouseVisible)
                        {
                            //Set the mouse back to visible
                            IsMouseVisible = true;
                        }

                        //Set the player's points in the most recent game and add it to the total points in the playthrough
                        player.SetPoints(curGame.GetType(), curGame.GetPts());
                        totPts += player.GetCurPoints(curGame.GetType());

                        //Set the results texts
                        resultsPtsText[LAST] = "GAME POINTS: " + player.GetCurPoints(curGame.GetType());
                        resultsPtsText[NEXT] = "TOTAL POINTS: " + totPts;
                        resultsGameTexts[LAST] = "LAST GAME: " + gameBtnTexts[curGame.GetType()];

                        //Check if a minigame remains in the queue
                        if (minigameQueue.Peek() is Minigame)
                        {
                            //Set the next game text
                            resultsGameTexts[NEXT] = "NEXT GAME: " + gameBtnTexts[minigameQueue.Peek().GetType()];
                        }

                        //Setup the results text locations
                        resultsGameLocs[LAST].X = screenWidth / 2 - fonts[BIG].MeasureString(resultsGameTexts[LAST]).X / 2;
                        resultsGameLocs[LAST].Y = TITLE_OFFSET * 2;
                        resultsPtsLocs[LAST].X = screenWidth / 2 - fonts[BIG].MeasureString(resultsPtsText[LAST]).X / 2;
                        resultsPtsLocs[LAST].Y = resultsGameLocs[LAST].Y + TITLE_OFFSET * 2;
                        resultsGameLocs[NEXT].X = screenWidth / 2 - fonts[BIG].MeasureString(resultsGameTexts[NEXT]).X / 2;
                        resultsGameLocs[NEXT].Y = resultsPtsLocs[LAST].Y + TITLE_OFFSET * 2;
                        resultsPtsLocs[NEXT].X = screenWidth / 2 - fonts[BIG].MeasureString(resultsPtsText[NEXT]).X / 2;
                        resultsPtsLocs[NEXT].Y = resultsGameLocs[NEXT].Y + TITLE_OFFSET * 2;

                        //Check if a game is up next
                        if (minigameQueue.Peek() != null)
                        {
                            //Change the gamestate to results
                            gameState = RESULTS;
                        }
                        else
                        {
                            //Check the chosen game type
                            if (chosenGameType[TRI_PACK])
                            {
                                //Save the player's triathlon stats
                                player.SaveTri(chosenGames[ONE], chosenGames[TWO], chosenGames[THREE]);
                            }
                            else if (chosenGameType[FOUR_PACK])
                            {
                                //Save the player's quadrathlon stats
                                player.SaveQuad();
                            }

                            //Set the total points text and location
                            totalText = "TOTAL POINTS: " + totPts;
                            totalLoc.X = screenWidth / 2 - fonts[BIG].MeasureString(totalText).X / 2;
                            totalLoc.Y = TITLE_OFFSET * 2;

                            //Loop through the chosen games
                            for (int i = 0; i < chosenGames.Length; i++)
                            {
                                //Check if the chosen game is a game
                                if (chosenGames[i] != DEFAULT)
                                {
                                    //Set the engame texts and locations
                                    endGameTexts[i] = gameBtnTexts[chosenGames[i]] + ": " + player.GetCurPoints(chosenGames[i]);
                                    endGameLocs[i].X = screenWidth / 2 - fonts[BIG].MeasureString(endGameTexts[i]).X / 2;
                                    endGameLocs[i].Y = totalLoc.Y + TITLE_OFFSET * 2 + TITLE_OFFSET * 2 * i;
                                }
                            }

                            //Enter the endgame phase
                            gameState = ENDGAME;
                        }
                    }
                    break;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            //Check that the gamestate is not gameplay
            if (gameState != GAMEPLAY)
            {
                //Draw the background image
                spriteBatch.Draw(bgs[gameState], bgRec, Color.White);
            }

            //Check the gameState to determine what phase to draw
            switch (gameState)
            {
                case TITLE:
                    //Draw the title image
                    spriteBatch.Draw(titleImg, titleRec, Color.White);

                    //Draw the menu button
                    spriteBatch.Draw(btnImg, btnRecs[PLAY_BTN], Color.White);

                    //Check the button's hover status or whether or not the user has clicked to enter the menu in order to draw the button text
                    if (btnHovers[TITLE] || hasClickedMenu)
                    {
                        //Draw the hovered button text
                        spriteBatch.DrawString(fonts[BTN], titleBtnText, titleBtnLoc, Color.Gold);
                    }
                    else
                    {
                        //Draw the button text
                        spriteBatch.DrawString(fonts[BTN], titleBtnText, titleBtnLoc, Color.White);
                    }

                    //Check if the player has entered their password
                    if (hasPutInfo[PASSWORD])
                    {
                        //Draw the continue button
                        spriteBatch.Draw(btnImg, btnRecs[EXIT_BTN], Color.White);

                        //Check the button's hover status in order to draw the button text
                        if (btnHovers[EXIT_BTN])
                        {
                            //Draw the hovered button text
                            spriteBatch.DrawString(fonts[BTN], continueText, continueLoc, Color.Gold);
                        }
                        else
                        {
                            //Draw the button text
                            spriteBatch.DrawString(fonts[BTN], continueText, continueLoc, Color.White);
                        }
                    }

                    //Loop through the number of prompting texts to draw the prompts and the player information texts
                    for (int i = 0; i < promptLocs.Length; i++)
                    {
                        //Draw the prompt text and the player info texts
                        spriteBatch.DrawString(fonts[BOLD], promptPlayerTexts[i], promptLocs[i], Color.Gold);
                        spriteBatch.DrawString(fonts[BOLD], playerInfoTexts[i], infoLocs[i], Color.White);
                    }

                    //Draw the result of the player's input
                    spriteBatch.DrawString(fonts[BOLD], resultText, resultLoc, Color.Gold);
                    break;
                case MENU:
                    //Draw the title image
                    spriteBatch.Draw(titleImg, titleRec, Color.White);

                    //Loop through the buttons to draw them
                    for (int i = 0; i < btnRecs.Length; i++)
                    {
                        //Draw the button
                        spriteBatch.Draw(btnImg, btnRecs[i], Color.White);

                        //Check the hover status of the buttons
                        if (btnHovers[i])
                        {
                            //Draw the hovered button text
                            spriteBatch.DrawString(fonts[BTN], menuBtnTexts[i], menuBtnLocs[i], Color.Gold);
                        }
                        else
                        {
                            //Draw the button text
                            spriteBatch.DrawString(fonts[BTN], menuBtnTexts[i], menuBtnLocs[i], Color.White);
                        }
                    }
                    break;
                case STATS:
                    //Loop through all the stat buttons except the last
                    for (int i = 0; i <= QUAD; i++)
                    {
                        //Draw the stat button rectangle
                        spriteBatch.Draw(btnImg, statRecs[i], Color.White);

                        //Check the hover status of the buttons
                        if (statHovers[i])
                        {
                            //Draw the hovered button text
                            spriteBatch.DrawString(fonts[REG], gameBtnTexts[i], statBtnLocs[i], Color.Gold);
                        }
                        else
                        {
                            //Draw the button text
                            spriteBatch.DrawString(fonts[REG], gameBtnTexts[i], statBtnLocs[i], Color.White);
                        }
                    }

                    //Draw the go back button
                    spriteBatch.Draw(btnImg, statRecs[QUAD + 1], Color.White);

                    //Check the hover status of the buttons
                    if (statHovers[QUAD + 1])
                    {
                        //Draw the hovered button text
                        spriteBatch.DrawString(fonts[BTN], backText, statBtnLocs[QUAD + 1], Color.Gold);
                    }
                    else
                    {
                        //Draw the button text
                        spriteBatch.DrawString(fonts[BTN], backText, statBtnLocs[QUAD + 1], Color.White);
                    }

                    //Check the current stat page being viewed
                    if (curStat == DEFAULT)
                    {
                        //Draw the title image
                        spriteBatch.Draw(titleImg, titleRec, Color.White);

                        //Draw the player's points text
                        spriteBatch.DrawString(fonts[BTN], ptsText, playerNameLoc, Color.Gold);

                        //Loop through the games
                        for (int i = 0; i <= QUAD; i++)
                        {
                            //Draw the player's score at the game
                            spriteBatch.DrawString(fonts[BOLD], gameBtnTexts[i] + ": " + player.GetPoints(i), playerScoreLocs[i], Color.White);
                        }
                    }
                    else
                    {
                        //Draw the title image
                        spriteBatch.Draw(gameImgs[curStat], statTitleRec, Color.White);

                        //Loop through the stat info texts
                        for (int i = 0; i < statInfoTexts.Length; i++)
                        {
                            //Draw the stat info texts
                            spriteBatch.DrawString(fonts[BTN], statInfoTexts[i], statInfoLocs[ONE, i], Color.Gold);
                        }

                        //Loop through the stat location rows
                        for (int i = 0; i < statLocs.GetLength(0); i++)
                        {
                            //Loop through the stat location columns
                            for (int j = 0; j < statLocs.GetLength(1); j++)
                            {
                                //Check if the row is less than the player count in order to ensure unnecessary rows aren't drawn
                                if (i < players.Count)
                                {
                                    //Check the column index
                                    if (j == RANK)
                                    {
                                        //Draw the rank
                                        spriteBatch.DrawString(fonts[BOLD], "" + (i + 1), statLocs[i, j], Color.Gold);
                                    }
                                    else if (j == NAME)
                                    {
                                        //Draw the player's name
                                        spriteBatch.DrawString(fonts[BOLD], players[i].GetName(USER_NAME), statLocs[i, j], Color.Gold);
                                    }
                                    else
                                    {
                                        //Draw the player's points
                                        spriteBatch.DrawString(fonts[BOLD], "" + players[i].GetPoints(curStat), statLocs[i, j], Color.Gold);
                                    }
                                }
                            }
                        }
                    }
                    break;
                case INFO:
                    //Loop through the info buttons except the last one
                    for (int i = 0; i < infoRecs.Length - 1; i++)
                    {
                        //Draw the button
                        spriteBatch.Draw(btnImg, infoRecs[i], Color.White);

                        //Check the hover status of the button
                        if (infoHovers[i])
                        {
                            //Draw the hovered button text
                            spriteBatch.DrawString(fonts[REG], gameBtnTexts[i], infoBtnLocs[i], Color.Gold);
                        }
                        else
                        {
                            //Draw the button text
                            spriteBatch.DrawString(fonts[REG], gameBtnTexts[i], infoBtnLocs[i], Color.White);
                        }
                    }

                    //Draw the go back button
                    spriteBatch.Draw(btnImg, infoRecs[TRI], Color.White);

                    //Check the hover status of the go back button
                    if (infoHovers[TRI])
                    {
                        //Draw the hovered button text
                        spriteBatch.DrawString(fonts[BTN], backText, statBtnLocs[QUAD + 1], Color.Gold);
                    }
                    else
                    {
                        //Draw the button text
                        spriteBatch.DrawString(fonts[BTN], backText, statBtnLocs[QUAD + 1], Color.White);
                    }

                    //Check if the current page is the default one
                    if (curInfo == DEFAULT)
                    {
                        //Draw the title image
                        spriteBatch.Draw(titleImg, titleRec, Color.White);

                        //Draw the general description text
                        spriteBatch.DrawString(fonts[BOLD], descText, descLoc, Color.Gold);
                    }
                    else
                    {
                        //Draw the title image
                        spriteBatch.Draw(gameImgs[curInfo], statTitleRec, Color.White);

                        //Loop through the stat info texts
                        for (int i = 0; i <= TIPS; i++)
                        {
                            //Draw the stat info texts
                            spriteBatch.DrawString(fonts[BTN], descTexts[i], descTextLocs[i], Color.Gold);
                        }

                        //Loop through the stat location rows
                        for (int i = TIPS + 1; i < descTextLocs.Length; i++)
                        {
                            //Check the column index
                            if (i - (TIPS + 1) == GOAL)
                            {
                                //Draw the goal text
                                spriteBatch.DrawString(fonts[BOLD], goalTexts[curInfo], descTextLocs[i], Color.Gold);
                            }
                            else if (i - (TIPS + 1) == MOVE)
                            {
                                //Draw the movement text
                                spriteBatch.DrawString(fonts[BOLD], moveTexts[curInfo], descTextLocs[i], Color.Gold);
                            }
                            else
                            {
                                //Draw the tips text
                                spriteBatch.DrawString(fonts[BOLD], tipsTexts[curInfo], descTextLocs[i], Color.Gold);
                            }
                        }
                    }
                    break;
                case RESULTS:
                    //Loop through the result locations
                    for (int i = 0; i < resultsGameLocs.Length; i++)
                    {
                        //Draw the results points text and the game texts
                        spriteBatch.DrawString(fonts[BIG], resultsPtsText[i], resultsPtsLocs[i], Color.Gold);
                        spriteBatch.DrawString(fonts[BIG], resultsGameTexts[i], resultsGameLocs[i], Color.Gold);
                    }

                    //Draw the continue button
                    spriteBatch.Draw(btnImg, btnRecs[EXIT_BTN], Color.White);

                    //Check the button's hover status in order to draw the button text
                    if (btnHovers[EXIT_BTN])
                    {
                        //Draw the hovered button text
                        spriteBatch.DrawString(fonts[BTN], continueText, continueLoc, Color.Gold);
                    }
                    else
                    {
                        //Draw the button text
                        spriteBatch.DrawString(fonts[BTN], continueText, continueLoc, Color.White);
                    }
                    break;
                case ENDGAME:
                    //Draw the total points text
                    spriteBatch.DrawString(fonts[BIG], totalText, totalLoc, Color.Gold);

                    //Loop through the endgame locations
                    for (int i = 0; i < endGameLocs.Length; i++)
                    {
                        //Check that the chosen game is valid
                        if (chosenGames[i] != DEFAULT)
                        {
                            //Draw the scores for the game
                            spriteBatch.DrawString(fonts[BIG], endGameTexts[i], endGameLocs[i], Color.Gold);
                        }
                    }

                    //Draw the continue button
                    spriteBatch.Draw(btnImg, btnRecs[EXIT_BTN], Color.White);

                    //Check the button's hover status in order to draw the button text
                    if (btnHovers[EXIT_BTN])
                    {
                        //Draw the hovered button text
                        spriteBatch.DrawString(fonts[BTN], continueText, continueLoc, Color.Gold);
                    }
                    else
                    {
                        //Draw the button text
                        spriteBatch.DrawString(fonts[BTN], continueText, continueLoc, Color.White);
                    }
                    break;
                case PREGAME:
                    //Check if the player has not chosen an option yet
                    if (!hasChosenOption)
                    {
                        //Draw the title image
                        spriteBatch.Draw(titleImg, titleRec, Color.White);

                        //Loop through the game type options
                        for (int i = 0; i < optionRecs.Length; i++)
                        {
                            //Draw the option
                            spriteBatch.Draw(btnImg, optionRecs[i], Color.White);

                            //Check if the user is hovering over the option
                            if (optionHovers[i])
                            {
                                //Draw the option text in gold
                                spriteBatch.DrawString(fonts[BOLD], optionTexts[i], optionLocs[i], Color.Gold);
                            }
                            else
                            {
                                //Draw the regular option text
                                spriteBatch.DrawString(fonts[BOLD], optionTexts[i], optionLocs[i], Color.White);
                            }
                        }
                    }
                    else
                    {
                        //Check which option was chosen
                        if (chosenGameType[SINGLE])
                        {
                            //Draw the order screen elements
                            DrawOrderingScreen(ONE);
                        }
                        else if (chosenGameType[TRI_PACK])
                        {
                            //Loop through the drop rectangles
                            for (int i = 0; i < dropRecs.Length - 1; i++)
                            {
                                //Draw the order screen elements
                                DrawOrderingScreen(i);
                            }
                        }
                        else
                        {
                            //Loop through the drop rectangles
                            for (int i = 0; i < dropRecs.Length; i++)
                            {
                                //Draw the order screen elements
                                DrawOrderingScreen(i);
                            }
                        }

                        //Loop through the drag rectangles
                        for (int i = 0; i < dragRecs.Length; i++)
                        {
                            //Draw the drag images
                            spriteBatch.Draw(gameImgs[i], dragRecs[i], Color.White);
                        }

                        //Check if the user has chosen the order yet
                        if (!hasChosenOrder)
                        {
                            //Draw the ordering information text
                            spriteBatch.DrawString(fonts[BOLD], orderText, orderLoc, Color.Gold);
                        }
                        else
                        {
                            //Draw the continue button
                            spriteBatch.Draw(btnImg, btnRecs[EXIT_BTN], Color.White);

                            //Check the button's hover status in order to draw the button text
                            if (btnHovers[EXIT_BTN])
                            {
                                //Draw the hovered button text
                                spriteBatch.DrawString(fonts[BTN], continueText, continueLoc, Color.Gold);
                            }
                            else
                            {
                                //Draw the button text
                                spriteBatch.DrawString(fonts[BTN], continueText, continueLoc, Color.White);
                            }
                        }
                    }
                    break;
                case GAMEPLAY:
                    //Draw the current game
                    curGame.Draw(spriteBatch, player);
                    break;
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        //Pre: none
        //Post: none
        //Desc: resets the title phase
        private void ResetTitle()
        {
            //Set that the user did not click the menu
            hasClickedMenu = false;

            //Loop through the player info texts
            for (int i = 0; i < playerInfoTexts.Length; i++)
            {
                //Set the prompts offscreen
                promptLocs[i].Y = -200;

                //Reset the player's information and their putting info status
                playerInfoTexts[i] = "";
                hasPutInfo[i] = false;
            }

            //Update the prompt text
            promptPlayerTexts[USER_NAME] = "Enter your Username";
            promptPlayerTexts[PASSWORD] = "Enter your Password";

            //Set the result location to offscreen
            resultLoc.Y = -200;

            //Enter the title
            gameState = TITLE;
        }

        //Pre: none
        //Post: none
        //Desc: resets the stat page
        private void ResetStats()
        {
            //Calculate the player text and its location
            ptsText = player.GetName(USER_NAME) + "'s " + "High Scores";
            playerNameLoc.X = screenWidth / 2 - fonts[BTN].MeasureString(ptsText).X / 2;
            playerNameLoc.Y = titleRec.Bottom + SIDE_OFFSET;

            //Loop through the player game stats
            for (int i = 0; i <= QUAD; i++)
            {
                //Update the location of the player's scores
                playerScoreLocs[i].X = screenWidth / 2 - fonts[BOLD].MeasureString(gameBtnTexts[i] + ": " + player.GetPoints(i)).X / 2;
                playerScoreLocs[i].Y = playerNameLoc.Y + fonts[BTN].MeasureString(ptsText).Y + SIDE_OFFSET / 2 + TITLE_OFFSET * i;
            }

            //Loop through the stat hover statuses
            for (int i = 0; i < statHovers.Length; i++)
            {
                //Set the hover status to false
                statHovers[i] = false;
            }
        }

        //Pre: none
        //Post: none
        //Desc: resets the pregame phase
        private void ResetPreGame()
        {
            //Set the players points to none
            totPts = 0;

            //Set that the user has not chose a game type or an order for the games
            hasChosenOption = false;
            hasChosenOrder = false;

            //Set the dragged rectangle index to the default index
            draggedRectangleIndex = DEFAULT;

            //Loop through the dragged rectangles
            for (int i = 0; i < dragRecs.Length; i++)
            {
                //Reset the rectangles
                dragRecs[i].X = ogDragRecs[i].X;
                dragRecs[i].Y = ogDragRecs[i].Y;
            }

            //Loop through the chosen games
            for (int i = 0; i < chosenGames.Length; i++)
            {
                //Reset the chosen games, and the drag and drop booleans
                chosenGames[i] = DEFAULT;
                hasBeenDropped[i] = false;
                isBoxFull[i] = false;
                isXHovered[i] = false;
            }

            //Loop through the game types
            for (int i = 0; i < chosenGameType.Length; i++)
            {
                //Reset the option hovers and set the chosen game types to false
                optionHovers[i] = false;
                chosenGameType[i] = false;
            }
        }

        //Pre: none
        //Post: returns the name text
        //Desc: updates the player's username/password by checking if there is space to write, and then checking their keyboard input
        private string WriteName(string nameText)
        {
            //Check if there is space for the player to add letters to their username
            if (nameText.Length < MAX_CHARS)
            {
                //Loop through the number of keys
                for (int i = 0; i < keys.Length; i++)
                {
                    //Check if a key is pressed
                    if (kb.IsKeyDown(keys[i]) && !prevKb.IsKeyDown(keys[i]))
                    {
                        //Check if the shift button is pressed
                        if (kb.IsKeyDown(Keys.LeftShift) || kb.IsKeyDown(Keys.RightShift))
                        {
                            //Add the uppercase letter
                            nameText += letters[i].ToUpper();
                        }
                        else
                        {
                            //Add the lowercase letter
                            nameText += letters[i];
                        }
                    }
                }
            }

            //Check if the user pressed keys that are for other purposes than adding letters to the username
            if (kb.IsKeyDown(Keys.Back) && !prevKb.IsKeyDown(Keys.Back))
            {
                //Check if the username is longer than 0 to allow the player to delete letters
                if (!nameText.Equals(""))
                {
                    //Delete the most recent letter
                    nameText = nameText.Remove(nameText.Length - 1);
                }
            }

            //Return the name
            return nameText;
        }

        //Pre: none
        //Post: returns the validity of the player's login
        //Desc: checks the validity of the player's login
        private int CheckUserValidity()
        {
            //Create the counter
            int count = 0;

            //Set the location of the result
            resultLoc.Y = infoLocs[PASSWORD].Y + TITLE_OFFSET;

            //Loop through the players
            while (count < players.Count)
            {
                //Check if the player inputted an existing username
                if (playerInfoTexts[USER_NAME].Equals(players[count].GetName(USER_NAME)))
                {
                    //Check if the player entered the correct password
                    if (playerInfoTexts[PASSWORD].Equals(players[count].GetName(PASSWORD)))
                    {
                        //Set the player to the one at the count
                        player = players[count];

                        //Setup the results text and its location
                        resultText = "Welcome Back, " + player.GetName(USER_NAME) + "!";
                        resultLoc.X = btnRecs[TITLE].Center.X - fonts[BOLD].MeasureString(resultText).X / 2;

                        //Return a valid login
                        return VALID;
                    }
                    else
                    {
                        //Setup the results text and its location
                        resultText = "Wrong Password, Try Again!";
                        resultLoc.X = btnRecs[TITLE].Center.X - fonts[BOLD].MeasureString(resultText).X / 2;

                        //Return an invalid login
                        return INVALID;
                    }
                }

                //Increase the counter
                count++;
            }

            //Create a temporary scores array
            int[] tempScores = new int[6];

            //Loop through the scores array
            for (int i = 0; i <= QUAD; i++)
            {
                //Set the points to 0
                tempScores[i] = 0;
            }

            //Add the player to the list, and set the current player to the added one
            players.Add(new Player(playerInfoTexts, tempScores, playerImgs, bulletImgs[AlienShooter.PLAYER], flameImgs, playerSnds));
            player = players.Last();

            //Setup the results text and its location
            resultText = "Welcome " + player.GetName(USER_NAME) + "!";
            resultLoc.X = btnRecs[TITLE].Center.X - fonts[BOLD].MeasureString(resultText).X / 2;

            //Return a new login
            return NEW;
        }

        //Pre: none
        //Post: none
        //Desc: handles drag and drop in the pregame
        private void HandleDragAndDrop()
        {
            //Loop through the hover statuses of the removers
            for (int i = 0; i < isXHovered.Length; i++)
            {
                //Calculate the hover status
                isXHovered[i] = removeRecs[i].Contains(mouse.Position);
            }

            //Check user input
            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released)
            {
                //Loop through the dragged rectangles
                for (int i = 0; i < dragRecs.Length; i++)
                {
                    //Check if a rectangle was pressed and it has not yet been dropped
                    if (dragRecs[i].Contains(mouse.Position) && !hasBeenDropped[i])
                    {
                        //Store the index of the dragged rectangle and break the loop
                        draggedRectangleIndex = i;
                        break;
                    }
                    else if (removeRecs[i].Contains(mouse.Position) && isBoxFull[i])
                    {
                        //Check the chosen game type
                        if (chosenGameType[SINGLE])
                        {
                            //Check if the index is less than the 2nd one
                            if (i < TWO)
                            {
                                //Remove the dragged rectangle from the box
                                HandleRemoval(i);
                            }
                        }
                        else if (chosenGameType[TRI_PACK])
                        {
                            //Check if the index is less than the last one
                            if (i < FOUR)
                            {
                                //Remove the dragged rectangle from the box
                                HandleRemoval(i);
                            }
                        }
                        else
                        {
                            //Remove the dragged rectangle from the box
                            HandleRemoval(i);
                        }
                    }
                }
            }
            else if (mouse.LeftButton == ButtonState.Released && prevMouse.LeftButton == ButtonState.Pressed)
            {
                //Check if the dragged rectangle is valid
                if (draggedRectangleIndex != DEFAULT)
                {
                    //Loop through the drop rectangles
                    for (int i = 0; i < dropRecs.Length; i++)
                    {
                        //Check if the drop rectangle contains the mouse position and that its box is not yet full
                        if (dropRecs[i].Contains(mouse.Position) && !isBoxFull[i])
                        {
                            //Check the chosen game type
                            if (chosenGameType[SINGLE])
                            {
                                //Check if the index is less than the 2nd one
                                if (i < TWO)
                                {
                                    //Drop the rectangle
                                    HandleDrop(i);
                                }
                            }
                            else if (chosenGameType[TRI_PACK])
                            {
                                //Check if the index is less than the last one
                                if (i < FOUR)
                                {
                                    //Drop the rectangle
                                    HandleDrop(i);
                                }
                            }
                            else
                            {
                                //Drop the rectangle
                                HandleDrop(i);
                            }
                        }
                    }
                }

                //Stop dragging
                draggedRectangleIndex = DEFAULT;
            }
            else if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Pressed && draggedRectangleIndex != DEFAULT)
            {
                //Update the position of the dragged rectangle to the current mouse position, and set its bounds
                dragRecs[draggedRectangleIndex].X = mouse.Position.X;
                dragRecs[draggedRectangleIndex].Y = mouse.Position.Y;
                dragRecs[draggedRectangleIndex].X = MathHelper.Clamp(dragRecs[draggedRectangleIndex].X, 0, screenWidth - dragRecs[draggedRectangleIndex].Width);
                dragRecs[draggedRectangleIndex].Y = MathHelper.Clamp(dragRecs[draggedRectangleIndex].Y, 0, (int)orderLoc.Y - dragRecs[draggedRectangleIndex].Height);
            }
        }

        //Pre: idx is the index of the drop box
        //Post: none
        //Desc: handles the removal of a dragged rectangle from its box
        private void HandleRemoval(int idx)
        {
            //Reset the dragger's position
            dragRecs[chosenGames[idx]].X = ogDragRecs[chosenGames[idx]].X;
            dragRecs[chosenGames[idx]].Y = ogDragRecs[chosenGames[idx]].Y;

            //Reset the fill status of the box and the drop status of the index
            isBoxFull[idx] = false;
            hasBeenDropped[chosenGames[idx]] = false;

            //Set the chosen game at the index to an invalid one
            chosenGames[idx] = DEFAULT;
        }

        //Pre: idx is the index of the drop box
        //Post: none
        //Desc: handles the dropping of a dragged rectangle into a box
        private void HandleDrop(int idx)
        {
            //Update the dragged rectangles position
            dragRecs[draggedRectangleIndex].X = numCircleRecs[idx].Right + SIDE_OFFSET / 2;
            dragRecs[draggedRectangleIndex].Y = dropRecs[idx].Center.Y - dragRecs[draggedRectangleIndex].Height / 2;

            //Setup the chosen game at the index to the dragged rectangle's index
            chosenGames[idx] = draggedRectangleIndex;

            //Set the fill status and drop status
            isBoxFull[idx] = true;
            hasBeenDropped[draggedRectangleIndex] = true;
        }

        //Pre: idx is the index of the drop box
        //Post: none
        //Desc: draws the drop box and its elements
        private void DrawOrderingScreen(int idx)
        {
            //Draw the drop box and its elements
            spriteBatch.Draw(btnImg, dropRecs[idx], Color.White);
            spriteBatch.Draw(circleImg, numCircleRecs[idx], Color.White);
            spriteBatch.DrawString(fonts[BIG], numberTexts[idx], numberLocs[idx], Color.Gold);

            //Check if the remover is being hovered
            if (isXHovered[idx])
            {
                //Draw the hovered remover
                spriteBatch.Draw(blankTexture, removeHoverRecs[idx], Color.Gold);
            }
            else
            {
                //Draw the unhovered remover
                spriteBatch.Draw(blankTexture, removeHoverRecs[idx], Color.White);
            }

            //Draw the box's remover
            spriteBatch.Draw(removeImg, removeRecs[idx], Color.White);
        }

        //Pre: none
        //Post: none
        //Desc: writes the players stats into a file
        public void SaveStats()
        {
            try
            {
                //Create the stats file
                fileWriter = File.CreateText("Players.txt");

                //Set up the counter
                int count = 0;

                //Loop through all the users, then loop through their stats
                while (count < players.Count)
                {
                    //Write the current player's username and PASSWORD
                    fileWriter.Write(players[count].GetName(USER_NAME) + "," + players[count].GetName(PASSWORD) + ",");

                    //Loop through the users stats
                    for (int i = 0; i <= QUAD; i++)
                    {
                        //Write the player's stat to the file
                        fileWriter.Write(players[count].GetPoints(i));

                        //Check if the last item has not been reached
                        if (i != QUAD)
                        {
                            //Write the data splitter
                            fileWriter.Write(",");
                        }
                        else
                        {
                            //Move to the next line
                            fileWriter.WriteLine();
                        }
                    }

                    //Increase the counter
                    count++;
                }
            }
            catch (FormatException)
            {
                ////Writes a format exception message to the user, pausing the output from changing by checking for user input
                Console.WriteLine("Uh oh, a file formatting error occured, feel free to exit and reopen the game");
                Console.ReadLine();
            }
            catch (FileNotFoundException)
            {
                ////Writes a file not found exception message to the user, pausing the output from changing by checking for user input
                Console.WriteLine("Uh oh, we couldn't find an important file, feel free to exit and reopen the game");
                Console.ReadLine();
            }
            catch (Exception)
            {
                ////Writes a general exception message to the user, pausing the output from changing by checking for user input
                Console.WriteLine("Uh oh, an unknown error occured, feel free to exit and reopen the game");
                Console.ReadLine();
            }
            finally
            {
                //If the file was opened, close the file
                if (fileWriter != null)
                {
                    //Close the stats file
                    fileWriter.Close();
                }
            }
        }

        //Pre: none
        //Post: none
        //Desc: reads the stat file in order to set the user's stats equal to the read data
        public void ReadStats()
        {
            try
            {
                //Open the stats file
                fileReader = File.OpenText("Players.txt");

                //Store the line and its data
                string line;
                string[] data;

                //Set up the counter
                int count = 0;

                //Loop through the file until it finishes in order to add each stat to the current player
                while (!fileReader.EndOfStream)
                {
                    //Store the player's current scores
                    int[] curScores = new int[QUAD + 1];

                    //Store the file's current line
                    line = fileReader.ReadLine();

                    //Store the individual data in the line
                    data = line.Split(',');

                    //Loop through the current scores
                    for (int i = 0; i < curScores.Length; i++)
                    {
                        //Store the current score as the read data
                        curScores[i] = Convert.ToInt32(data[i + PASSWORD + 1]);
                    }

                    //store the player's names
                    string[] names = new string[] { data[USER_NAME], data[PASSWORD] };

                    //Add the player
                    players.Add(new Player(names, curScores, playerImgs, bulletImgs[AlienShooter.PLAYER], flameImgs, playerSnds));

                    //Increase the counter
                    count++;
                }
            }
            catch (FormatException)
            {
                //Writes a format exception message to the user, pausing the output from changing by checking for user input
                Console.WriteLine("Uh oh, a file formatting error occured, feel free to exit and reopen the game");
                Console.ReadLine();
            }
            catch (FileNotFoundException)
            {
                //Write the stats file because it does not exist yet
                SaveStats();
            }
            catch (Exception)
            {
                //Writes a general exception message to the user, pausing the output from changing by checking for user input
                Console.WriteLine("Uh oh, an unknown error occured, feel free to exit and reopen the game");
                Console.ReadLine();
            }
            finally
            {
                //If the file was opened, close the file
                if (fileReader != null)
                {
                    //Close the stats file
                    fileReader.Close();
                }
            }
        }

        //Pre: players is the list of players, left is the lower limit, right is the upper limit, gameType is the game being sorted for
        //Post: returns a merged list of players 
        //Desc: a recursive mergesort method for dividing up the list of players
        private List<Player> MergeSort(List<Player> players, int left, int right, int gameType)
        {
            //Store the mid index
            int mid;

            //Check the element count
            if (left > right)
            {
                //No element so return a null result
                return null;
            }
            else if (left == right)
            {
                //Return the 1 element as an array of 1
                return new List<Player>() { players[left] };
            }

            //Calculate the mid index
            mid = (left + right) / 2;

            //Return the merging of the mergesort methods
            return Merge(MergeSort(players, left, mid, gameType), MergeSort(players, mid + 1, right, gameType), gameType);
        }

        //Pre: left is the list of players on the left, right is the list of players on the right, gameType is the type of game being sorted for
        //Post: returns a sorted list of players
        //Desc: merges and sorts a divided list of players
        private List<Player> Merge(List<Player> left, List<Player> right, int gameType)
        {
            //Check for null sides
            if (left == null)
            {
                //Return the right side
                return right;
            }
            else if (right == null)
            {
                //Return the left side
                return left;
            }

            //Store the result
            List<Player> result = new List<Player>();

            //Set the indexes of both sides
            int idx1 = 0;
            int idx2 = 0;

            //For each element in the merged list, get the next smallest element between the two given lists
            for (int i = 0; i < left.Count + right.Count; i++)
            {
                //Compare the arrays at the indexes
                if (idx1 == left.Count)
                {
                    //Add the element at the index of the right side and increase the right index
                    result.Add(right[idx2]);
                    idx2++;
                }
                else if (idx2 == right.Count)
                {
                    //Add the element at the index of the left side and increase the left index
                    result.Add(left[idx1]);
                    idx1++;
                }
                else if (left[idx1].GetPoints(gameType) >= right[idx2].GetPoints(gameType))
                {
                    //Add the element at the index of the left side and increase the left index
                    result.Add(left[idx1]);
                    idx1++;
                }
                else
                {
                    //Add the element at the index of the right side and increase the right index
                    result.Add(right[idx2]);
                    idx2++;
                }
            }

            //Return the result
            return result;
        }
    }
}
