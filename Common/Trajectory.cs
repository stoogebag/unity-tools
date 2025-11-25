using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Trajectory : MonoBehaviour {
    [SerializeField] private LineRenderer _line;
    [SerializeField] private int _maxPhysicsFrameIterations = 100;
    [SerializeField] private Transform _obstaclesParent;

    private Scene _simulationScene;
    private PhysicsScene _physicsScene;
    private readonly Dictionary<Transform, Transform> _spawnedObjects = new Dictionary<Transform, Transform>();

    [SerializeField]
    private LocalPhysicsMode _physicsMode = LocalPhysicsMode.Physics2D;
    
    private void Awake() {
        CreatePhysicsScene();
    }

    private void CreatePhysicsScene() {
        _simulationScene = SceneManager.CreateScene("Simulation", new CreateSceneParameters(_physicsMode));
        _physicsScene = _simulationScene.GetPhysicsScene();

        //spawn ghost obstacles
        if (_obstaclesParent != null)
        {
            foreach (Transform obj in _obstaclesParent)
            {
                var ghostObj = Instantiate(obj.gameObject, obj.position, obj.rotation);
                if (ghostObj.TryGetComponent<Renderer>(out var r)) r.enabled = false;

                SceneManager.MoveGameObjectToScene(ghostObj, _simulationScene);
                if (!ghostObj.isStatic) _spawnedObjects.Add(obj, ghostObj.transform);
            }
        }
    }

    private void Update() {
        foreach (var item in _spawnedObjects) {
            item.Value.position = item.Key.position;
            item.Value.rotation = item.Key.rotation;
        }
    }

    public void SimulateTrajectory(Rigidbody prefab, Vector3 pos, Vector3 vel) {
        var ghostObj = Instantiate(prefab, pos, Quaternion.identity);
        //ghostObj.IsGhost = true;
        ghostObj.gameObject.name = "trajectoryObject";
        SceneManager.MoveGameObjectToScene(ghostObj.gameObject, _simulationScene);

        #if UNITY_6000_0_OR_NEWER
            ghostObj.linearVelocity = vel;
        #else
            ghostObj.velocity = vel;
        #endif
        
      
        _line.positionCount = _maxPhysicsFrameIterations;

        for (var i = 0; i < _maxPhysicsFrameIterations; i++) {
            _physicsScene.Simulate(Time.fixedDeltaTime);
            _line.SetPosition(i, ghostObj.transform.position);
        }

        Destroy(ghostObj.gameObject);
    }
}