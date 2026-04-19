
using UnityEngine;

public class LoaderOverlay_unityAnimation : MonoBehaviour, ILoaderOverlay
{
    public Animator animator;
    public string animationName_begin;
    public string animationName_idle;
    public string animationName_end;
    public float animationDuration_begin;
    public float animationDuration_end;
    
    public float PlayAnim_begin()
    {
        animator.Play(animationName_begin);
        return animationDuration_begin;
    }

    public void PlayAnim_idle()
    {
        animator.Play(animationName_idle);
    }

    public float PlayAnim_end()
    {
        animator.Play(animationName_end);
        return animationDuration_end;
    }
}
