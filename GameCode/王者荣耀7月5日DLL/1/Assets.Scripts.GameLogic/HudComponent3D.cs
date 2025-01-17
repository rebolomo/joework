using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using Assets.Scripts.Sound;
using Assets.Scripts.UI;
using ResData;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameLogic
{
	public class HudComponent3D : LogicComponent
	{
		public enum enDirtyFlag
		{
			PositionInMap = 1,
			ForwardInMap,
			ForcePositionInMap = 4,
			Immediate = 8
		}

		private const int OVERLAY_RENDER_QUEUE = 4000;

		private const int DEPTH = 30;

		public const string HUD_HERO_PREFAB = "UI3D/Battle/Blood_Bar_Hero.prefab";

		public const string HUD_BLOOD_PREFAB = "UI3D/Battle/BloodHud.prefab";

		public const string HUD_BLOOD_EYE_PREFAB = "UI3D/Battle/BloodHudEye.prefab";

		public const string HUD_BLOOD_BLACK_BAR_PREFAB = "UI3D/Battle/BlackBarBlack.prefab";

		public const string HUD_HONOR_PREFAB = "UI3D/Battle/Img_Badge_{0}_{1}.prefab";

		private const float DecSpeed = 0.0086f;

		private const int BASE_ATTACK_INTERVAL = 20000;

		private const int HeroProficiencyDelta = 3;

		private const int HeroProficiencyNum = 3;

		public int hudHeight;

		public HudCompType HudType;

		private int m_hudFrameStampOnCreated;

		private GameObject m_hud;

		private Sprite3D m_bloodImage;

		private Sprite3D m_VoiceIconImage;

		private Sprite3D m_timerImg;

		private List<GameObject> m_blackBars = new List<GameObject>();

		private GameObject _shieldGo;

		private float _shieldImagWidth;

		private int _lastMaxBarValue;

		private Sprite3D _bloodDecImage;

		private int _curShield1;

		private bool _isDecreasingHp;

		private Sprite3D m_soulImage;

		private Sprite3D m_energyImage;

		private TextMesh m_soulLevel;

		private ListView<CoutofControlInfo> m_outofControlList;

		private Sprite3D m_outOfControlBar;

		private GameObject m_outOfControlGo;

		private GameObject m_skillTimerObj;

		private Sprite3D m_skillTimerBar;

		private SkillTimerInfo m_skillTimeInfo;

		private GameObject m_reviveTimerObj;

		private Sprite3D m_reviveTimerBar;

		private GameObject m_bloodBackObj;

		private GameObject m_bloodForeObj;

		private GameObject m_energyForeObj;

		private int m_reviveTotalTime;

		private int hudHeightOffset;

		private bool bReviveTimerShow;

		private Vector3 m_actorPos;

		private Vector3 m_bloodPos;

		private bool m_bloodDirty;

		private VInt3 m_actorForward;

		private bool m_bHeroSameCamp;

		private GameObject m_heroIconBG_small;

		private GameObject m_heroIconBG_big;

		private GameObject m_mapPointer_small;

		private Transform m_mapPointer_small_Trans;

		private Transform m_heroHead_small_Trans;

		private GameObject m_mapPointer_big;

		private Transform m_mapPointer_big_Trans;

		private Transform m_heroHead_big_Trans;

		private Sprite3D bigTower_spt3d;

		private Sprite3D smallTower_spt3d;

		private UI3DEventCom m_evtCom;

		private Image m_signalImage;

		private GameObject m_effectRoot_small;

		private CUIContainerScript m_textHudContainer;

		private GameObject m_textHudNode;

		private Text textCom;

		private int txt_hud_offset_x;

		private int txt_hud_offset_y;

		private RectTransform rtTransform;

		private CUIContainerScript m_inOutEquipShopHudContainer;

		private GameObject m_inOutEquipShopHud;

		public GameObject m_exclamationObj;

		public float m_exclamationObjOffsetY;

		private ulong LastBaseAttackTime;

		public bool bBossHpBar;

		private Animation[] HeroProficiencyAni_ = new Animation[3];

		private GameObject[] HeroProficiencyObj_ = new GameObject[3];

		private static readonly string[] HeroProficiencyShowNames_ = new string[]
		{
			"Hero_Icon3_Show",
			"Hero_Icon4_Show",
			"Hero_Icon5_Show"
		};

		private static readonly string[] HeroProficiencyIconNames_ = new string[]
		{
			"Hero_Icon3_Ani",
			"Hero_Icon4_Ani",
			"Hero_Icon5_Ani"
		};

		private int CurAniIndex_ = -1;

		public GameObject m_honor;

		public Animation m_honorAni;

		private bool m_hudLogicVisible;

		private bool m_textLogicVisible;

		private bool m_pointerLogicVisible;

		private static string[] _statusResPath = new string[]
		{
			"Prefab_Skill_Effects/tongyong_effects/UI_fx/Yinshen_tongyong_01"
		};

		public static float OFFSET_HEIGHT = 400f;

		private GameObject _mountPoint;

		private GameObject[] _statusGo;

		private int m_dirtyFlags;

		public static int soilderFrameCount = 4;

		public static int heroFrameCount = 2;

		public static int forceUpdateFrameCount = 2;

		public GameObject MapPointerSmall
		{
			get
			{
				return this.m_mapPointer_small;
			}
		}

		public GameObject MapPointerBig
		{
			get
			{
				return this.m_mapPointer_big;
			}
		}

		public override void OnUse()
		{
			base.OnUse();
			this.hudHeight = 0;
			this.HudType = HudCompType.Type_Hero;
			this.m_hudFrameStampOnCreated = 0;
			this.m_hud = null;
			this.m_bloodImage = null;
			this.m_timerImg = null;
			this.m_blackBars.Clear();
			this.m_soulImage = null;
			this.m_energyImage = null;
			this.m_soulLevel = null;
			this.m_outOfControlBar = null;
			this.m_outofControlList = null;
			this.m_outOfControlGo = null;
			this.m_skillTimerObj = null;
			this.m_skillTimerBar = null;
			this.m_skillTimeInfo = null;
			this.m_reviveTimerObj = null;
			this.m_reviveTimerBar = null;
			this.m_bloodBackObj = null;
			this.m_bloodForeObj = null;
			this.m_energyForeObj = null;
			this.m_reviveTotalTime = 0;
			this.hudHeightOffset = 0;
			this.bReviveTimerShow = false;
			MiniMapSysUT.RecycleMapGameObject(this.m_mapPointer_small);
			this.m_mapPointer_small = null;
			this.m_mapPointer_small_Trans = null;
			MiniMapSysUT.RecycleMapGameObject(this.m_heroHead_small_Trans);
			this.m_heroHead_small_Trans = null;
			MiniMapSysUT.RecycleMapGameObject(this.m_mapPointer_big);
			this.m_mapPointer_big = null;
			this.m_mapPointer_big_Trans = null;
			MiniMapSysUT.RecycleMapGameObject(this.m_heroHead_big_Trans);
			this.m_heroHead_big_Trans = null;
			MiniMapSysUT.UnRegisterEventCom(this.m_evtCom);
			this.m_evtCom = null;
			this.bigTower_spt3d = null;
			this.smallTower_spt3d = null;
			this.m_signalImage = null;
			this.m_actorPos = Vector3.zero;
			this.m_bloodPos = Vector3.zero;
			this.m_bloodDirty = false;
			this.m_actorForward = VInt3.zero;
			this.m_bHeroSameCamp = false;
			this.m_heroIconBG_small = null;
			this.m_heroIconBG_big = null;
			this.m_textHudContainer = null;
			this.m_textHudNode = null;
			this.textCom = null;
			this.txt_hud_offset_x = 0;
			this.txt_hud_offset_y = 0;
			this.rtTransform = null;
			this.m_exclamationObj = null;
			this.m_exclamationObjOffsetY = 0f;
			this.LastBaseAttackTime = 0uL;
			this.m_inOutEquipShopHudContainer = null;
			this.m_inOutEquipShopHud = null;
			this.bBossHpBar = false;
			this.m_hudLogicVisible = false;
			this.m_textLogicVisible = false;
			this.m_pointerLogicVisible = false;
			this._statusGo = null;
			this._mountPoint = null;
			this._shieldGo = null;
			this._bloodDecImage = null;
			this._lastMaxBarValue = 0;
			this._curShield1 = 0;
			this._shieldImagWidth = 0f;
			this._isDecreasingHp = false;
			this.m_effectRoot_small = null;
			this.m_VoiceIconImage = null;
			this.ClearHeroProficiency();
			this.m_honor = null;
			this.m_honorAni = null;
			this.m_dirtyFlags = 0;
		}

		private void ClearHeroProficiency()
		{
			Array.Clear(this.HeroProficiencyAni_, 0, this.HeroProficiencyAni_.Length);
			Array.Clear(this.HeroProficiencyObj_, 0, this.HeroProficiencyObj_.Length);
			this.CurAniIndex_ = -1;
		}

		public override void Born(ActorRoot owner)
		{
			base.Born(owner);
			if (this.actor.CharInfo != null)
			{
				this.hudHeight = this.actor.CharInfo.hudHeight;
				this.HudType = this.actor.CharInfo.HudType;
			}
			this.m_hudFrameStampOnCreated = 0;
			this.m_hud = null;
			this.m_bloodImage = null;
			this.m_timerImg = null;
			this.m_blackBars.Clear();
			this.m_soulImage = null;
			this.m_energyImage = null;
			this.m_soulLevel = null;
			this.m_outOfControlBar = null;
			this.m_outofControlList = null;
			this.m_outOfControlGo = null;
			this.m_skillTimerObj = null;
			this.m_skillTimerBar = null;
			this.m_skillTimeInfo = null;
			this.m_reviveTimerObj = null;
			this.m_reviveTimerBar = null;
			this.m_bloodBackObj = null;
			this.m_bloodForeObj = null;
			this.m_energyForeObj = null;
			this.m_reviveTotalTime = 0;
			this.hudHeightOffset = 0;
			this.bReviveTimerShow = false;
			MiniMapSysUT.RecycleMapGameObject(this.m_mapPointer_small);
			this.m_mapPointer_small = null;
			this.m_mapPointer_small_Trans = null;
			MiniMapSysUT.RecycleMapGameObject(this.m_heroHead_small_Trans);
			this.m_heroHead_small_Trans = null;
			MiniMapSysUT.RecycleMapGameObject(this.m_mapPointer_big);
			this.m_mapPointer_big = null;
			this.m_mapPointer_big_Trans = null;
			MiniMapSysUT.RecycleMapGameObject(this.m_heroHead_big_Trans);
			this.m_heroHead_big_Trans = null;
			MiniMapSysUT.UnRegisterEventCom(this.m_evtCom);
			this.m_evtCom = null;
			this.bigTower_spt3d = null;
			this.smallTower_spt3d = null;
			this.m_signalImage = null;
			this.m_actorPos = Vector3.zero;
			this.m_bloodPos = Vector3.zero;
			this.m_actorForward = VInt3.zero;
			this.m_bHeroSameCamp = false;
			this.m_heroIconBG_small = null;
			this.m_heroIconBG_big = null;
			this.m_textHudContainer = null;
			this.m_textHudNode = null;
			this.textCom = null;
			this.txt_hud_offset_x = 0;
			this.txt_hud_offset_y = 0;
			this.rtTransform = null;
			this.m_inOutEquipShopHudContainer = null;
			this.m_inOutEquipShopHud = null;
			this.m_effectRoot_small = null;
			this.m_dirtyFlags = 0;
		}

		public override void Init()
		{
			base.Init();
		}

		public override void Uninit()
		{
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int, int, int>("HeroSoulExpChange", new Action<PoolObjHandle<ActorRoot>, int, int, int>(this.onSoulExpChange));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int, int>("HeroEnergyChange", new Action<PoolObjHandle<ActorRoot>, int, int>(this.onEnergyExpChange));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int>("HeroSoulLevelChange", new Action<PoolObjHandle<ActorRoot>, int>(this.onSoulLvlChange));
			Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<LimitMoveEventParam>(GameSkillEventDef.AllEvent_LimitMove, new GameSkillEvent<LimitMoveEventParam>(this.OnPlayerLimitMove));
			Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<LimitMoveEventParam>(GameSkillEventDef.AllEvent_CancelLimitMove, new GameSkillEvent<LimitMoveEventParam>(this.OnPlayerCancelLimitMove));
			Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<SkillTimerEventParam>(GameSkillEventDef.AllEvent_SetSkillTimer, new GameSkillEvent<SkillTimerEventParam>(this.OnPlayerSkillTimer));
			Singleton<GameEventSys>.get_instance().RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorBeAttack, new RefAction<DefaultGameEventParam>(this.OnBaseAttacked));
			for (int i = 0; i < this.m_blackBars.get_Count(); i++)
			{
				Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.m_blackBars.get_Item(i));
			}
			this.m_blackBars.Clear();
			if (this.m_honor != null)
			{
				Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.m_honor);
			}
			this.m_honor = null;
			this.m_honorAni = null;
			if (this.m_hud != null)
			{
				Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.m_hud);
			}
			this.m_hud = null;
			this.m_bloodImage = null;
			this.m_timerImg = null;
			this._shieldGo = null;
			this.m_soulImage = null;
			this.m_energyImage = null;
			this.m_soulLevel = null;
			this.m_outOfControlBar = null;
			this.m_outofControlList = null;
			this.m_outOfControlGo = null;
			this.m_skillTimerObj = null;
			this.m_skillTimerBar = null;
			this.m_skillTimeInfo = null;
			this.m_reviveTimerObj = null;
			this.m_reviveTimerBar = null;
			this.m_bloodBackObj = null;
			this.m_bloodForeObj = null;
			this.m_energyForeObj = null;
			this.m_reviveTotalTime = 0;
			this.hudHeightOffset = 0;
			this.bReviveTimerShow = false;
			MiniMapSysUT.RecycleMapGameObject(this.m_mapPointer_small);
			this.m_mapPointer_small = null;
			this.m_mapPointer_small_Trans = null;
			MiniMapSysUT.RecycleMapGameObject(this.m_heroHead_small_Trans);
			this.m_heroHead_small_Trans = null;
			MiniMapSysUT.RecycleMapGameObject(this.m_mapPointer_big);
			this.m_mapPointer_big = null;
			this.m_mapPointer_big_Trans = null;
			MiniMapSysUT.RecycleMapGameObject(this.m_heroHead_big_Trans);
			this.m_heroHead_big_Trans = null;
			MiniMapSysUT.UnRegisterEventCom(this.m_evtCom);
			this.m_evtCom = null;
			this.bigTower_spt3d = null;
			this.smallTower_spt3d = null;
			if (this.m_textHudContainer != null && this.m_textHudNode != null)
			{
				this.m_textHudContainer.RecycleElement(this.m_textHudNode);
			}
			this.m_textHudContainer = null;
			this.m_textHudNode = null;
			this.textCom = null;
			this.txt_hud_offset_x = 0;
			this.txt_hud_offset_y = 0;
			this.rtTransform = null;
			if (this.m_inOutEquipShopHudContainer != null && this.m_inOutEquipShopHud != null)
			{
				this.m_inOutEquipShopHudContainer.RecycleElement(this.m_inOutEquipShopHud);
			}
			this.m_inOutEquipShopHudContainer = null;
			this.m_inOutEquipShopHud = null;
			this._statusGo = null;
			if (this._mountPoint)
			{
				Object.Destroy(this._mountPoint);
				this._mountPoint = null;
			}
			this._shieldGo = null;
			this._bloodDecImage = null;
			this._curShield1 = 0;
			this._shieldImagWidth = 0f;
			this._isDecreasingHp = false;
			this._lastMaxBarValue = 0;
			this.m_effectRoot_small = null;
			this.m_actorForward = VInt3.zero;
			this.m_bHeroSameCamp = false;
			this.m_heroIconBG_small = null;
			this.m_heroIconBG_big = null;
			this.m_exclamationObj = null;
			this.m_exclamationObjOffsetY = 0f;
			this.ClearHeroProficiency();
			this.m_dirtyFlags = 0;
		}

		public override void Prepare()
		{
			if (!MonoSingleton<GameFramework>.get_instance().EditorPreviewMode)
			{
				this.InitHudUI();
				this.InitStatus();
			}
		}

		private static void PreallocMapPointer(List<int> seqs, GameObject go, int num)
		{
			if (!go)
			{
				return;
			}
			seqs.Clear();
			CUIContainerScript component = go.GetComponent<CUIContainerScript>();
			for (int i = 0; i < num; i++)
			{
				int element = component.GetElement();
				if (element != -1)
				{
					seqs.Add(element);
				}
			}
			for (int j = 0; j < seqs.get_Count(); j++)
			{
				component.RecycleElement(seqs.get_Item(j));
			}
		}

		public static void PreallocMapPointer(int aliesNum, int enemyNum)
		{
			MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
			if (theMinimapSys == null)
			{
				return;
			}
			List<int> seqs = new List<int>(enemyNum);
			HudComponent3D.PreallocMapPointer(seqs, theMinimapSys.mmpcAlies, aliesNum);
			HudComponent3D.PreallocMapPointer(seqs, theMinimapSys.mmpcEnemy, enemyNum);
		}

		public override void Fight()
		{
			if (MonoSingleton<GameFramework>.get_instance().EditorPreviewMode || GameObjMgr.isPreSpawnActors)
			{
				return;
			}
			if (this.HudType != HudCompType.Type_Hide && this.m_hud)
			{
				this.m_hud.CustomSetActive(true);
			}
			this.ResetHudUI();
			DragonIcon.Check_Dragon_Born_Evt(this.actor, true);
		}

		public override void Deactive()
		{
			if (this.HudType != HudCompType.Type_Hide && this.m_hud)
			{
				this.m_hud.CustomSetActive(false);
			}
			this.setHudLogicVisible(false);
			this.setPointerVisible(false, true);
			MiniMapSysUT.RecycleMapGameObject(this.m_mapPointer_small);
			this.m_mapPointer_small = null;
			this.m_mapPointer_small_Trans = null;
			MiniMapSysUT.RecycleMapGameObject(this.m_heroHead_small_Trans);
			this.m_heroHead_small_Trans = null;
			MiniMapSysUT.RecycleMapGameObject(this.m_mapPointer_big);
			this.m_mapPointer_big = null;
			this.m_mapPointer_big_Trans = null;
			MiniMapSysUT.RecycleMapGameObject(this.m_heroHead_big_Trans);
			this.m_heroHead_big_Trans = null;
			MiniMapSysUT.UnRegisterEventCom(this.m_evtCom);
			this.m_evtCom = null;
			base.Deactive();
		}

		public static void Preload(ref ActorPreloadTab preloadTab)
		{
			for (int i = 0; i < HudComponent3D._statusResPath.Length; i++)
			{
				preloadTab.AddParticle(HudComponent3D._statusResPath[i]);
			}
			preloadTab.AddParticle("UI3D/Battle/Blood_Bar_Hero.prefab");
			preloadTab.AddParticle("UI3D/Battle/BloodHud.prefab");
			preloadTab.AddParticle("UI3D/Battle/BloodHudEye.prefab");
		}

		private bool IsPlayerCopy()
		{
			return this.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster && this.HudType == HudCompType.Type_Hero;
		}

		private void FillHeroProficiencyAni(int index)
		{
			GameObject gameObject = this.m_hud.transform.Find(HudComponent3D.HeroProficiencyIconNames_[index]).gameObject;
			Animation component = gameObject.GetComponent<Animation>();
			gameObject.CustomSetActive(false);
			this.HeroProficiencyAni_[index] = component;
			this.HeroProficiencyObj_[index] = gameObject;
		}

		private void InitHudUI()
		{
			this.m_hudFrameStampOnCreated = CUIManager.s_uiSystemRenderFrameCounter;
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			if (this.HudType != HudCompType.Type_Hide && this.m_hud == null)
			{
				bool flag = (this.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero || this.HudType == HudCompType.Type_Hero) && Singleton<BattleLogic>.GetInstance().m_LevelContext.IsSoulGrow();
				string text = (!flag) ? "UI3D/Battle/BloodHud.prefab" : "UI3D/Battle/Blood_Bar_Hero.prefab";
				if (this.HudType == HudCompType.Type_Eye)
				{
					text = "UI3D/Battle/BloodHudEye.prefab";
				}
				bool flag2 = ActorHelper.IsHostCtrlActor(ref this.actorPtr);
				this.m_hud = Singleton<CGameObjectPool>.GetInstance().GetGameObject(text, 0);
				DebugHelper.Assert(this.m_hud != null, "wtf?");
				if (this.m_hud == null)
				{
					return;
				}
				this.m_timerImg = null;
				if (flag)
				{
					Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int, int, int>("HeroSoulExpChange", new Action<PoolObjHandle<ActorRoot>, int, int, int>(this.onSoulExpChange));
					Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int, int>("HeroEnergyChange", new Action<PoolObjHandle<ActorRoot>, int, int>(this.onEnergyExpChange));
					Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int>("HeroSoulLevelChange", new Action<PoolObjHandle<ActorRoot>, int>(this.onSoulLvlChange));
					Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<LimitMoveEventParam>(GameSkillEventDef.AllEvent_LimitMove, new GameSkillEvent<LimitMoveEventParam>(this.OnPlayerLimitMove));
					Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<LimitMoveEventParam>(GameSkillEventDef.AllEvent_CancelLimitMove, new GameSkillEvent<LimitMoveEventParam>(this.OnPlayerCancelLimitMove));
					Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<SkillTimerEventParam>(GameSkillEventDef.AllEvent_SetSkillTimer, new GameSkillEvent<SkillTimerEventParam>(this.OnPlayerSkillTimer));
					this.m_bloodImage = this.m_hud.transform.Find("BloodFore").GetComponent<Sprite3D>();
					DebugHelper.Assert(this.m_bloodImage != null);
					if (this.m_bloodImage != null)
					{
						if (flag2)
						{
							this.m_bloodImage.spriteName = "Battle_greenHp";
						}
						else if (this.actor.IsHostCamp())
						{
							this.m_bloodImage.spriteName = ((!Singleton<WatchController>.GetInstance().IsWatching) ? "Battle_blueHp" : "Battle_greenHp");
						}
						else
						{
							this.m_bloodImage.spriteName = "Battle_redHp";
						}
						this._shieldGo = this.m_bloodImage.gameObject.transform.FindChild("Shield").gameObject;
						this._shieldImagWidth = this.m_bloodImage.width;
						this._shieldGo.CustomSetActive(false);
						this._bloodDecImage = this.m_hud.transform.FindChild("BloodBack").gameObject.GetComponent<Sprite3D>();
						this._curShield1 = 0;
						if (this.actor.ValueComponent.actorHpTotal > 0)
						{
							this.m_bloodImage.fillAmount = (float)this.actor.ValueComponent.actorHp / (float)this.actor.ValueComponent.actorHpTotal;
						}
						else
						{
							this.m_bloodImage.fillAmount = 0f;
						}
						int actorHpTotal = this.actor.ValueComponent.actorHpTotal;
						if (this.IsPlayerCopy())
						{
							MonsterWrapper monsterWrapper = this.actor.ActorControl as MonsterWrapper;
							if (monsterWrapper != null && monsterWrapper.hostActor)
							{
								actorHpTotal = monsterWrapper.hostActor.get_handle().ValueComponent.actorHpTotal;
							}
						}
						this.SetBlackBar(actorHpTotal);
						if (this._bloodDecImage != null)
						{
							this._bloodDecImage.fillAmount = this.m_bloodImage.fillAmount;
						}
					}
					this.m_soulImage = this.m_hud.transform.Find("SoulFore").GetComponent<Sprite3D>();
					DebugHelper.Assert(this.m_soulImage != null);
					if (this.m_soulImage != null)
					{
						this.m_soulImage.fillAmount = 0f;
					}
					this.m_soulLevel = this.m_hud.transform.Find("SoulLevel").GetComponent<TextMesh>();
					DebugHelper.Assert(this.m_soulLevel != null);
					if (this.m_soulLevel != null)
					{
						this.m_soulLevel.text = "0";
						this.m_soulLevel.gameObject.GetComponent<MeshRenderer>().sharedMaterial.renderQueue = 4500;
					}
					GameObject gameObject = Utility.FindChild(this.m_hud, "EnergyFore");
					if (gameObject != null)
					{
						this.m_energyImage = gameObject.GetComponent<Sprite3D>();
					}
					DebugHelper.Assert(this.m_energyImage != null);
					if (this.m_energyImage != null)
					{
						if (!this.actor.ValueComponent.IsEnergyType(EnergyType.NoneResource))
						{
							this.m_energyImage.spriteName = EnergyCommon.GetSpriteName((int)this.actor.ValueComponent.mEnergy.energyType);
							if (this.actor.ValueComponent.actorEpTotal > 0)
							{
								this.m_energyImage.fillAmount = (float)this.actor.ValueComponent.actorEp / (float)this.actor.ValueComponent.actorEpTotal;
							}
							else
							{
								this.m_energyImage.fillAmount = 0f;
							}
						}
						else
						{
							gameObject.CustomSetActive(false);
						}
					}
					this.m_outOfControlGo = this.m_hud.transform.Find("OutOfControl").gameObject;
					this.m_outOfControlBar = this.m_outOfControlGo.transform.Find("OutOfControlFore").GetComponent<Sprite3D>();
					DebugHelper.Assert(this.m_outOfControlBar != null);
					if (this.m_outOfControlBar != null)
					{
						this.m_outOfControlGo.CustomSetActive(false);
						this.m_outOfControlBar.fillAmount = 0f;
					}
					this.m_outofControlList = new ListView<CoutofControlInfo>();
					this.m_skillTimerObj = Utility.FindChild(this.m_hud, "SkillTimer");
					if (this.m_skillTimerObj != null)
					{
						this.m_skillTimerObj.CustomSetActive(false);
						this.m_skillTimerBar = Utility.GetComponetInChild<Sprite3D>(this.m_skillTimerObj, "SkillTimerBar");
						if (this.m_skillTimerBar != null)
						{
							this.m_skillTimerBar.fillAmount = 0f;
						}
					}
					this.m_skillTimeInfo = new SkillTimerInfo(0, 0, 0uL);
					this.m_reviveTimerObj = Utility.FindChild(this.m_hud, "ReviveTimer");
					if (this.m_reviveTimerObj != null)
					{
						this.m_reviveTimerObj.CustomSetActive(false);
						this.m_reviveTimerBar = this.m_reviveTimerObj.GetComponent<Sprite3D>();
						if (this.m_reviveTimerBar != null)
						{
							this.m_reviveTimerBar.fillAmount = 0f;
						}
					}
					this.m_bloodBackObj = Utility.FindChild(this.m_hud, "BloodBack");
					this.m_bloodForeObj = Utility.FindChild(this.m_hud, "BloodFore");
					this.m_energyForeObj = Utility.FindChild(this.m_hud, "EnergyFore");
					this.FillHeroProficiencyAni(0);
					this.FillHeroProficiencyAni(1);
					this.FillHeroProficiencyAni(2);
					bool flag3 = curLvelContext != null && curLvelContext.IsMobaMode();
					TextMesh component = this.m_hud.transform.Find("PlayerName").GetComponent<TextMesh>();
					Player ownerPlayer = ActorHelper.GetOwnerPlayer(ref this.actorPtr);
					if (this.IsPlayerCopy() && !this.actor.IsHostCamp())
					{
						MonsterWrapper monsterWrapper2 = this.actor.ActorControl as MonsterWrapper;
						if (monsterWrapper2 != null && monsterWrapper2.hostActor)
						{
							PoolObjHandle<ActorRoot> hostActor = monsterWrapper2.hostActor;
							ownerPlayer = ActorHelper.GetOwnerPlayer(ref hostActor);
						}
					}
					DebugHelper.Assert(component != null);
					if (component != null)
					{
						if (flag3 && ownerPlayer != null)
						{
							component.text = ownerPlayer.Name;
							component.gameObject.CustomSetActive(true);
						}
						else
						{
							component.gameObject.CustomSetActive(false);
						}
						component.gameObject.GetComponent<MeshRenderer>().sharedMaterial.renderQueue = 4500;
					}
					if (curLvelContext != null && curLvelContext.m_isShowHonor && ownerPlayer != null)
					{
						this.SetHonor(ownerPlayer.HonorId, ownerPlayer.HonorLevel);
					}
				}
				else if (this.HudType != HudCompType.Type_Eye)
				{
					ActorTypeDef actorTypeDef = this.actor.TheActorMeta.ActorType;
					if (this.HudType == HudCompType.Type_Boss)
					{
						actorTypeDef = ActorTypeDef.Actor_Type_Hero;
					}
					Sprite3D component2 = this.m_hud.transform.Find("BloodHud").GetComponent<Sprite3D>();
					component2.spriteName = Enum.GetName(typeof(SpriteNameEnum), (int)(ActorTypeDef.Actor_Type_EYE * actorTypeDef));
					component2.SetNativeSize(Singleton<Camera_UI3D>.GetInstance().GetCurrentCamera(), 30f);
					this.m_bloodImage = this.m_hud.transform.Find("Fore").GetComponent<Sprite3D>();
					if (flag2)
					{
						this.m_bloodImage.spriteName = "bl_hero_self";
					}
					else
					{
						this.m_bloodImage.spriteName = Enum.GetName(typeof(SpriteNameEnum), (int)(ActorTypeDef.Actor_Type_EYE * actorTypeDef + ((!this.actor.IsHostCamp()) ? 2 : 1)));
					}
					this.m_bloodImage.SetNativeSize(Singleton<Camera_UI3D>.GetInstance().GetCurrentCamera(), 30f);
					if (this.actor.ValueComponent.actorHpTotal > 0)
					{
						this.m_bloodImage.fillAmount = (float)this.actor.ValueComponent.actorHp / (float)this.actor.ValueComponent.actorHpTotal;
					}
					else
					{
						this.m_bloodImage.fillAmount = 0f;
					}
					bool flag4 = this.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ;
					Sprite3D componetInChild = Utility.GetComponetInChild<Sprite3D>(this.m_hud, "Icon");
					if (componetInChild != null)
					{
						if (flag4)
						{
							componetInChild.spriteName = ((!this.actor.IsHostCamp()) ? SpriteNameEnum.bl_icon_organ_enemy.ToString() : SpriteNameEnum.bl_icon_organ_self.ToString());
							componetInChild.SetNativeSize(Singleton<Camera_UI3D>.GetInstance().GetCurrentCamera(), 30f);
							componetInChild.transform.localPosition = new Vector3(this.m_bloodImage.transform.localPosition.x - 0.5f * this.m_bloodImage.width - 0.25f * componetInChild.width, componetInChild.transform.localPosition.y, componetInChild.transform.localPosition.z);
							componetInChild.gameObject.CustomSetActive(true);
						}
						else
						{
							componetInChild.gameObject.CustomSetActive(false);
						}
					}
				}
				else
				{
					ActorTypeDef actorType = this.actor.TheActorMeta.ActorType;
					Sprite3D component3 = this.m_hud.transform.Find("BloodHud").GetComponent<Sprite3D>();
					component3.spriteName = Enum.GetName(typeof(SpriteNameEnum), (int)(ActorTypeDef.Actor_Type_EYE * actorType));
					this.m_bloodImage = this.m_hud.transform.Find("Fore").GetComponent<Sprite3D>();
					this.m_bloodImage.spriteName = Enum.GetName(typeof(SpriteNameEnum), (int)(ActorTypeDef.Actor_Type_EYE * actorType + ((!this.actor.IsHostCamp()) ? 2 : 1)));
					if (this.actor.ValueComponent.actorHpTotal > 0)
					{
						this.m_bloodImage.fillAmount = (float)this.actor.ValueComponent.actorHp / (float)this.actor.ValueComponent.actorHpTotal;
					}
					else
					{
						this.m_bloodImage.fillAmount = 0f;
					}
					Sprite3D componetInChild2 = Utility.GetComponetInChild<Sprite3D>(this.m_hud, "Icon");
					if (componetInChild2 != null)
					{
						componetInChild2.gameObject.CustomSetActive(false);
					}
					this.m_timerImg = this.m_hud.transform.Find("Timer").GetComponent<Sprite3D>();
				}
				Transform transform = this.m_hud.transform.Find("VoiceIcon");
				if (transform)
				{
					this.m_VoiceIconImage = transform.GetComponent<Sprite3D>();
					this.m_VoiceIconImage.gameObject.CustomSetActive(false);
				}
				Transform hudPanel = this.GetHudPanel(this.HudType, this.actor.IsHostCamp());
				if (hudPanel != null)
				{
					this.m_hud.transform.SetParent(hudPanel, true);
					this.m_hud.transform.localScale = Vector3.one;
					this.m_hud.transform.localRotation = Quaternion.identity;
				}
				Singleton<Camera_UI3D>.GetInstance().GetCurrentCanvas().RefreshLayout(null);
				this.setHudLogicVisible(false);
				if (this.m_bloodDirty)
				{
					this.m_bloodDirty = false;
					bool bActive = this.m_hudLogicVisible && this.actor.Visible && this.actor.InCamera;
					if (this.m_hud != null)
					{
						this.m_hud.CustomSetActive(bActive);
					}
				}
			}
			if (this.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ && !this.actor.TheStaticData.TheOrganOnlyInfo.ShowInMinimap)
			{
				return;
			}
			if (!this.IsCallMonster() && this.actor.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_EYE)
			{
				this.HudInit_MapPointer();
			}
		}

		private void HudInit_MapPointer()
		{
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			if (this.m_mapPointer_small == null)
			{
				this.initHudIcon(curLvelContext, true, out this.m_mapPointer_small);
			}
			if (this.m_mapPointer_big == null)
			{
				this.initHudIcon(curLvelContext, false, out this.m_mapPointer_big);
			}
			MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
			if (theMinimapSys != null)
			{
				this.m_effectRoot_small = theMinimapSys.mmpcEffect;
			}
			if (curLvelContext != null)
			{
				this.setPointerVisible(curLvelContext.IsMobaMode(), true);
			}
		}

		private void initHudIcon(SLevelContext levelContext, bool bMiniMap, out GameObject mapPointer)
		{
			mapPointer = null;
			this.m_bHeroSameCamp = this.actor.IsHostCamp();
			MiniMapSysUT.UnRegisterEventCom(this.m_evtCom);
			this.m_evtCom = null;
			mapPointer = MiniMapSysUT.GetMapGameObject(this.actor, bMiniMap, out this.m_evtCom);
			if (mapPointer == null)
			{
				return;
			}
			mapPointer.CustomSetActive(false);
			if (bMiniMap)
			{
				this.m_mapPointer_small_Trans = mapPointer.transform;
			}
			else
			{
				this.m_mapPointer_big_Trans = mapPointer.transform;
			}
			Transform transform = mapPointer.transform;
			if (transform == null)
			{
				return;
			}
			if (this.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
			{
				GameObject gameObject = mapPointer.transform.Find("bg").gameObject;
				if (bMiniMap)
				{
					this.m_heroIconBG_small = gameObject;
				}
				else
				{
					this.m_heroIconBG_big = gameObject;
				}
				ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey((uint)this.actor.TheActorMeta.ConfigId);
				DebugHelper.Assert(dataByKey != null);
				string hero_Icon = KillNotifyUT.GetHero_Icon(this.actor, true);
				bool bSelf = this.actor.TheActorMeta.PlayerId == Singleton<GamePlayerCenter>.GetInstance().HostPlayerId;
				GameObject heroIconObj = MiniMapSysUT.GetHeroIconObj(hero_Icon, bMiniMap, bSelf, this.m_bHeroSameCamp);
				if (heroIconObj != null)
				{
					heroIconObj.CustomSetActive((!bMiniMap) ? this.m_mapPointer_big_Trans.gameObject.activeSelf : this.m_mapPointer_small_Trans.gameObject.activeSelf);
					if (bMiniMap)
					{
						this.m_heroHead_small_Trans = heroIconObj.transform;
					}
					else
					{
						this.m_heroHead_big_Trans = heroIconObj.transform;
					}
				}
				this.m_actorPos = this.actor.myTransform.position;
				this.UpdateUIMap(ref this.m_actorPos);
				BackCityCom_3DUI.SetVisible(mapPointer, false);
			}
			else if (this.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ)
			{
				Transform transform2 = mapPointer.transform.Find("icon");
				if (transform2 != null)
				{
					Sprite3D component = transform2.GetComponent<Sprite3D>();
					if (bMiniMap)
					{
						this.smallTower_spt3d = component;
					}
					else
					{
						this.bigTower_spt3d = component;
					}
				}
				if (this.actor.IsHostCamp())
				{
					if (this.actor.TheStaticData.TheOrganOnlyInfo.OrganType == 2 && !bMiniMap)
					{
						Singleton<GameEventSys>.get_instance().RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorBeAttack, new RefAction<DefaultGameEventParam>(this.OnBaseAttacked));
						Singleton<GameEventSys>.get_instance().AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorBeAttack, new RefAction<DefaultGameEventParam>(this.OnBaseAttacked));
					}
				}
				transform.SetAsFirstSibling();
				this.m_actorPos = this.actor.myTransform.position;
				if (levelContext != null && levelContext.IsMobaMode())
				{
					this.UpdateUIMap(ref this.m_actorPos);
				}
			}
			else if (this.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_EYE)
			{
				string prefabPath = (!this.actor.IsHostCamp()) ? MinimapSys.enemy_Eye : MinimapSys.self_Eye;
				Image component2 = mapPointer.transform.Find("eye").gameObject.GetComponent<Image>();
				component2.SetSprite(prefabPath, Singleton<CBattleSystem>.GetInstance().FormScript, true, false, false, false);
				transform.SetAsFirstSibling();
				this.m_actorPos = this.actor.myTransform.position;
				this.UpdateUIMap(ref this.m_actorPos);
			}
			else if (this.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster)
			{
				byte actorSubSoliderType = this.actor.ActorControl.GetActorSubSoliderType();
				bool flag = actorSubSoliderType == 8 || actorSubSoliderType == 9 || actorSubSoliderType == 7 || actorSubSoliderType == 13;
				if (flag)
				{
					return;
				}
				transform.SetAsFirstSibling();
				if (levelContext != null && levelContext.IsMobaMode())
				{
					this.m_actorPos = this.actor.myTransform.position;
					this.UpdateUIMap(ref this.m_actorPos);
				}
			}
			mapPointer.CustomSetActive(false);
			this.m_pointerLogicVisible = false;
		}

		public Vector3 GetSmallMapPointer_WorldPosition()
		{
			if (this.m_mapPointer_small != null)
			{
				return this.m_mapPointer_small.transform.position;
			}
			return new Vector3(0f, 0f, 0f);
		}

		private void ResetHudUI()
		{
			if ((this.actor.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Monster && this.actor.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_EYE) || this.HudType == HudCompType.Type_Hero || this.m_hud == null || this.HudType == HudCompType.Type_Hide)
			{
				return;
			}
			bool flag = this.actor.IsHostCamp();
			this.m_bHeroSameCamp = flag;
			ActorTypeDef actorTypeDef = this.actor.TheActorMeta.ActorType;
			if (this.HudType == HudCompType.Type_Boss)
			{
				actorTypeDef = ActorTypeDef.Actor_Type_Hero;
			}
			this.m_bloodImage.spriteName = Enum.GetName(typeof(SpriteNameEnum), (int)(ActorTypeDef.Actor_Type_EYE * actorTypeDef + ((!flag) ? 2 : 1)));
			if (this.HudType != HudCompType.Type_Eye)
			{
				this.m_bloodImage.SetNativeSize(Singleton<Camera_UI3D>.GetInstance().GetCurrentCamera(), 30f);
			}
			if (this.actor.ValueComponent.actorHpTotal > 0)
			{
				this.m_bloodImage.fillAmount = (float)this.actor.ValueComponent.actorHp / (float)this.actor.ValueComponent.actorHpTotal;
			}
			else
			{
				this.m_bloodImage.fillAmount = 0f;
			}
			if (this.m_VoiceIconImage)
			{
				this.m_VoiceIconImage.gameObject.CustomSetActive(false);
			}
			this.m_timerImg = null;
			if (this.HudType == HudCompType.Type_Eye)
			{
				this.m_timerImg = this.m_hud.transform.Find("Timer").GetComponent<Sprite3D>();
			}
			Transform hudPanel = this.GetHudPanel(this.HudType, flag);
			if (hudPanel != null)
			{
				this.m_hud.transform.SetParent(hudPanel, true);
				this.m_hud.transform.localScale = Vector3.one;
				this.m_hud.transform.localRotation = Quaternion.identity;
			}
			Singleton<Camera_UI3D>.GetInstance().GetCurrentCanvas().RefreshLayout(null);
			this.setHudLogicVisible(true);
			this.m_bloodDirty = true;
			this.HudInit_MapPointer();
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			if (curLvelContext != null)
			{
				this.setPointerVisible(curLvelContext.IsMobaMode(), true);
			}
			if (FogOfWar.enable && this.IsNormalJungle())
			{
				this.m_mapPointer_small.CustomSetActive(true);
				this.m_mapPointer_big.CustomSetActive(true);
				if (this.m_heroHead_small_Trans != null)
				{
					this.m_heroHead_small_Trans.gameObject.CustomSetActive(true);
				}
				if (this.m_heroHead_big_Trans != null)
				{
					this.m_heroHead_big_Trans.gameObject.CustomSetActive(true);
				}
				if (this.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
				{
					this.AddDirtyFlag(HudComponent3D.enDirtyFlag.PositionInMap);
					this.AddDirtyFlag(HudComponent3D.enDirtyFlag.Immediate);
				}
			}
		}

		public void ShowVoiceIcon(bool bShow)
		{
			if (this.m_VoiceIconImage)
			{
				this.m_VoiceIconImage.gameObject.CustomSetActive(bShow);
			}
		}

		public void SetSelected(bool bHost)
		{
			if (bHost)
			{
				this.m_bloodImage.spriteName = "bl_hero_self";
			}
			else
			{
				this.m_bloodImage.spriteName = "bl_hero_mid";
			}
		}

		public void OnActorDead()
		{
			if (this.actor.TheStaticData.TheBaseAttribute.DeadControl)
			{
				return;
			}
			this.setHudLogicVisible(false);
			this.setTextLogicVisible(false);
			this.setPointerVisible(false, true);
			DragonIcon.Check_Dragon_Born_Evt(this.actor, false);
			TowerHitMgr towerHitMgr = Singleton<CBattleSystem>.GetInstance().TowerHitMgr;
			if (this.actor != null && towerHitMgr != null)
			{
				towerHitMgr.Remove(this.actor.ObjID);
			}
			if (this.m_evtCom != null)
			{
				this.m_evtCom.isDead = true;
			}
		}

		public void OnActorRevive()
		{
			if (this.m_evtCom != null)
			{
				this.m_evtCom.isDead = false;
			}
			if (this.bReviveTimerShow)
			{
				this.HideReviveTimer();
			}
			this.setPointerVisible(true, true);
		}

		private void ResetStatusHeight(int hight)
		{
			for (int i = 0; i < 1; i++)
			{
				GameObject gameObject = this._statusGo[i];
				if (gameObject != null)
				{
					gameObject.transform.localPosition = new Vector3(0f, ((float)hight - HudComponent3D.OFFSET_HEIGHT) * 0.001f, 0f);
				}
			}
		}

		public void ShowReviveTimer(int yOffset, int reviveTime)
		{
			this.m_reviveTimerObj.CustomSetActive(true);
			this.m_bloodBackObj.CustomSetActive(false);
			this.m_bloodForeObj.CustomSetActive(false);
			this.m_energyForeObj.CustomSetActive(false);
			if (this.m_reviveTimerBar != null)
			{
				this.m_reviveTimerBar.fillAmount = 0f;
			}
			this.m_reviveTotalTime = reviveTime;
			this.hudHeightOffset = yOffset;
			this.hudHeight += this.hudHeightOffset;
			if (this.hudHeightOffset > 0)
			{
				this.ResetStatusHeight(this.hudHeight);
			}
			this.bReviveTimerShow = true;
			this.setHudLogicVisible(true);
		}

		public void HideReviveTimer()
		{
			this.m_reviveTimerObj.CustomSetActive(false);
			this.m_bloodBackObj.CustomSetActive(true);
			this.m_bloodForeObj.CustomSetActive(true);
			this.m_energyForeObj.CustomSetActive(true);
			if (this.m_reviveTimerBar != null)
			{
				this.m_reviveTimerBar.fillAmount = 0f;
			}
			this.m_reviveTotalTime = 0;
			this.hudHeight -= this.hudHeightOffset;
			if (this.hudHeightOffset > 0)
			{
				this.ResetStatusHeight(this.hudHeight);
			}
			this.hudHeightOffset = 0;
			this.bReviveTimerShow = false;
		}

		private void setHudLogicVisible(bool isVisible)
		{
			this.m_hudLogicVisible = isVisible;
			this.refreshHudVisible();
		}

		private void refreshHudVisible()
		{
			if (this.m_hud)
			{
				bool flag = this.m_hudLogicVisible && this.actor.Visible && this.actor.InCamera;
				if (flag != this.m_hud.activeSelf)
				{
					this.m_bloodDirty = true;
				}
			}
		}

		private void setTextLogicVisible(bool isVisible)
		{
			this.m_textLogicVisible = isVisible;
			this.refreshTextVisible();
		}

		private void refreshTextVisible()
		{
			if (this.m_textHudNode)
			{
				bool flag = this.m_textLogicVisible && this.actor.Visible && this.actor.InCamera;
				if (flag != this.m_textHudNode.activeSelf)
				{
					this.m_textHudNode.CustomSetActive(flag);
				}
			}
		}

		private void setPointerVisible(bool show, bool isLogicSet)
		{
			bool flag = !this.IsCallMonster() && this.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster && this.actor.ActorControl.GetActorSubType() == 2;
			bool flag2 = flag && this.actor.TheActorMeta.ConfigId == Singleton<BattleLogic>.get_instance().DragonId;
			if (flag && !flag2 && isLogicSet && !this.m_pointerLogicVisible && show && this.actor.Visible)
			{
				bool bActive = !this.actor.ActorControl.IsDeadState;
				this.m_mapPointer_small.CustomSetActive(bActive);
				this.m_mapPointer_big.CustomSetActive(bActive);
				if (this.m_heroHead_small_Trans != null)
				{
					this.m_heroHead_small_Trans.gameObject.CustomSetActive(bActive);
				}
				if (this.m_heroHead_big_Trans != null)
				{
					this.m_heroHead_big_Trans.gameObject.CustomSetActive(bActive);
				}
				this.m_pointerLogicVisible = false;
				return;
			}
			if (isLogicSet)
			{
				this.m_pointerLogicVisible = show;
			}
			if (!flag)
			{
				bool flag3 = this.m_pointerLogicVisible && this.actor.Visible && (!this.actor.ActorControl.IsDeadState || this.actor.TheStaticData.TheBaseAttribute.DeadControl);
				if (this.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ && !this.actor.ActorControl.IsDeadState)
				{
					flag3 = true;
				}
				if (this.m_mapPointer_small)
				{
					this.m_mapPointer_small.CustomSetActive(flag3);
				}
				if (this.m_mapPointer_big)
				{
					this.m_mapPointer_big.CustomSetActive(flag3);
				}
				if (this.m_heroHead_small_Trans != null)
				{
					this.m_heroHead_small_Trans.gameObject.CustomSetActive(flag3);
				}
				if (this.m_heroHead_big_Trans != null)
				{
					this.m_heroHead_big_Trans.gameObject.CustomSetActive(flag3);
				}
				if (this.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero && flag3)
				{
					this.AddDirtyFlag(HudComponent3D.enDirtyFlag.PositionInMap);
					this.AddDirtyFlag(HudComponent3D.enDirtyFlag.Immediate);
				}
			}
		}

		private bool IsCallMonster()
		{
			if (this.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster)
			{
				MonsterWrapper monsterWrapper = this.actor.ActorControl as MonsterWrapper;
				if (monsterWrapper != null)
				{
					PoolObjHandle<ActorRoot> hostActor = monsterWrapper.hostActor;
					if (hostActor && hostActor.get_handle().TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
					{
						return true;
					}
				}
			}
			return false;
		}

		private void setReviveTimerBar()
		{
			if (this.bReviveTimerShow && this.m_reviveTotalTime > 0 && this.m_reviveTimerBar != null)
			{
				this.m_reviveTimerBar.fillAmount = 1f - (float)this.actor.ActorControl.ReviveCooldown / (float)this.m_reviveTotalTime;
			}
		}

		private void setSkillTimerBar()
		{
			if (this.m_skillTimerObj == null || this.m_skillTimerBar == null || this.m_skillTimeInfo == null)
			{
				return;
			}
			if (this.m_skillTimeInfo.totalTime <= 0 || this.m_skillTimeInfo.leftTime <= 0)
			{
				this.m_skillTimerObj.CustomSetActive(false);
				if (this.m_skillTimerBar.fillAmount != 0f)
				{
					this.m_skillTimerBar.fillAmount = 0f;
				}
			}
			else
			{
				float fillAmount = (float)this.m_skillTimeInfo.leftTime / (float)this.m_skillTimeInfo.totalTime;
				this.m_skillTimerBar.fillAmount = fillAmount;
				this.m_skillTimerObj.CustomSetActive(true);
				ulong logicFrameTick = Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
				this.m_skillTimeInfo.leftTime = this.m_skillTimeInfo.totalTime - (int)(logicFrameTick - this.m_skillTimeInfo.starTime);
			}
		}

		public void SetOutOfControlBar()
		{
			if (this.m_outofControlList.get_Count() <= 0)
			{
				this.m_outOfControlGo.CustomSetActive(false);
				if (this.m_outOfControlBar.fillAmount != 0f)
				{
					this.m_outOfControlBar.fillAmount = 0f;
				}
				return;
			}
			CoutofControlInfo coutofControlInfo = this.m_outofControlList.get_Item(0);
			for (int i = 1; i < this.m_outofControlList.get_Count(); i++)
			{
				if (coutofControlInfo.leftTime < this.m_outofControlList.get_Item(i).leftTime)
				{
					coutofControlInfo = this.m_outofControlList.get_Item(i);
				}
			}
			this.m_outOfControlGo.CustomSetActive(true);
			float fillAmount = (float)coutofControlInfo.leftTime / (float)coutofControlInfo.totalTime;
			this.m_outOfControlBar.fillAmount = fillAmount;
		}

		private void OnPlayerLimitMove(ref LimitMoveEventParam _param)
		{
			if (!_param.src)
			{
				return;
			}
			if (_param.src.get_handle() == this.actor)
			{
				DebugHelper.Assert(_param.totalTime > 0, "被控时间是0，还控个毛啊, combineid" + _param.combineID);
				if (_param.totalTime <= 0)
				{
					return;
				}
				CoutofControlInfo coutofControlInfo = new CoutofControlInfo(_param.combineID, _param.totalTime, _param.totalTime);
				this.m_outofControlList.Add(coutofControlInfo);
			}
		}

		private void OnPlayerCancelLimitMove(ref LimitMoveEventParam _param)
		{
			if (!_param.src)
			{
				return;
			}
			if (_param.src.get_handle() == this.actor)
			{
				for (int i = 0; i < this.m_outofControlList.get_Count(); i++)
				{
					if (this.m_outofControlList.get_Item(i).combId == _param.combineID)
					{
						this.m_outofControlList.RemoveAt(i);
						i--;
					}
				}
			}
		}

		private void OnPlayerSkillTimer(ref SkillTimerEventParam _param)
		{
			if (!_param.src)
			{
				return;
			}
			if (_param.src.get_handle() == this.actor)
			{
				if (this.m_skillTimeInfo == null)
				{
					this.m_skillTimeInfo = new SkillTimerInfo(_param.totalTime, _param.totalTime, _param.starTime);
				}
				else
				{
					this.m_skillTimeInfo.setSkillTimerParam(_param.totalTime, _param.totalTime, _param.starTime);
				}
			}
		}

		public void SetTextHud(string text, int txt_offset_x, int txt_offset_y, bool bCenter = false)
		{
			if (this.m_textHudContainer == null)
			{
				this.m_textHudContainer = Singleton<CBattleSystem>.GetInstance().TextHudContainer;
			}
			if (this.m_textHudContainer != null && this.m_textHudNode == null)
			{
				this.Init_TextHud(this.m_textHudContainer.GetElement(), false);
			}
			this.txt_hud_offset_x = txt_offset_x;
			this.txt_hud_offset_y = txt_offset_y;
			bool flag = string.IsNullOrEmpty(text);
			if (flag)
			{
				this.setTextLogicVisible(false);
			}
			if (flag || this.m_textHudNode == null)
			{
				return;
			}
			this.SetTextHudContent(text);
			this.setTextLogicVisible(true);
		}

		public void AddInEquipShopHud()
		{
			if (this.m_inOutEquipShopHudContainer == null)
			{
				this.m_inOutEquipShopHudContainer = Singleton<CBattleSystem>.GetInstance().GetInOutEquipShopHudContainer();
			}
			if (this.m_inOutEquipShopHudContainer != null && this.m_inOutEquipShopHud == null)
			{
				int element = this.m_inOutEquipShopHudContainer.GetElement();
				if (element >= 0)
				{
					this.m_inOutEquipShopHud = this.m_inOutEquipShopHudContainer.GetElement(element);
					if (this.m_inOutEquipShopHud != null)
					{
						this.m_inOutEquipShopHud.CustomSetActive(this.actor.Visible && this.actor.InCamera && (!this.actor.ActorControl.IsDeadState || this.actor.TheStaticData.TheBaseAttribute.DeadControl));
					}
				}
			}
		}

		public void RemoveInEquipShopHud()
		{
			if (this.m_inOutEquipShopHudContainer != null && this.m_inOutEquipShopHud != null)
			{
				this.m_inOutEquipShopHudContainer.RecycleElement(this.m_inOutEquipShopHud);
				this.m_inOutEquipShopHud = null;
			}
		}

		private void Init_TextHud(int hudSequence, bool bCenter = false)
		{
			this.m_textHudNode = this.m_textHudContainer.GetElement(hudSequence);
			if (this.m_textHudNode != null)
			{
				this.textCom = Utility.GetComponetInChild<Text>(this.m_textHudNode, "bg/Text");
				this.m_textHudNode.CustomSetActive(true);
			}
		}

		private void SetTextHudContent(string txt)
		{
			this.textCom.text = txt;
		}

		public override void UpdateLogic(int delta)
		{
		}

		public void UpdateMiniMapHeroRotation()
		{
			if (this.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero && this.m_bHeroSameCamp)
			{
				VInt3 forward = this.actor.forward;
				if (this.m_actorForward != forward)
				{
					this.AddDirtyFlag(HudComponent3D.enDirtyFlag.ForwardInMap);
					this.m_actorForward = forward;
				}
				if (this.actor.Visible && (this.IsNeedUpdateUI() || this.HasDirtyFlag(HudComponent3D.enDirtyFlag.ForwardInMap)))
				{
					Quaternion actorForward_MiniMap = HudComponent3D.GetActorForward_MiniMap(ref this.m_actorForward);
					if (this.m_heroIconBG_small != null && this.m_heroIconBG_small.transform != null)
					{
						this.m_heroIconBG_small.transform.rotation = actorForward_MiniMap;
					}
					if (this.m_heroIconBG_big != null && this.m_heroIconBG_big.transform != null)
					{
						this.m_heroIconBG_big.transform.rotation = actorForward_MiniMap;
					}
					this.RemoveDirtyFlag(HudComponent3D.enDirtyFlag.ForwardInMap);
				}
			}
		}

		public static Quaternion GetActorForward_MiniMap(ref VInt3 forward)
		{
			float num = Mathf.Atan2((float)forward.z, (float)forward.x) * 57.29578f - 90f;
			if (Singleton<BattleLogic>.get_instance().GetCurLvelContext().m_isCameraFlip)
			{
				num -= 180f;
			}
			return Quaternion.AngleAxis(num, Vector3.forward);
		}

		public override void LateUpdate(int delta)
		{
			if (this.m_hud == null)
			{
				return;
			}
			Vector3 position = this.actor.myTransform.position;
			if (this.m_actorPos != position)
			{
				this.AddDirtyFlag(HudComponent3D.enDirtyFlag.PositionInMap);
			}
			if (this.HasDirtyFlag(HudComponent3D.enDirtyFlag.ForcePositionInMap))
			{
				if (this.IsNormalJungle())
				{
					Vector3 vector = (Vector3)this.actor.BornPos;
					this.UpdateUIMap(ref vector);
				}
				else
				{
					this.UpdateUIMap(ref position);
					this.m_actorPos = position;
				}
				this.RemoveDirtyFlag(HudComponent3D.enDirtyFlag.ForcePositionInMap);
				this.RemoveDirtyFlag(HudComponent3D.enDirtyFlag.PositionInMap);
				this.RemoveDirtyFlag(HudComponent3D.enDirtyFlag.Immediate);
			}
			else if (this.actor.Visible && !this.IsNormalJungle() && (this.HasDirtyFlag(HudComponent3D.enDirtyFlag.PositionInMap) || this.IsNeedUpdateUI()))
			{
				if (this.actor.ActorControl.GetActorSubType() == 1 && (ulong)this.actor.ObjID % (ulong)((long)HudComponent3D.soilderFrameCount) == (ulong)Singleton<FrameSynchr>.get_instance().CurFrameNum % (ulong)((long)HudComponent3D.soilderFrameCount))
				{
					this.UpdateUIMap(ref position);
					this.m_actorPos = position;
					this.RemoveDirtyFlag(HudComponent3D.enDirtyFlag.PositionInMap);
				}
				if (this.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero && (this.HasDirtyFlag(HudComponent3D.enDirtyFlag.Immediate) || (ulong)this.actor.ObjID % (ulong)((long)HudComponent3D.heroFrameCount) == (ulong)Singleton<FrameSynchr>.get_instance().CurFrameNum % (ulong)((long)HudComponent3D.heroFrameCount)))
				{
					this.UpdateUIMap(ref position);
					this.m_actorPos = position;
					this.RemoveDirtyFlag(HudComponent3D.enDirtyFlag.PositionInMap);
					this.RemoveDirtyFlag(HudComponent3D.enDirtyFlag.Immediate);
				}
			}
			this.UpdateMiniMapHeroRotation();
			if (this.m_bloodDirty)
			{
				this.m_bloodDirty = false;
				bool flag = this.m_hudLogicVisible && this.actor.Visible && this.actor.InCamera;
				this.m_hud.CustomSetActive(flag);
				if (this.m_signalImage != null && flag != this.m_signalImage.gameObject.activeSelf)
				{
					this.m_signalImage.gameObject.CustomSetActive(flag);
				}
			}
			if (this.HudType == HudCompType.Type_Hide || !this.actor.Visible || !this.actor.InCamera)
			{
				return;
			}
			if (this.m_hud.activeSelf)
			{
				Vector3 position2 = position;
				position2.y += (float)this.hudHeight * 0.001f;
				Vector3 vector2 = Camera.main.WorldToScreenPoint(position2);
				bool flag2 = this.m_bloodPos != vector2;
				this.m_bloodPos = vector2;
				if (this.IsNeedUpdateUI() || flag2)
				{
					this.UpdateUIHud(ref vector2);
				}
			}
			if (this.m_inOutEquipShopHud != null)
			{
				this.m_inOutEquipShopHud.CustomSetActive(this.actor.Visible && this.actor.InCamera && (!this.actor.ActorControl.IsDeadState || this.actor.TheStaticData.TheBaseAttribute.DeadControl));
				if (this.m_inOutEquipShopHud.activeSelf)
				{
					Vector3 position3 = position;
					position3.y += (float)this.hudHeight * 0.001f;
					Vector3 v = Camera.main.WorldToScreenPoint(position3);
					FightForm fightForm = Singleton<CBattleSystem>.GetInstance().FightForm;
					if (fightForm != null && fightForm._formScript != null)
					{
						this.m_inOutEquipShopHud.transform.position = CUIUtility.ScreenToWorldPoint(fightForm._formScript.GetCamera(), v, v.z);
					}
				}
			}
			if (this.m_exclamationObj != null)
			{
				this.m_exclamationObj.transform.position = new Vector3(position.x, position.y + this.m_exclamationObjOffsetY, position.z);
			}
			if (this.m_outOfControlBar != null)
			{
				int i = 0;
				while (i < this.m_outofControlList.get_Count())
				{
					this.m_outofControlList.get_Item(i).leftTime -= (int)(Time.deltaTime * 1000f);
					if (this.m_outofControlList.get_Item(i).leftTime <= 0)
					{
						this.m_outofControlList.RemoveAt(i);
					}
					else
					{
						i++;
					}
				}
				this.SetOutOfControlBar();
			}
			this.setReviveTimerBar();
			this.setSkillTimerBar();
			if (this.CurAniIndex_ != -1 && !this.HeroProficiencyAni_[this.CurAniIndex_].isPlaying)
			{
				this.HeroProficiencyObj_[this.CurAniIndex_].CustomSetActive(false);
			}
			if (!this._isDecreasingHp)
			{
				return;
			}
			if (this._bloodDecImage == null || this._bloodDecImage.fillAmount < this.m_bloodImage.fillAmount)
			{
				return;
			}
			this._bloodDecImage.fillAmount = this._bloodDecImage.fillAmount - 0.0086f;
			if (this._bloodDecImage.fillAmount > this.m_bloodImage.fillAmount)
			{
				return;
			}
			this._bloodDecImage.fillAmount = this.m_bloodImage.fillAmount;
			this._isDecreasingHp = false;
		}

		public void UpdateBloodBar(int curValue, int maxValue)
		{
			if (this.m_hud != null && maxValue != 0)
			{
				if (curValue == 0 || (this.HudType == HudCompType.Type_Soldier && curValue >= maxValue))
				{
					if (!this.actor.TheStaticData.TheBaseAttribute.DeadControl)
					{
						this.setHudLogicVisible(false);
					}
					if (this.m_textHudNode != null)
					{
						this.setTextLogicVisible(false);
					}
				}
				else
				{
					this.setHudLogicVisible(true);
					if (ActorHelper.IsHostCtrlActor(ref this.actorPtr))
					{
						int childCount = this.m_hud.transform.parent.childCount;
						if (childCount - 1 != this.m_hud.transform.GetSiblingIndex())
						{
							this.m_hud.transform.SetAsLastSibling();
						}
					}
					if (this.m_bloodImage != null)
					{
						this.m_bloodImage.fillAmount = (float)curValue / (float)maxValue;
						this.UpdateHpAndShieldBarEffect();
						this.TryToStartDecreaseHpEffect();
					}
				}
			}
		}

		public void UpdateTimerBar(int curValue, int totalValue)
		{
			if (this.m_timerImg != null)
			{
				this.m_timerImg.fillAmount = (float)curValue / (float)totalValue;
			}
		}

		public void UpdateShieldValue(ProtectType shieldType, int changeValue)
		{
			this._curShield1 += changeValue;
			if (this._curShield1 < 0)
			{
				this._curShield1 = 0;
			}
			this.UpdateHpAndShieldBarEffect();
		}

		protected void UpdateHpAndShieldBarEffect()
		{
			if (this._shieldGo == null)
			{
				return;
			}
			this._shieldGo.CustomSetActive(this._curShield1 > 0);
			int num = (this._curShield1 <= 0) ? this.actor.ValueComponent.actorHpTotal : Math.Max(this.actor.ValueComponent.actorHpTotal, this._curShield1 + this.actor.ValueComponent.actorHp);
			if ((float)num > 0f)
			{
				this.m_bloodImage.fillAmount = (float)this.actor.ValueComponent.actorHp / (float)num;
			}
			else
			{
				this.m_bloodImage.fillAmount = 0f;
			}
			this.SetBlackBar(num);
			if (this._curShield1 <= 0)
			{
				return;
			}
			float x = this._shieldImagWidth * this.m_bloodImage.fillAmount;
			this._shieldGo.GetComponent<Sprite3D>().fillAmount = (float)this._curShield1 / (float)num;
			this._shieldGo.GetComponent<Transform>().localPosition = new Vector3(x, 0f, 0f);
		}

		protected void SetBlackBar(int curMaxHpBarValue)
		{
			if (curMaxHpBarValue == this._lastMaxBarValue)
			{
				return;
			}
			this._lastMaxBarValue = curMaxHpBarValue;
			int i = 1000;
			int num = -1;
			while (i < curMaxHpBarValue)
			{
				num++;
				if (num < this.m_blackBars.get_Count())
				{
					if (!this.m_blackBars.get_Item(num).activeSelf)
					{
						this.m_blackBars.get_Item(num).SetActive(true);
					}
				}
				else
				{
					GameObject gameObject = Singleton<CGameObjectPool>.GetInstance().GetGameObject("UI3D/Battle/BlackBarBlack.prefab", 0);
					if (!gameObject.activeSelf)
					{
						gameObject.SetActive(true);
					}
					gameObject.transform.parent = this.m_bloodImage.transform;
					gameObject.transform.SetSiblingIndex(this.m_bloodImage.transform.childCount - 1);
					gameObject.transform.localRotation = Quaternion.identity;
					this.m_blackBars.Add(gameObject);
					Singleton<Camera_UI3D>.GetInstance().GetCurrentCanvas().RefreshLayout(null);
				}
				bool flag = (num + 1) % 5 == 0;
				this.m_blackBars.get_Item(num).transform.localScale = ((!flag) ? new Vector3(1f, 0.72f, 1f) : new Vector3(1.25f, 1f, 1f));
				this.m_blackBars.get_Item(num).transform.localPosition = new Vector3((float)i / (float)curMaxHpBarValue * this._shieldImagWidth, (!flag) ? 0.065f : 0f, 0f);
				i += 1000;
			}
			for (int j = num + 1; j < this.m_blackBars.get_Count(); j++)
			{
				this.m_blackBars.get_Item(j).SetActive(false);
			}
		}

		protected void SetHonor(int honorId = 0, int honorLevel = 0)
		{
			if (honorId == 0)
			{
				return;
			}
			string text = string.Format("UI3D/Battle/Img_Badge_{0}_{1}.prefab", honorId, honorLevel);
			this.m_honor = Singleton<CGameObjectPool>.GetInstance().GetGameObject(text, 0);
			DebugHelper.Assert(this.m_honor != null, string.Format("{0} doesn't exist!", text));
			if (this.m_honor == null)
			{
				return;
			}
			this.m_honorAni = this.m_honor.GetComponent<Animation>();
			if (!this.m_honor.activeSelf)
			{
				this.m_honor.SetActive(true);
			}
			this.EndHonorAni();
			this.m_honor.transform.parent = this.m_hud.transform;
			Vector3 position = this.m_hud.transform.position;
			position.y += -0.64f;
			position.x += 2.68f;
			this.m_honor.transform.localPosition = position;
		}

		protected void TryToStartDecreaseHpEffect()
		{
			if (this._bloodDecImage == null)
			{
				return;
			}
			if (this.m_bloodImage.fillAmount >= this._bloodDecImage.fillAmount)
			{
				this._bloodDecImage.fillAmount = this.m_bloodImage.fillAmount;
				this._isDecreasingHp = false;
			}
			if (!this._isDecreasingHp && this.m_bloodImage.fillAmount < this._bloodDecImage.fillAmount)
			{
				this._isDecreasingHp = true;
			}
		}

		private void UpdateUIHud(ref Vector3 bloodPosition)
		{
			if (this.m_hud != null)
			{
				bloodPosition.Set(bloodPosition.x, bloodPosition.y, 30f);
				Camera currentCamera = Singleton<Camera_UI3D>.GetInstance().GetCurrentCamera();
				if (currentCamera == null)
				{
					return;
				}
				this.m_hud.transform.position = currentCamera.ScreenToWorldPoint(bloodPosition);
				if (this.m_textHudNode != null && Singleton<CBattleSystem>.GetInstance().FormScript != null && (!this.actor.ActorControl.IsDeadState || this.actor.TheStaticData.TheBaseAttribute.DeadControl))
				{
					Vector3 vector = CUIUtility.ScreenToWorldPoint(Singleton<CBattleSystem>.GetInstance().FormScript.GetCamera(), bloodPosition, this.m_textHudNode.transform.position.z);
					this.m_textHudNode.transform.position = new Vector3(vector.x + (float)this.txt_hud_offset_x, vector.y + (float)this.txt_hud_offset_y, vector.z);
				}
			}
		}

		private void UpdateUIMap(ref Vector3 actorPosition)
		{
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			if (curLvelContext != null && curLvelContext.IsMobaMode())
			{
				this.UpdateUIMap_Inside(ref actorPosition);
			}
		}

		private void onSoulExpChange(PoolObjHandle<ActorRoot> act, int changeValue, int curVal, int maxVal)
		{
			if (this.m_soulImage != null && act.get_handle() == this.actor)
			{
				this.m_soulImage.fillAmount = (float)curVal / (float)maxVal;
				return;
			}
			if (this.IsPlayerCopy())
			{
				MonsterWrapper monsterWrapper = this.actor.ActorControl as MonsterWrapper;
				if (monsterWrapper != null && monsterWrapper.hostActor && monsterWrapper.hostActor == act && this.m_soulImage != null)
				{
					this.m_soulImage.fillAmount = (float)curVal / (float)maxVal;
				}
			}
		}

		private void onEnergyExpChange(PoolObjHandle<ActorRoot> act, int curVal, int maxVal)
		{
			if (this.m_energyImage != null && act.get_handle() == this.actor && maxVal > 0)
			{
				if (curVal < 0)
				{
					curVal = 0;
				}
				this.m_energyImage.fillAmount = (float)curVal / (float)maxVal;
			}
		}

		private void onSoulLvlChange(PoolObjHandle<ActorRoot> act, int curVal)
		{
			if (this.m_soulLevel != null && act.get_handle() == this.actor)
			{
				this.m_soulLevel.text = curVal.ToString();
				return;
			}
			if (this.IsPlayerCopy())
			{
				MonsterWrapper monsterWrapper = this.actor.ActorControl as MonsterWrapper;
				if (monsterWrapper != null && monsterWrapper.hostActor && monsterWrapper.hostActor == act && this.m_soulLevel != null)
				{
					this.m_soulLevel.text = curVal.ToString();
				}
			}
		}

		private void UpdateUIMap_Inside(ref Vector3 actorPosition)
		{
			if (this.m_mapPointer_small_Trans != null)
			{
				Vector3 position = MiniMapSysUT.Set3DUIWorldPos_ByScreenPoint(ref actorPosition, true);
				this.m_mapPointer_small_Trans.position = position;
				if (this.m_heroHead_small_Trans != null)
				{
					this.m_heroHead_small_Trans.position = position;
				}
			}
			if (this.m_mapPointer_big_Trans != null)
			{
				float x;
				float y;
				Vector3 position2 = MiniMapSysUT.Set3DUIWorldPos_ByScreenPoint(ref actorPosition, false, out x, out y);
				this.m_mapPointer_big_Trans.position = position2;
				Vector3 localPosition = this.m_mapPointer_big_Trans.localPosition;
				localPosition.z = 0f;
				this.m_mapPointer_big_Trans.localPosition = localPosition;
				if (this.m_heroHead_big_Trans != null)
				{
					this.m_heroHead_big_Trans.position = position2;
					Vector3 localPosition2 = this.m_heroHead_big_Trans.localPosition;
					localPosition2.z = 0f;
					this.m_heroHead_big_Trans.localPosition = localPosition2;
				}
				if (this.m_evtCom != null)
				{
					this.m_evtCom.m_screenSize.center = new Vector2(x, y);
				}
			}
		}

		public void RefreshMapPointerBig()
		{
			CUIFormScript formScript = Singleton<CBattleSystem>.GetInstance().FormScript;
			if (formScript != null && formScript.m_sgameGraphicRaycaster != null && this.m_mapPointer_big != null)
			{
				formScript.m_sgameGraphicRaycaster.RefreshGameObject(this.m_mapPointer_big);
			}
		}

		public void PlayMapEffect(MiniMapEffect mme)
		{
			if (this.m_effectRoot_small)
			{
				GameObject gameObject = Utility.FindChild(this.m_effectRoot_small, mme.ToString());
				SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
				if (gameObject && curLvelContext != null)
				{
					this.m_actorPos = this.actor.myTransform.position;
					(gameObject.transform as RectTransform).anchoredPosition = new Vector2(this.m_actorPos.x * Singleton<CBattleSystem>.get_instance().world_UI_Factor_Small.x, this.m_actorPos.z * Singleton<CBattleSystem>.get_instance().world_UI_Factor_Small.y);
					if (gameObject.activeSelf)
					{
						gameObject.CustomSetActive(false);
					}
					gameObject.CustomSetActive(true);
				}
			}
		}

		private bool IsNeedUpdateUI()
		{
			return CUIManager.s_uiSystemRenderFrameCounter - this.m_hudFrameStampOnCreated <= 2;
		}

		public CUIContainerScript GetMapPointerContainer(bool bMiniMap)
		{
			MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
			if (theMinimapSys != null)
			{
				GameObject gameObject = null;
				if (this.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
				{
					gameObject = ((!bMiniMap) ? theMinimapSys.bmpcHero : theMinimapSys.mmpcHero);
				}
				else if (this.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_EYE)
				{
					gameObject = ((!bMiniMap) ? theMinimapSys.bmpcEye : theMinimapSys.mmpcEye);
				}
				else if (this.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster)
				{
					byte actorSubType = this.actor.ActorControl.GetActorSubType();
					if (actorSubType == 2)
					{
						byte actorSubSoliderType = this.actor.ActorControl.GetActorSubSoliderType();
						if (actorSubSoliderType == 8 || actorSubSoliderType == 9 || actorSubSoliderType == 13 || actorSubSoliderType == 7)
						{
							return null;
						}
						if (actorSubSoliderType == 11)
						{
							gameObject = ((!bMiniMap) ? theMinimapSys.bmpcRedBuff : theMinimapSys.mmpcRedBuff);
						}
						else if (actorSubSoliderType == 10)
						{
							gameObject = ((!bMiniMap) ? theMinimapSys.bmpcBlueBuff : theMinimapSys.mmpcBlueBuff);
						}
						else if (bMiniMap)
						{
							gameObject = theMinimapSys.mmpcJungle;
						}
						else
						{
							gameObject = theMinimapSys.bmpcJungle;
						}
					}
					else if (bMiniMap)
					{
						gameObject = ((!this.actor.IsHostCamp()) ? theMinimapSys.mmpcEnemy : theMinimapSys.mmpcAlies);
					}
					else
					{
						gameObject = ((!this.actor.IsHostCamp()) ? theMinimapSys.bmpcEnemy : theMinimapSys.bmpcAlies);
					}
				}
				else if (this.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ)
				{
					if (this.actor.TheStaticData.TheOrganOnlyInfo.OrganType != 1 && this.actor.TheStaticData.TheOrganOnlyInfo.OrganType != 2 && this.actor.TheStaticData.TheOrganOnlyInfo.OrganType != 4)
					{
						return null;
					}
					gameObject = ((!bMiniMap) ? theMinimapSys.bmpcOrgan : theMinimapSys.mmpcOrgan);
				}
				if (gameObject != null)
				{
					return gameObject.GetComponent<CUIContainerScript>();
				}
			}
			return null;
		}

		private Transform GetHudPanel(HudCompType hudType, bool isHostCamp)
		{
			string text = "Unknown_Panel";
			if (null == Moba_Camera.currentMobaCamera)
			{
				return null;
			}
			Camera currentCamera = Singleton<Camera_UI3D>.GetInstance().GetCurrentCamera();
			if (currentCamera == null)
			{
				return null;
			}
			Transform transform = currentCamera.transform.Find("Hud");
			if (transform == null)
			{
				GameObject gameObject = new GameObject("Hud");
				transform = gameObject.transform;
				transform.parent = currentCamera.transform;
				transform.localPosition = Vector3.zero;
				transform.localRotation = Quaternion.identity;
				transform.localScale = Vector3.one;
				string[] array = new string[]
				{
					enBattleFormWidget.Panel_EnemySoliderHud.ToString(),
					enBattleFormWidget.Panel_SelfSoliderHud.ToString(),
					enBattleFormWidget.Panel_EnemyHeroHud.ToString(),
					enBattleFormWidget.Panel_SelfHeroHud.ToString(),
					text
				};
				for (int i = 0; i < array.Length; i++)
				{
					GameObject gameObject2 = new GameObject(array[i]);
					Transform transform2 = gameObject2.transform;
					transform2 = gameObject2.transform;
					transform2.parent = transform;
					transform2.localPosition = Vector3.zero;
					transform2.localRotation = Quaternion.identity;
					transform2.localScale = Vector3.one;
				}
			}
			string text2 = string.Empty;
			if (hudType == HudCompType.Type_Soldier)
			{
				text2 = ((!isHostCamp) ? enBattleFormWidget.Panel_EnemySoliderHud.ToString() : enBattleFormWidget.Panel_SelfSoliderHud.ToString());
			}
			else if (hudType == HudCompType.Type_Hero || hudType == HudCompType.Type_Organ)
			{
				text2 = ((!isHostCamp) ? enBattleFormWidget.Panel_EnemyHeroHud.ToString() : enBattleFormWidget.Panel_SelfHeroHud.ToString());
			}
			if (string.IsNullOrEmpty(text2))
			{
				text2 = text;
			}
			Transform transform3 = transform.Find(text2);
			if (transform3 == null)
			{
			}
			return transform3;
		}

		public void ShowHeadExclamationMark(string eftPath, float offset_height)
		{
			if (FogOfWar.enable && this.actor.HorizonMarker != null && !this.actor.HorizonMarker.IsVisibleFor(Singleton<GamePlayerCenter>.get_instance().GetHostPlayer().PlayerCamp))
			{
				return;
			}
			SignalPanel signalPanel = (Singleton<CBattleSystem>.GetInstance().FightForm == null) ? null : Singleton<CBattleSystem>.GetInstance().FightForm.GetSignalPanel();
			if (signalPanel == null || this.m_exclamationObj != null)
			{
				return;
			}
			Vector3 position = this.actor.myTransform.position;
			Vector3 worldPosition = new Vector3(position.x, position.y + offset_height, position.z);
			this.m_exclamationObj = this.CreateSignalGameObject(eftPath, worldPosition);
			this.m_exclamationObjOffsetY = offset_height;
			Singleton<CGameObjectPool>.GetInstance().RecycleGameObjectDelay(this.m_exclamationObj, 5000, new CGameObjectPool.OnDelayRecycleDelegate(this.OnSinglePlayEnd));
		}

		public GameObject CreateSignalGameObject(string singlePath, Vector3 worldPosition)
		{
			GameObject pooledGameObjLOD = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(singlePath, true, SceneObjType.Temp, worldPosition);
			pooledGameObjLOD.CustomSetActive(true);
			return pooledGameObjLOD;
		}

		private void OnSinglePlayEnd(GameObject go)
		{
			if (this.m_exclamationObj == go)
			{
				this.m_exclamationObj = null;
			}
		}

		private void InitStatus()
		{
			this._statusGo = new GameObject[1];
			if (!this._mountPoint)
			{
				this._mountPoint = new GameObject("MountPoint");
				this._mountPoint.transform.SetParent(this.actor.myTransform);
				this._mountPoint.transform.localPosition = Vector3.zero;
				this._mountPoint.transform.localScale = Vector3.one;
				this._mountPoint.transform.localRotation = Quaternion.identity;
			}
		}

		public void SetComVisible(bool bVisiable)
		{
			if (this._mountPoint)
			{
				this._mountPoint.CustomSetActive(bVisiable);
			}
			this.refreshHudVisible();
			this.refreshTextVisible();
			bool flag = this.actor.ActorControl.GetActorSubType() == 2;
			if (flag && this.actor.TheActorMeta.ConfigId != Singleton<BattleLogic>.get_instance().DragonId)
			{
				this.setPointerVisible(bVisiable, true);
			}
			else
			{
				this.setPointerVisible(false, false);
			}
		}

		public void ShowStatus(StatusHudType st)
		{
			if (!this._mountPoint)
			{
				return;
			}
			this._mountPoint.CustomSetActive(this.actor.Visible);
			GameObject gameObject = this._statusGo[(int)st];
			if (!gameObject)
			{
				Object content = Singleton<CResourceManager>.GetInstance().GetResource(HudComponent3D._statusResPath[(int)st], typeof(GameObject), 0, false, false).m_content;
				if (content)
				{
					gameObject = (GameObject)Object.Instantiate(content);
					this._statusGo[(int)st] = gameObject;
					if (gameObject != null)
					{
						gameObject.transform.SetParent(this._mountPoint.transform);
						gameObject.transform.localPosition = new Vector3(0f, ((float)this.hudHeight - HudComponent3D.OFFSET_HEIGHT) * 0.001f, 0f);
					}
				}
			}
			if (gameObject)
			{
				gameObject.CustomSetActive(true);
			}
		}

		public void HideStatus(StatusHudType st)
		{
			DebugHelper.Assert(this._statusGo != null, "_statusGo ==null");
			GameObject gameObject = (this._statusGo == null) ? null : this._statusGo[(int)st];
			if (gameObject)
			{
				gameObject.CustomSetActive(false);
			}
		}

		public bool HasStatus(StatusHudType st)
		{
			DebugHelper.Assert(this._statusGo != null, "_statusGo ==null");
			return this._statusGo != null && this._statusGo[(int)st] != null && this._statusGo[(int)st].activeSelf;
		}

		private void OnBaseAttacked(ref DefaultGameEventParam evtParam)
		{
			if (evtParam.src && evtParam.src == this.actorPtr)
			{
				if (this.actor.IsHostCamp())
				{
					if (Singleton<FrameSynchr>.GetInstance().LogicFrameTick - this.LastBaseAttackTime > 20000uL)
					{
						GameObject mapPointer_small = this.m_mapPointer_small;
						if (mapPointer_small != null)
						{
							Animator component = mapPointer_small.GetComponent<Animator>();
							if (component != null)
							{
								component.enabled = true;
								component.Play("BaseTip", 0, 0f);
							}
						}
						Singleton<CSoundManager>.GetInstance().PlayBattleSound2D("UI_retreat");
						this.LastBaseAttackTime = Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
					}
					TowerHitMgr towerHitMgr = Singleton<CBattleSystem>.GetInstance().TowerHitMgr;
					if (towerHitMgr != null)
					{
						towerHitMgr.TryActive(this.actor.ObjID, this.MapPointerSmall);
					}
				}
				if (this.bigTower_spt3d != null && this.smallTower_spt3d != null)
				{
					Sprite3D arg_12E_0 = this.smallTower_spt3d;
					float single = evtParam.src.get_handle().ValueComponent.GetHpRate().get_single();
					this.bigTower_spt3d.fillAmount = single;
					arg_12E_0.fillAmount = single;
				}
			}
		}

		public Sprite3D GetBigTower_Spt3D()
		{
			return this.bigTower_spt3d;
		}

		public Sprite3D GetSmallTower_Spt3D()
		{
			return this.smallTower_spt3d;
		}

		private int TranslateProficiencyLevelToIndex(uint proficiencyLevel)
		{
			return (int)(proficiencyLevel - 3u);
		}

		public bool PlayProficiencyAni(uint proficiencyLevel)
		{
			if (this.m_hud == null)
			{
				return false;
			}
			if (this.CurAniIndex_ != -1 && this.HeroProficiencyAni_[this.CurAniIndex_].isPlaying)
			{
				return false;
			}
			int num = this.TranslateProficiencyLevelToIndex(proficiencyLevel);
			if (num >= 0 && num < 3 && this.HeroProficiencyAni_[num] != null)
			{
				this.HeroProficiencyObj_[num].CustomSetActive(true);
				this.HeroProficiencyAni_[num].Play(HudComponent3D.HeroProficiencyShowNames_[num]);
				this.CurAniIndex_ = num;
			}
			return true;
		}

		public void PlayHonorAni(int honorId, int honorLevel)
		{
			if (this.m_honor == null || this.m_honorAni == null)
			{
				return;
			}
			if (this.m_honor.layer != LayerMask.NameToLayer("3DUI"))
			{
				MMGame_Math.SetLayer(this.m_honor, "3DUI", false);
			}
			if (!this.m_honorAni.enabled)
			{
				this.m_honorAni.enabled = true;
			}
			if (!this.m_honorAni.isPlaying)
			{
				this.m_honorAni.Play();
			}
		}

		public void EndHonorAni()
		{
			if (this.m_honor == null || this.m_honorAni == null)
			{
				return;
			}
			if (this.m_honorAni.isPlaying)
			{
				this.m_honorAni.Stop();
			}
			if (this.m_honorAni.enabled)
			{
				this.m_honorAni.enabled = false;
			}
			if (this.m_honor.layer != LayerMask.NameToLayer("Hide"))
			{
				MMGame_Math.SetLayer(this.m_honor, "Hide", false);
			}
		}

		private bool IsNormalJungle()
		{
			return this.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster && this.actor.ActorControl.GetActorSubType() == 2 && this.actor.ActorControl.GetActorSubSoliderType() != 7 && this.actor.ActorControl.GetActorSubSoliderType() != 8 && this.actor.ActorControl.GetActorSubSoliderType() != 9;
		}

		private void AddDirtyFlag(HudComponent3D.enDirtyFlag dirtyFlag)
		{
			this.m_dirtyFlags |= (int)dirtyFlag;
		}

		private void RemoveDirtyFlag(HudComponent3D.enDirtyFlag dirtyFlag)
		{
			this.m_dirtyFlags &= (int)(~(int)dirtyFlag);
		}

		private bool HasDirtyFlag(HudComponent3D.enDirtyFlag dirtyFlag)
		{
			return (this.m_dirtyFlags & (int)dirtyFlag) != 0;
		}

		public void AddForceUpdateFlag()
		{
			this.AddDirtyFlag(HudComponent3D.enDirtyFlag.ForcePositionInMap);
		}
	}
}
