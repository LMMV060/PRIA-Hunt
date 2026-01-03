using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class InfoPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private RawImage characterImage;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Texture[] characterSprites;
    [SerializeField] private GameObject playerInfoPanel;
    
    private PlayerData _localPlayerData;
    private int _lastCharacterType = -1;
    
    void Update()
    {
        
        if (_localPlayerData == null)
        {
            FindLocalPlayer();
            return;
        }
        
        try
        {
            //Variables locales:
            
            // Nombre LOCAL
            playerNameText.text = _localPlayerData.Name;

            //Score LOCAL
            if (_localPlayerData.CharacterType == 0)
            {
                scoreText.text = _localPlayerData.Score.ToString();
            }
            else
            {
                //Tiempo LOCAL
                scoreText.text = FormatTime(_localPlayerData.TimeAlive);
            }
            
            // Imagen del personaje
            UpdateCharacterImage();
            
            //Visual
            playerInfoPanel.SetActive(Input.GetKey(KeyCode.Tab));
        }
        catch (System.Exception e)
        {
            // Ignoramos el error si el objeto fue despawneado
            Debug.LogWarning($"No se pudo acceder a Networked property: {e.Message}");
            _localPlayerData = null; // Forzamos a volver a buscar el jugador
        }
    }

    private void UpdateCharacterImage()
    {
        int type = _localPlayerData.CharacterType;

        if (type == _lastCharacterType) return;

        if (type >= 0 && type < characterSprites.Length)
        {
            characterImage.texture = characterSprites[type];
            _lastCharacterType = type;
        }
    }

    private void FindLocalPlayer()
    {
        foreach (PlayerData player in FindObjectsByType<PlayerData>(FindObjectsSortMode.None))
        {
            if (player.Object.HasInputAuthority)
            {
                _localPlayerData = player;
                break;
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