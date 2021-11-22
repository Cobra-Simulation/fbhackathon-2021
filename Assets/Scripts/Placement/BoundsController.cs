using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using FireTypes;
using TMPro;


#region SPAWNING ENUMS

[System.Serializable]
public enum SpawnMode
{
    None,
    Bounds,
    Wall,
    Item,
    Fire,
    Occluder,
    SmokeHeight,
    Reposition
}
[System.Serializable]
public enum ItemType
{
    None,
    Pan,
    Log,
    Phone
}

public enum LockPositionType
{
    None,
    X,
    Y,
    Z
}

#endregion


public class BoundsController : MonoBehaviour
{
    
    #region VARIABLES

    // VR controller reference
    OVRInput.Controller controller = OVRInput.Controller.RTouch;
    OVRInput.Controller controllerAlt = OVRInput.Controller.LTouch;
    private Vector3 controllerDirection = Vector3.zero;

    #region GRAB SETTINGS
    // all-purpose timer to use for blending after object is grabbed/released
    float grabTime = 0.0f;
    // the grabbed object's transform relative to the controller
    Vector3 localGrabOffset = Vector3.zero;
    Quaternion localGrabRotation = Quaternion.identity;
    // the camera and grabbing hand's world position when grabbing
    Vector3 camGrabPosition = Vector3.zero;
    Quaternion camGrabRotation = Quaternion.identity;
    Vector3 handGrabPosition = Vector3.zero;
    Quaternion handGrabRotation = Quaternion.identity;
    Vector3 cursorPosition = Vector3.zero;
    float rotationOffset = 0.0f;
    public Transform objectInfo;
    #endregion

    [Header("Bounds Prefabs")]
    public GameObject boundsPrefab;

    #region HOLOGRAM
    private bool isTriggerDown = false;
    private GameObject currentBoundsBox = null;
    private Vector3 startPosition = Vector3.zero;
    private bool isHologramShowing = false;
    private GameObject hologram;
    #endregion

    #region CACHED VARIABLES
    private Vector3 controllerPos;
    private Quaternion controllerRot;
    private Vector3 controllerOffset;
    #endregion

    #region USER TOOLTIPS
    [Header("Hologram preview")]
    public GameObject hologramPrefab;
    public float controllerOffsetAmount = 0.05f;

    [Header("Holograms")]
    public Material previewSelect;
    public Material wirePreviewSelect;
    private Material currentMaterialHolder;
    private Material currentMaterialInstance;
    private GameObject currentObjectHolder;
    #endregion

    [Header("Type")]
    private SpawnMode activeSpawnMode = SpawnMode.None;
    private ItemType activeItemSpawn = ItemType.None;

    // Lock position moving type
    private LockPositionType lockType = LockPositionType.None;
    private float forwardMoveAmount = 0f;
    private float yRotAmount = 0.0f;
    public float rotationSpeed = 90.0f;

    // TEMP
    private FireSource fireSource = FireSource.Generic;
    private Dictionary<GameObject, FireSource> fireDict = new Dictionary<GameObject, FireSource>();

    public GameObject firePrefab;
    private bool isLevelEditorOpen = false;

    [Header("Bound Materials")]
    public Material defaultMaterial;
    public Material occlusionMaskedMaterial;
   
    private GameObject itemPrefab = null;

    #region UI MANAGMENT
    [Header("UI Management")]
    public TMP_Text helpInfoHolder;
    public GameObject helpInfoGameObject;
    public Image xIcon;
    public Image yIcon;
    public Image zIcon;
    public Image xyzIcon;
    public AudioSource uiSoundPlayer;
    #endregion

    #endregion


    private void Start()
    {
        hologram = Instantiate(hologramPrefab, Vector3.zero, Quaternion.identity);
        hologram.transform.position = controllerPos + controllerOffset;
        hologram.SetActive(false);
        isHologramShowing = false;

        activeItemSpawn = ItemType.None;
        activeSpawnMode = SpawnMode.None;
        itemPrefab = null;

        // Subscribe to level editor enabling events
        LevelEditorManager.OnEnabledEvent += StartLevelEditor;
        LevelEditorManager.OnDisabledEvent += CloseLevelEditor;
    }
    private void OnDestroy()
    {
        ResetAll();
        LevelEditorManager.OnEnabledEvent -=  StartLevelEditor;
        LevelEditorManager.OnDisabledEvent -=  CloseLevelEditor;
    }
    private void Update()
    {
        // Only enable editing when the level editor is open
        if (isLevelEditorOpen)
        {
            UpdateControllerData();
            DisplayHologram();

            // Decides what the spawner mode is and handles object placement and deletion
            ManageObjectsAndBounds();
        }
    }
    private void StartLevelEditor()
    {      
        // Convert all bound materials to a default visible one
        GameObject[] bounds = GameObject.FindGameObjectsWithTag("Bounds");
        for (int i = 0; i < bounds.Length; i++)
        {
            bounds[i].GetComponent<MeshRenderer>().material = defaultMaterial;
        }

        // Remove all active fires
        FireManager.Instance.Reset();
        SetActiveLockType();

        isLevelEditorOpen = true;
    }
    private void CloseLevelEditor()
    {
        // Spawn fire instance at each placement then remove the placement
        for (int i = 0; i < fireDict.Count; i++)
        {
            GameObject holder = Instantiate(firePrefab, fireDict.ElementAt(i).Key.transform.position, fireDict.ElementAt(i).Key.transform.rotation);
            holder.transform.localScale = fireDict.ElementAt(i).Key.transform.localScale * 1.25f;
            FireManager.Instance.SpawnFire(
                new Fire(
                    holder,
                    0.1f,
                    100f,
                    200f,
                    fireDict.ElementAt(i).Value));

            holder = null;
            DestroyImmediate(fireDict.ElementAt(i).Key);
        }

        // Convert each bound into an occlusion mask
        GameObject[] bounds = GameObject.FindGameObjectsWithTag("Bounds");
        for (int i = 0; i < bounds.Length; i++)
        {
            bounds[i].GetComponent<MeshRenderer>().material = occlusionMaskedMaterial;
        }

        ResetAll();
        fireDict.Clear();
        isLevelEditorOpen = false;
    }


    #region UI TRIGGERS
    public void SetItemPrefab(GameObject asset)
    {
        itemPrefab = asset;
        QuickMenuManager.Instance.HideMenu();
    }

    public void SetFireSource(int fireSourceIndex)
    {
        fireSource = (FireSource)fireSourceIndex;
    }

    public void SetPosLockType(int lockIndex)
    {
        lockType = (LockPositionType)lockIndex;
        SetActiveLockType();
    }

    public void SetHelpText(string helpText)
    {
        helpInfoGameObject.SetActive(true);
        helpInfoHolder.text = helpText;
    }

    private void SetActiveLockType()
    {
        switch (lockType)
        {
            case LockPositionType.None:
                xyzIcon.color = Color.blue;
                xIcon.color = Color.white;
                yIcon.color = Color.white;
                zIcon.color = Color.white;
                break;
            case LockPositionType.X:
                xyzIcon.color = Color.white;
                xIcon.color = Color.blue;
                yIcon.color = Color.white;
                zIcon.color = Color.white;
                break;
            case LockPositionType.Y:
                xyzIcon.color = Color.white;
                xIcon.color = Color.white;
                yIcon.color = Color.blue;
                zIcon.color = Color.white;
                break;
            case LockPositionType.Z:
                xyzIcon.color = Color.white;
                xIcon.color = Color.white;
                yIcon.color = Color.white;
                zIcon.color = Color.blue;
                break;
            default:
                xyzIcon.color = Color.blue;
                xIcon.color = Color.white;
                yIcon.color = Color.white;
                zIcon.color = Color.white;
                break;
        }
    }

    public void ClearWorld() 
    {
        fireDict.Clear();
        FireManager.Instance.Reset();
        

        GameObject[] bounds = GameObject.FindGameObjectsWithTag("Bounds");
        for (int i = 0; i < bounds.Length; i++)
        {
            DestroyImmediate(bounds[i].gameObject);
        }
        BoundsManager.Instance.Reset();

        GameObject[] objs = GameObject.FindGameObjectsWithTag("Object");
        for (int i = 0; i < objs.Length; i++)
        {
            DestroyImmediate(objs[i]);
        }


    }

    public void PlaySound(AudioClip audio)
    {
        uiSoundPlayer.clip = audio;
        uiSoundPlayer.Play();
    }
    #endregion

    // Manages which objects or bounds are spawnable and deletable
    private void ManageObjectsAndBounds()
    {
        if (!QuickMenuManager.Instance.IsMenuActive)
        {
            switch (activeSpawnMode)
            {
                case SpawnMode.None:
                    ResetAll();
                    break;
                case SpawnMode.Bounds:
                    SpawnBoxBounds();
                    RemoveBounds();
                    break;
                case SpawnMode.Wall:
                    SpawnWallBounds();
                    RemoveBounds();
                    break;
                case SpawnMode.Item:
                    SpawnItem(itemPrefab);
                    RepositionWithJoyStick();
                    RemoveItem();
                    break;
                case SpawnMode.Fire:
                    SpawnFire(itemPrefab);
                    break;
                case SpawnMode.Occluder:
                    break;
                case SpawnMode.SmokeHeight:
                    CheckSmokeHeight();
                    break;
                case SpawnMode.Reposition:
                    GrabBounds();
                    RepositionWithJoyStick();
                    break;
                default:
                    break;
            }
        }
        else
        {
            if (currentBoundsBox != null)
            {
                ResetAll();
            }            
        }
    }

    #region USER TOOLTIPS
    // Displays a hologram cursor above the user's controller
    private void DisplayHologram()
    {
        // Spawn a hologram cube when sides are held
        if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, controller))
        {
            if (!isHologramShowing)
            {
                hologram.SetActive(true);
                isHologramShowing = true;
            }

            //HighlightBounds(controllerPos + controllerOffset);
        }
        else
        {
            if (isHologramShowing)
            {
                hologram.SetActive(false);
                isHologramShowing = false;
                ResetHighlight();
            }
        }

        // Update hologram position
        if (hologram.activeInHierarchy)
        {
            hologram.transform.position = controllerPos + controllerOffset;
            hologram.transform.rotation = controllerRot;
        }
    }

    // Change the material of "highlighted" objects/bounds when the controller is inside them
    private void HighlightBounds(Vector3 controllerPosition)
    {
        // Check if controller position is still in the same bounds
        if (currentObjectHolder != null)
        {
            // Check if controller isn't inside the bounds
            if (currentObjectHolder.GetComponent<Collider>().bounds.Contains(controllerPosition))
            {
                return;
            }
        }

        // Get all placeable items in the scene
        currentMaterialHolder = currentMaterialInstance;
        GameObject[] bounds = GameObject.FindGameObjectsWithTag("Bounds");
        GameObject[] items = GameObject.FindGameObjectsWithTag("Items");
        GameObject[] highlightObjects = bounds.Union(items).ToArray();

        for (int i = 0; i < highlightObjects.Length; i++)
        {
            if (highlightObjects[i].GetComponent<Collider>().bounds.Contains(controllerPosition))
            {
                currentObjectHolder = highlightObjects[i];
                currentMaterialHolder = highlightObjects[i].GetComponent<MeshRenderer>().material;
                currentMaterialInstance = Instantiate(currentMaterialHolder);
                break;
            }
        }
    }

    #endregion

    #region SPAWNING
    // Spawn box bounds for objects
    private void SpawnBoxBounds()
    {
        // Spawn a box on trigger press and ensure player is holding the trigger
        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, controller) && isTriggerDown == false)
        {
            // Player is now holding down the trigger
            isTriggerDown = true;
            yRotAmount = 0.0f;
            forwardMoveAmount = 0.0f;

            // Spawn a bounds box instance
            currentBoundsBox = Instantiate(boundsPrefab, controllerPos + controllerOffset, controllerRot);
            startPosition = controllerPos + controllerOffset;
        }
        else if (!OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, controller) && isTriggerDown == true)
        {
            // Player is no longer holding down the trigger
            isTriggerDown = false;

            // Create no bounds via manager from the newly created bound volume 
            BoundsManager.Instance.CreateBounds(currentBoundsBox.GetComponent<Collider>());

            // Reset local bound settings
            startPosition = Vector3.zero;
            currentBoundsBox = null;
        }

        // Update the current bounds box
        if (isTriggerDown) { ResizeWithHands(controllerPos, controllerOffset); }

    }

    // Spawn wall bounds for real life walls
    private void SpawnWallBounds()
    {
        // Spawn a box on trigger press and ensure player is holding the trigger
        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, controller) && isTriggerDown == false)
        {
            // Player is now holding down the trigger
            isTriggerDown = true;
            yRotAmount = 0.0f;
            forwardMoveAmount = 0.0f;

            // Spawn a bounds box instance
            currentBoundsBox = Instantiate(boundsPrefab, controllerPos + controllerOffset, controllerRot);
            startPosition = controllerPos + controllerOffset;
        }
        else if (!OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, controller) && isTriggerDown == true)
        {
            // Player is no longer holding down the trigger
            isTriggerDown = false;

            // Create no bounds via manager from the newly created bound volume 
            BoundsManager.Instance.CreateBounds(currentBoundsBox.GetComponent<Collider>());

            // Reset local bound settings
            startPosition = Vector3.zero;
            currentBoundsBox = null;
        }

        // Update the current bounds box
        if (isTriggerDown) { ResizeWithJoyStick(); }
    }

    private void SpawnItem(GameObject itemPrefab)
    {
        // Spawn a box on trigger press and ensure player is holding the trigger
        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, controller) && isTriggerDown == false)
        {
            // Player is now holding down the trigger
            isTriggerDown = true;
            yRotAmount = 0.0f;
            forwardMoveAmount = 0.0f;

            // Spawn a bounds box instance
            currentBoundsBox = Instantiate(itemPrefab, controllerPos + controllerOffset, Quaternion.identity);
            startPosition = controllerPos + controllerOffset;
        }
        else if (!OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, controller) && isTriggerDown == true)
        {
            // Player is no longer holding down the trigger
            isTriggerDown = false;

            // Reset local bound settings
            startPosition = Vector3.zero;
            currentBoundsBox = null;
            forwardMoveAmount = 0.0f;
        }

        // Update the current bounds box
        if (isTriggerDown) { MoveObjectWithHands(); }
    }

    private void SpawnFire(GameObject itemPrefab)
    {
        // Spawn a box on trigger press and ensure player is holding the trigger
        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, controller) && isTriggerDown == false)
        {
            // Player is now holding down the trigger
            isTriggerDown = true;
            yRotAmount = 0.0f;
            forwardMoveAmount = 0.0f;

            // Spawn a bounds box instance
            currentBoundsBox = Instantiate(itemPrefab, controllerPos + controllerOffset, Quaternion.identity);
            startPosition = controllerPos + controllerOffset;
            fireDict.Add(currentBoundsBox, fireSource);
        }
        else if (!OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, controller) && isTriggerDown == true)
        {
            // Player is no longer holding down the trigger
            isTriggerDown = false;

            // Reset local bound settings
            startPosition = Vector3.zero;
            currentBoundsBox = null;
        }

        // Update the current bounds box
        if (isTriggerDown) { MoveObjectWithHands(); ResizeWithJoyStick(); }
    }

    private void CheckSmokeHeight()
    {
        // Spawn a box on trigger press and ensure player is holding the trigger
        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, controller) && isTriggerDown == false)
        {
            // Player is now holding down the trigger
            isTriggerDown = true;
            yRotAmount = 0.0f;
            forwardMoveAmount = 0.0f;
        }
        else if (!OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, controller) && isTriggerDown == true)
        {
            // Player is no longer holding down the trigger
            isTriggerDown = false;
            BoundsManager.Instance.SetSmokeHeight(controllerPos.y);
        }
    }
    #endregion

    #region OBJECT/BOUND REMOVAL
    // Remove bound's from a controller's position
    private void RemoveBounds()
    {
        // Check if controller is currently in a bound's location
        if (OVRInput.Get(OVRInput.Button.One, controller) && !isTriggerDown)
        {
            // Check if controller is in bounds of a box bounds
            GameObject[] bounds = GameObject.FindGameObjectsWithTag("Bounds");
            for (int i = 0; i < bounds.Length; i++)
            {
                if (bounds[i].GetComponent<Collider>().bounds.Contains(controllerPos + controllerOffset))
                {
                    BoundsManager.Instance.RemoveBounds(bounds[i].GetComponent<Collider>());
                    DestroyImmediate(bounds[i].gameObject);
                    break;
                }
            }
        }
    }

    private void GrabBounds()
    {
        // Check if controller is currently in a bound's location
        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, controller) && !isTriggerDown && activeSpawnMode == SpawnMode.Reposition)
        {
            isTriggerDown = true;
            ResetAll();

            // Check if controller is in bounds of a fire
            GameObject[] fireBounds = GameObject.FindGameObjectsWithTag("Fire");
            for (int i = 0; i < fireBounds.Length; i++)
            {
                if (fireBounds[i].GetComponent<Collider>().bounds.Contains(controllerPos + controllerOffset))
                {
                    currentBoundsBox = fireBounds[i];
                    startPosition = fireBounds[i].gameObject.transform.position;
                    return;
                }
            }

            // Check if controller is in bounds of a box/wall
            GameObject[] bounds = GameObject.FindGameObjectsWithTag("Bounds");        
            for (int i = 0; i < bounds.Length; i++)
            {
                if (bounds[i].GetComponent<Collider>().bounds.Contains(controllerPos + controllerOffset))
                {
                    currentBoundsBox = bounds[i];
                    startPosition = bounds[i].gameObject.transform.position;
                    return;
                }
            }
        }
        else if (!OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, controller) && isTriggerDown == true)
        {
            // Player is no longer holding down the trigger
            isTriggerDown = false;

            currentBoundsBox = null;
        }

        if (isTriggerDown && currentBoundsBox != null) { MoveObjectWithHands(); }

    }

    private void RemoveItem()
    {
        // Check if controller is currently in a bound's location
        if (OVRInput.Get(OVRInput.Button.One, controller))
        {
            if (!isTriggerDown && currentBoundsBox == null)
            {
                Collider[] hitColliders = Physics.OverlapSphere(controllerPos + controllerOffset, 0.1f, Physics.AllLayers, QueryTriggerInteraction.Collide);
                for (int i = 0; i < hitColliders.Length; i++)
                {
                    if (hitColliders[i].gameObject.tag == "Object")
                    {
                        DestroyImmediate(hitColliders[i].gameObject);
                        return;
                    }
                }
            }
        }

        // Draw hologram eraser

    }
    #endregion

    #region SET SPAWNER CONFIGURATION
   

    public void SetSpawnMode(int mode)
    {
        activeSpawnMode = (SpawnMode)mode;
    }

    public void SelectItemSpawn(int itemType)
    {
        activeItemSpawn = (ItemType)itemType;
    }

    private GameObject GetObjectToSpawn()
    {
        switch (activeItemSpawn)
        {
            case ItemType.None:
                break;
            case ItemType.Pan:
                break;
            case ItemType.Log:
                break;
            case ItemType.Phone:
                break;
            default:
                break;
        }

        return new GameObject();
    }
    #endregion

    #region OBJECT/BOUND RESIZERS & REPOSITIONERS
    private void ResizeWithJoyStick()
    {
        // resize the box to be relative to the controller
        Vector3 posLock = GetPosLock();
        currentBoundsBox.transform.localScale +=
            new Vector3(
                OVRInput.Get(
                    OVRInput.Axis2D.PrimaryThumbstick, controller).x * posLock.x,
                OVRInput.Get(
                    OVRInput.Axis2D.PrimaryThumbstick, controller).y * posLock.y,
                OVRInput.Get(
                    OVRInput.Axis2D.PrimaryThumbstick, controllerAlt).y * posLock.z)
            * Time.deltaTime;
    }

    private void RepositionWithJoyStick()
    {
        // reposition current object relative to the controller
        forwardMoveAmount += OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, controller).y * Time.deltaTime;
        yRotAmount += OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, controller).x * Time.deltaTime * rotationSpeed;
    }


    private void ResizeWithHands(Vector3 controllerPos, Vector3 controllerOffset)
    {
        Vector3 posLock = GetPosLock();

        // Gets a vector that points from the player's position to the target's.
        Vector3 heading = controllerPos + controllerOffset - startPosition;
        float distance = heading.magnitude;
        // Vector3 direction = heading / distance;
        Vector3 midPoint = ((startPosition + controllerPos + controllerOffset) / 2.0f);

        // pos locked version
        Vector3 headingLock = new Vector3(heading.x * posLock.x, heading.y * posLock.y, heading.z * posLock.z);

        SetGlobalTransform(currentBoundsBox.transform, midPoint, headingLock, true, false);
    }

    private void MoveObjectWithHands()
    {
        if (currentBoundsBox != null)
        {
            // Move the object to the new position and rotation of the controller
            Vector3 posLock = GetPosLock();
            currentBoundsBox.transform.position = controllerPos + controllerOffset;

            Quaternion alt = new Quaternion(controllerRot.x * posLock.x, controllerRot.y * posLock.y, controllerRot.z * posLock.z, controllerRot.w);
            currentBoundsBox.transform.rotation = alt;



            currentBoundsBox.transform.position += controllerDirection * forwardMoveAmount;
            currentBoundsBox.transform.Rotate(0, yRotAmount, 0, Space.Self);
        }
    }

    private Vector3 GetPosLock()
    {
        switch (lockType)
        {
            case LockPositionType.None:
                return new Vector3(1, 1, 1);
            case LockPositionType.X:
                return new Vector3(1, 0, 0);
            case LockPositionType.Y:
                return new Vector3(0, 1, 0);
            case LockPositionType.Z:
                return new Vector3(0, 0, 1);
            default:
                return new Vector3(1, 1, 1);
        }
    }


    #endregion
  
    #region UTILITIES
    void ClampGrabOffset(ref Vector3 localOffset, float thumbY)
    {
        Vector3 projectedGrabOffset = localOffset + Vector3.forward * thumbY * 0.01f;
        if (projectedGrabOffset.z > 0.1f)
        {
            localOffset = projectedGrabOffset;
        }
    }

    Vector3 ClampScale(Vector3 localScale, Vector2 thumb)
    {
        float newXscale = localScale.x + thumb.x * 0.01f;
        if (newXscale <= 0.1f) newXscale = 0.1f;
        float newZscale = localScale.z + thumb.y * 0.01f;
        if (newZscale <= 0.1f) newZscale = 0.1f;
        return new Vector3(newXscale, 0.0f, newZscale);
    }

    #region GLOBAL TRANSFORM SCALING
    public void SetGlobalTransform(Transform trans, Vector3 pos, Vector3 scale, bool bSetScale, bool bSetScaleOnGlobalAxes)
    {
        if (bSetScale)
        {
            trans.position = pos;
            trans.localScale = Vector3.one;
            var m = trans.worldToLocalMatrix;
            if (bSetScaleOnGlobalAxes)
            {
                m.SetColumn(0, new Vector4(m.GetColumn(0).magnitude, 0f));
                m.SetColumn(1, new Vector4(0f, m.GetColumn(1).magnitude));
                m.SetColumn(2, new Vector4(0f, 0f, m.GetColumn(2).magnitude));
            }
            m.SetColumn(3, new Vector4(0f, 0f, 0f, 1f));
            trans.localScale = m.MultiplyPoint(scale);
        }
        else
        {
            trans.position = pos;
        }
    }

    public void SetGlobalTransform(Transform trans, Vector3 pos, Quaternion rot, Vector3 scale, bool bSetScale, bool bSetScaleOnGlobalAxes)
    {
        if (bSetScale)
        {
            trans.position = pos;
            trans.rotation = rot;
            trans.localScale = Vector3.one;
            var m = trans.worldToLocalMatrix;
            if (bSetScaleOnGlobalAxes)
            {
                m.SetColumn(0, new Vector4(m.GetColumn(0).magnitude, 0f));
                m.SetColumn(1, new Vector4(0f, m.GetColumn(1).magnitude));
                m.SetColumn(2, new Vector4(0f, 0f, m.GetColumn(2).magnitude));
            }
            m.SetColumn(3, new Vector4(0f, 0f, 0f, 1f));
            trans.localScale = m.MultiplyPoint(scale);
        }
        else
        {
            trans.position = pos;
            trans.rotation = rot;
        }
    }
    #endregion

    #endregion

    #region DATA MANIPULATION
    private void UpdateControllerData()
    {
        // Update controller stats for this frame
        controllerPos = RayPointer.Instance.transform.position;
        //OVRInput.GetLocalControllerPosition(controller);
        controllerRot = OVRInput.GetLocalControllerRotation(controller);

        // Get controller forward direction
        controllerOffset = controllerRot * Vector3.forward;
        controllerDirection = controllerOffset.normalized;
        controllerOffset = controllerOffset.normalized * controllerOffsetAmount;
    }
    private void ResetHighlight()
    {
        currentMaterialHolder = currentMaterialInstance;
        currentObjectHolder = null;
    }
    private void ResetAll()
    {
        ResetHighlight();
        currentObjectHolder = null;
        currentMaterialInstance = null;
        currentMaterialHolder = null;
        currentBoundsBox = null;
        forwardMoveAmount = 0.0f;
        yRotAmount = 0.0f;

    }
    #endregion
}
