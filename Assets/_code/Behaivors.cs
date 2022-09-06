using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behaivors : MonoBehaviour
{
    private enum Behavior
    {
        Friction, 
        Wind, 
        Magnetic
    }

    [SerializeField] private Behavior _behaivor;

    [Header("Dependencies")]
    [SerializeField] private GameObject _target;

    [Header("Settings")]
    [SerializeField] private float _speed;
    [Tooltip("For friction it can't be higher than speed")]
    [SerializeField] private float _force; 

    private GameObject _thisObject;
    private Vector3 _initialPos;
    private Vector3 _moveDirection;
    private Vector3 _windDirection;
    private float _resetTime;

    private void Awake()
    {
        _thisObject = this.gameObject;
        _initialPos = this.transform.position;
        _moveDirection = _thisObject.transform.position;
        _windDirection = RotateWind(_thisObject, _target, _force);
    }

    private void FixedUpdate()
    {
        ApplyBehavior();

        if (_resetTime > 4f)
        {
            Reset();
        } else
        {
            _resetTime += Time.deltaTime;
        }
    }

    private void ApplyBehavior()
    {
        switch (_behaivor)
        {
            case Behavior.Friction:
                _moveDirection += Friction(_thisObject, _target, _speed, _force);
                break;

            case Behavior.Wind:
                _moveDirection += Wind(_thisObject, _target, _speed, _force, _windDirection);
                break;

            case Behavior.Magnetic:
                _moveDirection += Magnetic(_thisObject, _target, _speed, _force);
                break;
        }
        _thisObject.transform.position = _moveDirection;
    }

    public Vector3 Friction(GameObject seeker, GameObject target, float speed, float force)
    {
        Vector3 distance = target.transform.position - seeker.transform.position;
        Vector3 desiredV = distance.normalized * speed; //normal movement
        Vector3 desiredVF = distance.normalized * -1f * force; //friction
        Vector3 currentV = Vector3.zero;
        Vector3 steering = desiredV - currentV + desiredVF;
        currentV += steering;
        return currentV * Time.deltaTime;
    }

    public Vector3 Wind(GameObject seeker, GameObject target, float speed, float force, Vector3 windDirection)
    {
        Vector3 distance = target.transform.position - seeker.transform.position;
        Vector3 desiredV = distance.normalized * speed; //normal movement
        Vector3 currentV = Vector3.zero;
        Vector3 steering = desiredV - currentV + windDirection;
        currentV += steering;
        return currentV * Time.deltaTime;
    }

    public Vector3 Magnetic(GameObject seeker, GameObject target, float speed, float force)
    {
        Vector3 distance = target.transform.position - seeker.transform.position;
        Vector3 desiredV = distance.normalized * speed; //normal movement
        Vector3 desiredVM = distance.normalized * force; //magnetic
        Vector3 currentV = Vector3.zero;
        Vector3 steering = desiredV - currentV + desiredVM;
        currentV += steering;
        return currentV * Time.deltaTime;
    }

    private Vector3 RotateWind(GameObject seeker, GameObject target, float force)
    {
        Vector3 distanceW = target.transform.position - seeker.transform.position;
        distanceW = distanceW.normalized;
        distanceW = Vector3.RotateTowards(distanceW, new Vector3(Random.Range(-100, 100), Random.Range(-100, 100), 0f), 2f, 1f);
        distanceW = distanceW * force;
        return distanceW;
    }

    private void Reset()
    {
        _resetTime = 0f;
        _thisObject.transform.position = _initialPos;
        _moveDirection = _thisObject.transform.position;
        _windDirection = RotateWind(_thisObject, _target, _force);
    }
}
