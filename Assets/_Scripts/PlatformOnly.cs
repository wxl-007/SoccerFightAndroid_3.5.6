using UnityEngine;
using System.Collections;

public class PlatformOnly : MonoBehaviour {
	
	public enum Platform  {
		PortableOnly,
		PCOnly
	}
	
	public PlatformOnly.Platform platform;
	
	void Awake () {
		bool isPortable = (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer);
		if (isPortable != (platform == Platform.PortableOnly)) {
			gameObject.SetActiveRecursively(false);
		}
	}
}
