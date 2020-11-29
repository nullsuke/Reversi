using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] Board boardPrefab = default;
    [SerializeField] Button undoButton = default;
    [SerializeField] Text blackScoreText = default;
    [SerializeField] Text whiteScoreText = default;
    [SerializeField] Text blackNameText = default;
    [SerializeField] Text whiteNameText = default;
    [SerializeField] PassPanel passPanel = default;
    [SerializeField] ResultPanel resultPanel = default;
    private readonly Player[] players = new Player[2];
    private IEnumerator coroutineMain;
    private Board board;
    private bool isOver;

    public Player[] Players { get; set; }

    public void BackToTitle()
    {
        SceneManager.LoadScene("Title");
    }

    public void Undo()
    {
        board.Undo();
        board.RenderAll();
    }

    private void Awake()
    {
        board = Instantiate(boardPrefab);    
    }

    private void Start()
    {
        if (Players == null)
        {
            var h = new Human();
            var c = new Computer();

            players[0] = h;
            players[1] = c;
            players[(int)Stone.ColorType.Black].Color = Stone.ColorType.Black;
            players[(int)Stone.ColorType.White].Color = Stone.ColorType.White;

            board.SetUp(players);
        }
        else
        {
            board.SetUp(Players);
            blackNameText.text = Players[(int)Stone.ColorType.Black].Name;
            whiteNameText.text = Players[(int)Stone.ColorType.White].Name;
        }

        //ターンが変わるときのイベントを登録
        board.TurnChanged += (s, e) =>
        {
            var b = (Board)s;

            undoButton.interactable = b.CurrentPlayer is Human && b.CanUndo;

            blackScoreText.text = b.CountScore()[(int)Stone.ColorType.Black].ToString();
            whiteScoreText.text = b.CountScore()[(int)Stone.ColorType.White].ToString();
        };

        board.RenderAll();

        coroutineMain = CoroutineMain();
        StartCoroutine(coroutineMain);
    }
    
    private IEnumerator CoroutineMain()
    {
        while (!isOver)
        {
            //合法手ある
            if (board.ExistsMovablePoint())
            {
                board.Render();
                
                yield return new WaitForSeconds(0.5f);

                board.OnTurn();

                yield return new WaitWhile(() => board.IsThinking);
            }
            else //合法手がない
            {
                board.Pass();
                board.Next();

                //相手も合法手がない。ゲームオーバー
                if (!board.ExistsMovablePoint())
                {
                    isOver = true;

                    resultPanel.SetActive(true);

                    var scores = board.CountScore();
                    int b = (int)Stone.ColorType.Black;
                    int w = (int)Stone.ColorType.White;
                    int d = scores[b] - scores[w];

                    if (d > 0) resultPanel.Show(b);
                    else if (d < 0) resultPanel.Show(w);
                    else resultPanel.Show(2);

                    yield break;
                }
                else board.Previous(); //相手は合法手がある

                passPanel.SetActive(true);
                passPanel.Show(board.CurrentPlayer.Color);

                yield return new WaitForSeconds(0.5f);

                passPanel.SetActive(false);
            }

            board.Next();
        }

        yield return null;
    }
}
