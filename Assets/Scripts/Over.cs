using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Over : MonoBehaviour
{
    public GameObject gameOverText;
    public GameObject winText;
    public static GameObject GameOverStatic;
    public static GameObject WinStatic;
    public static int contadorMuertes = 0;
    private static Over instance;

    void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        Over.GameOverStatic = gameOverText;
        Over.WinStatic = winText;

        Over.GameOverStatic.gameObject.SetActive(false);
        Over.WinStatic.gameObject.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void showGameOver()
    {
        Over.GameOverStatic.gameObject.SetActive(true);
        instance.StartCoroutine(instance.EndGame());
    }

    public static void showVictory()
    {
        Over.WinStatic.gameObject.SetActive(true);
    }

    IEnumerator EndGame()
    {
        yield return new WaitForSeconds(5);
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
