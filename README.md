# Journey of the Return

## Description
The game will be a puzzle platformer. Each level consists of searching the level for pieces the character needs in order to repair the psychologically damaged robot sitting at the center of the level. The idea I've been toying with is that the pieces will actually be the component pieces of a neural network (i.e., the robot's "brain") with the puzzle being to arrange the pieces of the neural network to complete a machine learning task (like being able to identify alphabetical characters, categorize images, etc). We're also considering throwing in boss battles that takes place within each robot's mind after they've been fixed to represent to mental illness/psychological problem they've been dealing with.

## Story
### Act 1: The Ninety and Nine
Jerrod, an artificial intelligence expert working as a technician on a space ship called "The Return" crash lands next to a small village on a desert planet full of robots. He escapes from the wreckage with one other survivor, and is taken to the village by a Robot named Ephraim in order to tend the other survivor's wounds.

On this planet, the robots have highly advanced biological technology parallel to our own. They genetically synthesize and modify DNA to build creatures and plants that meet their needs the way that we build computers and devices. However, in the village it's a religious taboo to create lifeforms that resemble robot-kind (aka, human-like creatures).

As Ephraim enters the village with Jerrod and the injured shipmate, Jerrod notices that the village seems deserted. Ephraim explains that the city has lured away most of his clan. When Jerrod hears that he gets excited at the possibility of getting the ship repaired, but Ephraim warns Jerrod to stay away, that the city has a way of twisting people.

Ephraim brings the injured shipmate to his younger brother. The brother does not believe in the religious traditions of the village, and so has no qualms working on the ship-mate. The shipmate dies while the brother is operating on him, and in desparation the brother re-animates the shipmate, though instead of restoring life, he ends up turning the shipmate into little more than a puppet.

Ephraim gets into an argument with the brother over the results of the operation, and the brother decides to leave the village to head to the city, where the old traditions are not in force.



In order to repair the ship, Jerrod will need several tons of raw material ends up on a journey with one of the robots from the village to find his (the robot's) wayward brother, who rebelled and left the village to go to "The City," a shining utopian paradise on the otherwise undeveloped planet. Along the way, he finds several other robots who are also trying to get to The City, each of whom is psychologically damaged in some way. He repairs each of them and brings them along.

When they make it to The City, they find out that it's actually in ruins. The robots that live there have all fallen prey to idleness and addiction, and are all mentally sick to one degree or another. The player notices that each of the robots has modified themselves with things like decorative perforations in their outer sheet metal, carvings, and welded-on objects & symbols, and that the robots there are convinced that it's still somehow a paradise. As the main character searches The City, he learns that the wayward brother has left and is making his way up a nearby mountainside.

They follow after the brother, and finally catch up to him close to the summit. By this time, the brother has undergone a complete transformation from a simple farm robot to a heavily modified "prophet of The City." So heavily modified in fact, that he's barely functioning. You learn that he's here because he discovered that all robots that live on this planet come from atop the mountain, and he's directing them away from the village and towards the city. Just as you learn this from him, he breaks down and collapses. The player is able to repair his mind, and get him functioning but can't completely fix him. The player then decides to carry the brother the rest of the way up the summit, where he finds the creator of the robots. The creator completely fixes the brother, and invites the player to come live with him on the summit of the mountain. The player refuses, and instead asks for the parts he needs to repair his ship and go home. The player and all of the other robots return to the village. The player repairs his ship and gets back to Earth.

## How this Repo is Organized
The project follows the basic Unity Project structure:
* [Assets](): This is the meat of the project. Code, scenes, art, sound, animation, you name it.
* [Packages](): A shell folder. When opening the project in Unity, the manifest.json will be used to determine which Unity packages get loaded up. 
* [ProjectSettings](): Contains all of the project's global preferences and settings. 

## Getting Started with Unity
[Unity Homepage](https://unity.com)

## Getting Started with Python
[Using C# code to talk to Python and Vice Versa](https://www.codeproject.com/Articles/602112/Scripting-NET-Applications-with-IronPython)

## Ideas
Some other thoughts and ideas we're playing with:
- I'm hoping to weave in a personal story for the character as well. The idea is that he left Earth with a laundry list full of his own problems, and by the end of the story decides to go back and face his problems and mistakes.
- The player has an AI he's created who comes along with him, and who acts as a bit of a nagging shoulder angel. You later learn that this AI is actually modeled after his wife, who he's (divorced from/separated from/grieving the death of, we don't really know which).
- There was a very early idea to make it possible to steal the components cattered throughout each level and use them to repair the ship early, but we're not sure if we can really fit that nicely into the story as it exists.


Some example machine learning tasks:
- Recognizing Handwriting (this is actually a beginner level project in machine learning classes)
- Sentiment analysis (Reading a sentence and determining its "mood")
- Creating convincing sentences
- Creating realistic looking images (This is actually one of the most challenging tasks in machine learning that is currently being studied at both an academic and industry level).
- Reinforcement learning (teaching a do a task like balancing a broom stick) (The coolest version I've seen of this category of task is: https://www.youtube.com/watch?v=UuhECwm31dM)
- Threat detection (this could be anything from filtering spam to identifying threats on a battle field)
