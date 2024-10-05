using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class PuzzGrid
{
    
    public Vector3 Origin;
    
#if ODIN_INSPECTOR
    [Button]
#endif
    public void ResetGrid()
    {
        var origin = Vector3.zero;

        //var nodeEnts = FindObjectsOfType<NodeEntity>();
        
        //print(nodeEnts.Length);
        
       // var min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        
        //min= new Vector3()
        // foreach (var nodeEnt in nodeEnts)
        // {
        //     min = Vector3.Min(min, nodeEnt.transform.position);
        // }
        
        //CreateGrid(Origin, SizeX, SizeY, SizeZ, SpacingX, SpacingY,SpacingZ, WrapX, WrapY, WrapZ);
    }
    public void RequestReset()
    {
        //clear the queue
        MoveQueue.Queue.Clear();
        _ = UndoAll();
    }
    public async UniTask RequestUndo()
    {
        //todo: clear the queue OF ACTIONS ONLY. we want multiple undo to work properly
        await Undo();
    }
    

    public void PrevLevel()
    {
        var index = SceneManager.GetActiveScene().buildIndex;
        if(index == 0) return;
        
        SceneLoadManager.Instance.ChangeSceneWithFade(index-1, 0.5f, Color.white);
    }
    public void NextLevel()
    {
        var index = SceneManager.GetActiveScene().buildIndex;
        if(index == SceneManager.sceneCountInBuildSettings-1) return;
        
        SceneLoadManager.Instance.ChangeSceneWithFade(index+1, 0.5f, Color.white);
    }
    
    public void ReloadLevel()
    {
        var index = SceneManager.GetActiveScene().buildIndex;
        //if(index == SceneManager.sceneCountInBuildSettings-1) return;
        
        SceneLoadManager.Instance.ChangeSceneWithFade(index, 0.5f, Color.white);
    }
    public void LoadLevelAtIndex(int index)
    {
        //if(index == SceneManager.sceneCountInBuildSettings-1) return;
        SceneLoadManager.Instance.ChangeSceneWithFade(index, 0.5f, Color.white);
    }

    public void PauseUnpause()
    {
        if (StageWon) return; 
        if (Paused) _ = Unpause();
        else _ = Pause();
    }
    
    public async UniTask Pause()
    {
        if (Paused) return;
        print("pause");
        Paused = true;
        await UIManager.Open("PauseMenuPanel");
        
        Time.timeScale = 0;
        
    }

    public bool Paused { get; set; } =  false;
    public bool StageWon { get; set; } =  false;
    public bool StageLost { get; set; } =  false;

    public async UniTask Unpause()
    {
        Time.timeScale = 1;
        await UIManager.Close("PauseMenuPanel");
        Paused = false;
    }

    public void Won()
    {
        Paused = true;
        StageWon = true;
        UIManager.Open("WinPanel");
    }
    public void Lost(string message)
    {
        StageLost = true;
        StageWon = false;
        UIManager.Open("LosePanel");
    }
    
    public void UnWon()
    {
        Paused = false;
        StageWon = false;
        UIManager.Close("WinPanel");
    }

    public void UnLost()
    {
        StageLost = false;
        UIManager.Close("LosePanel");
    }


}
