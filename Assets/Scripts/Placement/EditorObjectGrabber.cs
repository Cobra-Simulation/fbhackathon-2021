using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorObjectGrabber : MonoBehaviour
{
    public enum GrabbableType
    {
        PICKUP,
        MOVE,
        TURN,
    }
    [SerializeField] private GrabbableType type = GrabbableType.PICKUP;
    public GrabbableType Type { get { return type; } }
    public bool canGrab = true;
    [Tooltip("If this isn't set, it will try to get the rigidbody component from this object")]
    [SerializeField] private Rigidbody rigidBody = null;
    public Rigidbody RB { get { return rigidBody; } }

    public bool grabbed = false, isEnabled = true;
    protected Transform startParent;
    protected ControllerCollisionTrigger controller;
    protected Coroutine moveTowardsController, rotateTowardsController;

    private void Awake()
    {
        startParent = transform.parent;
        // If a rigidbody hasn't been set try to get it from this object
        if (!rigidBody)
        {
            rigidBody = GetComponent<Rigidbody>();
        }
    }
    public virtual void Update()
    {
        if (isEnabled && LevelEditorManager.Instance.IsActive)
        {
            Enable();
        }
        else if (!isEnabled && !LevelEditorManager.Instance.IsActive)
        {
            Disable();
        }
    }

    public virtual void Disable()
    {
        isEnabled = false;
    }
    public virtual void Enable()
    {
        isEnabled = true;
    }
    public virtual void ApplyForce(Vector3 force)
    {
        RB.AddForce(force);
    }
    public virtual void ApplyRotation()
    {

    }
    public virtual void PickUp(ControllerCollisionTrigger _controller)
    {
        if (canGrab)
        {
            controller = _controller;
            switch (Type)
            {
                case GrabbableType.PICKUP:
                    transform.parent = controller.transform;
                    RB.isKinematic = true;
                    break;

                case GrabbableType.MOVE:
                    if (moveTowardsController != null)
                    {
                        StopCoroutine(moveTowardsController);
                    }
                    moveTowardsController = StartCoroutine(MoveTowardsController(RB));
                    break;
                case GrabbableType.TURN:
                    if (moveTowardsController != null)
                    {
                        StopCoroutine(rotateTowardsController);
                    }
                    rotateTowardsController = StartCoroutine(RotateTowardController());
                    break;
                default:
                    break;
            }
            grabbed = true;
        }
    }

    public virtual void LetGo()
    {
        switch (Type)
        {
            case GrabbableType.PICKUP:
                transform.parent = startParent;
                RB.isKinematic = false;
                break;

            case GrabbableType.MOVE:
                StopCoroutine(moveTowardsController);
                moveTowardsController = null;
                break;
            case GrabbableType.TURN:
                StopCoroutine(rotateTowardsController);
                rotateTowardsController = null;
                break;
            default:
                break;
        }
        grabbed = false;
        controller = null;
    }
    private IEnumerator RotateTowardController()
    {
        bool rotating = true;
        while (rotating)
        {
            ApplyRotation();
            yield return null;
        }
    }
    private IEnumerator MoveTowardsController(Rigidbody rb)
    {
        bool moving = true;
        // Keep adding the pull force to the grabbable object while moveGrabbableTowardsController is true
        while (moving)
        {
            Vector3 dirToController = controller.transform.position - transform.position;
            ApplyForce(dirToController * ControllerCollisionTrigger.pullForce);

            yield return null;
        }
    }
}