using UnityEngine;

public class xrotationTest : MonoBehaviour
{
    [Header("检测参数")]
    [Tooltip("用于发射射线的检测点，通常设置在角色脚底。若不指定则使用当前物体的位置。")]
    public Transform detectionPoint;
    [Tooltip("角色 Transform（用于获取 position 和 y rotation）")]
    public Transform player;
    
    [Tooltip("射线最大检测距离")]
    public float rayDistance = 2.0f;
    
    [Tooltip("左右检测点距离中心的偏移（控制检测区域宽度）")]
    public float lateralOffset = 0.5f;
    [Tooltip("前后检测点距离中心的偏移（控制检测区域深度）")]
    public float forwardOffset = 0.5f;
    
    [Tooltip("检测地面的 Layer")]
    public LayerMask groundLayer;

    [Header("旋转调整")]
    [Tooltip("旋转平滑速度")]
    public float adjustSpeed = 5.0f;

    void Update()
    {
        // 使该对象的位置始终跟随 player
        transform.position = player.position;
        // 将检测点放在物体下方（可根据需要调整偏移）
        detectionPoint.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
        
        // 使用检测点作为射线检测中心点
        Transform source = detectionPoint != null ? detectionPoint : transform;
        Vector3 centerPos = source.position;
        
        // 计算左右、前后四个检测点
        Vector3 leftPos  = centerPos - transform.right * lateralOffset;
        Vector3 rightPos = centerPos + transform.right * lateralOffset;
        Vector3 frontPos = centerPos + transform.forward * forwardOffset;
        Vector3 backPos  = centerPos - transform.forward * forwardOffset;
        
        // 发射 5 条射线（中心、左右、前、后）收集地面法线
        Vector3 sumNormals = Vector3.zero;
        int hitCount = 0;
        RaycastHit hit;
        
        if (Physics.Raycast(centerPos, Vector3.down, out hit, rayDistance, groundLayer))
        {
            sumNormals += hit.normal;
            hitCount++;
        }
        if (Physics.Raycast(leftPos, Vector3.down, out hit, rayDistance, groundLayer))
        {
            sumNormals += hit.normal;
            hitCount++;
        }
        if (Physics.Raycast(rightPos, Vector3.down, out hit, rayDistance, groundLayer))
        {
            sumNormals += hit.normal;
            hitCount++;
        }
        if (Physics.Raycast(frontPos, Vector3.down, out hit, rayDistance, groundLayer))
        {
            sumNormals += hit.normal;
            hitCount++;
        }
        if (Physics.Raycast(backPos, Vector3.down, out hit, rayDistance, groundLayer))
        {
            sumNormals += hit.normal;
            hitCount++;
        }
        
        // 如果至少有一条射线检测成功，则计算平均法线
        if (hitCount > 0)
        {
            Vector3 avgNormal = sumNormals / hitCount;
            
            // 计算当前对象的水平 forward（仅保留 x/z 分量），作为计算基础的前向
            Vector3 objectForward = transform.forward;
            //objectForward.y = 0;

                objectForward.Normalize();
            
            // 使用当前对象的水平 forward 与平均法线，计算出一个旋转（slopeRotation），
            // 该旋转的 x 与 z 分量反映地面坡度对物体倾斜（pitch/roll）的影响
            Quaternion slopeRotation = Quaternion.LookRotation(objectForward, avgNormal);
            Vector3 slopeEuler = slopeRotation.eulerAngles;
            
            // 获取 player 的 y 轴旋转（yaw）
            float playerYaw = player.eulerAngles.y;
            
            // 构造目标旋转：x 和 z 使用 slopeRotation 的欧拉值，y 则用 player 的 yaw
            Quaternion targetRotation = Quaternion.Euler(slopeEuler.x, playerYaw, slopeEuler.z);
            
            // 平滑过渡到目标旋转
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, adjustSpeed * Time.deltaTime);
        }
    }

    // 在 Scene 视图中绘制检测射线和 player forward 方向线，用于调试
    void OnDrawGizmos()
    {
        if (detectionPoint != null)
        {
            Vector3 center = detectionPoint.position;
            Vector3 left   = center - transform.right * lateralOffset;
            Vector3 right  = center + transform.right * lateralOffset;
            Vector3 front  = center + transform.forward * forwardOffset;
            Vector3 back   = center - transform.forward * forwardOffset;
            
            Gizmos.color = Color.red;
            Gizmos.DrawLine(center, center + Vector3.down * rayDistance);
            Gizmos.DrawLine(left,   left   + Vector3.down * rayDistance);
            Gizmos.DrawLine(right,  right  + Vector3.down * rayDistance);
            Gizmos.DrawLine(front,  front  + Vector3.down * rayDistance);
            Gizmos.DrawLine(back,   back   + Vector3.down * rayDistance);
        }
        
        if (player != null)
        {
            // 绘制 player 的 forward 方向（绿色，仅水平分量）
            Vector3 playerForward = this.transform.forward;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(this.transform.position, this.transform.position + this.transform.forward * 2.0f);
        }
        
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.up * 2.0f);
    }
}
