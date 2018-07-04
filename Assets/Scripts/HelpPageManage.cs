using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpPageManage : MonoBehaviour {
    public GameObject Page1;
    public GameObject Page2;
    public GameObject Page3;
    public GameObject HelpPanel;
    public GameObject GroupPanel;

	void Start () {
        Page1.SetActive(false);
        Page2.SetActive(false);
        Page3.SetActive(false);
        HelpPanel.SetActive(false);
    }

    public void ToPage1()
    {
        Page1.SetActive(true);
        Page2.SetActive(false);
        Page3.SetActive(false);
        HelpPanel.SetActive(true);
    }
    public void ToPage2()
    {
        Page1.SetActive(false);
        Page2.SetActive(true);
        Page3.SetActive(false);
        HelpPanel.SetActive(true);
    }
    public void ToPage3()
    {
        Page1.SetActive(false);
        Page2.SetActive(false);
        Page3.SetActive(true);
        HelpPanel.SetActive(true);
    }
    public void ToGroup()
    {
        HelpPanel.SetActive(false);
        GroupPanel.SetActive(true);
    }
    public void Reset()
    {
        HelpPanel.SetActive(false);
        GroupPanel.SetActive(false);
    }
}
