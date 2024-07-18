using Cysharp.Threading.Tasks;

public class Cooldown
{
    public Cooldown(float cooldownInSeconds)
    {
        this.CooldownInSeconds = cooldownInSeconds;
    }
    
    //returns true if the cooldown is 'active' IE if the thing should NOT proceed.
    public bool CooldownActive {
        get
        {
            if (_active) return true;
            else
            {
                _active = true;
                UniTask.WaitForSeconds(CooldownInSeconds).ContinueWith(()=>_active = false).Forget();
                return false;
            }
        }
    }

    private float CooldownInSeconds;
    private bool _active;
}