using UnityEngine;
using UnityEngine.Events;

public class AnimTrigger : MonoBehaviour
{
    [SerializeField] private UnityEvent action;
    
    public void Trigger()
    {
        action.Invoke();
    }
}