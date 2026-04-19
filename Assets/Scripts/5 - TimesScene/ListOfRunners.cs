using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class ListOfRunners : MonoBehaviour
{
    public static ListOfRunners instance;

    private List<GameObject> runners = new List<GameObject>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddRunner(GameObject runner)
    {
        runners.Add(runner);

        if (runners.Count == 0)
            GetComponent<TextMeshProUGUI>().text = "";

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("List of Runners:");

        for (int i = 0; i < runners.Count; i++)
        {
            sb.Append($"{i + 1} - {runners[i].name}");
            if (i < runners.Count - 1)
                sb.AppendLine();
        }

        GetComponent<TextMeshProUGUI>().text = sb.ToString();
    }

    public void CleanListOfRunners()
    {
        runners.Clear();
        GetComponent<TextMeshProUGUI>().text = "";
    }
}