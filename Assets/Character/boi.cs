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
    public float _dynamic_friction = 1.2f; // horizontal velocity is divided by this amount each frame without movement input

    private Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private enum TouchingGround { No = 0, Yes, Coyote };
    private List<Collider2D> _colliding = new List<Collider2D>();
    private TouchingGround _isGrounded = TouchingGround.No;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        _animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Jump") > 0 && _isGrounded != TouchingGround.No)
        {
            _rigidbody.velocity = Vector2.up * _jumpForce;
            _isGrounded = TouchingGround.No;
        }

        Vector2 inputVector = new Vector2(System.Math.Sign(Input.GetAxis("Horizontal")), 0);
        _rigidbody.velocity = new Vector2(
            Mathf.Clamp(_rigidbody.velocity.x + _horizontalAccel * inputVector.x * Time.deltaTime, 
                -_maxMoveSpeed, _maxMoveSpeed),
            _rigidbody.velocity.y
        );

        _animator.enabled = Input.anyKey;

        if (Input.GetAxis("Horizontal") != 0 && _rigidbody.velocity.x != 0)
        {
            _spriteRenderer.flipX = _rigidbody.velocity.x < 0;
        }
        else 
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x / _dynamic_friction, _rigidbody.velocity.y);
        }


        // Debug coyote time
        //_spriteRenderer.color = _isGrounded == TouchingGround.Yes ? Color.white :
        //  (_isGrounded == TouchingGround.Coyote ? Color.blue : Color.red);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _colliding.Add(collision.collider);
        if (collision.collider.gameObject.CompareTag("Ground"))
        {
            _isGrounded = TouchingGround.Yes;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        _colliding.Remove(collision.collider);

        if (!_colliding.Any(collider => collider.gameObject.CompareTag("Ground")) && _isGrounded == TouchingGround.Yes)
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
