using UnityEngine;
using System.Collections;

namespace Assets.Scripts.World
{
	public class Toggable: MonoBehaviour
	{

		public virtual IEnumerator Toggle()
		{
			Debug.Log("Toggable toggled.");
			yield return null;
		}
	}
}