using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.GameKernal;
using CSProtocol;
using System;

namespace Assets.Scripts.GameLogic
{
	[FrameCSSYNCCommandClass]
	public struct UseDirectionalSkillCommand : ICommandImplement
	{
		public short Degree;

		public SkillSlotType SlotType;

		public uint dwObjectID;

		[FrameCommandCreator]
		public static IFrameCommand Creator(ref CSDT_GAMING_CSSYNCINFO msg)
		{
			FrameCommand<UseDirectionalSkillCommand> frameCommand = FrameCommandFactory.CreateCSSyncFrameCommand<UseDirectionalSkillCommand>();
			frameCommand.cmdData.Degree = msg.stCSSyncDt.get_stDirectionSkill().nDegree;
			frameCommand.cmdData.SlotType = (SkillSlotType)msg.stCSSyncDt.get_stDirectionSkill().chSlotType;
			frameCommand.cmdData.dwObjectID = msg.stCSSyncDt.get_stDirectionSkill().dwObjectID;
			return frameCommand;
		}

		public bool TransProtocol(FRAME_CMD_PKG msg)
		{
			return true;
		}

		public bool TransProtocol(CSDT_GAMING_CSSYNCINFO msg)
		{
			msg.stCSSyncDt.get_stDirectionSkill().nDegree = this.Degree;
			msg.stCSSyncDt.get_stDirectionSkill().chSlotType = (sbyte)this.SlotType;
			msg.stCSSyncDt.get_stDirectionSkill().dwObjectID = this.dwObjectID;
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
				SkillUseParam skillUseParam = default(SkillUseParam);
				VInt3 inVec = VInt3.right;
				inVec = inVec.RotateY((int)this.Degree);
				skillUseParam.Init(this.SlotType, inVec, false, this.dwObjectID);
				player.Captain.get_handle().ActorControl.CmdUseSkill(cmd, ref skillUseParam);
			}
		}

		public void AwakeCommand(IFrameCommand cmd)
		{
		}
	}
}
