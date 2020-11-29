using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [SerializeField] Dropdown blackDropdown = default;
    [SerializeField] Dropdown whiteDropdown = default;
    private readonly Player[] players = new Player[2];

    public void StartGame()
    {
        int black = (int)Stone.ColorType.Black;
        int white = (int)Stone.ColorType.White;

        players[black] = CreatePlayer(blackDropdown.value, Stone.ColorType.Black);
        players[white] = CreatePlayer(whiteDropdown.value, Stone.ColorType.White);

        SceneManager.sceneLoaded += LoadScene;
        SceneManager.LoadScene("Main");
    }

    private void LoadScene(Scene scene, LoadSceneMode mode)
    {
        var gm = GameObject.FindObjectOfType<Camera>().GetComponent<GameManager>();
        gm.Players = players;
        SceneManager.sceneLoaded -= LoadScene;
    }

    private Player CreatePlayer(int n, Stone.ColorType color)
    {
        Player p;
        if (n == 0) p = new Human();
        else p = new Computer(n);

        p.Color = color;

        return p;
    }
}
