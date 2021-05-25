using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [HideInInspector] public float speed = 0;
    public float swipeDownDistance;
    [HideInInspector] public bool swipeDown;
    [HideInInspector] public bool isPush;
    [SerializeField] private float horizontalSpeed;
    [SerializeField] private float maxSwipeTime;
    [SerializeField] private Transform leftCube;
    [SerializeField] private Transform rightCube;
    [SerializeField] private LineRenderer lineRenderer;

    [Header("Rope Settings")]
    [SerializeField] private ObiRopeCursor cursorLeft;
    [SerializeField] private ObiRope ropeLeft;
    [SerializeField] private ObiParticleAttachment attachmentLeft;
    [Space]
    [SerializeField] private ObiRopeCursor cursorRight;
    [SerializeField] private ObiRope ropeRight;
    [SerializeField] private ObiParticleAttachment attachmentRight;
    [SerializeField] private Vector3 initalVelocity;

    private Rigidbody rb;
    private Animator animator;
    private Touch theTouch;
    private Vector2 touchStartPosition, touchEndPosition;
    private float oldRopeLengthL;
    private float oldRopeLengthR;
    private float swipeTime;

    void Start() {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        oldRopeLengthL = ropeLeft.restLength;
        oldRopeLengthR = ropeRight.restLength;
    }

    void Update() {
        //GEÇİCİDİR --------
        if (transform.position.z > 120)
            Invoke("Restart", 0.5f);
        //GEÇİCİDİR --------

        if (rb.transform.position.y <= -2.484f)
        {
            isPush = false;

            cursorLeft.ChangeLength(oldRopeLengthL);
            cursorRight.ChangeLength(oldRopeLengthR);

            attachmentLeft.enabled = true;
            attachmentRight.enabled = true;
           
            animator.ResetTrigger("Jump");
        }
        else
        {
            isPush = true;
        }

        if (isPush)
            return;

        SwipeControl();
        HorizontalMove();
    }

    void FixedUpdate() {
        Move();

        if (!swipeDown)
            return;

        lineRenderer.positionCount = 100;

        float t;
        t = (-1f * initalVelocity.y) / Physics.gravity.y;
        t = 2f * t;

        Vector3 trajectoryPoint;

        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            float time = t * i / lineRenderer.positionCount;
            trajectoryPoint = lineRenderer.transform.position + initalVelocity * time + 0.5f * Physics.gravity * time * time;
            lineRenderer.SetPosition(i, trajectoryPoint);
        }
    }

    private void Move() {
        if (rb.transform.position.y <= -2.484f)
            if (!swipeDown && !isPush)
                transform.Translate(transform.forward * speed * Time.deltaTime);
    }

    private void HorizontalMove() {
        if (Input.touchCount > 0 && transform.position.x < 3.3f && transform.position.x > -3.3f)
        {
            theTouch = Input.GetTouch(0);

            if (theTouch.phase == TouchPhase.Moved)
            {
                Vector2 touchPos = theTouch.deltaPosition;
                transform.Translate(touchPos.x * horizontalSpeed / 100 * Time.deltaTime, 0, 0);

                float clampX = Mathf.Clamp(transform.position.x, -2.5f, 2.5f);
                transform.position = new Vector3(clampX, transform.position.y, transform.position.z);

                if (!swipeDown)
                {
                    cursorLeft.ChangeLength(Vector3.Distance(leftCube.position, transform.position) - 2f);
                    cursorRight.ChangeLength(Vector3.Distance(rightCube.position, transform.position) - 2f);

                    oldRopeLengthL = ropeLeft.restLength;
                    oldRopeLengthR = ropeRight.restLength;
                }

                float clamValue = Mathf.Clamp(transform.position.x, -1.5f, 1.5f);
                animator.SetFloat("Sittings", clamValue, 0.1f, Time.deltaTime);
            }
        }
    }

    private void SwipeControl() {
        if (theTouch.phase == TouchPhase.Began)
        {
            touchStartPosition = theTouch.position;
            rb.velocity = Vector3.zero;

            swipeTime = maxSwipeTime;
        }
        else if (theTouch.phase == TouchPhase.Moved || theTouch.phase == TouchPhase.Stationary)
        {
            touchEndPosition = theTouch.position;

            float y = touchEndPosition.y - touchStartPosition.y;

            if (y < -swipeDownDistance)
            {
                animator.SetTrigger("Pulling");
                swipeDown = true;
                lineRenderer.enabled = true;

                /*swipeTime -= Time.deltaTime;
                if (swipeTime <= 0)
                    Push();*/

                //initalVelocity += new Vector3(0, Time.deltaTime * 6, Time.deltaTime * 6);
            }
            /*else
            {
                Push();
            }*/
        }
        else if (theTouch.phase == TouchPhase.Ended)
        {
            if (swipeDown && !isPush)
                Push();
        }
    }

    private void Push() {
        isPush = true;

        animator.ResetTrigger("Pulling");
        lineRenderer.positionCount = 0;
        rb.velocity = initalVelocity;

        StartCoroutine(PullLeftRope());
        StartCoroutine(PullRightRope());

        swipeDown = false;
        animator.SetTrigger("Jump");
    }

    private IEnumerator PullLeftRope() {
        while (ropeLeft.restLength > 0.5f)
        {
            yield return null;
            cursorLeft.ChangeLength(ropeLeft.restLength - 0.5f);
        }

        attachmentLeft.enabled = false;
    }

    private IEnumerator PullRightRope() {
        while (ropeRight.restLength > 0.5f)
        {
            yield return null;
            cursorRight.ChangeLength(ropeRight.restLength - 0.5f); //bounds of the list : warning! //zıplamadan çarparsan fail ver // 5 adet küp olsun
        }

        attachmentRight.enabled = false;
    }

    private void OnTriggerEnter(Collider col) {
        if (!col.CompareTag("Rock"))
            return;

        //GEÇİCİDİR --------
        if (!isPush)
            Invoke("Restart", 1.5f);
        //GEÇİCİDİR --------

        foreach (Rigidbody rb in col.GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = false;
            rb.AddForce(transform.forward * Time.deltaTime * 20);
        }
    }

    //GEÇİCİDİR --------
    private void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);   
    }
    //GEÇİCİDİR --------
}
