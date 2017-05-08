using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Media;

namespace SudokuSolver
{
    public class Sudoku
    {
        private SudokuCell[,] Board;
        private SudokuCell[,] Gameboard;
        public Difficulty _Difficulty { get; private set; }
        public enum Difficulty { Hard, Normal, Easy };

        public Sudoku(Difficulty difficulty)
        {
            _Difficulty = difficulty;
            Board = new SudokuCell[9,9];
            InitBoard(Board);
            GenerateSolution();
            Gameboard = new SudokuCell[9, 9];
            InitBoard(Gameboard);
            GenerateGameBoard();
        }

        private void InitBoard(SudokuCell[,] board)
        {
            //i = row
            //j = col

            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    board[i, j] = new SudokuCell();
        }

        void ClearBoard(SudokuCell[,] value)
        {
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                {
                    value[i, j].ResetCell();
                }
        }

        public void PrintBoard()
        {
            for (int i = 0; i < 9; i++)
            {
                if (i > 0)
                    Console.WriteLine();

                for (int j = 0; j < 9; j++)
                {
                    Console.Write(String.Format(" {0} ", Board[i, j].Value));
                }
            }
        }

        private void GenerateGameBoard()
        {
            Random random = new Random((int)DateTime.Now.Ticks);

            switch (_Difficulty)
            {
                case Difficulty.Easy:
                    for (int i = 0; i < 9; i++)
                        for (int j = 0; j < 9; j++)
                            if (random.Next(100) % 20 == 0)
                                Gameboard[i, j] = new SudokuCell(false);
                            else
                                Gameboard[i, j] = Board[i, j];
                    break;
                case Difficulty.Normal:
                    for (int i = 0; i < 9; i++)
                        for (int j = 0; j < 9; j++)
                            if (random.Next(100) % 10 == 0)
                                Gameboard[i, j] = new SudokuCell(false);
                            else
                                Gameboard[i, j] = Board[i, j];
                    break;
                case Difficulty.Hard:
                    for (int i = 0; i < 9; i++)
                        for (int j = 0; j < 9; j++)
                            if (random.Next(100) % 5 == 0)
                                Gameboard[i, j] = new SudokuCell(false);
                            else
                                Gameboard[i, j] = Board[i, j];
                    break;
            }
        }

        public List<List<SudokuCell>> GetGameBoard()
        {
            var board = new List<List<SudokuCell>>();

            for (int i = 0; i < 9; i++)
            {
                board.Add(new List<SudokuCell>());

                for (int j = 0; j < 9; j++)
                {
                    board[i].Add(Gameboard[j, i]);
                }
            }

            return board;
        }

        public bool CheckSolution()
        {
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    if (!CheckCol(j, Gameboard[i, j].Value.Value, Gameboard, i)
                    || !CheckRow(i, Gameboard[i, j].Value.Value, Gameboard, j)
                    || !CheckBox(i, j, Gameboard[i, j].Value.Value, Gameboard, true))
                        return false;
            return true;
        }

        private void GenerateSolution()
        {
            Random random = new Random();

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    SudokuCell cell = Board[i, j];

                    //random starter
                    List<int> picks = cell.GetRemaining();
                    int num = picks.ElementAt(random.Next(0, picks.Count));
                    bool working = true;

                    do
                    {
                        if (CheckCol(j, num, Board) && CheckRow(i, num, Board) && CheckBox(i, j, num, Board))
                        {
                            Board[i, j].SetValue(num);
                            working = false;
                        }
                        else
                        {
                            picks.Remove(num);

                            if (picks.Count == 0)
                            {
                                j = j - 2;

                                if (j <= -1)
                                {
                                    i--;
                                    j = 7;
                                }

                                //back down to previous row, reset to last col
                                cell.ResetCell();
                                break;
                            }
                            else
                                num = picks.ElementAt(random.Next(0, picks.Count));
                        }


                    } while (working);
                }
            }
        }

        private bool CheckRow(int row, int num, SudokuCell[,] board, int? skipIndex = null)
        {
            for (int i = 0; i < 9; i++)
                if(!skipIndex.HasValue || (i != skipIndex.GetValueOrDefault()))
                    if (board[row, i].Value == num)
                        return false;

            return true;
        }

        private bool CheckCol(int col, int num, SudokuCell[,] board, int? skipIndex = null)
        {
            for (int i = 0; i < 9; i++)
                if(!skipIndex.HasValue || i != skipIndex.GetValueOrDefault())
                    if (board[i, col].Value == num)
                        return false;

            return true;
        }

        private bool CheckBox(int row, int col, int num, SudokuCell[,] board, bool skipIndex = false)
        {
            int sCol = (col / 3) * 3;
            int sRow = (row / 3) * 3;

            for (int i = sRow; i < sRow + 3; i++)
                for (int j = sCol; j < sCol + 3; j++)
                    if(!skipIndex || (i != row && j != col))
                        if (board[i, j].Value == num)
                            return false;

            return true;
        }

        public class SudokuCell
        {
            public bool isVisible = true;
            public bool isLocked = true;
            public int? Value { get; set; }
            private List<int> Tried = new List<int>();

            public Brush Border
            {
                get
                {
                    return isLocked ? null : Brushes.Blue;
                }
            }

            public SudokuCell()
            {
                ResetCell();
            }

            public SudokuCell(bool visible)
               : this()
            {
                isVisible = visible;
                isLocked = false;
            }

            public void SetValue(int value)
            {
                Tried.Add(value);
                Value = value;
            }

            public void ResetCell()
            {
                Tried = new List<int>();
                Value = null;
            }

            public List<int> GetRemaining()
            {
                return new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 }.Where(x => !Tried.Contains(x)).ToList();
            }
        }
    }
}

