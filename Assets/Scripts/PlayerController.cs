using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    /*[SerializeField] private float _speed;
    [SerializeField] private float _maxDistance;
    [SerializeField] private Transform _target;

    private void Update()
    {
        if (_target != null)
        {
            Vector3 newPosition = Vector3.MoveTowards(transform.position, _target.position, _speed * Time.deltaTime);
            transform.position = newPosition;
        }
        Vector3 distance = transform.position - _target.position;
        if(distance.magnitude >= _maxDistance)
        {

        }
    }*/

    public Transform _target; // Список контрольных точек
    public float speed = 5f; // Скорость движения
    public float rotationSpeed = 2f; // Скорость поворота
    public float _searchRadius;
    private bool isRotating = false; // Флаг для проверки статуса вращения
    private bool turnLeft = false;

    void Update()
    {
        if (_target != null)
        {
            MoveToWaypoint();
        }
    }

    private void MoveToWaypoint()
    {
        Vector3 direction = (_target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        if (!isRotating && Vector3.Distance(transform.position, _target.position) < 1f)
        {
            isRotating = true;
            StartCoroutine(RotateAndSwitchWaypoint());
            Destroy(_target.gameObject);
        }
    }

    private IEnumerator RotateAndSwitchWaypoint()
    {
        float rotationAngle = turnLeft ? 90f : -90f;
        Quaternion targetRotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + rotationAngle, 0);
        float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);

        while (angleDifference > 0.01f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            angleDifference = Quaternion.Angle(transform.rotation, targetRotation); // Обновление разницы угла
            yield return null;
        }

        isRotating = false; // Сброс флага после завершения вращения
        turnLeft = !turnLeft;
        FindNextWaypoint(); // Поиск следующей контрольной точки после вращения
    }

    private void FindNextWaypoint()
    {
        GameObject[] allObjects = GameObject.FindGameObjectsWithTag("Waypoint");

        foreach (GameObject obj in allObjects)
        {
            float distance = Vector3.Distance(transform.position, obj.transform.position);
            if (distance <= _searchRadius)
            {
                _target = obj.transform;
            }
        }
    }
}
