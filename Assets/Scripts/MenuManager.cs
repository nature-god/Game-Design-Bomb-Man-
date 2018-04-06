using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {

    public static bool choose;
    
    public void ClickFirstViewButton()
    {
        choose = true;
        SceneManager.LoadScene("Local2PlayerGame");
    }

    public void ClickThirdViewButton()
    {
        choose = false;
        SceneManager.LoadScene("Local2PlayerGame");
    }
}
