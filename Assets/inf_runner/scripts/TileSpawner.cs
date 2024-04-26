using System.Collections;
using UnityEngine;

public class TileSpawner : MonoBehaviour {
    [Header("Spawn Settings")]
    [SerializeField] int _maxTileCount;
    [SerializeField] int _maxDistanceFromOrigin = 10000;
    [SerializeField] int _mapSectionLength = 25;
    [SerializeField] bool _randomTile;

    [Header("Visualised Internals")]
    [SerializeField] private int _spawnedTileCount;
    [SerializeField] ObjectPool _tilePool;
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
        GameObject instance = _randomTile ? _tilePool.GetRandomInstance() : _tilePool.GetInstance();
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
        if (_playerTransform.position.z > _maxDistanceFromOrigin + (_mapSectionLength / 2)) {
            _playerExceededPlayArea = true;
        }

        for (int i = 0; i < _tilePool.Pool.Count; i++) {
            GameObject tile = _tilePool.Pool[i];
            bool despawnTile = tile.transform.position.z - _playerTransform.position.z < -_mapSectionLength;

            if (tile.activeInHierarchy && despawnTile) {
                SpawnTile();
                DespawnTile(tile);
                i--;
            }
        }
    }

    private void ResetGameWorld() {
        _playerExceededPlayArea = false;

        int activeTiles = 0;

        for (int i = 0; i < _tilePool.Pool.Count; i++) {
            GameObject tile = _tilePool.Pool[i];

            if (tile.activeInHierarchy) {
                tile.transform.position -= new Vector3(0, 0, _maxDistanceFromOrigin);
                activeTiles++;
            }
        }

        _nextTileLocation = new Vector3(0, 0, (activeTiles - 1) * _mapSectionLength);
        _playerTransform.position = Vector3.zero;
    }

    private void ResetGameWorld1() {
        _playerExceededPlayArea = false;
        _playerTransform.position = Vector3.zero;

        int activeTiles = 0;
        float furthestTileZ = float.MinValue;

        // Find the furthest tile from the origin
        for (int i = 0; i < _tilePool.Pool.Count; i++) {
            GameObject tile = _tilePool.Pool[i];

            if (tile.activeInHierarchy) {
                float tileZ = tile.transform.position.z;
                if (tileZ > furthestTileZ)
                    furthestTileZ = tileZ;
            }
        }

        // Calculate the offset to bring all tiles back to the positive Z axis
        float offset = _maxDistanceFromOrigin - furthestTileZ;

        // Reposition active tiles and count them
        for (int i = 0; i < _tilePool.Pool.Count; i++) {
            GameObject tile = _tilePool.Pool[i];

            if (tile.activeInHierarchy) {
                tile.transform.position += new Vector3(0, 0, offset);
                activeTiles++;
            }
        }

        // If no active tiles are found, spawn a new tile at the origin
        if (activeTiles == 0) {
            SpawnTile();
        }

        // Update the next tile location
        _nextTileLocation = new Vector3(0, 0, activeTiles * _mapSectionLength);
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
