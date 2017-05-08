using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SudokuCell = SudokuSolver.Sudoku.SudokuCell;

namespace SudokuSolver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Sudoku Game = new Sudoku(Sudoku.Difficulty.Easy);

        public MainWindow()
        {
            InitializeComponent();
            lst.ItemsSource = Game.GetGameBoard();

            Easy.DataContext = Sudoku.Difficulty.Easy;
            Med.DataContext = Sudoku.Difficulty.Normal;
            Hard.DataContext = Sudoku.Difficulty.Hard;

            ResetCheckboxes();

            Easy.Click += CheckboxChange;
            Med.Click += CheckboxChange;
            Hard.Click += CheckboxChange;
        }

        private void CheckboxChange(object sender, RoutedEventArgs e)
        {
            var s = sender as MenuItem;

            var result = MessageBox.Show("Start new game?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                s.IsChecked = true;
                Game = new Sudoku((Sudoku.Difficulty)s.DataContext);
                ResetCheckboxes();
                lst.ItemsSource = Game.GetGameBoard();
            }
        }

        private void ResetCheckboxes()
        {
            Easy.IsChecked = Game._Difficulty == Sudoku.Difficulty.Easy;
            Med.IsChecked = Game._Difficulty == Sudoku.Difficulty.Normal;
            Hard.IsChecked = Game._Difficulty == Sudoku.Difficulty.Hard;
        }

        private void Submit_Solution(object sender, RoutedEventArgs e)
        {
            if (Game.CheckSolution())
                MessageBox.Show("Winner!");
            else
                MessageBox.Show("Loser!");
        }

        private void Input(object sender, RoutedEventArgs e)
        {
            var s = sender as Button;
            var tile = (SudokuCell)s.DataContext;

            btnSubmit.Visibility = Visibility.Hidden;

            if (tile.isLocked)
                return;
            else
            {
                var txb = new TextBox();
                txb.Width = 20;
                txb.DataContext = new object[] { s, tile };
                txb.TextChanged += Txb_TextChanged;
                s.Content = txb;
            }
        }

        private void HandleInput(object sender)
        {
            TextBox txb = sender as TextBox;
            object[] data = txb.DataContext as object[];
            Button btn = data[0] as Button;
            SudokuCell tile = data[1] as SudokuCell;

            btnSubmit.Visibility = Visibility.Hidden;

            int number = 0;

            if (!int.TryParse(txb.Text, out number))
                MessageBox.Show("Input needs to be a number between 1 and 9");
            else if (number > 9 || number < 1)
                MessageBox.Show("Input needs to be a number between 1 and 9");
            else
            {
                tile.Value = number;
                btn.DataContext = tile;
                btn.Content = tile.Value;
                txb.Visibility = Visibility.Hidden;

                var gameboard = lst.ItemsSource as List<List<SudokuCell>>;

                if (gameboard.All(x => x.All(y => y.Value.HasValue)))
                    btnSubmit.Visibility = Visibility.Visible;
            }
        }

        private void Txb_TextChanged(object sender, TextChangedEventArgs e)
        {
            HandleInput(sender);
        }

        private void AppExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("It's Sudoku!");
        }
    }
}
