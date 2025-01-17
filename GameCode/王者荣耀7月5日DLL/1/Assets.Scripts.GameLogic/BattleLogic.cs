using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using Assets.Scripts.Sound;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class BattleLogic : Singleton<BattleLogic>, IUpdateLogic
	{
		public const int SKILL_LEVEL_MAX = 6;

		public const int SKILL3_LEVEL_MAX = 3;

		public bool isRuning;

		public bool isFighting;

		public bool isGameOver;

		public bool isWaitMultiStart;

		public bool isWaitGameEnd;

		public CSPkg m_cachedSvrEndData;

		public SLevelContext m_LevelContext;

		public Vector2 m_battleSceneSize = new Vector2(133f, 22f);

		public MapWrapper mapLogic;

		public SoldierControl soldierCtrl;

		public BattleStatistic battleStat;

		public IncomeControl incomeCtrl;

		public CBattleValueAdjust valAdjustCtrl = new CBattleValueAdjust();

		public AttackOrder attackOrder = new AttackOrder();

		public DynamicProperty dynamicProperty = new DynamicProperty();

		public ClashAddition clashAddition = new ClashAddition();

		public DynamicDifficulty dynamicDifficulty = new DynamicDifficulty();

		public GameTaskSys battleTaskSys = new GameTaskSys();

		public Horizon horizon = new Horizon();

		public HostPlayerLogic hostPlayerLogic = new HostPlayerLogic();

		public bool IsModifyingCamera;

		public SpawnGroup m_dragonSpawn;

		private Dictionary<uint, int> s_dragonBuffIds;

		public string m_countDownTips;

		private static int m_DelayForceKillCrystalCamp = -1;

		public bool m_bIsPayStat;

		private int m_totalLowFPSTime;

		private int m_lowFPSTimeDeadline = 20000;

		private int m_qualitySetting;

		private int m_qualitySettingParticle;

		private bool m_isUserConfirmedQualityDegrade;

		private bool m_needAutoCheckQUality = true;

		public float m_fAveFPS;

		public long m_fpsCount;

		public long m_fpsCunt18;

		public long m_fpsCunt10;

		public int m_iAutoLODState;

		private float m_FpsTimeBegin;

		public float m_LastFps;

		public float m_Ab_FPS_time;

		public float m_Ab_4FPS_time;

		public float m_Abnormal_FPS_Count;

		public float m_Abnormal_4FPS_Count;

		public GlobalTrigger m_globalTrigger
		{
			get;
			private set;
		}

		public int DragonId
		{
			get
			{
				if (this.m_dragonSpawn != null)
				{
					return this.m_dragonSpawn.TheActorsMeta[0].ConfigId;
				}
				return 0;
			}
		}

		public static void OnFailureEvaluationChanged(IStarEvaluation InStarEvaluation, IStarCondition InStarCondition)
		{
			if (Singleton<StarSystem>.get_instance().failureEvaluation == InStarEvaluation && Singleton<StarSystem>.get_instance().isFailure)
			{
				PoolObjHandle<ActorRoot> src;
				PoolObjHandle<ActorRoot> atker;
				InStarCondition.GetActorRef(out src, out atker);
				Singleton<BattleLogic>.get_instance().OnFailure(src, atker);
			}
		}

		public static void OnStarSystemChanged(IStarEvaluation InStarEvaluation, IStarCondition InStarCondition)
		{
			if (Singleton<StarSystem>.get_instance().winEvaluation == InStarEvaluation && Singleton<StarSystem>.get_instance().isFirstStarCompleted)
			{
				PoolObjHandle<ActorRoot> src;
				PoolObjHandle<ActorRoot> atker;
				InStarCondition.GetActorRef(out src, out atker);
				Singleton<BattleLogic>.get_instance().OnWinning(src, atker);
			}
		}

		public static void OnWinStarSysChanged(IStarEvaluation InStarEvaluation, IStarCondition InStarCondition)
		{
			if (Singleton<WinLoseByStarSys>.get_instance().WinnerEvaluation == InStarEvaluation && Singleton<WinLoseByStarSys>.get_instance().isSuccess)
			{
				PoolObjHandle<ActorRoot> src;
				PoolObjHandle<ActorRoot> atker;
				InStarCondition.GetActorRef(out src, out atker);
				Singleton<BattleLogic>.get_instance().OnWinning(src, atker);
			}
		}

		public static void OnLoseStarSysChanged(IStarEvaluation InStarEvaluation, IStarCondition InStarCondition)
		{
			if (Singleton<WinLoseByStarSys>.get_instance().LoserEvaluation == InStarEvaluation && Singleton<WinLoseByStarSys>.get_instance().isFailure)
			{
				PoolObjHandle<ActorRoot> src;
				PoolObjHandle<ActorRoot> atker;
				InStarCondition.GetActorRef(out src, out atker);
				Singleton<BattleLogic>.get_instance().OnFailure(src, atker);
			}
		}

		public void OnWinning(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker)
		{
			if (this.m_LevelContext != null && this.m_LevelContext.IsFireHolePlayMode())
			{
				if (Singleton<GamePlayerCenter>.get_instance().GetHostPlayer().PlayerCamp == 1)
				{
					BattleLogic.ForceKillCrystal(2);
				}
				else
				{
					BattleLogic.ForceKillCrystal(1);
				}
			}
			else
			{
				this.OnFinish(src, atker, this.m_LevelContext.m_passDialogId);
			}
		}

		public void OnFailure(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker)
		{
			if (this.m_LevelContext != null && this.m_LevelContext.IsFireHolePlayMode())
			{
				BattleLogic.ForceKillCrystal(Singleton<GamePlayerCenter>.get_instance().GetHostPlayer().PlayerCamp);
			}
			else
			{
				this.OnFinish(src, atker, this.m_LevelContext.m_failureDialogId);
			}
		}

		public void MakeAllHeroActorInvincible()
		{
			List<PoolObjHandle<ActorRoot>> heroActors = Singleton<GameObjMgr>.get_instance().HeroActors;
			int count = heroActors.get_Count();
			for (int i = 0; i < count; i++)
			{
				PoolObjHandle<ActorRoot> poolObjHandle = heroActors.get_Item(i);
				if (poolObjHandle && poolObjHandle.get_handle().ActorControl != null)
				{
					HeroWrapper heroWrapper = poolObjHandle.get_handle().ActorControl as HeroWrapper;
					if (heroWrapper != null)
					{
						heroWrapper.bGodMode = true;
					}
				}
			}
		}

		protected void OnFinish(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, int dialogID)
		{
			DefaultGameEventParam defaultGameEventParam = new DefaultGameEventParam(src, atker);
			Singleton<GameEventSys>.get_instance().SendEvent<DefaultGameEventParam>(GameEventDef.Event_FightOver, ref defaultGameEventParam);
			this.MakeAllHeroActorInvincible();
			if (dialogID > 0)
			{
				GameObject inSrc = (!src) ? null : src.get_handle().gameObject;
				GameObject inAtker = (!atker) ? null : atker.get_handle().gameObject;
				MonoSingleton<DialogueProcessor>.GetInstance().PlayDrama(dialogID, inSrc, inAtker, false);
			}
			else
			{
				DefaultGameEventParam defaultGameEventParam2 = new DefaultGameEventParam(src, atker);
				Singleton<GameEventSys>.get_instance().SendEvent<DefaultGameEventParam>(GameEventDef.Event_GameEnd, ref defaultGameEventParam2);
			}
		}

		public void BindFightPrepareFinListener()
		{
			Singleton<GameEventSys>.get_instance().AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightPrepareFin, new RefAction<DefaultGameEventParam>(this.OnFightPrepareFin));
		}

		private void OnFightPrepareFin(ref DefaultGameEventParam prm)
		{
			Singleton<GameEventSys>.get_instance().RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightPrepareFin, new RefAction<DefaultGameEventParam>(this.OnFightPrepareFin));
			this.DoBattleStart();
		}

		private void onActorDead(ref GameDeadEventParam prm)
		{
			if (prm.bImmediateRevive)
			{
				return;
			}
			if (this.m_LevelContext != null && this.m_LevelContext.IsMobaMode() && prm.src && ActorHelper.IsHostCtrlActor(ref prm.src))
			{
				Singleton<CSoundManager>.get_instance().PostEvent("Set_Dead_Low_Pass", null);
				this.SendHostPlayerDieOrReLive(1);
			}
			if (prm.src && prm.src.get_handle().TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
			{
				PoolObjHandle<ActorRoot> attker = default(PoolObjHandle<ActorRoot>);
				if (prm.orignalAtker && prm.orignalAtker.get_handle().TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
				{
					attker = prm.orignalAtker;
				}
				else if (prm.src.get_handle().ActorControl.IsKilledByHero())
				{
					attker = prm.src.get_handle().ActorControl.LastHeroAtker;
				}
				HashSet<uint> assistSet = BattleLogic.GetAssistSet(prm.src, attker, true);
				HashSet<uint>.Enumerator enumerator = assistSet.GetEnumerator();
				while (enumerator.MoveNext())
				{
					PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(enumerator.get_Current());
					if (actor)
					{
						prm.src.get_handle().ActorControl.NotifyAssistActor(ref actor);
					}
				}
			}
		}

		private void onActorRevive(ref DefaultGameEventParam prm)
		{
			if (this.m_LevelContext != null && this.m_LevelContext.IsMobaMode() && prm.src && ActorHelper.IsHostCtrlActor(ref prm.src))
			{
				Singleton<CSoundManager>.get_instance().PostEvent("Reset_Dead_Low_Pass", null);
				this.SendHostPlayerDieOrReLive(2);
			}
		}

		public override void Init()
		{
			int layer = LayerMask.NameToLayer("Ignore Raycast");
			int layer2 = LayerMask.NameToLayer("Actor");
			Physics.IgnoreLayerCollision(layer, layer2);
			this.battleStat = Singleton<BattleStatistic>.GetInstance();
			Singleton<SkillDetectionControl>.GetInstance();
			Singleton<SkillSelectControl>.GetInstance();
			this.IsModifyingCamera = false;
		}

		public void SetupMap(MapWrapper map)
		{
			this.mapLogic = map;
			if (this.mapLogic)
			{
				this.battleTaskSys.Initial("Assets.Scripts.GameLogic", GameDataMgr.gameTaskDatabin, GameDataMgr.gameTaskGroupDatabin, this.mapLogic.ActionHelper);
			}
		}

		public void SetupSoldier(SoldierControl soldier)
		{
			this.soldierCtrl = soldier;
		}

		public void UpdateLogic(int delta)
		{
			if (!this.isRuning)
			{
				return;
			}
			this.m_fAveFPS = GameFramework.m_fFps + this.m_fAveFPS;
			this.m_fpsCount += 1L;
			if (GameFramework.m_fFps <= 10f)
			{
				this.m_fpsCunt10 += 1L;
			}
			else if (GameFramework.m_fFps <= 18f && GameFramework.m_fFps > 10f)
			{
				this.m_fpsCunt18 += 1L;
			}
			if (Time.time - this.m_FpsTimeBegin > 5f)
			{
				this.m_FpsTimeBegin = Time.time;
				if (this.m_LastFps == 0f)
				{
					this.m_LastFps = GameFramework.m_fFps;
				}
				if (Mathf.Abs(this.m_LastFps - GameFramework.m_fFps) > 10f)
				{
					this.m_Ab_FPS_time = Singleton<FrameSynchr>.get_instance().LogicFrameTick * 0.001f;
					this.m_Abnormal_FPS_Count += 1f;
					this.m_LastFps = GameFramework.m_fFps;
				}
				else if (Mathf.Abs(this.m_LastFps - GameFramework.m_fFps) > 4f)
				{
					this.m_Ab_4FPS_time = Singleton<FrameSynchr>.get_instance().LogicFrameTick * 0.001f;
					this.m_Abnormal_4FPS_Count += 1f;
					this.m_LastFps = GameFramework.m_fFps;
				}
			}
			if (!FogOfWar.enable)
			{
				this.horizon.UpdateLogic(delta);
			}
			if (this.mapLogic)
			{
				this.mapLogic.UpdateLogic(delta);
			}
			if (this.soldierCtrl)
			{
				this.soldierCtrl.UpdateLogic(delta);
			}
			this.UpdateDragonSpawnUI(delta);
			this.hostPlayerLogic.UpdateLogic(delta);
			if (this.dynamicProperty != null)
			{
				this.dynamicProperty.UpdateLogic(delta);
			}
			if (this.m_globalTrigger != null)
			{
				this.m_globalTrigger.UpdateLogic(delta);
			}
			Singleton<CBattleSystem>.GetInstance().UpdateLogic(delta);
			Singleton<BattleStatistic>.get_instance().UpdateLogic(delta);
			this.DynamicCheckQualitySetting(delta);
			if (FogOfWar.enable)
			{
				FogOfWar.UpdateMain();
			}
		}

		public void Update()
		{
			if (FogOfWar.enable && Singleton<BattleLogic>.get_instance().isFighting)
			{
				FogOfWar.UpdateTextures();
			}
		}

		public void InitDynamicQualityCheck()
		{
			this.m_needAutoCheckQUality = true;
			this.m_isUserConfirmedQualityDegrade = false;
			this.m_totalLowFPSTime = 0;
			this.m_lowFPSTimeDeadline = 20000;
			this.m_qualitySetting = GameSettings.ModelLOD;
			this.m_qualitySettingParticle = GameSettings.ParticleLOD;
			int @int = PlayerPrefs.GetInt("autoCheckQualityCoolDown", 0);
			if (@int > 0)
			{
				this.m_needAutoCheckQUality = false;
				PlayerPrefs.SetInt("autoCheckQualityCoolDown", @int - 1);
				PlayerPrefs.Save();
			}
		}

		private bool IsAreadyLowestQuality()
		{
			return GameSettings.ModelLOD == 2 && GameSettings.ParticleLOD == 2 && !GameSettings.EnableOutline;
		}

		private void ApplyDynamicQualityCheck()
		{
			GameSettings.ModelLOD = this.m_qualitySetting;
			if (GameSettings.DynamicParticleLOD)
			{
				GameSettings.ParticleLOD = this.m_qualitySettingParticle;
			}
		}

		private void DynamicCheckQualitySetting(int delta)
		{
			if (MonoSingleton<Reconnection>.GetInstance().isProcessingRelayRecover)
			{
				return;
			}
			if (Singleton<FrameSynchr>.GetInstance().tryCount > 1)
			{
				return;
			}
			if (!this.isFighting || !this.m_needAutoCheckQUality)
			{
				return;
			}
			if (this.IsAreadyLowestQuality())
			{
				return;
			}
			if (GameFramework.m_fFps < 15f)
			{
				this.m_totalLowFPSTime += delta;
			}
			if (this.m_totalLowFPSTime > this.m_lowFPSTimeDeadline)
			{
				this.LevelDownQuality();
				this.m_lowFPSTimeDeadline = Mathf.Max(11000, this.m_lowFPSTimeDeadline / 2);
			}
		}

		public void LevelDownShadowQuality()
		{
			GameSettings.EnableOutline = false;
			if (GameSettings.ShadowQuality == SGameRenderQuality.High)
			{
				GameSettings.ShadowQuality = SGameRenderQuality.Medium;
			}
		}

		private void LevelDownQuality()
		{
			if (!this.m_isUserConfirmedQualityDegrade)
			{
				string text = Singleton<CTextManager>.GetInstance().GetText("TIPS_DEGRADE_QUALITY");
				stUIEventParams par = default(stUIEventParams);
				Singleton<CUIManager>.GetInstance().OpenSmallMessageBox(text, true, enUIEventID.Degrade_Quality_Accept, enUIEventID.Degrade_Quality_Cancel, par, 10, enUIEventID.Degrade_Quality_Accept, string.Empty, string.Empty, false);
				this.m_needAutoCheckQUality = false;
				return;
			}
			if (GameSettings.ParticleLOD < 2)
			{
				string text2 = Singleton<CTextManager>.GetInstance().GetText("TIPS_AUTO_DEGRADE_QUALITY");
				Singleton<CUIManager>.GetInstance().OpenTips(text2, false, 1.5f, null, new object[0]);
			}
			this._LevelDownQuality();
		}

		private void _LevelDownQuality()
		{
			GameSettings.EnableOutline = false;
			if (GameSettings.DynamicParticleLOD)
			{
				GameSettings.ParticleLOD++;
				if (this.m_qualitySetting < GameSettings.ParticleLOD)
				{
					this.m_qualitySetting = GameSettings.ParticleLOD;
				}
			}
			else
			{
				this.m_qualitySettingParticle++;
				if (this.m_qualitySetting < this.m_qualitySettingParticle)
				{
					this.m_qualitySetting = this.m_qualitySettingParticle;
				}
			}
			this.LevelDownShadowQuality();
			this.m_totalLowFPSTime = 0;
			PlayerPrefs.SetInt("degrade", 1);
			PlayerPrefs.Save();
			this.m_needAutoCheckQUality = false;
		}

		public void LevelDownQualityAccept(CUIEvent uiEvent)
		{
			this.m_iAutoLODState = 1;
			this.m_isUserConfirmedQualityDegrade = true;
			this.m_needAutoCheckQUality = true;
			this._LevelDownQuality();
		}

		public void LevelDownQualityCancel(CUIEvent uiEvent)
		{
			this.m_iAutoLODState = 2;
			this.m_needAutoCheckQUality = false;
			PlayerPrefs.SetInt("autoCheckQualityCoolDown", 5);
			PlayerPrefs.Save();
		}

		public bool PrepareFight()
		{
			bool isCameraFlip = this.m_LevelContext.m_isCameraFlip;
			this.m_LevelContext.m_isCameraFlip = false;
			Player hostPlayer = Singleton<GamePlayerCenter>.get_instance().GetHostPlayer();
			if (this.m_LevelContext != null && this.m_LevelContext.IsMobaModeWithOutGuide() && hostPlayer != null && hostPlayer.PlayerCamp == 2)
			{
				this.m_LevelContext.m_isCameraFlip = (!Singleton<WatchController>.GetInstance().IsWatching && isCameraFlip);
			}
			Singleton<CChatController>.get_instance().model.channelMgr.Clear(EChatChannel.Select_Hero, 0uL, 0u);
			this.battleStat.m_playerKDAStat.reset();
			Singleton<CBattleSystem>.GetInstance().OpenForm((!Singleton<WatchController>.GetInstance().IsWatching) ? CBattleSystem.FormType.Fight : CBattleSystem.FormType.Watch);
			if (this.m_LevelContext != null && this.m_LevelContext.m_isShowTrainingHelper)
			{
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Training_HelperInit);
			}
			if (this.valAdjustCtrl == null)
			{
				this.valAdjustCtrl = new CBattleValueAdjust();
			}
			this.valAdjustCtrl.Init();
			Singleton<GameObjMgr>.GetInstance().PrepareFight();
			MonoSingleton<CameraSystem>.GetInstance().PrepareFight();
			this.IsModifyingCamera = false;
			GameObject gameObject = GameObject.Find("Design");
			if (gameObject)
			{
				GlobalTrigger component = gameObject.GetComponent<GlobalTrigger>();
				if (component)
				{
					component.PrepareFight();
					this.m_globalTrigger = component;
				}
			}
			bool result = MonoSingleton<DialogueProcessor>.GetInstance().PrepareFight();
			DefaultGameEventParam defaultGameEventParam = new DefaultGameEventParam(Singleton<GamePlayerCenter>.get_instance().GetHostPlayer().Captain, Singleton<GamePlayerCenter>.get_instance().GetHostPlayer().Captain);
			Singleton<GameEventSys>.get_instance().SendEvent<DefaultGameEventParam>(GameEventDef.Event_FightPrepare, ref defaultGameEventParam);
			return result;
		}

		public void StartFightMultiGame()
		{
			this.isWaitMultiStart = false;
			Singleton<FrameSynchr>.get_instance().ResetSynchrSeed();
			Singleton<FrameSynchr>.get_instance().SetSynchrRunning(true);
			Singleton<GameReplayModule>.GetInstance().BattleStart();
			Singleton<BattleLogic>.GetInstance().DoBattleStart();
		}

		private void DoBattleStartEvent()
		{
			Player hostPlayer = Singleton<GamePlayerCenter>.get_instance().GetHostPlayer();
			DebugHelper.Assert(hostPlayer != null, "Fatal Error when DoBattleStartEvent, HostPlayer is null!");
			DefaultGameEventParam defaultGameEventParam = new DefaultGameEventParam(hostPlayer.Captain, hostPlayer.Captain);
			Singleton<GameEventSys>.get_instance().SendEvent<DefaultGameEventParam>(GameEventDef.Event_FightStart, ref defaultGameEventParam);
		}

		private void DoBattleStartFightStart()
		{
			Debug.Log("Enter DoBattleStartFightStart");
			GameSettings.FightStart();
			DebugHelper.Assert(this.attackOrder != null, "attackOrder is null");
			this.attackOrder.FightStart();
			DebugHelper.Assert(this.dynamicProperty != null, "dynamicProperty is null");
			this.dynamicProperty.FightStart();
			DebugHelper.Assert(this.clashAddition != null, "clashAddition is null");
			this.clashAddition.FightStart();
			DebugHelper.Assert(this.dynamicDifficulty != null, "dynamicDifficulty is null");
			this.dynamicDifficulty.FightStart();
			DebugHelper.Assert(this.horizon != null, "horizon is null");
			this.horizon.FightStart();
			SLevelContext curLvelContext = this.GetCurLvelContext();
			DebugHelper.Assert(curLvelContext != null, "slc is null");
			if (curLvelContext != null)
			{
				DebugHelper.Assert(this.incomeCtrl != null, "incomeCtrl is null");
				this.incomeCtrl.Init(curLvelContext);
				DebugHelper.Assert(curLvelContext.m_battleTaskOfCamps != null, "slc.battleTaskOfCamps is null");
				DebugHelper.Assert(this.battleTaskSys != null, "battleTaskSys is null");
				for (int i = 0; i < curLvelContext.m_battleTaskOfCamps.Length; i++)
				{
					if (curLvelContext.m_battleTaskOfCamps[i] > 0u)
					{
						this.battleTaskSys.AddTask(curLvelContext.m_battleTaskOfCamps[i], true);
					}
				}
				if (Singleton<CBattleSystem>.GetInstance().FightForm != null)
				{
					Singleton<CBattleSystem>.get_instance().FightForm.ShowTaskView(this.battleTaskSys.HasTask);
				}
				if (curLvelContext.IsGameTypeArena() && Singleton<CBattleSystem>.GetInstance().FightForm != null)
				{
					Singleton<CBattleSystem>.GetInstance().FightForm.ShowArenaTimer();
				}
			}
			if (Singleton<CBattleSystem>.GetInstance().FightForm != null && Singleton<CBattleSystem>.get_instance().FightForm.GetBattleMisc() != null)
			{
				Singleton<CBattleSystem>.get_instance().FightForm.GetBattleMisc().RebindBoss();
				Singleton<CBattleSystem>.get_instance().FightForm.GetBattleMisc().BindHP();
			}
			DebugHelper.Assert(this.hostPlayerLogic != null, "hostPlayerLogic is null");
			this.hostPlayerLogic.FightStart();
		}

		public void DoBattleStart()
		{
			this.m_fAveFPS = 0f;
			this.m_fpsCount = 0L;
			this.m_fpsCunt10 = 0L;
			this.m_fpsCunt18 = 0L;
			this.m_iAutoLODState = 0;
			this.m_FpsTimeBegin = Time.time;
			this.m_Ab_FPS_time = 0f;
			this.m_Ab_4FPS_time = 0f;
			this.m_Abnormal_FPS_Count = 0f;
			this.m_Abnormal_4FPS_Count = 0f;
			this.m_LastFps = 0f;
			DebugHelper.Assert(!this.isFighting, "isFighting == false");
			if (this.isFighting)
			{
				return;
			}
			this.isFighting = true;
			this.isWaitGameEnd = false;
			this.m_cachedSvrEndData = null;
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Degrade_Quality_Accept, new CUIEventManager.OnUIEventHandler(this.LevelDownQualityAccept));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Degrade_Quality_Cancel, new CUIEventManager.OnUIEventHandler(this.LevelDownQualityCancel));
			if (this.battleStat != null)
			{
				this.battleStat.StartStatistic();
			}
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Battle_ActivateForm);
			this.RegistBattleEvent();
			Singleton<StarSystem>.GetInstance().StartFight();
			Singleton<WinLoseByStarSys>.GetInstance().StartFight();
			if (this.mapLogic)
			{
				this.mapLogic.Startup();
			}
			if (this.soldierCtrl)
			{
				this.soldierCtrl.Startup();
			}
			if (this.incomeCtrl == null)
			{
				this.incomeCtrl = new IncomeControl();
			}
			this.incomeCtrl.StartFight();
			Singleton<GameObjMgr>.GetInstance().StartFight();
			Singleton<GameStateCtrl>.GetInstance().GotoState("BattleState");
			Singleton<GameInput>.GetInstance().ChangeBattleMode(false);
			this.DoBattleStartEvent();
			this.DoBattleStartFightStart();
			this.SpawnMapBuffs();
			this.SendSecureStartInfoReq();
			Singleton<CSurrenderSystem>.get_instance().Reset();
			this.InitDynamicQualityCheck();
			if (FogOfWar.enable)
			{
				Singleton<GameFowManager>.get_instance().OnStartFight();
			}
			Singleton<CBattleSystem>.GetInstance().BattleStart();
			if (this.GetCurLvelContext().IsMultilModeWithWarmBattle())
			{
				MonoSingleton<VoiceSys>.get_instance().StartSyncVoiceStateTimer(4000);
			}
			Singleton<CResourceManager>.GetInstance().UnloadUnusedAssets();
		}

		public bool FilterEnemyActor(ref PoolObjHandle<ActorRoot> actor)
		{
			return ActorHelper.IsHostEnemyActor(ref actor);
		}

		public bool FilterTeamActor(ref PoolObjHandle<ActorRoot> actor)
		{
			return ActorHelper.IsHostCampActor(ref actor);
		}

		public string GetLevelTypeDescription()
		{
			if (this.m_LevelContext == null)
			{
				return string.Empty;
			}
			if (!this.m_LevelContext.IsMobaMode())
			{
				return "PVE";
			}
			return this.m_LevelContext.m_pvpPlayerNum / 2 + "V" + this.m_LevelContext.m_pvpPlayerNum / 2;
		}

		private void SendSecureStartInfoReq()
		{
			Singleton<NetworkModule>.get_instance().RecvGameMsgCount = 0u;
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1062u);
			CSPKG_SECURE_INFO_START_REQ stSecureInfoStartReq = cSPkg.stPkgData.get_stSecureInfoStartReq();
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 1);
			TimeSpan timeSpan = DateTime.get_Now() - dateTime;
			cSPkg.stPkgData.get_stSecureInfoStartReq().dwClientStartTime = (uint)timeSpan.get_TotalSeconds();
			List<PoolObjHandle<ActorRoot>> list = ActorHelper.FilterActors(Singleton<GameObjMgr>.get_instance().HeroActors, new ActorFilterDelegate(this.FilterEnemyActor));
			stSecureInfoStartReq.iSvrBossCount = list.get_Count();
			for (int i = 0; i < list.get_Count(); i++)
			{
				stSecureInfoStartReq.iSvrBossHPMax = ((stSecureInfoStartReq.iSvrBossHPMax <= list.get_Item(i).get_handle().ValueComponent.actorHp) ? list.get_Item(i).get_handle().ValueComponent.actorHp : stSecureInfoStartReq.iSvrBossHPMax);
				if (stSecureInfoStartReq.iSvrBossHPMin == 0)
				{
					stSecureInfoStartReq.iSvrBossHPMin = list.get_Item(i).get_handle().ValueComponent.actorHp;
				}
				else
				{
					stSecureInfoStartReq.iSvrBossHPMin = ((stSecureInfoStartReq.iSvrBossHPMin >= list.get_Item(i).get_handle().ValueComponent.actorHp) ? list.get_Item(i).get_handle().ValueComponent.actorHp : stSecureInfoStartReq.iSvrBossHPMin);
				}
				stSecureInfoStartReq.iSvrBossHPTotal += list.get_Item(i).get_handle().ValueComponent.actorHp;
				int totalValue = list.get_Item(i).get_handle().ValueComponent.mActorValue[1].totalValue;
				stSecureInfoStartReq.iSvrBossAttackMax = ((stSecureInfoStartReq.iSvrBossAttackMax <= totalValue) ? totalValue : stSecureInfoStartReq.iSvrBossAttackMax);
				if (stSecureInfoStartReq.iSvrBossAttackMin == 0)
				{
					stSecureInfoStartReq.iSvrBossAttackMin = totalValue;
				}
				else
				{
					stSecureInfoStartReq.iSvrBossAttackMin = ((stSecureInfoStartReq.iSvrBossAttackMin >= totalValue) ? totalValue : stSecureInfoStartReq.iSvrBossAttackMin);
				}
				if (list.get_Count() > 0)
				{
					stSecureInfoStartReq.iSvrEmenyCardID1 = list.get_Item(0).get_handle().TheActorMeta.ConfigId;
					stSecureInfoStartReq.iSvrEmenyCardHP1 = list.get_Item(0).get_handle().ValueComponent.actorHp;
					int num = list.get_Item(0).get_handle().ValueComponent.mActorValue[1].totalValue;
					stSecureInfoStartReq.iSvrEmenyCardATN1 = num;
					num = list.get_Item(0).get_handle().ValueComponent.mActorValue[2].totalValue;
					stSecureInfoStartReq.iSvrEmenyCardINT1 = num;
					num = list.get_Item(0).get_handle().ValueComponent.actorMoveSpeed;
					stSecureInfoStartReq.iSvrEmenyCardSpeed1 = num;
					num = list.get_Item(0).get_handle().ValueComponent.mActorValue[3].totalValue;
					stSecureInfoStartReq.iSvrEmenyCardPhyDef1 = num;
					num = list.get_Item(0).get_handle().ValueComponent.mActorValue[4].totalValue;
					stSecureInfoStartReq.iSvrEmenyCardSpellDef1 = num;
				}
				if (list.get_Count() > 1)
				{
					stSecureInfoStartReq.iSvrEmenyCardID2 = list.get_Item(1).get_handle().TheActorMeta.ConfigId;
					stSecureInfoStartReq.iSvrEmenyCardHP2 = list.get_Item(1).get_handle().ValueComponent.actorHp;
					int num2 = list.get_Item(1).get_handle().ValueComponent.mActorValue[1].totalValue;
					stSecureInfoStartReq.iSvrEmenyCardATN2 = num2;
					num2 = list.get_Item(1).get_handle().ValueComponent.mActorValue[2].totalValue;
					stSecureInfoStartReq.iSvrEmenyCardINT2 = num2;
					num2 = list.get_Item(1).get_handle().ValueComponent.actorMoveSpeed;
					stSecureInfoStartReq.iSvrEmenyCardSpeed2 = num2;
					num2 = list.get_Item(1).get_handle().ValueComponent.mActorValue[3].totalValue;
					stSecureInfoStartReq.iSvrEmenyCardPhyDef2 = num2;
					num2 = list.get_Item(1).get_handle().ValueComponent.mActorValue[4].totalValue;
					stSecureInfoStartReq.iSvrEmenyCardSpellDef2 = num2;
				}
				if (list.get_Count() > 2)
				{
					stSecureInfoStartReq.iSvrEmenyCardID3 = list.get_Item(2).get_handle().TheActorMeta.ConfigId;
					stSecureInfoStartReq.iSvrEmenyCardHP3 = list.get_Item(2).get_handle().ValueComponent.actorHp;
					int num3 = list.get_Item(2).get_handle().ValueComponent.mActorValue[1].totalValue;
					stSecureInfoStartReq.iSvrEmenyCardATN3 = num3;
					num3 = list.get_Item(2).get_handle().ValueComponent.mActorValue[2].totalValue;
					stSecureInfoStartReq.iSvrEmenyCardINT3 = num3;
					num3 = list.get_Item(2).get_handle().ValueComponent.actorMoveSpeed;
					stSecureInfoStartReq.iSvrEmenyCardSpeed3 = num3;
					num3 = list.get_Item(2).get_handle().ValueComponent.mActorValue[3].totalValue;
					stSecureInfoStartReq.iSvrEmenyCardPhyDef3 = num3;
					num3 = list.get_Item(2).get_handle().ValueComponent.mActorValue[4].totalValue;
					stSecureInfoStartReq.iSvrEmenyCardSpellDef3 = num3;
				}
			}
			List<PoolObjHandle<ActorRoot>> organActors = Singleton<GameObjMgr>.get_instance().OrganActors;
			for (int j = 0; j < organActors.get_Count(); j++)
			{
				PoolObjHandle<ActorRoot> poolObjHandle = organActors.get_Item(j);
				int actorHp = poolObjHandle.get_handle().ValueComponent.actorHp;
				int totalValue2 = poolObjHandle.get_handle().ValueComponent.mActorValue[1].totalValue;
				if (ActorHelper.IsHostEnemyActor(ref poolObjHandle))
				{
					stSecureInfoStartReq.iSvrEmenyBuildingHPMax = ((stSecureInfoStartReq.iSvrEmenyBuildingHPMax <= actorHp) ? actorHp : stSecureInfoStartReq.iSvrEmenyBuildingHPMax);
					if (stSecureInfoStartReq.iSvrEmenyBuildingHPMin == 0)
					{
						stSecureInfoStartReq.iSvrEmenyBuildingHPMin = actorHp;
					}
					else
					{
						stSecureInfoStartReq.iSvrEmenyBuildingHPMin = ((stSecureInfoStartReq.iSvrEmenyBuildingHPMin >= actorHp) ? actorHp : stSecureInfoStartReq.iSvrEmenyBuildingHPMin);
					}
					stSecureInfoStartReq.iSvrEmenyHPTotal += actorHp;
					stSecureInfoStartReq.iSvrEmenyBuildingAttackMax = ((stSecureInfoStartReq.iSvrEmenyBuildingAttackMax <= totalValue2) ? totalValue2 : stSecureInfoStartReq.iSvrEmenyBuildingAttackMax);
					if (stSecureInfoStartReq.iSvrEmenyBuildingAttackMin == 0)
					{
						stSecureInfoStartReq.iSvrEmenyBuildingAttackMin = totalValue2;
					}
					else
					{
						stSecureInfoStartReq.iSvrEmenyBuildingAttackMin = ((stSecureInfoStartReq.iSvrEmenyBuildingAttackMin >= totalValue2) ? totalValue2 : stSecureInfoStartReq.iSvrEmenyBuildingAttackMin);
					}
				}
				else
				{
					stSecureInfoStartReq.iSvrBuildingHPMax = ((stSecureInfoStartReq.iSvrBuildingHPMax <= actorHp) ? actorHp : stSecureInfoStartReq.iSvrBuildingHPMax);
					if (stSecureInfoStartReq.iSvrBuildingHPMin == 0)
					{
						stSecureInfoStartReq.iSvrBuildingHPMin = actorHp;
					}
					else
					{
						stSecureInfoStartReq.iSvrBuildingHPMin = ((stSecureInfoStartReq.iSvrBuildingHPMin >= actorHp) ? actorHp : stSecureInfoStartReq.iSvrBuildingHPMin);
					}
					stSecureInfoStartReq.iSvrBuildingAttackMax = ((stSecureInfoStartReq.iSvrBuildingAttackMax <= totalValue2) ? totalValue2 : stSecureInfoStartReq.iSvrBuildingAttackMax);
					if (stSecureInfoStartReq.iSvrBuildingAttackMin == 0)
					{
						stSecureInfoStartReq.iSvrBuildingAttackMin = totalValue2;
					}
					else
					{
						stSecureInfoStartReq.iSvrBuildingAttackMin = ((stSecureInfoStartReq.iSvrBuildingAttackMin >= totalValue2) ? totalValue2 : stSecureInfoStartReq.iSvrBuildingAttackMin);
					}
				}
			}
			List<PoolObjHandle<ActorRoot>> list2 = ActorHelper.FilterActors(Singleton<GameObjMgr>.get_instance().SoldierActors, new ActorFilterDelegate(this.FilterEnemyActor));
			for (int k = 0; k < list2.get_Count(); k++)
			{
				PoolObjHandle<ActorRoot> poolObjHandle2 = list2.get_Item(k);
				int actorHp2 = poolObjHandle2.get_handle().ValueComponent.actorHp;
				int totalValue3 = poolObjHandle2.get_handle().ValueComponent.mActorValue[1].totalValue;
				stSecureInfoStartReq.iSvrEmenyHPMax = ((stSecureInfoStartReq.iSvrEmenyHPMax <= actorHp2) ? actorHp2 : stSecureInfoStartReq.iSvrEmenyHPMax);
				if (stSecureInfoStartReq.iSvrEmenyHPMin == 0)
				{
					stSecureInfoStartReq.iSvrEmenyHPMin = actorHp2;
				}
				else
				{
					stSecureInfoStartReq.iSvrEmenyHPMin = ((stSecureInfoStartReq.iSvrEmenyHPMin >= actorHp2) ? actorHp2 : stSecureInfoStartReq.iSvrEmenyHPMin);
				}
				stSecureInfoStartReq.iSvrEmenyAttackMax = ((stSecureInfoStartReq.iSvrEmenyAttackMax <= totalValue3) ? totalValue3 : stSecureInfoStartReq.iSvrEmenyAttackMax);
				if (stSecureInfoStartReq.iSvrEmenyAttackMin == 0)
				{
					stSecureInfoStartReq.iSvrEmenyAttackMin = totalValue3;
				}
				else
				{
					stSecureInfoStartReq.iSvrEmenyAttackMin = ((stSecureInfoStartReq.iSvrEmenyAttackMin >= totalValue3) ? totalValue3 : stSecureInfoStartReq.iSvrEmenyAttackMin);
				}
			}
			List<PoolObjHandle<ActorRoot>> list3 = ActorHelper.FilterActors(Singleton<GameObjMgr>.get_instance().HeroActors, new ActorFilterDelegate(this.FilterTeamActor));
			if (list3.get_Count() > 0)
			{
				stSecureInfoStartReq.iSvrUserCardID1 = list3.get_Item(0).get_handle().TheActorMeta.ConfigId;
				stSecureInfoStartReq.iSvrUserCardHP1 = list3.get_Item(0).get_handle().ValueComponent.actorHp;
				int num4 = list3.get_Item(0).get_handle().ValueComponent.mActorValue[1].totalValue;
				stSecureInfoStartReq.iSvrUserCardATN1 = num4;
				num4 = list3.get_Item(0).get_handle().ValueComponent.mActorValue[2].totalValue;
				stSecureInfoStartReq.iSvrUserCardINT1 = num4;
				num4 = list3.get_Item(0).get_handle().ValueComponent.actorMoveSpeed;
				stSecureInfoStartReq.iSvrUserCardSpeed1 = num4;
				num4 = list3.get_Item(0).get_handle().ValueComponent.mActorValue[3].totalValue;
				stSecureInfoStartReq.iSvrUserCardPhyDef1 = num4;
				num4 = list3.get_Item(0).get_handle().ValueComponent.mActorValue[4].totalValue;
				stSecureInfoStartReq.iSvrUserCardSpellDef1 = num4;
			}
			if (list3.get_Count() > 1)
			{
				stSecureInfoStartReq.iSvrUserCardID2 = list3.get_Item(1).get_handle().TheActorMeta.ConfigId;
				stSecureInfoStartReq.iSvrUserCardHP2 = list3.get_Item(1).get_handle().ValueComponent.actorHp;
				int num5 = list3.get_Item(1).get_handle().ValueComponent.mActorValue[1].totalValue;
				stSecureInfoStartReq.iSvrUserCardATN2 = num5;
				num5 = list3.get_Item(1).get_handle().ValueComponent.mActorValue[2].totalValue;
				stSecureInfoStartReq.iSvrUserCardINT2 = num5;
				num5 = list3.get_Item(1).get_handle().ValueComponent.actorMoveSpeed;
				stSecureInfoStartReq.iSvrUserCardSpeed2 = num5;
				num5 = list3.get_Item(1).get_handle().ValueComponent.mActorValue[3].totalValue;
				stSecureInfoStartReq.iSvrUserCardPhyDef2 = num5;
				num5 = list3.get_Item(1).get_handle().ValueComponent.mActorValue[4].totalValue;
				stSecureInfoStartReq.iSvrUserCardSpellDef2 = num5;
			}
			if (list3.get_Count() > 2)
			{
				stSecureInfoStartReq.iSvrUserCardID3 = list3.get_Item(2).get_handle().TheActorMeta.ConfigId;
				stSecureInfoStartReq.iSvrUserCardHP3 = list3.get_Item(2).get_handle().ValueComponent.actorHp;
				int num6 = list3.get_Item(2).get_handle().ValueComponent.mActorValue[1].totalValue;
				stSecureInfoStartReq.iSvrUserCardATN3 = num6;
				num6 = list3.get_Item(2).get_handle().ValueComponent.mActorValue[2].totalValue;
				stSecureInfoStartReq.iSvrUserCardINT3 = num6;
				num6 = list3.get_Item(2).get_handle().ValueComponent.actorMoveSpeed;
				stSecureInfoStartReq.iSvrUserCardSpeed3 = num6;
				num6 = list3.get_Item(2).get_handle().ValueComponent.mActorValue[3].totalValue;
				stSecureInfoStartReq.iSvrUserCardPhyDef3 = num6;
				num6 = list3.get_Item(2).get_handle().ValueComponent.mActorValue[4].totalValue;
				stSecureInfoStartReq.iSvrUserCardSpellDef3 = num6;
			}
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		private static void OnDelayForceKillCrystalTimerComplete(int timerSequence)
		{
			if (BattleLogic.m_DelayForceKillCrystalCamp >= 0)
			{
				BattleLogic.ForceKillCrystal(BattleLogic.m_DelayForceKillCrystalCamp);
			}
		}

		public static void DelayForceKillCrystal(uint delay, int Camp)
		{
			BattleLogic.m_DelayForceKillCrystalCamp = Camp;
			Singleton<CTimerManager>.get_instance().AddTimer((int)delay, 1, new CTimer.OnTimeUpHandler(BattleLogic.OnDelayForceKillCrystalTimerComplete), true);
		}

		public static void ForceKillCrystal(int Camp)
		{
			List<PoolObjHandle<ActorRoot>> gameActors = Singleton<GameObjMgr>.get_instance().GameActors;
			int count = gameActors.get_Count();
			for (int i = 0; i < count; i++)
			{
				PoolObjHandle<ActorRoot> poolObjHandle = gameActors.get_Item(i);
				if (poolObjHandle && poolObjHandle.get_handle().ActorControl != null && poolObjHandle.get_handle().TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ && poolObjHandle.get_handle().TheActorMeta.ActorCamp == Camp && poolObjHandle.get_handle().TheStaticData.TheOrganOnlyInfo.OrganType == 2)
				{
					poolObjHandle.get_handle().ValueComponent.actorHp = 0;
					break;
				}
			}
		}

		public void DealGameSurrender(byte bWinCamp)
		{
			if (this.isWaitGameEnd)
			{
				return;
			}
			COM_PLAYERCAMP playerCamp = Singleton<GamePlayerCenter>.get_instance().GetHostPlayer().PlayerCamp;
			this.battleStat.iBattleResult = ((playerCamp != bWinCamp) ? 2 : 1);
			COM_PLAYERCAMP camp;
			if (this.battleStat.iBattleResult == 1)
			{
				camp = BattleLogic.GetOppositeCmp(playerCamp);
			}
			else
			{
				camp = playerCamp;
			}
			uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey(182u).dwConfValue;
			KillNotify theKillNotify = Singleton<CBattleSystem>.GetInstance().TheKillNotify;
			if (theKillNotify != null)
			{
				theKillNotify.ClearKillNotifyList();
				if (this.battleStat.iBattleResult == 1)
				{
					theKillNotify.PlayAnimator("TouXiang_B");
				}
				else
				{
					theKillNotify.PlayAnimator("TouXiang_A");
				}
			}
			BattleLogic.DelayForceKillCrystal(dwConfValue, camp);
		}

		public void onFightOver(ref DefaultGameEventParam prm)
		{
			if (!this.isFighting)
			{
				DebugHelper.Assert(false, "wtf, 重复触发onFightOver");
				return;
			}
			Singleton<GameEventSys>.get_instance().SendEvent<DefaultGameEventParam>(GameEventDef.Event_BeginFightOver, ref prm);
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Degrade_Quality_Accept, new CUIEventManager.OnUIEventHandler(this.LevelDownQualityAccept));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Degrade_Quality_Cancel, new CUIEventManager.OnUIEventHandler(this.LevelDownQualityCancel));
			this.DoFightOver(true);
			if (Singleton<WatchController>.GetInstance().IsWatching)
			{
				Singleton<WatchController>.GetInstance().MarkOver();
			}
			else if (Singleton<LobbyLogic>.get_instance().inMultiGame)
			{
				Singleton<LobbyLogic>.GetInstance().StartSettleTimers();
				Singleton<LobbyLogic>.GetInstance().ReqMultiGameOver(false);
			}
			else
			{
				if (Singleton<CBattleSystem>.GetInstance().FightForm != null)
				{
					Singleton<WinLose>.get_instance().LastSingleGameWin = (Singleton<BattleLogic>.get_instance().JudgeBattleResult(prm.src, prm.orignalAtker) == 1);
					if (this.m_LevelContext.IsMobaMode())
					{
						SettleEventParam settleEventParam = default(SettleEventParam);
						settleEventParam.isWin = Singleton<WinLose>.get_instance().LastSingleGameWin;
						Singleton<GameEventSys>.GetInstance().SendEvent<SettleEventParam>(GameEventDef.Event_SettleComplete, ref settleEventParam);
					}
				}
				Singleton<LobbyLogic>.GetInstance().ReqSingleGameFinish(false, false);
			}
		}

		public void DoFightOver(bool bNormalEnd)
		{
			if (!this.isFighting)
			{
				DebugHelper.Assert(false, "wtf, 重复调用DoFightOver");
				return;
			}
			this.horizon.FightOver();
			Singleton<GameObjMgr>.GetInstance().FightOver();
			if (this.mapLogic != null)
			{
				this.mapLogic.Reset();
			}
			this.attackOrder.FightOver();
			this.dynamicProperty.FightOver();
			this.clashAddition.FightOver();
			this.battleTaskSys.Clear();
			if (Singleton<CBattleSystem>.GetInstance().FightForm != null)
			{
				Singleton<CBattleSystem>.GetInstance().FightForm.DisableUIEvent();
			}
			this.hostPlayerLogic.FightOver();
			Singleton<FrameSynchr>.get_instance().SwitchSynchrLocal();
			this.isFighting = false;
			this.isGameOver = true;
			this.isWaitGameEnd = bNormalEnd;
			this.m_cachedSvrEndData = null;
			BattleLogic.m_DelayForceKillCrystalCamp = -1;
		}

		public void onPreGameSettle()
		{
			if (this.mapLogic != null)
			{
				this.mapLogic.Reset();
			}
		}

		public void onGameEnd(ref DefaultGameEventParam prm)
		{
			MonoSingleton<VoiceSys>.get_instance().ClearVoiceStateData();
			Singleton<LobbyLogic>.GetInstance().StopGameEndTimer();
			if (!Singleton<WatchController>.GetInstance().IsWatching)
			{
				Singleton<LobbyLogic>.GetInstance().StartSettlePanelTimer();
				if (this.isWaitGameEnd && this.m_cachedSvrEndData != null)
				{
					CSPkg cachedSvrEndData = this.m_cachedSvrEndData;
					this.m_cachedSvrEndData = null;
					if (Singleton<LobbyLogic>.get_instance().inMultiGame)
					{
						if (cachedSvrEndData.stPkgData.get_stMultGameSettleGain().iErrCode == 0)
						{
							SLevelContext.SetMasterPvpDetailWhenGameSettle(cachedSvrEndData.stPkgData.get_stMultGameSettleGain().stDetail.stGameInfo);
						}
						LobbyMsgHandler.HandleGameSettle(cachedSvrEndData.stPkgData.get_stMultGameSettleGain().iErrCode == 0, true, cachedSvrEndData.stPkgData.get_stMultGameSettleGain().stDetail.stGameInfo.bGameResult, cachedSvrEndData.stPkgData.get_stMultGameSettleGain().stDetail.stHeroList, cachedSvrEndData.stPkgData.get_stMultGameSettleGain().stDetail.stRankInfo, cachedSvrEndData.stPkgData.get_stMultGameSettleGain().stDetail.stAcntInfo, cachedSvrEndData.stPkgData.get_stMultGameSettleGain().stDetail.stMultipleDetail, cachedSvrEndData.stPkgData.get_stMultGameSettleGain().stDetail.stSpecReward, cachedSvrEndData.stPkgData.get_stMultGameSettleGain().stDetail.stReward);
					}
					else
					{
						LobbyMsgHandler.HandleSingleGameSettle(cachedSvrEndData);
					}
				}
			}
			this.isWaitGameEnd = false;
			this.UnRegistBattleEvent();
		}

		public void ResetBattleSystem()
		{
			this.isWaitGameEnd = false;
			this.m_cachedSvrEndData = null;
			this.UnRegistBattleEvent();
			if (this.mapLogic != null)
			{
				this.mapLogic.Reset();
			}
			this.battleTaskSys.Clear();
			this.ApplyDynamicQualityCheck();
			if (Singleton<CBattleSystem>.GetInstance().FightForm != null)
			{
				Singleton<CBattleSystem>.GetInstance().FightForm.DisableUIEvent();
			}
		}

		public void SingleReqLoseGame()
		{
			Singleton<WinLose>.GetInstance().LastSingleGameWin = false;
			bool clickGameOver = true;
			SLevelContext curLvelContext = Singleton<BattleLogic>.get_instance().GetCurLvelContext();
			if (curLvelContext != null && curLvelContext.IsGameTypeGuide() && curLvelContext.IsMobaMode() && curLvelContext.m_mapID == 7)
			{
				Singleton<WinLose>.GetInstance().LastSingleGameWin = true;
				clickGameOver = false;
			}
			Singleton<LobbyLogic>.GetInstance().ReqSingleGameFinish(clickGameOver, false);
		}

		private void RegistBattleEvent()
		{
			Singleton<GameEventSys>.get_instance().RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightOver, new RefAction<DefaultGameEventParam>(this.onFightOver));
			Singleton<GameEventSys>.get_instance().RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_GameEnd, new RefAction<DefaultGameEventParam>(this.onGameEnd));
			Singleton<GameEventSys>.get_instance().RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
			Singleton<GameEventSys>.get_instance().RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorRevive, new RefAction<DefaultGameEventParam>(this.onActorRevive));
			Singleton<GameEventSys>.get_instance().RmvEventHandler<HurtEventResultInfo>(GameEventDef.Event_ActorDamage, new RefAction<HurtEventResultInfo>(this.OnActorDamage));
			Singleton<GameEventSys>.get_instance().RmvEventHandler<HemophagiaEventResultInfo>(GameEventDef.Event_Hemophagia, new RefAction<HemophagiaEventResultInfo>(this.OnActorHemophagia));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int>("HeroSoulLevelChange", new Action<PoolObjHandle<ActorRoot>, int>(this.OnHeroSoulLvlChange));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, byte, byte>("HeroSkillLevelUp", new Action<PoolObjHandle<ActorRoot>, byte, byte>(this.OnHeroSkillLvlup));
			Singleton<GameEventSys>.get_instance().AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightOver, new RefAction<DefaultGameEventParam>(this.onFightOver));
			Singleton<GameEventSys>.get_instance().AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_GameEnd, new RefAction<DefaultGameEventParam>(this.onGameEnd));
			Singleton<GameEventSys>.get_instance().AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
			Singleton<GameEventSys>.get_instance().AddEventHandler<HurtEventResultInfo>(GameEventDef.Event_ActorDamage, new RefAction<HurtEventResultInfo>(this.OnActorDamage));
			Singleton<GameEventSys>.get_instance().AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorRevive, new RefAction<DefaultGameEventParam>(this.onActorRevive));
			Singleton<GameEventSys>.get_instance().AddEventHandler<HemophagiaEventResultInfo>(GameEventDef.Event_Hemophagia, new RefAction<HemophagiaEventResultInfo>(this.OnActorHemophagia));
			Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int>("HeroSoulLevelChange", new Action<PoolObjHandle<ActorRoot>, int>(this.OnHeroSoulLvlChange));
			Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, byte, byte>("HeroSkillLevelUp", new Action<PoolObjHandle<ActorRoot>, byte, byte>(this.OnHeroSkillLvlup));
		}

		private void UnRegistBattleEvent()
		{
			Singleton<GameEventSys>.get_instance().RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightOver, new RefAction<DefaultGameEventParam>(this.onFightOver));
			Singleton<GameEventSys>.get_instance().RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_GameEnd, new RefAction<DefaultGameEventParam>(this.onGameEnd));
			Singleton<GameEventSys>.get_instance().RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
			Singleton<GameEventSys>.get_instance().RmvEventHandler<HurtEventResultInfo>(GameEventDef.Event_ActorDamage, new RefAction<HurtEventResultInfo>(this.OnActorDamage));
			Singleton<GameEventSys>.get_instance().RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightPrepareFin, new RefAction<DefaultGameEventParam>(this.OnFightPrepareFin));
			Singleton<GameEventSys>.get_instance().RmvEventHandler<HemophagiaEventResultInfo>(GameEventDef.Event_Hemophagia, new RefAction<HemophagiaEventResultInfo>(this.OnActorHemophagia));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int>("HeroSoulLevelChange", new Action<PoolObjHandle<ActorRoot>, int>(this.OnHeroSoulLvlChange));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, byte, byte>("HeroSkillLevelUp", new Action<PoolObjHandle<ActorRoot>, byte, byte>(this.OnHeroSkillLvlup));
			if (this.incomeCtrl != null)
			{
				this.incomeCtrl.uninit();
			}
			if (this.valAdjustCtrl != null)
			{
				this.valAdjustCtrl.UnInit();
			}
		}

		public void ShowWinLose(bool bWin)
		{
			if (Singleton<CBattleSystem>.GetInstance().FightForm != null && this.m_LevelContext.IsMobaMode())
			{
				Singleton<CBattleSystem>.GetInstance().FightForm.ShowWinLosePanel(bWin);
			}
		}

		private void OnActorHemophagia(ref HemophagiaEventResultInfo hri)
		{
			if (hri.src && (!hri.src.get_handle().Visible || !hri.src.get_handle().InCamera))
			{
				return;
			}
			if (ActorHelper.IsHostActor(ref hri.src))
			{
				CBattleSystem instance = Singleton<CBattleSystem>.GetInstance();
				if (hri.hpChanged != 0)
				{
					Vector3 position = hri.src.get_handle().myTransform.position;
					float num = Random.Range(-0.5f, 0.5f);
					position = new Vector3(position.x, position.y + num, position.z);
					instance.CollectFloatDigitInSingleFrame(hri.src, hri.src, DIGIT_TYPE.ReviveHp, hri.hpChanged);
				}
			}
		}

		private void OnActorDamage(ref HurtEventResultInfo hri)
		{
			if (Singleton<CBattleSystem>.GetInstance().TheMinimapSys != null)
			{
				Singleton<CBattleSystem>.GetInstance().TheMinimapSys.OnActorDamage(ref hri);
			}
			if (hri.src && (!hri.src.get_handle().Visible || !hri.src.get_handle().InCamera))
			{
				return;
			}
			DIGIT_TYPE dIGIT_TYPE = DIGIT_TYPE.Invalid;
			PoolObjHandle<ActorRoot> poolObjHandle;
			if (hri.src && hri.src.get_handle().ActorControl != null)
			{
				poolObjHandle = hri.src.get_handle().ActorControl.GetOrignalActor();
			}
			else
			{
				poolObjHandle = hri.src;
			}
			PoolObjHandle<ActorRoot> poolObjHandle2;
			if (hri.atker && hri.atker.get_handle().ActorControl != null)
			{
				poolObjHandle2 = hri.atker.get_handle().ActorControl.GetOrignalActor();
			}
			else
			{
				poolObjHandle2 = hri.atker;
			}
			if ((poolObjHandle && ActorHelper.IsHostActor(ref poolObjHandle)) || (poolObjHandle2 && ActorHelper.IsHostActor(ref poolObjHandle2)))
			{
				if (hri.hurtInfo.hurtType == HurtTypeDef.Therapic)
				{
					dIGIT_TYPE = DIGIT_TYPE.ReviveHp;
				}
				else if (hri.hurtInfo.hurtType == HurtTypeDef.MagicHurt)
				{
					dIGIT_TYPE = ((hri.critValue <= 0) ? DIGIT_TYPE.MagicAttackNormal : DIGIT_TYPE.MagicAttackCrit);
				}
				else if (hri.hurtInfo.hurtType == HurtTypeDef.PhysHurt)
				{
					dIGIT_TYPE = ((hri.critValue <= 0) ? DIGIT_TYPE.PhysicalAttackNormal : DIGIT_TYPE.PhysicalAttackCrit);
				}
				else
				{
					dIGIT_TYPE = ((hri.critValue <= 0) ? DIGIT_TYPE.RealAttackNormal : DIGIT_TYPE.RealAttackCrit);
				}
			}
			if (dIGIT_TYPE != DIGIT_TYPE.Invalid)
			{
				CBattleSystem instance = Singleton<CBattleSystem>.GetInstance();
				if (hri.hpChanged != 0)
				{
					int value = (dIGIT_TYPE != DIGIT_TYPE.ReviveHp) ? hri.hurtTotal : hri.hpChanged;
					instance.CollectFloatDigitInSingleFrame(hri.atker, hri.src, dIGIT_TYPE, value);
				}
			}
		}

		public SLevelContext GetCurLvelContext()
		{
			return this.m_LevelContext;
		}

		public int JudgeBattleResult(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker)
		{
			SLevelContext curLvelContext = this.GetCurLvelContext();
			Player hostPlayer = Singleton<GamePlayerCenter>.get_instance().GetHostPlayer();
			if (hostPlayer == null)
			{
				return 0;
			}
			int playerCamp = hostPlayer.PlayerCamp;
			if (curLvelContext != null && playerCamp >= 0 && playerCamp < curLvelContext.m_battleTaskOfCamps.Length)
			{
				GameTask task = this.battleTaskSys.GetTask(curLvelContext.m_battleTaskOfCamps[playerCamp], false);
				if (task != null)
				{
					return (!task.Achieving) ? 2 : 1;
				}
			}
			if (Singleton<WinLoseByStarSys>.get_instance().bStarted)
			{
				if (Singleton<WinLoseByStarSys>.get_instance().isSuccess)
				{
					return 1;
				}
				if (Singleton<WinLoseByStarSys>.get_instance().isFailure)
				{
					return 2;
				}
				if (src)
				{
					if (playerCamp == src.get_handle().TheActorMeta.ActorCamp)
					{
					}
					return (playerCamp == src.get_handle().TheActorMeta.ActorCamp) ? 2 : 1;
				}
				return 0;
			}
			else
			{
				if (src)
				{
					if (playerCamp == src.get_handle().TheActorMeta.ActorCamp)
					{
					}
					return (playerCamp == src.get_handle().TheActorMeta.ActorCamp) ? 2 : 1;
				}
				return 0;
			}
		}

		private void UpdateDragonSpawnUI(int delta)
		{
			if (Singleton<CBattleSystem>.GetInstance().FightForm != null)
			{
				Singleton<CBattleSystem>.GetInstance().FightForm.OnUpdateDragonUI(delta);
			}
		}

		public static COM_PLAYERCAMP GetOppositeCmp(COM_PLAYERCAMP InCamp)
		{
			if (InCamp == 1)
			{
				return 2;
			}
			return 1;
		}

		public static COM_PLAYERCAMP[] GetOthersCmp(COM_PLAYERCAMP InCamp)
		{
			int num = 3;
			COM_PLAYERCAMP[] array = new COM_PLAYERCAMP[num - 1];
			for (int i = 0; i < num; i++)
			{
				if (i != InCamp)
				{
					array[((i <= InCamp) ? num : 0) + (i - InCamp) - 1] = i;
				}
			}
			return array;
		}

		public static int MapOtherCampIndex(COM_PLAYERCAMP myCamp, COM_PLAYERCAMP otherCamp)
		{
			return ((otherCamp <= myCamp) ? 3 : 0) + (otherCamp - myCamp) - 1;
		}

		public static COM_PLAYERCAMP MapOtherCampType(COM_PLAYERCAMP myCamp, int index)
		{
			return (myCamp + index + 1) % 3;
		}

		public int GetDragonBuffId(RES_SKILL_SRC_TYPE type)
		{
			int result = 0;
			if (this.s_dragonBuffIds == null)
			{
				this.s_dragonBuffIds = new Dictionary<uint, int>();
				GameDataMgr.skillCombineDatabin.Accept(new Action<ResSkillCombineCfgInfo>(this.OnVisit));
			}
			this.s_dragonBuffIds.TryGetValue(type, ref result);
			return result;
		}

		private void OnVisit(ResSkillCombineCfgInfo InCfg)
		{
			byte b = 1;
			byte b2 = 5;
			if (InCfg.bSrcType >= b && InCfg.bSrcType < b2)
			{
				if (this.s_dragonBuffIds.ContainsKey((uint)InCfg.bSrcType))
				{
					this.s_dragonBuffIds.set_Item((uint)InCfg.bSrcType, InCfg.iCfgID);
				}
				else
				{
					this.s_dragonBuffIds.Add((uint)InCfg.bSrcType, InCfg.iCfgID);
				}
			}
		}

		private void SpawnMapBuffs()
		{
			SLevelContext curLvelContext = this.GetCurLvelContext();
			if (curLvelContext == null || curLvelContext.m_mapBuffs == null)
			{
				return;
			}
			for (int i = 0; i < curLvelContext.m_mapBuffs.Length; i++)
			{
				ResDT_MapBuff resDT_MapBuff = curLvelContext.m_mapBuffs[i];
				if (resDT_MapBuff.dwID == 0u)
				{
					break;
				}
				List<PoolObjHandle<ActorRoot>> heroActors = Singleton<GameObjMgr>.GetInstance().HeroActors;
				for (int j = 0; j < heroActors.get_Count(); j++)
				{
					PoolObjHandle<ActorRoot> inTargetActor = heroActors.get_Item(j);
					if ((1 << inTargetActor.get_handle().TheActorMeta.ActorCamp & (int)resDT_MapBuff.bCamp) > 0 && (resDT_MapBuff.bHeroType == 0 || inTargetActor.get_handle().TheStaticData.TheHeroOnlyInfo.HeroCapability == (int)resDT_MapBuff.bHeroType) && (resDT_MapBuff.bHeroDamageType == 0 || inTargetActor.get_handle().TheStaticData.TheHeroOnlyInfo.HeroDamageType == (int)resDT_MapBuff.bHeroDamageType) && (resDT_MapBuff.bHeroAttackType == 0 || inTargetActor.get_handle().TheStaticData.TheHeroOnlyInfo.HeroAttackType == (int)resDT_MapBuff.bHeroAttackType))
					{
						SkillUseParam skillUseParam = default(SkillUseParam);
						inTargetActor.get_handle().SkillControl.SpawnBuff(inTargetActor, ref skillUseParam, (int)resDT_MapBuff.dwID, false);
					}
				}
			}
		}

		public void SendHostPlayerDieOrReLive(CS_MULTGAME_DIE_TYPE type)
		{
			if (!Singleton<WatchController>.GetInstance().IsWatching)
			{
				CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1098u);
				cSPkg.stPkgData.get_stMultGameDieReq().bType = type;
				Singleton<NetworkModule>.GetInstance().SendGameMsg(ref cSPkg, 0u);
			}
		}

		public static HashSet<uint> GetAssistSet(PoolObjHandle<ActorRoot> victim, PoolObjHandle<ActorRoot> attker, bool excludeAttker)
		{
			HashSet<uint> hashSet = new HashSet<uint>();
			uint num = (!attker) ? 0u : attker.get_handle().ObjID;
			if (victim)
			{
				List<KeyValuePair<uint, ulong>> hurtSelfActorList = victim.get_handle().ActorControl.hurtSelfActorList;
				for (int i = hurtSelfActorList.get_Count() - 1; i >= 0; i--)
				{
					uint key = hurtSelfActorList.get_Item(i).get_Key();
					if (!excludeAttker || num != key)
					{
						hashSet.Add(key);
					}
				}
			}
			if (attker)
			{
				List<KeyValuePair<uint, ulong>> helpSelfActorList = attker.get_handle().ActorControl.helpSelfActorList;
				for (int j = helpSelfActorList.get_Count() - 1; j >= 0; j--)
				{
					uint key2 = helpSelfActorList.get_Item(j).get_Key();
					if (!excludeAttker || num != key2)
					{
						hashSet.Add(key2);
					}
				}
			}
			return hashSet;
		}

		public int CalcCurrentTime()
		{
			int num = (int)Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
			SLevelContext curLvelContext = this.GetCurLvelContext();
			if (curLvelContext != null && curLvelContext.m_isShowTrainingHelper && this.dynamicProperty != null)
			{
				num = (int)this.dynamicProperty.m_frameTimer;
			}
			return (int)((float)num * 0.001f);
		}

		public void AutoLearnSkill(PoolObjHandle<ActorRoot> hero)
		{
			if (!hero)
			{
				return;
			}
			if (hero.get_handle().ActorAgent.IsAutoAI())
			{
				for (int i = 3; i >= 1; i--)
				{
					if (this.IsMatchLearnSkillRule(hero, (SkillSlotType)i))
					{
						FrameCommand<LearnSkillCommand> frameCommand = FrameCommandFactory.CreateFrameCommand<LearnSkillCommand>();
						frameCommand.cmdData.dwHeroID = hero.get_handle().ObjID;
						frameCommand.cmdData.bSlotType = (byte)i;
						byte bSkillLevel = 0;
						if (hero.get_handle().SkillControl != null && hero.get_handle().SkillControl.SkillSlotArray[i] != null)
						{
							bSkillLevel = (byte)hero.get_handle().SkillControl.SkillSlotArray[i].GetSkillLevel();
						}
						frameCommand.cmdData.bSkillLevel = bSkillLevel;
						hero.get_handle().ActorControl.CmdCommonLearnSkill(frameCommand);
					}
				}
			}
		}

		public bool IsMatchLearnSkillRule(PoolObjHandle<ActorRoot> hero, SkillSlotType slotType)
		{
			bool result = false;
			if (!hero || slotType < SkillSlotType.SLOT_SKILL_1 || slotType > SkillSlotType.SLOT_SKILL_3)
			{
				return result;
			}
			if (hero.get_handle().SkillControl != null && hero.get_handle().SkillControl.m_iSkillPoint > 0 && hero.get_handle().SkillControl.SkillSlotArray[(int)slotType] != null)
			{
				int allSkillLevel = hero.get_handle().SkillControl.GetAllSkillLevel();
				if (hero.get_handle().ValueComponent != null && allSkillLevel >= hero.get_handle().ValueComponent.actorSoulLevel)
				{
					return false;
				}
				int skillLevel = hero.get_handle().SkillControl.SkillSlotArray[(int)slotType].GetSkillLevel();
				int num = skillLevel + 1;
				int actorSoulLevel = hero.get_handle().ValueComponent.actorSoulLevel;
				if (skillLevel < 6)
				{
					if (slotType == SkillSlotType.SLOT_SKILL_3 && skillLevel < 3)
					{
						if (num * 4 - 1 < actorSoulLevel)
						{
							result = true;
						}
					}
					else if (slotType >= SkillSlotType.SLOT_SKILL_1 && slotType < SkillSlotType.SLOT_SKILL_3 && num * 2 - 1 <= actorSoulLevel)
					{
						result = true;
					}
				}
			}
			else if (hero.get_handle().SkillControl != null && hero.get_handle().SkillControl.m_iSkillPoint > 0 && hero.get_handle().SkillControl.SkillSlotArray[(int)slotType] == null)
			{
				if (slotType == SkillSlotType.SLOT_SKILL_3)
				{
					if (hero.get_handle().ValueComponent.actorSoulLevel >= 4)
					{
						result = true;
					}
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		private void TryAutoLearSkill(PoolObjHandle<ActorRoot> hero)
		{
			if (hero && hero.get_handle().SkillControl.m_iSkillPoint > 0 && hero.get_handle().ActorAgent != null && hero.get_handle().ActorAgent.IsAutoAI())
			{
				Singleton<BattleLogic>.GetInstance().AutoLearnSkill(hero);
			}
		}

		private void OnHeroSoulLvlChange(PoolObjHandle<ActorRoot> hero, int level)
		{
			this.TryAutoLearSkill(hero);
		}

		private void OnHeroSkillLvlup(PoolObjHandle<ActorRoot> hero, byte bSlotType, byte bSkillLevel)
		{
			this.TryAutoLearSkill(hero);
		}
	}
}
