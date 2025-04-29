using UnityEngine;

public class ReadyButton : MonoBehaviour
{
    public static ReadyButton instance;

    private void Awake()
    {
        instance = this;
    }
}
