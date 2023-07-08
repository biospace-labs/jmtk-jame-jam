using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private Useable _using;

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
        if (Input.GetButtonDown("Vertical") && Input.GetAxis("Vertical") > 0 && _isGrounded != TouchingGround.No)
        {
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

        if (Input.GetButtonDown("Grab"))
        {
            if (_heldObject) {
                ThrowObject();
            } else
            {
                PickUpNearest();
            }
        }

        if (Input.GetButtonDown("Use"))
        {
            Useable closest = getClosestOverlapping<Useable>();
            if (closest != null)
            {
                _using = closest;
                _using.BeginUse();
            }
        }

        if (_using != null && Input.GetButtonUp("Use"))
        {
            _using.EndUse();
            _using = null;
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

    private void ThrowObject()
    {
        _handsJoint.enabled = false;
        _heldObject.GetComponent<Rigidbody2D>().velocity = Input.GetAxis("Vertical") < 0 ? Vector2.zero:
            new Vector2(_rigidbody.velocity.x * _throwStrength, 0) + 
            new Vector2 (_spriteRenderer.flipX ? -1 : 1, 2) * _lobStrength;
        _heldObject = null;
    }

    private void PickUpNearest()
    {
        Rigidbody2D closest = getClosestOverlapping<Rigidbody2D>("Item");
        if (!closest) return;

        _heldObject = closest.gameObject;
        _heldObject.transform.position = gameObject.transform.position;
        _handsJoint.connectedBody = closest;
        _handsJoint.enabled = true;
    }

    private T getClosestOverlapping<T>(string layerName = null)
        where T : class
    {
        List<Collider2D> colliders = new List<Collider2D>();
        var filter = new ContactFilter2D();
        filter.useTriggers = true;
        if (layerName != null) filter.SetLayerMask(LayerMask.GetMask(layerName));
        _rigidbody.OverlapCollider(filter, colliders);
        if (colliders.Count == 0) return null;
        
        T closest = null;
        float min = Single.PositiveInfinity;
        foreach (var collider in colliders)
        {
            T component = collider.gameObject.GetComponent<T>();
            if (component == null) continue;

            float sqrDist = Vector3.SqrMagnitude(collider.transform.position - transform.position);
            if (sqrDist < min)
            {
                closest = component;
                min = sqrDist;
            }
        }
        return closest;
    }
}
