using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    class Bishop : ChessPiece
    {
        public Bishop(bool white, int r, int c) : base(ChessPieceType.Bishop, white, r, c)
        {
        }

        public override bool[,] GenerateHits(Game simulation)
        {
            bool[,] hits = new bool[8, 8];

            //r+,c+
            bool whilelogic = true;
            int r2 = r + 1;
            int c2 = c + 1;
            while (whilelogic)
            {
                if (MoveLogic(out whilelogic, simulation, isWhite, r2, c2))
                {
                    hits[r2, c2] = true;
                }
                r2++;
                c2++;
            }

            //r-,c-
            whilelogic = true;
            r2 = r - 1;
            c2 = c - 1;
            while (whilelogic)
            {
                if (MoveLogic(out whilelogic, simulation, isWhite, r2, c2))
                {
                    hits[r2, c2] = true;
                }
                r2--;
                c2--;
            }

            //r+,c-
            whilelogic = true;
            r2 = r + 1;
            c2 = c - 1;
            while (whilelogic)
            {
                if (MoveLogic(out whilelogic, simulation, isWhite, r2, c2))
                {
                    hits[r2, c2] = true;
                }
                r2++;
                c2--;
            }

            //r-,c+
            whilelogic = true;
            r2 = r - 1;
            c2 = c + 1;
            while (whilelogic)
            {
                if (MoveLogic(out whilelogic, simulation, isWhite, r2, c2))
                {
                    hits[r2, c2] = true;
                }
                r2--;
                c2++;
            }

            return hits;
        }
    }
}
