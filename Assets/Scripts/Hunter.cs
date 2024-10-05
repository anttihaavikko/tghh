using System;
using AnttiStarterKit.Animations;
using UnityEngine;

public class Hunter : MonoBehaviour
{
    [SerializeField] private Appearer plane;
    [SerializeField] private Appearer dude;

    public void Lift(Vector3 target)
    {
        var dir = target - transform.position;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        plane.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        
        // dude.Hide();
        plane.Show();
    }

    public void ScaleUp(float duration)
    {
        Tweener.ScaleTo(transform, Vector3.one * 1.3f, duration, TweenEasings.QuadraticEaseInOut);
    }

    public void ScaleDown(float duration)
    {
        Tweener.ScaleTo(transform, Vector3.one, duration, TweenEasings.QuadraticEaseInOut);
    }

    public void Land()
    {
        plane.Hide();
        // dude.Show();
    }
}