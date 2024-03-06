using ASD;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class FootIKCharacter : MonoBehaviour
{
    [SerializeField] private float m_LevitationForce = 5.0f;
    [SerializeField] private float m_LevitationDamp = 0.4f;
    [SerializeField] private float m_ClampVelocityY = 0.5f;
    [SerializeField] private float m_LevitationOffset = 0.1f;
    [SerializeField] private float m_MaxStepHeight = 0.45f;
    [SerializeField] private float m_UnitRadius = 0.4f;

    [SerializeField] private LayerMask ignoreLayers;
    [SerializeField] private float m_Drag = 5;

    private Rigidbody m_RB;
    private FootControllerIK m_FIK;
    private Vector3 m_MoveDirection;

    public bool OnGround { get; private set; }
    public bool isMoving { get; private set; }
    private void Levitation()
    {
        bool hovering;
        float hitDistance = 0;

        Vector3 origin = transform.position + transform.up * (m_MaxStepHeight + 0.05f);

        float rigidbodyVelocityY = Vector3.Dot(m_RB.velocity, Physics.gravity);

        if (rigidbodyVelocityY < m_ClampVelocityY)
            OnGround = true;
        else
            OnGround = false;

        if (Physics.Raycast(origin, -transform.up, out RaycastHit hit, m_MaxStepHeight + m_LevitationOffset + 0.05f, ~ignoreLayers))
        {
            hovering = true;
            hitDistance = hit.distance;
        }
        else if (Physics.SphereCast(origin, m_UnitRadius / 2, -Vector3.up, out hit, m_MaxStepHeight + m_LevitationOffset + 0.05f, ~ignoreLayers))
        {
            hovering = true;

            hit.point = new Vector3(transform.position.x, hit.point.y, transform.position.z);
            hitDistance = Vector3.Distance(origin, hit.point);
        }
        else
        {
            hovering = false;
            OnGround = false;
        }

        if (m_FIK && m_FIK.CanReachTargets && hovering)
        {
            if (isMoving)
                hit.point = new Vector3(hit.point.x, m_FIK.DirectionalFootHeight(m_MoveDirection).y, hit.point.z);
            else
                hit.point = new Vector3(hit.point.x, m_FIK.LowestFootHeight, hit.point.z);

            hitDistance = Vector3.Distance(origin, hit.point);
        }

        if (hovering)
        {
            float hoverError = m_MaxStepHeight + m_LevitationOffset - hitDistance;

            if (hoverError > 0)
            {
                float upwardSpeed = m_RB.velocity.y;
                float lift = hoverError * m_LevitationForce - upwardSpeed * m_LevitationDamp;
                m_RB.AddForce(lift * transform.up, ForceMode.VelocityChange);
            }
        }

        if (OnGround == false) m_RB.drag = 0;
        else m_RB.drag = m_Drag;
    }
}
