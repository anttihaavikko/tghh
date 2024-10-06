using AnttiStarterKit.Managers;
using UnityEngine;
using UnityEngine.Events;

public class AnimTrigger : MonoBehaviour
{
    [SerializeField] private UnityEvent action;
    
    public void Trigger()
    {
        action.Invoke();
    }

    public void Land()
    {
        AudioManager.Instance.PlayEffectAt(4, transform.position, 0.5f);
    }
}