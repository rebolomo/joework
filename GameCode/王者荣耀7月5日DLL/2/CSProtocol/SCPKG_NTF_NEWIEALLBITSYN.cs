using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCPKG_NTF_NEWIEALLBITSYN : ProtocolObject
	{
		public COMDT_NEWBIE_STATUS_BITS stNewbieBits;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 661;

		public SCPKG_NTF_NEWIEALLBITSYN()
		{
			this.stNewbieBits = (COMDT_NEWBIE_STATUS_BITS)ProtocolObjectPool.Get(COMDT_NEWBIE_STATUS_BITS.CLASS_ID);
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
			if (cutVer == 0u || SCPKG_NTF_NEWIEALLBITSYN.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_NTF_NEWIEALLBITSYN.CURRVERSION;
			}
			if (SCPKG_NTF_NEWIEALLBITSYN.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stNewbieBits.pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || SCPKG_NTF_NEWIEALLBITSYN.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_NTF_NEWIEALLBITSYN.CURRVERSION;
			}
			if (SCPKG_NTF_NEWIEALLBITSYN.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stNewbieBits.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return SCPKG_NTF_NEWIEALLBITSYN.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.stNewbieBits != null)
			{
				this.stNewbieBits.Release();
				this.stNewbieBits = null;
			}
		}

		public override void OnUse()
		{
			this.stNewbieBits = (COMDT_NEWBIE_STATUS_BITS)ProtocolObjectPool.Get(COMDT_NEWBIE_STATUS_BITS.CLASS_ID);
		}
	}
}
