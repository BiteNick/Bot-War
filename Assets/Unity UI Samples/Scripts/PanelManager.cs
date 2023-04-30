using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class PanelManager : MonoBehaviour {

	public Animator initiallyOpen;

	private int m_OpenParameterId;
	private Animator m_Open;
	private GameObject m_PreviouslySelected;

	const string k_OpenTransitionName = "Open";
	const string k_ClosedStateName = "Closed";

	[SerializeField] private Slider volumeSettings;
	[SerializeField] private Slider CameraZoomSlider;
	[SerializeField] private Slider CameraKeysMovingSlider;
	[SerializeField] private Slider CameraMouseMovingSlider;

    private void Start()
    {
		if (volumeSettings != null)
			volumeSettings.value = gameManagerStatic.Volume;

		if (CameraKeysMovingSlider != null)
			CameraKeysMovingSlider.value = gameManagerStatic.CameraKeysMovingSpeed / gameManagerStatic.CameraMaxKeysMovingSpeed;

		if (CameraMouseMovingSlider != null)
			CameraMouseMovingSlider.value = gameManagerStatic.CameraMoveSpeed / gameManagerStatic.CameraMaxMoveSpeed;

	}

    public void OnEnable()
	{
		m_OpenParameterId = Animator.StringToHash (k_OpenTransitionName);

		if (initiallyOpen == null)
			return;

		OpenPanel(initiallyOpen);
	}

	public void OpenPanel (Animator anim)
	{
		if (m_Open == anim)
			return;

		anim.gameObject.SetActive(true);
		var newPreviouslySelected = EventSystem.current.currentSelectedGameObject;

		anim.transform.SetAsLastSibling();

		CloseCurrent();

		m_PreviouslySelected = newPreviouslySelected;

		m_Open = anim;
		m_Open.SetBool(m_OpenParameterId, true);

		GameObject go = FindFirstEnabledSelectable(anim.gameObject);

		SetSelected(go);
	}

	static GameObject FindFirstEnabledSelectable (GameObject gameObject)
	{
		GameObject go = null;
		var selectables = gameObject.GetComponentsInChildren<Selectable> (true);
		foreach (var selectable in selectables) {
			if (selectable.IsActive () && selectable.IsInteractable ()) {
				go = selectable.gameObject;
				break;
			}
		}
		return go;
	}

	public void CloseCurrent()
	{
		if (m_Open == null)
			return;

		m_Open.SetBool(m_OpenParameterId, false);
		SetSelected(m_PreviouslySelected);
		StartCoroutine(DisablePanelDeleyed(m_Open));
		m_Open = null;
	}

	IEnumerator DisablePanelDeleyed(Animator anim)
	{
		bool closedStateReached = false;
		bool wantToClose = true;
		while (!closedStateReached && wantToClose)
		{
			if (!anim.IsInTransition(0))
				closedStateReached = anim.GetCurrentAnimatorStateInfo(0).IsName(k_ClosedStateName);

			wantToClose = !anim.GetBool(m_OpenParameterId);

			yield return new WaitForEndOfFrame();
		}

		if (wantToClose)
			anim.gameObject.SetActive(false);
	}

	private void SetSelected(GameObject go)
	{
		EventSystem.current.SetSelectedGameObject(go);
	}

	public void LoadLevel(string sceneName)
    {
		SceneManager.LoadScene(sceneName);
    }

	public void SetVolume(float volume)
    {
		gameManagerStatic.Volume = volume;
    }

	public void SetKeysMovingSpeed(float speed)
	{
		gameManagerStatic.CameraKeysMovingSpeed = speed * gameManagerStatic.CameraMaxKeysMovingSpeed;
	}

	public void SetMoveSpeed(float speed)
	{
		gameManagerStatic.CameraMoveSpeed = speed * gameManagerStatic.CameraMaxMoveSpeed;
	}

}
