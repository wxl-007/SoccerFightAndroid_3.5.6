/*
Maintaince Logs:
2010-11-22    Waigo    Initial version.  Sound effects for pushable objects. 
*/


// Small script to hold a reference to an audioclip to play when the player hits me.

// This script is attached to game object making up your level. 
// The "Foot" script (which is attached to the player) looks for this script on whatever it touches.
// If it finds it, then it will play the sound when the foot comes in contact

using UnityEngine;

public class CollisionSoundEffect : MonoBehaviour
{

	public AudioClip audioClip ;
	public float volumeModifier = 1.0f;
}