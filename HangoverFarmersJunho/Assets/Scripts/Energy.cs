using UnityEngine;
using TMPro;
using System;
using System.Collections;

public class EnergySystem : MonoBehaviour
{
    public int maxEnergy = 5;
    public float energyRechargeTime = 20f * 60f; // 20 minutos em segundos
    public TextMeshProUGUI energyText;
    public TextMeshProUGUI timeToNextEnergyText; // Novo campo para mostrar o tempo restante

    private int currentEnergy;
    private DateTime lastEnergyUseTime;


    #region Singleton

    public static EnergySystem instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        energyText = GameObject.Find("Energia").GetComponent<TextMeshProUGUI>();
        timeToNextEnergyText = GameObject.Find("Tempo").GetComponent <TextMeshProUGUI>();
    }

    #endregion


    void Start()
    {
        LoadEnergyData();
        UpdateEnergyText();
        DontDestroyOnLoad(gameObject);
    }

   public IEnumerable initializedscene()
    {
        yield return new WaitForSeconds(2);
        energyText = GameObject.Find("Energia").GetComponent<TextMeshProUGUI>();
        timeToNextEnergyText = GameObject.Find("Tempo").GetComponent<TextMeshProUGUI>();
        
    }

    void Update()
    {
        
        RechargeEnergy();
        UpdateTimeToNextEnergyText(); // Atualiza o tempo restante a cada frame
    }

    public void UseEnergy()
    {
        if (currentEnergy > 0)
        {
            currentEnergy--;
            lastEnergyUseTime = DateTime.Now;
            SaveEnergyData();
            UpdateEnergyText();
        }
        else
        {
            Debug.Log("Sem energia suficiente!");
        }
    }

    private void RechargeEnergy()
    {
        TimeSpan timeSinceLastUse = DateTime.Now - lastEnergyUseTime;
        int energyToRecharge = (int)(timeSinceLastUse.TotalSeconds / energyRechargeTime);

        if (energyToRecharge > 0)
        {
            currentEnergy = Mathf.Min(maxEnergy, currentEnergy + energyToRecharge);
            lastEnergyUseTime = lastEnergyUseTime.AddSeconds(energyToRecharge * energyRechargeTime);
            SaveEnergyData();
            UpdateEnergyText();
        }
    }

    private void UpdateEnergyText()
    {
        energyText.text = "Energia: " + currentEnergy.ToString();
    }

    private void UpdateTimeToNextEnergyText()
    {
        if (currentEnergy < maxEnergy)
        {
            TimeSpan timeUntilNextEnergy = lastEnergyUseTime.AddSeconds(energyRechargeTime) - DateTime.Now;

            if (timeUntilNextEnergy.TotalSeconds > 0)
            {
                timeToNextEnergyText.text = string.Format("Recarga: {1:D2}:{2:D2}",
                    timeUntilNextEnergy.Hours,
                    timeUntilNextEnergy.Minutes,
                    timeUntilNextEnergy.Seconds);
            }
            else
            {
                timeToNextEnergyText.text = "Recarga: 00:00";
            }
        }
        else
        {
            timeToNextEnergyText.text = "Energia cheia!";
        }
    }

    public void StartGame()
    {
        if (currentEnergy > 0)
        {
            Debug.Log("Jogo iniciado!");
            // Código para iniciar o jogo
        }
        else
        {
            Debug.Log("Você precisa de mais energia para iniciar o jogo.");
        }
    }

    public void OnGameLost()
    {
        UseEnergy();
    }

    private void SaveEnergyData()
    {
        PlayerPrefs.SetInt("CurrentEnergy", currentEnergy);
        PlayerPrefs.SetString("LastEnergyUseTime", lastEnergyUseTime.ToString());
        PlayerPrefs.Save();
    }

    private void LoadEnergyData()
    {
        currentEnergy = PlayerPrefs.GetInt("CurrentEnergy", maxEnergy);
        string lastEnergyUseTimeStr = PlayerPrefs.GetString("LastEnergyUseTime", DateTime.Now.ToString());
        lastEnergyUseTime = DateTime.Parse(lastEnergyUseTimeStr);
    }

    public void Adcionar()
    {
        currentEnergy++;
        UpdateEnergyText();
    }
}
