using UnityEngine;
using System.Collections;

public class Raptor : MonoBehaviour {

	//*** Movement Variables ***
	public Transform[] waypoints;
	protected int currentWaypointIndex;

	public float slowSpeed;
	public float fastSpeed;
	protected float moveSpeed;

	public float slowTurnSpeed;
	public float fastTurnSpeed;
    public float targetAttractionThreshold;
    public float targetAngleThreshold;
	protected float turnSpeed;

    public bool loopingBack = false;
    private Vector3 loopingTarget;

	protected Vector3 target;
	protected Vector3 avoidanceTarget;  

	public float waypointDistanceThreshold; //Minimum distance to a waypoint before the raptor picks a new one
	public float obstacleRayDistance; //Distance to check for obstacles in front of the raptor

	protected bool avoidingObstacle; //Used for when the raptor is avoiding an obstacle
	//*** Movement Variables ***

	//*** Gameplay Variables ***
	public float health;

    Quaternion lookDir;
    Vector3 relativePos;

	protected virtual void Move(){
		transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
		
		if (!avoidingObstacle)
		{
            if (loopingBack)
            {
                relativePos = loopingTarget - transform.position;
                if (Vector3.Distance(target, transform.position) > targetAttractionThreshold)
                {
                    loopingBack = false;
                    turnSpeed *= 3;
                }
            }
            if(!loopingBack)
            {
                relativePos = target - transform.position;
                if (Vector3.Distance(target, transform.position) < targetAttractionThreshold)
                {
                    turnSpeed *= 1.5f;
                    float angle = Vector3.Angle(transform.forward, relativePos);
                    if (angle > targetAngleThreshold)
                    {
                        loopingBack = true;
                        Vector3 perp = Vector3.Cross(transform.forward, relativePos);
                        float dir = Vector3.Dot(perp, transform.up);
                        if (dir > 0f)
                        {
                            Debug.Log("Right");
                            loopingTarget = transform.position - transform.right * targetAttractionThreshold;
                        }
                        else if (dir < 0f)
                        {
                            Debug.Log("Left");
                            loopingTarget = transform.position + transform.right * targetAttractionThreshold;
                        }
                    }
                }
            }
		}
		else
		{
			//If the Raptor is avoiding an obstacle, then move towards the avoidanceTarget
			relativePos = avoidanceTarget - transform.position;
		}
        if (!loopingBack)
        {
            lookDir = Quaternion.LookRotation(relativePos);
        }
        else
        {
            lookDir = Quaternion.LookRotation(relativePos);
        }
		transform.rotation = Quaternion.Slerp(transform.rotation, lookDir, turnSpeed * Time.deltaTime);
	}

    protected virtual void print()
    {
        Vector3 targetDir = target - transform.position;
        float angle = Vector3.Angle(transform.forward, targetDir);
        //Debug.Log(angle + " : " + (targetDir - new Vector3(180,180,180)).ToString() + " : " + targetDir);
    }

	protected void PickWaypoint(){
		bool hasFoundTarget = false;
		while (!hasFoundTarget)
		{
			//We used a newWaypointIndex variable to ensure the AI doesn't pick the same waypoint twice in a row
			int newWaypointIndex = Random.Range(0,waypoints.Length);
			if(newWaypointIndex == currentWaypointIndex)
			{
				continue;
			}

			target = waypoints[newWaypointIndex].position;
			currentWaypointIndex = newWaypointIndex;
			hasFoundTarget = true;
		}
	}

	public void ApplyDamage(int damage){
		health -= damage;
		if (health <= 0) {
			Destroy (gameObject);
		}
	}
}
