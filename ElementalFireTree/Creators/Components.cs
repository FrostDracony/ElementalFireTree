using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Console = SRML.Console.Console;
using SRML.Utils;
using MonomiPark.SlimeRancher.Regions;
using ElementalFireTree;
using DG.Tweening;
using SRML;

namespace Creators
{

/*	public class MoveTowardsCombinator : MonoBehaviour
    {
		public StartFusionProcess combinator;
		public float speed;
		public float limitDistance;
		public bool liquidArrived;
		void Start()
        {
			("Combinator is: " + combinator.name).Log();
        }
		void Update()
        {
			//"FirstUpdateLine".Log();
			var step = speed * Time.deltaTime; // calculate distance to move

			//"SecondUpdateLine".Log();
			transform.position = Vector3.MoveTowards(transform.position, combinator.transform.position, step);

			*//*"Following field is: ".Log();

			combinator.GetType().GetField(valueToChangeAtEnd).Name.Log();*//*

			//"Oh hey, no error over here".Log();

			if (Vector3.Distance(transform.position, combinator.transform.position) < limitDistance) //0.001f
			{
				if(liquidArrived)
					combinator.liquidArrived = true;
				else
					combinator.slimeArrived = true;

				Destroy(gameObject);
			}
		}

    }
*/
	public class CrystalAbsorbElementalFire : MonoBehaviour
    {
		int counter = 5;
		public void OnCollisionEnter(Collision col)
        {
			Identifiable ident = col.gameObject.GetComponent<Identifiable>();
			if (ident != null && ident.id == Ids.FIRE_LIQUID)
            {
				counter--;
				if(counter == 0)
                {
					SRBehaviour.InstantiateActor(
						SRSingleton<GameContext>.Instance.LookupDirector.GetPrefab(Ids.ELEMENTAL_FIRE_ENHANCER),
						SceneContext.Instance.Player.GetComponent<RegionMember>().setId,
						transform.position,
						transform.rotation
					);
					Destroyer.DestroyActor(gameObject, "transforming using elemental fire liquid");
                }
            }
        }

	}

	public class StartFusionProcess : MonoBehaviour
    {
		public float radius = .001f;
		public List<GameObject> objectsInRadius = new List<GameObject>();
		public List<GameObject> timeSlowedObjs = new List<GameObject>();
		//public Dictionary<GameObject, Identifiable.Id> objectsInRadius = new Dictionary<GameObject, Identifiable.Id>();

		public bool liquidArrived = false;
		public bool slimeArrived = false;

		void Start()
        {
			"Start of StartFusionProcess Component".Log();
			//transform.localScale = new Vector3(radius, radius, radius);
        }

/*		void Update()
        {
			if(slimeArrived && liquidArrived)
            {
				SRBehaviour.InstantiateActor(
					SRSingleton<GameContext>.Instance.LookupDirector.GetPrefab(Ids.ELEMENTAL_FIRE_SLIME), 
					SceneContext.Instance.Player.GetComponent<RegionMember>().setId, 
					true);
				Destroy(gameObject);
            }
        }*/

        void OnTriggerEnter(Collider other)
        {
			GameObject gameObject = other.gameObject;
			Identifiable idenComponent = gameObject.GetComponent<Identifiable>();
			if (idenComponent == null)
				return;
			
			/*("contains fireliquid? " + objectsInRadius.ContainsValue(Ids.FIRE_LIQUID)).Log();
			("contains both? " + (objectsInRadius.ContainsValue(Identifiable.Id.FIRE_SLIME)
				|| objectsInRadius.ContainsValue(Ids.FIRE_LIQUID))).Log();*/

			/*!objectsInRadius.FirstOrDefault(x =>
				x.GetComponent<Identifiable>().id == idenComponent.id))*/

			//if Id isn't null, and if there isn't another liquid and/or slime already present in the radius
			if (idenComponent != null && 
				objectsInRadius
				.Where(x => x?.GetComponent<Identifiable>().id == idenComponent.id)
				.Count() == 0
				)
            {
				("gameObject's name: " + gameObject.name + " with Id: " + idenComponent.id).Log();
				("Containers of objectsInRadius:").Log();
				int i1 = 0;
				foreach (GameObject obj in objectsInRadius)
				{
					("	" + obj).Log();
					i1.Log();
					i1++;
				}
				"Adding this to the radius".Log();
				objectsInRadius.Add(gameObject);
				//objectsInRadius.Add(gameObject, idenComponent.id);

				if (gameObject.GetComponent<Identifiable>().id == Identifiable.Id.FIRE_SLIME
					|| gameObject.GetComponent<Identifiable>().id == Ids.FIRE_LIQUID)
                {
					//gameObject.GetComponent<Rigidbody>().velocity /= 1.1f;
					gameObject.GetComponent <Rigidbody>().isKinematic = true;

					timeSlowedObjs.Add(gameObject);
				}


				if (RequirementsCompleted())
                {
					GameObject slimeFire = objectsInRadius.First(x => x.GetComponent<Identifiable>().id == Identifiable.Id.FIRE_SLIME);
					GameObject liquidFire = objectsInRadius.First(x => x.GetComponent<Identifiable>().id == Ids.FIRE_LIQUID);
					
					/*GameObject slimeFire = SRBehaviour.InstantiateActor(
						SRSingleton<GameContext>.Instance.LookupDirector.GetPrefab(Identifiable.Id.FIRE_SLIME), 
						SceneContext.Instance.Player.GetComponent<RegionMember>().setId,
						_slimeFire.GetComponent<Transform>().position,
						_slimeFire.GetComponent<Transform>().rotation,
						true);

					GameObject liquidFire = SRBehaviour.InstantiateActor(
						SRSingleton<GameContext>.Instance.LookupDirector.GetPrefab(Ids.FIRE_LIQUID),
						SceneContext.Instance.Player.GetComponent<RegionMember>().setId,
						_liquidFire.GetComponent<Transform>().position,
						_liquidFire.GetComponent<Transform>().rotation,
						true);*/

					/*Destroyer.D(_slimeFire);
					Destroy(_liquidFire);*/


					"Both GameObjects are there".Log();

					/*Rigidbody rigidBodyFireSlime = slimeFire.GetComponent<Rigidbody>();
					Rigidbody rigidBodyLiquidFire = liquidFire.GetComponent<Rigidbody>();*/

					"Both RigidBodies".Log();

					//Destroy(slimeFire.GetComponent<PuddleSlimeScoot>());
					Destroy(liquidFire.GetComponent<DestroyOnTouching>());

					"Destroying the combonents".Log();

					/*rigidBodyFireSlime.isKinematic = true;
					rigidBodyLiquidFire.isKinematic = true;*/

					"isKinematic set".Log();

					/*rigidBodyFireSlime.detectCollisions = false;
					rigidBodyLiquidFire.detectCollisions = false;*/

					"detectCollisions set".Log();
					
					/*slimeFire.GetComponent<Collider>().isTrigger = false;
					liquidFire.GetComponent<Collider>().isTrigger = false;*/

					"Lets UnTimeBlock the 2 items now".Log();
					//slimeFire.GetComponent<Rigidbody>().velocity *= 1.1f;
					slimeFire.GetComponent<Rigidbody>().isKinematic = false;

					"Kekw?".Log();
					//liquidFire.GetComponent<Rigidbody>().velocity *= 1.1f;
					liquidFire.GetComponent<Rigidbody>().isKinematic = false;

					Destroyer.DestroyActor(this.gameObject.transform.parent.gameObject, "Elemental Enhancer Finished Enhancing");
					
					/*MoveTowardsCombinator mtcSlime = slimeFire.AddComponent<MoveTowardsCombinator>();
					MoveTowardsCombinator mtcLiquid = liquidFire.AddComponent<MoveTowardsCombinator>();
					
					"MTC created".Log();

					SetMTC(mtcSlime, this, 1f, false, radius);
					SetMTC(mtcLiquid, this, 1f, true, radius);

					"MTC set".Log();*/
				}

			}
        }

		void OnTriggerExit(Collider other)
        {
			/*if (!objectsInRadius.Contains(other.gameObject))
				return;*/
			Identifiable idenComponent = other.gameObject.GetComponent<Identifiable>();
			("OnTriggerExit for " + other.gameObject).Log();

			for(int i = 0;  i < timeSlowedObjs.Count; i++)
			{
				GameObject gameObject = timeSlowedObjs[i].gameObject;
				"No way it's this... right? right?".Log();
				("Lets see gameObject: " + gameObject).Log();

				if (gameObject != null)
				{
					//gameObject.GetComponent<Rigidbody>().velocity *= 1.1f;
					gameObject.GetComponent<Rigidbody>().isKinematic = false;

					timeSlowedObjs.Remove(gameObject);
					"as if...".Log();
				}
			}
			"This is fine".Log();
			if (idenComponent != null && objectsInRadius.Contains(other.gameObject)) //objectsInRadius.Contains(other.gameObject)
			{
				"This... too?".Log();

				("Object being removed has an Id of: " + idenComponent.id).Log();
				objectsInRadius.Remove(other.gameObject);
				"FOUND YA BITC-".Log();
			}
		}

		/*void SetMTC(MoveTowardsCombinator mtc, StartFusionProcess combinator, float speed, bool liquidArrived, float limitDistance)
        {
			mtc.combinator = combinator;
			mtc.speed = speed;
			mtc.liquidArrived = liquidArrived;
			mtc.limitDistance = limitDistance;
        }*/

		GameObject FindByIdAndNameInList(Identifiable.Id IdOfObjectToFind, string name)
        {
			foreach(GameObject content in objectsInRadius)
            {
				if(content.GetComponent<Identifiable>().id == IdOfObjectToFind && content.name == name)
					return content;
            }

			return null;
        }

		bool RequirementsCompleted()
        {
			bool fireSlime = false;
			bool liquidFire = false;

			foreach (GameObject content in objectsInRadius)
			{
				if (content.GetComponent<Identifiable>().id == Identifiable.Id.FIRE_SLIME)
					fireSlime = true;

				if (content.GetComponent<Identifiable>().id == Ids.FIRE_LIQUID)
					liquidFire = true;

				if(fireSlime && liquidFire)
					return true;
			}

			return false;
		}

	}

	public class FireLiquidIgnition :
  RegisteredActorBehaviour,
  Ignitable,
  LiquidConsumer,
  RegistryUpdateable,
  ControllerCollisionListener
	{
		private bool isIgnited;
		//private GameObject fireFXObj;
		private readonly double reigniteAtTime = double.PositiveInfinity;
		private TimeDirector timeDir;

		public void Awake() => timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;

		public override void Start()
		{
			base.Start();
			/*GameObject fireFXObj = ElementalFireTree.Main.assetBundle.LoadAsset<GameObject>("ElementalFireParticles");
			fireFXObj.transform.position = new Vector3(0, 0.5F, 0);

			if (Shader.Find("SR/Particles/Additive") != null)
			{
				foreach (ParticleSystemRenderer particleSystemRenderer in fireFXObj.GetComponentsInChildren<ParticleSystemRenderer>())
				{
					particleSystemRenderer.material.shader = Shader.Find("SR/Particles/Additive");
				}
			}*/
			Ignite(gameObject);
		}

		public void OnCollisionEnter(Collision col)
		{
			if (!isIgnited)
				return;
			col.gameObject.GetComponent<Ignitable>()?.Ignite(gameObject);
		}

		public void OnControllerCollision(GameObject gameObj)
		{
			if (!isIgnited)
				return;
			gameObj.GetComponent<Ignitable>()?.Ignite(gameObject);
		}

		public void RegistryUpdate()
		{
			if (!timeDir.HasReached(reigniteAtTime) || isIgnited)
				return;
			Ignite(gameObject);
		}

		public void Ignite(GameObject igniter)
		{
			isIgnited = true;
			/*if (!(fireFXObj != null))
				return;
			fireFXObj.SetActive(true);*/
		}

		public void AddLiquid(Identifiable.Id liquidId, float units)
		{
		}
	}


	public class ElementalFireSlimeIgnition :
  RegisteredActorBehaviour,
  Ignitable,
  LiquidConsumer,
  RegistryUpdateable,
  ControllerCollisionListener
    {
		public bool isIgnited;
		public GameObject fireFXObj;
		public double reigniteAtTime = double.PositiveInfinity;
		public TimeDirector timeDir;
		public List<LiquidSource> waterSources = new List<LiquidSource>();
		public const float EXTINGUISH_HRS = 0.5f;
		public int touchedByWater = 0;
		public void Awake() 
		{
			"Wait this shouldn't be working right now".Log();
			timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;

		}

		public override void Start()
		{
			"Neither should this".Log();

			isIgnited = !gameObject.GetComponent<ChangeParticlesAngry>().blocked;
			("isIgnited at the start is now: " + !isIgnited).Log();
			base.Start();
			ExtractFire();
			GetComponent<SlimeAppearanceApplicator>().OnAppearanceChanged += appearance => ExtractFire();
			Ignite(gameObject);
		}

		public void ExtractFire()
		{
			fireFXObj = gameObject.FindChild("ElementalFireParticles(Clone)");
			fireFXObj.SetActive(isIgnited);
		}

		public void OnCollisionEnter(Collision col)
		{
			if (!isIgnited)
				return;
			col.gameObject.GetComponent<Ignitable>()?.Ignite(gameObject);
			//"if it worked so far...".Log();
		}

		public void OnControllerCollision(GameObject gameObj)
		{
			if (!isIgnited)
				return;
			
			gameObj.GetComponent<Ignitable>()?.Ignite(gameObject);
		}

		public void OnTriggerEnter(Collider col)
		{
			LiquidSource component = col.gameObject.GetComponent<LiquidSource>();

			if (!(component != null) || !Identifiable.IsWater(component.liquidId))
				return;
			waterSources.Add(component);

			if (waterSources.Count > 0)
				Extinguish();
		}

		public void OnTriggerExit(Collider col)
		{
			LiquidSource component = col.gameObject.GetComponent<LiquidSource>();
			if (!(component != null) || !Identifiable.IsWater(component.liquidId))
				return;
			waterSources.Remove(component);
		}

		public void RegistryUpdate()
		{
			if (isIgnited || !timeDir.HasReached(reigniteAtTime))
				return;
			Ignite(gameObject);
		}

		public void Ignite(GameObject igniter)
		{
			waterSources.RemoveAll(w => w == null || w.gameObject == null);
			if (waterSources.Count > 0)
				return;
			touchedByWater = 0;
			isIgnited = true;
			if (!(fireFXObj != null))
				return;
			fireFXObj.SetActive(true);
			gameObject.GetComponent<DamagePlayerOnTouch>().damagePerTouch = 140;
		}

		public void Extinguish()
		{
			isIgnited = false;
			if (fireFXObj != null)
				fireFXObj.SetActive(false);
			reigniteAtTime = timeDir.HoursFromNow(0.5f);
			gameObject.GetComponent<DamagePlayerOnTouch>().damagePerTouch = 20;
		}

		public void AddLiquid(Identifiable.Id liquidId, float units)
		{
			"wait... NANI, NO WAY THIS IS ACTALLY FIRING?!".Log();
			if (!Identifiable.IsWater(liquidId))
				return;
			touchedByWater++;
			if (touchedByWater > 2)
				Extinguish();
		}
	}

	public class ChangeParticlesAngry : SlimeSubbehaviour
	{
		public bool blocked = false;

		/*float duration;
		float startDuration = 0;
		float endDuration = 10;

		float timeBtwUse;
		public float startTimeBtwUse = 10;*/

		public GameObject projectile = null; //SRObjects.GetInst<GameObject>("FireBall"); //SRSingleton<GameContext>.Instance.LookupDirector.GetPrefab(Identifiable.Id.VALLEY_AMMO_1);
		public GameObject cannonProjectile = SRSingleton<GameContext>.Instance.LookupDirector.GetPrefab(Identifiable.Id.VALLEY_AMMO_2);

		float timeBtwCannonShots;
		public float startTimeBtwCannonShots = SceneContext.Instance.GameModeConfig.gameModel.currGameMode == PlayerState.GameMode.CLASSIC ? 15 : 30;

		float timeBtwShots;
		//public float startTimeBtwShots = SceneContext.Instance.GameModeConfig.gameModel.currGameMode == PlayerState.GameMode.CLASSIC ? 6 : 10;
		public float startTimeBtwShots = SceneContext.Instance.GameModeConfig.gameModel.currGameMode == PlayerState.GameMode.CLASSIC ? 2 : 2;

		public bool canShoot = true;

		public override void Selected()
		{
		}

		public override void Start()
		{
			//duration = startDuration;
			timeBtwShots = startTimeBtwShots;
			//timeBtwUse = startTimeBtwUse;
			timeBtwCannonShots = startTimeBtwCannonShots;
			base.Start();
		}

		public override void Awake() => base.Awake();

		public override void Action()
		{

			if (emotions.GetCurr(SlimeEmotions.Emotion.AGITATION) >= 0.6f)
			{

				if (!blocked)
				{
					gameObject.GetComponentInChildren<ParticleSystem>().gameObject.SetActive(false);

					/*GameObject particles = Main.assetBundle.LoadAsset<GameObject>("MagicCircleVFX");
					particles.transform.position = new Vector3(0, 0.25F, 0);
					if (Shader.Find("SR/Particles/Additive") != null)
					{
						foreach (ParticleSystemRenderer particleSystemRenderer in particles.GetComponentsInChildren<ParticleSystemRenderer>())
						{
							particleSystemRenderer.material.shader = Shader.Find("SR/Particles/Additive");
						}
					}

					Instantiate(particles, transform);*/
					blocked = true;
					//GetComponent<ChangeParticlesNormal>().blocked = false;
				}

				void CreateShot(Vector3 origin, Vector3 direction)
				{
					GameObject Shoot = SRObjects.GetInst<GameObject>("FireBall"); ;
					Shoot.AddComponent<DamagePlayerOnTouch>().GetCopyOf(SRSingleton<GameContext>.Instance.LookupDirector.GetPrefab(Identifiable.Id.ROCK_SLIME).GetComponent<DamagePlayerOnTouch>());
					Shoot.GetComponent<DamagePlayerOnTouch>().damagePerTouch = 250;

					WeaponVacuum weaponVacuum = FindObjectOfType<WeaponVacuum>();
					vp_FPController componentInParent = FindObjectOfType<vp_FPController>();

					Vector3 velocity = direction * weaponVacuum.ejectSpeed * 0.05f + (componentInParent.Velocity);
					GameObject Shot = InstantiateActor(Shoot, weaponVacuum.GetPrivateField<RegionRegistry>("regionRegistry").GetCurrentRegionSetId(), origin + direction.normalized * 1.5f, Quaternion.identity, false);
					Shot.transform.LookAt(weaponVacuum.transform);

					PhysicsUtil.RestoreFreezeRotationConstraints(Shot);

					Shot.GetComponent<Rigidbody>().velocity = velocity;

					//Shot.GetComponent<Vacuumable>().Launch(Ids.NONE);
					//Shot.GetComponent<Vacuumable>().size = Vacuumable.Size.GIANT;
						
					Shot.transform.DOScale(Shot.transform.localScale, 0.1f).From(Shot.transform.localScale * 0.2f, true).SetEase(Ease.Linear);
				}

				if (timeBtwShots <= 0 && canShoot)
				{
					Vector3 targetPosition = SRSingleton<SceneContext>.Instance.Player.transform.position + new Vector3(0, -3f, 0);
					CreateShot(gameObject.transform.position + new Vector3(0, 3, 0), targetPosition - (gameObject.transform.position + new Vector3(0, 1.5f, 0)));

					timeBtwShots = startTimeBtwShots;
				}
				else
				{
					timeBtwShots -= Time.deltaTime;
				}
			}

			if (emotions.GetCurr(SlimeEmotions.Emotion.AGITATION) < 0.6f)
            {
				if (blocked)
				{
					gameObject.GetComponentInChildren<ParticleSystem>().gameObject.SetActive(true);

					blocked = false;
					//GetComponent<ChangeParticlesNormal>().blocked = false;
				}
			}

			/*if (emotions.GetCurr(SlimeEmotions.Emotion.AGITATION) >= 0.85f)
			{
			}*/

			
		}

		public override float Relevancy(bool isGrounded)
		{
			if (emotions.GetCurr(SlimeEmotions.Emotion.AGITATION) >= 0.5f)
			{
				return 1f;
			}
			else if (emotions.GetCurr(SlimeEmotions.Emotion.AGITATION) < 0.5f)
			{

				/*GameObject particles = ElementalFireTree.Main.assetBundle.LoadAsset<GameObject>("ElementalFireParticles");
				particles.transform.position = new Vector3(0, 0.5F, 0);

				if (Shader.Find("SR/Particles/Additive") != null)
				{
					foreach (ParticleSystemRenderer particleSystemRenderer in particles.GetComponentsInChildren<ParticleSystemRenderer>())
					{
						particleSystemRenderer.material.shader = Shader.Find("SR/Particles/Additive");
					}
				}

				Instantiate(particles, transform);*/


				/*if (transform.Find("MagicCircleVFX(Clone)"))
				{
					Destroy(transform.Find("MagicCircleVFX(Clone)").gameObject);
					GameObject particles = Main.assetBundle.LoadAsset<GameObject>("ElectricSparklesSlimes");
					particles.transform.position = new Vector3(0, 0.5F, 0);

					if (Shader.Find("SR/Particles/Additive") != null)
					{
						foreach (ParticleSystemRenderer particleSystemRenderer in particles.GetComponentsInChildren<ParticleSystemRenderer>())
						{
							particleSystemRenderer.material.shader = Shader.Find("SR/Particles/Additive");
						}
					}

					Instantiate(particles, transform);
				}*/
			}

			return 0f;
		}

	}
}
