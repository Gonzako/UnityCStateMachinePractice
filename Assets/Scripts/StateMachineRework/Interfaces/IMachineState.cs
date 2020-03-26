using System.Collections.Generic;

namespace GDMachine
{

	public interface IMachineStateTransition
	{
		IMachineState enterState { get; set; }


		//Might not be used
		IMachineState exitState { get; set; }

		IBoolFunc evaluator { get; set; }

		IMachineState transition();

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