﻿using System.Collections.ObjectModel;
using MapChanger;
using MapChanger.Map;
using MapChanger.MonoBehaviours;
using TMPro;
using UnityEngine;

namespace RandoMapCore.Rooms;

internal class RmcRoomManager : HookModule
{
    private static Dictionary<string, RoomTextDef> _roomTextDefs;

    internal static MapObject MoRoomTexts { get; private set; }
    internal static ReadOnlyDictionary<string, RoomText> RoomTexts { get; private set; }
    internal static RoomSelector Selector { get; private set; }

    public override void OnEnterGame()
    {
        _roomTextDefs = JsonUtil
            .DeserializeFromAssembly<RoomTextDef[]>(RandoMapCoreMod.Assembly, "RandoMapCore.Resources.roomTexts.json")
            .Where(r => !Finder.IsMappedScene(r.SceneName))
            .ToDictionary(r => r.SceneName, r => r);

        if (!MapChanger.Dependencies.HasAdditionalMaps)
        {
            return;
        }

        foreach (
            var rtd in JsonUtil.DeserializeFromAssembly<RoomTextDef[]>(
                RandoMapCoreMod.Assembly,
                "RandoMapCore.Resources.roomTextsAM.json"
            )
        )
        {
            if (_roomTextDefs.ContainsKey(rtd.SceneName))
            {
                _roomTextDefs[rtd.SceneName] = rtd;
            }
        }
    }

    public override void OnQuitToMenu()
    {
        _roomTextDefs = null;
        MoRoomTexts = null;
        RoomTexts = null;
        Selector = null;
    }

    internal static void Make(GameObject goMap)
    {
        MoRoomTexts = Utils.MakeMonoBehaviour<MapObject>(goMap, "Room Texts");
        MoRoomTexts.Initialize();

        var tmpFont = goMap.transform.Find("Cliffs").Find("Area Name (1)").GetComponent<TextMeshPro>().font;

        Dictionary<string, RoomText> roomTexts = [];
        foreach (var rtd in _roomTextDefs.Values)
        {
            var roomText = Utils.MakeMonoBehaviour<RoomText>(null, $"Room Text {rtd.SceneName}");
            roomText.Initialize(rtd, tmpFont);
            MoRoomTexts.AddChild(roomText);
            roomTexts[rtd.SceneName] = roomText;
        }

        RoomTexts = new(roomTexts);

        MapObjectUpdater.Add(MoRoomTexts);

        List<ISelectable> rooms = [.. BuiltInObjects.SelectableRooms.Values.Cast<ISelectable>()];
        rooms.AddRange(RoomTexts.Values.Cast<ISelectable>());

        if (RandoMapCoreMod.Data.EnableRoomSelection)
        {
            Selector = Utils.MakeMonoBehaviour<RoomSelector>(null, "RandoMapCore Transition Room Selector");
            Selector.Initialize(rooms);
        }
    }

    internal static void Update()
    {
        Selector?.MainUpdate();
    }
}
