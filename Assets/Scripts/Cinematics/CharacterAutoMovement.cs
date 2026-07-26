using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController2D))]
public class CharacterAutoMovement : MonoBehaviour
{
    [Header("Movimiento automático")]
    [SerializeField]
    [Min(0.01f)]
    private float arrivalDistance = 0.1f;

    [Header("Seguimiento")]
    [SerializeField]
    [Min(0.1f)]
    private float followDistance = 1.5f;

    private CharacterController2D characterController;

    private Transform followTarget;
    private bool following;
    private bool runWhileFollowing;

    private void Awake()
    {
        characterController =
            GetComponent<CharacterController2D>();
    }

    private void Update()
    {
        if (!following || followTarget == null)
        {
            return;
        }

        FollowCurrentTarget();
    }

    public IEnumerator MoveTo(
        Transform destination,
        bool run = false
    )
    {
        if (destination == null)
        {
            Debug.LogError(
                "El punto de destino no está asignado.",
                this
            );

            yield break;
        }

        StopFollowing();

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

    public void StartFollowing(
        Transform target,
        bool run = false
    )
    {
        followTarget = target;
        runWhileFollowing = run;
        following = true;
    }

    public void StopFollowing()
    {
        following = false;
        followTarget = null;

        characterController.Stop();
    }

    private void FollowCurrentTarget()
    {
        float horizontalDistance =
            followTarget.position.x -
            transform.position.x;

        if (
            Mathf.Abs(horizontalDistance) <=
            followDistance
        )
        {
            characterController.Stop();
            return;
        }

        float direction = Mathf.Sign(
            horizontalDistance
        );

        characterController.Move(
            direction,
            runWhileFollowing
        );
    }
}