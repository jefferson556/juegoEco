using System;
using UnityEngine;

public class CharacterHealth : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField]
    [Min(1f)]
    private float maxLife = 100f;

    [SerializeField]
    [Min(0f)]
    private float initialLife = 100f;

    public float CurrentLife { get; private set; }
    public float MaxLife => maxLife;

    public float NormalizedLife =>
        maxLife <= 0f
            ? 0f
            : CurrentLife / maxLife;

    public bool HasLife => CurrentLife > 0f;

    public event Action LifeChanged;
    public event Action LifeDepleted;

    private void Awake()
    {
        CurrentLife = Mathf.Clamp(
            initialLife,
            0f,
            maxLife
        );

        LifeChanged?.Invoke();
    }

    public void ConsumeLife(float amount)
    {
        if (amount <= 0f || CurrentLife <= 0f)
        {
            return;
        }

        float previousLife = CurrentLife;

        CurrentLife = Mathf.Max(
            0f,
            CurrentLife - amount
        );

        LifeChanged?.Invoke();

        if (
            previousLife > 0f &&
            CurrentLife <= 0f
        )
        {
            LifeDepleted?.Invoke();
        }
    }

    public void RestoreLife(float amount)
    {
        if (amount <= 0f)
        {
            return;
        }

        CurrentLife = Mathf.Min(
            maxLife,
            CurrentLife + amount
        );

        LifeChanged?.Invoke();
    }

    public void ResetLife()
    {
        CurrentLife = Mathf.Clamp(
            initialLife,
            0f,
            maxLife
        );

        LifeChanged?.Invoke();
    }
}
