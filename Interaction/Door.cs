#if UNITASK
#if CINEMACHINE
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// A door that moves between two authoring poses. Pure motion — it has no
/// interaction of its own; something else (an <see cref="Interactable"/> offering
/// an <see cref="IInteraction"/>, or a trigger) calls <see cref="Open"/>,
/// <see cref="Close"/> or <see cref="Toggle"/>.
///
/// Closed and open are marker transforms. The door snaps to one on Awake, then
/// tweens position and rotation between them, so where the prefab is authored
/// does not matter. A hinge is the case where the poses share a position
/// (rotation only); a sliding door is where they share a rotation.
/// </summary>
public class Door : MonoBehaviour
{
    [SerializeField] private Transform _closedPose;
    [SerializeField] private Transform _openPose;
    [SerializeField] private bool _startOpen;
    [SerializeField] private float _openTime = 0.5f;
    [SerializeField] private float _closeTime = 0.3f;
    [SerializeField] private AnimationCurve _ease = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private bool _isOpen;
    private bool _inMotion;

    public bool IsOpen => _isOpen;
    public bool IsLocked { get; private set; }
    public bool IsInMotion => _inMotion;
    public bool BlocksMovement => !_isOpen;

    public bool CanOperate(IInteractor interactor) => !_inMotion && !IsLocked;

    public void SetLocked(bool locked) => IsLocked = locked;

    private void Awake()
    {
        _isOpen = _startOpen;

        var pose = _isOpen ? _openPose : _closedPose;
        if (pose != null)
        {
            transform.localPosition = pose.localPosition;
            transform.localRotation = pose.localRotation;
        }
    }

    public void Toggle()
    {
        if (_isOpen) Close();
        else Open();
    }

    public async void Open()
    {
        if (_isOpen || _inMotion || _openPose == null || IsLocked)
            return;

        _isOpen = true;
        _inMotion = true;

        await TweenTo(_openPose, _openTime);

        _inMotion = false;
    }

    public async void Close()
    {
        if (!_isOpen || _inMotion || _closedPose == null)
            return;

        _isOpen = false;
        _inMotion = true;

        await TweenTo(_closedPose, _closeTime);

        _inMotion = false;
    }

    private async UniTask TweenTo(Transform pose, float time)
    {
        var sequence = DOTween.Sequence();
        sequence.Join(transform.DOLocalMove(pose.localPosition, time).SetEase(_ease));
        sequence.Join(transform.DOLocalRotateQuaternion(pose.localRotation, time).SetEase(_ease));
        await sequence.ToUniTask();
    }
}
#endif
#endif
