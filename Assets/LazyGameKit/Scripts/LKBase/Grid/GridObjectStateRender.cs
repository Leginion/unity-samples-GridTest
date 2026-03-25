using UnityEngine;

namespace LazyGameKit.Base.Grid
{

    class GridObjectStateRender : MonoBehaviour
    {
        [SerializeField] GridObjectIndexer indexer;
        [SerializeField] SpriteRenderer sr;

        void Start()
        {
            // indexer = GetComponent<GridObjectIndexer>();
            // sr = GetComponent<SpriteRenderer>();
        }

        int tick = 0;
        bool lastState = false;

        void Update()
        {
            if (++tick % 60 == 0)
            {
                bool indexed = indexer.isIndexed;
                if (indexed != lastState)
                {
                    lastState = indexed;

                    if (indexed)
                    {
                        sr.color = Color.yellow;
                    }
                    else
                    {
                        sr.color = Color.red;
                    }
                }
            }
        }
    }

}