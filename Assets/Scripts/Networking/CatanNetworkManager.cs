using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class CatanNetworkManager : NetworkManager
{
    private GameController gameController;

    protected virtual void Start( )
    {
        gameController = GameObject.Find( "GameController" ).GetComponent<GameController>( );
    }

	// Notifications
	public const string DisconnectedServer = "CatanNetworkManager.DisconnectedServer";

    public override void OnClientConnect( NetworkConnection conn )
    {
        DebugConsole.Log( "CatanNetworkManager::OnClientConnection" );
        base.OnClientConnect( conn );
    }

	// called on clients when disconnected from a server
    public override void OnClientDisconnect( NetworkConnection conn )
    {
        DebugConsole.Log( "CatanNetworkManager::OnClientDisconnect" );
        base.OnClientDisconnect( conn );

        // The client has disconnected from the server
        // Reset the local gameController
        DebugConsole.Log( "Players in match: " + NetworkManager.singleton.numPlayers );
        gameController.ResetGame( );
    }

    public override void OnClientError( NetworkConnection conn, int errorCode )
    {
        DebugConsole.Log( "CatanNetworkManager::OnClientError" );
        base.OnClientError( conn, errorCode );
    }

    public override void OnServerConnect( NetworkConnection conn )
    {
        DebugConsole.Log( "CatanNetworkManager::OnServerConnect" );
        base.OnServerConnect( conn );
    }

    public override void OnServerDisconnect( NetworkConnection conn )
    {
        DebugConsole.Log( "CatanNetworkManager::OnServerDisconnect" );
        base.OnServerDisconnect( conn );
    }

    public override void OnServerError( NetworkConnection conn, int errorCode )
    {
        DebugConsole.Log( "CatanNetworkManager::OnServerError" );
        base.OnServerError( conn, errorCode );
    }

    public override void OnServerReady( NetworkConnection conn )
    {
        DebugConsole.Log( "CatanNetworkManager::OnServerReady" );
        base.OnServerReady( conn );
    }

    public override void OnMatchCreate( bool success, string extendedInfo, MatchInfo matchInfo )
    {
        DebugConsole.Log( "CatanNetworkManager::OnMatchCreate" );
        base.OnMatchCreate( success, extendedInfo, matchInfo );
    }

    public override void OnMatchJoined( bool success, string extendedInfo, MatchInfo matchInfo )
    {
        DebugConsole.Log( "CatanNetworkManager::OnMatchJoined" );
        base.OnMatchJoined( success, extendedInfo, matchInfo );
    }

    public override void OnDestroyMatch( bool success, string extendedInfo )
    {
        DebugConsole.Log( "CatanNetworkManager::OnDestroyMatch" );
        base.OnDestroyMatch( success, extendedInfo );
    }

    public override void OnDropConnection( bool success, string extendedInfo )
    {
        DebugConsole.Log( "CatanNetworkManager::OnDropConnection" );
        base.OnDropConnection( success, extendedInfo );
    }

    public override void OnStartHost( )
    {
        DebugConsole.Log( "CatanNetworkManager::OnStartHost" );
        base.OnStartHost( );
    }

    public override void OnStopHost( )
    {
        DebugConsole.Log( "CatanNetworkManager::OnStopHost" );
        base.OnStopHost( );
    }

    public override void OnStartServer( )
    {
        DebugConsole.Log( "CatanNetworkManager::OnStartServer" );
        base.OnStartServer( );
    }

    public override void OnStopServer( )
    {
        DebugConsole.Log( "CatanNetworkManager::OnStopServer" );
        base.OnStopServer( );
    }

    public override void OnStartClient( NetworkClient client )
    {
        DebugConsole.Log( "CatanNetworkManager::OnStartClient" );
        base.OnStartClient( client );
    }

    public override void OnStopClient( )
    {
        DebugConsole.Log( "CatanNetworkManager::OnStopClient" );
        base.OnStopClient( );
    }

    public override void OnServerAddPlayer( NetworkConnection conn, short playerControllerId )
    {
        DebugConsole.Log( "CatanNetworkManager::OnServerAddPlayer" );
        base.OnServerAddPlayer( conn, playerControllerId );
    }

    public override void OnServerAddPlayer( NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader )
    {
        DebugConsole.Log( "CatanNetworkManager::OnServerAddPlayer(extended)" );
        base.OnServerAddPlayer( conn, playerControllerId, extraMessageReader );
    }

    public override void OnServerRemovePlayer( NetworkConnection conn, UnityEngine.Networking.PlayerController player )
    {
        DebugConsole.Log( "CatanNetworkManager::OnServerRemovePlayer" );
        base.OnServerRemovePlayer( conn, player );
    }
}
