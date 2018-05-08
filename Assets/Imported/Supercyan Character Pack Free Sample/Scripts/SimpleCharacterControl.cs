using UnityEngine;
using System.Collections.Generic;


public class SimpleCharacterControl : MonoBehaviour
{

    public Node nextNode;
    public GameObject waypoints;
    public Camera cameraPlayer;
    public GameObject face;
    public Material foodFace;
    public Material thirstFace;
    public Material energyFace;
    public Material comfortFace;
    public Material hygeneFace;
    public Material funFace;
    public Material happyFace;


    private Stack<Node> path;

    [SerializeField] private float m_moveSpeed = 2;
    [SerializeField] private float m_turnSpeed = 1;
    [SerializeField] private float m_jumpForce = 4;
    [SerializeField] private Animator m_animator;
    [SerializeField] private Rigidbody m_rigidBody;

    private float m_currentV = 0;
    private float m_currentH = 0;

    private readonly float m_interpolation = 10;
    private readonly float m_walkScale = 0.33f;
    private readonly float m_backwardsWalkScale = 0.16f;
    private readonly float m_backwardRunScale = 0.66f;
    private readonly float m_distanceMin = 0.2f;

    private bool m_wasGrounded;
    private Vector3 m_currentDirection = Vector3.zero;

    private float m_jumpTimeStamp = 0;
    private float m_minJumpInterval = 0.25f;

    private bool m_isGrounded;
    private List<Collider> m_collisions = new List<Collider>();

    private int MAX_FOOD = 200;
    private int MAX_THIRST = 200;
    private int MAX_ENERGY = 200;
    private int MAX_COMFORT = 200;
    private int MAX_HYGENE = 200;
    private int MAX_FUN = 200;

    private int FOOD_RATE = -1;
    private int THIRST_RATE = -1;
    private int ENERGY_RATE = -1;
    private int COMFORT_RATE = -1;
    private int HYGENE_RATE = -1;
    private int FUN_RATE = -1;

    private float food = 50;
    private float thirst = 90;
    private float energy = 200;
    private float comfort = 180;
    private float hygene = 120;
    private float fun = 150;

    void Start()
    {
    }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                if (!m_collisions.Contains(collision.collider))
                {
                    m_collisions.Add(collision.collider);
                }
                m_isGrounded = true;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        bool validSurfaceNormal = false;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                validSurfaceNormal = true; break;
            }
        }

        if (validSurfaceNormal)
        {
            m_isGrounded = true;
            if (!m_collisions.Contains(collision.collider))
            {
                m_collisions.Add(collision.collider);
            }
        }
        else
        {
            if (m_collisions.Contains(collision.collider))
            {
                m_collisions.Remove(collision.collider);
            }
            if (m_collisions.Count == 0) { m_isGrounded = false; }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (m_collisions.Contains(collision.collider))
        {
            m_collisions.Remove(collision.collider);
        }
        if (m_collisions.Count == 0) { m_isGrounded = false; }
    }

    void Update()
    {
        m_animator.SetBool("Grounded", m_isGrounded);

        ClickUpdate();
        UpdateFace();
        UseNode();

        m_wasGrounded = m_isGrounded;
    }

    private void UpdateFace()
    {

        if (food < 30 && food < thirst && food < energy && food < comfort && food < hygene && food < fun)
        {
            face.GetComponent<MeshRenderer>().material = foodFace;
        }
        else if (thirst < 30 && thirst < energy && thirst < comfort && thirst < hygene && thirst < fun)
        {
            face.GetComponent<MeshRenderer>().material = thirstFace;
        }
        else if (energy < 30 && energy < comfort && energy < hygene && energy < fun)
        {
            face.GetComponent<MeshRenderer>().material = energyFace;
        }
        else if (comfort < 30 && comfort < hygene && comfort < fun)
        {
            face.GetComponent<MeshRenderer>().material = comfortFace;
        }
        else if (hygene < 30 && hygene < fun)
        {
            face.GetComponent<MeshRenderer>().material = hygeneFace;
        }
        else if (fun < 30)
        {
            face.GetComponent<MeshRenderer>().material = funFace;
        }
        else
        {
            face.GetComponent<MeshRenderer>().material = happyFace;
        }


    }

    private void UseNode()
    {
        food += (FOOD_RATE + nextNode.food) * Time.deltaTime;
        thirst += (THIRST_RATE + nextNode.thirst) * Time.deltaTime;
        energy += (ENERGY_RATE + nextNode.energy) * Time.deltaTime;
        comfort += (COMFORT_RATE + nextNode.comfort) * Time.deltaTime;
        hygene += (HYGENE_RATE + nextNode.hygene) * Time.deltaTime;
        fun += (FUN_RATE + nextNode.fun) * Time.deltaTime;

        if (food > MAX_FOOD)
        {
            food = MAX_FOOD;
        }

        if (thirst > MAX_THIRST)
        {
            thirst = MAX_THIRST;
        }

        if (energy > MAX_ENERGY)
        {
            energy = MAX_ENERGY;
        }

        if (comfort > MAX_COMFORT)
        {
            comfort = MAX_COMFORT;
        }

        if (hygene > MAX_HYGENE)
        {
            hygene = MAX_HYGENE;
        }

        if (fun > MAX_FUN)
        {
            fun = MAX_FUN;
        }
    }

    private void ClickUpdate()
    {
        if (Input.GetMouseButtonDown(0) || GvrControllerInput.ClickButtonDown)
        {
            float distance = int.MaxValue;
            Node closestNode = nextNode;

            foreach (Node node in waypoints.GetComponentsInChildren<Node>(false))
            {
                //create a ray cast and set it to the mouses cursor position in game
//				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//                RaycastHit hit;
//                if (Physics.Raycast(ray, out hit, 50))
//                {
                    float currentDistance = Vector3.Distance(node.transform.position, GvrPointerInputModule.Pointer.CurrentRaycastResult.worldPosition);
//                    float currentDistance = Vector3.Distance(node.transform.position, hit.point);
                    if (currentDistance < distance)
                    {
                        distance = currentDistance;
                        closestNode = node;
                    }
//                }
            }

            path = nextNode.FindShortest(closestNode, new List<Node>());
        }


        Vector3 difference = nextNode.transform.position - transform.position;

        bool reached = false;

        if (difference.magnitude < 0.1f)
        {
            reached = true;
        }

        m_animator.SetFloat("MoveSpeed", 0.5f);

        if (reached && path != null && path.Count != 0)
        {
            nextNode = path.Pop();
        } else if (reached)
        {
            difference = Vector3.zero;
            m_animator.SetFloat("MoveSpeed", 0f);
        }

        if (difference != Vector3.zero)
        {

            difference = difference.normalized;
            m_currentDirection = Vector3.Lerp(m_currentDirection, difference, Time.deltaTime * 2f);

           transform.rotation = Quaternion.LookRotation(m_currentDirection);
           transform.position += m_currentDirection * m_walkScale * m_moveSpeed * Time.deltaTime;

        }

    }


}