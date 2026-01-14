using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Vector2 speed;

    private void Update()
    {
        transform.Translate(speed * Time.deltaTime);
    }

    public void HitByPlayerShot()
    {
        Destroy(this.gameObject);
    }
}
