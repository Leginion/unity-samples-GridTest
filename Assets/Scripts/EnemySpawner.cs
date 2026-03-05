using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("生成参数")]
    [SerializeField] public GameObject enemyPrefab;
    [SerializeField] public int enemyCount = 1000;
    [SerializeField] public bool usePooling = true;
    [SerializeField] public float minDistanceBetweenEnemies = 1.5f; // 防止过于密集
    [SerializeField] public Vector2 spawnArea = new Vector2(200f, 200f); // 生成区域（覆盖 Grid worldSize）

    [Header("Pooling 设置")]
    [SerializeField] private int poolInitialSize = 100;
    [SerializeField] private int poolMaxSize = 200000;

    private ObjectPool<EnemyPoolable> pool;

    private void Start()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("[EnemySpawner] enemyPrefab 未赋值！");
            return;
        }

        // 强制 Tag
        enemyPrefab.tag = "Enemy";

        if (usePooling)
        {
            InitializePool();
            StartCoroutine(SpawnWithPooling());
        }
        else
        {
            StartCoroutine(SpawnWithoutPooling());
        }
    }

    private void InitializePool()
    {
        pool = new ObjectPool<EnemyPoolable>(
            createFunc: () =>
            {
                GameObject go = Instantiate(enemyPrefab);
                var poolable = go.GetComponent<EnemyPoolable>();
                if (poolable == null)
                {
                    poolable = go.AddComponent<EnemyPoolable>();
                }
                poolable.SetOwningPool(pool);
                // 确保有 EnemyIndexer
                if (go.GetComponent<EnemyIndexer>() == null)
                {
                    go.AddComponent<EnemyIndexer>();
                }
                go.SetActive(false);
                return poolable;
            },
            onGet: poolable => poolable.OnSpawned(Vector3.zero),           // 这里 position 后面会覆盖
            onRelease: poolable => poolable.OnDespawned(),
            onDestroyPoolObject: poolable => Destroy(poolable.gameObject),
            collectionCheck: true,
            defaultCapacity: poolInitialSize,
            maxSize: poolMaxSize
        );

        Debug.Log($"[EnemySpawner] 对象池初始化完成，容量：{poolInitialSize} ~ {poolMaxSize}");
    }

    private IEnumerator SpawnWithPooling()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            if (i % 2000 == 0 && i > 0) yield return null; // 每2000个让出一帧

            Vector3 pos = GetValidSpawnPosition();
            var pooledEnemy = pool.Get();
            pooledEnemy.transform.position = pos;
            pooledEnemy.OnSpawned(pos);  // 触发重置逻辑

            // 确保被 Grid 索引（如果 EnemyIndexer 的 Start 没执行）
            var indexer = pooledEnemy.GetComponent<EnemyIndexer>();
            if (indexer != null)
            {
                GridManager.Instance.Add(indexer);
            }
        }

        Debug.Log($"[EnemySpawner] 使用对象池生成 {enemyCount} 个敌人完成");
    }

    private IEnumerator SpawnWithoutPooling()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            if (i % 1000 == 0 && i > 0) yield return null;

            Vector3 pos = GetValidSpawnPosition();
            GameObject enemy = Instantiate(enemyPrefab, pos, Quaternion.identity);

            // 确保组件存在
            if (enemy.GetComponent<EnemyIndexer>() == null)
                enemy.AddComponent<EnemyIndexer>();
        }

        Debug.Log($"[EnemySpawner] 普通 Instantiate 生成 {enemyCount} 个敌人完成");
    }

    private Vector3 GetValidSpawnPosition()
    {
        Vector3 pos;
        int attempts = 0;
        const int maxAttempts = 30;

        do
        {
            pos = new Vector3(
                Random.Range(-100f, 100f),
                0f,
                Random.Range(-100f, 100f)
            );

            attempts++;
            if (attempts > maxAttempts) break; // 防止死循环

        } while (Physics.CheckSphere(pos, minDistanceBetweenEnemies, LayerMask.GetMask("Default"))); // 根据需要调整 Layer

        return pos;
    }

    private void OnDestroy()
    {
        if (pool != null)
        {
            pool.Clear();
        }
    }
}