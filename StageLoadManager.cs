// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using stoogebag.Utils;
// using UnityEngine;
//
// public class StageLoadManager : Singleton<StageLoadManager>
// {
//     public StageManager CurrentStage;
//     public List<StageManager> AllStagesPrefabs;
//
//     public async Task LoadStage(int index)
//     {
//         await UnloadCurrentStage();
//
//         var stage = Instantiate(AllStagesPrefabs[index]);
//         CurrentStage = stage;
//         stage.BindToScene();
//         await stage.InitStage();
//     }
//
//     private async Task UnloadCurrentStage()
//     {
//         if (CurrentStage != null)
//         {
//             await CurrentStage.UnloadStage();
//             Destroy(CurrentStage.gameObject);
//         }
//     }
//
//
//     private void Update()
//     {
//         if (AppManager.Instance.IsDebug)
//         {
//             if (Input.GetKeyDown(KeyCode.Alpha1)) LoadStage(0);
//             if (Input.GetKeyDown(KeyCode.Alpha2)) LoadStage(1);
//             if (Input.GetKeyDown(KeyCode.Alpha3)) LoadStage(2);
//             if (Input.GetKeyDown(KeyCode.Alpha4)) LoadStage(3);
//             if (Input.GetKeyDown(KeyCode.Alpha5)) LoadStage(4);
//             if (Input.GetKeyDown(KeyCode.Alpha6)) LoadStage(5);
//             if (Input.GetKeyDown(KeyCode.Alpha7)) LoadStage(6);
//             if (Input.GetKeyDown(KeyCode.Alpha8)) LoadStage(7);
//             if (Input.GetKeyDown(KeyCode.Alpha9)) LoadStage(8);
//             if (Input.GetKeyDown(KeyCode.Alpha0)) LoadStage(9);
//             
//             if (Input.GetKeyDown(KeyCode.KeypadPlus)) LoadStage(CurrentStage.Index + 1);
//             if (Input.GetKeyDown(KeyCode.KeypadMinus)) LoadStage(CurrentStage.Index - 1);
//
//         }
//     }
// }
