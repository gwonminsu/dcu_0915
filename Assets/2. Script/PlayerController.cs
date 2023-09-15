using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5.0f; // 플레이어 이동 속도
    public float rotationSpeed = 10.0f; // 회전 속도
    public float jumpForce = 7.0f; // 점프 힘
    private bool isGrounded = true; // 플레이어가 땅에 있는지 여부를 나타내는 변수
    private Rigidbody rb;
    private float currentRotationVelocity;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // 플레이어 이동 처리
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontalInput, 0.0f, verticalInput).normalized;
        Vector3 movement = moveDirection * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);

        // 플레이어 회전 처리
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }

        // 점프 처리
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 땅에 닿으면 isGrounded를 true로 설정
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
