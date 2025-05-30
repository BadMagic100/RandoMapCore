﻿using ConnectionMetadataInjector;
using ItemChanger;
using RandoMapCore.Settings;
using SD = ConnectionMetadataInjector.SupplementalMetadata;

namespace RandoMapCore.Pins;

internal class PinSpriteManager
{
    private readonly Dictionary<string, ScaledPinSprite> _builtInPinSprites = [];
    private readonly Dictionary<TaggableObject, ScaledPinSprite> _connectionPinSprites = [];

    internal ScaledPinSprite GetLocationSprite(string key)
    {
        if (TryGetBuiltInSpriteKey(key, out var otherKey))
        {
            return GetLocationSprite(otherKey);
        }

        if (RandoMapCoreMod.Data.EnableVisualCustomization)
        {
            if (RandoMapCoreMod.GS.QMarks is QMarkSetting.Red)
            {
                key = key switch
                {
                    "Shop" => "Shop",
                    _ => "Unknown",
                };
            }
            else if (RandoMapCoreMod.GS.QMarks is QMarkSetting.Mixed)
            {
                key = key switch
                {
                    "Grub" => "UnknownGrub",
                    "Cocoon" => "UnknownLifeblood",
                    "Rock" => "UnknownGeoRock",
                    "Totem" => "UnknownTotem",
                    "Shop" => "Shop",
                    _ => "Unknown",
                };
            }
        }

        return GetSpriteInternal(key);
    }

    internal ScaledPinSprite GetSprite(string key)
    {
        if (TryGetBuiltInSpriteKey(key, out var otherKey))
        {
            return GetSpriteInternal(otherKey);
        }

        return GetSpriteInternal(key);
    }

    private ScaledPinSprite GetSpriteInternal(string key)
    {
        if (_builtInPinSprites.TryGetValue(key, out var sprite))
        {
            return sprite;
        }

        sprite = new(key);

        _builtInPinSprites.Add(key, sprite);

        return sprite;
    }

    internal ScaledPinSprite GetLocationSprite(AbstractPlacement placement)
    {
        if (RandoMapCoreMod.Data.EnableVisualCustomization && RandoMapCoreMod.GS.QMarks is not QMarkSetting.Off)
        {
            return GetLocationSprite(SD.Of(placement).Get(InjectedProps.LocationPoolGroup));
        }

        if (_connectionPinSprites.TryGetValue(placement, out var sprite))
        {
            return sprite;
        }

        if (SD.Of(placement).Get(InteropProperties.LocationPinSpriteKey) is string key)
        {
            sprite = GetLocationSprite(key);
        }
        else if (SD.Of(placement).Get(InteropProperties.LocationPinSprite) is ISprite iSprite)
        {
            sprite = new(iSprite, SD.Of(placement).Get(InteropProperties.LocationPinSpriteSize));
        }
        else
        {
            sprite = GetLocationSprite(SD.Of(placement).Get(InjectedProps.LocationPoolGroup));
        }

        _connectionPinSprites.Add(placement, sprite);

        return sprite;
    }

    internal ScaledPinSprite GetSprite(AbstractItem item)
    {
        if (_connectionPinSprites.TryGetValue(item, out var sprite))
        {
            return sprite;
        }

        if (SD.Of(item).Get(InteropProperties.ItemPinSpriteKey) is string key)
        {
            sprite = GetSprite(key);
        }
        else if (SD.Of(item).Get(InteropProperties.ItemPinSprite) is ISprite iSprite)
        {
            sprite = new(iSprite, SD.Of(item).Get(InteropProperties.ItemPinSpriteSize));
        }
        else
        {
            sprite = GetSprite(SD.Of(item).Get(InjectedProps.ItemPoolGroup));
        }

        _connectionPinSprites.Add(item, sprite);

        return sprite;
    }

    private static bool TryGetBuiltInSpriteKey(string poolGroup, out string key)
    {
        key = poolGroup switch
        {
            "Dreamers" => "Dreamer",
            "Skills" => "Skill",
            "Charms" => "Charm",
            "Keys" => "Key",
            "Mask Shards" => "Mask",
            "Vessel Fragments" => "Vessel",
            "Charm Notches" => "Notch",
            "Pale Ore" => "Ore",
            "Geo Chests" => "Geo",
            "Rancid Eggs" => "Egg",
            "Relics" => "Relic",
            "Whispering Roots" => "Root",
            "Boss Essence" => "EssenceBoss",
            "Grubs" => "Grub",
            "Mimics" => "Grub",
            "Maps" => "Map",
            "Stags" => "Stag",
            "Lifeblood Cocoons" => "Cocoon",
            "Grimmkin Flames" => "Flame",
            "Journal Entries" => "Journal",
            "Geo Rocks" => "Rock",
            "Boss Geo" => "Geo",
            "Soul Totems" => "Totem",
            "Lore Tablets" => "Lore",
            "Shops" => "Shop",
            "Levers" => "Lever",
            "Mr Mushroom" => "Lore",
            "Benches" => "Bench",
            "Other" => "Unknown",
            _ => null,
        };

        return key is not null;
    }
}
