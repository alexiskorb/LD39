using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
	[SerializeField]
    private int m_objectiveCount = 0;
	[SerializeField]
	private int m_objectiveMax = 20;

	public int ObjectiveProgress { get { return m_objectiveCount; } }
	public int ObjectiveTarget { get { return m_objectiveMax; } }

	public delegate void ObjectiveIncremented();
	public event ObjectiveIncremented OnObjectiveIncremented;

	void OnEnable()
	{
		foreach (GameObject player in GameManager.Instance.Players)
		{
			player.GetComponent<PlayerInteraction>().depositedObjects += addObjectives;
		}
	}

	private void OnDisable()
	{
		foreach (GameObject player in GameManager.Instance.Players)
		{
			player.GetComponent<PlayerInteraction>().depositedObjects -= addObjectives;
		}
	}

	public void addObjectives(int num)
    {
        m_objectiveCount += num;
        m_objectiveCount = Mathf.Min(m_objectiveCount, m_objectiveMax);

		if (OnObjectiveIncremented != null)
		{
			OnObjectiveIncremented();
		}

		if (m_objectiveMax > 0 && m_objectiveCount >= m_objectiveMax)
		{
			SceneLoader.Instance.NextScene();
		}
	}
}

