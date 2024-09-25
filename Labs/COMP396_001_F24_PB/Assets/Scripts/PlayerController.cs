using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private int _horizontal = 0;
    [SerializeField] private int _vertical = 0;
    [SerializeField] private Vector3 _movementVector;

    // Update is called once per frame
    void Update()
    {
        HandleKeyStrokes();
        Move();
    }

    private void HandleKeyStrokes()
    {
        HandlePressedDown();
        HandlePressedUp();

        _movementVector = new Vector3(_horizontal, 0f, _vertical);
    }

    private void HandlePressedDown()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            _vertical = Mathf.Clamp(_vertical + 1, 0, 1);
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            _vertical = Mathf.Clamp(_vertical - 1, -1, 0);
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            _horizontal = Mathf.Clamp(_horizontal - 1, -1, 0);
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            _horizontal = Mathf.Clamp(_horizontal + 1, 0, 1);
        }
    }

    private void HandlePressedUp()
    {
        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow))
        {
            _vertical = 0;
        }

        //if (Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow))
        //{
        //    _vertical = Mathf.Clamp(_vertical - 1, -1, 0);
        //}

        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow))
        {
            _horizontal = 0;
        }

        //if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow))
        //{
        //    _horizontal = Mathf.Clamp(_horizontal + 1, 0, 1);
        //}
    }

    private void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, transform.position + _movementVector, _speed * Time.deltaTime);
    }
}
