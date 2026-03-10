using UnityEngine;
using System.Collections;

using LazyGameKit.Base.Pool;
using LazyGameKit.Base.SpawnBounds;

namespace LazyGameKit.Game
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Generation Setting")]
        public int enemyCount = 1000;

        [Header("RectBounds Setting")]
        [SerializeField] private RectBounds rectBounds;

        private void Start()
        {
            StartCoroutine(Spawn());
        }

        private IEnumerator Spawn()
        {
            var pool = PoolManager.Instance.GetEnemyPool();

            for (int i = 0; i < enemyCount; i++)
            {
                if (i % 2000 == 0 && i > 0) yield return null;

                Vector3 pos = rectBounds.GetValidPosition();
                var pooled = pool.Get();

                pooled.transform.position = pos;
                pooled.OnSpawned(pos);
            }

            Debug.Log($"[EnemySpawner] 生成 {enemyCount} 个敌人完成");
        }
    }

}