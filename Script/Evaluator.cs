public static class Evaluator
{
    //合法手の数に対する重み
    private static readonly float mobilityWeight= 67f;
    //確定石の数に対する重み
    private static readonly float stableWeight = 101f;
    //C打ちの数に対する重み
    private static readonly float cMoveWeight = -552f;
    //X打ちの数に対する重み
    private static readonly float xMoveWeight = -449f;
    private static LogicBoard board;

    public static float Evaluate(LogicBoard logicBoard, Stone.ColorType corrent)
    {
        board = logicBoard;

        var currentBit = board.CurrentBitBoards[(int)corrent];
        int currentMovable = CountMovableCells(corrent);

        EdgeEvaluator.Setup(currentBit);
        int currentStable = EdgeEvaluator.CountStableStone(currentBit);
        int currentCMove = EdgeEvaluator.CountCMove(currentBit);
        int currentXMove = EdgeEvaluator.CountXMove(currentBit);

        var opponentColor = Stone.GetReverseColor(corrent);
        
        var opponentBit = board.CurrentBitBoards[(int)opponentColor];
        int opponentMovable = CountMovableCells(opponentColor);

        EdgeEvaluator.Setup(opponentBit);
        int opponentStable = EdgeEvaluator.CountStableStone(opponentBit);
        int opponentCMove = EdgeEvaluator.CountCMove(opponentBit);
        int opponentXMove = EdgeEvaluator.CountXMove(opponentBit);

        float eval = (currentMovable - opponentMovable) * mobilityWeight+
            (currentStable - opponentStable) * stableWeight +
            (currentCMove - opponentCMove) * cMoveWeight +
            (currentXMove - opponentXMove) * xMoveWeight;

        return eval;
    }

    //合法手の数を取得
    private static int CountMovableCells(Stone.ColorType color)
    {
        var movable = board.GetMovableBitBoard(color);
        return LogicBoard.CountBit(movable);
    }
}
