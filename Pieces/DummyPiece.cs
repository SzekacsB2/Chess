using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    class DummyPiece : ChessPiece
    {
        public DummyPiece(ChessPieceType type, int r, int c, bool white = true) : base(type, white, r, c)
        {
        }

        public override bool[,] GenerateHits(Game situation)
        {
            bool[,] hits = new bool[8, 8];
            return hits;
        }

        public override Coords[] CalculatePossibleMoves(Game situation, Coords coords)
        {
            List<Coords> possibleMoves = new List<Coords>();
            return possibleMoves.ToArray();
        }
    }
}
