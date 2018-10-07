/*
Maintaince Logs:
2011-12-16    XuMingzhao    Initial version.  Controll competence.
*/

using UnityEngine;
using System.Collections;

public class CompetenceController : MonoBehaviour {
	
	[System.Serializable]
	public class OneCompetence{
		public string name;
		private int level;
		public UIButton btn_plus;
		public UIButton btn_needPt;
		public UIButton btn_cut;
		public UIProgressBar bar;
		public float maxVal;
		public float minVal;
		public int[] levelCost;
		public int levelMax = 7;
		
		public void CheckButtonState(){
			if (level == 1)
				btn_cut.SetControlState(UIButton.CONTROL_STATE.DISABLED);
			else if (btn_cut.controlState != UIButton.CONTROL_STATE.ACTIVE)
				btn_cut.SetControlState(UIButton.CONTROL_STATE.NORMAL);
			
			if (level == levelMax || PlayerPrefs.GetInt("SkillPoints", 0) < levelCost[level])
				btn_plus.SetControlState(UIButton.CONTROL_STATE.DISABLED);
			else if (btn_plus.controlState != UIButton.CONTROL_STATE.ACTIVE)
				btn_plus.SetControlState(UIButton.CONTROL_STATE.NORMAL);
			
			if (level != levelMax && PlayerPrefs.GetInt("SkillPoints", 0) < levelCost[level])
				btn_needPt.Hide(false);
			else
				btn_needPt.Hide(true);
		}
		
		public void init(){
			level = PlayerPrefs.GetInt(name+"Level", 1);
			bar.Value = level / (float) levelMax;
			CheckButtonState();
			btn_plus.AddValueChangedDelegate(ClickPlus);
			btn_cut.AddValueChangedDelegate(ClickCut);
			btn_needPt.AddValueChangedDelegate(ClickNeedPt);
		}
		
		public void ChangeLevel(int delta)
		{
			int points = PlayerPrefs.GetInt("SkillPoints", 0);
			if (levelCost[level + (delta < 0 ? -1 : 0)] * delta > points) return;
			points -= levelCost[level + (delta < 0 ? -1 : 0)] * delta;
			PlayerPrefs.SetInt("SkillPoints",points);
			level = Mathf.Clamp(level+delta, 1, levelMax);
			PlayerPrefs.SetInt(name + "Level", level);
			bar.Value = level / (float) levelMax;
			float curVal = Mathf.Lerp(minVal, maxVal, bar.Value);
			
			if (name != "Skill")
				PlayerPrefs.SetFloat(name, curVal);
			else
				PlayerPrefs.SetInt(name, (int) curVal);
			
			PlayerPrefs.Save();
		}
		
		public void ClickPlus(IUIObject btn){
			ChangeLevel(1);
		}
		
		public void ClickCut(IUIObject btn){
			ChangeLevel(-1);
		}
		
		public void ClickNeedPt(IUIObject btn){
			MainMenuController.Instance.needPtText.Text = levelCost[level].ToString();
			MainMenuController.Instance.windowManager.BringIn("UpgradePanel");
		}
	}
	
	public OneCompetence Attack,Defend,Jump,Skill;
	
	void CheckAllButtonState()
	{
		Attack.CheckButtonState();
		Defend.CheckButtonState();
		Jump.CheckButtonState();
		Skill.CheckButtonState();
	}
	
	void Awake () {
	}
	
	// Use this for initialization
	void Start () {
		Attack.init();
		Defend.init();
		Jump.init();
		Skill.init();
	}
	
	// Update is called once per frame
	void Update () {
		CheckAllButtonState();
	}
}
