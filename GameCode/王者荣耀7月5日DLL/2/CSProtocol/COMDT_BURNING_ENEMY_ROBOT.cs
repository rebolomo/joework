using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_BURNING_ENEMY_ROBOT : ProtocolObject
	{
		public COMDT_PLAYERINFO stEnemyDetail;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 160u;

		public static readonly int CLASS_ID = 328;

		public COMDT_BURNING_ENEMY_ROBOT()
		{
			this.stEnemyDetail = (COMDT_PLAYERINFO)ProtocolObjectPool.Get(COMDT_PLAYERINFO.CLASS_ID);
		}

		public override TdrError.ErrorType construct()
		{
			TdrError.ErrorType errorType = this.stEnemyDetail.construct();
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
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
			if (cutVer == 0u || COMDT_BURNING_ENEMY_ROBOT.CURRVERSION < cutVer)
			{
				cutVer = COMDT_BURNING_ENEMY_ROBOT.CURRVERSION;
			}
			if (COMDT_BURNING_ENEMY_ROBOT.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stEnemyDetail.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
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
			if (cutVer == 0u || COMDT_BURNING_ENEMY_ROBOT.CURRVERSION < cutVer)
			{
				cutVer = COMDT_BURNING_ENEMY_ROBOT.CURRVERSION;
			}
			if (COMDT_BURNING_ENEMY_ROBOT.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stEnemyDetail.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_BURNING_ENEMY_ROBOT.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.stEnemyDetail != null)
			{
				this.stEnemyDetail.Release();
				this.stEnemyDetail = null;
			}
		}

		public override void OnUse()
		{
			this.stEnemyDetail = (COMDT_PLAYERINFO)ProtocolObjectPool.Get(COMDT_PLAYERINFO.CLASS_ID);
		}
	}
}
