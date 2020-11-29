using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public event EventHandler<EventArgs> TurnChanged;
    private readonly Stack<int> humanLog = new Stack<int>();
    private Player[] players;
    private LogicBoard logicBoard;
    private ViewBoard viewBoard;
    private ulong movable;
    private int turn;

    public Player CurrentPlayer { get => players[turn % 2]; }
    public bool IsThinking { get; private set; }
    public bool CanUndo { get => humanLog.Count > 0; }

    public void SetUp(Player[] players)
    {
        this.players = players;

        foreach(var p in players)
        {
            var cmp = p as Computer;

            if (cmp == null) continue;

            cmp.LogicBoard = logicBoard;
            //コンピュータが石を置いたときのイベントを登録
            cmp.Move += (s, e) => Move(e.MovePoint);
        }

        logicBoard.Move(3, 3, Stone.ColorType.Black);
        logicBoard.Move(4, 3, Stone.ColorType.White);
        logicBoard.Move(4, 4, Stone.ColorType.Black);
        logicBoard.Move(3, 4, Stone.ColorType.White);
   
        turn = 0;

        humanLog.Clear();
    }

    //合法手があるか判定
    public bool ExistsMovablePoint()
    {
        IsThinking = true;

        movable = logicBoard.GetMovableBitBoard(CurrentPlayer.Color);

        return movable > 0;
    }

    //合法手のセルにマークを表示
    public void Render()
    {
        var movables = LogicBoard.BitToPoints(movable);

        viewBoard.RenderMarks(movables, CurrentPlayer.Color);
    }

    public void RenderAll()
    {
        var bits = logicBoard.CurrentBitBoards;

        int black = (int)Stone.ColorType.Black;
        int white = (int)Stone.ColorType.White;
        var blacks = LogicBoard.BitToPoints(bits[black]);
        var whites = LogicBoard.BitToPoints(bits[white]);

        if (blacks.Count == 0 && whites.Count == 0) return;

        movable = logicBoard.GetMovableBitBoard(CurrentPlayer.Color);
        var movables = LogicBoard.BitToPoints(movable);

        viewBoard.RenderAll(blacks, whites, movables, CurrentPlayer.Color);
    }

    public void Next()
    {
        turn++;
        TurnChanged?.Invoke(this, EventArgs.Empty);
    }
    
    public void Previous()
    {
        turn--;
    }

    public void OnTurn()
    {
        CurrentPlayer.OnTurn();
    }

    public void Move(Vector2Int p)
    {
        StartCoroutine(CoroutineMove(p));
    }

    public void Pass()
    {
        logicBoard.Log();
    }

    public void Undo()
    {
        var l = humanLog.Pop();

        for (int i = turn; i > l; i--)
        {
            logicBoard.Undo();
            turn--;
        }

        TurnChanged?.Invoke(this, EventArgs.Empty);
    }

    public int[] CountScore()
    {
        return logicBoard.CountScore();
    }

    private void Awake()
    {
        logicBoard = new LogicBoard();

        viewBoard = GetComponent<ViewBoard>();
        
        viewBoard.Setup((s, e) =>
        {
            if (!(CurrentPlayer is Human)) return;

            var cell = s as Cell;

            var mps = LogicBoard.BitToPoints(movable);

            if (!mps.Exists(p => p.x == cell.X && p.y == cell.Y))
            {
                Debug.Log("can't move");
                return;
            }

            if (cell != null)
            {
                humanLog.Push(turn);
                var p = new Vector2Int(cell.X, cell.Y);
                StartCoroutine(CoroutineMove(p));
            }
        });

        movable = 0;
    }

    //石を置いた時の処理。石の回転を表示する為にコルーチンを用いる
    private IEnumerator CoroutineMove(Vector2Int p)
    {
        while (true)
        {
            //logicBoard.Move(p.x, p.y, CurrentPlayer.Color);
            //描画のみ。ビットボードの更新はReverse実行時
            viewBoard.RenderOne(p, CurrentPlayer.Color);

            logicBoard.Log();
            var r = logicBoard.Reverse(p.x, p.y, CurrentPlayer.Color);
            var rps = LogicBoard.BitToPoints(r);
            viewBoard.Reverse(rps);

            yield return new WaitWhile(() => !viewBoard.HasAllReversed);

            IsThinking = false;
            break;
        }
    }
}
