using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GDMachine
{

	public interface IMachineStateTransition
	{



	}


	public interface IMachineState
	{

		List<IMachineStateTransition> transitions { get; set; }

		/// <summary>
		/// Function use for setting up your state
		/// </summary>
		void onStateEnter();

		/// <summary>
		/// Function called for cleanup code
		/// </summary>
		void onStateExit();

		/// <summary>
		/// Called every frame to handle logic
		/// </summary>
		void Tick();

		IMachineState checkForTransistion();

	}

}