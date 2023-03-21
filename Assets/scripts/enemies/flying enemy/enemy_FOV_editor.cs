using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
// [CustomEditor(typeof(enemy_FOV))]
// public class enemy_FOV_editor : Editor
// {
//     private void OnSceneGUI()
//     {
//         enemy_FOV fov = (enemy_FOV)target;
//         Handles.color = Color.white;
//         Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.radius);

//         Vector3 viewAngle01 = DirectionFromAngle(fov.transform.eulerAngles.y, -fov.angle / 2);
//         Vector3 viewAngle02 = DirectionFromAngle(fov.transform.eulerAngles.y, fov.angle / 2);

//         Handles.color = Color.yellow;
//         Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngle01 * fov.radius);
//         Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngle02 * fov.radius);

//         if (fov.canSeeTarget)
//         {
//             Handles.color = Color.green;
//             Handles.DrawLine(fov.transform.position, fov.TargetRef.transform.position);
//         }
//     }

//     private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
//     {
//         angleInDegrees += eulerY;

//         return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
//     }
// }
