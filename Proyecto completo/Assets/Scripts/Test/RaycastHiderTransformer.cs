using System.Collections;
using Fusion;
using TMPro;
using UnityEngine;
using Behaviour = Fusion.Behaviour;

public class RaycastHiderTransformer : NetworkBehaviour
{
    [SerializeField] private Transform shootCam;
    [SerializeField] private float rango = 1f;
    [SerializeField] private Transform modelRoot;
    [SerializeField] private LayerMask raycastMask;
    [SerializeField] private GameObject propInfoPanel;
    [SerializeField] private TextMeshProUGUI propNameText;
    
    [SerializeField] private AudioClip failSound;
    [SerializeField] private AudioClip successSound;

    [Networked] private NetworkString<_32> CurrentProp { get; set; }

    private string _lastAppliedProp;

    void Update()
    {
        if (!Object.HasStateAuthority) return;

        Transform();
    }

    private void Transform()
    {
        bool foundProp = false;
        Vector3 endPos = shootCam.position + shootCam.forward * rango;
        RaycastHit[] hits = Physics.RaycastAll(shootCam.position, shootCam.forward, rango, raycastMask);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("Hider"))
                continue;

            Debug.Log($"Hit object: {hit.collider.gameObject.name} (Tag: {hit.collider.tag})");

            foundProp = true;

            // Para mostrar la UI
            propInfoPanel.SetActive(true);
            propNameText.text = hit.collider.gameObject.name;

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (hit.collider.CompareTag("Propt"))
                {
                    //Cambiamos el estado
                    Rpc_ReplaceModel(hit.collider.gameObject.name);
                    Rpc_PlayTransformSound(transform.position);
                    return;
                }
                else
                {
                    Rpc_PlayFailSound(transform.position);
                    return;
                }
            }
            break;
        }

        if (!foundProp)
        {
            propInfoPanel.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && !foundProp)
        {
            Rpc_PlayFailSound(transform.position);
        }
    }

    // RPC Actualiza el estado
    [Rpc(RpcSources.StateAuthority, RpcTargets.StateAuthority)]
    private void Rpc_ReplaceModel(string propName)
    {
        CurrentProp = propName;
    }

    // Aplicamos el modelo segun cuando se renderiza
    public override void Render()
    {
        string propToApply = CurrentProp.ToString();
        if (propToApply != _lastAppliedProp)
        {
            _lastAppliedProp = propToApply;
            ApplyModel(propToApply);
        }
    }

    private void ApplyModel(string propName)
    {
        if (string.IsNullOrEmpty(propName)) return;

        // Eliminar modelo actual
        foreach (Transform child in modelRoot)
            Destroy(child.gameObject);

        // Buscar el prop en escena
        GameObject prop = GameObject.Find(propName);
        if (prop == null) return;

        GameObject newModel = Instantiate(prop, modelRoot);
        newModel.transform.localRotation = prop.transform.localRotation;
        newModel.transform.localScale = prop.transform.localScale;
        //Si el modelo da problemas a la hora de cambiar el mapa cambia el 1.35
        float yDifference = prop.transform.position.y - transform.position.y;
        newModel.transform.localPosition = new Vector3(0, yDifference, 0);
        newModel.tag = "Hider";
        
        // Eliminar todos los componentes que NO sean MeshRenderer, MeshFilter o Collider
        foreach (var comp in newModel.GetComponentsInChildren<Component>())
        {
            if (comp is MeshRenderer || comp is MeshFilter || comp is Collider || comp is Transform)
                continue;
            Destroy(comp);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void Rpc_PlayTransformSound(Vector3 position)
    {
        float distance = Vector3.Distance(shootCam.transform.position, position);
        float volume = Mathf.Clamp01(1f - distance / 10f);

        AudioSource.PlayClipAtPoint(successSound, position, volume);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void Rpc_PlayFailSound(Vector3 position)
    {
        float distance = Vector3.Distance(shootCam.transform.position, position);
        float volume = Mathf.Clamp01(1f - distance / 10f);

        AudioSource.PlayClipAtPoint(failSound, position, volume);
    }
}
