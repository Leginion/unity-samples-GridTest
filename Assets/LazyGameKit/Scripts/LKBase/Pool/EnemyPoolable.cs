using UnityEngine;

using LazyGameKit.Base.Grid;

namespace LazyGameKit.Base.Pool
{
    /// <summary>
    /// use to mark Enemy Object is owning by ObjectPool.
    /// could provide extra actions for OnSpawned/OnDespawned.
    /// </summary>
    public class EnemyPoolable : MonoBehaviour
    {
        private ObjectPool<EnemyPoolable> owningPool;

        public void SetOwningPool(ObjectPool<EnemyPoolable> pool)
        {
            owningPool = pool;
        }

        public void OnSpawned(Vector3 position)
        {
            transform.position = position;
            transform.rotation = Quaternion.identity;
            gameObject.SetActive(true);

            // TODO -> to Generic
            // 示例：重置组件状态
            var indexer = GetComponent<EnemyIndexer>();
            if (indexer != null)
            {
                indexer.isIndexed = false;
                // 如果有其他状态，也在这里重置
            }
        }

        public void OnDespawned()
        {
            gameObject.SetActive(false);
            // 可在这里清理引用、粒子等
        }

        public void ReturnToPool()
        {
            if (owningPool != null)
            {
                owningPool.Release(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
