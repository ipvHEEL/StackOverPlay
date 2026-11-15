using UnityEngine;

public class ManipulatorView : MonoBehaviour
{
    public Transform Arm1;
    public Transform Arm2;
    public Transform Claw;

    public void SetProgress(float progress)
    {
        float angle1 = 107f + 30f * progress;
        float angle2 = 107f + 60f * progress;
        float angle3 = 120f - 40f * progress;

        //Arm1.localRotation = Quaternion.Euler(0f, 0f, angle);
        Arm1.localRotation = Quaternion.Euler(0f, 0f, angle1);
        Arm2.localRotation = Quaternion.Euler(0f, 0f, angle2);
        Claw.localRotation = Quaternion.Euler(0f, 0f, angle3);
    }
}