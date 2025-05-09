using UnityEngine;

public class SpinObject : MonoBehaviour
{
    [SerializeField] private float _rotateSpeed = 5.0f;
    [SerializeField] private float _xRotation;
    [SerializeField] private float _yRotation;
    [SerializeField] private float _zRotation;
    private Vector3 _speedToRotateBy;
    private Vector3 _currentAngles;
    private Quaternion _currentRotation;
    
    void Start()
    {
        _currentAngles = transform.rotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        _speedToRotateBy += new Vector3(_xRotation, _yRotation, _zRotation) * Time.deltaTime * _rotateSpeed;

        _currentRotation.eulerAngles = _currentAngles + _speedToRotateBy;
    }

    void FixedUpdate()
    {
        transform.rotation = _currentRotation;
    }
}
