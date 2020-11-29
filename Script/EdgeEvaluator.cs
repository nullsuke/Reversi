using System;

public class EdgeEvaluator
{
    private static ulong topLeft;
    private static ulong topRight;
    private static ulong bottomLeft;
    private static ulong bottomRight;
    private static readonly ulong topLeftMask = 0x8000000000000000;
    private static readonly ulong topRightMask = 0x0100000000000000;
    private static readonly ulong bottomLeftMask = 0x0000000000000080;
    private static readonly ulong bottomRightMask = 0x0000000000000001;

    public static void Setup(ulong bit)
    {
        //4隅を取っているか調べる
        topLeft = bit & topLeftMask;
        topRight = bit & topRightMask;
        bottomLeft = bit & bottomLeftMask;
        bottomRight = bit & bottomRightMask;
    }

    //確定石(4隅とそれに隣接する石)の数を数える
    public static int CountStableStone(ulong bit)
    {
        int cnt = 0;

        cnt += CountTopStableStone(bit);
        cnt += CountLeftStableStone(bit);
        cnt += CountBottomStableStone(bit);
        cnt += CountRightStableStone(bit);

        if (topLeft != 0) cnt++;    
        if (topRight != 0) cnt++;
        if (bottomLeft != 0) cnt++;
        if (bottomRight != 0) cnt++;

        return cnt;
    }

    //C打ちの数を数える
    public static int CountCMove(ulong bit)
    {
        ulong topLeftC1 = 0x4000000000000000;
        ulong topLeftC2 = 0x0080000000000000;
        ulong topRightC1 = 0x0200000000000000;
        ulong topRightC2 = 0x0001000000000000;
        ulong bottomLeftC1 = 0x0000000000000040;
        ulong bottomLeftC2 = 0x0000000000008000;
        ulong bottomRightC1 = 0x0000000000000002;
        ulong bottomRightC2 = 0x0000000000000100;
        int cnt = 0;

        if (topLeft == 0)
        {
            cnt += (bit & topLeftC1) > 0 ? 1 : 0;
            cnt += (bit & topLeftC2) > 0 ? 1 : 0;
        }

        if (topRight == 0)
        {
            cnt += (bit & topRightC1) > 0 ? 1 : 0;
            cnt += (bit & topRightC2) > 0 ? 1 : 0;
        }

        if (bottomLeft == 0)
        {
            cnt += (bit & bottomLeftC1) > 0 ? 1 : 0;
            cnt += (bit & bottomLeftC2) > 0 ? 1 : 0;
        }

        if (bottomRight == 0)
        {
            cnt += (bit & bottomRightC1) > 0 ? 1 : 0;
            cnt += (bit & bottomRightC2) > 0 ? 1 : 0;
        }

        return cnt;
    }

    //X打ちの数を数える
    public static int CountXMove(ulong bit)
    {
        ulong topLeftX = 0x0040000000000000;
        ulong topRightX = 0x0002000000000000;
        ulong bottomLeftX = 0x0000000000004000;
        ulong bottomRightX = 0x0000000000000200;
        int cnt = 0;

        if (topLeft == 0)
        {
            cnt += (bit & topLeftX) > 0 ? 1 : 0;
        }

        if (topRight == 0)
        {
            cnt += (bit & topRightX) > 0 ? 1 : 0;
        }

        if (bottomLeft == 0)
        {
            cnt += (bit & bottomLeftX) > 0 ? 1 : 0;
        }

        if (bottomRight == 0)
        {
            cnt += (bit & bottomRightX) > 0 ? 1 : 0;
        }

        return cnt;
    }

    //上辺の確定石の数を数える
    private static int CountTopStableStone(ulong bit)
    {
        int cnt = 0;

        cnt = CountStableStoneInLine(bit, topLeft,
            corner => corner >> 1);

        if (cnt < 6)
        {
            cnt += CountStableStoneInLine(bit, topRight,
                corner => corner << 1);
        }

        return cnt;
    }

    //右辺の確定石の数を数える
    private static int CountRightStableStone(ulong bit)
    {
        int cnt = 0;

        cnt = CountStableStoneInLine(bit, topRight,
            corner => corner >> 8);

        if (cnt < 6)
        {
            cnt += CountStableStoneInLine(bit, bottomRight,
                corner => corner << 8);
        }

        return cnt;
    }

    //下辺の確定石の数を数える
    private static int CountBottomStableStone(ulong bit)
    {
        int cnt = 0;

        cnt = CountStableStoneInLine(bit, bottomLeft,
            corner => corner >> 1);

        if (cnt < 6)
        {
            cnt += CountStableStoneInLine(bit, bottomRight,
                corner => corner << 1);
        }

        return cnt;
    }

    //左辺の確定石の数を数える
    private static int CountLeftStableStone(ulong bit)
    {
        int cnt = 0;

        cnt = CountStableStoneInLine(bit, topLeft,
            corner => corner >> 8);

        if (cnt < 6)
        {
            cnt += CountStableStoneInLine(bit, bottomLeft,
                corner => corner << 8);
        }

        return cnt;
    }

    //辺の確定石の数を数える
    private static int CountStableStoneInLine(ulong bit, ulong corner,
        Func<ulong, ulong> shift)
    {
        int cnt = 0;

        if (corner != 0)
        {
            ulong next = bit & shift(corner);

            while (next != 0 && cnt < 6)
            {
                cnt++;
                next = bit & shift(next);
            }
        }

        return cnt;
    }
}