/// This is a collection of all common-use enums.
/// These will all be public enums that will find use between several classes.

public enum NetworkMessage
{
	None,
	Start,
	End,
	Quit,
	ServerDeviceClaim,
	ServerDeviceUnClaim,
	ClientDeviceClaim,
	ClientDeviceUnClaim,
	Ready,
	NotReady
}

public enum GameMode
{
	Boot,
	MainMenu,
	HostLocal,
	HostNetwork,
	JoinNetwork,
	Setup,
	Gameplay
}

public enum InputDeviceType
{
	Network,
	Keyboard,
	Controller,
	AI
}

/// An Enum that represents "Commands"
	/// CommandCodes are the bridge between Input events and object behaviour.
	/// CommandCodes enumerate the different activities that a object will perform, and can be sent from Users or AI players to game objects.
public enum CommandCode
{
	/// <summary>
	/// No Command Code. No CommandCode will be sent.
	/// </summary>
	None,
	/// <summary>
	/// Generic rotation command.
	/// Expects to be paired with a signed magnitude.
	/// </summary>
	Rotate,
	/// <summary>
	/// Left rotation command.
	/// </summary>
	RotateLeft,
	/// <summary>
	/// Right rotation command.
	/// </summary>
	RotateRight,
	/// <summary>
	/// Stops rotating.
	/// </summary>
	StopRotate,
	/// <summary>
	/// Thrust command.
	/// </summary>
	Thrust,
	/// <summary>
	/// Stops thrusting.
	/// </summary>
	StopThrust,
	/// <summary>
	/// Shoot command.
	/// </summary>
	Shoot,
	/// <summary>
	/// Stops shooting.
	/// </summary>
	StopShoot,
	/// <summary>
	/// Generic "Start" command. Will be used for drop-in game joining.
	/// </summary>
	Start,
	/// <summary>
	/// "Soft" quit command. This is used for dropping out of a game, but not quitting the application.
	/// </summary>
	SoftQuit,
	/// <summary>
	/// "Hard" quit command. This is used for quitting the application entirely.
	/// </summary>
	HardQuit,
	
	/// <summary>
	/// Up command for menus.
	/// </summary>
	MenuUp,
	/// <summary>
	/// Down command for menus.
	/// </summary>
	MenuDown,
	/// <summary>
	/// Left command for menus.
	/// </summary>
	MenuLeft,
	/// <summary>
	/// Right command for menus.
	/// </summary>
	MenuRight,
	/// <summary>
	/// Accept command for menus.
	/// This will tend to be used for locking in values.
	/// It will also be used for buttons.
	/// </summary>
	MenuAccept,
	/// <summary>
	/// Constant menu cancel.
	/// This will tend to be used for cancelling out an inputted value.
	/// It will also be used for buttons.
	/// </summary>
	MenuCancel
}
