using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverUI;

    private void Start()
    {
        SceneManager.activeSceneChanged += voltar;
    }
    public void updateScene(string Main)
    {
        SceneManager.LoadScene(Main);
        EnergySystem.instance.initializedscene();
    }
    public void doExitGame()
    {
        Application.Quit();
    }

    public void gameOver()
    {
        gameOverUI.SetActive(true);
        EnergySystem.instance.UseEnergy();
        Debug.Log("energia");
    }

    public void voltar (string FarmHub)
    {
        SceneManager.LoadScene(FarmHub);
        EnergySystem.instance.initializedscene();
        Debug.Log("fjuhu");
    }

}
