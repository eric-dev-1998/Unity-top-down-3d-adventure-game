using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    public Transform focus_target;
    public Vector3 offset;

    public bool showDebugRay = false;
    public bool inPosition = false;

    public float followSpeed = 0.5f;
    public float rotationSpeed = 0.5f;
    public float startAngle = 45f;
    public float maxVerticalAngle = 75f;
    public float minPlayerDistanceToAdjustCameraAngle = 3f;

    float targetZOffset = 0f;

    Vector3 velocity = Vector3.zero;

    private void Start()
    {
        ResetFocusTarget();
        SnapToPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        followPlayer();
        LookAtPlayer();
    }

    void followPlayer()
    {
        Vector3 targetPos = new Vector3(focus_target.position.x + offset.x, focus_target.position.y + offset.y, focus_target.position.z - targetZOffset);
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * followSpeed);

        if (Vector3.Distance(transform.position, targetPos) <= 0.25f)
            inPosition = true;
        else
            inPosition = false;
    }

    void LookAtPlayer()
    {
        if(showDebugRay)
            Debug.DrawRay(focus_target.position, new Vector3(0, 0, -minPlayerDistanceToAdjustCameraAngle), Color.blue);

        float targetAngle = 0;
        RaycastHit hit;

        if (Physics.Raycast(focus_target.position, new Vector3(0, 0, -1), out hit, minPlayerDistanceToAdjustCameraAngle))
        {
            if (hit.transform.tag == "Solid_High")
            {
                targetZOffset = hit.distance;

                float hipo = Mathf.Sqrt(targetZOffset * targetZOffset + offset.y * offset.y);
                targetAngle = 90 - Mathf.Sin(hit.distance / hipo) * Mathf.Rad2Deg;
            }
            else
            {
                targetAngle = startAngle;
                targetZOffset = offset.z;
            }
        }
        else
        {
            targetAngle = startAngle;
            targetZOffset = offset.z;
        }

        //targetAngle = limitMaxAngle(targetAngle);
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(targetAngle, 0, 0), Time.deltaTime * rotationSpeed);
    }

    public void SnapToPlayer()
    {
        // Snap position.
        Vector3 targetPos = new Vector3(focus_target.position.x + offset.x, focus_target.position.y + offset.y, focus_target.position.z - offset.z);
        transform.position = targetPos;

        // Snap rotation.
        transform.eulerAngles = new Vector3(startAngle, 0, 0);
    }

    float limitMaxAngle(float a)
    {
        if (a >= 70)
            a = 70;

        return a;
    }

    public void SwitchFocusTarget(string newTarget)
    {
        try
        {
            focus_target = GameObject.Find(newTarget).transform;
        }
        catch (Exception e)
        {
            Debug.LogErrorFormat("Error finding new camera target '{0}':\n{1}", newTarget, e);
        }
    }

    public void ResetFocusTarget()
    {
        focus_target = GameObject.Find("Player").transform;
    }
}
