using UnityEngine;

namespace LazyGameKit.Base.Grid
{

    public class EnemyIndexer : MonoBehaviour
    {
        [Header("Indexing State")]
        public bool isIndexed = false;

        private Vector3 positionCache; // last position

        private void Start()
        {
            GridManager.Instance.Add(this);
            positionCache = transform.position;
        }

        private void Update()
        {
            if (Vector3.Distance(transform.position, positionCache) > 0.01f)
            {
                GridManager.Instance.UpdatePositionCache(this, positionCache);
                positionCache = transform.position;
            }
        }

        private void OnDestroy()
        {
            GridManager.Instance.Remove(this);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = isIndexed ? Color.green : Color.red;
            Gizmos.DrawSphere(transform.position + Vector3.forward * 1f, 0.5f);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = isIndexed ? Color.green : Color.red;
            Gizmos.DrawWireSphere(transform.position, 2f);
            UnityEditor.Handles.Label(transform.position + Vector3.forward * 3f, isIndexed ? "已索引" : "未索引");
        }

        public void SetIndexed(bool state)
        {
            isIndexed = state;
        }
    }

}