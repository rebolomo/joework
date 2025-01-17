using System;

namespace CSProtocol
{
	public enum COM_TRANSACTION_TCAPLUST_EVNET_TYPE
	{
		COM_TRANSACTION_TCAP_INSERTRANKDETAIL = 1,
		COM_TRANSACTION_TCAP_DELETERANKDETAIL,
		COM_TRANSACTION_TCAP_UPDATERANKCLASSID,
		COM_TRANSACTION_TCAP_GETONLINEINFO,
		COM_TRANSACTION_TCAP_INSERTONLINEINFO,
		COM_TRANSACTION_TCAP_UPDONLINEINFO,
		COM_TRANSACTION_TCAP_INSERTGUILDMEMBER,
		COM_TRANSACTION_TCAP_UPDACNTGUILDINFO,
		COM_TRANSACTION_TCAP_GETACNTINFO,
		COM_TRANSACTION_TCAP_DELJOINGUILDREQ,
		COM_TRANSACTION_TCAP_DELGUILDMEMBER,
		COM_TRANSACTION_TCAP_GETGUILDINVITE,
		COM_TRANSACTION_TCAP_UPDGUILDINVITETIME,
		COM_TRANSACTION_TCAP_INSERTGUILDINVITE,
		COM_TRANSACTION_TCAP_GETGUILDID,
		COM_TRANSACTION_TCAP_GETGUILDMAIN,
		COM_TRANSACTION_TCAP_DELGUILDINVITE,
		COM_TRANSACTION_TCAP_INSERTGUILDRECOMMEND,
		COM_TRANSACTION_TCAP_GETGLOBALREWARD,
		COM_TRANSACTION_TCAP_GETARENA_TARGETHERO,
		COM_TRANSACTION_TCAP_GETARENA_TARGETITEM,
		COM_TRANSACTION_TCAP_ADDARENA_FIGHTHISTORY,
		COM_TRANSACTION_TCAP_DELETE_BURNINGENEMY,
		COM_TRANSACTION_TCAP_INSERT_BURNINGENEMY,
		COM_TRANSACTION_TCAP_UPDVISITORSVRTRANSFLAG,
		COM_TRANSACTION_TCAP_GETACNTINFOFROMVISITORSVR,
		COM_TRANSACTION_TCAP_GETHEROINFOFROMVISITORSVR,
		COM_TRANSACTION_TCAP_GETITEMINFOFROMVISITORSVR,
		COM_TRANSACTION_TCAP_INSERTREGISTERINFO,
		COM_TRANSACTION_TCAP_INSERTACNTINFO,
		COM_TRANSACTION_TCAP_INSERTHEROINFO,
		COM_TRANSACTION_TCAP_INSERTITEMINFO,
		COM_TRANSACTION_TCAP_ADDRANKCURSEASONRECORD,
		COM_TRANSACTION_TCAP_ADDRANKPASTSEASONRECORD,
		COM_TRANSACTION_TCAP_INSERTREGISTER,
		COM_TRANSACTION_TCAP_UPDACNTNAME,
		COM_TRANSACTION_TCAP_DELETEREGISTER,
		COM_TRANSACTION_TCAP_UPDGUILDNAME,
		COM_TRANSACTION_TCAP_ADDFIGHTHISTORYRECORD,
		COM_TRANSACTION_TCAP_GETDESTACNTDATA,
		COM_TRANSACTION_TCAP_GET_MASTERREQNUM,
		COM_TRANSACTION_TCAP_INSERT_MASTERREQ,
		COM_TRANSACTION_TCAP_DEL_MASTERREQ,
		COM_TRANSACTION_TCAP_INSERT_TBMASTER,
		COM_TRANSACTION_TCAP_INSERT_TBSTUDENT,
		COM_TRANSACTION_TCAP_GET_ACNTPROFILE,
		COM_TRANSACTION_TCAP_DEL_TBMASTER,
		COM_TRANSACTION_TCAP_DEL_TBSTUDENT,
		COM_TRANSACTION_TCAP_UPD_TBMASTER,
		COM_TRANSACTION_TCAP_INSERT_TBGRADUATESTUDENT,
		COM_TRANSACTION_TCAP_GET_STUDENTNUM,
		COM_TRANSACTION_TCAP_GET_PROCESS_STUDENT_UNIQ,
		COM_TRANSACTION_TCAP_GET_GRADUATE_STUDENT_UNIQ,
		COM_TRANSACTION_TCAP_BATCHGET_ACNTPROFILE,
		COM_TRANSACTION_TCAP_DEL_TBGRADUATESTUDENT,
		COM_TRANSACTION_TCAP_GET_TBMASTER
	}
}
