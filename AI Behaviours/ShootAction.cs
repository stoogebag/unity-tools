using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace stoogebag.AI_Behaviours
{
    [TaskDescription("Fires the character's gun at the target.")]
    [TaskCategory("stoogebag")]
    //[BehaviorDesigner.Runtime.Tasks.HelpURL("https://www.opsive.com/support/documentation/behavior-designer-movement-pack/")]
    [TaskIcon("04fb8138ea905c04ea39265f778fe1a4", "9bded0edc8b2a2f478fc28396fa41df2")]
    public class ShootAction : Action
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("The GameObject that the agent is shooting at")]
        public SharedGameObject m_Target;

        public override void OnStart()
        {
            base.OnStart();
            Debug.Log($"! started to shoot at {m_Target.Value.name}");
        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }

        // Reset the public variables
        public override void OnReset()
        {
        }
    }
}