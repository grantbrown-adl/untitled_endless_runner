using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour {
    [SerializeField] private GameObject[] _spawnableObjects;
    [SerializeField] private string _poolName;
    [SerializeField] private int _poolSize;
    [SerializeField] private int _initialItemMultiplier;
    [SerializeField] private List<GameObject> _objectPool;
    [SerializeField] private GameObject _container;
    [SerializeField] private GameObject _poolParent;

    public GameObject[] SpawnableObjects { get => _spawnableObjects; set => _spawnableObjects = value; }
    public List<GameObject> Pool { get => _objectPool; set => _objectPool = value; }

    private void Awake() {
        if (_initialItemMultiplier <= 0) _initialItemMultiplier = 1;
        _poolSize = _spawnableObjects.Length * _initialItemMultiplier;
        RefreshPool();
    }

    void InitialiseObjectPool() {
        for (int i = 0; i < _poolSize; i++) {
            _objectPool.Add(CreateInstance(index: i));
        }
    }

    private GameObject CreateInstance(int index) {
        int modIndex = (index + 1) % _spawnableObjects.Length;

        GameObject instance = Instantiate(_spawnableObjects[modIndex]);
        instance.transform.SetParent(_container.transform);
        instance.transform.position = transform.position;
        instance.SetActive(false);
        return instance;
    }

    public GameObject GetInstance() {
        foreach (GameObject spawnableObject in _objectPool) {
            if (!spawnableObject.activeInHierarchy) {
                spawnableObject.transform.position = transform.position;
                return spawnableObject;
            }
        }

        ExpandPool();
        return GetInstance();
    }

    public GameObject GetRandomInstance() {
        int randomIndex = Random.Range(0, _objectPool.Count - 1);

        for (int i = 0; i < _objectPool.Count; i++) {
            if (!_objectPool[randomIndex].activeInHierarchy) {
                return _objectPool[randomIndex];
            }

            randomIndex = (randomIndex + 1) % _objectPool.Count;
        }

        ExpandPool();
        return GetRandomInstance();
    }

    public void RefreshPool() {
        if (_container != null) {
            Destroy(_container);
            _container = null;
        }
        if (_poolSize <= 0) _poolSize = 1;
        _objectPool = new();
        if (_poolParent == null) _poolParent = new GameObject($"Object Pool");
        if (_container == null) _container = new GameObject($"~~~ {_poolName} ~~~");
        _container.transform.SetParent(_poolParent.transform);

        InitialiseObjectPool();
    }

    public void ReturnInstance(GameObject instance) {
        instance.SetActive(false);
    }

    public IEnumerator DelayedReturnInstance(GameObject instance, float delay) {
        yield return new WaitForSeconds(delay);
        ReturnInstance(instance);
    }

    private void ExpandPool() {
        int currentPoolSize = _poolSize;
        _poolSize *= 2;

        for (int i = 0; i < currentPoolSize; i++) {
            _objectPool.Add(CreateInstance(index: i));
        }
    }
}