using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCPKG_GET_GUILD_MEMBER_GAME_STATE_RSP : ProtocolObject
	{
		public int iMemberCnt;

		public COMDT_GUILD_MEMBER_GAME_STATE[] astMemberInfo;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 1408;

		public SCPKG_GET_GUILD_MEMBER_GAME_STATE_RSP()
		{
			this.astMemberInfo = new COMDT_GUILD_MEMBER_GAME_STATE[150];
			for (int i = 0; i < 150; i++)
			{
				this.astMemberInfo[i] = (COMDT_GUILD_MEMBER_GAME_STATE)ProtocolObjectPool.Get(COMDT_GUILD_MEMBER_GAME_STATE.CLASS_ID);
			}
		}

		public override TdrError.ErrorType construct()
		{
			return TdrError.ErrorType.TDR_NO_ERROR;
		}

		public TdrError.ErrorType pack(ref byte[] buffer, int size, ref int usedSize, uint cutVer)
		{
			if (buffer == null || buffer.GetLength(0) == 0 || size > buffer.GetLength(0))
			{
				return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
			}
			TdrWriteBuf tdrWriteBuf = ClassObjPool<TdrWriteBuf>.Get();
			tdrWriteBuf.set(ref buffer, size);
			TdrError.ErrorType errorType = this.pack(ref tdrWriteBuf, cutVer);
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				buffer = tdrWriteBuf.getBeginPtr();
				usedSize = tdrWriteBuf.getUsedSize();
			}
			tdrWriteBuf.Release();
			return errorType;
		}

		public override TdrError.ErrorType pack(ref TdrWriteBuf destBuf, uint cutVer)
		{
			if (cutVer == 0u || SCPKG_GET_GUILD_MEMBER_GAME_STATE_RSP.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_GET_GUILD_MEMBER_GAME_STATE_RSP.CURRVERSION;
			}
			if (SCPKG_GET_GUILD_MEMBER_GAME_STATE_RSP.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeInt32(this.iMemberCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (0 > this.iMemberCnt)
			{
				return TdrError.ErrorType.TDR_ERR_MINUS_REFER_VALUE;
			}
			if (150 < this.iMemberCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astMemberInfo.Length < this.iMemberCnt)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < this.iMemberCnt; i++)
			{
				errorType = this.astMemberInfo[i].pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public TdrError.ErrorType unpack(ref byte[] buffer, int size, ref int usedSize, uint cutVer)
		{
			if (buffer == null || buffer.GetLength(0) == 0 || size > buffer.GetLength(0))
			{
				return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
			}
			TdrReadBuf tdrReadBuf = ClassObjPool<TdrReadBuf>.Get();
			tdrReadBuf.set(ref buffer, size);
			TdrError.ErrorType result = this.unpack(ref tdrReadBuf, cutVer);
			usedSize = tdrReadBuf.getUsedSize();
			tdrReadBuf.Release();
			return result;
		}

		public override TdrError.ErrorType unpack(ref TdrReadBuf srcBuf, uint cutVer)
		{
			if (cutVer == 0u || SCPKG_GET_GUILD_MEMBER_GAME_STATE_RSP.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_GET_GUILD_MEMBER_GAME_STATE_RSP.CURRVERSION;
			}
			if (SCPKG_GET_GUILD_MEMBER_GAME_STATE_RSP.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readInt32(ref this.iMemberCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (0 > this.iMemberCnt)
			{
				return TdrError.ErrorType.TDR_ERR_MINUS_REFER_VALUE;
			}
			if (150 < this.iMemberCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < this.iMemberCnt; i++)
			{
				errorType = this.astMemberInfo[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return SCPKG_GET_GUILD_MEMBER_GAME_STATE_RSP.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.iMemberCnt = 0;
			if (this.astMemberInfo != null)
			{
				for (int i = 0; i < this.astMemberInfo.Length; i++)
				{
					if (this.astMemberInfo[i] != null)
					{
						this.astMemberInfo[i].Release();
						this.astMemberInfo[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astMemberInfo != null)
			{
				for (int i = 0; i < this.astMemberInfo.Length; i++)
				{
					this.astMemberInfo[i] = (COMDT_GUILD_MEMBER_GAME_STATE)ProtocolObjectPool.Get(COMDT_GUILD_MEMBER_GAME_STATE.CLASS_ID);
				}
			}
		}
	}
}
