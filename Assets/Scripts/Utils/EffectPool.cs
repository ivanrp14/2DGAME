using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour, IPoolable
{
    public void OnReturnToPool()
    {
        throw new System.NotImplementedException();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnDisable()
    {
        PoolManager.Instance.Return(gameObject);
    }
}
