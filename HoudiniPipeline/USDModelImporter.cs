using UnityEngine;

public class USDModelImporter : MonoBehaviour
{
    public const string DefaultPipelineRootFolder = "Assets/HoudiniPipeline/Pipeline";
    public const string DefaultUsdOutputFolder = "Assets/HoudiniPipeline/USD";
    public const string DefaultUsdExportFolder = "Assets/HoudiniPipeline/USD/Export";

    [Header("Houdini")]
    public string houdiniProject = "";
    public string houdiniNodePath = "/obj/geo1";
    public string hythonPath = "";
    public string processScriptPath = "";

    [Header("Pipeline Folders")]
    public string pipelineRootFolder = DefaultPipelineRootFolder;
    public string usdOutputFolder = DefaultUsdOutputFolder;
    public string usdExportFolder = DefaultUsdExportFolder;

    [Header("Behaviour")]
    public bool disableSourceOnImport;
}
