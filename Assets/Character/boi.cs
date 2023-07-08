using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEditor;
using UnityEngine;

public class boi : MonoBehaviour
{
    public float _jumpForce = 20;
    public float _maxMoveSpeed = 10;
    public float _horizontalAccel = 1000;
    public float _coyoteTime = 0.1f;
    public float _dynamicFriction = 1.2f; // horizontal velocity is divided by this amount each frame without movement input
    public float _throwStrength = 1; // scales velocity-based throw velocity
    public float _lobStrength = 10; // scales base throw velocity

    public GameObject _heldObject;

    private Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;
    private SpriteRenderer _handsRenderer;
    private FixedJoint2D _handsJoint;
    private Animator _animator;
    private enum TouchingGround { No = 0, Yes, Coyote };
    private List<Collider2D> _colliding = new List<Collider2D>();
    private TouchingGround _isGrounded = TouchingGround.No;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        _handsRenderer = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
        _handsJoint = gameObject.GetComponent<FixedJoint2D>();
        _animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump") && _isGrounded != TouchingGround.No)
        {
            Debug.Log("junp");
            _rigidbody.velocity = Vector2.up * _jumpForce;
            _isGrounded = TouchingGround.No;
        }

        Vector2 inputVector = new Vector2(System.Math.Sign(Input.GetAxis("Horizontal")), System.Math.Sign(Input.GetAxis("Vertical")));
        _rigidbody.velocity = new Vector2(
            Mathf.Clamp(_rigidbody.velocity.x + _horizontalAccel * inputVector.x * Time.deltaTime, 
                -_maxMoveSpeed, _maxMoveSpeed),
            _rigidbody.velocity.y
        );

        _animator.enabled = Input.anyKey;

        if (Input.GetAxis("Horizontal") != 0 && _rigidbody.velocity.x != 0)
        {
            _spriteRenderer.flipX = _rigidbody.velocity.x < 0;
            _handsRenderer.flipX = _rigidbody.velocity.x < 0;
        }
        else 
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x / _dynamicFriction, _rigidbody.velocity.y);
        }

        _handsRenderer.enabled = _heldObject != null;

        System.Action ThrowObject = () =>
        {
            _handsJoint.enabled = false;
            _heldObject.GetComponent<Rigidbody2D>().velocity = Input.GetAxis("Vertical") < 0 ? Vector2.zero:
                new Vector2(_rigidbody.velocity.x * _throwStrength, 0) + 
                new Vector2 (_spriteRenderer.flipX ? -1 : 1, 2) * _lobStrength;
            _heldObject = null;
        };

        System.Action PickUpNearest = () =>
        {
            List<Collider2D> colliders = new List<Collider2D>();
            _rigidbody.OverlapCollider(new ContactFilter2D(), colliders);
            colliders = (from collider in colliders where collider.gameObject.layer == LayerMask.NameToLayer("Item") select collider).ToList<Collider2D>();
            if (colliders.Count == 0) return;
            _heldObject = colliders.Aggregate((curMin, x) =>
                (curMin == null || Vector3.Distance(x.transform.position, gameObject.transform.position) <
                Vector3.Distance(curMin.transform.position, gameObject.transform.position) ? x : curMin)).gameObject;
            _heldObject.transform.position = gameObject.transform.position;
            _handsJoint.connectedBody = _heldObject.GetComponent<Rigidbody2D>();
            _handsJoint.enabled = true;
        };

        if (Input.GetButtonDown("Grab"))
        {
            if (_heldObject != null) {
                ThrowObject();
            } else
            {
                PickUpNearest();
            }
        }
        // Debug coyote time
        //_spriteRenderer.color = _isGrounded == TouchingGround.Yes ? Color.white :
        //  (_isGrounded == TouchingGround.Coyote ? Color.blue : Color.red);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _colliding.Add(collision.collider);
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Ground") )
        {
            _isGrounded = TouchingGround.Yes;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        _colliding.Remove(collision.collider);

        if (!_colliding.Any(collider => collision.collider.gameObject.layer == LayerMask.NameToLayer("Ground")) 
            && _isGrounded == TouchingGround.Yes)
        {
             StartCoroutine(StartCoyoteTime());
        }
    }

    private IEnumerator StartCoyoteTime()
    {
        _isGrounded = TouchingGround.Coyote;
        yield return new WaitForSeconds(_coyoteTime);
        if (!_colliding.Any(collider => collider.gameObject.CompareTag("Ground")))
        {
            _isGrounded = TouchingGround.No;
        }
    }
}
