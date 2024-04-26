using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSpawner : MonoBehaviour {
    [Header("Spawn Settings")]
    [SerializeField] int _maxTileCount;
    [SerializeField] int _maxDistanceFromOrigin = 10000;
    [SerializeField] int _mapSectionLength = 25;

    [Header("Visualised Internals")]
    [SerializeField] private int _spawnedTileCount;
    [SerializeField] ObjectPool _tilePool;
    [SerializeField] private List<GameObject> _activeTiles;
    [SerializeField] Vector3 _nextTileLocation;
    [SerializeField] Transform _playerTransform;
    [SerializeField] Vector3 _playerTransformPosition;
    [SerializeField] bool _playerExceededPlayArea;

    readonly WaitForSeconds _waitFor100ms = new(0.1f);
    readonly WaitForSeconds _waitFor200ms = new(0.2f);
    readonly WaitForSeconds _waitFor500ms = new(0.5f);


    public ObjectPool TilePool { get => _tilePool; set => _tilePool = value; }
    #region Singleton
    public static TileSpawner Instance { get => _instance; set => _instance = value; }

    private static TileSpawner _instance;

    private void CreateSingleton() {
        if (_instance != null && _instance != this) Destroy(gameObject);
        else {
            _instance = this;
        }
    }
    #endregion

    private void Awake() {
        CreateSingleton();
    }

    private void Start() {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        InitialiseTiles();
        StartGameTicks();
    }

    private void Update() {
        _playerTransformPosition = _playerTransform.position;
    }

    private void InitialiseTiles() {
        for (int i = 0; i < _maxTileCount; i++) {
            SpawnTile();
        }
    }

    private void SpawnTile() {
        _spawnedTileCount++;
        GameObject instance = _tilePool.GetInstance();
        instance.transform.position = _nextTileLocation;
        _nextTileLocation = instance.transform.position + new Vector3(0, 0, _mapSectionLength);
        instance.SetActive(true);
    }

    private void DespawnTile(GameObject tile) {
        _spawnedTileCount--;
        tile.transform.position = Vector3.zero;
        _tilePool.ReturnInstance(tile);
    }

    private void HandleTiles() {
        if (_playerTransform.position.z > _maxDistanceFromOrigin) {
            _playerExceededPlayArea = true;
        }

        foreach (GameObject tile in _tilePool.Pool) {
            bool despawnTile = tile.transform.position.z - _playerTransform.position.z < -_mapSectionLength;

            if (tile.activeInHierarchy && despawnTile) {
                SpawnTile();
                DespawnTile(tile);
            }
        }
    }

    private void StartGameTicks() {
        StartCoroutine(Update100Ms());
        StartCoroutine(Update500Ms());
        StartCoroutine(Update200Ms());
    }

    IEnumerator Update100Ms() {
        while (true) {
            HandleTiles();
            yield return _waitFor100ms;
        }
    }

    IEnumerator Update500Ms() {
        while (true) {
            yield return _waitFor500ms;
        }
    }

    IEnumerator Update200Ms() {
        while (true) {
            yield return _waitFor200ms;
        }
    }
}
