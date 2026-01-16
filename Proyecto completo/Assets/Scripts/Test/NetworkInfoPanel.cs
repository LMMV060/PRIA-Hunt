using System;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;

public class NetworkInfoPanel : NetworkBehaviour
{
    private List<PlayerData> networkedPlayers = new List<PlayerData>();
    [SerializeField] private GameObject hunterRanking;
    [SerializeField] private TextMeshProUGUI[] hunterRankingTexts;
    [SerializeField] private TextMeshProUGUI[] hunterRankingPoints;
    [SerializeField] private GameObject hiderRanking;
    [SerializeField] private TextMeshProUGUI[] hiderRankingTexts;
    [SerializeField] private TextMeshProUGUI[] hiderRankingTime;
    [SerializeField] private TextMeshProUGUI hunterNumberOne;
    [SerializeField] private TextMeshProUGUI hiderNumberOne;

    private void Update()
    {
        //Visual
        try
        {
            UpdateHunterRanking();
            UpdateHiderRanking();
            hunterRanking.SetActive(Input.GetKey(KeyCode.Tab));
            hiderRanking.SetActive(Input.GetKey(KeyCode.Tab));
        }
        catch (System.Exception e)
        {
            // Ignoramos el error si el objeto fue despawneado
            Debug.LogWarning($"No se pudo acceder a Networked property: {e.Message}");
        }
        
    }

    private void ShowPlayers()
    {
        PlayerData[] allPlayers = FindObjectsByType<PlayerData>(FindObjectsSortMode.None);
        
        foreach (var player in allPlayers)
        {
            Debug.Log(
                $"Nombre: {player.Name} | " +
                $"Score: {player.Score} | " +
                $"Tiempo: {FormatTime(player.TimeAlive)}"
            );
        }

        Debug.Log("----------------------------------");
    }
    
    private void UpdateHunterRanking()
    {
        PlayerData[] allPlayers = FindObjectsByType<PlayerData>(FindObjectsSortMode.None);

        // 1. Filtrar solo hunters
        List<PlayerData> hunters = new List<PlayerData>();
        foreach (var p in allPlayers)
        {
            if (p.CharacterType == 0)
                hunters.Add(p);
        }

        // 2. Ordenar por score (mayor a menor)
        hunters.Sort((a, b) => b.Score.CompareTo(a.Score));

        // 3. Rellenar UI
        for (int i = 0; i < hunterRankingTexts.Length; i++)
        {
            if (i < hunters.Count)
            {
                hunterRankingTexts[i].text = hunters[i].Name;
                hunterRankingPoints[i].text = hunters[i].Score.ToString();
            }
            else
            {
                // Si no hay jugador para ese slot
                hunterRankingTexts[i].text = "---";
                hunterRankingPoints[i].text = "0";
            }
        }
        // 4. Mostrar Hunter Number One solo si supera los 30 segundos

        if (hunters.Count > 0 && hunters[0].Score >= 3)
        {
            hunterNumberOne.text = $"El hunter {hunters[0].Name} esta ganando por <color=yellow>{hunters[0].Score}</color> puntos";
        }
        else
        {
            hunterNumberOne.text = "";
        }
    }

    private void UpdateHiderRanking()
    {
        PlayerData[] allPlayers = FindObjectsByType<PlayerData>(FindObjectsSortMode.None);

        // 1. Filtrar solo hiders
        List<PlayerData> hiders = new List<PlayerData>();
        foreach (var p in allPlayers)
        {
            if (p.CharacterType == 1)
                hiders.Add(p);
        }

        // 2. Ordenar por tiempo vivo (mayor a menor)
        hiders.Sort((a, b) => b.TimeAlive.CompareTo(a.TimeAlive));

        // 3. Rellenar UI
        for (int i = 0; i < hiderRankingTexts.Length; i++)
        {
            if (i < hiders.Count)
            {
                hiderRankingTexts[i].text = hiders[i].Name;
                hiderRankingTime[i].text = FormatTime(hiders[i].TimeAlive);
            }
            else
            {
                hiderRankingTexts[i].text = "---";
                hiderRankingTime[i].text = "00:00";
            }
        }
        
        // 4. Mostrar Hider Number One solo si supera los 30 segundos
        if (hiders.Count > 0 && hiders[0].TimeAlive >= 30f)
        {
            hiderNumberOne.text = $"El hider {hiders[0].Name} esta ganando con <color=yellow>{FormatTime(hiders[0].TimeAlive)}</color>";
        }
        else
        {
            hiderNumberOne.text = "";
        }
    }


    private string FormatTime(float time)
    {
        int totalSeconds = Mathf.FloorToInt(time);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        return $"{minutes:00}:{seconds:00}";
    }
}
