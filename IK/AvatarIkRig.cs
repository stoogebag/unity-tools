using RootMotion;
using RootMotion.FinalIK;
using stoogebag.Extensions;
using UnityEngine;

public class AvatarIkRig : MonoBehaviour
{
    [SerializeField] private Animator avatar;
    [SerializeField] private Transform targetContainer;
    [SerializeField] private Transform lookTarget;
    [SerializeField] private Transform bodyTarget;
    [SerializeField] private Transform leftHandTarget;
    [SerializeField] private Transform rightHandTarget;
    [SerializeField] private Transform leftFootTarget;
    [SerializeField] private Transform rightFootTarget;
    [SerializeField] private bool bindOnAwake = true;

    private FullBodyBipedIK _fbbik;
    private LookAtIK _lookAtIK;
    private BipedReferences _references;

    public Animator Avatar => avatar;
    public FullBodyBipedIK FullBody => _fbbik;
    public LookAtIK LookAt => _lookAtIK;

    public Transform LookTarget
    {
        get => lookTarget;
        set
        {
            lookTarget = value;
            if (_lookAtIK != null) _lookAtIK.solver.target = value;
        }
    }

    public float LookWeight
    {
        get => _lookAtIK != null ? _lookAtIK.solver.IKPositionWeight : 0f;
        set
        {
            if (_lookAtIK != null) _lookAtIK.solver.IKPositionWeight = Mathf.Clamp01(value);
        }
    }

    private void Awake()
    {
        if (bindOnAwake) Bind();
    }

    [ContextMenu("Bind")]
    public void Bind()
    {
        if (!ResolveAvatar()) return;

        _fbbik = GetComponentInParent<FullBodyBipedIK>();
        if (_fbbik == null && avatar != null) _fbbik = avatar.GetComponent<FullBodyBipedIK>();
        if (_fbbik == null) _fbbik = gameObject.AddComponent<FullBodyBipedIK>();

        _lookAtIK = GetComponentInParent<LookAtIK>();
        if (_lookAtIK == null && avatar != null) _lookAtIK = avatar.GetComponent<LookAtIK>();
        if (_lookAtIK == null) _lookAtIK = gameObject.AddComponent<LookAtIK>();

        _references = new BipedReferences();
        if (!BipedReferences.AutoDetectReferences(ref _references, avatar.transform, new BipedReferences.AutoDetectParams(true, true)))
        {
            Debug.LogError($"AvatarIkRig: failed to auto-detect a biped on '{avatar.name}'.", this);
            return;
        }

        _fbbik.SetReferences(_references, IKSolverFullBodyBiped.DetectRootNodeBone(_references));
        _fbbik.enabled = true;

        var container = targetContainer != null ? targetContainer : transform;

        bodyTarget = EnsureTarget(bodyTarget, container, "body", _references.pelvis);
        leftHandTarget = EnsureTarget(leftHandTarget, container, "leftHand", _references.leftHand);
        rightHandTarget = EnsureTarget(rightHandTarget, container, "rightHand", _references.rightHand);
        leftFootTarget = EnsureTarget(leftFootTarget, container, "leftFoot", _references.leftFoot);
        rightFootTarget = EnsureTarget(rightFootTarget, container, "rightFoot", _references.rightFoot);
        lookTarget = EnsureTarget(lookTarget, container, "look", _references.head);

        _fbbik.solver.bodyEffector.target = bodyTarget;
        _fbbik.solver.leftHandEffector.target = leftHandTarget;
        _fbbik.solver.rightHandEffector.target = rightHandTarget;
        _fbbik.solver.leftFootEffector.target = leftFootTarget;
        _fbbik.solver.rightFootEffector.target = rightFootTarget;

        _lookAtIK.solver.target = lookTarget;
        _lookAtIK.solver.SetChain(
            _references.spine ?? new Transform[0],
            _references.head,
            _references.eyes ?? new Transform[0],
            _references.root);
    }

    private bool ResolveAvatar()
    {
        if (avatar != null) return true;

        avatar = GetComponentInParent<Animator>();
        if (avatar == null) avatar = GetComponentInChildren<Animator>();
        if (avatar == null && transform.parent != null) avatar = transform.parent.GetComponentInChildren<Animator>();
        if (avatar == null) avatar = transform.root.GetComponentInChildren<Animator>();

        if (avatar == null)
            Debug.LogError("AvatarIkRig: no Animator/Avatar found on this object, a parent, or a child.", this);

        return avatar != null;
    }

    private static Transform EnsureTarget(Transform existing, Transform container, string name, Transform bone)
    {
        if (existing != null) return existing;

        var existingChild = container.Find(name);
        if (existingChild != null) return existingChild;

        var target = new GameObject(name).transform;
        target.SetParent(container, false);
        if (bone != null) target.SetPositionAndRotation(bone.position, bone.rotation);
        return target;
    }

    private void OnDrawGizmosSelected()
    {
        if (lookTarget != null) lookTarget.gameObject.DrawSphere();
        if (bodyTarget != null) bodyTarget.gameObject.DrawSphere();
        if (leftHandTarget != null) leftHandTarget.gameObject.DrawSphere();
        if (rightHandTarget != null) rightHandTarget.gameObject.DrawSphere();
        if (leftFootTarget != null) leftFootTarget.gameObject.DrawSphere();
        if (rightFootTarget != null) rightFootTarget.gameObject.DrawSphere();
    }
}
