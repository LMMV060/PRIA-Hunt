using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Fusion;

public class GameManagerTest : NetworkBehaviour
{
    public static GameManagerTest Instance;

    // Lista de jugadores solo en el host
    private List<PlayerData> players = new List<PlayerData>();

    private void Update()
    {
        try
        {
            //ShowAllHidersTime();
        }
        catch (Exception e)
        {
            Debug.LogWarning(e);
        }
        
    }
    
    private void ShowAllPlayers()
    {
        PlayerData[] allPlayers = FindObjectsByType<PlayerData>(FindObjectsSortMode.None);
        Debug.Log("---- Lista de jugadores ----");
        foreach (var p in allPlayers)
        {
            Debug.Log("Usuario: " + p.Name + " Tiempo vivo: " + p.TimeAlive + "Score:  " + p.Score); 
        }
        Debug.Log("---------------------------");
    }
    
    private void ShowAllPlayersScore()
    {
        PlayerData[] allPlayers = FindObjectsByType<PlayerData>(FindObjectsSortMode.None);
        Debug.Log("---- Lista de jugadores ----");
        foreach (var p in allPlayers)
        {
            Debug.Log("Usuario: " + p.Name + "Score:  " + p.Score); 
        }
        Debug.Log("---------------------------");
    }
    
    private void ShowAllPlayersTime()
    {
        PlayerData[] allPlayers = FindObjectsByType<PlayerData>(FindObjectsSortMode.None);
        Debug.Log("---- Lista de jugadores ----");
        foreach (var p in allPlayers)
        {
            Debug.Log("Usuario: " + p.Name + "Score:  " + p.Score); 
        }
        Debug.Log("---------------------------");
    }
    
    private void ShowAllHidersTime()
    {
        PlayerData[] allPlayers = FindObjectsByType<PlayerData>(FindObjectsSortMode.None);

        Debug.Log("---- Tiempo vivo de los hiders ----");

        foreach (var p in allPlayers)
        {
            //Recordatorio de que estoy hay que cambiarlo por 1
            if (p.CharacterType == 2) 
            {
                string timeFormatted = FormatTime(p.TimeAlive);
                Debug.Log($"Usuario: {p.Name} | Tiempo Vivo: {timeFormatted}");
            }
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