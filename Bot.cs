using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Chess
{
    public struct Move
    {
        public Coords From { get; }
        public Coords To { get; }



        public Move(Coords from, Coords to)
        {
            From = from;
            To = to;
        }
    }
    class Bot
    {
        Game activeGame;
        Move bestMove;
        List<Move> allMoves;
        int depht;

        public Bot(Game activeGame)
        {
            allMoves = new List<Move>();
            this.activeGame = new Game(activeGame);

            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    if (!activeGame.logicalBoard[r, c].isWhite)
                    {
                        Coords from = new Coords(Convert.ToByte(r), Convert.ToByte(c));
                        Coords[] moves = activeGame.logicalBoard[r, c].CalculatePossibleMoves(activeGame, from);
                        foreach (Coords to in moves)
                        {
                            Move move = new Move(from, to);
                            allMoves.Add(move);
                        }
                    }
                }
            }

            bestMove = allMoves[0];
            depht = 3;
        }

        public Move GetBestMove()
        {
            int bestValue = -1000;
            int alpha = -1000;
            int beta = 1000;
            List<Move> bestMoveList = new List<Move>(); 

            foreach (Move move in allMoves)
            {
                Game newSituation = new Game(activeGame);
                newSituation.logicalBoard[move.To.Row, move.To.Column] = newSituation.logicalBoard[move.From.Row, move.From.Column];
                newSituation.logicalBoard[move.From.Row, move.From.Column] = new DummyPiece(ChessPieceType.Empty, move.From.Row, move.From.Column);
                if (newSituation.logicalBoard[move.To.Row, move.To.Column].Type == ChessPieceType.King) newSituation.blackKing = move.To;
                newSituation.logicalBoard[move.To.Row, move.To.Column].r = move.To.Row;
                newSituation.logicalBoard[move.To.Row, move.To.Column].c = move.To.Column;
                newSituation.WhiteOnTurn = true;

                int value = minimax(0, true, newSituation, alpha, beta);
                if (value > bestValue)
                {
                    bestMoveList.Clear();
                    bestMoveList.Add(new Move(move.From, move.To));
                    bestValue = value;
                    bestMove = move;
                }
                if (value == bestValue) bestMoveList.Add(new Move(move.From, move.To));
                alpha = Math.Max(alpha, bestValue);
                if (beta <= alpha) break;
            }

            return bestMove;
            if (bestMoveList.Count != 1) changeBest(bestMoveList.ToArray());
            return bestMove;
        }

        private void changeBest(Move[] list)
        {
            int bestValue = -1000;

            foreach (Move move in list)
            {
                Game newSituation = new Game(activeGame);
                newSituation.logicalBoard[move.To.Row, move.To.Column] = newSituation.logicalBoard[move.From.Row, move.From.Column];
                newSituation.logicalBoard[move.From.Row, move.From.Column] = new DummyPiece(ChessPieceType.Empty, move.From.Row, move.From.Column);
                if (newSituation.logicalBoard[move.To.Row, move.To.Column].Type == ChessPieceType.King) newSituation.blackKing = move.To;
                newSituation.logicalBoard[move.To.Row, move.To.Column].r = move.To.Row;
                newSituation.logicalBoard[move.To.Row, move.To.Column].c = move.To.Column;
                newSituation.WhiteOnTurn = true;

                int value = evaluateMove(newSituation, move);
                if(value > bestValue)
                {
                    bestValue = value;
                    bestMove = move;
                }
            }
        }

        private int minimax(int depht, bool isMax, Game situation, int alpha, int beta)
        {
            if (depht == this.depht) return evaulateBoard(situation);

            if(isMax)
            {
                int best = -1000;
                List<Move> moveList = new List<Move>();
                for (int r = 0; r < 8; r++)
                {
                    for (int c = 0; c < 8; c++)
                    {
                        if (!activeGame.logicalBoard[r, c].isWhite)
                        {
                            Coords from = new Coords(Convert.ToByte(r), Convert.ToByte(c));
                            Coords[] moves = activeGame.logicalBoard[r, c].CalculatePossibleMoves(activeGame, from);
                            foreach (Coords to in moves)
                            {
                                Move move = new Move(from, to);
                                moveList.Add(move);
                            }
                        }
                    }
                }

                if (moveList.Count == 0) return evaulateBoard(situation);

                foreach (Move move in moveList)
                {
                    Game newSituation = new Game(situation);
                    newSituation.logicalBoard[move.To.Row, move.To.Column] = newSituation.logicalBoard[move.From.Row, move.From.Column];
                    newSituation.logicalBoard[move.From.Row, move.From.Column] = new DummyPiece(ChessPieceType.Empty, move.From.Row, move.From.Column);
                    if (newSituation.logicalBoard[move.To.Row, move.To.Column].Type == ChessPieceType.King) newSituation.blackKing = move.To;
                    newSituation.logicalBoard[move.To.Row, move.To.Column].r = move.To.Row;
                    newSituation.logicalBoard[move.To.Row, move.To.Column].c = move.To.Column;
                    newSituation.WhiteOnTurn = true;

                    int value = minimax(depht + 1, false, newSituation, alpha, beta);
                    best = Math.Max(best, value);
                    alpha = Math.Max(alpha, best);
                    if (beta <= alpha) break;
                }
                return best;
            }
            else
            {
                int best = 1000;
                List<Move> moveList = new List<Move>();
                for (int r = 0; r < 8; r++)
                {
                    for (int c = 0; c < 8; c++)
                    {
                        if (activeGame.logicalBoard[r, c].isWhite)
                        {
                            Coords from = new Coords(Convert.ToByte(r), Convert.ToByte(c));
                            Coords[] moves = activeGame.logicalBoard[r, c].CalculatePossibleMoves(activeGame, from);
                            foreach (Coords to in moves)
                            {
                                Move move = new Move(from, to);
                                moveList.Add(move);
                            }
                        }
                    }
                }

                if (moveList.Count == 0) return evaulateBoard(situation);

                foreach (Move move in moveList)
                {
                    Game newSituation = new Game(situation);
                    newSituation.logicalBoard[move.To.Row, move.To.Column] = newSituation.logicalBoard[move.From.Row, move.From.Column];
                    newSituation.logicalBoard[move.From.Row, move.From.Column] = new DummyPiece(ChessPieceType.Empty, move.From.Row, move.From.Column);
                    if (newSituation.logicalBoard[move.To.Row, move.To.Column].Type == ChessPieceType.King) newSituation.whiteKing = move.To;
                    newSituation.logicalBoard[move.To.Row, move.To.Column].r = move.To.Row;
                    newSituation.logicalBoard[move.To.Row, move.To.Column].c = move.To.Column;
                    newSituation.WhiteOnTurn = false;

                    int value = minimax(depht + 1, true, newSituation, alpha, beta);
                    best = Math.Min(best, value);
                    beta = Math.Min(beta, best);
                    if (beta <= alpha) break;
                }
                return best;
            }
        }

        private int evaulateBoard(Game situation)
        {
            int value = 0;

            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    if (situation.logicalBoard[r, c].isWhite)
                    {
                        switch (situation.logicalBoard[r, c].Type)
                        {
                            case ChessPieceType.Pawn:
                                value -= 1;
                                break;
                            case ChessPieceType.Tower:
                                value -= 5;
                                break;
                            case ChessPieceType.Bishop:
                                value -= 3;
                                break;
                            case ChessPieceType.Knight:
                                value -= 3;
                                break;
                            case ChessPieceType.Queen:
                                value -= 9;
                                break;
                            default: 
                                break;
                        }
                    }
                    else
                    {
                        switch (situation.logicalBoard[r, c].Type)
                        {
                            case ChessPieceType.Pawn:
                                value += 1;
                                break;
                            case ChessPieceType.Tower:
                                value += 5;
                                break;
                            case ChessPieceType.Bishop:
                                value += 3;
                                break;
                            case ChessPieceType.Knight:
                                value += 3;
                                break;
                            case ChessPieceType.Queen:
                                value += 9;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }


            value += CheckCheck(situation);
            value += CheckMate(situation);
            return value;
        }

        private int evaluateMove(Game situation, Move move)
        {
            Coords[] moveArray = situation.logicalBoard[move.To.Row, move.To.Column].CalculatePossibleMoves(situation, move.To);
            int mValue = moveArray.Length;
            int[,] place = new int[8, 8] { { 1, 1, 1, 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1, 1, 1, 1}, { 1, 1, 2, 2, 2, 2, 1, 1}, { 1, 1, 2, 3, 3, 2, 1, 1}, { 2, 2, 2, 3, 3, 2, 2, 2}, { 3, 3, 4, 4, 4, 4, 3, 3}, { 4, 4, 4, 4, 4, 4, 4, 4}, { 4, 4, 4, 4, 4, 4, 4, 4} };
            int pValue = place[move.To.Row, move.To.Column];

            return pValue*mValue;
        }

        private int CheckCheck(Game situation)
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
                            if (hits[situation.whiteKing.Row, situation.whiteKing.Column]) return 30;
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
                            if (hits[situation.blackKing.Row, situation.blackKing.Column]) return -30;
                        }
                    }
                }
            }

            return 0;
        }

        private int CheckMate(Game situation)
        {
            if (situation.WhiteOnTurn)
            {
                for (int r = 0; r < 8; r++)
                {
                    for (int c = 0; c < 8; c++)
                    {
                        if (situation.logicalBoard[r, c].isWhite)
                        {
                            Coords[] moves = situation.logicalBoard[r, c].CalculatePossibleMoves(situation, new Coords(Convert.ToByte(r), Convert.ToByte(c)));
                            if (moves.Length != 0) return 300;
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
                        if (!situation.logicalBoard[r, c].isWhite)
                        {
                            Coords[] moves = situation.logicalBoard[r, c].CalculatePossibleMoves(situation, new Coords(Convert.ToByte(r), Convert.ToByte(c)));
                            if (moves.Length != 0) return -300;
                        }
                    }
                }
            }

            return 0;
        }
    }
}
