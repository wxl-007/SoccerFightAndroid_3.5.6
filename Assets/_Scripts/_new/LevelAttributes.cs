/*
Maintaince Logs:
2010-11-22    Waigo    Initial version.  Players controller.
2011-10-21    Waigo    Initial version.  Copy from Robot Bros. Setup the bounds of scene.
                                     

*/



using UnityEngine;
using System.Collections;

public class LevelAttributes : MonoBehaviour
{
// Size of the level
	public Rect bounds;
	public float fallOutBuffer = 5.0f;
	public float colliderThickness = 20.0f;
	

// Sea Green For the Win!
	private Color sceneViewDisplayColor = new Color (0.20f, 0.74f, 0.27f, 0.50f);

	private static LevelAttributes instance;
	
	public static LevelAttributes GetInstance ()
	{
		if (!instance) {
			instance = FindObjectOfType(typeof(LevelAttributes)) as LevelAttributes;
			if (!instance)
				Debug.LogError ("There needs to be one active LevelAttributes script on a GameObject in your scene.");
		}
		return instance;
	}

	void OnDisable ()
	{
		instance = null;
	}

	void OnDrawGizmos ()
	{
		Gizmos.color = sceneViewDisplayColor;
		Vector3 lowerLeft = new Vector3 (bounds.xMin, bounds.yMax, 0);
		Vector3 upperLeft = new Vector3 (bounds.xMin, bounds.yMin, 0);
		Vector3 lowerRight = new Vector3 (bounds.xMax, bounds.yMax, 0);
		Vector3 upperRight = new Vector3 (bounds.xMax, bounds.yMin, 0);
		
		Gizmos.DrawLine (lowerLeft, upperLeft);
		Gizmos.DrawLine (upperLeft, upperRight);
		Gizmos.DrawLine (upperRight, lowerRight);
		Gizmos.DrawLine (lowerRight, lowerLeft);
	}

	void Start ()
	{
		GameObject createdBoundaries = new GameObject ("Created Boundaries");
		createdBoundaries.transform.parent = transform;
		
		GameObject leftBoundary = new GameObject ("Left Boundary");
		leftBoundary.transform.parent = createdBoundaries.transform;
		BoxCollider boxCollider = leftBoundary.AddComponent<BoxCollider>();
		boxCollider.size = new Vector3 (colliderThickness, bounds.height + colliderThickness * 2.0f + fallOutBuffer, colliderThickness);
		boxCollider.center = new Vector3 (bounds.xMin - colliderThickness * 0.5f, bounds.y + bounds.height * 0.5f - fallOutBuffer * 0.5f, 0.0f);
		boxCollider.tag = "_Boundary";
		DrawABlackPlane(leftBoundary, boxCollider.bounds);
		
		//GameObject  leftBoundaryBlackPlane =  Instantiate(blackPlane, boxCollider.center, blackPlane.transform.rotation) as GameObject;
		//leftBoundaryBlackPlane.transform.parent = leftBoundary.transform;
		
		
		GameObject rightBoundary = new GameObject ("Right Boundary");
		rightBoundary.transform.parent = createdBoundaries.transform;
		boxCollider = rightBoundary.AddComponent<BoxCollider>();
		boxCollider.tag = "_Boundary";
		boxCollider.size = new Vector3 (colliderThickness, bounds.height + colliderThickness * 2.0f + fallOutBuffer, colliderThickness);
		boxCollider.center = new Vector3 (bounds.xMax + colliderThickness * 0.5f, bounds.y + bounds.height * 0.5f - fallOutBuffer * 0.5f, 0.0f);
		DrawABlackPlane(rightBoundary, boxCollider.bounds);
		
		GameObject topBoundary = new GameObject ("Top Boundary");
		topBoundary.transform.parent = createdBoundaries.transform;
		boxCollider = topBoundary.AddComponent<BoxCollider>();
		boxCollider.size = new Vector3 (bounds.width + colliderThickness * 2.0f, colliderThickness, colliderThickness);
		boxCollider.center = new Vector3 (bounds.x + bounds.width * 0.5f, bounds.yMax + colliderThickness * 0.5f, 0.0f);
		// Don't draw the top black plane
		//DrawABlackPlane(topBoundary, boxCollider.bounds);
		
		GameObject bottomBoundary = new GameObject ("Bottom Boundary (Including Fallout Buffer)");
		bottomBoundary.transform.parent = createdBoundaries.transform;
		boxCollider = bottomBoundary.AddComponent<BoxCollider>();
		boxCollider.tag = "_Boundary";
		boxCollider.size = new Vector3 (bounds.width + colliderThickness * 2.0f, colliderThickness, colliderThickness);
		boxCollider.center = new Vector3 (bounds.x + bounds.width * 0.5f, bounds.yMin - colliderThickness * 0.5f - fallOutBuffer, 0.0f);
		
		Bounds bottomBounds = boxCollider.bounds;
		bottomBounds.size = new Vector3 (bounds.width + colliderThickness * 2.0f, colliderThickness + fallOutBuffer, colliderThickness);
		bottomBounds.center = new Vector3 (bounds.x + bounds.width * 0.5f, bounds.yMin - (colliderThickness+ fallOutBuffer) * 0.5f, 0.0f);
		
		
		DrawABlackPlane(bottomBoundary, bottomBounds);
		//Invoke("DelayActiveDeadZone",1f);
		
		//DrawBlackBoundsPlanes();
	}
	


	void DrawBlackBoundsPlanes(){
		//DrawABlackPlane(new Rect(-10, -10, 20, 10));
	}

	void DrawABlackPlane(GameObject gObj, Bounds planeBounds){
		return;
		Mesh modelMesh;
		MeshFilter meshFilter;
		MeshRenderer meshRenderer;
		Material colorMaterial;
		modelMesh = new Mesh();
		modelMesh.name = "PlaneMesh";
		modelMesh.Clear();
		
		Vector3[] Vertices = new Vector3[4];
		Vector2[] UVs = new Vector2[4];
		int[] Tris = new int[6];
		
		modelMesh.vertices = new Vector3[4]; 
		modelMesh.uv = new Vector2[ 4 ]; 
		modelMesh.triangles = new int[ 6 ];
		
		/*
		Vertices[0] = new Vector3( -1.0f, 10.0f, -5.0f ); 
		Vertices[1] = new Vector3( 10.0f, 10.0f, -5.0f ); 
		Vertices[2] = new Vector3( -1.0f, 0.0f, -5.0f ); 
		Vertices[3] = new Vector3( 10.0f, 0.0f, -5.0f );
		*/
		
		Vertices[0] = new Vector3( planeBounds.min.x, planeBounds.min.y, -1.01f ); 
		Vertices[1] = new Vector3( planeBounds.max.x, planeBounds.min.y, -1.01f ); 
		Vertices[2] = new Vector3( planeBounds.min.x, planeBounds.max.y, -1.01f ); 
		Vertices[3] = new Vector3( planeBounds.max.x, planeBounds.max.y, -1.01f );
		
		UVs[0] = new Vector2( 0.001f, 0.001f ); 
		UVs[1] = new Vector2( 0.999f, 0.001f ); 
		UVs[2] = new Vector2( 0.001f, 0.999f ); 
		UVs[3] = new Vector2( 0.999f, 0.999f );
		
		// First Face      then opposite side   by Waigo
		Tris[0] = 0; Tris[1] = 2; Tris[2] = 1; 
		// Second Face 
		Tris[3] = 1; Tris[4] = 2; Tris[5] = 3;
		
		modelMesh.vertices = Vertices; 
		modelMesh.uv = UVs; 
		modelMesh.triangles = Tris;
		
		modelMesh.RecalculateNormals();
		modelMesh.RecalculateBounds();
		meshFilter = (MeshFilter) gObj.AddComponent(typeof(MeshFilter)); 
		meshFilter.mesh = modelMesh; 
		meshRenderer = (MeshRenderer) gObj.AddComponent(typeof(MeshRenderer));
		
		colorMaterial = null;
		if( colorMaterial ) { 
			meshRenderer.renderer.material = colorMaterial;		} else meshRenderer.renderer.material.color = new Color(0f,0f,0f);



	}


	void DelayActiveDeadZone(){
		gameObject.SetActiveRecursively(true);
	}
}
