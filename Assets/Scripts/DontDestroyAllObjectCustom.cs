using UnityEngine;

public class DontDestroyAllObjectCustom : MonoBehaviour
{
    [Header("Tags")] [SerializeField] private string customTag;

    private void Awake()
    {
        GameObject obj = GameObject.FindGameObjectWithTag(this.customTag);
        if (obj != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            this.gameObject.tag = this.customTag;
            DontDestroyOnLoad(this.gameObject);
        }
    }
}
