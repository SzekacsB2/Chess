using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    class Knight : ChessPiece
    {
        public Knight(bool white, int r, int c) : base(ChessPieceType.Knight, white, r, c)
        {
        }

        public override bool[,] GenerateHits(Game situation)
        {
            bool[,] hits = new bool[8, 8];
            int r2, c2;

            r2 = r + 2;
            c2 = c + 1;
            if (CheckWall(r2, c2) && CheckPieces(situation, isWhite, r2, c2) != 2) hits[r2, c2] = true;
            c2 -= 2;
            if (CheckWall(r2, c2) && CheckPieces(situation, isWhite, r2, c2) != 2) hits[r2, c2] = true;
            r2 -= 4;
            if (CheckWall(r2, c2) && CheckPieces(situation, isWhite, r2, c2) != 2) hits[r2, c2] = true;
            c2 += 2;
            if (CheckWall(r2, c2) && CheckPieces(situation, isWhite, r2, c2) != 2) hits[r2, c2] = true;

            r2 = r + 1;
            c2 = c + 2;
            if (CheckWall(r2, c2) && CheckPieces(situation, isWhite, r2, c2) != 2) hits[r2, c2] = true;
            r2 -= 2;
            if (CheckWall(r2, c2) && CheckPieces(situation, isWhite, r2, c2) != 2) hits[r2, c2] = true;
            c2 -= 4;
            if (CheckWall(r2, c2) && CheckPieces(situation, isWhite, r2, c2) != 2) hits[r2, c2] = true;
            r2 += 2;
            if (CheckWall(r2, c2) && CheckPieces(situation, isWhite, r2, c2) != 2) hits[r2, c2] = true;

            return hits;
        }
    }
}
