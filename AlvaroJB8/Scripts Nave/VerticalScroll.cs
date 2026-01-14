using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class VerticalScroll : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 1f;
    [SerializeField] private bool limitCameraToSprites = false;
    [SerializeField] private List<Sprite> sprites;
    [SerializeField] private Player player;

    // SpriteRenderer reference (for bounds calculation)
    private SpriteRenderer _tileA;

    // next/Previous GameObject-SpriteRenderer
    private SpriteRenderer _tileB;

    // actual sprite index
    private int _spriteIndex = 0;

    // track if the player was in upper half on last frame
    private bool _wasPlayerInUpperHalf = false;

    // previous signed distance from player to tile border
    private float _prevPlayerToBorder;

    private void Awake()
    {
        _tileA = GetComponent<SpriteRenderer>();
        
        // create tile B
        var goB = new GameObject(this.gameObject.name + "_TileB");
        _tileB = goB.AddComponent<SpriteRenderer>();
        // copy renderer settings from tile a to tile b
        _tileB.sortingLayerID = _tileA.sortingLayerID;
        _tileB.sortingOrder = _tileA.sortingOrder;
        _tileB.color = _tileA.color;
        _tileB.sharedMaterial = _tileA.sharedMaterial;
        _tileB.flipX = _tileA.flipX;
        _tileB.flipY = _tileA.flipY;
        _tileB.drawMode = _tileA.drawMode;
        _tileB.size = _tileA.size;
        _tileB.maskInteraction = _tileA.maskInteraction;
        // keep tile B aligned with tile a transform settings
        goB.transform.position = transform.position;
        goB.transform.rotation = transform.rotation;
        goB.transform.localScale = transform.localScale;
        
        // if sprites list is empty (or only one sprite), add the current sprite to fill it until have at least two sprites
        if (sprites.Count == 0)
        {
            sprites.Add(_tileA.sprite);
        }
        else if (sprites.Count == 1)
        {
            sprites.Add(sprites[0]);
        }
        
        // set the actual sprite
        if (sprites.Count > 0)
        {
            _tileA.sprite = sprites[_spriteIndex];
        }
    }
    
    private void Start()
    {
        // get reference to player if not set
        if (player == null)
        {
            player = GameManager.Instance.Player.GetComponent<Player>();
        }
        
        // set the initial half state and place the adjacent tile accordingly
        if (player != null)
        {
            _wasPlayerInUpperHalf = player.transform.position.y >= _tileA.bounds.center.y;
            UpdateAdjacentTile();
            // initialize relative distance to border
            //_prevPlayerToBorder = GetPlayerToTileBorderDelta();
        }
        
        // if limitCameraToSprites is true, create GameObjects (LeftLimit and RightLimit) to limit the camera movement on X axis on Start
        if (limitCameraToSprites)
        {
            float spriteWidth = _tileA.bounds.size.x;
            float minX = transform.position.x - spriteWidth / 2;
            float maxX = transform.position.x + spriteWidth / 2;

            GameObject leftLimit = new GameObject("LeftCameraLimit");
            leftLimit.transform.position = new Vector3(minX, 0, 0);

            GameObject rightLimit = new GameObject("RightCameraLimit");
            rightLimit.transform.position = new Vector3(maxX, 0, 0);
            
            // register the limits in the GameManager "CameraLimits"
            GameManager.Instance.CameraLimitLeft = leftLimit;
            GameManager.Instance.CameraLimitRight = rightLimit;
        }
    }
    
    private void Update()
    {
        // apply autonomous vertical scroll
        ScrollTiles();
        
        // detect when the player crosses from lower half to upper half (or vice versa)
        if (player == null || sprites == null || sprites.Count < 2) return;

        bool isPlayerInUpperHalf = player.transform.position.y >= _tileA.bounds.center.y;

        if (isPlayerInUpperHalf != _wasPlayerInUpperHalf)
        {
            _wasPlayerInUpperHalf = isPlayerInUpperHalf;
            UpdateAdjacentTile();
        }
        
        // swap tiles when the player crosses the border between tiles
        float currentDelta = GetPlayerToTileBorderDelta();

        bool crossed =
            (_prevPlayerToBorder < 0f && currentDelta >= 0f) ||
            (_prevPlayerToBorder > 0f && currentDelta <= 0f);

        if (crossed)
        {
            SwapTiles();
            // recompute delta after the swap because tile A / B order changed
            _prevPlayerToBorder = GetPlayerToTileBorderDelta();
        }
        else
        {
            _prevPlayerToBorder = currentDelta;
        }
    }
    
    private void UpdateAdjacentTile()
    {
        // update tile B sprite and position based on the player's half within tile A
        if (sprites == null || sprites.Count < 2) return;

        bool isPlayerInUpperHalf = player.transform.position.y >= _tileA.bounds.center.y;

        // if the player is in the upper half, the "next" tile is above (forward = +1)
        // if the player is in the lower half, the "next" tile is below (backward = -1)
        int adjacentIndex = isPlayerInUpperHalf
            ? (_spriteIndex + 1) % sprites.Count
            : (_spriteIndex - 1 + sprites.Count) % sprites.Count;

        _tileB.sprite = sprites[adjacentIndex];

        // position tile B by aligning bounds edges to avoid gaps/overlaps
        float tileA_top = _tileA.bounds.max.y;
        float tileA_bottom = _tileA.bounds.min.y;
        float tileB_halfHeight = _tileB.bounds.extents.y;

        float newY = isPlayerInUpperHalf
            ? (tileA_top + tileB_halfHeight)     // place tile B above tile A
            : (tileA_bottom - tileB_halfHeight); // place tile B below tile A

        _tileB.transform.position = new Vector3(_tileA.transform.position.x, newY, _tileA.transform.position.z);
        
        // keep border delta consistent after repositioning tile B
        _prevPlayerToBorder = GetPlayerToTileBorderDelta();
    }

    private void SwapTiles()
    {
        // determine whether tile B is above or below tile A
        bool tileBIsAbove = _tileB.transform.position.y > _tileA.transform.position.y;
        
        // update the current sprite index to match the new active tile after the swap
        _spriteIndex = tileBIsAbove ? (_spriteIndex + 1) % sprites.Count : (_spriteIndex - 1 + sprites.Count) % sprites.Count;
        
        // swap references so tile B becomes the current tile
        // SpriteRenderer temp = _tileA;
        // _tileA = _tileB;
        // _tileB = temp;
        (_tileA, _tileB) = (_tileB, _tileA);

        // refresh half state based on the new current tile
        _wasPlayerInUpperHalf = player.transform.position.y >= _tileA.bounds.center.y;

        // update the adjacent tile sprite and position coherently
        UpdateAdjacentTile();
    }

    private void ScrollTiles()
    {
        // move both tiles downward using scroll speed
        float deltaY = scrollSpeed * Time.deltaTime;

        Vector3 offset = Vector3.down * deltaY;

        _tileA.transform.position += offset;
        _tileB.transform.position += offset;
    }
    private float GetPlayerToTileBorderDelta()
    {
        //detect whether tile B is above or below tile A
        bool tileBIsAbove = _tileB.transform.position.y > _tileA.transform.position.y;
        float borderY = tileBIsAbove ? _tileA.bounds.max.y : _tileA.bounds.min.y;
        
        //signed distance: positive means player is past the border
        return player.transform.position.y - borderY;
    }
}