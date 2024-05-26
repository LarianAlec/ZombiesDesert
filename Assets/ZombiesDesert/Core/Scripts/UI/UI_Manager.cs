using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager instance;

    [Header("Prefabs to create")]
    [SerializeField] private Canvas canvasPrefab;

    [Space]
    [Header("Created instances")]
    public PlayerHUD playerHUD;
    public Canvas canvas;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        canvas = Instantiate(canvasPrefab);
        playerHUD = canvas.GetComponentInChildren<PlayerHUD>();
    }
}
