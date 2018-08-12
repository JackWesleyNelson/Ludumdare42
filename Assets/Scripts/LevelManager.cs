using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {
    public LevelManager Instance { get; private set; }
    [SerializeField]
    private UpdateUI updateUI;

    public void Start() {
        if(Instance == null) {
            Instance = this;
        }
        if(this != Instance) {
            Destroy(this.gameObject);
        }
    }

    public void MainMenu() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        updateUI.StartGame();
    }

    public void ExitGame() {
        Application.Quit();
    }

}
