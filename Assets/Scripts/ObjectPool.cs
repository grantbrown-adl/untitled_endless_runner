using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour {
    [SerializeField] private GameObject _spawnableObject;
    [SerializeField] private int _poolSize;
    [SerializeField] private List<GameObject> _objectPool;
    [SerializeField] private GameObject _container;
    private GameObject _poolParent;

    public GameObject SpawnableObject { get => _spawnableObject; set => _spawnableObject = value; }
    public List<GameObject> Pool { get => _objectPool; set => _objectPool = value; }

    private void Start() {
        if (_poolSize <= 0) _poolSize = 1;
        RefreshPool();
    }

    void InitialiseObjectPool() {
        for (int i = 0; i < _poolSize; i++) {
            _objectPool.Add(CreateInstance());
        }
    }

    private GameObject CreateInstance() {
        GameObject instance = Instantiate(_spawnableObject);
        instance.transform.SetParent(_poolParent.transform);
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

    public void RefreshPool() {
        if (_container != null) {
            Destroy(_container);
            _container = null;
        }
        _poolSize = 1;
        _objectPool = new();
        _poolParent = new GameObject($"Object Pool - {_spawnableObject.name}");
        if (_container == null) _container = new GameObject($"Object Pool - {_spawnableObject.name}");
        _poolParent.transform.SetParent(_container.transform);

        InitialiseObjectPool();
    }

    public static void ReturnInstance(GameObject instance) {
        instance.SetActive(false);
    }

    public static IEnumerator DelayedReturnInstance(GameObject instance, float delay) {
        yield return new WaitForSeconds(delay);
        ReturnInstance(instance);
    }

    private void ExpandPool() {
        int currentPoolSize = _poolSize;
        _poolSize *= 2;

        for (int i = 0; i < currentPoolSize; i++) {
            _objectPool.Add(CreateInstance());
        }
    }
}