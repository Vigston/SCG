using System.Collections.Generic;
using UnityEngine;

public class CursorMgr : MonoBehaviour
{
	public Camera mainCamera;

	[SerializeField]
	private SelectableObject selectedObject;

	[SerializeField]
	private List<SelectableObject> selectedObjects = new List<SelectableObject>();

	void Update()
	{
		HandleMouseInput();
	}

	void HandleMouseInput()
	{
		if (Input.GetMouseButtonDown(0)) // 左クリック
		{
			Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
			RaycastHit[] hits = Physics.RaycastAll(ray);

			float closestDistance = Mathf.Infinity;
			SelectableObject closestSelectable = null;

			foreach (RaycastHit hit in hits)
			{
				SelectableObject selectable = hit.collider.GetComponent<SelectableObject>();
				if (selectable != null && hit.distance < closestDistance)
				{
					closestDistance = hit.distance;
					closestSelectable = selectable;
				}
			}

			if(selectedObject != closestSelectable)
			{
				closestSelectable.Select();
				selectedObject = closestSelectable;
			}

			//if (closestSelectable != null && !selectedObjects.Contains(closestSelectable))
			//{
			//	closestSelectable.Select();
			//	selectedObjects.Add(closestSelectable);
			//}
		}

		if (Input.GetMouseButtonDown(1)) // 右クリック
		{
			foreach (var obj in selectedObjects)
			{
				obj.Deselect();
			}
			selectedObjects.Clear();
		}
	}
}