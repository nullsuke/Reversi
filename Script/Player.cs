using System;
using UnityEngine;

public abstract class Player
{
    public Stone.ColorType Color { get; set; }
    public string Name { get; protected set; }

    public abstract void OnTurn();
}

public class Human : Player
{
    public Human()
    {
        Name = "You";
    }

    public override void OnTurn()
    {
    }
}

public class Computer : Player
{
    public event EventHandler<MoveEventArgs> Move;
    private readonly string[] Names = { "Comp Lv1", "Comp Lv2", "Comp Lv3" };
    private readonly AI ai;
    
    public Computer(int lv = 2)
    {
        Name = Names[lv - 1];
        ai = new AI(lv);
    }
    
    public LogicBoard LogicBoard { private get; set; }

    public override void OnTurn()
    {
        //var sw = new System.Diagnostics.Stopwatch();
        //sw.Start();
        var p = ai.Search(LogicBoard, Color);
        //Debug.Log(sw.Elapsed.TotalMilliseconds);
        //sw.Stop();
        Move?.Invoke(this, new MoveEventArgs(p));
    }
}

public class MoveEventArgs:EventArgs
{
    public Vector2Int MovePoint;

    public MoveEventArgs(Vector2Int p)
    {
        MovePoint = p;
    }
}
