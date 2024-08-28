using UnityEngine;

public class GridController : MonoBehaviour
{
	public Vector2Int gridSize;
	public float cellRadius = 0.5f;
	public FlowField curFlowField;
	public GridDebug gridDebug;

	private void InitializeFlowField()
	{
		curFlowField = new FlowField(cellRadius, gridSize);
		curFlowField.CreateGrid();
		gridDebug.SetFlowField(curFlowField);
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			InitializeFlowField();

			curFlowField.CreateCostField();

			Vector3 worldMousePos = Vector3.zero;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit))
			{
				worldMousePos = hit.point;
			}
			//Vector3 mousePos = Input.mousePosition;
			//Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
			Cell destinationCell = curFlowField.GetCellFromWorldPos(worldMousePos);
			curFlowField.CreateIntegrationField(destinationCell);

			curFlowField.CreateFlowField();

			gridDebug.DrawFlowField();
		}
	}
}