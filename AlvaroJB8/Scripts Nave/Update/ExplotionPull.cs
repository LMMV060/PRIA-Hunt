using System.Collections.Generic;
using UnityEngine;

public class ExplotionPull : MonoBehaviour
{
    [SerializeField] private GameObject explotionPrefab;
    [SerializeField] private int initialpoolSize = 5;
    
    private List<GameObject> _pool;

    private void Awake()
    {
        _pool = new List<GameObject>();

        for (int i = 0; i < initialpoolSize; i++)
        {
            GameObject fire = Instantiate(explotionPrefab, transform, true);
            fire.SetActive(false);
            _pool.Add(fire);
        }
    }

    public GameObject RequestExplotion()
    {
        for (int i = 0; i < _pool.Count; i++)
        {
            if (!_pool[i].activeSelf)
            {
                _pool[i].SetActive(true);
                return _pool[i];
            }
        }
        initialpoolSize++;
        GameObject shot = Instantiate(explotionPrefab, transform, true);
        shot.SetActive(true);
        _pool.Add(shot);
        return shot;
    }
}
