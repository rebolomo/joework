using CSProtocol;
using System;

[CheatCommand("工具/ProfitLimit", "重置收益上限", 82)]
internal class ProfitLimitCommand : CheatCommandNetworking
{
	protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
	{
		CheatCmdRef.set_stClrProfitLimit(new CSDT_CHEAT_COMVAL());
		CheatCmdRef.get_stClrProfitLimit().iValue = 1;
		return CheatCommandBase.Done;
	}
}
