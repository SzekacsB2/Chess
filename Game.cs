using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Chess
{
    public class Game
    {
        /// <summary>
        /// Logical chessboard.
        /// </summary>
        public ChessPiece[,] logicalBoard { get; set; }
        /// <summary>
        /// Specifies whether white player is on turn.
        /// </summary>
        public bool WhiteOnTurn { get; set; }

        /// <summary>
        /// Default constructor sets up pieces into default position and sets the turn to the white player.
        /// </summary>
        
        public Coords whiteKing { get; set; }
        public Coords blackKing { get; set; }

        public Game()
        {
            logicalBoard = new ChessPiece[8, 8];
            WhiteOnTurn = true;
            whiteKing = new Coords(7, 4);
            blackKing = new Coords(0, 4);
            
            for (int row = 0; row < 8; row++)
            {
                for (int column = 0; column < 8; column++)
                {
                    if (row == 1)
                    {
                        logicalBoard[row, column] = new Pawn(false, row, column);
                    }
                    else if (row == 6)
                    {
                        logicalBoard[row, column] = new Pawn(true, row, column);
                    }
                    else if (row == 0 && (column == 0 || column == 7))
                    {
                        logicalBoard[row, column] = new Tower(false, row, column);
                    }
                    else if (row == 7 && (column == 0 || column == 7))
                    {
                        logicalBoard[row, column] = new Tower(true, row, column);
                    }
                    else if (row == 0 && (column == 6 || column == 1))
                    {
                        logicalBoard[row, column] = new Knight(false, row, column);
                    }
                    else if (row == 7 && (column == 6 || column == 1))
                    {
                        logicalBoard[row, column] = new Knight(true, row, column);
                    }
                    else if (row == 0 && (column == 5 || column == 2))
                    {
                        logicalBoard[row, column] = new Bishop(false, row, column);
                    }
                    else if (row == 7 && (column == 5 || column == 2))
                    {
                        logicalBoard[row, column] = new Bishop(true, row, column);
                    }
                    else if (row == 0 && column == 3)
                    {
                        logicalBoard[row, column] = new Queen(false, row, column);
                    }
                    else if (row == 7 && column == 3)
                    {
                        logicalBoard[row, column] = new Queen(true, row, column);
                    }
                    else if (row == 0 && column == 4)
                    {
                        logicalBoard[row, column] = new King(false, row, column);
                    }
                    else if (row == 7 && column == 4)
                    {
                        logicalBoard[row, column] = new King(true, row, column);
                    }
                    else
                    {
                        logicalBoard[row, column] = new DummyPiece(ChessPieceType.Empty, row, column);
                    }
                }
            }
        }

        public Game(Game situation)
        { 
            logicalBoard = new ChessPiece[8, 8];
            WhiteOnTurn = situation.WhiteOnTurn;
            whiteKing = situation.whiteKing;
            blackKing = situation.blackKing;
        
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    ChessPieceType p = situation.logicalBoard[r, c].Type;
                    bool isWhite = situation.logicalBoard[r, c].isWhite;
                    switch (p)
                    {
                        case ChessPieceType.Pawn:
                            logicalBoard[r, c] = new Pawn(isWhite, r, c);
                            break;
                        case ChessPieceType.Bishop:
                            logicalBoard[r, c] = new Bishop(isWhite, r, c);
                            break;
                        case ChessPieceType.Tower:
                            logicalBoard[r, c] = new Tower(isWhite, r, c);
                            break;
                        case ChessPieceType.Knight:
                            logicalBoard[r, c] = new Knight(isWhite, r, c);
                            break;
                        case ChessPieceType.Queen:
                            logicalBoard[r, c] = new Queen(isWhite, r, c);
                            break;
                        case ChessPieceType.King:
                            logicalBoard[r, c] = new King(isWhite, r, c);
                            break;
                        default:
                            logicalBoard[r, c] = new DummyPiece(ChessPieceType.Empty, r, c);
                            break;
                    }
                }
            }
        }

        public Game(bool WhiteOnTurn, string line)
        {
            logicalBoard = new ChessPiece[8, 8];
            this.WhiteOnTurn = WhiteOnTurn;
            int count = 0;

            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    bool isWhite = true;
                    if (line[count] == 'w') isWhite = true;
                    else if (line[count] == 'b') isWhite = false;
                    count++;

                    switch (line[count])
                    {
                        case 'p':
                            logicalBoard[r, c] = new Pawn(isWhite, r, c);
                            break;
                        case 'b':
                            logicalBoard[r, c] = new Bishop(isWhite, r, c);
                            break;
                        case 't':
                            logicalBoard[r, c] = new Tower(isWhite, r, c);
                            break;
                        case 'h':
                            logicalBoard[r, c] = new Knight(isWhite, r, c);
                            break;
                        case 'q':
                            logicalBoard[r, c] = new Queen(isWhite, r, c);
                            break;
                        case 'k':
                            logicalBoard[r, c] = new King(isWhite, r, c);
                            if (isWhite) whiteKing = new Coords(Convert.ToByte(r), Convert.ToByte(c));
                            else blackKing = new Coords(Convert.ToByte(r), Convert.ToByte(c));
                            break;
                        default:
                            logicalBoard[r, c] = new DummyPiece(ChessPieceType.Empty, r, c);
                            break;
                    }
                    count++;
                }
            }
        }
    }
}
