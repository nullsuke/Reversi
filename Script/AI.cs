using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AI
{
    private readonly Dictionary<ulong, float> sortedMoves = new Dictionary<ulong, float>();
    private readonly int depth;
    private LogicBoard board;

    public AI(int lv = 2)
    {
        depth = lv * 2 + 1;
    }

    //最善手を探す
    public Vector2Int Search(LogicBoard logicBoard, Stone.ColorType color)
    {
        board = logicBoard;
        ulong found = 0;
        var maxEval = Mathf.NegativeInfinity;
        var beta = Mathf.Infinity;
        var a = Mathf.NegativeInfinity;
        var b = beta;

        var movables = GetSortedMovailable(color, a, b);

        if (movables.Count == 1) return LogicBoard.BitToPoints(movables[0])[0];

        for (int i = 0; i < movables.Count; i++)
        {
            var p = movables[i];

            board.Log();
            board.Reverse(p, color);

            var eval = -NegaScout(Stone.GetReverseColor(color), depth - 1,
                -b, -a);

            //最初の子ノードは再探索しない(ソートがうまくいっていれば評価が最大だから？)
            //葉から2つ以内のノードは再探索しない(なぜ？）
            if (i != 0 && depth > 2 && eval > a && eval < beta)
            {
                eval = -NegaScout(Stone.GetReverseColor(color), depth - 1,
                -beta, -eval);
            }

            board.Undo();

            if (maxEval < eval)
            {
                a = Mathf.Max(a, eval);
                maxEval = eval;
                found = p;
            }

            b = a + 1;
        }

        return LogicBoard.BitToPoints(found)[0];
    }

    //NegaScout法
    private float NegaScout(Stone.ColorType color, int depth,
        float alpha, float beta)
    {
        if (depth == 0) return Evaluator.Evaluate(board, color);

        var movables = GetSortedMovailable(color, alpha, beta);

        if (movables.Count == 0)
        {
            board.Log();

            var eval = -NegaScout(Stone.GetReverseColor(color), depth - 1,
                -beta, -alpha);

            board.Undo();

            return eval;
        }

        var maxEval = Mathf.NegativeInfinity;
        var a = alpha;
        var b = beta;

        for (int i = 0; i < movables.Count; i++)
        {
            var p = movables[i];

            board.Log();
            board.Reverse(p, color);

            var eval = -NegaScout(Stone.GetReverseColor(color), depth - 1,
                -b, -a);
            
            //最初の子ノードは再探索しない(ソートがうまくいっていれば評価が最大だから？)
            //葉から2つ以内のノードは再探索しない(なぜ？）
            if (i != 0 && depth > 2 && eval > a && eval < beta)
            {
                eval = -NegaScout(Stone.GetReverseColor(color), depth - 1,
                -beta, -eval);
            }

            board.Undo();

            if (maxEval < eval)
            {
                if (beta <= eval)
                {
                    return eval;
                }

                a = Mathf.Max(a, eval);
                maxEval = eval;
            }

            b = a + 1;
        }

        //子ノードの最大値を返す(fail-soft)
        return maxEval;
    }

    //NegaAlpha法
    private float NegaAlpha(Stone.ColorType color, int depth,
        float alpha, float beta)
    {
        if (depth == 0) return Evaluator.Evaluate(board, color);

        var movable = board.GetMovableBitBoard(color);

        if (LogicBoard.CountBit(movable) == 0)
        {
            board.Log();

            var eval = -NegaAlpha(Stone.GetReverseColor(color), depth - 1,
                -beta, -alpha);

            board.Undo();

            return eval;
        }

        while (movable != 0)
        {
            ulong next = GetLeastSignificantBit(movable);

            board.Log();
            board.Reverse(next, color);

            var eval = -NegaAlpha(Stone.GetReverseColor(color), depth - 1,
                -beta, -alpha);

            board.Undo();

            if (beta <= eval)
            {
                return eval;
            }

            alpha = Mathf.Max(eval, alpha);

            movable ^= next;
        }

        return alpha;
    }

    //浅い探索を行いソートした合法手を得る
    private List<ulong> GetSortedMovailable(Stone.ColorType color, float alpha, float beta)
    {
        sortedMoves.Clear();

        var movable = board.GetMovableBitBoard(color);

        while (movable != 0)
        {
            var next = GetLeastSignificantBit(movable);

            board.Log();
            board.Reverse(next, color);

            var eval = -NegaAlpha(Stone.GetReverseColor(color), 1,
                -beta, -alpha);

            board.Undo();

            sortedMoves.Add(next, eval);

            movable ^= next;
        }

        return sortedMoves.OrderByDescending(s => s.Value).Select(x => x.Key).ToList<ulong>();
    }

    //一番右端のビットを取得
    private ulong GetLeastSignificantBit(ulong bit)
    {
        var comp = ~bit + 1;
        return bit & comp;
    }
}
