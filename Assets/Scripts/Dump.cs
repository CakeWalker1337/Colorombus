using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dump : MonoBehaviour {

	public static AudioClip buttonClip { get; set; }
	public static AudioClip backgroundClip { get; set; }
	public static AudioClip putBlockClip { get; set; }
	public static AudioClip coinClip { get; set; }
	public static AudioClip endClip { get; set; }
	public static AudioClip destroyClip { get; set; }
	public static AudioClip bombClip { get; set; }
	public static AudioClip laserClip { get; set; }
	public static AudioClip skullClip { get; set; }

	public static Sprite soundOnSprite { get; set; }
	public static Sprite soundOffSprite { get; set; }
	public static Sprite musicOnSprite { get; set; }
	public static Sprite musicOffSprite { get; set; }
	public static Sprite effectsOnSprite { get; set; }
	public static Sprite effectsOffSprite { get; set; }

	public static Sprite slotSpriteRectangle{ get; set;}
	public static Sprite triSprite{ get; set;}
	public static Sprite newRectSprite{ get; set;}
	public static Sprite oldRectSprite{ get; set;}

	public static Sprite bombSprite{ get; set;}
	public static Sprite electricSprite{ get; set;}
	public static Sprite skullSprite{ get; set;}
	public static Sprite superBombSprite{ get; set;}
	public static Sprite superElectricSprite{ get; set;}
	public static Sprite superSkullSprite{ get; set;}
	public static Sprite doubleMultSprite{ get; set;}
	public static Sprite coinSprite{ get; set;}

	public static GameObject electricParticleSmall{ get; set;}
	public static GameObject electricParticleBig{ get; set;}
	public static GameObject devastateParticle{ get; set;}
	public static GameObject explosionParticle{ get; set;}
	public static GameObject hitScore{ get; set;}
	public static RuntimeAnimatorController spawnController{ get; set;}
	public static RuntimeAnimatorController scoreController{ get; set;}
	public static Color RedColor{ get; set;}
	public static Color OrangeColor{ get; set;}
	public static Color YellowColor{ get; set;}
	public static Color GreenColor{ get; set;}
	public static Color BlueColor{ get; set;}
	public static Color DarkBlueColor{ get; set;}
	public static Color PurpleColor{ get; set;}

	// Use this for initialization
	public static void Init () {
		
		buttonClip = Resources.Load<AudioClip> ("Music/button") as AudioClip;
		backgroundClip = Resources.Load<AudioClip> ("Music/background") as AudioClip;
		putBlockClip = Resources.Load<AudioClip> ("Music/put") as AudioClip;
		coinClip = Resources.Load<AudioClip> ("Music/combo") as AudioClip;
		endClip = Resources.Load<AudioClip> ("Music/end") as AudioClip;
		destroyClip = Resources.Load<AudioClip> ("Music/destroy") as AudioClip;
		bombClip = Resources.Load<AudioClip> ("Music/bomb") as AudioClip;
		laserClip = Resources.Load<AudioClip> ("Music/laser") as AudioClip;
		skullClip = Resources.Load<AudioClip> ("Music/skull") as AudioClip;

		soundOnSprite = Resources.Load<Sprite> ("Textures/Icons/sound_on");
		soundOffSprite = Resources.Load<Sprite> ("Textures/Icons/sound_off");
		musicOnSprite = Resources.Load<Sprite> ("Textures/Icons/music_on");
		musicOffSprite = Resources.Load<Sprite> ("Textures/Icons/music_off");
		effectsOnSprite = Resources.Load<Sprite> ("Textures/Icons/effects_on");
		effectsOffSprite = Resources.Load<Sprite> ("Textures/Icons/effects_off");

		triSprite = Resources.Load<Sprite> ("Textures/Shapes/triangle");
		slotSpriteRectangle = Resources.Load<Sprite> ("Textures/Shapes/four");
		newRectSprite = Resources.Load<Sprite> ("Textures/Shapes/flat_rectangle_radiance_small_border");
		oldRectSprite = Resources.Load<Sprite> ("Textures/Shapes/rectangle");

		bombSprite = Resources.Load<Sprite> ("Textures/Icons/bomb");
		electricSprite = Resources.Load<Sprite> ("Textures/Icons/flash");
		skullSprite = Resources.Load<Sprite> ("Textures/Icons/skull");

		superBombSprite = Resources.Load<Sprite> ("Textures/Icons/super_bomb");
		superElectricSprite = Resources.Load<Sprite> ("Textures/Icons/super_flash");
		superSkullSprite = Resources.Load<Sprite> ("Textures/Icons/super_skull");

		coinSprite = Resources.Load<Sprite> ("Textures/Icons/coin");
		doubleMultSprite = Resources.Load<Sprite> ("Textures/Icons/x2");

		electricParticleSmall = Resources.Load<GameObject> ("Particles/LaserEffectSmall") as GameObject;
		electricParticleBig = Resources.Load<GameObject> ("Particles/LaserEffectBig") as GameObject;
		devastateParticle = Resources.Load<GameObject> ("Particles/DevastateEffect") as GameObject;
		explosionParticle = Resources.Load<GameObject> ("Particles/ExplosionEffect") as GameObject;
		hitScore = Resources.Load<GameObject> ("Prefabs/HitScore") as GameObject;
		spawnController = Resources.Load<RuntimeAnimatorController> ("Animations/spawnController") as RuntimeAnimatorController;
		scoreController = Resources.Load<RuntimeAnimatorController> ("Animations/scoreController") as RuntimeAnimatorController;

		LoadColors ();
	}

	/// <summary>
	/// Loads the colors.
	/// </summary>
	public static void LoadColors(){
		if (GameController.Design == GameController.DESIGN_NEW) {
			RedColor = new Color (1f, 0.388f, 0.278f);
			OrangeColor = new Color (0.91f, 0.64f, 0.23f);
			YellowColor = new Color (0.88f, 0.88f, 0.02f);
			GreenColor = new Color (0.19f, 0.84f, 0.48f);
			BlueColor = new Color (0.02f, 0.69f, 0.82f);
			DarkBlueColor = new Color (0.06f, 0.25f, 0.82f);
			PurpleColor = new Color (0.85f, 0f, 0.79f);
		}
		else
		{
			RedColor = new Color (1f, 0.267f, 0.267f);
			OrangeColor = new Color (1f, 0.722f, 0.22f);
			YellowColor = new Color (0.965f, 1f, 0.235f);
			GreenColor = new Color (0.211f, 1f, 0.38f);
			BlueColor = new Color (0.27f, 1f, 0.941f);
			DarkBlueColor = new Color (0.11f, 0.373f, 1f);
			PurpleColor = new Color (0.988f, 0.278f, 1f);
		}
	}

}
