using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.GameKernal;
using CSProtocol;
using System;

namespace Assets.Scripts.GameLogic
{
	[FrameCommandClass]
	public struct LockAttackTargetCommand : ICommandImplement
	{
		public uint LockAttackTarget;

		[FrameCommandCreator]
		public static IFrameCommand Creator(ref FRAME_CMD_PKG msg)
		{
			FrameCommand<LockAttackTargetCommand> frameCommand = FrameCommandFactory.CreateFrameCommand<LockAttackTargetCommand>();
			frameCommand.cmdData.LockAttackTarget = msg.stCmdInfo.get_stCmdPlayerLockAttackTarget().dwLockAttackTarget;
			return frameCommand;
		}

		public bool TransProtocol(FRAME_CMD_PKG msg)
		{
			msg.stCmdInfo.get_stCmdPlayerLockAttackTarget().dwLockAttackTarget = this.LockAttackTarget;
			return true;
		}

		public bool TransProtocol(CSDT_GAMING_CSSYNCINFO msg)
		{
			return true;
		}

		public void OnReceive(IFrameCommand cmd)
		{
		}

		public void Preprocess(IFrameCommand cmd)
		{
		}

		public void ExecCommand(IFrameCommand cmd)
		{
			Player player = Singleton<GamePlayerCenter>.GetInstance().GetPlayer(cmd.playerID);
			if (player != null && player.Captain)
			{
				player.Captain.get_handle().ActorControl.SetLockTargetID(this.LockAttackTarget);
			}
		}

		public void AwakeCommand(IFrameCommand cmd)
		{
		}
	}
}
