#if UNITASK
#if CINEMACHINE
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// A door that animates open and shut. Pure motion — it has no interaction of
/// its own. Something else (an <see cref="Interactable"/> offering an
/// <see cref="IInteraction"/>, or a trigger) calls <see cref="Open"/>,
/// <see cref="Close"/> or <see cref="Toggle"/>.
/// </summary>
public class Door : MonoBehaviour
{
    [SerializeField] private GameObject _leaf;
    [SerializeField] private float _openTime = 0.5f;
    [SerializeField] private float _closeTime = 0.3f;
    [SerializeField] private Vector3 _openEuler = new(0f, 95f, 0f);

    private bool _isOpen;
    private bool _inMotion;

    public bool IsOpen => _isOpen;
    public bool IsInMotion => _inMotion;
    public bool BlocksMovement => !_isOpen;

    public bool CanOperate(IInteractor interactor) => !_inMotion;

    public void Toggle()
    {
        if (_isOpen) Close();
        else Open();
    }

    public async void Open()
    {
        if (_isOpen || _inMotion || _leaf == null)
            return;

        _isOpen = true;
        _inMotion = true;
        await _leaf.transform.DOLocalRotate(_openEuler, _openTime).ToUniTask();
        _inMotion = false;
    }

    public async void Close()
    {
        if (!_isOpen || _inMotion || _leaf == null)
            return;

        _isOpen = false;
        _inMotion = true;
        await _leaf.transform.DOLocalRotate(Vector3.zero, _closeTime).ToUniTask();
        _inMotion = false;
    }
}
#endif
#endif
