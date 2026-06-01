using System.Collections;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

public class TeleportPuzzleSequenceRule : MonoBehaviour
{
    private static readonly string[] ShapeSequence = { "Ractangle", "Triangle", "Circle" };
    private const string FloorTag = "Floor";

    [Header("Reset")]
    [SerializeField] private Vector3 resetPosition = new Vector3(0f, 2f, -42f);

    [Header("Debug")]
    [SerializeField] private bool logRuleChecks;

    private XROrigin xrOrigin;
    private Transform fallbackPlayerRoot;
    private TeleportPuzzleFloorContactWatcher floorContactWatcher;
    private int expectedShapeIndex;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void CreateRuleRunner()
    {
        if (FindFirstObjectByType<TeleportPuzzleSequenceRule>() != null)
            return;

        var runner = new GameObject(nameof(TeleportPuzzleSequenceRule));
        runner.AddComponent<TeleportPuzzleSequenceRule>();
        DontDestroyOnLoad(runner);
    }

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        UnsubscribeTeleportTargets();
    }

    private void Start()
    {
        BindSceneObjects();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        expectedShapeIndex = 0;
        BindSceneObjects();
    }

    private void BindSceneObjects()
    {
        UnsubscribeTeleportTargets();
        RemoveFloorContactWatcher();

        xrOrigin = FindFirstObjectByType<XROrigin>();
        fallbackPlayerRoot = xrOrigin != null ? xrOrigin.transform : GameObject.FindWithTag("Player")?.transform;

        foreach (var target in FindObjectsByType<BaseTeleportationInteractable>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            target.teleporting.AddListener(OnTeleporting);

        BindFloorContactWatcher();
    }

    private void UnsubscribeTeleportTargets()
    {
        foreach (var target in FindObjectsByType<BaseTeleportationInteractable>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            target.teleporting.RemoveListener(OnTeleporting);
    }

    private void BindFloorContactWatcher()
    {
        var characterController = fallbackPlayerRoot != null
            ? fallbackPlayerRoot.GetComponentInChildren<CharacterController>()
            : FindFirstObjectByType<CharacterController>();

        var watcherTarget = characterController != null
            ? characterController.gameObject
            : fallbackPlayerRoot != null ? fallbackPlayerRoot.gameObject : null;

        if (watcherTarget == null)
            return;

        floorContactWatcher = watcherTarget.GetComponent<TeleportPuzzleFloorContactWatcher>();
        if (floorContactWatcher == null)
            floorContactWatcher = watcherTarget.AddComponent<TeleportPuzzleFloorContactWatcher>();

        floorContactWatcher.Initialize(this);
    }

    private void RemoveFloorContactWatcher()
    {
        if (floorContactWatcher == null)
            return;

        floorContactWatcher.Initialize(null);
        floorContactWatcher = null;
    }

    private void OnTeleporting(TeleportingEventArgs args)
    {
        var currentShape = GetShapeName(args.interactableObject.transform);
        if (string.IsNullOrEmpty(currentShape))
            return;

        var expectedShape = ShapeSequence[expectedShapeIndex];
        var isCorrect = currentShape == expectedShape;

        if (logRuleChecks)
            Debug.Log($"Teleport rule check: expected {expectedShape}, actual {currentShape}", this);

        if (isCorrect)
        {
            expectedShapeIndex = (expectedShapeIndex + 1) % ShapeSequence.Length;
            return;
        }

        expectedShapeIndex = 0;
        StartCoroutine(ResetPlayerAfterTeleport());
    }

    private static string GetShapeName(Transform target)
    {
        while (target != null)
        {
            if (IsShapeName(target.tag))
                return target.tag;

            if (IsShapeName(target.name))
                return target.name;

            target = target.parent;
        }

        return null;
    }

    private static bool IsShapeName(string value)
    {
        for (var i = 0; i < ShapeSequence.Length; i++)
        {
            if (value == ShapeSequence[i])
                return true;
        }

        return false;
    }

    private IEnumerator ResetPlayerAfterTeleport()
    {
        yield return null;
        yield return null;

        ResetPlayer();
    }

    public void ResetPlayerFromFloor()
    {
        expectedShapeIndex = 0;
        ResetPlayer();
    }

    private void ResetPlayer()
    {
        MovePlayer(resetPosition);
    }

    private void MovePlayer(Vector3 position)
    {
        var controller = fallbackPlayerRoot != null ? fallbackPlayerRoot.GetComponent<CharacterController>() : null;
        if (controller != null)
            controller.enabled = false;

        if (xrOrigin != null)
            xrOrigin.MoveCameraToWorldLocation(position);
        else if (fallbackPlayerRoot != null)
            fallbackPlayerRoot.position = position;

        if (controller != null)
            controller.enabled = true;

        Physics.SyncTransforms();
    }
}

sealed class TeleportPuzzleFloorContactWatcher : MonoBehaviour
{
    private TeleportPuzzleSequenceRule rule;
    private bool isResetting;

    public void Initialize(TeleportPuzzleSequenceRule owner)
    {
        rule = owner;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        TryResetFromFloor(hit.collider);
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryResetFromFloor(collision.collider);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryResetFromFloor(other);
    }

    private void TryResetFromFloor(Collider other)
    {
        if (rule == null || isResetting || other == null || !IsFloor(other.transform))
            return;

        StartCoroutine(ResetAfterPhysicsStep());
    }

    private IEnumerator ResetAfterPhysicsStep()
    {
        isResetting = true;
        yield return null;

        rule.ResetPlayerFromFloor();

        yield return new WaitForSeconds(0.2f);
        isResetting = false;
    }

    private static bool IsFloor(Transform target)
    {
        while (target != null)
        {
            if (target.CompareTag("Floor") || target.name == "Floor")
                return true;

            target = target.parent;
        }

        return false;
    }
}
