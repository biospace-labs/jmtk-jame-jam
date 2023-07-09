using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class boi : MonoBehaviour
{
    public float _jumpForce = 20;
    public float _maxMoveSpeed = 10;
    public float _horizontalAccel = 1000;
    public float _coyoteTime = 0.1f;
    public float _dynamicFriction = 1.2f; // horizontal velocity is divided by this amount each frame without movement input
    public float _throwStrength = 1; // scales velocity-based throw velocity
    public float _lobStrength = 10; // scales base throw velocity
    public float _immuneTime = 2;
    public float _minimumHoldTime = 0.2f;

    public GameObject _heldObject;

    private Rigidbody2D _rigidbody;
    private CapsuleCollider2D _foot;
    private SpriteRenderer _spriteRenderer;
    private SpriteRenderer _handsRenderer;
    private FixedJoint2D _handsJoint;
    private Animator _animator;
    private enum TouchingGround { No = 0, Yes, Coyote };
    private enum KnockedDown { No = 0, Yes, Immune };
    private List<Collider2D> _colliding = new List<Collider2D>();
    private TouchingGround _isGrounded = TouchingGround.No;
    private KnockedDown _isProne = KnockedDown.No;
    private Useable _using;
    private float _throwStart;
    private float _throwEnd;
    private bool _throwTimerStarted = false;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        _handsRenderer = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
        _handsJoint = gameObject.GetComponent<FixedJoint2D>();
        _animator = gameObject.GetComponent<Animator>();
        _foot = gameObject.GetComponent<CapsuleCollider2D>(); // Make sure the foot is the first box collider!!
    }

    // Update is called once per frame
    void Update()
    {
        if (_isProne != KnockedDown.Yes)
        {
            handleInput();
        }

        _animator.enabled = Input.GetButton("Horizontal") && _isProne != KnockedDown.Yes;

        _handsRenderer.enabled = _heldObject != null;

        if (_colliding.Count != 0)
            _isGrounded = TouchingGround.Yes;

        if (Input.GetKeyDown(KeyCode.H))
        {
            GetHit(new Vector2(3, 25));
        }
    }

    private void handleInput()
    {
        if (Input.GetAxis("Vertical") > 0 && _isGrounded != TouchingGround.No)
        {
            _rigidbody.velocity = Vector2.up * _jumpForce;
            _isGrounded = TouchingGround.No;
        }

        Vector2 inputVector = new Vector2(System.Math.Sign(Input.GetAxis("Horizontal")), System.Math.Sign(Input.GetAxis("Vertical")));
        Debug.Log(inputVector);
        _rigidbody.velocity = new Vector2(
            Mathf.MoveTowards(_rigidbody.velocity.x, inputVector.x * _maxMoveSpeed, _horizontalAccel),
            _rigidbody.velocity.y
        );
    

        if (Input.GetButtonDown("Grab"))
        {
            if (_heldObject) {
                _throwTimerStarted = true;
                _throwStart = Time.timeSinceLevelLoad;
            } else
            {
                PickUpNearest();
            }
        }

        if (Input.GetButtonUp("Grab"))
        {
            if (_throwTimerStarted) {
                _throwEnd = Time.timeSinceLevelLoad;

                if (_throwEnd - _throwStart < _minimumHoldTime)
                {
                    DropObject();
                } else {
                    ThrowObject();
                }

                _throwTimerStarted = false;
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

        if (Input.GetAxis("Horizontal") != 0 && _rigidbody.velocity.x != 0)
        {
            _spriteRenderer.flipX = _rigidbody.velocity.x < 0;
            _handsRenderer.flipX = _rigidbody.velocity.x < 0;
        }
        else
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x / _dynamicFriction, _rigidbody.velocity.y);
        }

        // Debug coyote time
        //_spriteRenderer.color = _isGrounded == TouchingGround.Yes ? Color.white :
        //  (_isGrounded == TouchingGround.Coyote ? Color.blue : Color.red);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        List<Collider2D> colliders = new List<Collider2D>();
        var filter = new ContactFilter2D();
        filter.SetLayerMask(LayerMask.GetMask("Ground"));
        _foot.OverlapCollider(filter, colliders);
        _colliding = colliders;
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        List<Collider2D> colliders = new List<Collider2D>();
        var filter = new ContactFilter2D();
        filter.SetLayerMask(LayerMask.GetMask("Ground"));
        _foot.OverlapCollider(filter, colliders);
        _colliding = colliders;

        if (_colliding.Count == 0 && _isGrounded == TouchingGround.Yes)
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

    private IEnumerator StartKnockDownTime(float knockDownTime)
    {
        _isProne = KnockedDown.Yes;
        _animator.enabled = false;
        gameObject.GetComponent<SpriteResolver>().SetCategoryAndLabel("Normal", "Prone");
        yield return new WaitForSeconds(knockDownTime);
        StartCoroutine(StartImmuneTime());
    }
    private IEnumerator StartImmuneTime()
    {
        _isProne = KnockedDown.Immune;
        float elapsedTime = 0f;
        while (elapsedTime < _immuneTime)
        {
            elapsedTime += Time.deltaTime;
            _spriteRenderer.color = Math.Round(elapsedTime * 10) % 2 == 0 ? Color.white : Color.clear;
            yield return null;
        }
        _spriteRenderer.color = Color.white;
        _isProne = KnockedDown.No;
    }

    private void ThrowObject()
    {
        _handsJoint.enabled = false;
        _heldObject.GetComponent<Collider2D>().enabled = true;
        _heldObject.GetComponent<Rigidbody2D>().velocity = Input.GetAxis("Vertical") < 0 ? Vector2.zero:
            new Vector2(_rigidbody.velocity.x * _throwStrength, 0) + 
            new Vector2 (_spriteRenderer.flipX ? -1 : 1, 2) * _lobStrength;
        _heldObject = null;
    }

    private void DropObject()
    {
        _handsJoint.enabled = false;
        _heldObject.GetComponent<Collider2D>().enabled = true;
        _heldObject = null;
    }

    private void PickUpNearest()
    {
        Rigidbody2D closest = getClosestOverlapping<Rigidbody2D>("Item");
        if (!closest) return;

        _heldObject = closest.gameObject;
        _heldObject.transform.position = gameObject.transform.position;
        _heldObject.GetComponent<Collider2D>().enabled = false;
        _handsJoint.connectedBody = closest;
        _handsJoint.enabled = true;
    }
    public void GetHit(Vector2 knockback, float downTime = 1)
    {
        if (_isProne != KnockedDown.No) return; // Can't get hit if down already or immune
        if (_heldObject)
        {
            ThrowObject();
        }
        if (_using)
        {
            _using.EndUse();
            _using = null;
        }
        _rigidbody.velocity = knockback;
        foreach(ParticleSystem particleSystem in gameObject.transform.GetChild(2).transform.GetComponentsInChildren<ParticleSystem>())
        {
            particleSystem.Play();
        }
        StartCoroutine(StartKnockDownTime(downTime));
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
