﻿using System.Collections.Generic;
using System.Linq;
using RomUtilities;
using System.ComponentModel;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FF1Lib
{
	public enum LockHitMode
	{
		[Description("Vanilla")]
		Vanilla = 0,
		[Description("107 Accuracy")]
		Accuracy107,
		[Description("162 Accuracy")]
		Accuracy162,
		[Description("Auto Hit")]
		AutoHit
	}

	public enum AutohitThreshold
	{
		[Description("300")]
		Vanilla = 0,
		[Description("600")]
		Autohit600,
		[Description("900")]
		Autohit900,
		[Description("1200")]
		Autohit1200,
		[Description("Unlimited")]
		Autohit65535,
		[Description("300 or 600")]
		Autohit300to600,
		[Description("300, 600, or 900")]
		Autohit300to900,
		[Description("300, 600, 900, or 1200")]
		Autohit300to1200,
		[Description("Any of the above")]
		Any,
	}

	public enum SpellRoutine : byte
	{
	        None = 0,
		Damage = 0x01,
		DamageUndead = 0x02,
		Heal = 0x07,
		CureAilment = 0x08,
		FullHeal = 0x0F,
		ArmorUp = 0x09,
		DefElement = 0x0A,
		Fast = 0x0C,
		Sabr = 0x0D,
		Lock = 0x0E,
		Ruse = 0x10,
		PowerWord = 0x12,
		InflictStatus = 0x03,
		Life = 0xF0,
		Smoke = 0xF1
	}

	public enum SpellTargeting : byte
	{
		Any = 0xFF,
		None = 0,
		AllEnemies = 0x01,
		OneEnemy = 0x02,
		Self = 0x04,
		AllCharacters = 0x08,
		OneCharacter = 0x10
	}

	public enum SpellElement : byte
	{
		Any = 0b10101010,
		None = 0x00,
		Earth = 0b10000000,
		Lightning = 0b01000000,
		Ice = 0b00100000,
		Fire = 0b00010000,
		Death =	0b00001000,
		Time = 0b00000100,
		Poison = 0b00000010,
		Status = 0b00000001,
		All = 0xFF
	}

	public enum SpellStatus : byte
	{
	        None = 0,
		Any = 0xFF,
		Confuse = 0b10000000,
		Mute = 0b01000000,
		Dark = 0b00001000,
		Stun = 0b00010000,
		Sleep = 0b00100000,
		Stone = 0b00000010,
		Death = 0b00000001,
		Poison = 0b00000100
	}

	public enum OOBSpellRoutine : byte {
	    CURE = 0,
	    CUR2 = 1,
	    CUR3 = 2,
	    CUR4 = 3,
	    HEAL = 4,
	    HEL3 = 5,
	    HEL2 = 6,
	    PURE = 7,
	    LIFE = 8,
	    LIF2 = 9,
	    WARP = 10,
	    SOFT = 11,
	    EXIT = 12,
	    None = 255
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class MagicSpell
	{
	    [JsonProperty]
	    public byte Index;

	    public bool ShouldSerializeIndex() {
		return !isRegularSpell;
	    }

	    public Blob Data;

	    [JsonProperty]
	    public string Name;

	    [JsonProperty]
	    public byte TextPointer;

	    public bool ShouldSerializeTextPointer() {
		return isRegularSpell;
	    }

	    [JsonProperty]
	    public string Message;

	    public bool ShouldSerializeMessage() {
		return isRegularSpell;
	    }

	    [JsonProperty]
	    public byte accuracy = 0;

	    [JsonProperty]
	    [JsonConverter(typeof(StringEnumConverter))]
	    public SpellElement elem = 0;

	    [JsonProperty]
	    [JsonConverter(typeof(StringEnumConverter))]
	    public SpellTargeting targeting = 0;

	    [JsonProperty]
	    [JsonConverter(typeof(StringEnumConverter))]
	    public SpellRoutine routine = 0;

	    [JsonProperty]
	    public byte effect = 0;

	    public bool ShouldSerializeeffect() {
		return routine == SpellRoutine.Damage || routine == SpellRoutine.DamageUndead || routine == SpellRoutine.Heal ||
		    routine == SpellRoutine.ArmorUp || routine == SpellRoutine.Sabr || routine == SpellRoutine.Ruse;
	    }

	    [JsonProperty]
	    [JsonConverter(typeof(StringEnumConverter))]
	    public SpellStatus status {
		get { return (SpellStatus)effect; }
		set { effect = (byte)value; }
	    }
	    public bool ShouldSerializestatus() {
		return routine == SpellRoutine.CureAilment || routine == SpellRoutine.InflictStatus;
	    }

	    [JsonProperty]
	    public string defelement {
		get {
		    var sp = new Dictionary<SpellElement, string> {
			{SpellElement.Earth, "Earth"},
			{SpellElement.Lightning, "Lightning"},
			{SpellElement.Ice, "Ice"},
			{SpellElement.Fire, "Fire"},
			{SpellElement.Death, "Death"},
			{SpellElement.Time, "Time"},
			{SpellElement.Poison, "Poison"},
			{SpellElement.Status, "Status"}
		    };
		    string ret = "";
		    foreach (var kv in sp) {
			if ((effect & (byte)kv.Key) != 0) {
			    if (ret.Length > 0) {
				ret += ",";
			    }
			    ret += kv.Value;
			}
		    }
		    return ret;
		}
	    }
	    public bool ShouldSerializedefelement() {
		return routine == SpellRoutine.DefElement;
	    }

	    [JsonProperty]
	    public byte gfx = 0;

	    public bool ShouldSerializegfx() {
		return isRegularSpell;
	    }

	    [JsonProperty]
	    public byte palette = 0;

	    public bool ShouldSerializepalette() {
		return isRegularSpell;
	    }

	    void updateMagicIndex(byte level, byte slot, string type) {
		this.Index = (byte)((level-1) * 8 + (slot-1));
		if (type == "black") {
		    this.Index += 4;
		}
	    }

	    [JsonProperty]
	    public byte Level {
		get {
		    return (byte)((Index / 8)+1);
		}
		set {
		    this.updateMagicIndex(value, Slot, MagicType);
		}
	    }

	    public bool ShouldSerializeLevel() {
		return isRegularSpell;
	    }

	    [JsonProperty]
	    public byte Slot {
		get {
		    return (byte)((Index % 4) + 1);
		}
		set {
		    this.updateMagicIndex(Level, value, MagicType);
		}
	    }

	    public bool ShouldSerializeSlot() {
		return isRegularSpell;
	    }

	    [JsonProperty]
	    public string MagicType {
		get {
		    if (Index % 8 < 4) {
			return "white";
		    }
		    return "black";
		}
		set {
		    this.updateMagicIndex(Level, Slot, value);
		}
	    }

	    public bool ShouldSerializeMagicType() {
		return isRegularSpell;
	    }

	    [JsonProperty]
	    [JsonConverter(typeof(StringEnumConverter))]
	    public OOBSpellRoutine oobSpellRoutine = OOBSpellRoutine.None;

	    public bool ShouldSerializeoobSpellRoutine() {
		return isRegularSpell;
	    }

	    List<Classes> _permissions = new();

	    [JsonProperty]
	    public string permissions {
		get {
		    if (_permissions == null) {
			return "";
		    }
		    string ret = "";
		    foreach (var c in _permissions) {
			if (ret != "") {
			    ret += ",";
			}
			ret += Enum.GetName(c);
		    }
		    return ret;
		}
	    }

	    public bool ShouldSerializepermissions() {
		return isRegularSpell;
	    }

	    bool isRegularSpell;

	    public MagicSpell(byte _Index,
			      Blob _Data,
			      string _Name,
			      byte _TextPointer,
			      string _Message,
			      List<Classes> __permissions)
	    {
		Index = _Index;
		Data = _Data;
		Name = _Name;
		TextPointer = _TextPointer;
		Message = _Message;
		_permissions = __permissions;
		isRegularSpell = true;
		this.decompressData(Data);
	    }

	    public MagicSpell(byte _Index,
			      Blob _Data,
			      string _Name,
			      bool _isRegularSpell)
	    {
		Index = _Index;
		Data = _Data;
		Name = _Name;
		isRegularSpell = _isRegularSpell;
		this.decompressData(Data);
	    }

	    public byte[] compressData()
	    {
		Data[0] = accuracy;
		Data[1] = effect;
		Data[2] = (byte)elem;
		Data[3] = (byte)targeting;
		Data[4] = (byte)routine;
		Data[5] = gfx;
		Data[6] = palette;
		Data[7] = 0x00; // last byte is always 00
		return Data;
	    }

	    public void writeData(FF1Rom rom) {
		compressData();
		if (isRegularSpell) {
		    rom.Put(FF1Rom.MagicOffset + FF1Rom.MagicSize * Index, Data);
		    rom.ItemsText[176 + Index] = Name;
		}
	    }

	    public void decompressData(byte[] data)
	    {
		accuracy = data[0];
		effect = data[1];
		elem = (SpellElement)data[2];
		targeting = (SpellTargeting)data[3];
		routine = (SpellRoutine)data[4];
		gfx = data[5];
		palette = data[6];
	    }

	    public override string ToString()
	    {
		return Index.ToString() + ": " + Name;
	    }
	}

	public partial class FF1Rom : NesRom
	{
		public const int MagicOffset = 0x301E0;
		public const int MagicSize = 8;
		public const int MagicCount = 64;
		public const int MagicNamesOffset = 0x2BE03;
		public const int MagicNameSize = 5;
		public const int MagicTextPointersOffset = 0x304C0;
		public const int MagicPermissionsOffset = 0x3AD18;
		public const int MagicPermissionsSize = 8;
		public const int MagicPermissionsCount = 12;
		public const int MagicOutOfBattleOffset = 0x3AEFA;
		public const int MagicOutOfBattleSize = 7;
		public const int MagicOutOfBattleCount = 13;

		public const int OldLevelUpDataOffset = 0x2D094; // this was moved to bank 1B
		public const int NewLevelUpDataOffset = 0x6CDA9; // this was moved from bank 1B
		public const int oldKnightNinjaMaxMPOffset = 0x6C907;
		public const int newKnightNinjaMaxMPOffset = 0x6D344;

		public const int ConfusedSpellIndexOffset = 0x3321E;
		public const int FireSpellIndex = 4;

		public const int WeaponOffset = 0x30000;
		public const int WeaponSize = 8;
		public const int WeaponCount = 40;

		public const int ArmorOffset = 0x30140;
		public const int ArmorSize = 4;
		public const int ArmorCount = 40;

		public void BuffTier1DamageSpells()
		{
			Put(MagicOffset + MagicSize * 4 + 1, new byte[] { 60 }); // replace FIRE effectivity
			Put(MagicOffset + MagicSize * 7 + 1, new byte[] { 70 }); // replace LIT effectivity
			Put(MagicOffset + MagicSize * 12 + 1, new byte[] { 80 }); // replace ICE effectivity
			Put(MagicOffset + MagicSize * 1 + 1, new byte[] { 80 }); // replace HARM effectivity
		}

		public void BuffHealingSpells()
		{
			// improves CURE and HEAL spells both in and out of battle
			// this is also done by Spellcrafter
			// CURE
			Put(MagicOffset + 1, new byte[] { 0x20 }); // replace CURE effectivity with 32 (was 16)
			Put(0x3AF5F, Blob.FromHex("1F0920")); // changing the oob code for CURE to reflect new values
			// CUR2
			Put(MagicOffset + MagicSize * 16 + 1, new byte[] { 0x40 }); // replace CUR2 effectivity with 64 (was 32)
			Put(0x3AF67, Blob.FromHex("3F0940")); // changing the oob code for CUR2 to reflect the above effect
			// CUR3
			Put(MagicOffset + MagicSize * 32 + 1, new byte[] { 0x80 }); // replace CUR3 effectivity with 128 (was 64)
			Put(0x3AF6F, Blob.FromHex("7F0980")); // changing the oob code for CUR3 to reflect the above effect
			// HEAL
			Put(MagicOffset + MagicSize * 19 + 1, new byte[] { 0x10 }); // replace HEAL effectivity with 16 (was 12)
			Put(0x3AFDF, Blob.FromHex("0F")); // changing the oob code for HEAL to reflect the above effect
			// HEL2
			Put(MagicOffset + MagicSize * 35 + 1, new byte[] { 0x20 }); // replace HEL2 effectivity with 32 (was 24)
			Put(0x3AFE8, Blob.FromHex("1F")); // changing the oob code for HEL2 to reflect the above effect
			// HEL3
			Put(MagicOffset + MagicSize * 51 + 1, new byte[] { 0x40 }); // replace HEL3 effectivity with 64 (was 48)
			Put(0x3AFF1, Blob.FromHex("3F")); // changing the oob code for HEL3 to reflect the above effect
			// LAMP
			Put(MagicOffset + MagicSize * 8 + 1, new byte[] { 0x18 }); // LAMP heals paralysis as well as darkness
			// AMUT
			Put(MagicOffset + MagicSize * 27 + 1, new byte[] { 0x50 }); // AMUT heals paralysis as well as silence
		}

		public void ShuffleMagicLevels(MT19337 rng, bool keepPermissions, bool tieredShuffle, bool mixSpellbooks, bool noSpellcrafter)
		{
			var magicSpells = GetSpells();
			if(tieredShuffle && noSpellcrafter)
			{
				// if we are doing a tiered shuffle, swap the position of TMPR and SABR before further shuffling for balance purposes
				MagicSpell tmpTMPR = magicSpells[14];
				magicSpells[14] = magicSpells[54];
				magicSpells[54] = tmpTMPR;
			}

			// First we have to un-interleave white and black spells.
			var whiteSpells = magicSpells.Where((spell, i) => (i / 4) % 2 == 0).ToList();
			var blackSpells = magicSpells.Where((spell, i) => (i / 4) % 2 == 1).ToList();

			if(tieredShuffle)
			{
				// weigh spell probability of landing in a tier based on where it was in the original game
				var whiteSpellList = new List<MagicSpell>[3];
				var blackSpellList = new List<MagicSpell>[3];
				var whiteSpellFinalList = new List<MagicSpell>[3];
				var blackSpellFinalList = new List<MagicSpell>[3];
				int mergedSpellDoubler = 1;
				whiteSpellList[0] = magicSpells.Where((spell, i) => (i / 4) % 2 == 0 && i < 24).ToList();
				whiteSpellList[1] = magicSpells.Where((spell, i) => (i / 4) % 2 == 0 && i < 48 && i >= 24).ToList();
				whiteSpellList[2] = magicSpells.Where((spell, i) => (i / 4) % 2 == 0 && i >= 48).ToList();
				blackSpellList[0] = magicSpells.Where((spell, i) => (i / 4) % 2 == 1 && i < 24).ToList();
				blackSpellList[1] = magicSpells.Where((spell, i) => (i / 4) % 2 == 1 && i < 48 && i >= 24).ToList();
				blackSpellList[2] = magicSpells.Where((spell, i) => (i / 4) % 2 == 1 && i >= 48).ToList();
				if(mixSpellbooks)
				{
					whiteSpellList[0] = whiteSpellList[0].Concat(blackSpellList[0]).ToList();
					whiteSpellList[1] = whiteSpellList[1].Concat(blackSpellList[1]).ToList();
					whiteSpellList[2] = whiteSpellList[2].Concat(blackSpellList[2]).ToList();
					mergedSpellDoubler = 2;
				}
				whiteSpellFinalList[0] = new List<MagicSpell> { };
				whiteSpellFinalList[1] = new List<MagicSpell> { };
				whiteSpellFinalList[2] = new List<MagicSpell> { };
				blackSpellFinalList[0] = new List<MagicSpell> { };
				blackSpellFinalList[1] = new List<MagicSpell> { };
				blackSpellFinalList[2] = new List<MagicSpell> { };
				whiteSpells.Clear();
				blackSpells.Clear();
				foreach (MagicSpell spell in whiteSpellList[2])
				{
					// 70% chance of tier 7-8, 25% chance of tier 4-6, 5% chance of tier 1-3
					int diceRoll = rng.Between(0, 19);
					if(diceRoll < 14)
					{
						whiteSpellFinalList[2].Add(spell);
					}
					else if (diceRoll < 19)
					{
						whiteSpellFinalList[1].Add(spell);
					}
					else
					{
						whiteSpellFinalList[0].Add(spell);
					}
				}
				foreach (MagicSpell spell in whiteSpellList[1])
				{
					// 60% chance of tier 4-6, 25% chance of tier 1-3, 15% chance of tier 7-8
					// if a section of the final list is full, move to another section
					int diceRoll = rng.Between(0, 19);
					if(diceRoll < 12)
					{
						if(whiteSpellFinalList[1].Count >= 12 * mergedSpellDoubler)
						{
							if(whiteSpellFinalList[0].Count >= 12 * mergedSpellDoubler)
							{
								whiteSpellFinalList[2].Add(spell);
							}
							else
							{
								whiteSpellFinalList[0].Add(spell);
							}
						}
						else
						{
							whiteSpellFinalList[1].Add(spell);
						}
					}
					else if (diceRoll < 17)
					{
						if(whiteSpellFinalList[0].Count >= 12 * mergedSpellDoubler)
						{
							if(whiteSpellFinalList[1].Count >= 12 * mergedSpellDoubler)
							{
								whiteSpellFinalList[2].Add(spell);
							}
							else
							{
								whiteSpellFinalList[1].Add(spell);
							}
						}
						else
						{
							whiteSpellFinalList[0].Add(spell);
						}
					}
					else
					{
						if(whiteSpellFinalList[2].Count >= 8 * mergedSpellDoubler)
						{
							if(whiteSpellFinalList[1].Count >= 12 * mergedSpellDoubler)
							{
								whiteSpellFinalList[0].Add(spell);
							}
							else
							{
								whiteSpellFinalList[1].Add(spell);
							}
						}
						else
						{
							whiteSpellFinalList[2].Add(spell);
						}
					}
				}
				foreach(MagicSpell spell in whiteSpellList[0])
				{
					// fill the remaining tiers with the tier 1-3 base magic
					if(whiteSpellFinalList[0].Count >= 12 * mergedSpellDoubler)
					{
						if(whiteSpellFinalList[1].Count >= 12 * mergedSpellDoubler)
						{
							whiteSpellFinalList[2].Add(spell);
						}
						else
						{
							whiteSpellFinalList[1].Add(spell);
						}
					}
					else
					{
						whiteSpellFinalList[0].Add(spell);
					}
				}
				// and repeat the process for black magic if we didn't mix spellbooks
				if(mixSpellbooks)
				{
					// if we mixed spellbooks, split the white (merged) spellbook in halves to set the black spell list
					blackSpellFinalList[0] = whiteSpellFinalList[0].Take(12).ToList();
					whiteSpellFinalList[0] = whiteSpellFinalList[0].Except(blackSpellFinalList[0]).ToList();
					blackSpellFinalList[1] = whiteSpellFinalList[1].Take(12).ToList();
					whiteSpellFinalList[1] = whiteSpellFinalList[1].Except(blackSpellFinalList[1]).ToList();
					blackSpellFinalList[2] = whiteSpellFinalList[2].Take(8).ToList();
					whiteSpellFinalList[2] = whiteSpellFinalList[2].Except(blackSpellFinalList[2]).ToList();
				}
				else
				{
					foreach (MagicSpell spell in blackSpellList[2])
					{
						// 70% chance of tier 7-8, 25% chance of tier 4-6, 5% chance of tier 1-3
						int diceRoll = rng.Between(0, 19);
						if (diceRoll < 14)
						{
							blackSpellFinalList[2].Add(spell);
						}
						else if (diceRoll < 19)
						{
							blackSpellFinalList[1].Add(spell);
						}
						else
						{
							blackSpellFinalList[0].Add(spell);
						}
					}
					foreach (MagicSpell spell in blackSpellList[1])
					{
						// 60% chance of tier 4-6, 25% chance of tier 1-3, 15% chance of tier 7-8
						// if a section of the final list is full, move to another section
						int diceRoll = rng.Between(0, 19);
						if (diceRoll < 12)
						{
							if (blackSpellFinalList[1].Count >= 12)
							{
								if (blackSpellFinalList[0].Count >= 12)
								{
									blackSpellFinalList[2].Add(spell);
								}
								else
								{
									blackSpellFinalList[0].Add(spell);
								}
							}
							else
							{
								blackSpellFinalList[1].Add(spell);
							}
						}
						else if (diceRoll < 17)
						{
							if (blackSpellFinalList[0].Count >= 12)
							{
								if (blackSpellFinalList[1].Count >= 12)
								{
									blackSpellFinalList[2].Add(spell);
								}
								else
								{
									blackSpellFinalList[1].Add(spell);
								}
							}
							else
							{
								blackSpellFinalList[0].Add(spell);
							}
						}
						else
						{
							if (blackSpellFinalList[2].Count >= 8)
							{
								if (blackSpellFinalList[1].Count >= 12)
								{
									blackSpellFinalList[0].Add(spell);
								}
								else
								{
									blackSpellFinalList[1].Add(spell);
								}
							}
							else
							{
								blackSpellFinalList[2].Add(spell);
							}
						}
					}
					foreach (MagicSpell spell in blackSpellList[0])
					{
						// fill the remaining tiers with the tier 1-3 base magic
						if (blackSpellFinalList[0].Count >= 12)
						{
							if (blackSpellFinalList[1].Count >= 12)
							{
								blackSpellFinalList[2].Add(spell);
							}
							else
							{
								blackSpellFinalList[1].Add(spell);
							}
						}
						else
						{
							blackSpellFinalList[0].Add(spell);
						}
					}
				}
				// shuffle each of the final lists
				foreach(List<MagicSpell> spellList in whiteSpellFinalList)
				{
					spellList.Shuffle(rng);
				}
				if(!mixSpellbooks)
				{
					foreach (List<MagicSpell> spellList in blackSpellFinalList)
					{
						spellList.Shuffle(rng);
					}
				}
				// and append each in turn to the whitespells / blackspells list
				whiteSpells = whiteSpells.Concat(whiteSpellFinalList[0]).ToList();
				whiteSpells = whiteSpells.Concat(whiteSpellFinalList[1]).ToList();
				whiteSpells = whiteSpells.Concat(whiteSpellFinalList[2]).ToList();
				blackSpells = blackSpells.Concat(blackSpellFinalList[0]).ToList();
				blackSpells = blackSpells.Concat(blackSpellFinalList[1]).ToList();
				blackSpells = blackSpells.Concat(blackSpellFinalList[2]).ToList();
			}
			else
			{
				if(mixSpellbooks)
				{
					var mergedList = magicSpells.ToList();
					mergedList.Shuffle(rng);
					whiteSpells = mergedList.Where((spell, i) => (i / 4) % 2 == 0).ToList();
					blackSpells = mergedList.Where((spell, i) => (i / 4) % 2 == 1).ToList();
				}
				else
				{
					whiteSpells.Shuffle(rng);
					blackSpells.Shuffle(rng);
				}
			}

			// Now we re-interleave the spells.
			var shuffledSpells = new List<MagicSpell>();
			for (int i = 0; i < MagicCount; i++)
			{
				var sourceIndex = 4 * (i / 8) + i % 4;
				if ((i / 4) % 2 == 0)
				{
					shuffledSpells.Add(whiteSpells[sourceIndex]);
				}
				else
				{
					shuffledSpells.Add(blackSpells[sourceIndex]);
				}
			}

			Put(MagicOffset, shuffledSpells.Select(spell => spell.Data).Aggregate((seed, next) => seed + next));
			PutSpellNames(shuffledSpells);
			Put(MagicTextPointersOffset, shuffledSpells.Select(spell => spell.TextPointer).ToArray());

			if (keepPermissions)
			{
				// Shuffle the permissions the same way the spells were shuffled.
				for (int c = 0; c < MagicPermissionsCount; c++)
				{
					SpellPermissions[(Classes)c] = SpellPermissions[(Classes)c].Select(x => (SpellSlots)shuffledSpells.FindIndex(y => y.Index == (int)x)).ToList();
				}
			}

			// Map old indices to new indices.
			var newIndices = new byte[MagicCount];
			for (byte i = 0; i < MagicCount; i++)
			{
				newIndices[shuffledSpells[i].Index] = i;
			}

			// Fix enemy spell pointers to point to where the spells are now.
			var scripts = Get(ScriptOffset, ScriptSize * ScriptCount).Chunk(ScriptSize);
			foreach (var script in scripts)
			{
				// Bytes 2-9 are magic spells.
				for (int i = 2; i < 10; i++)
				{
					if (script[i] != 0xFF)
					{
						script[i] = newIndices[script[i]];
					}
				}
			}
			Put(ScriptOffset, scripts.SelectMany(script => script.ToBytes()).ToArray());

			// Fix weapon and armor spell pointers to point to where the spells are now.
			var weapons = Get(WeaponOffset, WeaponSize * WeaponCount).Chunk(WeaponSize);
			foreach (var weapon in weapons)
			{
				if (weapon[3] != 0x00)
				{
					weapon[3] = (byte)(newIndices[weapon[3] - 1] + 1);
				}
			}
			Put(WeaponOffset, weapons.SelectMany(weapon => weapon.ToBytes()).ToArray());

			var armors = Get(ArmorOffset, ArmorSize * ArmorCount).Chunk(ArmorSize);
			foreach (var armor in armors)
			{
				if (armor[3] != 0x00)
				{
					armor[3] = (byte)(newIndices[armor[3] - 1] + 1);
				}
			}
			Put(ArmorOffset, armors.SelectMany(armor => armor.ToBytes()).ToArray());

			// Fix the crazy out of battle spell system.
			var outOfBattleSpellOffset = MagicOutOfBattleOffset;
			for (int i = 0; i < MagicOutOfBattleCount; i++)
			{
				var oldSpellIndex = Data[outOfBattleSpellOffset] - 0xB0;
				var newSpellIndex = newIndices[oldSpellIndex];

				Put(outOfBattleSpellOffset, new[] { (byte)(newSpellIndex + 0xB0) });

				outOfBattleSpellOffset += MagicOutOfBattleSize;
			}

			// Confused enemies are supposed to cast FIRE, so figure out where FIRE ended up.
			var newFireSpellIndex = shuffledSpells.FindIndex(spell => spell.Data == magicSpells[FireSpellIndex].Data);
			Put(ConfusedSpellIndexOffset, new[] { (byte)newFireSpellIndex });
		}

		public void ChangeLockMode(LockHitMode lockHitMode)
		{
			if (lockHitMode == LockHitMode.Accuracy107)
			{
				Put(MagicOffset + (MagicSize * 6), new byte[] { 107 });
				Put(MagicOffset + (MagicSize * 23), new byte[] { 107 });
			}
			else if (lockHitMode == LockHitMode.Accuracy162)
			{
				Put(MagicOffset + (MagicSize * 6), new byte[] { 162 });
				Put(MagicOffset + (MagicSize * 23), new byte[] { 162 });
			}
			else if (lockHitMode == LockHitMode.AutoHit)
			{
				PutInBank(0x0C, 0xBA46, Blob.FromHex("2029B9AD856838ED7468B002A9008D85682085B860EAEAEAEAEAEAEAEAEAEAEAEAEA"));
			}
		}

		public void UpdateMagicAutohitThreshold(MT19337 rng, AutohitThreshold threshold)
		{
			short limit = 300;
			switch (threshold)
			{
				case AutohitThreshold.Vanilla: limit = 300; break;
				case AutohitThreshold.Autohit600: limit = 600; break;
				case AutohitThreshold.Autohit900: limit = 900; break;
				case AutohitThreshold.Autohit1200: limit = 1200; break;
				case AutohitThreshold.Autohit65535: limit = short.MaxValue; break;
				case AutohitThreshold.Autohit300to600: limit = (short)(rng.Between(1, 2) * 300); break;
				case AutohitThreshold.Autohit300to900: limit = (short)(rng.Between(1, 3) * 300); break;
				case AutohitThreshold.Autohit300to1200: limit = (short)(rng.Between(1, 4) * 300); break;
				case AutohitThreshold.Any:
				{
					short[] any = { 300, 600, 900, 1200, short.MaxValue };
					limit = any.PickRandom(rng);
					break;
				}
			}

			// Set the low and high bytes of the limit which are then loaded and compared to the targets hp.
			Data[0x33AE0] = (byte)(limit & 0x00ff);
			Data[0x33AE5] = (byte)((limit >> 8) & 0x00ff);
		}

		public List<MagicSpell> GetSpells() {
			var spells = Get(MagicOffset, MagicSize * MagicCount).Chunk(MagicSize);
			var pointers = Get(MagicTextPointersOffset, MagicCount);

			var battleMessages = new BattleMessages(this);

			var spellsList = spells.Select((spell, i) => new MagicSpell((byte)i, spell, ItemsText[176 + i], pointers[i],
										    pointers[i] > 0 ? battleMessages[pointers[i]-1] : "",
										    SpellPermissions.PermissionsFor((SpellSlots)i))
			).ToList();

			for (int i = 0; i < MagicOutOfBattleCount; i++) {
			    var spellIndex = Data[MagicOutOfBattleOffset  + i*MagicOutOfBattleSize] - 0xB0;
			    spellsList[spellIndex].oobSpellRoutine = (OOBSpellRoutine)i;
			}
			return spellsList;
		}

		void PutSpells(FF1Rom rom, List<MagicSpell> spellsList) {
		    foreach (var sp in spellsList) {
			sp.writeData(rom);

			if (sp.oobSpellRoutine != OOBSpellRoutine.None) {
			    continue;
			}

			// update the out of battle magic code, it's a simple hardcoded table
			// that compares the spell index and jumps to the desired routine
			Data[MagicOutOfBattleOffset + (MagicOutOfBattleSize * (int)sp.oobSpellRoutine)] = (byte)(sp.Index + 0xB0);

			// update the effectivity of healing spells
			byte mask = 1;
			while (sp.effect >= mask) {
			    mask = (byte)(mask << 1);
			}
			mask = (byte)(mask >> 1);
			mask = (byte)(mask - 1);

			switch (sp.oobSpellRoutine) {
			    case OOBSpellRoutine.CURE:
				// AND #mask
				// ADC #effect
				Put(0x3AF5E, new byte[] { 0x29, mask, 0x69, sp.effect }); // changing the oob code for CURE to reflect new values
				break;
			    case OOBSpellRoutine.CUR2:
				Put(0x3AF66, new byte[] { 0x29, mask, 0x69, sp.effect }); // changing the oob code for CUR2 to reflect new values
				break;
			    case OOBSpellRoutine.CUR3:
				Put(0x3AF6E, new byte[] { 0x29, mask, 0x69, sp.effect }); // changing the oob code for CUR3 to reflect new values
				break;

			    case OOBSpellRoutine.HEAL:
				// AND #mask
				// CLC
				// ADC #effect
				Put(0x3AFDB, new byte[] { 0x29, mask, 0x18, 0x69, sp.effect }); // changing the oob code for HEAL to reflect the above effect
				break;
			    case OOBSpellRoutine.HEL2:
				Put(0x3AFE4, new byte[] { 0x29, mask, 0x18, 0x69, sp.effect }); // changing the oob code for HEL2 to reflect the above effect
				break;
			    case OOBSpellRoutine.HEL3:
				Put(0x3AFED, new byte[] { 0x29, mask, 0x18, 0x69, sp.effect }); // changing the oob code for HEL3 to reflect the above effect
				break;
			    default:
				break;
			}
		    }

		    // Fix enemy spell pointers to point to where the spells are now.
		    // ???

		    // Fix weapon and armor spell pointers to point to where the spells are now.
		    // ???

		    // Confused enemies are supposed to cast FIRE, so figure out where FIRE ended up.
		    //var newFireSpellIndex = shuffledSpells.FindIndex(spell => spell.Data == magicSpells[FireSpellIndex].Data);
		    //Put(ConfusedSpellIndexOffset, new[] { (byte)newFireSpellIndex });
		}

		public void PutSpellNames(List<MagicSpell> spells)
		{

			for(int i = 0; i < spells.Count; i++)
			{
				ItemsText[176 + i] = spells[i].Name;
			}
		}

		public void AccessibleSpellNames(Flags flags)
		{
			// If Spellcrafter mode is on, abort. We need a check here as the setting on the site can be in a random state.
			if ((bool)flags.GenerateNewSpellbook)
			{
				return;
			}

			var magicSpells = GetSpells();

			// Since this can be performed independent of the magic shuffling, we can't assume the location of spell names.
			// We will loop through the spell list and replace the appropriate names as we find them.
			for (int i = 0; i < magicSpells.Count; i++)
			{
				MagicSpell newSpell = magicSpells[i];
				string spellName = magicSpells[i].Name;

				switch (spellName)
				{
					// Note that 3 letter spell names actually have a trailing space
					case "LIT ":
						newSpell.Name = "THUN";
						break;
					case "LIT2":
						newSpell.Name = "THN2";
						break;
					case "LIT3":
						newSpell.Name = "THN3";
						break;
					case "FAST":
						newSpell.Name = "HAST";
						break;
					case "SLEP":
						newSpell.Name = "DOZE";
						break;
					case "SLP2":
						newSpell.Name = "DOZ2";
						break;

					case "HARM":
						newSpell.Name = "DIA ";
						break;
					case "HRM2":
						newSpell.Name = "DIA2";
						break;
					case "HRM3":
						newSpell.Name = "DIA3";
						break;
					case "HRM4":
						newSpell.Name = "DIA4";
						break;
					case "ALIT":
						newSpell.Name = "ATHN";
						break;
					case "AMUT":
						newSpell.Name = "VOX ";
						break;
					case "FOG ":
						newSpell.Name = "PROT";
						break;
					case "FOG2":
						newSpell.Name = "PRO2";
						break;
					case "FADE":
						newSpell.Name = "HOLY";
						break;
				}

				// Update the entry in the list
				magicSpells[i] = newSpell;
			}

			// Now update the spell names!
			PutSpellNames(magicSpells);
		}

		public void MixUpSpellNames(SpellNameMadness mode, MT19337 rng)
		{
			if (mode == SpellNameMadness.MixedUp)
			{
				string[] spellnames = new string[64];
				Array.Copy(ItemsText.ToList().ToArray(), 176, spellnames, 0, 64);

				var spellnamelist = new List<string>(spellnames);
				spellnamelist.Shuffle(rng);

				for (int i = 0; i < spellnamelist.Count; i++)
				{
					ItemsText[176 + i] = spellnamelist[i];
				}
			}
			else if (mode == SpellNameMadness.Madness)
			{
				List<string> alphabet = new List<string> { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
				List<string> numbers = new List<string> { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };

				for (int i = 176; i < 176 + 64; i++)
				{
					ItemsText[i] = alphabet.PickRandom(rng) + alphabet.PickRandom(rng) + numbers.PickRandom(rng) + alphabet.PickRandom(rng);
				}
			}
		}

		public void EnableSoftInBattle()
		{
			var spellInfos = LoadSpells().ToList();
			var spells = GetSpells().ToDictionary(s => s.Name.ToLowerInvariant());


			foreach (var spl in spells.Where(s => s.Key.StartsWith("soft") || s.Key.StartsWith("sft")).Select(s => s.Value))
			{
				SpellInfo spell = new SpellInfo
				{
					routine = 0x08, //cure ailment
					effect = 0x02, //earth element
					targeting = 0x10, //single target
					accuracy = 00,
					elem = 0,
					gfx = 184,
					palette = 40
				};

				Put(MagicOffset + spl.Index * MagicSize, spell.compressData());
			}
		}



		public void EnableLifeInBattle()
		{
			var spellInfos = LoadSpells().ToList();
			var spells = GetSpells().ToDictionary(s => s.Name.ToLowerInvariant());


			foreach (var spl in spells.Where(s => s.Key.StartsWith("life") || s.Key.StartsWith("lif")).Select(s => s.Value))
			{
				SpellInfo spell = new SpellInfo
				{
					routine = 0x08, //cure ailment
					effect = spl.Name == "LIF2" ? (byte)0x81 : (byte)0x01, //death element
					targeting = 0x10, //single target
					accuracy = 00,
					elem = 0,
					gfx = 224,
					palette = spl.Name == "LIF2" ? (byte)44 : (byte)43,
				};

				Put(MagicOffset + spl.Index * MagicSize, spell.compressData());
			}
		}
	}

	public enum SpellNameMadness
	{
		[Description("None")]
		None,

		[Description("MixedUp")]
		MixedUp,

		[Description("Madness")]
		Madness
	}

}
