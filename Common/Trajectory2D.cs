using System.Collections.Generic;
using stoogebag.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Trajectory2D : MonoBehaviour {
    [SerializeField] private LineRenderer _line;
    [SerializeField] private Transform _obstaclesParent;

    private static Scene _simulationScene;
    private static PhysicsScene _physicsScene;
    private readonly Dictionary<Transform, Transform> _spawnedObjects = new Dictionary<Transform, Transform>();

    public static bool Initialized = false;
    
    [SerializeField]
    private LocalPhysicsMode _physicsMode = LocalPhysicsMode.Physics2D;
    
    //this exists for arcane hopefully never needed again reasons.
    [SerializeField] private float VelocityScale = 1;
    
    private void Awake() {
        CreatePhysicsScene();
    }

    private void CreatePhysicsScene()
    {
        if (Initialized) return;
        print("initializing physics scene");
        Initialized = true;
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

    public void SimulateTrajectory(Rigidbody2D prefab, Vector3 pos, Vector3 vel, float time, float timeStep = -1) {
        var ghostObj = Instantiate(prefab, pos, Quaternion.identity);
        //ghostObj.IsGhost = true;
        ghostObj.gameObject.name = "trajectoryObject";
        SceneManager.MoveGameObjectToScene(ghostObj.gameObject, _simulationScene);

        Physics2D.simulationMode = SimulationMode2D.Script;
        
        _physicsScene.Simulate(Time.fixedDeltaTime);
        //ghostObj.AddForce(vel.ToVector2(), ForceMode2D.Impulse);
        
        
#if UNITY_6
        ghostObj.linearVelocity = vel.ToVector2();

#else  
        ghostObj.velocity = vel.ToVector2();
#endif
        
        ghostObj.simulated = true;
        ghostObj.transform.position = ghostObj.transform.position.WithZ(1);

        if(timeStep == -1) timeStep = Time.fixedDeltaTime;
        var iterationCount = time / timeStep;
        _line.positionCount = Mathf.FloorToInt(iterationCount);
        

        var v = vel* VelocityScale;
        
        
        for (var i = 0; i < iterationCount; i++) {
        //    _physicsScene.Simulate(Time.fixedDeltaTime);
            
            //for some reason, this shit isn't working. so i am just gonna do it myself. this works for a parabola but won't do anything else...
            _line.SetPosition(i, ghostObj.transform.position);
            ghostObj.transform.position = ghostObj.transform.position + v * timeStep;
            v += (Physics2D.gravity * Time.fixedDeltaTime).WithZ(0);
            
            
            
        }

        
        Physics2D.simulationMode = SimulationMode2D.FixedUpdate;
        Destroy(ghostObj.gameObject);
    }
}