using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSelector : MonoBehaviour
{
    public void LoadScene(string target)
    {
        SceneManager.LoadScene(target);
        SceneManager.LoadScene("Game", LoadSceneMode.Additive);
        GameManager.ActiveMap = target;
        GameManager.Instance?.Restart();
    }
}
