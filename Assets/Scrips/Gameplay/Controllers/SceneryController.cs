using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Car
{
    public GameObject carObj;
    public Rigidbody rb;
    public float speed;
    public Vector2 speedRange;
    public float currentLifetime = 0.0f;

    public Car(GameObject carObj, float speed)
    {
        this.carObj = carObj;
        this.speed = speed;
    }
}

public class SceneryController : MonoBehaviour
{
    [Tooltip("How far the rig should be to stop animating")]
    [SerializeField] private float heightLimit = 50.0f;

    [SerializeField] private Transform carLane;
    [SerializeField] private float laneSize;

    [SerializeField] private Vector2 carsPosLimit;

    [SerializeField] private List<CarData> cars;

    [SerializeField] private List<Car> carInstances;

    [SerializeField] private float maxLifetime = 20.0f;

    private bool active = true;

    private void Start()
    {
        GameplayManager.OnTowerPosUpdated += UpdateTargetHeight;

        foreach (CarData car in cars)
        {
            if (car != null && car.carModel != null)
            {
                // Sugestion: linea muy larga con varios numeros magicos (2.0f, 0.05f); extraer a metodo helper GetRandomSpawnPosition() mejoraria la legibilidad.
                // Warning: Esta logica esta duplicada casi identica en Update().
                Vector3 carPos = new Vector3(carsPosLimit.x - Random.Range(car.speedRange.x, car.speedRange.y)*2.0f, carLane.position.y, carLane.position.z + ((Random.value > 0.5f) ? -laneSize : laneSize) + Random.Range(-0.05f,0.05f));
                GameObject spawnedCar = Instantiate(car.carModel, carPos, carLane.rotation, carLane);
                spawnedCar.name = car.carName;
                Car newCar = new Car(spawnedCar, Random.Range(car.speedRange.x, car.speedRange.y));
                // Sugestion: agregar BoxCollider y Rigidbody por codigo en cada coche es costoso; mejor tenerlos ya en el prefab del modelo.
                spawnedCar.AddComponent<BoxCollider>();
                newCar.rb = spawnedCar.AddComponent<Rigidbody>();
                newCar.speedRange = car.speedRange;
                newCar.rb.AddForce(new Vector3(Random.Range(car.speedRange.x, car.speedRange.y), 0.0f, 0.0f), ForceMode.VelocityChange);
                carInstances.Add(newCar);
            }
        }
    }

    private void OnDestroy()
    {
        GameplayManager.OnTowerPosUpdated -= UpdateTargetHeight;
    }

    private void UpdateTargetHeight(Vector2 newHeight)
    {
        if (newHeight.y > heightLimit)
        {
            active = false;
        }
    }

    void Update()
    {
        if (!active) return;

        foreach (Car car in carInstances)
        {
            // Warning: se manipula transform.position de objetos con Rigidbody desde Update; mover fisicas debe hacerse en FixedUpdate via rb.MovePosition.
            car.currentLifetime += Time.deltaTime;
            Vector3 carPos = car.carObj.transform.position;
            if (carPos.x >= carsPosLimit.y || car.currentLifetime > maxLifetime)
            {
                carPos = new Vector3(carsPosLimit.x - (Random.Range(car.speedRange.x, car.speedRange.y) * 2.0f), carLane.position.y, carLane.position.z + ((Random.value > 0.5f) ? -laneSize : laneSize) + Random.Range(-0.05f, 0.05f));
                car.carObj.transform.rotation = carLane.rotation;
                car.rb.linearVelocity = Vector3.zero;
                car.rb.angularVelocity = Vector3.zero;
                car.speed = Random.Range(car.speedRange.x, car.speedRange.y);
                car.currentLifetime = 0.0f;
                car.rb.AddForce(new Vector3(car.speed, 0.0f, 0.0f), ForceMode.VelocityChange);
            }

            if (carPos.y < carLane.position.y)
            {
                Vector3 linearVel = car.rb.linearVelocity;
                linearVel.y = Mathf.Abs(car.rb.linearVelocity.y);
                car.rb.linearVelocity = linearVel;
                carPos.y = carLane.position.y;
            }
            car.carObj.transform.position = carPos;
        }
    }
}