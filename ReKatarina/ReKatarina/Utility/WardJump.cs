﻿using EloBuddy;
using EloBuddy.SDK;
using System;
using System.Linq;
using EloBuddy.SDK.Enumerations;
using System.Collections.Generic;

namespace ReKatarina.Utility
{
    public static class WardJump
    {
        private static int LastWardCast;
        public static List<ItemId> wards = new List<ItemId>();
        private static InventorySlot GetWardSlot()
        {
            wards.Clear();
            //if (ConfigList.WardJump.UseWardingTotem)
                wards.Add(ItemId.Warding_Totem_Trinket);
            //if (ConfigList.WardJump.UseVisionWard)
                wards.Add(ItemId.Vision_Ward);
            //if (ConfigList.WardJump.UseTrackersKnife)
                wards.Add(ItemId.Trackers_Knife);
            //if (ConfigList.WardJump.UseSightstone)
                wards.Add(ItemId.Sightstone);
            /*if (ConfigList.WardJump.UseRubySightstone)
                wards.Add(ItemId.Ruby_Sightstone);
            if (ConfigList.WardJump.UseEyeoftheWatchers)
                wards.Add(ItemId.Eye_of_the_Watchers);
            if (ConfigList.WardJump.UseEyeoftheOasis)
                wards.Add(ItemId.Eye_of_the_Oasis);
            if (ConfigList.WardJump.UseEyeoftheEquinox)
                wards.Add(ItemId.Eye_of_the_Equinox);*/
            
            //wards.Add(ItemId.Warding_Totem_Trinket);

            return Player.Instance.InventoryItems.FirstOrDefault(use => wards.Contains(use.Id) && use.CanUseItem() && use.IsWard);
        }
        public static void Execute()
        {
            if (!SpellManager.E.IsReady() || !SpellManager.E.IsLearned)
                return;

            Flee.Execute();
            if (Environment.TickCount - SpellManager.LastJumpCast < 500)
                return;

            var mouse = Game.CursorPos;

            Orbwalker.OrbwalkTo(mouse);

            var wardSlot = GetWardSlot();
            if (wardSlot != null && SpellManager.E.IsReady() && Environment.TickCount - LastWardCast > 1500)
            {
                var pos = mouse;
                if (mouse.Distance(Player.Instance.Position) > SpellManager.E.Range)
                    pos = Player.Instance.Position.Extend(mouse, SpellManager.E.Range).To3D();
                mouse = pos;

                wardSlot.Cast(mouse);
                var ward = ObjectManager.Get<Obj_AI_Base>().
                            Where(a => a.IsAlly && a.IsInRange(a, SpellManager.E.Range) && a.Distance(mouse) <= 150);
                Core.DelayAction(() => SpellManager.E.Cast(ward.FirstOrDefault()), 100 + Damage.GetAditionalDelay());
                LastWardCast = Environment.TickCount;
            }
        }
    }
}