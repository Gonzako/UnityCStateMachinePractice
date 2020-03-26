using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDMachine
{
	public class floatOperators : IBoolFunc
	{


		/// <summary>
		/// Input[0] is used to check the 
		/// </summary>
		/// <param name="val1"></param>
		/// <param name="val2"></param>
		/// <param name="input">Array used to give flexibility to the parameter creator</param>
		/// <returns></returns>
		public bool evaluate(float val1, float val2, params string[] input)
		{
			var operationType = (floatOperatorTypes)int.Parse(input[0]);
			switch (operationType)
			{
				case floatOperatorTypes.GreaterThan:
					return val1 > val2;
				case floatOperatorTypes.LessThan:
					return val1 < val2;
				case floatOperatorTypes.Equals:
					return Mathf.Approximately(val1, val2);
				default:
					Debug.LogFormat("There was an error in the operation type casting");
					throw (new System.InvalidOperationException());
			}
		} 
	}


	public enum floatOperatorTypes
	{
		GreaterThan, LessThan, Equals
	}
}
