using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffectSystem : MonoBehaviour {

	public GameObject hitEffectPrefab;

	private static HitEffectSystem instance;
    private GameManager.PauseMode pauseRegister = (GameManager.PauseMode)(~(1 << (int)GameManager.PauseMode.Sleep));

    void OnEnable() {
		if(instance == null) {
			instance = this;
		} else {
			Destroy(this.gameObject);
		}
	}

	public static HitEffectSystem Instance {
		get {
			return instance;
		}
	}

	public void SpawnHitEffect(string trigger, Vector3 position) {
		GameObject temp = GameObject.Instantiate(hitEffectPrefab, position, Quaternion.identity);
		temp.GetComponent<Animator>().SetTrigger(trigger);
		StartCoroutine(DestroyAfterTime(temp, 2f));
	}

    private IEnumerator DestroyAfterTime(GameObject go, float time)
    {
        float timer = 0f;
        while(timer < time)
        {
            timer += GameManager.Instance.DeltaTime(pauseRegister);
            yield return null;
        }

        Destroy(go);
    }
}
