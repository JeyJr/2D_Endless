using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class PlayerController : MonoBehaviour
{
    public PlayerMove playerInput;
    [SerializeField] private CharacterController controller;
    [SerializeField] private float playerSpeed = 2.0f;
    PlayerHand playerHand;


    //Y Detect
    [SerializeField] private LayerMask target;
    [SerializeField] private Transform yPosition;
    [SerializeField] private float yTopRange, yTop;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerInput = new PlayerMove();
        playerInput.Enable();
  
        playerHand = GetComponentInChildren<PlayerHand>();
    }

    void Update()
    {
        Vector2 moveInput = playerInput.Player.Move.ReadValue<Vector2>();
        Vector3 move = new Vector3(moveInput.x, moveInput.y, 0f);
        controller.Move(move * Time.deltaTime * playerSpeed);

        if (move.x < 0)
        {
            transform.localEulerAngles = new Vector3(0, 180, 0);
            playerHand.delayBar.GetComponent<Slider>().direction = Slider.Direction.RightToLeft;
            //PlayerAnimMove
        }
        else if (move.x > 0)
        {
            transform.localEulerAngles = new Vector3(0, 0, 0);
            playerHand.delayBar.GetComponent<Slider>().direction = Slider.Direction.LeftToRight;
            //PlayerAnimMove
        }
        else
        {
            //Stop anim move
        }

        RayCastSetPlayerZPosition();
    }


    void RayCastSetPlayerZPosition()
    {
        RaycastHit2D yTopHit = Physics2D.Raycast(yPosition.position + new Vector3(0, yTop, 0), yPosition.up, yTopRange, target);
        RaycastHit2D yTopHit2 = Physics2D.Raycast(yPosition.position + new Vector3(-1f, yTop, 0), yPosition.up, yTopRange, target);
        RaycastHit2D yTopHit3 = Physics2D.Raycast(yPosition.position + new Vector3(1f, yTop, 0), yPosition.up, yTopRange, target);

        if (yTopHit || yTopHit2 || yTopHit3)
            transform.position = new Vector3(transform.position.x, transform.position.y, 1);
        else
            transform.position = new Vector3(transform.position.x, transform.position.y, -1);
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(yPosition.position + new Vector3(0,  yTop, 0), yPosition.up * yTopRange, Color.blue);
        Debug.DrawRay(yPosition.position + new Vector3(-1f,  yTop, 0), yPosition.up * yTopRange, Color.blue);
        Debug.DrawRay(yPosition.position + new Vector3(1f,  yTop, 0), yPosition.up * yTopRange, Color.blue);

    }
}
