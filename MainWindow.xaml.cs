using Chess.UserControls;
using System;
using System.Collections.Generic;
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

namespace Chess
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ChessUserControls chessUserControl;

        public MainWindow()
        {
            InitializeComponent();
            chessUserControl = new ChessUserControls(LoadNewGameUserControl);
            LoadChessUserControl();
        }

        void LoadChessUserControl()
        {
            Content = chessUserControl;
        }

        void LoadNewGameUserControl(bool aaa)
        {
        }

        #region Controls events
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (!chessUserControl.isReplay) return;
            if (e.Key == Key.Left) chessUserControl.OnArrow(false);
            if (e.Key == Key.Right) chessUserControl.OnArrow(true);
        }
        #endregion
    }
}
