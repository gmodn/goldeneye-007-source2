﻿


partial class DeathmatchGame : Game
{
	public static GameStates CurrentState => (Current as DeathmatchGame)?.GameState ?? GameStates.Warmup;

	[Net]
	public RealTimeUntil StateTimer { get; set; } = 0f;

	[Net]
	public GameStates GameState { get; set; } = GameStates.Warmup;
	[Net]
	public string NextMap { get; set; } = "facepunch.datacore";

	[ConCmd.Admin]
	public static void SkipStage()
	{
		if ( Current is not DeathmatchGame dmg ) return;

		dmg.StateTimer = 1;
	}

	private async Task WaitStateTimer()
	{
		while ( StateTimer > 0 )
		{
			await Task.DelayRealtimeSeconds( 1.0f );
		}

		// extra second for fun
		await Task.DelayRealtimeSeconds( 1.0f );
	}

	private async Task GameLoopAsync()
	{
		GameState = GameStates.Warmup;
		StateTimer = 10;
		await WaitStateTimer();

		GameState = GameStates.Live;
		StateTimer = 10 * 60;
		FreshStart();
		await WaitStateTimer();

		GameState = GameStates.GameEnd;
		StateTimer = 10.0f;
		await WaitStateTimer();

		GameState = GameStates.MapVote;
		var mapVote = new MapVoteEntity();
		mapVote.VoteTimeLeft = 10.0f;
		StateTimer = mapVote.VoteTimeLeft;
		await WaitStateTimer();

		Global.ChangeLevel( mapVote.WinningMap );
	}

	private bool HasEnoughPlayers()
	{
		if ( All.OfType<DeathmatchPlayer>().Count() < 2 )
			return false;

		return true;
	}

	private void FreshStart()
	{
		foreach ( var cl in Client.All )
		{
			cl.SetInt( "kills", 0 );
			cl.SetInt( "deaths", 0 );
		}

		All.OfType<DeathmatchPlayer>().ToList().ForEach( x =>
		{
			x.Respawn();
		} );
	}

	public enum GameStates
	{
		Warmup,
		Live,
		GameEnd,
		MapVote
	}

}
