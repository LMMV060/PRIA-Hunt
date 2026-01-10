using System.Collections;
using Fusion;
using TMPro;
using UnityEngine;
using Behaviour = Fusion.Behaviour;

public class RaycastHiderTransformer : NetworkBehaviour
{
    [SerializeField] private Transform shootCam;
    [SerializeField] private float rango = 20f;
    //[SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform modelRoot;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private LayerMask raycastMask;
    [SerializeField] private GameObject propInfoPanel;
    [SerializeField] private TextMeshProUGUI propNameText;
    
    void Update()
    {
        if (!Object.HasStateAuthority) return;

        Transform();
        
    }

    private void Transform()
    {
        bool foundProp = false;
        Vector3 endPos = shootCam.position + shootCam.forward * rango;
        RaycastHit[] hits = Physics.RaycastAll( shootCam.position,  shootCam.forward, rango, raycastMask);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("Hider"))
                continue;

            Debug.Log($"Hit object: {hit.collider.gameObject.name} (Tag: {hit.collider.tag})");

            foundProp = true;

            // Mostrar UI
            propInfoPanel.SetActive(true);
            propNameText.text = hit.collider.gameObject.name;
            
            if (Input.GetKeyDown(KeyCode.Mouse0) && hit.collider.CompareTag("Propt"))
            {
                Rpc_ReplaceModel(hit.collider.gameObject.name);
            }

            break; 
        }
        
        if (!foundProp)
        {
            propInfoPanel.SetActive(false);
        }

        // Mostrar la línea en todos los clientes
        //Rpc_DrawShotLine(shootCam.position, endPos);
    }
   
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void Rpc_DrawShotLine(Vector3 start, Vector3 end)
    {
        StartCoroutine(DrawShotLine(start, end));
    }
    

    
    private IEnumerator DrawShotLine(Vector3 start, Vector3 end)
    {
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);

        lineRenderer.enabled = true;

        yield return new WaitForSeconds(0.1f);

        lineRenderer.enabled = false;
    }
    
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void Rpc_ReplaceModel(string propName)
    {
        // Eliminar modelo actual
        foreach (Transform child in modelRoot)
            Destroy(child.gameObject);

        // Buscar el prop en escena
        GameObject prop = GameObject.Find(propName);
        if (prop == null) return;

        GameObject newModel = Instantiate(prop, modelRoot);
        newModel.transform.localRotation = prop.transform.localRotation;
        newModel.transform.localScale = prop.transform.localScale;
        newModel.transform.localPosition = new Vector3(0, prop.transform.position.y+1.5f, 0);
        newModel.tag = "Hider";
        
        /*
        Renderer rend = newModel.GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            float localMinY = rend.localBounds.min.y;
            newModel.transform.localPosition = new Vector3(0, -localMinY, 0);
        }
        */

        // Eliminar todos los componentes que NO sean MeshRenderer, MeshFilter o Collider
        
        foreach (var comp in newModel.GetComponentsInChildren<Component>())
        {
            if (comp is MeshRenderer || comp is MeshFilter || comp is Collider || comp is Transform)
                continue;
            Destroy(comp);
        }
    }


}