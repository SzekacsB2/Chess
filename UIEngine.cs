using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.Concurrent;
using System.Windows.Threading;
using Microsoft.Win32;
using System.Windows.Controls.Primitives;
using System.IO;

namespace Chess
{
    public struct Coords
    {
        public byte Row { get; set; }
        public byte Column { get; set; }
        public Coords(byte row, byte column)
        {
            Row = row;
            Column = column;
        }

        public override bool Equals(object obj)
        {
            Coords c = (Coords)obj;
            if (Row == c.Row && Column == c.Column) return true;

            return false;
        }
    }

    class UIEngine
    {
        Button[,] physicalBoard;
        Coords[] selectedCoords;
        Game activeGame;
        public Stack<Game> memory = new Stack<Game>();
        public bool isBot { get; set; }
        Bot bot;

        bool isLoad;
        Game[] replayArray;
        int replayCount;
        bool isReplay;

        Grid StartMenu;
        Grid MainBoard;
        Menu GameMenu;
        Menu EndReplayMenu;
        StatusBar wnStatusBar;
        TextBlock wnTextBlock;
        StatusBar mStatusBar;
        TextBlock mTextBlock;


        public UIEngine(Button[,] physicalBoard, Grid StartMenu, Grid MainBoard, Menu GameMenu, Menu EndReplayMenu, StatusBar wnStatusBar, TextBlock wnTextBlock, StatusBar mStatusBar, TextBlock mTextBlock)
        {
            this.physicalBoard = physicalBoard;

            isLoad = false;
            replayArray = memory.ToArray();
            replayCount = 0;
            isReplay = false;

            this.StartMenu = StartMenu;
            this.MainBoard = MainBoard;
            this.GameMenu = GameMenu;
            this.EndReplayMenu = EndReplayMenu;
            this.wnStatusBar = wnStatusBar;
            this.wnTextBlock = wnTextBlock;
            this.mStatusBar = mStatusBar;
            this.mTextBlock = mTextBlock;

        }

        #region User control

        public void Undo()
        {
            if (memory.Count < 2) return;
            memory.Pop();
            activeGame = new Game(memory.Peek());
            ChangeProgressBar();
            DrawPieces();

            wnStatusBar.Visibility = Visibility.Visible;
            mStatusBar.Visibility = Visibility.Hidden;
            mTextBlock.Visibility = Visibility.Hidden;
        }

        public void Save()
        {
            string[] lines = new string[memory.Count + 2];
            if (isBot) lines[0] = "bot";
            else lines[0] = "not";

            Game[] game = memory.ToArray();

            for (int i = 1; i < game.Length + 1; i++)
            {
                string line = "";
                for (int r = 0; r < 8; r++)
                {
                    for (int c = 0; c < 8; c++)
                    {
                        line += game[i - 1].logicalBoard[r, c];
                    }
                }
                lines[i] = line;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text file (*.txt)|*.txt|C# file (*.cs)|*.cs";
            saveFileDialog.FileName = "ChessGame  " + DateTime.Now.ToString();
            if (saveFileDialog.ShowDialog() == true)
                File.AppendAllLines(saveFileDialog.FileName, lines);
        }

        public void Load()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text file (*.txt)|*.txt|C# file (*.cs)|*.cs";
            if (openFileDialog.ShowDialog() == true)
            {
                string[] lines = File.ReadAllLines(openFileDialog.FileName);
                bool WhiteOnTurn = true;
                memory.Clear();

                for (int i = lines.Length - 1; i > 1; i--)
                {
                    string line = lines[i - 1];
                    Game situation = new Game(WhiteOnTurn, line);
                    memory.Push(new Game(situation));
                    WhiteOnTurn = !WhiteOnTurn;
                }

                if (lines[0] == "bot") isBot = true;
                else isBot = false;

                activeGame = new Game(memory.Peek());
                DrawPieces();
                ChangeProgressBar();
                wnStatusBar.Visibility = Visibility.Visible;
                mStatusBar.Visibility = Visibility.Hidden;
                mTextBlock.Visibility = Visibility.Hidden;
                CheckMate(activeGame);

                ResizeWindow(500, 500);
                StartMenu.Visibility = Visibility.Collapsed;
                MainBoard.Visibility = Visibility.Visible;
            }
        }

        public void NewGame(bool isBot)
        {
            memory.Clear();
            activeGame = new Game();
            this.isBot = isBot;
            DrawPieces();
            memory.Push(new Game(activeGame));
            ChangeProgressBar();
            wnStatusBar.Visibility = Visibility.Visible;
            mStatusBar.Visibility = Visibility.Hidden;
            mTextBlock.Visibility = Visibility.Hidden;

            ResizeWindow(500, 500);
            StartMenu.Visibility = Visibility.Collapsed;
            MainBoard.Visibility = Visibility.Visible;
        }

        public void StartLoadReplay()
        {
            Load();
            isLoad = true;
            StartReplay();
        }

        public void StartReplay()
        {
            wnStatusBar.Visibility = Visibility.Visible;
            mStatusBar.Visibility = Visibility.Collapsed;
            mTextBlock.Visibility = Visibility.Collapsed;
            GameMenu.Visibility = Visibility.Hidden;
            EndReplayMenu.Visibility = Visibility.Visible;

            isReplay = true;
            replayArray = memory.ToArray();
            replayCount = memory.Count() - 1;
            activeGame = replayArray[replayCount];
            DrawPieces();
            wnTextBlock.Text = "1 - " + replayArray.Length;
        }

        public void OnArrow(bool b)
        {
            if (!b && replayCount != replayArray.Length - 1)
            {
                replayCount++;
                activeGame = replayArray[replayCount];
                DrawPieces();
                wnTextBlock.Text = (replayArray.Length - replayCount) + " - " + replayArray.Length;
            }

            if (b && replayCount != 0)
            {
                replayCount--;
                activeGame = replayArray[replayCount];
                DrawPieces();
                wnTextBlock.Text = (replayArray.Length - replayCount) + " - " + replayArray.Length;
            }
        }

        public void EndReplay()
        {
            GameMenu.Visibility = Visibility.Visible;
            EndReplayMenu.Visibility = Visibility.Collapsed;
            isReplay = false;
            activeGame = new Game(memory.Peek());
            DrawPieces();
            ChangeProgressBar();
            CheckMate(activeGame);

            if (isLoad)
            {
                ResizeWindow(250, 170);
                MainBoard.Visibility = Visibility.Collapsed;
                StartMenu.Visibility = Visibility.Visible;
            }
        }

        public void ExitGame()
        {
            MessageBoxResult yesno = MessageBox.Show("Do you want to exit the game?", "Exit", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (yesno == MessageBoxResult.Yes)
            {
                ResizeWindow(250, 170);
                MainBoard.Visibility = Visibility.Collapsed;
                StartMenu.Visibility = Visibility.Visible;
            }
        }

        private void ResizeWindow(int height, int width)
        {
            Application.Current.MainWindow.Width = width;
            Application.Current.MainWindow.Height = height;
            Application.Current.MainWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            Application.Current.MainWindow.Left = (SystemParameters.WorkArea.Width - Application.Current.MainWindow.ActualWidth) / 2 + SystemParameters.WorkArea.Left;
            Application.Current.MainWindow.Top = (SystemParameters.WorkArea.Height - Application.Current.MainWindow.ActualHeight) / 2 + SystemParameters.WorkArea.Top;
        }

        #endregion

        #region Public methods

        public void ButtonClick(Coords coords)
        {
            if (isReplay) return;
            if (!activeGame.WhiteOnTurn && isBot) return;

            if (selectedCoords == null)
            {
                // If the square contains a piece of player that is on turn...
                if (activeGame.logicalBoard[coords.Row, coords.Column].Type != ChessPieceType.Empty && 
                    activeGame.logicalBoard[coords.Row, coords.Column].isWhite == activeGame.WhiteOnTurn)
                {
                    SelectPossibleMoves(coords);
                }
            }
            else
            {
                // Graphically selected squares are unselected
                foreach (Coords coordsPossibleMove in selectedCoords)
                {
                    DeselectSquare(coordsPossibleMove);
                }
                // Whether selected square was clicked
                bool selectedClicked = false; 
                // Length - 1, because the last coord depends on piece position
                for (int i = 0; i < selectedCoords.Length - 1; i++)
                {
                    // Selected button clicked?
                    if (selectedCoords[i].Equals(coords))
                    {
                        selectedClicked = true;
                        break;
                    }
                }
                if (selectedClicked)
                {
                    // Moves the piece from currect position to the clicked square
                    MovePiece(selectedCoords[selectedCoords.Length - 1], coords);
                    selectedCoords = null;
                    DrawPieces();

                    if (isBot && mStatusBar.Visibility != Visibility.Visible)
                    {
                        bot = new Bot(activeGame);
                        Move bestMove = bot.GetBestMove();
                        MovePiece(bestMove.From, bestMove.To);
                        DrawPieces();
                    }
                }
                else // Selected square wasn't clicked
                {
                    // If different piece was clicked, its square is selected
                    if (selectedCoords[selectedCoords.Length - 1].Equals(coords) == false)
                    {
                        selectedCoords = null; // Necessary to null after condition (selectedCoords is used in the condition)
                        // If the square contains piece of player that is on turn...
                        if (activeGame.logicalBoard[coords.Row, coords.Column].Type != ChessPieceType.Empty &&
                            activeGame.logicalBoard[coords.Row, coords.Column].isWhite == activeGame.WhiteOnTurn)
                            SelectPossibleMoves(coords);
                    }
                    else
                    {
                        selectedCoords = null; // Necessary to null after condition (selectedCoords is used in the previous condition)
                    }
                }
            }
        }

        public void MovePiece(Coords from, Coords to)
        {
            // Moving piece to new position
            activeGame.logicalBoard[to.Row, to.Column] = activeGame.logicalBoard[from.Row, from.Column];
            // Removing piece from old position
            activeGame.logicalBoard[from.Row, from.Column] = new DummyPiece(ChessPieceType.Empty, Convert.ToInt32(from.Row), Convert.ToInt32(from.Column));
            // Swapping color on turn
            activeGame.WhiteOnTurn = !activeGame.WhiteOnTurn;

            if (from.Equals(activeGame.whiteKing)) activeGame.whiteKing = to;
            if (from.Equals(activeGame.blackKing)) activeGame.blackKing = to;
            activeGame.logicalBoard[to.Row, to.Column].r = to.Row;
            activeGame.logicalBoard[to.Row, to.Column].c = to.Column;
            memory.Push(new Game(activeGame));
            ChangeProgressBar();
            CheckMate(activeGame);
        }
 
        public void LoadedChessUserControl()
        {
            activeGame = new Game();
            DrawPieces();
            memory.Push(new Game(activeGame));
        }

        public void ChangeProgressBar()
        {
            if (activeGame.WhiteOnTurn) wnTextBlock.Text = "White's turn";
            else wnTextBlock.Text = "Black's turn";

        }

        #endregion

        #region Private methods

        void CheckMate(Game situation)
        {
            if(situation.WhiteOnTurn)
            {
                for (int r = 0; r < 8; r++)
                {
                    for (int c = 0; c < 8; c++)
                    {
                        if (situation.logicalBoard[r, c].isWhite)
                        {
                            Coords[] moves = situation.logicalBoard[r, c].CalculatePossibleMoves(situation, new Coords(Convert.ToByte(r), Convert.ToByte(c)));
                            if (moves.Length != 0) return;
                        }
                    }
                }
            }
            else
            {
                for (int r = 0; r < 8; r++)
                {
                    for (int c = 0; c < 8; c++)
                    {
                        if (!situation.logicalBoard[r, c].isWhite)
                        {
                            Coords[] moves = situation.logicalBoard[r, c].CalculatePossibleMoves(situation, new Coords(Convert.ToByte(r), Convert.ToByte(c)));
                            if (moves.Length != 0) return;
                        }
                    }
                }
            }

            wnStatusBar.Visibility = Visibility.Hidden;
            if (situation.WhiteOnTurn)
            {
                mTextBlock.Text = "Black won";
            }

            mStatusBar.Visibility = Visibility.Visible;
            mTextBlock.Visibility = Visibility.Visible;
        }

        void DrawPieces()
        {
            for (int row = 0; row < 8; row++)
            {
                for (int column = 0; column < 8; column++)
                {
                    switch (activeGame.logicalBoard[row, column].Type)
                    {
                        // Free square
                        case ChessPieceType.Empty:
                            if (((Grid)(physicalBoard[row, column].Content)).Children[1] is Image)
                            {
                                ((Grid)(physicalBoard[row, column].Content)).Children.RemoveRange(0, 2);
                                ((Grid)(physicalBoard[row, column].Content)).Children.Add(new UIElement());
                                ((Grid)(physicalBoard[row, column].Content)).Children.Add(new UIElement());
                            }
                            break;
                        // Other pieces
                        default:
                            #region Displays piece on square
                            Image img = GeneratePieceImage(activeGame.logicalBoard[row, column]);
                            ((Grid)(physicalBoard[row, column].Content)).Children.RemoveRange(0, 2);
                            ((Grid)(physicalBoard[row, column].Content)).Children.Add(new UIElement());
                            ((Grid)(physicalBoard[row, column].Content)).Children.Add(img);
                            #endregion
                            break;
                    }
                }
            }

            if (selectedCoords != null)
            {
                for (int i = 0; i < selectedCoords.Length - 1; i++)
                {
                    SelectSquare(selectedCoords[i]);
                }
                SelectSquare(selectedCoords[selectedCoords.Length - 1], true);
            }
        }
 
        void SelectPossibleMoves(Coords coords)
        {
            Coords[] possibleMoves = activeGame.logicalBoard[coords.Row, coords.Column].CalculatePossibleMoves(activeGame, coords);
            selectedCoords = new Coords[possibleMoves.Length + 1];
            selectedCoords[selectedCoords.Length - 1] = coords;
            SelectSquare(coords, true);
            for (int i = 0; i < possibleMoves.Length; i++)
            {
                selectedCoords[i] = possibleMoves[i];
                SelectSquare(possibleMoves[i]);
            }
        }
        void SelectSquare(Coords coords, bool main = false)
        {
            UIElement uie = ((Grid)(physicalBoard[coords.Row, coords.Column].Content)).Children[1];

            ((Grid)(physicalBoard[coords.Row, coords.Column].Content)).Children.RemoveRange(0, 2);


            ((Grid)(physicalBoard[coords.Row, coords.Column].Content)).Children.Add(GenerateSelectedSquare(Brushes.LightSkyBlue));
            ((Grid)(physicalBoard[coords.Row, coords.Column].Content)).Children.Add(uie);
        }
        void DeselectSquare(Coords coords)
        {
            UIElement uie = ((Grid)(physicalBoard[coords.Row, coords.Column].Content)).Children[1];

            ((Grid)(physicalBoard[coords.Row, coords.Column].Content)).Children.RemoveRange(0, 2);
            ((Grid)(physicalBoard[coords.Row, coords.Column].Content)).Children.Add(new UIElement());
            ((Grid)(physicalBoard[coords.Row, coords.Column].Content)).Children.Add(uie);
        }

 
        Rectangle GenerateSelectedSquare(Brush color)
        {
            Rectangle selectedSquare = new Rectangle();
            selectedSquare.Fill = color;
            selectedSquare.Opacity = 0.5;
            selectedSquare.Stretch = Stretch.UniformToFill;
            selectedSquare.HorizontalAlignment = HorizontalAlignment.Stretch;
            selectedSquare.IsHitTestVisible = false;
            return selectedSquare;
        }

        Image GeneratePieceImage(ChessPiece piece)
        {
            Image image = new Image() { IsHitTestVisible = false };
            image.SetValue(RenderOptions.BitmapScalingModeProperty, BitmapScalingMode.Fant);
            image.Tag = piece;
            string path = "/Chess;component/Resources/" + (piece.isWhite ? "White" : "Black") + piece.Type.ToString() + ".png";
            image.Source = new BitmapImage(new Uri(path, UriKind.Relative));
            return image;
        }


        #endregion
    }
}
