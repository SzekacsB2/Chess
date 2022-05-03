using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Chess.UserControls
{

    public partial class ChessUserControls : UserControl
    {
        public bool isReplay { get; set; }
        UIEngine engine;
        Action<bool> loadNewGameUserControl;
        System.Windows.Media.Effects.BlurEffect objBlur = new System.Windows.Media.Effects.BlurEffect() { Radius = 5 };
        public ChessUserControls(Action<bool> loadNewGameUserControl)
        {
            InitializeComponent();
           #region Create chessboard made of buttons
            Button[,] physicalBoard = new Button[8, 8];
            for (byte row = 0; row < 8; row++)
            {
                for (byte column = 0; column < 8; column++)
                {
                    Button button = new Button();
                    button.SetValue(Grid.ColumnProperty, (int)column);
                    button.SetValue(Grid.RowProperty, (int)row);
                    button.SetValue(Border.BorderThicknessProperty, new Thickness(0));
                    button.Focusable = false;
                    button.Padding = new Thickness(0);
                    button.Tag = new Coords(row, column);
                    
                    if (row % 2 != column % 2)
                        button.Background = Brushes.DimGray;
                    else
                        button.Background = Brushes.White;
                    
                    Grid g = new Grid();                    
                    g.Children.Add(new UIElement());
                    g.Children.Add(new UIElement());
                    button.Content = g;

                    physicalBoard[row, column] = button;
                    grid.Children.Add(button);
                }
            }
            #endregion
            engine = new UIEngine(physicalBoard, StartMenu, mainBoardGrid, GameMenu, EndReplayMenu, wnStatusBar, wnTextBlock, mStatusBar, mTextBlock);
            this.loadNewGameUserControl = loadNewGameUserControl;
            DataContext = engine;
            isReplay = false;
        }

        #region Controls events and variables
 
        /// <summary>
        /// Square click event
        /// </summary>
        private void grid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            engine.ButtonClick((Coords)((Button)e.Source).Tag);
        }
        
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            engine.LoadedChessUserControl();
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            if (engine.isBot)
            {
                engine.Undo();
                engine.Undo();
            }
            else engine.Undo();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            engine.Save();
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            engine.Load();
        }

        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            engine.NewGame(false);
        }

        private void NewGameBot_Click(object sender, RoutedEventArgs e)
        {
            engine.NewGame(true);
        }

        private void ReplayLoad_Click(object sender, RoutedEventArgs e)
        {
            isReplay = true;
            engine.StartLoadReplay();
        }

        private void ReplayCurrent_Click(object sender, RoutedEventArgs e)
        {
            isReplay = true;
            engine.StartReplay();
        }

        public void OnArrow(bool b)
        {
            engine.OnArrow(b);
        }

        private void EndReplay_Click(object sender, RoutedEventArgs e)
        {
            isReplay = false;
            engine.EndReplay();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            engine.ExitGame();
        }

        #endregion
    }
}
