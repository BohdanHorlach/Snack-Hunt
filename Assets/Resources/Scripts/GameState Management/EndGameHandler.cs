using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Unity.Cinemachine;


public class EndGameHandler : MonoBehaviour
{
    [SerializeField] private GameOverTrigger _gameOverTrigger;
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private FoodCollector _foodCollector;
    [SerializeField] private CinemachineBrain _camera;
    [SerializeField] private CinemachineCamera _endGameVirtualCamera;
    [SerializeField] private UIAnimateHandler _UIAnimator;
    [SerializeField] private Animator _playerAnimator;
    [SerializeField] private float _delayForEndGame = 5f;
    

    private const string TITLE_SCREEN_SCENE_NAME = "TitleScreen";
    private bool _isEndGame;


    public UnityEvent OnGameEnded;


    private void Awake()
    {
        _camera.DefaultBlend.Style = CinemachineBlendDefinition.Styles.EaseIn;
        _endGameVirtualCamera.Priority = -1;
    }


    private void OnEnable()
    {
        _gameOverTrigger.OnTriggered += EndGame;
        _foodCollector.OnDropingEding += OnDropingEnd;
    }


    private void OnDisable()
    {
        _gameOverTrigger.OnTriggered -= EndGame;
        _foodCollector.OnDropingEding -= OnDropingEnd;
    }


    private void EndGame()
    {
        _UIAnimator.EndGame();
        _playerMovement.enabled = false;
        _camera.DefaultBlend.Style = CinemachineBlendDefinition.Styles.Cut;
    }


    //Calls from animation
    private void ChangeVirtualCamera()
    {
        _endGameVirtualCamera.Priority = int.MaxValue;
    }


    //Calls from animation
    private void StartDropingFood()
    {
        _foodCollector.StartDropingFood();
    }


    private void SetEndGameState()
    {
        _isEndGame = true;
        OnGameEnded.Invoke();
    }


    private void SetAnimationTrigger()
    {
        if (FoodCollector.Counter < FoodCollector.Goal)
            _playerAnimator.SetTrigger("Lose");
        else
            _playerAnimator.SetTrigger("Victory");
    }


    private void OnDropingEnd()
    {
        SetAnimationTrigger();
        Invoke("SetEndGameState", _delayForEndGame);
    }


    public void GoToTitleScreen()
    {
        AsyncLoader.LoadScene(TITLE_SCREEN_SCENE_NAME);
    }


    public void ExitFromLevel(InputAction.CallbackContext context)
    {
        if (_isEndGame)
            GoToTitleScreen();
    }
}