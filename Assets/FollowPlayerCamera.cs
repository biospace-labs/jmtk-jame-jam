using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FollowPlayerCamera : MonoBehaviour
{
    public float _smoothTime = 0.2f;
    public float verticalOffset = 2f;
    private GameObject _player;
    private Vector3 _velocity = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void LateUpdate()
    {
        gameObject.transform.position =
             Vector3.SmoothDamp(
                 gameObject.transform.position, 
                 new Vector3(
                    _player.transform.position.x,
                    _player.transform.position.y + verticalOffset,
                    gameObject.transform.position.z
                    ),
                 ref _velocity, 
                 _smoothTime
                 );
    }
}
