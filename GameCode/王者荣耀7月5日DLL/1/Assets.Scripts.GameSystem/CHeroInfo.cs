using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameSystem
{
	public class CHeroInfo
	{
		public uint m_Proficiency;

		public byte m_ProficiencyLV;

		public ResHeroCfgInfo cfgInfo;

		public ResHeroShop shopCfgInfo;

		public PropertyHelper mActorValue = new PropertyHelper();

		public CSkillData skillInfo;

		public int m_selectPageIndex;

		public uint MaskBits;

		public CSkinInfo m_skinInfo = new CSkinInfo();

		public byte[] m_talentBuyList;

		public int m_awakeState;

		public uint m_awakeStepID;

		public bool m_isStepFinish;

		public uint m_experienceDeadLine;

		public uint m_masterHeroFightCnt;

		public uint m_masterHeroWinCnt;

		private static int m_maxProficiency;

		public void Init(ulong playerId, COMDT_HEROINFO svrInfo)
		{
			this.cfgInfo = GameDataMgr.heroDatabin.GetDataByKey(svrInfo.stCommonInfo.dwHeroID);
			GameDataMgr.heroShopInfoDict.TryGetValue(svrInfo.stCommonInfo.dwHeroID, ref this.shopCfgInfo);
			this.m_selectPageIndex = (int)svrInfo.stCommonInfo.bSymbolPageWear;
			if (this.mActorValue == null)
			{
				this.mActorValue = new PropertyHelper();
			}
			this.mActorValue.Init(svrInfo);
			if (this.skillInfo == null)
			{
				this.skillInfo = new CSkillData();
			}
			this.skillInfo.InitSkillData(this.cfgInfo, svrInfo.stCommonInfo.stSkill);
			this.m_Proficiency = svrInfo.stCommonInfo.stProficiency.dwProficiency;
			this.m_ProficiencyLV = svrInfo.stCommonInfo.stProficiency.bLv;
			this.MaskBits = svrInfo.stCommonInfo.dwMaskBits;
			this.m_skinInfo.Init(svrInfo.stCommonInfo.wSkinID);
			this.m_talentBuyList = new byte[svrInfo.stCommonInfo.stTalent.astTalentInfo.Length];
			for (int i = 0; i < svrInfo.stCommonInfo.stTalent.astTalentInfo.Length; i++)
			{
				this.m_talentBuyList[i] = svrInfo.stCommonInfo.stTalent.astTalentInfo[i].bIsBuyed;
			}
			this.m_awakeState = (int)svrInfo.stCommonInfo.stTalent.bWakeState;
			this.m_awakeStepID = svrInfo.stCommonInfo.stTalent.stWakeStep.dwStepID;
			this.m_isStepFinish = (svrInfo.stCommonInfo.stTalent.stWakeStep.bIsFinish == 1);
			this.m_experienceDeadLine = ((!this.IsExperienceHero()) ? 0u : svrInfo.stCommonInfo.dwDeadLine);
			this.m_masterHeroFightCnt = svrInfo.stCommonInfo.dwMasterTotalFightCnt;
			this.m_masterHeroWinCnt = svrInfo.stCommonInfo.dwMasterTotalWinCnt;
		}

		public int GetCombatEft()
		{
			DebugHelper.Assert(this.mActorValue != null, "GetCombatEft mActorValue is null");
			int num = 0;
			if (this.mActorValue != null)
			{
				num = CHeroInfo.GetCombatEftByStarLevel(this.mActorValue.actorLvl, this.mActorValue.actorStar);
			}
			int combatEft = CSkinInfo.GetCombatEft(this.cfgInfo.dwCfgID, this.m_skinInfo.GetWearSkinId());
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			DebugHelper.Assert(masterRoleInfo != null, "GetCombatEft master is null");
			int num2 = 0;
			if (masterRoleInfo != null)
			{
				num2 = masterRoleInfo.m_symbolInfo.GetSymbolPageEft(this.m_selectPageIndex);
			}
			return num + combatEft + num2;
		}

		public static int GetCombatEftByStarLevel(int level, int star)
		{
			ResHeroLvlUpInfo dataByKey = GameDataMgr.heroLvlUpDatabin.GetDataByKey((uint)level);
			if (dataByKey != null && star >= 1 && star <= 5)
			{
				return dataByKey.StarCombatEft[star - 1];
			}
			return 0;
		}

		public static int GetInitCombatByHeroId(uint id)
		{
			ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(id);
			if (dataByKey == null)
			{
				return 0;
			}
			int combatEftByStarLevel = CHeroInfo.GetCombatEftByStarLevel(1, dataByKey.iInitialStar);
			int combatEft = CSkinInfo.GetCombatEft(id, 0u);
			return combatEftByStarLevel + combatEft;
		}

		public void OnHeroInfoUpdate(SCPKG_NTF_HERO_INFO_UPD svrHeroInfoUp)
		{
			for (int i = 0; i < svrHeroInfoUp.iHeroUpdNum; i++)
			{
				CS_HEROINFO_UPD_TYPE bUpdType = svrHeroInfoUp.astHeroUpdInfo[i].bUpdType;
				int num = svrHeroInfoUp.astHeroUpdInfo[i].stValueParam.Value[0];
				switch (bUpdType)
				{
				case 1:
					this.mActorValue.actorLvl = num;
					break;
				case 2:
					this.mActorValue.actorExp = num;
					break;
				case 3:
					this.mActorValue.actorStar = num;
					break;
				case 4:
					this.mActorValue.actorQuality = num;
					break;
				case 5:
					this.mActorValue.actorSubQuality = num;
					break;
				case 6:
					this.skillInfo.UnLockSkill(num);
					break;
				case 7:
					this.m_ProficiencyLV = (byte)num;
					this.m_Proficiency = (uint)svrHeroInfoUp.astHeroUpdInfo[i].stValueParam.Value[1];
					break;
				case 8:
				{
					uint maskBits = this.MaskBits;
					this.MaskBits = (uint)num;
					if ((maskBits & 2u) == 0u && (this.MaskBits & 2u) > 0u)
					{
						Singleton<EventRouter>.get_instance().BroadCastEvent<string>("HeroUnlockPvP", StringHelper.UTF8BytesToString(ref this.cfgInfo.szName));
					}
					break;
				}
				case 9:
				{
					string text = StringHelper.UTF8BytesToString(ref this.cfgInfo.szName);
					Singleton<EventRouter>.get_instance().BroadCastEvent<string, uint, uint>("HeroExperienceTimeUpdate", text, this.m_experienceDeadLine, (uint)num);
					this.m_experienceDeadLine = (uint)num;
					break;
				}
				case 10:
					this.m_masterHeroWinCnt = (uint)num;
					this.m_masterHeroFightCnt = (uint)svrHeroInfoUp.astHeroUpdInfo[i].stValueParam.Value[1];
					break;
				}
			}
		}

		public void OnHeroSkinWear(uint skinId)
		{
			uint wearSkinId = this.m_skinInfo.GetWearSkinId();
			this.mActorValue.SetSkinProp(this.cfgInfo.dwCfgID, wearSkinId, false);
			this.m_skinInfo.SetWearSkinId(skinId);
			this.mActorValue.SetSkinProp(this.cfgInfo.dwCfgID, skinId, true);
		}

		public static string GetHeroJob(uint heroId)
		{
			string text = string.Empty;
			ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(heroId);
			CTextManager instance = Singleton<CTextManager>.GetInstance();
			if (dataByKey != null)
			{
				if (dataByKey.bMainJob > 0)
				{
					text += CHeroInfo.GetHeroJobStr(dataByKey.bMainJob);
				}
				if (dataByKey.bMinorJob > 0)
				{
					text = string.Format("{0}/{1}", text, CHeroInfo.GetHeroJobStr(dataByKey.bMinorJob));
				}
			}
			return text;
		}

		public static string GetJobFeature(uint heroId)
		{
			string text = string.Empty;
			ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(heroId);
			CTextManager instance = Singleton<CTextManager>.GetInstance();
			if (dataByKey != null)
			{
				string text2 = string.Empty;
				for (int i = 0; i < dataByKey.JobFeature.Length; i++)
				{
					text2 = CHeroInfo.GetFeatureStr(dataByKey.JobFeature[i]);
					if (!string.IsNullOrEmpty(text2))
					{
						if (!string.IsNullOrEmpty(text))
						{
							text = string.Format("{0}/{1}", text, text2);
						}
						else
						{
							text = text2;
						}
					}
				}
			}
			return text;
		}

		public static string GetHeroJobStr(RES_HERO_JOB jobType)
		{
			string result = string.Empty;
			CTextManager instance = Singleton<CTextManager>.GetInstance();
			switch (jobType)
			{
			case 1:
				result = instance.GetText("Hero_Job_Tank");
				break;
			case 2:
				result = instance.GetText("Hero_Job_Soldier");
				break;
			case 3:
				result = instance.GetText("Hero_Job_Assassin");
				break;
			case 4:
				result = instance.GetText("Hero_Job_Master");
				break;
			case 5:
				result = instance.GetText("Hero_Job_Archer");
				break;
			case 6:
				result = instance.GetText("Hero_Job_Aid");
				break;
			}
			return result;
		}

		public static string GetFeatureStr(RES_HERO_JOB_FEATURE featureType)
		{
			string result = string.Empty;
			CTextManager instance = Singleton<CTextManager>.GetInstance();
			switch (featureType)
			{
			case 1:
				result = instance.GetText("Hero_Job_Feature_Dash");
				break;
			case 2:
				result = instance.GetText("Hero_Job_Feature_Control");
				break;
			case 3:
				result = instance.GetText("Hero_Job_Feature_Active");
				break;
			case 4:
				result = instance.GetText("Hero_Job_Feature_Slave");
				break;
			case 5:
				result = instance.GetText("Hero_Job_Feature_Recover");
				break;
			case 6:
				result = instance.GetText("Hero_Job_Feature_HpSteal");
				break;
			case 7:
				result = instance.GetText("Hero_Job_Feature_Poke");
				break;
			case 8:
				result = instance.GetText("Hero_Job_Feature_Buff");
				break;
			}
			return result;
		}

		public static string GetHeroDesc(uint heroId)
		{
			ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(heroId);
			return Utility.UTF8Convert(dataByKey.szHeroDesc);
		}

		public static string GetHeroTitle(uint heroId, uint skinId)
		{
			string result = string.Empty;
			if (skinId == 0u)
			{
				ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(heroId);
				if (dataByKey != null)
				{
					result = StringHelper.UTF8BytesToString(ref dataByKey.szHeroTitle);
				}
			}
			else
			{
				ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(heroId, skinId);
				if (heroSkin != null)
				{
					result = StringHelper.UTF8BytesToString(ref heroSkin.szSkinName);
				}
			}
			return result;
		}

		public void OnSymbolPageChange(int pageIdx)
		{
			this.m_selectPageIndex = pageIdx;
			Singleton<EventRouter>.get_instance().BroadCastEvent<uint>("HeroSymbolPageChange", this.cfgInfo.dwCfgID);
		}

		public static ResHeroProficiency GetHeroProficiency(int job, int level)
		{
			HashSet<object> dataByKey = GameDataMgr.heroProficiencyDatabin.GetDataByKey((int)((byte)level));
			HashSet<object>.Enumerator enumerator = dataByKey.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ResHeroProficiency resHeroProficiency = (ResHeroProficiency)enumerator.get_Current();
				if ((int)resHeroProficiency.bJob == job)
				{
					return resHeroProficiency;
				}
			}
			return null;
		}

		public static int GetMaxProficiency()
		{
			if (CHeroInfo.m_maxProficiency > 0)
			{
				return CHeroInfo.m_maxProficiency;
			}
			for (int i = 0; i < GameDataMgr.heroProficiencyDatabin.Count(); i++)
			{
				HashSet<object> dataByIndex = GameDataMgr.heroProficiencyDatabin.GetDataByIndex(i);
				HashSet<object>.Enumerator enumerator = dataByIndex.GetEnumerator();
				while (enumerator.MoveNext())
				{
					CHeroInfo.m_maxProficiency = Math.Max(CHeroInfo.m_maxProficiency, (int)((ResHeroProficiency)enumerator.get_Current()).bLevel);
				}
			}
			return CHeroInfo.m_maxProficiency;
		}

		public bool IsExperienceHero()
		{
			return (this.MaskBits & 8u) != 0u;
		}

		public bool IsValidExperienceHero()
		{
			bool flag = (long)CRoleInfo.GetCurrentUTCTime() < (long)((ulong)this.m_experienceDeadLine);
			return this.IsExperienceHero() && flag;
		}

		public static int GetExperienceHeroOrSkinValidDays(uint experienceDeadLine)
		{
			int num = (int)(experienceDeadLine - (uint)CRoleInfo.GetCurrentUTCTime());
			TimeSpan timeSpan = new TimeSpan((long)(num + 3600) * 10000000L);
			return timeSpan.get_Days();
		}

		public static int GetExperienceHeroOrSkinExtendDays(uint extendSeconds)
		{
			TimeSpan timeSpan = new TimeSpan((long)((ulong)(extendSeconds + 3600u) * 10000000uL));
			return timeSpan.get_Days();
		}

		public static uint GetHeroCost(uint heroId, RES_SHOPBUY_COINTYPE costType)
		{
			ResHeroShop resHeroShop = null;
			GameDataMgr.heroShopInfoDict.TryGetValue(heroId, ref resHeroShop);
			uint result = 0u;
			if (resHeroShop != null)
			{
				switch (costType)
				{
				case 2:
					result = resHeroShop.dwBuyCoupons;
					break;
				case 4:
					result = resHeroShop.dwBuyCoin;
					break;
				case 5:
					result = resHeroShop.dwBuyBurnCoin;
					break;
				case 6:
					result = resHeroShop.dwBuyArenaCoin;
					break;
				case 10:
					result = resHeroShop.dwBuyDiamond;
					break;
				}
			}
			return result;
		}

		public static string GetHeroName(uint heroId)
		{
			ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(heroId);
			if (dataByKey != null)
			{
				return dataByKey.szName;
			}
			return string.Empty;
		}
	}
}
