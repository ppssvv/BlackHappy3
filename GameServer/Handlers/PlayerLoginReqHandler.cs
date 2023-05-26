﻿using Common;
using Common.Database;
using Common.Resources.Proto;
using Newtonsoft.Json;

namespace PemukulPaku.GameServer.Handlers
{
    [PacketCmdId(CmdId.PlayerLoginReq)]
    internal class PlayerLoginReqHandler : IPacketHandler
    {
        public void Handle(Session session, Packet packet)
        {
            User.UserScheme User = session.Player.User;

            PlayerLoginRsp Rsp = new()
            {
                retcode = PlayerLoginRsp.Retcode.Succ,
                IsFirstLogin = User.IsFirstLogin,
                RegionName = Global.config.Gameserver.RegionName,
                CgType = User.IsFirstLogin ? CGType.CgStart : CGType.CgSevenChapter,
                RegionId = 248,
                LoginSessionToken = 1,
                PsychoKey = 0
            };

            session.Send(Packet.FromProto(Rsp, CmdId.PlayerLoginRsp), Packet.FromProto(new GetMpDataRsp()
            {
                retcode = GetMpDataRsp.Retcode.Succ,
                DataType = MpDataType.MpDataPunishTime,
                op_type = GetMpDataRsp.OpType.UpdateData,
                PunishEndTime = 0
            }, CmdId.GetMpDataRsp));
        }
    }
}
