﻿using Common.Resources.Proto;
using Common.Utils.ExcelReader;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Common.Database
{
    public class Avatar
    {
        public static readonly IMongoCollection<AvatarScheme> collection = Global.db.GetCollection<AvatarScheme>("Avatars");

        public static AvatarScheme[] AvatarsFromUid(uint uid)
        {
            return collection.AsQueryable().Where(collection=> collection.OwnerUid == uid).ToArray();
        }

        public static AvatarScheme Create(int avatarId, uint uid, EquipmentScheme equipment)
        {
            AvatarScheme? tryAvatar = collection.AsQueryable().Where(collection => collection.AvatarId == avatarId && collection.OwnerUid == uid).FirstOrDefault();
            if (tryAvatar != null) { return tryAvatar; }

            AvatarDataExcel? avatarData = AvatarData.GetInstance().FromId(avatarId);
            if(avatarData == null) { throw new ArgumentException("Invalid avatarId"); }

            Weapon weapon = equipment.AddWeapon(avatarData.InitialWeapon);

            AvatarScheme avatar = new()
            {
                OwnerUid = uid,
                AvatarId = (uint)avatarData.AvatarId,
                DressId = (uint)avatarData.DefaultDressId,
                DressLists = new[] { (uint)avatarData.DefaultDressId },
                Exp = 0,
                Fragment = 0,
                Level = 1,
                Star = (uint)avatarData.UnlockStar,
                StigmataUniqueId1 = 0,
                StigmataUniqueId2 = 0,
                StigmataUniqueId3 = 0,
                SubStar = 0,
                TouchGoodfeel = 0,
                TodayHasAddGoodfeel = 0,
                WeaponUniqueId = weapon.UniqueId
            };

            if(avatarData.AvatarId == 101)
            {
                Stigmata defaultStigmata1 = equipment.AddStigmata(30007);
                Stigmata defaultStigmata2 = equipment.AddStigmata(30060);
                Stigmata defaultStigmata3 = equipment.AddStigmata(30113);

                avatar.StigmataUniqueId1 = defaultStigmata1.UniqueId;
                avatar.StigmataUniqueId2 = defaultStigmata2.UniqueId;
                avatar.StigmataUniqueId3 = defaultStigmata3.UniqueId;
            }

            avatar.SkillLists.AddRange(avatarData.SkillList.Select(skillId => new AvatarSkill { SkillId = (uint)skillId }));

            collection.InsertOne(avatar);

            return avatar;
        }

    }
    public class AvatarScheme : Resources.Proto.Avatar
    {
        public ObjectId Id { get; set; }
        public uint OwnerUid { get; set; }

        public void Save()
        {
            Avatar.collection.ReplaceOne(Builders<AvatarScheme>.Filter.Eq(avatar => avatar.Id, Id), this);
        }
    }
}