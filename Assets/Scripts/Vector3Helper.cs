using UnityEngine;
using System.Collections.Generic;

namespace APRLM.Utilities
{
    public static class Vector3Helper
	{
		/// <summary>
		/// Call this to find average of 1+ Vector3's
		/// </summary>
		/// <returns></returns>
		public static Vector3 FindAveragePosition(Vector3[] vectors)
		{
			Vector3 result = Vector3.zero;

			//go through each vector
			for (int i = 0; i < vectors.Length; i++)
			{
				//add em all up
				result += vectors[i];
			}
			//return all of them added up by the num vectors we are averaging
			return result / vectors.Length;
		}

		/// <summary>
		/// Call this to find average of 1+ Vector3's
		/// </summary>
		/// <returns></returns>
		public static Vector3 FindAveragePosition(List<Vector3> vectors)
		{
			Vector3 result = Vector3.zero;

			//go through each vector
			for (int i = 0; i < vectors.Count; i++)
			{
				//add em all up
				result += vectors[i];
			}
			//return all of them added up by the num vectors we are averaging
			return result / vectors.Count;
		}
	}
}