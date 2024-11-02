using System.Security.AccessControl;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace TetrisGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MediaPlayer backgroundMusic = new MediaPlayer();

        private readonly ImageSource[] tileImages = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/TileEmpty.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileCyan.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileBlue.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileOrange.png",  UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileYellow.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileGreen.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TilePurple.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileRed.png", UriKind.Relative))
        };

        private readonly ImageSource[] blockImages = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/Block-Empty.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-I.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-J.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-L.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-O.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-S.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-T.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-Z.png", UriKind.Relative))
        };

        public readonly Image[,] imageControls;
        private GameState gameState=new GameState();

        private readonly int maxDelay = 1000;
        private readonly int minDelay = 50;
        private readonly int delayDecrease = 500;

        public MainWindow()
        {
            InitializeComponent();
            imageControls=SetupGameCanVas(gameState.GameGrid);
            
            imageControls = SetupGameCanVas(gameState.GameGrid);
            PlayBackgroundMusic();

           


        }
        private void PlayBackgroundMusic()
        {
            try
            {
                backgroundMusic.Open(new Uri("D:/ltWin/game/TetrisGame/TetrisGame/Assets/tetris-theme-korobeiniki-arranged-for-piano-186249.mp3", UriKind.Relative));
                backgroundMusic.Volume = 1.0; // Đặt âm lượng tối đa
                backgroundMusic.MediaEnded += (sender, e) =>
                {
                    backgroundMusic.Position = TimeSpan.Zero;
                    backgroundMusic.Play();
                };
                backgroundMusic.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi phát nhạc: " + ex.Message); // Hiện thông báo lỗi nếu có
            }
        }


        private Image[,] SetupGameCanVas(GameGrid grid)
        {
            Image[,] imageControls = new Image[grid.Rows, grid.Columns];
            int cellSize = 25;
            for(int r =0; r < grid.Rows; r++)
            {
                for(int  c =0; c < grid.Columns; c++)
                {
                    Image imageControl = new Image
                    {
                        Width = cellSize,
                        Height = cellSize,
                    };
                    Canvas.SetTop(imageControl, (r - 2) * cellSize);
                    Canvas.SetLeft(imageControl,c* cellSize);
                    GameCanvas.Children.Add(imageControl);
                    imageControls[r, c] = imageControl;
                }
            }
            return imageControls;
        }

        
        private void DrawGrid(GameGrid grid)
        {
            for (int r = 0; r < grid.Rows; r++)
            {
                for (int c = 0; c < grid.Columns; c++)
                {
                    int id = grid[r, c];
                    imageControls[r, c].Opacity = 1;
                    imageControls[r, c].Source = tileImages[id];
                }
            }
        }

        private void DrawBlock(Block block)
        {
            foreach (Position p in block.TilePositions())
            {
                imageControls[p.Row, p.Column].Opacity = 1;
                imageControls[p.Row, p.Column].Source = tileImages[block.Id];
            }
        }

        private void DrawNextBlock(BlockQueue blockQueue)
        {
            Block next = blockQueue.NextBlock;
            NextImage.Source = blockImages[next.Id];
        }
        private void DrawHeldBlock(Block heldBlock)
        {
            if (heldBlock == null)
            {
                HoldImage.Source = blockImages[0];
            }
            else
            {
                HoldImage.Source = blockImages[heldBlock.Id];
            }
        }
        private void DrawGhostBlock(Block block)
        {
            int dropDistance = gameState.BlockDropDistance();
            foreach (Position p in block.TilePositions())
            {
                imageControls[p.Row + dropDistance, p.Column].Opacity = 0.25;
                imageControls[p.Row + dropDistance, p.Column].Source = tileImages[block.Id];
            }
        }

        private void Draw(GameState gameState)
        {
            DrawGrid(gameState.GameGrid);
            DrawGhostBlock(gameState.CurrentBlock);
            DrawBlock(gameState.CurrentBlock);
            DrawNextBlock(gameState.BlockQueue);
            DrawHeldBlock(gameState.HeldBlock);
            ScoreText.Text = $"Score:{gameState.Score}";

        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(gameState.GameOver)
            {
                return;
            }
            switch(e.Key)
            {
                case Key.Left:
                    gameState.MoveBlockLeft();
                    break;
                case Key.Right:
                    gameState.MoveBlockRight();
                    break;
                case Key.Down:
                    gameState.MoveBlockDown();
                    break;
                case Key.Up:
                    gameState.RotateBlockCW();
                    break;
                case Key.W:
                    gameState.RotateBlockCCW();
                    break;
                case Key.A:
                    gameState.HoldBlock();
                    break;

                case Key.Space:
                    gameState.DropBlock();
                    break;
                case Key.M:
                    ToggleMusic();
                    break;
                default:
                    return;


            }
            Draw(gameState);

        }
        private async Task GameLoop()
        {
            Draw(gameState);
            while(!gameState.GameOver)
            {   
                int delay =Math.Max(maxDelay-(gameState.Score*delayDecrease));
                await Task.Delay(delay);
                gameState.MoveBlockDown();
                Draw(gameState);
            }
            GameOverMenu.Visibility = Visibility.Visible;
            FinalScoreText.Text = $"Score:{gameState.Score}";
        }
        private  async void GameCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            await GameLoop();
        }
        private async  void PlayAgain_Click(object sender, RoutedEventArgs e)
        {
            gameState = new GameState();
            GameOverMenu.Visibility = Visibility.Hidden;
            await GameLoop();


        }

        private void ToggleMusic()
        {
            backgroundMusic.IsMuted = !backgroundMusic.IsMuted;
            MessageBox.Show(backgroundMusic.IsMuted ? "Music is muted!" : "Music is playing!");
        }

        protected override void OnClosed(EventArgs e)
        {
            backgroundMusic.Close();
            base.OnClosed(e);
        }
    }
}