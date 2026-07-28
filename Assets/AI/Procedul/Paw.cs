using UnityEngine;

public class Paw : MonoBehaviour
{
    public Transform BodyAnchor; // Точка поиска земли
    public Transform TargetTransform; // IK-таргет
    public float DistanceThreshold; // Пороговое значение для проверки расстояния
    private bool Inicialized = false;
    public float Luft = 0.5f;
    public float WalkPrediction = 0.5f;
    private Vector3 PredictionPlace;
    private float StepSpeed = 5f;
    private float StepHeight = 0.42f;
    private bool isMoving = false;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private Quaternion startRotation;
    private Quaternion endRotation;
    private float progress;
    private Vector3 BodyAnchorPosition;
    private Vector3 TargetPosition;
    private Vector3 StartRaycastPosition;
    public LayerMask PlayerMask;
    public PawCenter pawCenter;
    public int Index;


    void Update()
    {
        if(Index >= 1)
            if(pawCenter.PawsWalking[Index - 1] || pawCenter.PawsWalking[Index + 1]) return;
        else
            if(pawCenter.PawsWalking[Index + 1]) return;


        BodyAnchorPosition = BodyAnchor.position;
        TargetPosition = TargetTransform.position;
        BodyAnchorPosition.y = 0;
        TargetPosition.y = 0;
        StartRaycastPosition = BodyAnchor.position;
        StartRaycastPosition.y += 1.5f;

        if(!isMoving && (TargetPosition - BodyAnchorPosition).sqrMagnitude > DistanceThreshold || !Inicialized)
        {
            RaycastHit h;

            if(!Physics.Raycast(StartRaycastPosition, -BodyAnchor.up, out h, 8f, ~PlayerMask))
            {
                Vector3 endPoint = StartRaycastPosition + (-BodyAnchor.up * 8f);
                Debug.DrawLine(StartRaycastPosition, endPoint, Color.red, 3f);
                Debug.LogWarning("Paw.cs: Проверка попадания по Y не удалась");
                return;
            }

            if(h.point.x == 0 || h.point.z == 0 || h.collider.gameObject.tag == "Player")
            {
                Debug.LogWarning("Paw.cs: Проверка попадания по X или Z не удалась");
                return;
            }
            
            Debug.DrawLine(h.point + new Vector3(0, 0.1f, 0), h.point, Color.blue, 2f);


            PredictionPlace = (h.point - TargetTransform.position).normalized * WalkPrediction;

            if(Inicialized)
            {
                Vector3 predictRaycastPosition = h.point + new Vector3(PredictionPlace.x, Luft, PredictionPlace.z);
                predictRaycastPosition.y = StartRaycastPosition.y;

                startPosition = TargetTransform.position;
                startRotation = TargetTransform.rotation;

                RaycastHit h2;

                if(Physics.Raycast(predictRaycastPosition, -Vector3.up, out h2, 8f, ~PlayerMask))
                {
                    if(h2.point.x == 0 || h2.point.z == 0)
                    {
                        endPosition = h.point + new Vector3(PredictionPlace.x, Luft, PredictionPlace.z);
                        endRotation = Quaternion.FromToRotation(BodyAnchor.up, h.normal) * BodyAnchor.rotation;
                    }
                    else
                    {
                        endPosition = h2.point + new Vector3(PredictionPlace.x, Luft, PredictionPlace.z);
                        endRotation = Quaternion.FromToRotation(BodyAnchor.up, h2.normal) * BodyAnchor.rotation;
                    }

                    Debug.Log(Index + ": " + endPosition);
                }
                else
                {
                    Vector3 endPoint = predictRaycastPosition + (-BodyAnchor.up * 8f);
                    Debug.DrawLine(predictRaycastPosition, endPoint, Color.blue, 3f);


                    for(int i = 0; i < 8; i++)
                    {
                        if(Physics.Raycast(Vector3.Lerp(predictRaycastPosition, StartRaycastPosition, 0.5f), Vector3.down, out h2, 8f, ~PlayerMask))
                        {
                            StartRaycastPosition = h2.point;
                            StartRaycastPosition.y = predictRaycastPosition.y;
                        }
                        else
                        {
                            predictRaycastPosition = h2.point;
                            predictRaycastPosition.y = StartRaycastPosition.y;
                        }
                    }
                    
                    if(Physics.Raycast(StartRaycastPosition, Vector3.down, out h2, 8f, ~PlayerMask))
                    {
                        endPosition = h2.point + new Vector3(0, Luft, 0);
                        endRotation = Quaternion.FromToRotation(BodyAnchor.up, h2.normal) * BodyAnchor.rotation;
                    }
                }

                progress = 0f;
                pawCenter.PawsWalking[Index] = true;
                isMoving = true;
                Debug.DrawLine(endPosition, endPosition + new Vector3(0, 0.1f, 0), Color.yellow, 3f);
            }
            else
            {
                Inicialized = true;
                TargetTransform.position = h.point + new Vector3(0, Luft, 0);
            }
        }

        if (isMoving)
        {
            progress += Time.deltaTime * StepSpeed;

            Vector3 currentCenter = Vector3.Lerp(startPosition, endPosition, progress);
            
            TargetTransform.rotation = Quaternion.Slerp(startRotation, endRotation, progress);

            float height = Mathf.Sin(progress * Mathf.PI) * StepHeight;

            TargetTransform.position = new Vector3(currentCenter.x, currentCenter.y + height, currentCenter.z);

            if (progress >= 1f)
            {
                TargetTransform.position = endPosition;
                TargetTransform.rotation = endRotation;
                isMoving = false;
            }
            else if (progress >= 0.75f)
                pawCenter.PawsWalking[Index] = false;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(BodyAnchor.position, 0.2f);
    }
}