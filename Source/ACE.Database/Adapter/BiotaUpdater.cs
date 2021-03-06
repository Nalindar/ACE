using System;
using System.Linq;

using ACE.Database.Models.Shard;
using ACE.Entity.Enum;
using ACE.Entity.Enum.Properties;

namespace ACE.Database.Adapter
{
    public static class BiotaUpdater
    {
        public static void UpdateDatabaseBiota(ShardDbContext context, ACE.Entity.Models.Biota sourceBiota, ACE.Database.Models.Shard.Biota targetBiota)
        {
            targetBiota.WeenieClassId = sourceBiota.WeenieClassId;
            targetBiota.WeenieType = (int)sourceBiota.WeenieType;

            throw new NotImplementedException();
            /* Uncomment this when new biota model is used
            if (sourceBiota.PropertiesBool != null)
            {
                foreach (var kvp in sourceBiota.PropertiesBool)
                    targetBiota.SetProperty(kvp.Key, kvp.Value);
            }
            foreach (var value in targetBiota.BiotaPropertiesBool)
            {
                if (sourceBiota.PropertiesBool == null || !sourceBiota.PropertiesBool.ContainsKey((PropertyBool)value.Type))
                    context.BiotaPropertiesBool.Remove(value);
            }

            if (sourceBiota.PropertiesDID != null)
            {
                foreach (var kvp in sourceBiota.PropertiesDID)
                    targetBiota.SetProperty(kvp.Key, kvp.Value);
            }
            foreach (var value in targetBiota.BiotaPropertiesDID)
            {
                if (sourceBiota.PropertiesDID == null || !sourceBiota.PropertiesDID.ContainsKey((PropertyDataId)value.Type))
                    context.BiotaPropertiesDID.Remove(value);
            }

            if (sourceBiota.PropertiesFloat != null)
            {
                foreach (var kvp in sourceBiota.PropertiesFloat)
                    targetBiota.SetProperty(kvp.Key, kvp.Value);
            }
            foreach (var value in targetBiota.BiotaPropertiesFloat)
            {
                if (sourceBiota.PropertiesFloat == null || !sourceBiota.PropertiesFloat.ContainsKey((PropertyFloat)value.Type))
                    context.BiotaPropertiesFloat.Remove(value);
            }

            if (sourceBiota.PropertiesIID != null)
            {
                foreach (var kvp in sourceBiota.PropertiesIID)
                    targetBiota.SetProperty(kvp.Key, kvp.Value);
            }
            foreach (var value in targetBiota.BiotaPropertiesIID)
            {
                if (sourceBiota.PropertiesIID == null || !sourceBiota.PropertiesIID.ContainsKey((PropertyInstanceId)value.Type))
                    context.BiotaPropertiesIID.Remove(value);
            }

            if (sourceBiota.PropertiesInt != null)
            {
                foreach (var kvp in sourceBiota.PropertiesInt)
                    targetBiota.SetProperty(kvp.Key, kvp.Value);
            }
            foreach (var value in targetBiota.BiotaPropertiesInt)
            {
                if (sourceBiota.PropertiesInt == null || !sourceBiota.PropertiesInt.ContainsKey((PropertyInt)value.Type))
                    context.BiotaPropertiesInt.Remove(value);
            }

            if (sourceBiota.PropertiesInt64 != null)
            {
                foreach (var kvp in sourceBiota.PropertiesInt64)
                    targetBiota.SetProperty(kvp.Key, kvp.Value);
            }
            foreach (var value in targetBiota.BiotaPropertiesInt64)
            {
                if (sourceBiota.PropertiesInt64 == null || !sourceBiota.PropertiesInt64.ContainsKey((PropertyInt64)value.Type))
                    context.BiotaPropertiesInt64.Remove(value);
            }

            if (sourceBiota.PropertiesString != null)
            {
                foreach (var kvp in sourceBiota.PropertiesString)
                    targetBiota.SetProperty(kvp.Key, kvp.Value);
            }
            foreach (var value in targetBiota.BiotaPropertiesString)
            {
                if (sourceBiota.PropertiesString == null || !sourceBiota.PropertiesString.ContainsKey((PropertyString)value.Type))
                    context.BiotaPropertiesString.Remove(value);
            }*/


            if (sourceBiota.PropertiesPosition != null)
            {
                foreach (var kvp in sourceBiota.PropertiesPosition)
                {
                    BiotaPropertiesPosition existingValue = targetBiota.BiotaPropertiesPosition.FirstOrDefault(r => r.PositionType == (ushort)kvp.Key);

                    if (existingValue == null)
                    {
                        existingValue = new BiotaPropertiesPosition { ObjectId = sourceBiota.Id };

                        targetBiota.BiotaPropertiesPosition.Add(existingValue);
                    }

                    existingValue.PositionType = (ushort)kvp.Key;
                    existingValue.ObjCellId = kvp.Value.ObjCellId;
                    existingValue.OriginX = kvp.Value.PositionX;
                    existingValue.OriginY = kvp.Value.PositionY;
                    existingValue.OriginZ = kvp.Value.PositionZ;
                    existingValue.AnglesW = kvp.Value.RotationW;
                    existingValue.AnglesX = kvp.Value.RotationX;
                    existingValue.AnglesY = kvp.Value.RotationY;
                    existingValue.AnglesZ = kvp.Value.RotationZ;
                }
            }
            foreach (var value in targetBiota.BiotaPropertiesPosition)
            {
                if (sourceBiota.PropertiesPosition == null || !sourceBiota.PropertiesPosition.ContainsKey((PositionType)value.PositionType))
                    context.BiotaPropertiesPosition.Remove(value);
            }


            if (sourceBiota.PropertiesSpellBook != null)
            {
                foreach (var kvp in sourceBiota.PropertiesSpellBook)
                {
                    BiotaPropertiesSpellBook existingValue = targetBiota.BiotaPropertiesSpellBook.FirstOrDefault(r => r.Spell == (ushort)kvp.Key);

                    if (existingValue == null)
                    {
                        existingValue = new BiotaPropertiesSpellBook { ObjectId = sourceBiota.Id };

                        targetBiota.BiotaPropertiesSpellBook.Add(existingValue);
                    }

                    existingValue.Spell = kvp.Key;
                    existingValue.Probability = kvp.Value;
                }
            }
            foreach (var value in targetBiota.BiotaPropertiesSpellBook)
            {
                if (sourceBiota.PropertiesSpellBook == null || !sourceBiota.PropertiesSpellBook.ContainsKey(value.Spell))
                    context.BiotaPropertiesSpellBook.Remove(value);
            }


            if (sourceBiota.PropertiesAnimPart != null)
            {
                for (int i = 0; i < sourceBiota.PropertiesAnimPart.Count; i++)
                {
                    var value = sourceBiota.PropertiesAnimPart[i];

                    BiotaPropertiesAnimPart existingValue = targetBiota.BiotaPropertiesAnimPart.FirstOrDefault(r => r.Order == i);

                    if (existingValue == null)
                    {
                        existingValue = new BiotaPropertiesAnimPart { ObjectId = sourceBiota.Id };

                        targetBiota.BiotaPropertiesAnimPart.Add(existingValue);
                    }

                    existingValue.Index = value.Index;
                    existingValue.AnimationId = value.AnimationId;
                    existingValue.Order = (byte)i;
                }
            }
            foreach (var value in targetBiota.BiotaPropertiesAnimPart)
            {
                if (sourceBiota.PropertiesAnimPart == null || value.Order >= sourceBiota.PropertiesAnimPart.Count)
                    context.BiotaPropertiesAnimPart.Remove(value);
            }

            if (sourceBiota.PropertiesPalette != null)
            {
                foreach (var value in sourceBiota.PropertiesPalette)
                {
                    BiotaPropertiesPalette existingValue = targetBiota.BiotaPropertiesPalette.FirstOrDefault(r => r.SubPaletteId == value.SubPaletteId && r.Offset == value.Offset && r.Length == value.Length);

                    if (existingValue == null)
                    {
                        existingValue = new BiotaPropertiesPalette { ObjectId = sourceBiota.Id, SubPaletteId = value.SubPaletteId, Offset =value.Offset, Length = value.Length };

                        targetBiota.BiotaPropertiesPalette.Add(existingValue);
                    }
                }
            }
            foreach (var value in targetBiota.BiotaPropertiesPalette)
            {
                if (sourceBiota.PropertiesPalette == null || !sourceBiota.PropertiesPalette.Any(p => p.SubPaletteId == value.SubPaletteId && p.Offset == value.Offset && p.Length == value.Length))
                    context.BiotaPropertiesPalette.Remove(value);
            }

            if (sourceBiota.PropertiesTextureMap != null)
            {
                for (int i = 0; i < sourceBiota.PropertiesTextureMap.Count; i++)
                {
                    var value = sourceBiota.PropertiesTextureMap[i];

                    BiotaPropertiesTextureMap existingValue = targetBiota.BiotaPropertiesTextureMap.FirstOrDefault(r => r.Order == i);

                    if (existingValue == null)
                    {
                        existingValue = new BiotaPropertiesTextureMap { ObjectId = sourceBiota.Id };

                        targetBiota.BiotaPropertiesTextureMap.Add(existingValue);
                    }

                    existingValue.Index = value.PartIndex;
                    existingValue.OldId = value.OldTexture;
                    existingValue.NewId = value.NewTexture;
                    existingValue.Order = (byte)i;
                }
            }
            foreach (var value in targetBiota.BiotaPropertiesTextureMap)
            {
                if (sourceBiota.PropertiesTextureMap == null || value.Order >= sourceBiota.PropertiesTextureMap.Count)
                    context.BiotaPropertiesTextureMap.Remove(value);
            }


            // Properties for all world objects that typically aren't modified over the original Biota

            /*if (biota.PropertiesCreateList != null)
            {
                foreach (var value in biota.PropertiesCreateList)
                {
                    BiotaPropertiesCreateList existingValue = existingBiota.BiotaPropertiesCreateList.FirstOrDefault(r => r.Id == value.Id);

                    if (existingValue == null)
                        existingBiota.BiotaPropertiesCreateList.Add(value);
                    else
                    {
                        existingValue.DestinationType = value.DestinationType;
                        existingValue.WeenieClassId = value.WeenieClassId;
                        existingValue.StackSize = value.StackSize;
                        existingValue.Palette = value.Palette;
                        existingValue.Shade = value.Shade;
                        existingValue.TryToBond = value.TryToBond;
                    }
                }
            }
            foreach (var value in existingBiota.BiotaPropertiesCreateList)
            {
                if (biota.PropertiesCreateList == null || !biota.PropertiesCreateList.Any(p => p.Id == value.Id))
                    context.BiotaPropertiesCreateList.Remove(value);
            }*/

            /*if (biota.PropertiesEmote != null)
            {
                foreach (var value in biota.PropertiesEmote)
                {
                    BiotaPropertiesEmote existingValue = existingBiota.BiotaPropertiesEmote.FirstOrDefault(r => r.Id == value.Id);

                    if (existingValue == null)
                        existingBiota.BiotaPropertiesEmote.Add(value);
                    else
                    {
                        existingValue.Category = value.Category;
                        existingValue.Probability = value.Probability;
                        existingValue.WeenieClassId = value.WeenieClassId;
                        existingValue.Style = value.Style;
                        existingValue.Substyle = value.Substyle;
                        existingValue.Quest = value.Quest;
                        existingValue.VendorType = value.VendorType;
                        existingValue.MinHealth = value.MinHealth;
                        existingValue.MaxHealth = value.MaxHealth;

                        foreach (var value2 in value.PropertiesEmoteAction)
                        {
                            BiotaPropertiesEmoteAction existingValue2 = existingValue.BiotaPropertiesEmoteAction.FirstOrDefault(r => r.Id == value2.Id);

                            if (existingValue2 == null)
                                existingValue.BiotaPropertiesEmoteAction.Add(value2);
                            else
                            {
                                //existingValue2.EmoteId = value2.EmoteId;
                                existingValue2.Order = (uint)value.PropertiesEmoteAction.IndexOf(value2);
                                existingValue2.Type = value2.Type;
                                existingValue2.Delay = value2.Delay;
                                existingValue2.Extent = value2.Extent;
                                existingValue2.Motion = value2.Motion;
                                existingValue2.Message = value2.Message;
                                existingValue2.TestString = value2.TestString;
                                existingValue2.Min = value2.Min;
                                existingValue2.Max = value2.Max;
                                existingValue2.Min64 = value2.Min64;
                                existingValue2.Max64 = value2.Max64;
                                existingValue2.MinDbl = value2.MinDbl;
                                existingValue2.MaxDbl = value2.MaxDbl;
                                existingValue2.Stat = value2.Stat;
                                existingValue2.Display = value2.Display;
                                existingValue2.Amount = value2.Amount;
                                existingValue2.Amount64 = value2.Amount64;
                                existingValue2.HeroXP64 = value2.HeroXP64;
                                existingValue2.Percent = value2.Percent;
                                existingValue2.SpellId = value2.SpellId;
                                existingValue2.WealthRating = value2.WealthRating;
                                existingValue2.TreasureClass = value2.TreasureClass;
                                existingValue2.TreasureType = value2.TreasureType;
                                existingValue2.PScript = value2.PScript;
                                existingValue2.Sound = value2.Sound;
                                existingValue2.DestinationType = value2.DestinationType;
                                existingValue2.WeenieClassId = value2.WeenieClassId;
                                existingValue2.StackSize = value2.StackSize;
                                existingValue2.Palette = value2.Palette;
                                existingValue2.Shade = value2.Shade;
                                existingValue2.TryToBond = value2.TryToBond;
                                existingValue2.ObjCellId = value2.ObjCellId;
                                existingValue2.OriginX = value2.OriginX;
                                existingValue2.OriginY = value2.OriginY;
                                existingValue2.OriginZ = value2.OriginZ;
                                existingValue2.AnglesW = value2.AnglesW;
                                existingValue2.AnglesX = value2.AnglesX;
                                existingValue2.AnglesY = value2.AnglesY;
                                existingValue2.AnglesZ = value2.AnglesZ;
                            }
                        }
                        foreach (var value2 in value.PropertiesEmoteAction)
                        {
                            if (!existingValue.BiotaPropertiesEmoteAction.Any(p => p.Id == value2.Id))
                                context.BiotaPropertiesEmoteAction.Remove(value2);
                        }
                    }
                }
            }
            foreach (var value in existingBiota.BiotaPropertiesEmote)
            {
                if (biota.PropertiesEmote == null || !biota.PropertiesEmote.Any(p => p.Id == value.Id))
                    context.BiotaPropertiesEmote.Remove(value);
            }*/

            if (sourceBiota.PropertiesEventFilter != null)
            {
                foreach (var value in sourceBiota.PropertiesEventFilter)
                {
                    BiotaPropertiesEventFilter existingValue = targetBiota.BiotaPropertiesEventFilter.FirstOrDefault(r => r.Event == value);

                    if (existingValue == null)
                    {
                        var entity = new BiotaPropertiesEventFilter { ObjectId = sourceBiota.Id, Event = value };

                        targetBiota.BiotaPropertiesEventFilter.Add(entity);
                    }
                }
            }
            foreach (var value in targetBiota.BiotaPropertiesEventFilter)
            {
                if (sourceBiota.PropertiesEventFilter == null || !sourceBiota.PropertiesEventFilter.Any(p => p == value.Event))
                    context.BiotaPropertiesEventFilter.Remove(value);
            }

            /*if (biota.PropertiesGenerator != null)
            {
                foreach (var value in biota.PropertiesGenerator)
                {
                    BiotaPropertiesGenerator existingValue = existingBiota.BiotaPropertiesGenerator.FirstOrDefault(r => r.Id == value.Id);

                    if (existingValue == null)
                        existingBiota.BiotaPropertiesGenerator.Add(value);
                    else
                    {
                        existingValue.Probability = value.Probability;
                        existingValue.WeenieClassId = value.WeenieClassId;
                        existingValue.Delay = value.Delay;
                        existingValue.InitCreate = value.InitCreate;
                        existingValue.MaxCreate = value.MaxCreate;
                        existingValue.WhenCreate = value.WhenCreate;
                        existingValue.WhereCreate = value.WhereCreate;
                        existingValue.StackSize = value.StackSize;
                        existingValue.PaletteId = value.PaletteId;
                        existingValue.Shade = value.Shade;
                        existingValue.ObjCellId = value.ObjCellId;
                        existingValue.OriginX = value.OriginX;
                        existingValue.OriginY = value.OriginY;
                        existingValue.OriginZ = value.OriginZ;
                        existingValue.AnglesW = value.AnglesW;
                        existingValue.AnglesX = value.AnglesX;
                        existingValue.AnglesY = value.AnglesY;
                        existingValue.AnglesZ = value.AnglesZ;
                    }
                }
            }
            foreach (var value in existingBiota.BiotaPropertiesGenerator)
            {
                if (biota.PropertiesGenerator == null || !biota.PropertiesGenerator.Any(p => p.Id == value.Id))
                    context.BiotaPropertiesGenerator.Remove(value);
            }*/


            // Properties for creatures

            if (sourceBiota.PropertiesAttribute != null)
            {
                foreach (var kvp in sourceBiota.PropertiesAttribute)
                {
                    BiotaPropertiesAttribute existingValue = targetBiota.BiotaPropertiesAttribute.FirstOrDefault(r => r.Type == (ushort)kvp.Key);

                    if (existingValue == null)
                    {
                        existingValue = new BiotaPropertiesAttribute { ObjectId = sourceBiota.Id };

                        targetBiota.BiotaPropertiesAttribute.Add(existingValue);
                    }

                    existingValue.Type = (ushort)kvp.Key;
                    existingValue.InitLevel = kvp.Value.InitLevel;
                    existingValue.LevelFromCP = kvp.Value.LevelFromCP;
                    existingValue.CPSpent = kvp.Value.CPSpent;
                }
            }
            foreach (var value in targetBiota.BiotaPropertiesAttribute)
            {
                if (sourceBiota.PropertiesAttribute == null || !sourceBiota.PropertiesAttribute.ContainsKey((PropertyAttribute)value.Type))
                    context.BiotaPropertiesAttribute.Remove(value);
            }

            if (sourceBiota.PropertiesAttribute2nd != null)
            {
                foreach (var kvp in sourceBiota.PropertiesAttribute2nd)
                {
                    BiotaPropertiesAttribute2nd existingValue = targetBiota.BiotaPropertiesAttribute2nd.FirstOrDefault(r => r.Type == (ushort)kvp.Key);

                    if (existingValue == null)
                    {
                        existingValue = new BiotaPropertiesAttribute2nd { ObjectId = sourceBiota.Id };

                        targetBiota.BiotaPropertiesAttribute2nd.Add(existingValue);
                    }

                    existingValue.Type = (ushort)kvp.Key;
                    existingValue.InitLevel = kvp.Value.InitLevel;
                    existingValue.LevelFromCP = kvp.Value.LevelFromCP;
                    existingValue.CPSpent = kvp.Value.CPSpent;
                    existingValue.CurrentLevel = kvp.Value.CurrentLevel;
                }

            }
            foreach (var value in targetBiota.BiotaPropertiesAttribute2nd)
            {
                if (sourceBiota.PropertiesAttribute2nd == null || !sourceBiota.PropertiesAttribute2nd.ContainsKey((PropertyAttribute2nd)value.Type))
                    context.BiotaPropertiesAttribute2nd.Remove(value);
            }

            if (sourceBiota.PropertiesBodyPart != null)
            {
                foreach (var kvp in sourceBiota.PropertiesBodyPart)
                {
                    BiotaPropertiesBodyPart existingValue = targetBiota.BiotaPropertiesBodyPart.FirstOrDefault(r => r.Key == (uint)kvp.Key);

                    if (existingValue == null)
                    {
                        existingValue = new BiotaPropertiesBodyPart { ObjectId = sourceBiota.Id };

                        targetBiota.BiotaPropertiesBodyPart.Add(existingValue);
                    }

                    existingValue.Key = (ushort)kvp.Key;
                    existingValue.DType = (int)kvp.Value.DType;
                    existingValue.DVal = kvp.Value.DVal;
                    existingValue.DVar = kvp.Value.DVar;
                    existingValue.BaseArmor = kvp.Value.BaseArmor;
                    existingValue.ArmorVsSlash = kvp.Value.ArmorVsSlash;
                    existingValue.ArmorVsPierce = kvp.Value.ArmorVsPierce;
                    existingValue.ArmorVsBludgeon = kvp.Value.ArmorVsBludgeon;
                    existingValue.ArmorVsCold = kvp.Value.ArmorVsCold;
                    existingValue.ArmorVsFire = kvp.Value.ArmorVsFire;
                    existingValue.ArmorVsAcid = kvp.Value.ArmorVsAcid;
                    existingValue.ArmorVsElectric = kvp.Value.ArmorVsElectric;
                    existingValue.ArmorVsNether = kvp.Value.ArmorVsNether;
                    existingValue.BH = kvp.Value.BH;
                    existingValue.HLF = kvp.Value.HLF;
                    existingValue.MLF = kvp.Value.MLF;
                    existingValue.LLF = kvp.Value.LLF;
                    existingValue.HRF = kvp.Value.HRF;
                    existingValue.MRF = kvp.Value.MRF;
                    existingValue.LRF = kvp.Value.LRF;
                    existingValue.HLB = kvp.Value.HLB;
                    existingValue.MLB = kvp.Value.MLB;
                    existingValue.LLB = kvp.Value.LLB;
                    existingValue.HRB = kvp.Value.HRB;
                    existingValue.MRB = kvp.Value.MRB;
                    existingValue.LRB = kvp.Value.LRB;
                }
            }
            foreach (var value in targetBiota.BiotaPropertiesBodyPart)
            {
                if (sourceBiota.PropertiesBodyPart == null || !sourceBiota.PropertiesBodyPart.ContainsKey((CombatBodyPart)value.Key))
                    context.BiotaPropertiesBodyPart.Remove(value);
            }

            if (sourceBiota.PropertiesSkill != null)
            {
                foreach (var kvp in sourceBiota.PropertiesSkill)
                {
                    BiotaPropertiesSkill existingValue = targetBiota.BiotaPropertiesSkill.FirstOrDefault(r => r.Type == (ushort)kvp.Key);

                    if (existingValue == null)
                    {
                        existingValue = new BiotaPropertiesSkill { ObjectId = sourceBiota.Id };

                        targetBiota.BiotaPropertiesSkill.Add(existingValue);
                    }

                    existingValue.Type = (ushort)kvp.Key;
                    existingValue.LevelFromPP = kvp.Value.LevelFromPP;
                    existingValue.SAC = (uint)kvp.Value.SAC;
                    existingValue.PP = kvp.Value.PP;
                    existingValue.InitLevel = kvp.Value.InitLevel;
                    existingValue.ResistanceAtLastCheck = kvp.Value.ResistanceAtLastCheck;
                    existingValue.LastUsedTime = kvp.Value.LastUsedTime;
                }
            }
            foreach (var value in targetBiota.BiotaPropertiesSkill)
            {
                if (sourceBiota.PropertiesSkill == null || !sourceBiota.PropertiesSkill.ContainsKey((Skill)value.Type))
                    context.BiotaPropertiesSkill.Remove(value);
            }


            // Properties for books

            if (sourceBiota.PropertiesBook != null)
            {
                if (targetBiota.BiotaPropertiesBook == null)
                    targetBiota.BiotaPropertiesBook = new BiotaPropertiesBook { ObjectId = sourceBiota.Id, };

                targetBiota.BiotaPropertiesBook.MaxNumPages = sourceBiota.PropertiesBook.MaxNumPages;
                targetBiota.BiotaPropertiesBook.MaxNumCharsPerPage = sourceBiota.PropertiesBook.MaxNumCharsPerPage;
            }
            else
            {
                if (targetBiota.BiotaPropertiesBook != null)
                    context.BiotaPropertiesBook.Remove(targetBiota.BiotaPropertiesBook);
            }

            if (sourceBiota.PropertiesBookPageData != null)
            {
                for (int i = 0; i < sourceBiota.PropertiesBookPageData.Count; i++)
                {
                    var value = sourceBiota.PropertiesBookPageData[i];

                    BiotaPropertiesBookPageData existingValue = targetBiota.BiotaPropertiesBookPageData.FirstOrDefault(r => r.PageId == i);

                    if (existingValue == null)
                    {
                        existingValue = new BiotaPropertiesBookPageData { ObjectId = sourceBiota.Id };

                        targetBiota.BiotaPropertiesBookPageData.Add(existingValue);
                    }

                    existingValue.PageId = (uint)i;
                    existingValue.AuthorId = value.AuthorId;
                    existingValue.AuthorName = value.AuthorName;
                    existingValue.AuthorAccount = value.AuthorAccount;
                    existingValue.IgnoreAuthor = value.IgnoreAuthor;
                    existingValue.PageText = value.PageText;
                }
            }
            foreach (var value in targetBiota.BiotaPropertiesBookPageData)
            {
                if (sourceBiota.PropertiesBookPageData == null || value.PageId >= sourceBiota.PropertiesBookPageData.Count)
                    context.BiotaPropertiesBookPageData.Remove(value);
            }


            // Biota additions over Weenie

            if (sourceBiota.PropertiesAllegiance != null)
            {
                foreach (var kvp in sourceBiota.PropertiesAllegiance)
                {
                    BiotaPropertiesAllegiance existingValue = targetBiota.BiotaPropertiesAllegiance.FirstOrDefault(r => r.CharacterId == kvp.Key);

                    if (existingValue == null)
                    {
                        existingValue = new BiotaPropertiesAllegiance { AllegianceId = sourceBiota.Id };

                        targetBiota.BiotaPropertiesAllegiance.Add(existingValue);
                    }

                    existingValue.CharacterId = kvp.Key;
                    existingValue.Banned = kvp.Value.Banned;
                    existingValue.ApprovedVassal = kvp.Value.ApprovedVassal;
                }
            }
            foreach (var value in targetBiota.BiotaPropertiesAllegiance)
            {
                if (sourceBiota.PropertiesAllegiance == null || !sourceBiota.PropertiesAllegiance.ContainsKey(value.CharacterId))
                    context.BiotaPropertiesAllegiance.Remove(value);
            }

            if (sourceBiota.PropertiesEnchantmentRegistry != null)
            {
                foreach (var value in sourceBiota.PropertiesEnchantmentRegistry)
                {
                    BiotaPropertiesEnchantmentRegistry existingValue = targetBiota.BiotaPropertiesEnchantmentRegistry.FirstOrDefault(r => r.SpellId == value.SpellId && r.LayerId == value.LayerId && r.CasterObjectId == value.CasterObjectId);

                    if (existingValue == null)
                    {
                        existingValue = new BiotaPropertiesEnchantmentRegistry { ObjectId = sourceBiota.Id };

                        targetBiota.BiotaPropertiesEnchantmentRegistry.Add(existingValue);
                    }

                    existingValue.EnchantmentCategory = value.EnchantmentCategory;
                    existingValue.SpellId = value.SpellId;
                    existingValue.LayerId = value.LayerId;
                    existingValue.HasSpellSetId = value.HasSpellSetId;
                    existingValue.SpellCategory = (ushort)value.SpellCategory;
                    existingValue.PowerLevel = value.PowerLevel;
                    existingValue.StartTime = value.StartTime;
                    existingValue.Duration = value.Duration;
                    existingValue.CasterObjectId = value.CasterObjectId;
                    existingValue.DegradeModifier = value.DegradeModifier;
                    existingValue.DegradeLimit = value.DegradeLimit;
                    existingValue.LastTimeDegraded = value.LastTimeDegraded;
                    existingValue.StatModType = (uint)value.StatModType;
                    existingValue.StatModKey = value.StatModKey;
                    existingValue.StatModValue = value.StatModValue;
                    existingValue.SpellSetId = (uint)value.SpellSetId;
                }
            }
            foreach (var value in targetBiota.BiotaPropertiesEnchantmentRegistry)
            {
                if (sourceBiota.PropertiesEnchantmentRegistry == null || !sourceBiota.PropertiesEnchantmentRegistry.Any(p => p.SpellId == value.SpellId && p.LayerId == value.LayerId && p.CasterObjectId == value.CasterObjectId))
                    context.BiotaPropertiesEnchantmentRegistry.Remove(value);
            }

            if (sourceBiota.HousePermissions != null)
            {
                foreach (var kvp in sourceBiota.HousePermissions)
                {
                    HousePermission existingValue = targetBiota.HousePermission.FirstOrDefault(r => r.PlayerGuid == kvp.Key);

                    if (existingValue == null)
                    {
                        existingValue = new HousePermission { HouseId = sourceBiota.Id };

                        targetBiota.HousePermission.Add(existingValue);
                    }

                    existingValue.PlayerGuid = kvp.Key;
                    existingValue.Storage = kvp.Value;
                }
            }
            foreach (var value in targetBiota.HousePermission)
            {
                if (sourceBiota.HousePermissions == null || !sourceBiota.HousePermissions.ContainsKey(value.PlayerGuid))
                    context.HousePermission.Remove(value);
            }
        }
    }
}
