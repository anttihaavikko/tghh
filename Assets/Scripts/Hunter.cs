using System;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Utils;
using AnttiStarterKit.Visuals;
using UnityEngine;

public class Hunter : MonoBehaviour
{
    [SerializeField] private Appearer plane;
    [SerializeField] private Animator anim;
    [SerializeField] private SpeechBubble bubble;
    [SerializeField] private ParticleSystem confetti;
    [SerializeField] private EffectCamera effectCamera;
    [SerializeField] private GameObject planeTrail;
    [SerializeField] private Transform mouth;
    
    private static readonly int HopAnim = Animator.StringToHash("hop");
    private static readonly int ReadAnim = Animator.StringToHash("reading");
    private static readonly int Joy = Animator.StringToHash("joy");
    
    private Coroutine mouthOpening;

    public SpeechBubble Bubble => bubble;

    private void Start()
    {
        bubble.onWord += OpenMouth;
    }

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

    private void OpenMouth()
    {
        OpenMouth(1f);
    }

    private void OpenMouth(float duration)
    {
        if(mouthOpening != default) StopCoroutine(mouthOpening);
        Tweener.ScaleToBounceOut(mouth, Vector3.one, 0.075f);
        mouthOpening = this.StartCoroutine(() => Tweener.ScaleToBounceOut(mouth, new Vector3(1, 0, 1), 0.1f), 0.1f * duration);
    }

    private void Update()
    {
        if (DevKey.Down(KeyCode.W)) anim.SetTrigger(Joy);
    }

    public void ShootConfetti()
    {
        OpenMouth(3f);
        effectCamera.BaseEffect(0.3f);
        confetti.Play();
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
        planeTrail.SetActive(true);
        
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
        this.StartCoroutine(() => planeTrail.SetActive(false), 3f);
    }

    public void Read(bool state)
    {
        anim.SetBool(ReadAnim, state);
    }
}