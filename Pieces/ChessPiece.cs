using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    public enum ChessPieceType
    {
        Empty,
        Knight,
        King,
        Queen,
        Bishop,
        Pawn,
        Tower
    }


    public abstract class ChessPiece
    {
        public bool isWhite { get; }
        public int r { get;  set; }
        public int c { get; set; }
        public ChessPieceType Type { get; }

        protected ChessPiece(ChessPieceType type, bool white, int row, int column)
        {
            Type = type;
            isWhite = white;
            r = row;
            c = column;
        }

        public override string ToString()
        {
            switch (Type)
            {
                case ChessPieceType.Empty:
                    return "ee";
                case ChessPieceType.Knight:
                    if (isWhite) return "wh";
                    else return "bh";
                case ChessPieceType.King:
                    if (isWhite) return "wk";
                    else return "bk";
                case ChessPieceType.Queen:
                    if (isWhite) return "wq";
                    else return "bq";
                case ChessPieceType.Bishop:
                    if (isWhite) return "wb";
                    else return "bb";
                case ChessPieceType.Pawn:
                    if (isWhite) return "wp";
                    else return "bp";
                case ChessPieceType.Tower:
                    if (isWhite) return "wt";
                    else return "bt";
                default:
                    return "";
            }
        }

        #region Check
        public static bool CheckWall(int r, int c)
        {
            if (r > 7 || r < 0 || c > 7 || c < 0)
            {
                return false;
            }
            return true;
        }

        public static int CheckPieces(Game situation, bool isWhite, int r, int c)
        {
            if (situation.logicalBoard[r, c].Type == ChessPieceType.Empty) return 0;

            if (situation.logicalBoard[r, c].isWhite == isWhite) return 2;
            else return 1; 
        }

        public static bool CheckCheck(Game situation)
        {
            if (situation.WhiteOnTurn)
            {
                for (int r = 0; r < 8; r++)
                {
                    for (int c = 0; c < 8; c++)
                    {
                        if (situation.logicalBoard[r, c].Type != ChessPieceType.Empty && !situation.logicalBoard[r, c].isWhite)
                        {
                            bool[,] hits = situation.logicalBoard[r, c].GenerateHits(situation);
                            if (hits[situation.whiteKing.Row, situation.whiteKing.Column]) return true;
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
                        if (situation.logicalBoard[r, c].Type != ChessPieceType.Empty && situation.logicalBoard[r, c].isWhite)
                        {
                            bool[,] hits = situation.logicalBoard[r, c].GenerateHits(situation);
                            if (hits[situation.blackKing.Row, situation.blackKing.Column]) return true;
                        }
                    }
                }
            }
           
            return false;
        }
        #endregion

        #region Move
        public abstract bool[,] GenerateHits(Game situation);

        public bool MoveLogic(out bool whilelogic, Game situation, bool isWhite, int r, int c)
        {
            whilelogic = true;

            if (CheckWall(r, c))
            {
                int checkpiece = CheckPieces(situation, isWhite, r, c);

                if (checkpiece != 2)
                {
                    if (checkpiece == 0)
                    {
                        return true;
                    }

                    if (checkpiece == 1)
                    {
                        whilelogic = false;
                        return true;
                    }
                }
                else whilelogic = false;
            }
            else whilelogic = false;

            return false;
        }

        public virtual Coords[] CalculatePossibleMoves(Game situation, Coords coords)
        {
            bool[,] hits = GenerateHits(situation);
            List<Coords> moves = new List<Coords>();

            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    if (hits[r, c])
                    {
                        Game newSituation = new Game(situation);
                        newSituation.logicalBoard[r, c] = newSituation.logicalBoard[this.r, this.c];
                        newSituation.logicalBoard[this.r, this.c] = new DummyPiece(ChessPieceType.Empty, this.r, this.c);
                        if (this.Type == ChessPieceType.King)
                        {
                            if (this.isWhite) newSituation.whiteKing = new Coords(Convert.ToByte(r), Convert.ToByte(c));
                            else newSituation.blackKing = new Coords(Convert.ToByte(r), Convert.ToByte(c));
                        }
                        newSituation.logicalBoard[r, c].r = r;
                        newSituation.logicalBoard[r, c].c = c;

                        if (!CheckCheck(newSituation))
                        {
                            moves.Add(new Coords(Convert.ToByte(r), Convert.ToByte(c)));
                        }
                    }
                }
            }

            return moves.ToArray();
        }
        #endregion
    }
}
