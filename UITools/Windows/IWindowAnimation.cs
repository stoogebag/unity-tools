using System.Threading.Tasks;

public interface IWindowAnimation
{
    public Task Activate();
    public Task Deactivate();
}
