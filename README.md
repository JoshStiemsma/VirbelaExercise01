# VirbelaExercise01



Unity Version: 2021.3.6f1

Plugins
  Zenject/Extenject
  
Includes:
  Unity Testing with Dependecy Injection
  Multiple Selectable Distance Calculation
  Realtime Editor features
  Save/Load via Json
  XML documentation 
  

**Assignment:**

Exercise 1
In this exercise you'll configure a Unity scene and write scripts to create an interactive experience. As you progress through the steps, feel free to add comments to the code about why you choose to do things a certain way. Add comments if you felt like there's a better, but more time intensive way to implement specific functionality. It's OK to be more verbose in your comments than typical, to give us a better idea of your thoughts when writing the code.

What you need
Unity 2020 (latest, or whatever you have already)
IDE of your choice
Git
Instructions
This test is broken into multiple phases. You can implement one phase at a time or all phases at once, whatever you find to be best for you.

Phase 1
Project setup:

Create a new Unity project inside this directory, put "Virbela" and your name in the project name.
Configure the scene:
Add a central object named "Player"
Add 5 objects named "Item", randomly distributed around the central object
Add two C# scripts named "Player" and "Item" to your project
Attach the scripts to the objects in the scene according to their name, Item script goes on Item objects, Player script goes on Player object.
You may use these scripts or ignore them when pursuing the Functional Goals, the choice is yours. You're free to add any additional scripts you require to meet the functional goals.
Functional Goal 1:

When the game is running, make the Item closest to Player turn red. One and only one Item is red at a time. Ensure that when Player is moved around in the scene manually (by dragging the object in the scene view), the closest Item is always red.

Phase 2
Project modification:

Add 5 objects randomly distributed around the central object with the name "Bot"
Add a C# script named "Bot" to your project.
Attach the "Bot" script to the 5 new objects.
Again, you may use this script or ignore it when pursing the Functional Goals.
Functional Goal 2:

When the game is running, make the Bot closest to the Player turn blue. One and only one object (Item or Bot) has its color changed at a time. Ensure that when Player is moved around in the scene manually (by dragging the object in the scene view), the closest Item is red or the closest Bot is blue.

Phase 3
Functional Goal 3:

Ensure the scripts can handle any number of Items and Bots.

Functional Goal 4:

Allow the designer to choose the base color and highlight color for Items/Bots at edit time.

Questions
How can your implementation be optimized?
How much time did you spend on your implementation?
What was most challenging for you?
What else would you add to this exercise?

Optional
Find a way to make use of dependency injection when implementing the functional goals. Feel free to use an existing framework or create your own.
Add Unit Tests
Add XML docs
Optimize finding nearest
Add new Items/Bots automatically on key press
Read/write Item/Bot/Player state to a file and restore on launch

Next Steps
Confirm you've addressed the functional goals
Answer the questions above by adding them to this file
Commit and push the entire repository, with your completed project, back into a repository host of your choice (bitbucket, github, gitlab, etc.)
Share your project URL with your Virbela contact (Recruiter or Hiring Manager)
If you have questions
Reach out to your Virbela contact (Recruiter or Hiring Manager)






**Assignment:**


Questions
1. How can your implementation be optimized?
	An optimization would be to not use my "Run In Editor" option because although it is set up just fine and works as intended, it is never ideal to run a lot of scene processes or updates while in editor and calls for components to hold variable when not in play mode. Its a cool feature that works on this small project but would not be ideal for larger applications with more aspects to keep in mind, although these editor and inspector scripts can really help automate a lot of developer processes too. 
	Move everything to a positive quadrant so none of my calculations have to consider negatives or offsets due to negative values.



2. How much time did you spend on your implementation?

About 2 hours for goal 1-4
45 hour for XML documentation
2 hours for Optimizing finding nearest
3 hours for Unity Testing setup with Dependency Injection
Less than an hour for key press additions and Read/Write file for restoring on laucnh

8 hours over 2 days


3. What was most challenging for you?

	My largest challenge was just using Dependency Injections WITH Unit Testing, but once I started working with it it all went well. 

	Another chellenge I faced was optimizing the nearest algorithm away from doing so many distance calculations. My solution was to put each scene object into a 2d array where the position in the array relates to the int x and z position of the object so that the player only needs to search nearby items within that array for the distance calculation instead of doing that for every object.

	Another fun challeng I faced was allowing the user to change the color in edit mode and have them reflected in the scene immedietaly, even when in Edit mode and not in Play mode, for the fourth goal function. I was not sure if the exercise called for the scene to update immedietly or just allow the designer to change the color then and it update on play, but it was a fun challenge to have it update regardless of Play/Edit. This allowed me to work through running updates on object even when in Edit mode which brought on a lot of situations where things could go wrong. The dependency injection helped my scene get ready for the dependecies used in this situation and for unity Testing. Also the Scene Objects list had to be managed regardless of play mode. 


4. What else would you add to this exercise?
	I would add pushing and merging to the repo at every step so the process can be viewed instead of just the end product. A lot of optimizations I originally listed above I ended up incorperating later in the process.
