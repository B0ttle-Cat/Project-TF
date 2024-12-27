using System.Linq;

using Sirenix.OdinInspector;

using UnityEngine;

public class OrganizeObject : MonoBehaviour
{
	[Button("OrganizeChildrenObjects")]
	public void OrganizeChildrenObjects()
	{
		int childCount = transform.childCount;
		Bounds[] childBounds = new Bounds[childCount];

		// 1. 직속 자식들의 Bounds를 계산하여 배열에 저장
		for(int i = 0 ; i < childCount ; i++)
		{
			var childTr = transform.GetChild(i);
			var childObj = childTr.gameObject;

			// 각 자식의 Bounds를 계산
			Bounds? bounds = null;
			Bounds[] tempBounds = childObj.GetComponentsInChildren<MeshRenderer>(true)
				.Select(r => r.bounds)
				.Concat(childObj.GetComponentsInChildren<SkinnedMeshRenderer>(true)
				.Select(r => r.bounds))
				//.Concat(childObj.GetComponentsInChildren<Collider>(true)
				//.Select(c => c.bounds))
				.ToArray();

			// 모든 bounds를 결합하여 하나의 bounds로 만듦
			foreach(var bound in tempBounds)
			{
				if(bounds.HasValue)
				{
					Bounds value = bounds.Value;
					value.Encapsulate(bound);
					bounds = value;
				}
				else
				{
					bounds = bound;
				}
			}

			// bounds 배열에 저장
			if(bounds.HasValue)
			{
				childBounds[i] = bounds.Value;
			}
			else
			{
				Debug.LogError($"No Bounds: {childObj.name}");
			}
		}

		// 2. 직속 자식들을 X축을 기준으로 일렬로 정렬
		float totalWidth = 0f;
		for(int i = 0 ; i < childCount ; i++)
		{
			totalWidth += childBounds[i].size.x; // 각 자식의 너비를 합산
		}

		float currentX = -totalWidth / 2f; // 정렬 시작 위치 (왼쪽 끝에서 시작)

		// 3. 자식들의 위치를 일렬로 정렬
		for(int i = 0 ; i < childCount ; i++)
		{
			var childTr = transform.GetChild(i);
			Bounds bound = childBounds[i];

			// 현재 자식의 위치를 조정 (X축으로 이동)
			childTr.position = new Vector3(currentX + bound.extents.x, childTr.position.y, childTr.position.z);

			// X 위치를 다음 자식의 시작 위치로 업데이트
			currentX += bound.size.x;
		}
	}


	[Button("SortChildrenByName")]
	public void SortChildrenByName()
	{
		var children = Enumerable.Range(0, transform.childCount)
			.Select(i => transform.GetChild(i))
			.OrderBy(child => child.name)
			.ToList();

		for(int i = 0 ; i < children.Count ; i++)
		{
			children[i].SetSiblingIndex(i);
		}
	}



	[Button("ReplaceChildName")]
	public void ReplaceChildName()
	{
		string Room_Prop_Index = "Room_Prop_{0:0000}";

		int childCount = transform.childCount;
		for(int i = 0 ; i < transform.childCount ; i++)
		{
			var childTr = transform.GetChild(i);
			var childObj = childTr.gameObject;

			if(!childObj.name.StartsWith("Room_Prop"))
			{
				GameObject propPivot = new GameObject(string.Format(Room_Prop_Index, i));
				propPivot.transform.SetParent(transform);
				propPivot.transform.SetSiblingIndex(i);
				propPivot.transform.position = childTr.position;

				childTr.SetParent(propPivot.transform);
			}
			else
			{
				childObj.name = string.Format(Room_Prop_Index, i);
			}
		}
	}
}
