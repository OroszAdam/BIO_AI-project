using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class FloaterAssigner : MonoBehaviour
{
    [Header("Z-floating")]
    public float OffsetPerItem = 30.0f;
    public float AmplitudeModifierOfItems = 0.2f;

    [Header("Rotation")]
    public bool Rotate = false;
    public Vector3 RotationSpeed = new Vector3(0.0f, 0.0f, 1.0f);

    void Start()
    {
        int i = 0;

        // Add the Floater.cs script to all top level children
        foreach (Transform child in transform)
        {
            if (child != transform)
            {
                Floater childFloater = child.gameObject.AddComponent<Floater>();
                childFloater.StartOffset = OffsetPerItem * i;
                childFloater.AmplitudeModifier = AmplitudeModifierOfItems;

                childFloater.Rotate = Rotate;
                childFloater.RotationSpeed = RotationSpeed;

                i++;
            }
        }
    }
}
