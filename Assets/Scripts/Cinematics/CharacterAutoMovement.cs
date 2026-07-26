using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController2D))]
public class CharacterAutoMovement : MonoBehaviour
{
    [Header("Llegada")]
    [SerializeField]
    [Min(0.01f)]
    private float arrivalDistance = 0.1f;

    private CharacterController2D characterController;

    private void Awake()
    {
        characterController =
            GetComponent<CharacterController2D>();
    }

    public IEnumerator MoveTo(
        Transform destination,
        bool run = false
    )
    {
        if (destination == null)
        {
            Debug.LogError(
                "No se asignó el destino del personaje.",
                this
            );

            yield break;
        }

        while (
            Mathf.Abs(
                destination.position.x -
                transform.position.x
            ) > arrivalDistance
        )
        {
            float direction = Mathf.Sign(
                destination.position.x -
                transform.position.x
            );

            characterController.Move(
                direction,
                run
            );

            yield return null;
        }

        characterController.Stop();
    }

    public void StopMovement()
    {
        characterController.Stop();
    }
}