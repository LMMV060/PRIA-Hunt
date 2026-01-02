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
    [SerializeField] private GameObject hunterScoreRanking;
    [SerializeField] private GameObject hiderTimeRanking;
    
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
            // Nombre LOCAL
            playerNameText.text = _localPlayerData.Name;

            //Score LOCAL
            scoreText.text = _localPlayerData.Score.ToString();
            
            // Imagen del personaje
            UpdateCharacterImage();
            hunterScoreRanking.SetActive(Input.GetKey(KeyCode.Tab));
            hiderTimeRanking.SetActive(Input.GetKey(KeyCode.Tab));
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
}