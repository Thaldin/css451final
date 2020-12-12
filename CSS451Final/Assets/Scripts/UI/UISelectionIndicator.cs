using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// This class was intended to move a UI seletor to the current selection
/// and have the info panel float with it, however due to how we render 
/// objects with the 451shader and M4x4 the position matching was off 
/// by a slight value and I did not have time to figure it out.  Keeping
/// if there is time in the future because its a cool idea.
/// </summary>
public class UISelectionIndicator : MonoBehaviour {
	//MouseManager mm;
	public GameObject center = null;
	public Vector3 c = Vector3.zero;
	public float selectionPadding = 1.1f;
	[SerializeField] float radius = 0;

	[SerializeField] GameObject[] verts;
	[SerializeField] Vector3[] screenSpaceCorners;
	public Vector3[] worldPoints;


	PlanetInfo planetInfo;
	public TMPro.TextMeshProUGUI planetName = null;
	public TMPro.TextMeshProUGUI distanceFromSun = null;
	public TMPro.TextMeshProUGUI orbitalPeriod = null;
	public TMPro.TextMeshProUGUI diameter = null;
	public TMPro.TextMeshProUGUI rotationalPeriod = null;
	public TMPro.TextMeshProUGUI axisTilt = null;
	public TMPro.TextMeshProUGUI moons = null;
	public TMPro.TextMeshProUGUI rings = null;

	// current selection showing info
	Transform selectedObject = null;
	// Use this for initialization
	void Awake () {
		//mm = GameObject.FindObjectOfType<MouseManager>();
		verts = new GameObject[8];
		screenSpaceCorners = new Vector3[8];
		worldPoints = new Vector3[8];

		transform.gameObject.SetActive(false);
		for (int i = 0; i < this.transform.childCount; i++) {
			transform.GetChild(i).gameObject.SetActive(false);
		}
	}

	// TODO Planet Indicators 

	public void SetSelection(Transform t, PlanetInfo pInfo) {
		selectedObject = t;
		if (selectedObject != null) {


			planetName.text = t.name;
			distanceFromSun.text = pInfo.distanceFromSun.ToString() + "000000 km";
			orbitalPeriod.text = pInfo.orbitalPeriod.ToString() + " days";
			diameter.text = pInfo.planetDiameter.ToString() + " km";
			rotationalPeriod.text = pInfo.rotationPeriod.ToString() + " hours";
			axisTilt.text = pInfo.axisTilt.ToString() + " degrees";
			moons.text = pInfo.moonCount.ToString();
			rings.text = pInfo.ringCount.ToString();


			transform.gameObject.SetActive(true);
			for (int i = 0; i < this.transform.childCount; i++) {
				transform.GetChild(i).gameObject.SetActive(true);
			}
			/*
			center.SetActive(true);
			radius = selectedObject.GetComponent<SphereCollider>().radius;
			center.transform.localScale *= radius;
			center.transform.position = selectedObject.transform.position;
			*/


		} else {
			transform.gameObject.SetActive(false);
			for (int i = 0; i < this.transform.childCount; i++) {
				transform.GetChild(i).gameObject.SetActive(false);
			}
		}
	}

	/*
	// Update is called once per frame
	void FixedUpdate () {
		if(selectedObject != null) {

			List<string> l = selectedObject.GetComponent<SceneNode>().GetPlanetInfo();
			name.text = l[0];
			distanceFromSun.text = l[1];
			orbitalPeriod.text = [2];
			diameter.text = l[3];
			rotationalPeriod.text = l[4];

			if (selectedObject.tag != "moon") {
				moons.text = l[5];
			}


			for (int i = 0; i < this.transform.childCount; i++) {
				transform.GetChild(i).gameObject.SetActive(true);
			}
			
			center.SetActive(true);
			radius = selectedObject.GetComponent<SphereCollider>().radius;
			center.transform.localScale *= radius;
			center.transform.position = selectedObject.transform.position;
			


		}
		else {
			for (int i = 0; i < transform.childCount; i++) {
				transform.GetChild(i).gameObject.SetActive(false);
			}
			center.SetActive(false);

		}
	}
	*/
	
	 Rect TransformBoundsInScreenSpace(Transform t, float r) {
		// This is the space occupied by the object's visuals
		// in WORLD space.
		c = t.position;
		center.transform.position = c;
		

		//bottom
		worldPoints[0] = new Vector3(c.x - r, c.y - r, c.z - r);
		worldPoints[1] = new Vector3(c.x + r, c.y - r, c.z - r);
		worldPoints[2] = new Vector3(c.x - r, c.y - r, c.z + r);
		worldPoints[3] = new Vector3(c.x + r, c.y - r, c.z + r);

		//top
		worldPoints[4] = new Vector3(c.x - r, c.y + r, c.z - r);
		worldPoints[5] = new Vector3(c.x + r, c.y + r, c.z - r);
		worldPoints[6] = new Vector3(c.x - r, c.y + r, c.z + r);
		worldPoints[7] = new Vector3(c.x + r, c.y + r, c.z + r);

		foreach (var g in verts) {
			Destroy(g);
		}

		//debug
		int y = 0;
		foreach (var p in worldPoints) {
			GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			go.transform.position = p;
			go.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
			verts[y] = go;
			y++;
		}

		#region debug lines
		Debug.DrawLine(worldPoints[0], worldPoints[4], Color.white);
		Debug.DrawLine(worldPoints[3], worldPoints[7], Color.white);
		Debug.DrawLine(worldPoints[2], worldPoints[6], Color.white);
		Debug.DrawLine(worldPoints[1], worldPoints[5], Color.white);

		Debug.DrawLine(worldPoints[0], worldPoints[3], Color.white);
		Debug.DrawLine(worldPoints[3], worldPoints[2], Color.white);
		Debug.DrawLine(worldPoints[2], worldPoints[1], Color.white);
		Debug.DrawLine(worldPoints[1], worldPoints[0], Color.white);

		Debug.DrawLine(worldPoints[4], worldPoints[7], Color.white);
		Debug.DrawLine(worldPoints[7], worldPoints[6], Color.white);
		Debug.DrawLine(worldPoints[6], worldPoints[5], Color.white);
		Debug.DrawLine(worldPoints[5], worldPoints[4], Color.white);

		#endregion

		Camera theCamera = Camera.main;

		// For each of the 8 corners of our renderer's world space bounding box,
		// convert those corners into screen space.
		//bottom
		screenSpaceCorners[0] = theCamera.WorldToScreenPoint(worldPoints[0]);
		screenSpaceCorners[1] = theCamera.WorldToScreenPoint(worldPoints[0]);
		screenSpaceCorners[2] = theCamera.WorldToScreenPoint(worldPoints[0]);
		screenSpaceCorners[3] = theCamera.WorldToScreenPoint(worldPoints[0]);
		screenSpaceCorners[4] = theCamera.WorldToScreenPoint(worldPoints[0]);
		screenSpaceCorners[5] = theCamera.WorldToScreenPoint(worldPoints[0]);
		screenSpaceCorners[6] = theCamera.WorldToScreenPoint(worldPoints[0]);
		screenSpaceCorners[7] = theCamera.WorldToScreenPoint(worldPoints[0]);


		// Now find the min/max X & Y of these screen space corners.
		float min_x = screenSpaceCorners[0].x;
		float min_y = screenSpaceCorners[0].y;
		float max_x = screenSpaceCorners[0].x;
		float max_y = screenSpaceCorners[0].y;

		for (int i = 1; i < 8; i++) {
			if(screenSpaceCorners[i].x < min_x) {
				min_x = screenSpaceCorners[i].x;
			}
			if(screenSpaceCorners[i].y < min_y) {
				min_y = screenSpaceCorners[i].y;
			}
			if(screenSpaceCorners[i].x > max_x) {
				max_x = screenSpaceCorners[i].x;
			}
			if(screenSpaceCorners[i].y > max_y) {
				max_y = screenSpaceCorners[i].y;
			}
		}
		 
		Debug.Log("xmin: " + min_x + "ymin: " + min_y + "xmax: " + max_x + "ymax: " + max_y);
		return Rect.MinMaxRect( min_x, min_y, max_x, max_y );

	}
}
