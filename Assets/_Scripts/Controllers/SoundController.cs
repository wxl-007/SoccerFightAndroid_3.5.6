/*
Maintaince Logs:
2011-12-15    XuMingzhao    Initial version.  Add Sound Controll.
*/

using UnityEngine;
using System.Collections;

public class SoundController : MonoBehaviour {
	
	public AudioClip 
		ball_heading, 
		ball_kick, 
		beep_end, 
		beep_normal, 
		beep_start, 
		click,
		count_down,
		fail,
		fireball_burning,
		fireball_shoot,
		goal_or_win,
		prop_popup,
		prop_touch;
	
	public AudioSource
		SoundCheer,
		SoundFootball,
		SoundProp,
		SoundClick;
	
	#region Init / Add singleton
    private static SoundController instance;
    public static SoundController Instance
    {
        get
        {
            if (instance == null)
            {
				instance = (SoundController)FindObjectOfType(typeof(SoundController)); 
            }
			if (!instance)
            {
                Debug.LogError("SoundController could not find itself!");
             } 

            return instance;
        }
    }
    #endregion 
	
	void Awake () {
		int t = PlayerPrefs.GetInt("SoundToogle", 1);
		if (t == 0)
		{
			Vector3 tmp;
			tmp = SoundCheer.transform.position;
			SoundClick.transform.position = new Vector3(tmp.x, 10000f, tmp.z);
			SoundCheer.transform.position = new Vector3(tmp.x, 10000f, tmp.z);
			SoundFootball.transform.position = new Vector3(tmp.x, 10000f, tmp.z);
			SoundProp.transform.position = new Vector3(tmp.x, 10000f, tmp.z);
		}
	}
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	
    public void play_ball_heading()
    {
        SoundFootball.clip = ball_heading;
        SoundFootball.Play();
    }


    public void play_ball_kick()
    {
        SoundFootball.clip = ball_kick;
        SoundFootball.Play();
    }


    public void play_beep_end()
    {
        SoundCheer.clip = beep_end;
        SoundCheer.Play();
    }


    public void play_beep_normal()
    {
        SoundCheer.clip = beep_normal;
        SoundCheer.Play();
    }


    public void play_beep_start()
    {
        SoundCheer.clip = beep_start;
        SoundCheer.Play();
    }


    public void play_click()
    {
        audio.clip = click;
        audio.Play();
    }


    public void play_count_down()
    {
        SoundCheer.clip = count_down;
        SoundCheer.Play();
    }


    public void play_fail()
    {
        SoundCheer.clip = fail;
        SoundCheer.Play();
    }


    public void play_fireball_burning()
    {
        SoundFootball.clip = fireball_burning;
        SoundFootball.Play();
    }


    public void play_fireball_shoot()
    {
        SoundFootball.clip = fireball_shoot;
        SoundFootball.Play();
    }


    public void play_goal_or_win()
    {
        SoundCheer.clip = goal_or_win;
        SoundCheer.Play();
    }


    public void play_prop_popup()
    {
        SoundProp.clip = prop_popup;
        SoundProp.Play();
    }


    public void play_prop_touch()
    {
        SoundProp.clip = prop_touch;
        SoundProp.Play();
    }
}