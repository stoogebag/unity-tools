#if UNITASK
using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class MoveAndAwaitMixerBehaviour : PlayableBehaviour
{
    // NOTE: This function is called at runtime and edit time.  Keep that in mind when setting the values of properties.
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        Transform trackBinding = playerData as Transform;

        if (!trackBinding)
            return;

        int inputCount = playable.GetInputCount ();

        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            ScriptPlayable<MoveAndAwaitBehaviour> inputPlayable = (ScriptPlayable<MoveAndAwaitBehaviour>)playable.GetInput(i);
            MoveAndAwaitBehaviour input = inputPlayable.GetBehaviour ();
            
            // Use the above variables to process each frame of this playable.
            
        }
    }
}
#endif