using UnityEngine;

public class ItemCollector : MonoBehaviour
{
    private int items = 0;

    // 给道具主动调用
    public void CollectItem(GameObject item)
    {
        Destroy(item);
        items++;
        Debug.Log($"Items collected: {items}");
    }
}
