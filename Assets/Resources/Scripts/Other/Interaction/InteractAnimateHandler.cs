using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;


public class InteractAnimateHandler : MonoBehaviour, IPaused
{
    [SerializeField] protected Animator _animator;
    [SerializeField] private RuntimeAnimatorController _controller;
    [SerializeField] private float _blendDuration = 0.2f;

    private Action _endCallback;
    private PlayableGraph _playableGraph;
    private AnimatorControllerPlayable _animatorPlayable;
    private AnimationMixerPlayable _topLevelMixer;

    private AnimationMixerPlayable _multipleAnimMixer;
    private AnimationMixerPlayable _abortAnimMixer;
    private AnimatorUpdateMode _updateModeBuffer;

    private bool _animationInProcessing = false;
    private bool _isPaused;


    private void Start()
    {
        _playableGraph = PlayableGraph.Create(transform.gameObject.name);

        AnimationPlayableOutput playableOutput = AnimationPlayableOutput.Create(_playableGraph, "Animation", _animator);
        _topLevelMixer = AnimationMixerPlayable.Create(_playableGraph, 3);
        playableOutput.SetSourcePlayable(_topLevelMixer);

        _animatorPlayable = AnimatorControllerPlayable.Create(_playableGraph, _controller);
        _topLevelMixer.ConnectInput(0, _animatorPlayable, 0);

        _playableGraph.GetRootPlayable(0).SetInputWeight(0, 1f);
        _playableGraph.Play();
        GraphVisualizerClient.Show(_playableGraph);
    }


    private IEnumerator Blend(float duration, Action<float> blendCallback, float delay = 0f, Action callback = null)
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        yield return new WaitUntil(() => !_isPaused);

        float blendTime = 0f;
        while (blendTime < 1f)
        {
            yield return new WaitUntil(() => !_isPaused);

            blendTime += Time.deltaTime / duration;
            blendCallback(blendTime);
            yield return blendTime;
        }

        blendCallback(1f);

        callback?.Invoke();
    }


    private void MakeBlend(AnimationMixerPlayable mixer, int indexFrom, int indexTo,float weightEnd, float duration, float delay = 0, Action callback = null)
    {
        float weightStart = weightEnd > 0 ? 0 : 1f;

        StartCoroutine(Blend(duration, (blendTime) => {
            float weight = Mathf.Lerp(weightStart, weightEnd, blendTime);
            mixer.SetInputWeight(indexFrom, weight);
            mixer.SetInputWeight(indexTo, 1f - weight);
        }, delay, callback));
    }


    private void DisconnectClips(int inputIndex)
    {
        AnimationMixerPlayable mixer = (AnimationMixerPlayable)_topLevelMixer.GetInput(inputIndex);
        int inpitCount = mixer.GetInputCount();

        for (int i = inpitCount - 1; i >= 0; i--)
        {
            Playable clip = mixer.GetInput(i);
            mixer.DisconnectInput(i);
            _playableGraph.DestroyPlayable(clip);
        }

        _topLevelMixer.DisconnectInput(inputIndex);
        _playableGraph.DestroyPlayable(mixer);
    }


    private float CallculateBlendDuration(InteractClip interaction)
    {
        return interaction.NeedDinamicBlend ?
                Mathf.Clamp(interaction.Clip.length * 0.2f, 0.1f, interaction.Clip.length * 0.5f) :
                _blendDuration;
    }


    private void AddClipOnMixer(InteractClip clip, AnimationMixerPlayable mixer)
    {
        int inputIndex = mixer.GetInputCount();
        AnimationClipPlayable oneShot = AnimationClipPlayable.Create(_playableGraph, clip.Clip);

        mixer.SetInputCount(inputIndex + 1);
        mixer.ConnectInput(inputIndex, oneShot, 0);
        mixer.SetInputWeight(inputIndex, 0f);
    }


    private IEnumerator PlaySequance(AnimationMixerPlayable mixer, int inputIndexFrom, int inputIndexTo, InteractClip[] clips, bool automaticEndOfAnimation = true, Action callback = null)
    {
        _animationInProcessing = true;
        float blendDuration = CallculateBlendDuration(clips[0]);
        float outDelay = clips[0].Clip.length - blendDuration * 2;

        AddClipOnMixer(clips[0], mixer);
        mixer.SetInputWeight(0, 1f);
        MakeBlend(_topLevelMixer, inputIndexFrom, inputIndexTo, 0f, blendDuration);

        for (int i = 1; i < clips.Length; i++)
        {
            blendDuration = CallculateBlendDuration(clips[i]);

            float t = 0f;
            while (t < outDelay)
            {
                yield return new WaitUntil(() => !_isPaused);
                t += Time.deltaTime;
                yield return null;
            }

            AddClipOnMixer(clips[i], mixer);
            MakeBlend(mixer, i - 1, i, 0f, blendDuration);

            outDelay = clips[i].Clip.length - blendDuration * 2;
        }

        if (automaticEndOfAnimation)
        {
            MakeBlend(_topLevelMixer, 0, inputIndexTo, 1f, blendDuration, outDelay, callback);
        }
    }


    private void SetAnimatorUpdateMode()
    {
        if (_animationInProcessing == false)
            _updateModeBuffer = _animator.updateMode;

        _animator.updateMode = AnimatorUpdateMode.Normal;
    }


    private void PrepareToBlend()
    {
        SetAnimatorUpdateMode();

        if (_animationInProcessing == false)
            return;

        StopAllCoroutines();
    }


    private void ConnectMixer(int inputIndex, ref AnimationMixerPlayable mixer, int previousInputIndex)
    {
        mixer = AnimationMixerPlayable.Create(_playableGraph, 0);
        _topLevelMixer.ConnectInput(inputIndex, mixer, 0);
        _topLevelMixer.SetInputWeight(previousInputIndex, 1f);
    }



    private void PlayClips(InteractClip[] clips, int inputIndex, ref AnimationMixerPlayable mixer, int previousInputIndex, bool disconnectPrevious, bool automaticEndOfAnimation, Action callback = null)
    {
        PrepareToBlend();
        ConnectMixer(inputIndex, ref mixer, previousInputIndex);

        _endCallback = () =>
        {
            DisconnectClips(inputIndex);

            if(disconnectPrevious)
                DisconnectClips(previousInputIndex);

            _animator.updateMode = _updateModeBuffer;
            _animationInProcessing = false;
            callback.Invoke();
        };

        StartCoroutine(PlaySequance(mixer, previousInputIndex, inputIndex, clips, automaticEndOfAnimation, _endCallback));
    }


    public void PlayInteractClips(InteractClip[] clips, bool automaticEndOfAnimation = true, Action callback = null)
    {
        if(_animationInProcessing == false)
            PlayClips(clips, 1, ref _multipleAnimMixer, 0, false, automaticEndOfAnimation, callback);
    }


    public void PlayAbortInteract(InteractClip[] abortClips, Action callback = null)
    {
        if (_animationInProcessing)
            PlayClips(abortClips, 2, ref _abortAnimMixer, 1, true, true, callback);
    }


    public void Pause()
    {
        _isPaused = true;

        if(_topLevelMixer.IsNull() == false)
            _topLevelMixer.Pause();
    }


    public void Resume()
    {
        _isPaused = false;

        if (_topLevelMixer.IsNull() == false)
            _topLevelMixer.Play();
    }


    private void OnDestroy()
    {
        if (_playableGraph.IsValid())
            _playableGraph.Destroy();
    }
}