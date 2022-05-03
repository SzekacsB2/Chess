using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    class King : ChessPiece
    {
        public King(bool white, int r, int c) : base(ChessPieceType.King, white, r, c)
        {
        }

        public override bool[,] GenerateHits(Game situation)
        {
            bool[,] hits = new bool[8, 8];
            int r2 = r;
            int c2 = c;

            c2++;
            if (CheckWall(r2, c2) && CheckPieces(situation, isWhite, r2, c2) != 2) hits[r2, c2] = true;
            c2 -= 2;
            if (CheckWall(r2, c2) && CheckPieces(situation, isWhite, r2, c2) != 2) hits[r2, c2] = true;
            r2++;
            if (CheckWall(r2, c2) && CheckPieces(situation, isWhite, r2, c2) != 2) hits[r2, c2] = true;
            c2++;
            if (CheckWall(r2, c2) && CheckPieces(situation, isWhite, r2, c2) != 2) hits[r2, c2] = true;
            c2++;
            if (CheckWall(r2, c2) && CheckPieces(situation, isWhite, r2, c2) != 2) hits[r2, c2] = true;
            r2 -= 2;
            if (CheckWall(r2, c2) && CheckPieces(situation, isWhite, r2, c2) != 2) hits[r2, c2] = true;
            c2--;
            if (CheckWall(r2, c2) && CheckPieces(situation, isWhite, r2, c2) != 2) hits[r2, c2] = true;
            c2--;
            if (CheckWall(r2, c2) && CheckPieces(situation, isWhite, r2, c2) != 2) hits[r2, c2] = true;

            return hits;
        }
    }
}
