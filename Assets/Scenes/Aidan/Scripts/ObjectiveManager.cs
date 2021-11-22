using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance;

    [SerializeField] private int firstObjectiveIndex = 0;
    private List<Objective> objectives = new List<Objective>();
    private Objective currentObjective = null;
    private bool hasObjectivesLeftToComplete = true;
    private bool hasStarted = false;

    private void Awake()
    {
        // Singleton Pattern
        if (Instance)
        {
            if (Instance != this)
            {
                Destroy(this);
            }
        }
        else
        {
            Instance = this;
        }

        ChangeStartIndex(firstObjectiveIndex, false);
    }

    private void OnEnable() => MainMenuManager.OnShowEvent += DisableAllObjectives;

    private void OnDisable() => MainMenuManager.OnShowEvent -= DisableAllObjectives;

    private void Update()
    {
        // Don't update if the objectives have all been complete or they haven't been started yet
        if (!hasObjectivesLeftToComplete || !hasStarted) return;

        // Update the current objective
        bool isCurrentTaskFinished = true;
        if (currentObjective)
            isCurrentTaskFinished = currentObjective.Tick();

        // Get the next objective if the current one has finished
        if (isCurrentTaskFinished)
        {
            currentObjective.OnExit();
            currentObjective = GetNextObjective();
            if (!currentObjective)
                hasObjectivesLeftToComplete = false;
            else
                currentObjective.OnEnter();
        }
    }

    private Objective GetNextObjective()
    {
        // Find the next task that isn't complete in the list
        for (int i = 0; i < objectives.Count; i++)
        {
            if (!objectives[i].IsComplete)
                return objectives[i];
        }
        return null;
    }

    public void ChangeStartIndex(int index, bool start = true)
    {
        objectives.Clear();

        // Get the objectives in order
        int childCount = transform.childCount;
        for (int i = index; i < childCount; i++)
        {
            // Get the objective component from each of the children
            Objective objective = transform.GetChild(i).GetComponent<Objective>();
            objective.ResetObjective();
            objectives.Add(objective);
        }
        hasObjectivesLeftToComplete = objectives.Count > 0 ? true : false;

        hasStarted = start;

        if (currentObjective) 
            currentObjective.OnExit();
        currentObjective = objectives[0];
        currentObjective.OnEnter();
    }

    private void DisableAllObjectives()
    {
        objectives.Clear();
        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            // Get the objective component from each of the children
            Objective objective = transform.GetChild(i).GetComponent<Objective>();
            objective.ResetObjective();
        }
        hasStarted = false;
    }
}
