using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ViewBoard : MonoBehaviour
{
    [SerializeField] Cell cellPrefabs = default;
    private readonly int size = 8;
    private readonly List<Cell> cells = new List<Cell>();
    private readonly List<Stone> stones = new List<Stone>();
    private readonly List<Mark> marks = new List<Mark>();
    //相手の最後の手
    private RectTransform lastMoved;

    public bool HasAllReversed { get; private set; }

    public void Setup(EventHandler<EventArgs> handler)
    {
        CreateCells();
        cells.ForEach(c => c.PointerClick += handler );
    }

    //合法手を描画
    public void RenderMarks(List<Vector2Int> movable, Stone.ColorType color)
    {
        marks.ForEach(m => Destroy(m.gameObject));
        marks.Clear();

        RenderMovableMarks(movable, color);
    }

    //全てを描画
    public void RenderAll(List<Vector2Int> blacks, List<Vector2Int> whites, 
        List<Vector2Int> movable, Stone.ColorType color)
    {
        Clear();
        blacks.ForEach(p => RenderOne(p, Stone.ColorType.Black));
        whites.ForEach(p => RenderOne(p, Stone.ColorType.White));
        
        RenderMovableMarks(movable, color);

        if (lastMoved != null) Destroy(lastMoved.gameObject);
    }

    //指した石を描画
    public void RenderOne(Vector2Int p, Stone.ColorType color)
    {
        var cell = GetCell(p);

        if (stones.Exists(s => s.X == p.x && s.Y == p.y))
        {
            throw new Exception("duplication");
        }

        var stone = cell.CreateStone(color);
        stones.Add(stone);

        //最後の手
        if (lastMoved != null) Destroy(lastMoved.gameObject);
        lastMoved = cell.CreateMoved();
    }

    public void Reverse(List<Vector2Int> rp)
    {
        HasAllReversed = false;
        StartCoroutine(CoroutineReverse(rp));
    }

    private void CreateCells()
    {
        var cellSprites = Resources.LoadAll<Sprite>("Cells");
        int i = 0;

        for (var y = 0; y < size; y++)
        {
            for (var x = 0; x < size; x++)
            {
                var cell = Instantiate(cellPrefabs, transform);

                cell.Setup(new Vector3(x, y * -1), cellSprites[i]);
                cells.Add(cell);

                i++;
            }
        }
    }

    private void RenderMovableMarks(List<Vector2Int> points, Stone.ColorType color)
    {
        points.ForEach(p =>
        {
            var cell = GetCell(p);

            var mark = cell.CreateMark(color);
            marks.Add(mark);
        });
    }

    //回転できる石を回転させる
    private IEnumerator CoroutineReverse(List<Vector2Int> rps)
    {
        var revered = new List<Stone>();

        foreach(var p in rps)
        {
            var s = GetStone(p);
            s.Reverse();
            revered.Add(s);
        }

        //一つでも回転中なら待機
        yield return new WaitWhile(() => revered.Exists(s => !s.HasReversed));

        HasAllReversed = true;
    }

    private Cell GetCell(Vector2Int p)
    {
        Cell cell = null;
        cell = cells.FirstOrDefault(c => c.X == p.x && c.Y == p.y);

        if (cell == null) throw new Exception("no exsits cell");

        return cell;
    }

    private Stone GetStone(Vector2Int p)
    {
        Stone stone = null;
        stone = stones.FirstOrDefault(s => s.X == p.x && s.Y == p.y);

        if (stone == null) throw new Exception("no exsits stone");

        return stone;
    }

    private void Clear()
    {
        stones.ForEach(s => Destroy(s.gameObject));
        stones.Clear();

        marks.ForEach(m => Destroy(m.gameObject));
        marks.Clear();
    }
}
