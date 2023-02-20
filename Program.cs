using System;
using System.Threading;
using System.Collections.Generic;


namespace tetris
{
    class Program
    {   //Sets
        static int TRows = 20;//tetris rows
        static int TCols = 10;//tetris cols
        static int ICols = 20;//info cols
        static int CRows = 1 + TRows + 2;//console rows
        static int CCols = 1 + TCols + 1 + ICols + 1;//console cols
        static List<bool[,]> Figures = new List<bool[,]>
        {
            new bool[,] {
                {true,true,true,true} 
            }, //I
            new bool[,] {
                {true,true},
                {true,true }
            }, //O
            new bool[,] {
                {false,true,false },
                {true,true,true }
            }, //T
            new bool[,] {
                {false,true,true },
                {true,true,false }
            }, //S
            new bool[,] {
                {true,true,false },
                {false,true,true }
            }, //Z
            new bool[,] {
                {false,false,false,true },
                {true,true,true,true}
            }, //J
            new bool[,] {
                {true,false,false,false },
                {true,true,true,true }
            }  //L
        };
        
        //state
        static int score = 0;
        static int frame = 0;
        static int FrameToMoveFigure = 10;
        static int FullRowIndex=0;
        static bool[,] CurrentFigure = null;
        static int CurrentFigureRows = 0;//position, starting row
        static int CurrentFigureCols = 0;//position, starting col
        static bool[,] Field = new bool[TRows, TCols];
        static Random random = new Random();

        static void Main(string[] args)
        {   
            Console.WindowHeight = CRows;
            Console.WindowWidth = CCols;
            Console.BufferHeight = CRows;
            Console.BufferWidth = CCols;
            Console.CursorVisible = false;
            Console.ForegroundColor = ConsoleColor.DarkCyan;

            CurrentFigure = Figures[random.Next(0,Figures.Count)];
            
            while (true)
            {   
                frame++;
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey();
                    if (key.Key == ConsoleKey.Escape)
                    {
                        return;
                    }
                    if (key.Key == ConsoleKey.LeftArrow && CurrentFigureCols>=1)
                    {
                        sbyte a = 0;
                        for(int i=CurrentFigureRows;i<CurrentFigureRows + CurrentFigure.GetLength(0);i++)
                        {
                            int j = CurrentFigureCols;
                            if (Field[i, j - 1] == false)
                            {
                                a++;
                            }
                        }
                        if (a == CurrentFigure.GetLength(0))
                        {
                            CurrentFigureCols--;
                        }
                        
                    }
                    if (key.Key == ConsoleKey.RightArrow && CurrentFigureCols < TCols - CurrentFigure.GetLength(1))
                    {
                        sbyte a = 0;
                        for (int i = CurrentFigureRows; i < CurrentFigureRows + CurrentFigure.GetLength(0); i++)
                        {
                            int j = CurrentFigureCols+CurrentFigure.GetLength(1)-1;
                            if (Field[i, j + 1] == false)
                            {
                                a++;
                            }
                        }
                        if (a == CurrentFigure.GetLength(0))
                        {
                            CurrentFigureCols++;
                        }
                    }
                    if(key.Key == ConsoleKey.DownArrow)
                    {
                        CurrentFigureRows++;
                        score++;

                    }
                    if(key.Key == ConsoleKey.UpArrow)
                    {
                        RotateFigure();
                    }
                    
                }
                if (frame % FrameToMoveFigure == 0)
                {
                    frame = 1;
                    score++;
                    CurrentFigureRows++;
                }

                
                if (Collision()==true)
                {   AddCurrentFigureToTheField();
                    CurrentFigure = Figures[random.Next(0, Figures.Count)];
                    CurrentFigureRows = 0;
                    CurrentFigureCols = 0;
                    FindingFullRow();
                    if (Collision())
                    {
                        string scorestr = score.ToString();
                        scorestr += new string(' ', 12 - scorestr.Length);
                        Write("┌──────────────┐",5,5);
                        Write("│ Game         │",6,5);
                        Write("│       over!  │",7,5);
                        Write($"│  {scorestr}│",8,5);
                        Write("└──────────────┘",9,5);
                        Console.ReadKey();
                        return;
                        
                    }
                }
                
                
                DrawBorder();
                DrawInfo();
                DrawCurrentFigure();
                DrawField();

                Thread.Sleep(40);
            }
            
            Console.ReadKey();

            Console.SetCursorPosition(1,1);
        }
        static int FindingFullRow()
        {
            int lines = 0;
           
            for(int i = 0; i < TRows; i++)
            {   
                bool FullRowExists = true;
                for(int j = 0; j < TCols; j++)
                {
                    if (Field[i, j] == false)
                    {
                        FullRowExists = false;
                        break;
                    }
                    
                }
                if (FullRowExists)
                {
                    for(int row = i; row >= 1; row--)
                    {
                        for(int j = 0; j < TCols; j++)
                        {
                            Field[row, j] = Field[row - 1, j];
                        }
                    }
                    lines++;
                }
                
            }
            return lines;
        }
        
        static void RotateFigure()
        {
            bool[,] NewFigure = new bool[CurrentFigure.GetLength(1), CurrentFigure.GetLength(0)];
            for(int i = 0; i < CurrentFigure.GetLength(0); i++)
            {
                for(int j = 0; j < CurrentFigure.GetLength(1); j++)
                {
                    NewFigure[j,CurrentFigure.GetLength(0)- i-1] = CurrentFigure[i, j];
                }
            }
            CurrentFigure = NewFigure;
        }
        static bool Collision()
        {
            if (CurrentFigureRows + CurrentFigure.GetLength(0) == TRows)
            {
                return true;
            }
            else if(CurrentFigureRows + CurrentFigure.GetLength(0) < TRows)
            {
                for (int row = 0; row < CurrentFigure.GetLength(0); row++)
                {
                    for (int col = 0; col < CurrentFigure.GetLength(1); col++)
                    {
                        if (CurrentFigure[row, col] == true && Field[CurrentFigureRows+row+1, CurrentFigureCols + col] == true)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        static void AddCurrentFigureToTheField()
        {
            for (int i = 0; i < CurrentFigure.GetLength(0); i++)
            {
                for (int j = 0; j < CurrentFigure.GetLength(1); j++)
                {
                    if (CurrentFigure[i, j] == true)
                    {
                        Field[CurrentFigureRows + i, CurrentFigureCols + j] = true;
                    }
                }
            }
        }
        static void DrawField()
        {
            for(int i = 0; i < TRows; i++)
            {   string line="";
                for(int j = 0; j < TCols; j++)
                {
                    if (Field[i, j]==true)
                    {
                        Write("*", i + 1, j + 1);
                        //line += '*';
                    }
                    //else
                    //{
                    //    line += ' ';
                    //}
                }
                //Write(line, i +1, 1);
            }
        }
        static void Write(string text, int row, int col)
        {
            Console.SetCursorPosition(col, row);
            Console.Write(text);
        }
        static void DrawBorder()
        {
            Console.SetCursorPosition(0, 0);
                string line1 = "┌";
                line1 += new string('─', TCols);
                line1 += "┬";
                line1 += new string('─', ICols);
                line1 += "┐";
                Console.Write(line1);
            
            for(int i=1; i<= TRows; i++)
            {
                string middle = "│";
                middle += new string(' ', TCols);
                middle += "│";
                middle += new string(' ', ICols);
                middle += "│";
                Console.Write(middle);
            }
            string line2 = "└";
            line2 += new string('─', TCols);
            line2 += "┴";
            line2 += new string('─', ICols);
            line2 += "┘";
            Console.Write(line2);

        }
        static void DrawInfo()
        {
            Write("Score :", 1, TCols +3);
            Write(score.ToString(), 2, TCols + 3);
            Write("Frame :", 3, TCols + 3);
            Write(frame.ToString(), 4, TCols + 3);
        }
        static void DrawCurrentFigure()
        {
            for(int i = 0; i < CurrentFigure.GetLength(0); i++)
            {
                for (int j = 0; j < CurrentFigure.GetLength(1); j++)
                {
                    if (CurrentFigure[i, j]==true)
                    {
                        Write("*", i + 1 + CurrentFigureRows, j + 1 + CurrentFigureCols); 
                    } 
                }
            }
        }
    }
}
