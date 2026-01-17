using System;
using Fusion;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using TMPro;
using UnityEngine;

public class PlayerData : NetworkBehaviour
{
    [SerializeField] private TMP_Text nicknameText;
    
    [Networked] public string Name { get; set; }
    [Networked] public int CharacterType { get; set; }
    [Networked] public int Score { get; set; }
    [Networked] public float TimeAlive { get; set; }

    
    public override void Spawned()
    {
        // Solo el jugador local asigna valores
        if (Object.HasInputAuthority)
        {
            Name = PhotonNetwork.LocalPlayer.NickName;
            CharacterType = CharacterSelector.personajeSeleccionado;

            PhotonNetwork.LocalPlayer.SetScore(0);
            Score = PhotonNetwork.LocalPlayer.GetScore();

            TimeAlive = 0f;
        }

        UpdateNickname();
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasInputAuthority)
        {
            TimeAlive += Runner.DeltaTime;
        }
    }
    
    public void Update()
    {
        UpdateNickname(); 
        UpdateScore();
    }

    private void UpdateTimeAlive()
    {
        if (!Object.HasStateAuthority) return;

        TimeAlive += Runner.DeltaTime;

        // Mostrar en formato MM:SS
        //Debug.Log($"Tiempo vivo: {GetTimeAliveFormatted()}");
    }
    
    private string GetTimeAliveFormatted()
    {
        int totalSeconds = Mathf.FloorToInt(TimeAlive);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        return $"{minutes:00}:{seconds:00}";
    }
    
    public void ResetTimeAlive()
    {
        if (!Object.HasStateAuthority) return;

        TimeAlive = 0f;
        Debug.Log("Tiempo vivo reseteado: 00:00");
    }
    private void UpdateNickname()
    {
        if (nicknameText == null) return;

        try
        {
            nicknameText.text = Name;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"No se pudo actualizar el nickname: {e.Message}");
        }
    }

    private void UpdateScore()
    {
        try
        {
            if (!Object.HasInputAuthority)
                return;

            Score = PhotonNetwork.LocalPlayer.GetScore();
        }
        catch (Exception e)
        {
            Debug.LogWarning($"No se pudo actualizar el score: {e.Message}");
        }
    }
    
}
