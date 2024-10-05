using System;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
using UnityEngine;

public class Hunter : MonoBehaviour
{
    [SerializeField] private Appearer plane;
    [SerializeField] private Animator anim;
    [SerializeField] private SpeechBubble bubble;
    
    private static readonly int HopAnim = Animator.StringToHash("hop");
    private static readonly int ReadAnim = Animator.StringToHash("reading");
    private static readonly int Joy = Animator.StringToHash("joy");

    public SpeechBubble Bubble => bubble;

    public void HopAround(int count, bool win)
    {
        for (var i = 0; i < (win ? count + 1 : count); i++)
        {
            if (win && i == count - 1)
            {
                this.StartCoroutine(() => anim.SetTrigger(Joy), i * 0.8f);
                continue;
            }
            var target = i < count - 1 ? Vector3.zero.RandomOffset(0.3f) : Vector3.zero;
            this.StartCoroutine(() => HopTo(target), i * 0.8f);
        }
    }

    public void HopTo(Vector3 pos)
    {
        this.StartCoroutine(() => Tweener.MoveLocalToQuad(anim.transform, pos, 0.4f), 0.1f);
        Hop();
    }

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

    public void Read(bool state)
    {
        anim.SetBool(ReadAnim, state);
    }
}