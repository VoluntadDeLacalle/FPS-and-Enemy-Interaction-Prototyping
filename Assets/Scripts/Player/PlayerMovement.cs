using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    CharacterController controller;
    public float speed;
    public float runSpeed;
    private bool isSprinting;
    private bool isCrouching;
    
    Vector3 velocity;
    public float gravity;
    public float jumpHeight;
    
    public Transform groundCheck;
    float groundDistance = 0.4f;
    public LayerMask groundMask;
    bool isGrounded;

    public Transform weaponTransform;
    private Vector3 weaponOrigin;
    private Vector3 weaponBobPosition;

    private float movementCounter;
    private float idleCounter;

    public float swayIntensity;
    public float swaySmoothness;
    private Quaternion originRotation;

    public float crippledSpeed;
    
    public float crouchSpeed;
    public float crouchAmount;
    public GameObject crouchCollider;
    public GameObject standingCollider;
    private Vector3 cameraOrigin;

    private float x;
    private float z;
    private Camera normalCam;

    public float sprintStatDegredation;


    // Start is called before the first frame update
    void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        weaponOrigin = weaponTransform.localPosition;
        originRotation = weaponTransform.localRotation;
        cameraOrigin = gameObject.GetComponentInChildren<Camera>().transform.localPosition;
        normalCam = gameObject.GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        GroundCheck();

        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");

        isSprinting = Input.GetKey(KeyCode.LeftShift) && z > 0 && isGrounded;
        isCrouching = Input.GetKey(KeyCode.LeftControl) && isGrounded && !isSprinting;

        standingCollider.SetActive(true);
        crouchCollider.SetActive(false);

        //weaponTransform.localPosition += Vector3.down * crouchAmount;

        Movement();
        BobController();
        WeaponSway();
        CameraStuff();
    }

    private void HeadBob(float z, float x_intensity, float y_intensity)
    {
        weaponBobPosition = weaponOrigin + new Vector3(Mathf.Cos(z)*x_intensity, Mathf.Sin(z * 2)*y_intensity, 0);
    }

    private void WeaponSway()
    {
        float x_mouse = Input.GetAxis("Mouse X");
        float y_mouse = Input.GetAxis("Mouse Y");

        Quaternion x_adjust = Quaternion.AngleAxis(-swayIntensity * x_mouse, Vector3.up);
        Quaternion y_adjust = Quaternion.AngleAxis(swayIntensity * y_mouse, Vector3.right);
        Quaternion targetRotation = originRotation * x_adjust * y_adjust;

        weaponTransform.localRotation = Quaternion.Lerp(weaponTransform.localRotation, targetRotation, Time.deltaTime * swaySmoothness);
        
    }

    private void Movement()
    {
        if (isCrouching)
        {
            Vector3 move = transform.right * x + transform.forward * z;
            controller.Move(move * crouchSpeed * Time.deltaTime);

            standingCollider.SetActive(false);
            crouchCollider.SetActive(true);

            //Use this if you want the weapon to do a magic trick
            //weaponTransform.localPosition += Vector3.down * crouchAmount;
        }

        if (isSprinting && FindObjectOfType<PlayerUI>().sprint > 0)
        {
            Vector3 move = transform.right * x + transform.forward * z;
            controller.Move(move * runSpeed * Time.deltaTime);
            FindObjectOfType<PlayerUI>().hunger -= (runSpeed * Time.deltaTime) * sprintStatDegredation;
            FindObjectOfType<PlayerUI>().hydration -= (runSpeed * Time.deltaTime) * sprintStatDegredation;
            FindObjectOfType<PlayerUI>().sprint -= FindObjectOfType<PlayerUI>().sprintDegredation * Time.deltaTime;
        }

        else if (FindObjectOfType<PlayerUI>().health < 50)
        {
            Vector3 move = transform.right * x + transform.forward * z;
            controller.Move(move * crippledSpeed * Time.deltaTime);
        }

        else
        {
            Vector3 move = transform.right * x + transform.forward * z;
            controller.Move(move * speed * Time.deltaTime);
            if(FindObjectOfType<PlayerUI>().sprint < 100 && FindObjectOfType<PlayerUI>().health > 50)
                FindObjectOfType<PlayerUI>().sprint += FindObjectOfType<PlayerUI>().sprintRegeneration * Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if(FindObjectOfType<PlayerUI>().sprint <= 0)
        {
            isSprinting = false;
        }
    }

    private void BobController()
    {
        //Idle Head Bob
        if (x == 0 && z == 0)
        {
            HeadBob(idleCounter, 0.035f, 0.035f);
            idleCounter += Time.deltaTime;

            //Smoothen Head Bob
            weaponTransform.localPosition = Vector3.Lerp(weaponTransform.localPosition, weaponBobPosition, Time.deltaTime * 2f);
        }

        //Sprint Head Bob
        else if (isSprinting)
        {
            HeadBob(movementCounter, 0.1f, 0.1f);
            movementCounter += Time.deltaTime * 6f;

            //Smoothen Head Bob
            weaponTransform.localPosition = Vector3.Lerp(weaponTransform.localPosition, weaponBobPosition, Time.deltaTime * 8f);
        }

        else if (!isGrounded)
        {
            HeadBob(idleCounter, 0.035f, 0.035f);
            idleCounter += 0f;

            //Smoothen Head Bob
            weaponTransform.localPosition = Vector3.Lerp(weaponTransform.localPosition, weaponBobPosition, Time.deltaTime * 2f);
        }

        else if (isCrouching)
        {
            HeadBob(movementCounter, 0.02f, 0.02f);
            movementCounter += Time.deltaTime * 1.7f;

            //Smoothen Head Bob
            weaponTransform.localPosition = Vector3.Lerp(weaponTransform.localPosition, weaponBobPosition, Time.deltaTime * 8f);
        }

        //Motion Head Bob
        else
        {
            HeadBob(movementCounter, 0.05f, 0.05f);
            movementCounter += Time.deltaTime * 3f;

            //Smoothen Head Bob
            weaponTransform.localPosition = Vector3.Lerp(weaponTransform.localPosition, weaponBobPosition, Time.deltaTime * 8f);
        }
    }

    private void CameraStuff()
    {
        //Camera
        if (isCrouching)
        {
            normalCam.transform.localPosition = Vector3.Lerp(normalCam.transform.localPosition, cameraOrigin + Vector3.down * crouchAmount, Time.deltaTime * 6f);
        }
        else
        {
            normalCam.transform.localPosition = Vector3.Lerp(normalCam.transform.localPosition, cameraOrigin, Time.deltaTime * 6f);
        }
    }
    
    private void GroundCheck()
    {
        //Handles if we're on ground
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;
    }
}
