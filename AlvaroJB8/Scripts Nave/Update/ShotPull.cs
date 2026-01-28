using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ShotPull : MonoBehaviour
{
    [SerializeField] private GameObject shotPrefab;
    [SerializeField] private int initialpoolSize = 5;
    
    private List<GameObject> _pool;

    private void Awake()
    {
        _pool = new List<GameObject>();

        for (int i = 0; i < initialpoolSize; i++)
        {
            GameObject fire = Instantiate(shotPrefab, transform, true);
            fire.SetActive(false);
            _pool.Add(fire);
        }
    }

    public GameObject RequestShot()
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
        GameObject shot = Instantiate(shotPrefab, transform, true);
        shot.SetActive(true);
        _pool.Add(shot);
        return shot;
    }
}
