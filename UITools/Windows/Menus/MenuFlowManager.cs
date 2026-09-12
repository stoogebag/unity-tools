using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using stoogebag.UITools.Windows;

public class MenuFlowManager : MonoBehaviour
{
    [SerializeField] private Window mainMenuWindow;
    [SerializeField] private Window optionsWindow;
    [SerializeField] private Window creditsWindow;
    [SerializeField] private Window quitConfirmWindow;

    [SerializeField] private Button startButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button optionsBackButton;
    [SerializeField] private Button creditsBackButton;
    [SerializeField] private Button quitYesButton;
    [SerializeField] private Button quitNoButton;

    async void Start()
    {
        startButton.OnClickAsObservable().Subscribe(async _ =>
        {
            await UniTask.WaitForSeconds(3).AwaitWithLoadScreen();

        }).AddTo(this);

        optionsButton.OnClickAsObservable().Subscribe(async _ =>
        {
            await mainMenuWindow.Deactivate();
            await optionsWindow.Activate();
        }).AddTo(this);

        creditsButton.OnClickAsObservable().Subscribe(async _ =>
        {
            await mainMenuWindow.Deactivate();
            await creditsWindow.Activate();
        }).AddTo(this);

        quitButton.OnClickAsObservable().Subscribe(async _ =>
        {
            await mainMenuWindow.Deactivate();
            await quitConfirmWindow.Activate();
        }).AddTo(this);

        optionsBackButton.OnClickAsObservable().Subscribe(async _ =>
        {
            await optionsWindow.Deactivate();
            await mainMenuWindow.Activate();
        }).AddTo(this);

        creditsBackButton.OnClickAsObservable().Subscribe(async _ =>
        {
            await creditsWindow.Deactivate();
            await mainMenuWindow.Activate();
        }).AddTo(this);

        quitYesButton.OnClickAsObservable().Subscribe(async _ =>
        {
            await quitConfirmWindow.Deactivate();
            Application.Quit();
        }).AddTo(this);

        quitNoButton.OnClickAsObservable().Subscribe(async _ =>
        {
            await quitConfirmWindow.Deactivate();
            await mainMenuWindow.Activate();
        }).AddTo(this);

        var fader = SceneFader.Instance;
        //fader.SetColor(Color.white);
        fader.FadeIn(1f);
        await UniTask.Yield();
        await UniTask.Yield();
        mainMenuWindow.Activate();
    }
}
