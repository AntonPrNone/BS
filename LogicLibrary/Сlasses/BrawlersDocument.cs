using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LogicLibrary
{
    public enum RarityPriority
    {
        Начальный = 1,
        Редкий = 2,
        Сверхредкий = 3,
        Эпический = 4,
        Мифический = 5,
        Легендарный = 6,
        Хроматический = 7
    }

    [BsonIgnoreExtraElements]
    public class BrawlersDocument
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("photo")]
        public string Photo { get; set; }

        [BsonElement("photoIco")]
        public string PhotoIco { get; set; }

        [BsonElement("category")]
        public string Category { get; set; }

        [BsonElement("class")]
        public string Class { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("rarity")]
        public string Rarity { get; set; }

        [BsonElement("stats")]
        public Stats Stats { get; set; }

        [BsonElement("uploadDate")]
        public DateTime UploadDate { get; set; }

        [BsonIgnore]
        public string UploadDateString => UploadDate.ToString("dd.MM.yy HH:mm");

        public RarityPriority RarityPriority
        {
            get
            {
                switch (Rarity)
                {
                    case "Начальный":
                        return RarityPriority.Начальный;
                    case "Редкий":
                        return RarityPriority.Редкий;
                    case "Сверхредкий":
                        return RarityPriority.Сверхредкий;
                    case "Эпический":
                        return RarityPriority.Эпический;
                    case "Мифический":
                        return RarityPriority.Мифический;
                    case "Легендарный":
                        return RarityPriority.Легендарный;
                    case "Хроматический":
                        return RarityPriority.Хроматический;
                    default:
                        throw new Exception("Invalid rarity");
                }
            }
        }
    }

    public class Stats
    {
        [BsonElement("health")]
        public int Health { get; set; }

        [BsonElement("speed")]
        public string Speed { get; set; }

        [BsonElement("attackName")]
        public string AttackName { get; set; }

        [BsonElement("attackDescription")]
        public string AttackDescription { get; set; }

        [BsonElement("attackValue")]
        public string AttackValue { get; set; }

        [BsonElement("attackDistance")]
        public string AttackDistance { get; set; }

        [BsonElement("attackSpeed")]
        public string AttackSpeed { get; set; }

        [BsonElement("attackAdditionalItemName")]
        public string AttackAdditionalItemName { get; set; }

        [BsonElement("attackAdditionalItemValue")]
        public string AttackAdditionalItemValue { get; set; }


        [BsonElement("superAttackName")]
        public string SuperAttackName { get; set; }

        [BsonElement("superAttackDescription")]
        public string SuperAttackDescription { get; set; }

        [BsonElement("superAttackValue")]
        public string SuperAttackValue { get; set; }

        [BsonElement("superAttackDistance")]
        public string SuperAttackDistance { get; set; }

        [BsonElement("superAttackAdditionalItemName")]
        public string SuperAttackAdditionalItemName { get; set; }

        [BsonElement("superAttackAdditionalItemValue")]
        public string SuperAttackAdditionalItemValue { get; set; }


        [BsonElement("companionHealth")]
        public string CompanionHealth { get; set; }

        [BsonElement("companionAttack")]
        public string CompanionAttack { get; set; }

        [BsonElement("сompanionSpeed")]
        public string CompanionSpeed { get; set; }

        [BsonElement("companionDistance")]
        public string CompanionDistance { get; set; }


        [BsonElement("feature")]
        public string Feature { get; set; }
    }
}
