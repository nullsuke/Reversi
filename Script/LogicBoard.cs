using System;
using System.Collections.Generic;
using UnityEngine;

public class LogicBoard
{
    private readonly Stack<ulong[]> log = new Stack<ulong[]>();
    private ulong[] bitBoards = new ulong[2];

    public ulong[] CurrentBitBoards
    {
        get
        {
            return (ulong[])bitBoards.Clone();
        }
        set
        {
            bitBoards = value;
        }
    }

    public LogicBoard()
    {
        bitBoards[(int)Stone.ColorType.Black] = 0x0000000000000000;
        bitBoards[(int)Stone.ColorType.White] = 0x0000000000000000;
    }

    //ビットボードをList<Vector2Int>に変換
    public static List<Vector2Int> BitToPoints(ulong bitBoard)
    {
        var mask = 0x8000000000000000;

        var points = new List<Vector2Int>();

        for (int i = 0; i < 64; i++)
        {
            if ((bitBoard & (mask >> i)) != 0)
            {
                var x = i % 8;
                var y = (int)(i / 8);
                points.Add(new Vector2Int(x, y));
            }
        }

        return points;
    }

    //1の数を数える
    public static int CountBit(ulong bitBoard)
    {
        bitBoard = (bitBoard & 0x5555555555555555) + (bitBoard >> 1 & 0x5555555555555555);
        bitBoard = (bitBoard & 0x3333333333333333) + (bitBoard >> 2 & 0x3333333333333333);
        bitBoard = (bitBoard & 0x0f0f0f0f0f0f0f0f) + (bitBoard >> 4 & 0x0f0f0f0f0f0f0f0f);
        bitBoard = (bitBoard & 0x00ff00ff00ff00ff) + (bitBoard >> 8 & 0x00ff00ff00ff00ff);
        bitBoard = (bitBoard & 0x0000ffff0000ffff) + (bitBoard >> 16 & 0x0000ffff0000ffff);
        bitBoard = (bitBoard & 0xffffffffffffffff) + (bitBoard >> 32 & 0xffffffffffffffff);

        return (int)bitBoard;
    }

    public void Move(int x, int y, Stone.ColorType color)
    {
        var mask = 0x8000000000000000;
        var move = mask >> x + y * 8;

        bitBoards[(int)color] |= move;
    }

    //合法手を返す
    public ulong GetMovableBitBoard(Stone.ColorType color)
    {
        var current = bitBoards[(int)color];
        var opponent = bitBoards[(int)color ^ 1];
        //左右のセルを除いた相手の盤の状態
        var horizontalWatcher = opponent & 0x7e7e7e7e7e7e7e7e;
        //上下のセルを除いた相手の盤の状態
        var verticalWatcher = opponent & 0x00ffffffffffff00;
        //上下左右のセルを除いた相手の盤の状態
        var allSideWatcher = opponent & 0x007e7e7e7e7e7e00;
        var empty = ~(current | opponent);

        //8方向それぞれの合法手を調べ重ね合わせる
        var movable = GetMovableBitBoardInLine(current, empty, horizontalWatcher,
            c => c << 1);

        movable |= GetMovableBitBoardInLine(current, empty, horizontalWatcher,
            c => c >> 1);

        movable |= GetMovableBitBoardInLine(current, empty, verticalWatcher,
            c => c << 8);

        movable |= GetMovableBitBoardInLine(current, empty, verticalWatcher,
            c => c >> 8);

        movable |= GetMovableBitBoardInLine(current, empty, allSideWatcher,
            c => c << 7);

        movable |= GetMovableBitBoardInLine(current, empty, allSideWatcher,
            c => c >> 7);

        movable |= GetMovableBitBoardInLine(current, empty, allSideWatcher,
            c => c << 9);

        movable |= GetMovableBitBoardInLine(current, empty, allSideWatcher,
            c => c >> 9);

        return movable;
    }

    public ulong Reverse(ulong move, Stone.ColorType color)
    {
        int c = (int)color;
        var opponent = bitBoards[c ^ 1];
        ulong allReversed = 0;
        ulong lineReversed;

        //一方向ずつ反転できる石を探し重ね合わせる
        for (var i = 0; i < 8; i++)
        {
            var next = ShiftBitboard(move, i);
            lineReversed = 0;

            while (next != 0 && (next & opponent) != 0)
            {
                lineReversed |= next;
                next = ShiftBitboard(next, i);
            }

            if ((next & bitBoards[c]) != 0)
            {
                allReversed |= lineReversed;
            }
        }

        //石を置き、反転させる。
        bitBoards[c] |= move | allReversed;
        bitBoards[c ^ 1] ^= allReversed;

        return allReversed;
    }

    public ulong Reverse(int x, int y, Stone.ColorType color)
    {
        var mask = 0x8000000000000000;
        var move = mask >> (x + y * 8);

        return Reverse(move, color);
    }

    public void Log()
    {
        log.Push(CurrentBitBoards);
    }

    public void Undo()
    {
        if (log.Count == 0) throw new Exception("Undo Error");
        CurrentBitBoards = log.Pop();
    }

    public int[] CountScore()
    {
        var scores = new int[2];
        int black = (int)Stone.ColorType.Black;
        int white = (int)Stone.ColorType.White;

        scores[black] = CountBit(CurrentBitBoards[black]);
        scores[white] = CountBit(CurrentBitBoards[white]);

        return scores;
    }

    //任意の方向にある合法手を取得
    private ulong GetMovableBitBoardInLine(ulong current, ulong empty,
       ulong watcher, Func<ulong, ulong> shift)
    {
        ulong temp = watcher & shift(current);

        for (var i = 0; i < 5; i++)
        {
            temp |= watcher & shift(temp);
        }

        return empty & shift(temp);
    }

    //任意の方向にシフト
    private ulong ShiftBitboard(ulong current, int dir)
    {
        ulong next = 0;

        switch (dir)
        {
            case 0:
                next = 0xffffffffffffff00 & (current << 8);
                break;
            case 1:
                next = 0x00ffffffffffffff & (current >> 8);
                break;
            case 2:
                next = 0xfefefefefefefefe & (current << 1);
                break;
            case 3:
                next = 0x7f7f7f7f7f7f7f7f & (current >> 1);
                break;
            case 4:
                next = 0x7f7f7f7f7f7f7f00 & (current << 7);
                break;
            case 5:
                next = 0x00fefefefefefefe & (current >> 7);
                break;
            case 6:
                next = 0xfefefefefefefe00 & (current << 9);
                break;
            case 7:
                next = 0x007f7f7f7f7f7f7f & (current >> 9);
                break;
        }

        return next;
    }
}
