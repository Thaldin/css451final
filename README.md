# css451final
Scott Shirley [@scottin3d](https://github.com/scottin3d)  
Ed Abshire [@Thaldin](https://github.com/Thaldin)  

Project Repository: https://github.com/Thaldin/css451final  

# About 
This project is a culmanation of the topics covered in CSS451:
- Graphical User Interfaces
- Model / View / Controller pattern
- Vertor and Maxtrix4x4 mathematics
- Hierarchical Modeling
- Camera / View / Projection Modeling
- Indexed Polygon and Polygon Modeling
- Texture Mapping / Placement / Diffuse Illumination
- Input and Simple Physics

## Purpose    
A user interactable and defined solar system.  The project will consist of two scenes.  The first scene will be a user input screen, where the user will define the number of planets and moons in the solar system.  Once the user is done adding these details, they can click “Start” to begin the simulation.

## The Hierarchy    
The root of our hierarchy will be “the universe”.  This will allow us to add stars for the solar systems. The second generation will be the stars.  These stars will have multiple children in the form of planets that orbit the star. The third generation hierarchy planets will support moons.  The moons will be children of the planets and orbit them.  

The hierarchy for this project is not the same as Unity's scene hierarchy.  Every rendered object in this scene is a `SceneNode` located at the world origin (0, 0, 0).  The placement is done by offsetting the vertices using a Matrix4x4 and passing it to a `Primitive` where it is sent to the shader.  Therefore each rendered object in the scene has two objects, a `SceneNode` and a `Primitive`.  

There are a few important functions used to decompose a Unity Matrix4x4.  They are:
- Extract world position  
`Vector 3 worldPostion = new Vector3(m4x4.m03, m4x4.m13, m4x4.m23);`
- Extract new local position  
`Vector3 localPosition = m4x4.GetColumn(3);`
- Extract new local rotation  
`Quaternion rotation = Quaternion.LookRotation(m4x4.GetColumn(2), m4x4.GetColumn(1));`
- Extract new local scale  
`Vector3 scale = new Vector3(m4x4.GetColumn(0).magnitude,m4x4.GetColumn(1).magnitude, m4x4.GetColumn(2).magnitude);`

Each node in the hierarchy inherits its parent's Matrix4x4, like Unity's scene hierarchy.  This means, that the children of a `SceneNode` will inherit the offsets applied to it.

### CompositeXform
```C#
public void CompositeXform(ref Matrix4x4 parentXform, ref List<Matrix4x4> sceneM4x4) {
        if (np != null) {
			// --Offsets--
			// Rotation offset
            // check for 0
            float v = (orbitalPeriod == 0f) ? 0f : (OP_STD / orbitalPeriod); // * timeScale
            yAngle = (yAngle <= 360f) ? yAngle + v * Time.deltaTime : 0f;
            Matrix4x4 rotOffset = Matrix4x4.Rotate(Quaternion.Euler(axisTilt, yAngle, 0f));
			
			// Position offset
            Matrix4x4 posOffset = Matrix4x4.Translate(planetOrigin);
			
			// Matrix TRS
            Matrix4x4 trs = Matrix4x4.TRS(transform.localPosition, 
										  transform.localRotation, 
										  transform.localScale);

			// --Combination--
            mCombinedParentXform = parentXform * posOffset * rotOffset * trs;

            // propagate to all children
            foreach (Transform child in transform) {
                SceneNode cn = child.GetComponent<SceneNode>();
                if (cn != null) {
                    cn.CompositeXform(ref mCombinedParentXform, ref sceneM4x4);
                }
            }

            // disenminate to primitives for shader
            sceneM4x4.Add(np.LoadShaderMatrix(ref mCombinedParentXform));
			
			// update orbit ring of children
            if (transform.CompareTag("moon")) {
                UpdateRing();
            }
        }
    }
```

Additional offsets are applied to 

### LoadShaderMatrix
```C# {.line-numbers}
public Matrix4x4 LoadShaderMatrix(ref Matrix4x4 nodeMatrix) {
        // apply local roation
        // object rotaiton
        yAngle = (yAngle <= 360f) ? yAngle + (SIDEREAL_ROTATIONAL_PERIOD / planetRotation) * timeScale : 0f;
        Matrix4x4 rot = Matrix4x4.Rotate(Quaternion.Euler(0f, yAngle, 0f));
        // apply local scale
        // object diameter
        Matrix4x4 scale = Matrix4x4.Scale(new Vector3(planetDiameter / EMI, 
													  planetDiameter / EMI, 
													  planetDiameter / EMI));
        // apply pivot
        // object distance from center of system
        Vector3 pos = new Vector3((distanceFromSun / SUN_DISTANCE_SCALER) * systemScale, 0f, 0f);
        Matrix4x4 orgT = Matrix4x4.Translate(pos);

        // apply offset
		// this is for a moon of an object
        Matrix4x4 offsetT = Matrix4x4.Translate(offsetFromPlanet);

        // apply trs of obj
        Matrix4x4 trs = Matrix4x4.TRS(transform.localPosition, 
									  transform.localRotation, 
									  transform.localScale);
		
        // combine trs/ rotation/ sclae/ pivot to matrix
        Matrix4x4 m = nodeMatrix * orgT * trs * rot * scale * offsetT;
		
        // send to shader
        GetComponent<MeshRenderer>().material.SetMatrix("MyXformMat", m);
        GetComponent<MeshRenderer>().material.SetTexture("_MainTex", mainText);
		
        // return 4x4
        return m;
    }
```

## Interaction  
A master list, `List<Matrix4x4> m4x4s`, of all scene nodes was kept in the core logic of the scene.  This list was updated every frame when the Matrix4x4 was returned after calculated for the shader.  With a master list of object positions in the world, we could either pass the `List` to another class that wanted to interact with the objects, or retieve a position by index.  These interactions were delt with using `Event`s so that a script or an object did not have to reference the core logic itself.  

An example of this is the `AsteroidSpawner` in the scene.  When it "fires" an `Asteroid` object, it invokes the `OnFire` event in which the the `Projectile` and core logic subscript to where it sends the ID of the `Projectile` and the current target.  This triggers the `Projectile` to invoke another event to that gets the position of its target every frame from the core logic. They psotions are all calculated using the functions described [in the hierarchy](#the-hierarchy).  

## The Cameras and Views    
The simulation has three camera views.  

The main view the user can control to look around the solar system.  This view will leverage the camera abilities learned in MP4 (pan/tilt/tumble).  The user can free cam around the system to look at all the planets.  In addition, the user can select a single object by either clicking or selecting from a UI and focus the secondary camera on it.  

The second view is a  mini-viewport camera.  This camera appears when a `SceneNode` is selected by either the user clicking on it in the scene or selecting it from the UI drop menu.  

The third view is another mini-viewport camera.  This camera appears after the user presses the fire key for the Star Killer in the scene.  This camera gives the user a secondard look at the animation playing in the scene.  

## Stretch Goals  
If we have time, we would also like to add the ability for the user to add and/or remove new objects in the solar system at runtime.  In addition, we would like to be able to support multiple solar systems if able.

## Tasks  
- ~~Normal Uodate menu controls~~
- Required	TODO Demo
- Normal	TODO Update rings when objects are moved with xForm Controller
- ~~Required	TODO Camera behavious axis												CameraBehavious.cs 99~~
- ~~Normal 	TODO Illumination shader~~
- ~~Normal	TODO: if you hit resume, it does not set pauseMenuIsOn in this scope	MainControllerInput.cs	88~~
- ~~Normal	TODO Camera Manipulation~~		
- Mini cam click										MainControllerInput.cs	44  
- ~~Normal	TODO: Fix Saturns Ring renderer~~
- ~~Normal	TODO: spane Luna's orbit rings~~

- ~~List of tasks to get started:~~
- ~~Definite planet types~~
	- ~~Associate textures to planet types~~
- Create static object to hold data for the simulation
	- Test data storage between scene
- Create user input scene
- ~~Create simulation scene~~
	- Prototype creating and inserting objects dynamically
- ~~Camera Follow~~
- Camera manipulation
- ~~Object selection~~
- ~~Globally keep track of selection~~
- ~~Object interaction~~
	- gravity or asteroid collision?

## Bug List
- ~~projectile target changes~~
- ~~death star projectiles not destroying~~
- ~~Normal Auido lister~~
- ~~Camera does not pane screen left of right correctly~~
- ~~clicking into space does not unselect~~
	- ~~check raycast in maincontrollerinput~~
- ~~rings not generating correctly~~