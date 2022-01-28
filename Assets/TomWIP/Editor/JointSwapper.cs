using UnityEditor;
using UnityEngine;

public class JointSwapper
{
    [MenuItem("Tom/Joints/Hinge to configurable")]
    public static void HingeToConfigurable()
    {
        foreach (var obj in Selection.objects)
        {
            var go = obj as GameObject;
            if (go == null)
            {
                continue;
            }

            var hinge = go.GetComponent<HingeJoint>();
            // cache values
            var connectedBody = hinge.connectedBody;
            var connectedAnchor = hinge.connectedAnchor;
            var axis = hinge.axis;
            
            Object.DestroyImmediate(hinge);
            var joint = go.AddComponent<ConfigurableJoint>();
            joint.connectedBody = connectedBody;
            joint.connectedAnchor = connectedAnchor;
            joint.axis = connectedAnchor;
            joint.projectionMode = JointProjectionMode.PositionAndRotation;
            joint.projectionAngle = float.PositiveInfinity;
            joint.projectionDistance = 0f;
            joint.xMotion = ConfigurableJointMotion.Locked;
            joint.yMotion = ConfigurableJointMotion.Locked;
            joint.zMotion = ConfigurableJointMotion.Locked;
        }
    }
}