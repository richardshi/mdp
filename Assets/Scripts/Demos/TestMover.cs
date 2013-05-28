using UnityEngine;
using System.Collections;
using Vector2 = Microsoft.Xna.Framework.FVector2;

public class TestMover : MonoBehaviour {
	
	public static int collisionCounter = 0;
	private float collisionTime = 0;
	
    public float Speed;

	// Use this for initialization
	void Start () {
        FSBodyComponent bodyComponent = GetComponent<FSBodyComponent>();

        float xDirection = MDPUtility.RandomFloat() + 0.01f;
        float xSign = MDPUtility.RandomInt(2) == 1 ? 1 : -1;
        float yDirection = MDPUtility.RandomFloat() + 0.01f;
        float ySign = MDPUtility.RandomInt(2) == 1 ? 1 : -1;

        Vector2 velocity = new Vector2(xSign * xDirection, ySign * yDirection);
        velocity *= Speed;

        bodyComponent.PhysicsBody.LinearVelocity = velocity;
        bodyComponent.PhysicsBody.LinearDamping = 0f;
		//bodyComponent.PhysicsBody.
		
		//bodyComponent.PhysicsBody.
	}
	
/*	void Update(){
	collisionTime +=  Time.deltaTime;
		
		if (collisionCounter == 1 && gameObject.tag == "Enz")
		{	
			Transform.Destroy(gameObject);
			Buttons.EnzymesExist = false;
			collisionCounter = 0;
		}
	}*/
	
}
