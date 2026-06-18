using UnityEngine;

public class ArenaGeneratorReward : MonoBehaviour
{
    [SerializeField] private GeneratorScript generator;
    [SerializeField] private ThirdPersonController player;
    [SerializeField] private int requiredMinigamesComplete = 3;
    [SerializeField] private int condensorsRewarded = 2;
    [SerializeField] private bool rewardOnce = true;
    [SerializeField] private bool disableAfterReward = false;

    [Header("Runtime Debug")]
    [SerializeField] private bool rewardGranted;

    private void Awake()
    {
        if (generator == null)
        {
            generator = GetComponent<GeneratorScript>();
        }

        if (player == null)
        {
            player = FindFirstObjectByType<ThirdPersonController>();
        }
    }

    private void Update()
    {
        if (generator == null || player == null)
        {
            return;
        }

        if (rewardOnce && rewardGranted)
        {
            return;
        }

        if (generator.minigamesComplete < requiredMinigamesComplete)
        {
            return;
        }

        player.condensors += condensorsRewarded;
        player.UpdateUI();
        rewardGranted = true;

        if (disableAfterReward)
        {
            enabled = false;
        }
    }
}
