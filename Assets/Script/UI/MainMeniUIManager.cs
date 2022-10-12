using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMeniUIManager : MonoBehaviour
{
    [SerializeField] private GameObject OptionPannel;
    [Space]
    [SerializeField] private Slider gameTimeSlider;
    [SerializeField] private Slider shooterSpawnTimeSlider;
    [SerializeField] private Slider suicideSpawnTimeSlider;
    [Space]
    [SerializeField] private TextMeshProUGUI gameTimeText;
    [SerializeField] private TextMeshProUGUI shooterSpawnTimeText;
    [SerializeField] private TextMeshProUGUI suicideSpawnTimeText;
    [Space]
    [SerializeField] private MatchConfiguration matchConfigObj;
    [Space]
    [SerializeField] private string gameTimeStartText = "Match duration: ";
    [SerializeField] private string shooterSpawnTimeStartText = "Spawn time of shooting enemies: ";
    [SerializeField] private string suicideSpawnTimeStartText = "Spawn time of suicide enemies: ";

    private void Start()
    {
        OptionPannel.SetActive(false);

        gameTimeSlider.value = matchConfigObj.matchDuration > gameTimeSlider.maxValue ? gameTimeSlider.maxValue : matchConfigObj.matchDuration;
        shooterSpawnTimeSlider.value = matchConfigObj.shooterSpawnTime > shooterSpawnTimeSlider.maxValue ? shooterSpawnTimeSlider.maxValue : matchConfigObj.shooterSpawnTime;
        suicideSpawnTimeSlider.value = matchConfigObj.suicideSpawnTime > shooterSpawnTimeSlider.maxValue ? shooterSpawnTimeSlider.maxValue : matchConfigObj.matchDuration;

        ShooterSpawnTimeChanged();
        SuicideSpawnTimeChanged();
        GameTimeChanged();
    }

    public void PlayButtonClick()
    {
        Debug.Log("Play button clicked");
        SceneManager.LoadScene("MainGame");
    }

    public void OptionsButtonClick()
    {
        Debug.Log("Options button clicked");
        OptionPannel.SetActive(true);
    }

    public void BackButtonClick()
    {
        Debug.Log("Back button clicked");
        OptionPannel.SetActive(false);
    }

    public void GameTimeChanged()
    {
        gameTimeText.text = gameTimeStartText + SecondsToTimeFormat(gameTimeSlider.value);
        matchConfigObj.matchDuration = (int) gameTimeSlider.value;
    }

    public void ShooterSpawnTimeChanged()
    {
        shooterSpawnTimeText.text = shooterSpawnTimeStartText + SecondsToTimeFormat(shooterSpawnTimeSlider.value);
        matchConfigObj.shooterSpawnTime = (int)shooterSpawnTimeSlider.value;
    }

    public void SuicideSpawnTimeChanged()
    {
        suicideSpawnTimeText.text = suicideSpawnTimeStartText + SecondsToTimeFormat(suicideSpawnTimeSlider.value);
        matchConfigObj.suicideSpawnTime = (int)suicideSpawnTimeSlider.value;
    }


    private string SecondsToTimeFormat(float time)
    {
        int fullMinutes = (int)time / 60;
        int seconds = (int)time - (fullMinutes * 60);

        return fullMinutes + ":" + ((seconds < 10) ? "0" + seconds : seconds);
    }

}
