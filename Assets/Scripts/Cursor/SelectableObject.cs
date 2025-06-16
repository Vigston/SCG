using UnityEngine;

public class SelectableObject : MonoBehaviour
{
	public bool isSelected { get; private set; }

	public void Select()
	{
		Material material = GetComponent<Renderer>().material;

		material.SetFloat("_OutlineWidth", 0.03f);

		isSelected = true;

		Debug.Log($"{gameObject.name} selected.");
	}

	public void Deselect()
	{
		Material material = GetComponent<Renderer>().material;

		material.SetFloat("_OutlineWidth", 0f);
		isSelected = false;
		Debug.Log($"{gameObject.name} deselected.");
	}
}