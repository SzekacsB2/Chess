using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    class Pawn : ChessPiece
    {
        public Pawn(bool white, int r, int c) : base(ChessPieceType.Pawn, white, r, c) 
        {
        }

        public override bool[,] GenerateHits(Game situation)
        {
            bool[,] hits = new bool[8, 8];

            int r2;
            int c2 = c + 1;

            if (isWhite) r2 = r - 1;
            else r2 = r + 1;

            if (CheckWall(r2, c2) && CheckPieces(situation, isWhite, r2, c2) != 2) hits[r2, c2] = true;
            c2 -= 2;
            if (CheckWall(r2, c2) && CheckPieces(situation, isWhite, r2, c2) != 2) hits[r2, c2] = true;

            return hits;
        }

        #region Move
        public bool ValidMove(Game situation, int r2, int c2, bool diagonal = false, bool firstmove = false)
        {
            if (!CheckWall(r2, c2)) return false;
            int cp = CheckPieces(situation, isWhite, r2, c2);
            if (cp == 2) return false;
            if (!diagonal && cp == 1) return false;
            if (diagonal && cp != 1) return false;
            if (firstmove && (r != 1 && r != 6)) return false;

            Game newSituation = new Game(situation);
            newSituation.logicalBoard[r2, c2] = newSituation.logicalBoard[this.r, this.c];
            newSituation.logicalBoard[this.r, this.c] = new DummyPiece(ChessPieceType.Empty, this.r, this.c);
            if (this.Type == ChessPieceType.King)
            {
                if (this.isWhite) newSituation.whiteKing = new Coords(Convert.ToByte(r2), Convert.ToByte(c2));
                else newSituation.blackKing = new Coords(Convert.ToByte(r2), Convert.ToByte(c2));
            }
            newSituation.logicalBoard[r2, c2].r = r2;
            newSituation.logicalBoard[r2, c2].c = c2;

            if (CheckCheck(newSituation)) return false;

            return true;
        }

        public override Coords[] CalculatePossibleMoves(Game situation, Coords coords)
        {
            List<Coords> moves = new List<Coords>();
            int r2 = r;
            int c2 = c;

            if (isWhite)
            {
                r2--;
                if (ValidMove(situation, r2, c2)) moves.Add(new Coords(Convert.ToByte(r2), Convert.ToByte(c2)));
                c2--;
                if (ValidMove(situation, r2, c2, true)) moves.Add(new Coords(Convert.ToByte(r2), Convert.ToByte(c2)));
                c2 += 2;
                if (ValidMove(situation, r2, c2, true)) moves.Add(new Coords(Convert.ToByte(r2), Convert.ToByte(c2)));
                c2--;
                r2--;
                if (ValidMove(situation, r2, c2, false ,true)) moves.Add(new Coords(Convert.ToByte(r2), Convert.ToByte(c2)));
            }
            else
            {
                r2++;
                if (ValidMove(situation, r2, c2)) moves.Add(new Coords(Convert.ToByte(r2), Convert.ToByte(c2)));
                c2--;
                if (ValidMove(situation, r2, c2, true)) moves.Add(new Coords(Convert.ToByte(r2), Convert.ToByte(c2)));
                c2 += 2;
                if (ValidMove(situation, r2, c2, true)) moves.Add(new Coords(Convert.ToByte(r2), Convert.ToByte(c2)));
                c2--;
                r2++;
                if (ValidMove(situation, r2, c2, false, true)) moves.Add(new Coords(Convert.ToByte(r2), Convert.ToByte(c2)));
            }

            return moves.ToArray();
        }
        #endregion
    }
}
