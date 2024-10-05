using System;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
using UnityEngine;

public class Hunter : MonoBehaviour
{
    [SerializeField] private Appearer plane;
    [SerializeField] private Animator anim;
    
    private static readonly int HopAnim = Animator.StringToHash("hop");

    public void Hop()
    {
        anim.SetTrigger(HopAnim);
    }

    public void Lift(Vector3 target)
    {
        Hop();
        
        var dir = target - transform.position;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        plane.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        
        plane.Show();
    }

    public void ScaleUp(float duration)
    {
        Tweener.ScaleTo(transform, Vector3.one * 1.3f, duration, TweenEasings.QuadraticEaseInOut);
    }

    public void ScaleDown(float duration)
    {
        Tweener.ScaleTo(transform, Vector3.one, duration, TweenEasings.QuadraticEaseInOut);
        this.StartCoroutine(Hop, duration - 0.3f);
    }

    public void Land()
    {
        plane.Hide();
    }
}