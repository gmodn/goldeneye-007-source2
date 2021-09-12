using Sandbox;


[Library( "dm_pistol", Title = "Silenced PP7" )]
[Hammer.EditorModel( "weapons/rust_pistol/rust_pistol.vmdl" )]
partial class Pistol : BaseDmWeapon
{ 
	public override string ViewModelPath => "models/weapons/pp7_s/v_pp7.vmdl";

	public override float PrimaryRate => 16.0f;
	public override float SecondaryRate => 1.0f;
	public override float ReloadTime => 1.0f;

	public override int Bucket => 1;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/weapons/pp7_s/w_pp7_s.vmdl" );
		AmmoClip = 7;
	}

	public override bool CanPrimaryAttack()
	{
		return base.CanPrimaryAttack() && Input.Pressed( InputButton.Attack1 );
	}

	public override void AttackPrimary()
	{
		TimeSincePrimaryAttack = 16;
		TimeSinceSecondaryAttack = 0;

		if ( !TakeAmmo( 1 ) )
		{
			DryFire();
			return;
		}


		//
		// Tell the clients to play the shoot effects
		//
		ShootEffects();
		PlaySound( "pp7s.fire" );

		//
		// Shoot the bullets
		//
		//Rand.SetSeed( Time.Tick );
		ShootBullet( 0.05f, 1.5f, 9.0f, 3.0f );
	}
	
	public override void Reload()
	{
		if ( IsReloading )
			return;

		if ( AmmoClip >= ClipSize )
			return;

		TimeSinceReload = 0;

		if ( Owner is DeathmatchPlayer player )
		{
			if ( player.AmmoCount( AmmoType ) <= 0 )
				return;
		}

		IsReloading = true;

		(Owner as AnimEntity).SetAnimBool( "b_reload", true );

		StartReloadEffects();
		
		PlaySound( "reload" );
	}
}
