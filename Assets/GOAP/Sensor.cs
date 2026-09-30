//using System;
//using UnityEngine;


//[RequireComponent(typeof(SphereCollider))]
//public class Sensor:MonoBehaviour
//{
//    [SerializeField] float detectionRadius = 5f;
//    [SerializeField] float timeInterval = 1f;

//    SphereCollider detectionRange;

//    public event Action OnTargetChanged = delegate { };
//    public Vector3 TargetPosition=>target ? target.transform.position : Vector3.zero;
//    public bool isTargetInRange => TargetPosition != Vector3.zero;

//    GameObject target;
//    Vector3 lastKnownPosition;
//    CountdownTimer timer;

//    private void Awake()
//    {
//        detectionRange = GetComponent<SphereCollider>();
//        detectionRange.isTrigger = true;
//        detectionRange.radius = detectionRadius;
//    }
//    void Start()
//    {
//        timer = new CountdownTimer(timeInterval);
//    }
//    void UpdateTargetPosition(GameObject target=null)
//    {
//        this.target = target;
//    }
    
//    private void OnDrawGizmos()
//    {
//        Gizmos.color=isTargetInRange?Color.red:Color.green;
//        Gizmos.DrawWireSphere(transform.position, detectionRadius);
//    }

   
//}
