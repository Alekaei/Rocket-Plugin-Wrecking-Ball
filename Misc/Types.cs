using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WreckingBall
{
	public enum WreckType
	{
		Wreck,
		Cleanup,
		Scan,
		Counts,
	}

	public enum TeleportType
	{
		Barricades,
		Structures,
		Vehicles,
	}

	public enum FlagType
	{
		Normal,
		SteamID,
		ItemID,
	}

	public enum ElementType
	{
		Barricade,
		Structure,
		Vehicle,
		Zombie,
		Animal
	}

	public enum BuildableType
	{
		Element,
		Vehicle,
		VehicleElement
	}
}
