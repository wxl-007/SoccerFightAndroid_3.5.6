/*
Maintaince Logs:
2010-11-22    Waigo    Initial version.  Attached to the Players sound trigger, then make sounds several times by multi hitting.
*/


using UnityEngine;
using System.Collections;

public class ControllerSoundTriggers : MonoBehaviour {

	float baseFootAudioVolume = 1.0f;
	float soundEffectPitchRandomness = 0.05f;

	void OnTriggerEnter (Collider other) {		
		CollisionSoundEffect collisionSoundEffect = other.GetComponent(typeof(CollisionSoundEffect)) as CollisionSoundEffect;

		if (collisionSoundEffect) {
			audio.clip = collisionSoundEffect.audioClip;
			audio.volume = collisionSoundEffect.volumeModifier * baseFootAudioVolume;
			audio.pitch = Random.Range(1.0f - soundEffectPitchRandomness, 1.0f + soundEffectPitchRandomness);
			audio.Play();		
		}
	}
}
