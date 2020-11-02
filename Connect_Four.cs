using System;
using System.Collections.Generic;

public class ConnectFour
{
    public static string WhoIsWinner(List<string> piecesPositionList)
    {
        bool Yellow_Wins = false;
        bool Red_Wins = false;
        string output = "";
        int[] column_status = new int[7];
        int[,] board = new int[6, 7]; //rows, columns

        for (int Move = 0; Move < piecesPositionList.Count; Move++)
        {
            int column = Convert.ToInt32(piecesPositionList[Move][0]) - 65; //the column in which the piece is dropped
            string color = piecesPositionList[Move].Substring(2);
            if (color == "Red")
            {
                board[column_status[column], column] = 1;
            }
            else if (color == "Yellow")
            {
                board[column_status[column], column] = 2;
            }
            column_status[column]++; //the row in which the piece is dropped

            for (int column_check = 0; column_check < 7; column_check++) //checking for vertical win situation
            {
                for (int row_check = 3; row_check < 6; row_check++)
                {
                    if (board[row_check, column_check] == 2 && board[row_check - 1, column_check] == 2 && board[row_check - 2, column_check] == 2 && board[row_check - 3, column_check] == 2)
                    {
                        Yellow_Wins = true;
                    }

                    if (board[row_check, column_check] == 1 && board[row_check - 1, column_check] == 1 && board[row_check - 2, column_check] == 1 && board[row_check - 3, column_check] == 1)
                    {
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
                        Yellow_Wins = true;
                    }

                    if (board[row_check, column_check] == 1 && board[row_check, column_check - 1] == 1 && board[row_check, column_check - 2] == 1 && board[row_check, column_check - 3] == 1)
                    {
                        Red_Wins = true;
                    }
                }
            }

            for (int x = 5; x > 2; x--)  //rows  //checking for diagonal down
            {
                for (int y = 0; y < 4; y++) //columns
                {
                    if (board[x, y] == 2 && board[x - 1, y + 1] == 2 && board[x - 2, y + 2] == 2 && board[x - 3, y + 3] == 2)
                    {
                        Yellow_Wins = true;
                    }

                    if (board[x, y] == 1 && board[x - 1, y + 1] == 1 && board[x - 2, y + 2] == 1 && board[x - 3, y + 3] == 1)
                    {
                        //Console.WriteLine("{0},{1}", x, y);
                        Red_Wins = true;
                    }
                }
            }

            for (int x = 0; x < 3; x++)  //rows  //checking for diagonal up
            {
                for (int y = 0; y < 4; y++) //columns
                {
                    if (board[x, y] == 2 && board[x + 1, y + 1] == 2 && board[x + 2, y + 2] == 2 && board[x + 3, y + 3] == 2)
                    {
                        Yellow_Wins = true;
                    }

                    if (board[x, y] == 1 && board[x + 1, y + 1] == 1 && board[x + 2, y + 2] == 1 && board[x + 3, y + 3] == 1)
                    {
                        Red_Wins = true;
                        Console.WriteLine("Diagonal Up");
                    }
                }
            }

            if (Yellow_Wins == false && Red_Wins == false)
            {
                output = "Draw";
            }
            else if (Yellow_Wins)
            {
                output = "Yellow";
                break;
            }
            else if (Red_Wins)
            {
                output = "Red";
                break;
            }
        }

        return output;
    }
}