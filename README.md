# css451final
Scott Shirley @scottin3d
Ed Abshire @Thaldin

## Purpose  
A user interactable and defined solar system.  The project will consist of two scenes.  The first scene will be a user input screen, where the user will define the number of planets and moons in the solar system.  Once the user is done adding these details, they can click “Start” to begin the simulation.

## The Hierarchy  
The root of our hierarchy will be “the universe”.  This will allow us to add stars for the solar systems. The second generation will be the stars.  These stars will have multiple children in the form of planets that orbit the star. The third generation hierarchy planets will support moons.  The moons will be children of the planets and orbit them.

## The Cameras and Views  
The simulation will consist of two views. The two views will be the main screen view and a solar system view.

The main view will be the one the user can control to look around the solar system.  This view will leverage the camera abilities learned in MP4 (pan/tilt/tumble).  The user will be able to free cam around the system to look at all the planets.  In addition, the user will be able to select a single object by either clicking or selecting from a UI and focus the camera on it and either let the camera follow the planet or pan around it.

The solar system view will be the mini-viewport camera.  This is the overall view of the entire solar system for the user to show them the entire solar system.  See Figure 1 in the developer notes section.

## Program Flow
The program will launch with an input scene for the user to input data for the simulation.  Once the user has entered the data, this will be stored in a static object to pass to the simulation scene.  See Figure 2 in the developer notes section.
When the user selects “Start” the simulation scene will load and all the objects will be created at runtime.
##Stretch Goals
If we have time, we would also like to add the ability for the user to add and/or remove new objects in the solar system at runtime.  In addition, we would like to be able to support multiple solar systems if able.

##Tasks
- List of tasks to get started:
- Definite planet types
	- Associate textures to planet types
- Create static object to hold data for the simulation
	- Test data storage between scene
- Create user input scene
- Create simulation scene
	- Prototype creating and inserting objects dynamically
- Camera Follow
- Camera manipulation
- Object selection
- Globally keep track of selection
- Object interaction 
	- gravity or asteroid collision?

## Bug List
- ~~clicking into space does not unselect~~
	- ~~check raycast in maincontrollerinput~~
- rings not generating correctly