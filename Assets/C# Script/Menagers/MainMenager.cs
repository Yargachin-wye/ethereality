using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using TMPro;


public class MainMenager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textTime;
    [SerializeField] private TextMeshProUGUI _textMain;

    private float timer = 0;

    private void Start()
    {
        _textMain.gameObject.SetActive(false);
        Time.timeScale = 1;
        timer = 0;
        _textMain.text = "";
    }
    private void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        _textTime.text = Math.Round(timer, 2).ToString();
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Harpoon>() != null)
        {
            Stop();
        }
    }
    private void Stop()
    {
        Time.timeScale = 0;
        _textMain.text = "You survived\n" + Math.Round(timer,2).ToString() + " sec";
        _textMain.gameObject.SetActive(true);
    }
    public void RestartSurvival()
    {
        SceneManager.LoadScene("SurvivalScene");
        _textMain.gameObject.SetActive(false);
        Time.timeScale = 1;
    }
    public void Win()
    {
        _textMain.text = "You climbed to the top\n" + Math.Round(timer, 2).ToString() + " sec";
        _textMain.gameObject.SetActive(true);
        Time.timeScale = 0;
    }
}
