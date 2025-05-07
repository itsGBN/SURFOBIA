using UnityEngine;

public class SpinObject : MonoBehaviour
{
    [SerializeField] private float _rotateSpeed = 5.0f;
    [SerializeField] private float _xRotation;
    [SerializeField] private float _yRotation;
    [SerializeField] private float _zRotation;
    private Vector3 _currentRotationAngles;
    private Quaternion _currentRotation;
    
    void Start()
    {
        transform.rotation = Quaternion.identity;
        transform.position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        _currentRotationAngles += new Vector3(_xRotation, _yRotation, _zRotation) * Time.deltaTime * _rotateSpeed;

        _currentRotation.eulerAngles = _currentRotationAngles;
    }

    void FixedUpdate()
    {
        transform.rotation = _currentRotation;
    }
}
