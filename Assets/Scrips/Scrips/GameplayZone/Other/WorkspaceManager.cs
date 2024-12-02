using UnityEngine;
using UnityEngine.UI;

public class WorkspaceManager : MonoBehaviour
{
    [SerializeField] private GameObject workspacePanel;

    [SerializeField] private Button clearButton;

    private void Start()
    {
        if (clearButton != null)
        {
            clearButton.onClick.AddListener(ClearWorkspace);
        }
    }

    private void ClearWorkspace()
    {
        if (workspacePanel != null)
        {
            foreach (Transform child in workspacePanel.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
