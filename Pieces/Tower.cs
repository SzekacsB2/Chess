using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    class Tower : ChessPiece
    {
        public Tower(bool white, int r, int c) : base(ChessPieceType.Tower, white, r, c)
        {
        }

        public override bool[,] GenerateHits(Game situation)
        {
            bool[,] hits = new bool[8, 8];

            //r+
            bool whilelogic = true;
            int r2 = r + 1;
            while (whilelogic)
            {
                if (MoveLogic(out whilelogic, situation, isWhite, r2, c))
                {
                    hits[r2, c] = true;
                }
                r2++;
            }

            //r-
            whilelogic = true;
            r2 = r - 1;
            while (whilelogic)
            {
                if (MoveLogic(out whilelogic, situation, isWhite, r2, c))
                {
                    hits[r2, c] = true;
                }
                r2--;
            }

            //c+
            whilelogic = true;
            int c2 = c + 1;
            while (whilelogic)
            {
                if (MoveLogic(out whilelogic, situation, isWhite, r, c2))
                {
                    hits[r, c2] = true;
                }
                c2++;
            }

            //c-
            whilelogic = true;
            c2 = c - 1;
            while (whilelogic)
            {
                if (MoveLogic(out whilelogic, situation, isWhite, r, c2))
                {
                    hits[r, c2] = true;
                }
                c2--;
            }

            return hits;
        }
    }
}
