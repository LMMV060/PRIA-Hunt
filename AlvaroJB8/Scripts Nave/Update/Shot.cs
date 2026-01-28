using System;
using UnityEngine;
[RequireComponent(typeof(Collider2D))]
public class Shot : MonoBehaviour
{
    [SerializeField] private float speed = 10;
    //[SerializeField] private float ttl = 3;
    [SerializeField] private AudioClip shotSound;
    //[SerializeField] private float maxDistance = 6f;
    [SerializeField] private GameObject explotionPrefab;
    private ExplotionPull explotionPull;

    //private float _ttl;
    //private float _maxDistance;
    
    //private SpriteRenderer _spriteRenderer;
    //private bool _hasBeenVisible;

    private void Awake()
    {
        //_ttl = ttl;
        //_spriteRenderer = GetComponent<SpriteRenderer>();
        //_hasBeenVisible = false;
    }

    void Start()
    {
        //_maxDistance = maxDistance;
        explotionPull = FindFirstObjectByType<ExplotionPull>();
        AudioSource.PlayClipAtPoint(shotSound, transform.position, 1f);
    }

    void Update()
    {
        transform.Translate(Vector2.up * (speed * Time.deltaTime));
        
        /*_ttl -= Time.deltaTime;
        if (_ttl <= 0)
        {
            Destroy(gameObject);
        }*/
        
        //Eliges cuanto recorre
        /*_maxDistance -= speed * Time.deltaTime;
        if (_maxDistance <= 0f)
        {
            Destroy(gameObject);
        }*/
        
        //verify visibility
        /*if (_spriteRenderer.isVisible)
        {
            _hasBeenVisible = true;
        }
        else
        {
            if (_hasBeenVisible)
            {
                Destroy(gameObject);
            }
        }*/
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            //verify if the object is an Enemy
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.HitByPlayerShot();
                //GameObject explotion = Instantiate(explotionPrefab);
                GameObject explotion = explotionPull.RequestExplotion();
                explotion.transform.position = transform.position;
                //Destroy(this.gameObject);
                this.gameObject.SetActive(false);
            }
        }
    }
}
