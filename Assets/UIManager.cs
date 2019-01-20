using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	[SerializeField]
	private CommandSprites[] moveIcons;

	private Dictionary<Enums.FightCommands, CommandSprites> commandSpriteDict;

	public Image p1Delay, p2Delay, p1CommandSprite, p2CommandSprite;
	public Image p1InputPrefab, p2InputPrefab;
	public GameObject p1ScorePrefab, p2ScorePrefab;
	public Image inputTimer;

	public Animator roundAnimator, scoreAnim;

	private CanvasGroup cGroup;

	private static UIManager instance;

	public static UIManager Instance {
		get {
			return instance;
		}
	}

	// Use this for initialization
	void OnEnable () {
		if(instance == null) {
			instance = this;
			commandSpriteDict = new Dictionary<Enums.FightCommands, CommandSprites>();
			for(int i = 0; i < moveIcons.Length; i++) {
				commandSpriteDict.Add(moveIcons[i].command, moveIcons[i]);
			}
			p1InputPrefab.color = Color.clear;
			p2InputPrefab.color = Color.clear;
			p1CommandSprite.color = Color.clear;
			p2CommandSprite.color = Color.clear;
			cGroup = transform.GetChild(0).GetComponent<CanvasGroup>();
		} else {
			Destroy(gameObject);
		}
	}

	void Update() {
		for(int i = 0; i < p1InputPrefab.transform.parent.childCount; i++) {
			RectTransform rect = p1InputPrefab.transform.parent.GetChild(i).GetComponent<RectTransform>();
			rect.sizeDelta = Vector2.Lerp(rect.sizeDelta, Vector2.one*70f, Time.deltaTime*5f);
		}

		for(int i = 0; i < p2InputPrefab.transform.parent.childCount; i++) {
			RectTransform rect = p2InputPrefab.transform.parent.GetChild(i).GetComponent<RectTransform>();
			rect.sizeDelta = Vector2.Lerp(rect.sizeDelta, Vector2.one*70f, Time.deltaTime*5f);
		}
	}

	public void SetCommandSprite(int player, Enums.FightCommands command) {
		Debug.Log("Added: " + player + " .. " + command + ": " + p1InputPrefab.transform.parent.childCount + ", " + p2InputPrefab.transform.parent.childCount);
		if (player == 0) {
			if(p1InputPrefab.color == Color.clear) {
				p1InputPrefab.sprite = p1CommandSprite.sprite;
				p1InputPrefab.color = p1CommandSprite.color;
				p1InputPrefab.rectTransform.sizeDelta = new Vector2(70f, 0f);
			} else {
				GameObject copy = GameObject.Instantiate(p1InputPrefab.gameObject);
				copy.transform.parent = p1InputPrefab.transform.parent;
				copy.transform.SetAsFirstSibling();
				copy.transform.localScale = Vector3.one;
				copy.transform.localPosition = Vector3.zero;
				copy.GetComponent<RectTransform>().sizeDelta = new Vector2(70f,0f);
				copy.GetComponent<Image>().sprite = p1CommandSprite.sprite;
				copy.GetComponent<Image>().color = p1CommandSprite.color;
			}
		} else {
			if(p2InputPrefab.color == Color.clear) {
				p2InputPrefab.sprite = p2CommandSprite.sprite;
				p2InputPrefab.color = p2CommandSprite.color;
				p2InputPrefab.rectTransform.sizeDelta = new Vector2(70f, 0f);
			} else {
				GameObject copy = GameObject.Instantiate(p2InputPrefab.gameObject);
				copy.transform.parent = p2InputPrefab.transform.parent;
				copy.transform.SetAsFirstSibling();
				copy.transform.localScale = Vector3.one;
				copy.transform.localPosition = Vector3.zero;
				copy.GetComponent<RectTransform>().sizeDelta = new Vector2(70f,0f);
				copy.GetComponent<Image>().sprite = p2CommandSprite.sprite;
				copy.GetComponent<Image>().color = p2CommandSprite.color;
			}
		}

		if(commandSpriteDict.ContainsKey(command)) {
			if(player == 0) {
				p1CommandSprite.sprite = commandSpriteDict[command].sprite;
				p1CommandSprite.color = commandSpriteDict[command].tintColor;
			} else {
				p2CommandSprite.sprite = commandSpriteDict[command].sprite;
				p2CommandSprite.color = commandSpriteDict[command].tintColor;
			}
		} else {
			Debug.Log(command + " NOT FOUND FOR UI");
			if(player == 0) {
				p1CommandSprite.sprite = null;
				p1CommandSprite.color = Color.magenta;
			} else {
				p1CommandSprite.sprite = null;
				p1CommandSprite.color = Color.magenta;
			}
		}
	}

	public void PlayFightAnim() { 
		roundAnimator.SetTrigger("FightAnimation");
	}

	public void UpdatePlayerInputDelay(int player, float delay) {
		if(player == 0) {
			p1Delay.fillAmount = delay;
		} else { 
			p2Delay.fillAmount = delay;
		}
	}

	public void UpdateOverallTimer(float time) {
		inputTimer.fillAmount = time;
	}

	public void HideUI () {
		StartCoroutine(HideUIAnim(true));
	}

	public void ShowUI () {
		StartCoroutine(HideUIAnim(false));
	}

	public void ShowScoreUI() {
		StartCoroutine(ShowScoreUIAnim());
	}

	private int p1RecordedScore, p2RecordedScore;

	private IEnumerator ShowScoreUIAnim() {
		yield return new WaitForSeconds(1f);
		scoreAnim.SetTrigger("Play");
		yield return new WaitForSeconds(0.75f);
		if(p1RecordedScore < LevelManager.Instance.GetScore(0)) {
			p1RecordedScore++;
			if(p1ScorePrefab.activeSelf) {
				GameObject temp = GameObject.Instantiate(p1ScorePrefab);
				temp.transform.parent = p1ScorePrefab.transform.parent;
				temp.transform.localScale = Vector3.one;
				temp.transform.localPosition = Vector3.zero;
				temp.transform.localEulerAngles = Vector3.zero;
				StartCoroutine(AnimateScoreItem(temp));
			} else {
				p1ScorePrefab.SetActive(true);
				StartCoroutine(AnimateScoreItem(p1ScorePrefab));
			}
		} else {
			p2RecordedScore++;
			if(p2ScorePrefab.activeSelf) {
				GameObject temp = GameObject.Instantiate(p2ScorePrefab);
				temp.transform.parent = p2ScorePrefab.transform.parent;
				temp.transform.localScale = Vector3.one;
				temp.transform.localPosition = Vector3.zero;
				temp.transform.localEulerAngles = Vector3.zero;
				StartCoroutine(AnimateScoreItem(temp));
			} else {
				p2ScorePrefab.SetActive(true);
				StartCoroutine(AnimateScoreItem(p2ScorePrefab));
			}
		}
		yield break;
	}

	private IEnumerator AnimateScoreItem(GameObject prefab) {
		CanvasGroup cGroup = prefab.GetComponent<CanvasGroup>();
		cGroup.alpha = 0f;
		prefab.transform.localScale = Vector3.one*2f;
		float randomRotation = Random.Range(-10f,10f);
		prefab.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f))*5f;
		float timer = 0f;
		while(timer < 1f) {
			timer += Time.deltaTime * 4f;
			prefab.transform.localScale = Vector3.Lerp(Vector3.one*2f, Vector3.one, timer*timer);
			prefab.transform.GetChild(0).localEulerAngles = Vector3.forward*Mathf.Lerp(0f, randomRotation, timer*timer);
			cGroup.alpha = timer;
			yield return null;
		}

		yield break;
	}

	private IEnumerator HideUIAnim (bool hide) {
		if(hide) {
			while(cGroup.alpha > 0) {
				cGroup.alpha -= Time.deltaTime * 4f;
				yield return null;
			}
			int p1Commands = p1InputPrefab.transform.parent.childCount;
			for (int i = 0; i < p1Commands; i++) {
				if(p1InputPrefab.transform.parent.GetChild(0).gameObject != p1InputPrefab.gameObject) {
					DestroyImmediate(p1InputPrefab.transform.parent.GetChild(0).gameObject);
				}
			}

			int p2Commands = p2InputPrefab.transform.parent.childCount;
			for (int i = 0; i < p2Commands; i++) {
				if(p2InputPrefab.transform.parent.GetChild(0).gameObject != p2InputPrefab.gameObject) {
					DestroyImmediate(p2InputPrefab.transform.parent.GetChild(0).gameObject);
				}
			}

			p1InputPrefab.color = Color.clear;
			p2InputPrefab.color = Color.clear;
			p1CommandSprite.color = Color.clear;
			p2CommandSprite.color = Color.clear;
			Debug.Log("COUNT " + p1Commands + " " + p2Commands + " .... " + p1InputPrefab.transform.parent.childCount + " " + p2InputPrefab.transform.parent.childCount);
		} else {
			while(cGroup.alpha < 1) {
				cGroup.alpha += Time.deltaTime * 4f;
				yield return null;
			}
		}
		
		yield break;
	}

	[System.Serializable]
	private struct CommandSprites {
		public Sprite sprite;
		public Enums.FightCommands command;
		public Color tintColor;
	}
}
