using Crestron.SimplSharp;
using System;

public class ConnectFour
{
    public static ushort[,] board_tracker = new ushort[6, 7];  // 6 rows and 7 columns
    private static ushort[] which_row = new ushort[7];
    public static bool is_red = new bool();
    public static bool Winner = false;
    public static int[] Winning_Positions;

    public static void column_counter(ushort column_num)
    {
        if (Winner == false)
        {
            board_tracker[which_row[column_num], column_num] = is_red ? (ushort)1 : (ushort)2; //ternary oprator, if is_red is true, set column_tracker index value to 1, otherwise, set column_tracker index value to 2

            is_red = !is_red;

            which_row[column_num]++;
        }
    }

    public static void Clear_Board()
    {
        for (int x = 0; x < 6; x++)  //rows  //checking for diagonal down
        {
            for (int y = 0; y < 7; y++) //columns
            {
                board_tracker[x, y] = 0;
                which_row[x] = 0;
            }
            which_row[6] = 0;

            is_red = false;
        }
        Winner = false;
    }

   

    public static string Determine_Winner()
    {
        bool Yellow_Wins = false;
        bool Red_Wins = false;
        string output = "";
        ushort[,] board = board_tracker; //rows, columns

        for (int column_check = 0; column_check < 7; column_check++) //checking for vertical win situation
        {
            for (int row_check = 3; row_check < 6; row_check++)
            {
                if (board[row_check, column_check] == 2 && board[row_check - 1, column_check] == 2 && board[row_check - 2, column_check] == 2 && board[row_check - 3, column_check] == 2)
                {
                    Winning_Positions = new int[] { Convert.ToUInt16(string.Format("{0}{1}", column_check, row_check)) + 10, Convert.ToUInt16(string.Format("{0}{1}", column_check, row_check - 1)) + 10, Convert.ToUInt16(string.Format("{0}{1}", column_check, row_check -2 )) + 10, Convert.ToUInt16(string.Format("{0}{1}", column_check, row_check - 3)) + 10 };
                    Yellow_Wins = true;
                }

                if (board[row_check, column_check] == 1 && board[row_check - 1, column_check] == 1 && board[row_check - 2, column_check] == 1 && board[row_check - 3, column_check] == 1)
                {
                    Winning_Positions = new int[] { Convert.ToUInt16(string.Format("{0}{1}", column_check, row_check)) + 10, Convert.ToUInt16(string.Format("{0}{1}", column_check, row_check - 1)) + 10, Convert.ToUInt16(string.Format("{0}{1}", column_check, row_check - 2)) + 10, Convert.ToUInt16(string.Format("{0}{1}", column_check, row_check - 3)) + 10 };
                    Red_Wins = true;
                }
            }
        }

        for (int row_check = 0; row_check < 6; row_check++) ////checking for horizontal win situation
        {
            for (int column_check = 3; column_check < 7; column_check++)
            {
                if (board[row_check, column_check] == 2 && board[row_check, column_check - 1] == 2 && board[row_check, column_check - 2] == 2 && board[row_check, column_check - 3] == 2)
                {
                    Winning_Positions = new int[] {Convert.ToUInt16(string.Format("{0}{1}", column_check, row_check))+10, Convert.ToUInt16(string.Format("{0}{1}", column_check-1, row_check))+10, Convert.ToUInt16(string.Format("{0}{1}", column_check-2, row_check))+10, Convert.ToUInt16(string.Format("{0}{1}", column_check-3, row_check))+10};
                    Yellow_Wins = true;
                }

                if (board[row_check, column_check] == 1 && board[row_check, column_check - 1] == 1 && board[row_check, column_check - 2] == 1 && board[row_check, column_check - 3] == 1)
                {

                    Winning_Positions = new int[] { Convert.ToUInt16(string.Format("{0}{1}", column_check, row_check)) + 10, Convert.ToUInt16(string.Format("{0}{1}", column_check - 1, row_check)) + 10, Convert.ToUInt16(string.Format("{0}{1}", column_check - 2, row_check)) + 10, Convert.ToUInt16(string.Format("{0}{1}", column_check - 3, row_check)) + 10 };

                    Red_Wins = true;
                }
            }
        }

        for (int row_check = 5; row_check > 2; row_check--)  //rows  //checking for diagonal down
        {
            for (int column_check = 0; column_check < 4; column_check++) //columns
            {
                if (board[row_check, column_check] == 2 && board[row_check - 1, column_check + 1] == 2 && board[row_check - 2, column_check + 2] == 2 && board[row_check - 3, column_check + 3] == 2)
                {
                    Winning_Positions = new int[] { Convert.ToUInt16(string.Format("{0}{1}", column_check, row_check)) + 10, Convert.ToUInt16(string.Format("{0}{1}", column_check + 1, row_check-1)) + 10, Convert.ToUInt16(string.Format("{0}{1}", column_check + 2, row_check - 2)) + 10, Convert.ToUInt16(string.Format("{0}{1}", column_check + 3, row_check - 3)) + 10 };

                    Yellow_Wins = true;
                }

                if (board[row_check, column_check] == 1 && board[row_check - 1, column_check + 1] == 1 && board[row_check - 2, column_check + 2] == 1 && board[row_check - 3, column_check + 3] == 1)
                {
                    Winning_Positions = new int[] { Convert.ToUInt16(string.Format("{0}{1}", column_check, row_check)) + 10, Convert.ToUInt16(string.Format("{0}{1}", column_check + 1, row_check - 1)) + 10, Convert.ToUInt16(string.Format("{0}{1}", column_check + 2, row_check - 2)) + 10, Convert.ToUInt16(string.Format("{0}{1}", column_check + 3, row_check - 3)) + 10 };
                    Red_Wins = true;
                }
            }
        }

        for (int row_check = 0; row_check < 3; row_check++)  //rows  //checking for diagonal up
        {
            for (int column_check = 0; column_check < 4; column_check++) //columns
            {
                if (board[row_check, column_check] == 2 && board[row_check + 1, column_check + 1] == 2 && board[row_check + 2, column_check + 2] == 2 && board[row_check + 3, column_check + 3] == 2)
                {
                    Winning_Positions = new int[] { Convert.ToUInt16(string.Format("{0}{1}", column_check, row_check)) + 10, Convert.ToUInt16(string.Format("{0}{1}", column_check + 1, row_check + 1)) + 10, Convert.ToUInt16(string.Format("{0}{1}", column_check + 2, row_check + 2)) + 10, Convert.ToUInt16(string.Format("{0}{1}", column_check + 3, row_check + 3)) + 10 };
                    Yellow_Wins = true;
                }

                if (board[row_check, column_check] == 1 && board[row_check + 1, column_check + 1] == 1 && board[row_check + 2, column_check + 2] == 1 && board[row_check + 3, column_check + 3] == 1)
                {
                    Winning_Positions = new int[] { Convert.ToUInt16(string.Format("{0}{1}", column_check, row_check)) + 10, Convert.ToUInt16(string.Format("{0}{1}", column_check + 1, row_check + 1)) + 10, Convert.ToUInt16(string.Format("{0}{1}", column_check + 2, row_check + 2)) + 10, Convert.ToUInt16(string.Format("{0}{1}", column_check + 3, row_check + 3)) + 10 };

                    Red_Wins = true;
                }
            }
        }

        if (Yellow_Wins == false && Red_Wins == false)
        {
            if (is_red)
            {
                output = "Red to Play";
            }
            else
            {
                output = "Yellow to Play";
            }
        }

        if (Yellow_Wins)
        {
            output = "Yellow Wins!";
            Winner = true;

        }
        else if (Red_Wins)
        {
            output = "Red Wins!";
            Winner = true;

        }

        return output;
    }
}