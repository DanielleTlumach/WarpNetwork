﻿using AeroCore;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using System.Collections.Generic;
using WarpNetwork.models;

namespace WarpNetwork
{
    [ModInit]
    class ObeliskPatch
    {
        private static readonly Dictionary<string, Point> ObeliskTargets = new()
        {
            { "Farm", new Point(48, 7) },
            { "IslandSouth", new Point(11, 11) },
            { "Mountain", new Point(31, 20) },
            { "Beach", new Point(20, 4) },
            { "Desert", new Point(35, 43) }
        };
        internal static void Init()
        {
            ModEntry.helper.Events.Player.Warped += MoveAfterWarp;
        }
        public static void MoveAfterWarp(object sender, WarpedEventArgs ev)
        {
            if (ev.IsLocalPlayer)
            {
                string Name = (ev.NewLocation.Name == "BeachNightMarket") ? "Beach" : ev.NewLocation.Name;
                if (Name == "Desert")
                {
                    //desert warp patch

                    Point point = ObeliskTargets["Desert"];
                    if (point != ev.Player.getTileLocationPoint())
                        return;

                    Point to = point;
                    if (WarpHandler.DesertWarp.Value is not null)
                        to = (Point)WarpHandler.DesertWarp.Value;
                    else if (ModEntry.config.PatchObelisks)
                        if (Utils.GetWarpLocations().TryGetValue("desert", out var dest) && dest.OverrideMapProperty)
                            to = new(dest.X, dest.Y);
                        else
                            to = ev.NewLocation.GetMapPropertyPosition("WarpNetworkEntry", point.X, point.Y);
                    WarpHandler.DesertWarp.Value = null;
                    ev.Player.setTileLocation(new Vector2(to.X, to.Y));
                }
                else if (ModEntry.config.PatchObelisks)
                {
                    if (ObeliskTargets.ContainsKey(Name))
                    {
                        Point point = ObeliskTargets[Name];
                        if (Name == "Farm")
                            point = Utils.GetActualFarmPoint(point.X, point.Y);
                        if (ev.Player.getTileLocationPoint() == point)
                        {
                            Dictionary<string, WarpLocation> dests = Utils.GetWarpLocations();
                            string target = (Name == "IslandSouth") ? "island" : Name;
                            Point to = (dests.TryGetValue(target, out var dest) && dest.OverrideMapProperty) ?
                                new(dest.X, dest.Y) : ev.NewLocation.GetMapPropertyPosition("WarpNetworkEntry", point.X, point.Y);
                            ev.Player.setTileLocation(new Vector2(to.X, to.Y));
                        }
                    }
                }
            }
        }
    }
}
