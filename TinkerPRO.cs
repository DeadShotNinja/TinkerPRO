using System;
using System.Linq;
using System.Reflection;
using Ensage;
using Ensage.Common;
using Ensage.Common.Menu;
using Ensage.Common.Extensions;
using SharpDX;

namespace TinkerPRO
{
    internal class TinkerPRO : Variables
    {
        public static void Init()
        {
            Options.MenuInit();

            Events.OnLoad += OnLoad;
            Events.OnClose += OnClose;
        }

        private static void OnClose(object sender, EventArgs e)
        {
            Game.OnUpdate -= FullCombo;
            Drawing.OnDraw -= TargetIndicator;
            loaded = false;
            me = null;
            target = null;
        }

        private static void OnLoad(object sender, EventArgs e)
        {
            if (!loaded)
            {
                me = ObjectManager.LocalHero;
                if (!Game.IsInGame || me == null || me.Name != heroName)
                {
                    return;
                }

                loaded = true;
                Game.PrintMessage(
                    "<font face='Calibri Bold'><font color='#04B404'>" + AssemblyName +
                    " loaded.</font> (coded by <font color='#0404B4'>DeadShotNinja</font>) v" + Assembly.GetExecutingAssembly().GetName().Version,
                    MessageType.LogMessage);
                GetAbilities();
                Game.OnUpdate += FullCombo;
                Drawing.OnDraw += TargetIndicator;
            }

            if (me == null || !me.IsValid)
            {
                loaded = false;
            }
        }

        private static void FullCombo(EventArgs args)
        {
            if (!Game.IsInGame || Game.IsPaused || Game.IsWatchingGame || Game.IsChatOpen)
                return;

            // Selects target closest to mouse using user selected range from menu.
            target = me.ClosestToMouseTarget(ClosestToMouseRange.GetValue<Slider>().Value);

            // Full combo
            // Check if Combo key is being held down.
            if (Game.IsKeyDown(comboKey.GetValue<KeyBind>().Key))
            {
                // Gives values to ability and item variables.
                GetAbilities();

                if (target == null || !target.IsValid || !target.IsVisible || target.IsIllusion || !target.IsAlive ||
                    me.IsChanneling() || target.IsInvul() || HasModifiers() || me.Spellbook.Spells.Any(x => x.IsInAbilityPhase)) return;

                //if (!Utils.SleepCheck("TinkerPROcombosleep")) return;

                if (soulring != null && soulring.CanBeCasted() && soulRing.GetValue<bool>())
                    soulring.UseAbility();

                UseBlink();

                if (target.IsMagicImmune() || !MeHasMana())
                {
                    //if (me.IsChanneling() || me.Spellbook.Spells.Any(x => x.IsInAbilityPhase)) return;

                    //UseBlink();
                    // Will orbwalk if key is heald down and enemy is invul. or you are out of mana (less mana than BoTs need).
                    Orbwalk();
                }
                else if (target.NetworkPosition.Distance2D(me) > 650 && target.NetworkPosition.Distance2D(me) < 1000)
                {
                    //if (me.IsChanneling() || me.Spellbook.Spells.Any(x => x.IsInAbilityPhase)) return;

                    //UseBlink();
                    me.Attack(target);
                }
                else if (target.NetworkPosition.Distance2D(me) > 1000)
                {
                    //if (me.IsChanneling() || me.Spellbook.Spells.Any(x => x.IsInAbilityPhase)) return;

                    CastRearm();
                }
                else
                {
                    // Will blink to mouse position.
                    //UseBlink();

                    UseItem(ghost, 1200);

                    // Will not hex if target is hexed or stunned.
                    //if (!target.UnitState.HasFlag(UnitState.Hexed) && !target.UnitState.HasFlag(UnitState.Stunned))
                    UseItem(sheep, sheep.GetCastRange());
                    //Fast FULL Combo
                                        
                    UseItem(veil, veil.GetCastRange());
                    UseItem(ethereal, ethereal.GetCastRange());
                    CastAbility(missile, missile.GetCastRange());
                    CastAbility(laser, laser.GetCastRange());
                    UseDagon();
                    UseItem(shivas, shivas.GetCastRange());

                    // Use rearm after using ALL abilities/items in combo.
                    CastRearm();
                }

                // Use rearm after using ALL abilities/items in combo.
                //CastRearm();

                //Utils.Sleep(150, "TinkerPROcombosleep");

            }
        }

        private static void GetAbilities()
        {
            if (!Utils.SleepCheck("TinkerPROGetAbilities")) return;
            blink = me.FindItem("item_blink");
            soulring = me.FindItem("item_soul_ring");
            sheep = me.FindItem("item_sheepstick");
            veil = me.FindItem("item_veil_of_discord");
            shivas = me.FindItem("item_shivas_guard");
            dagon = me.GetDagon();
            ghost = me.FindItem("item_ghost");
            ethereal = me.FindItem("item_ethereal_blade");
            laser = me.FindSpell("tinker_laser");
            missile = me.FindSpell("tinker_heat_seeking_missile");
            march = me.FindSpell("tinker_march_of_the_machines");
            rearm = me.FindSpell("tinker_rearm");
            Utils.Sleep(1000, "TinkerPROGetAbilities");
        }

        private static bool HasModifiers()
        {
            if (target.HasModifiers(modifiersNames, false) ||
                (bladeMail.GetValue<bool>() && target.HasModifier("modifier_item_blade_mail_reflect")) ||
                !Utils.SleepCheck("TinkerPROHasModifiers"))
                return true;
            Utils.Sleep(100, "TinkerPROHasModifiers");
            return false;
        }

        private static bool MeHasMana()
        {
            //var itemCD = ObjectManager.GetEntities<Item>().Where(x => x.CanBeCasted());
            //foreach (var item in itemCD)
            //{

            //}
            if (
                (sheep != null && sheep.CanBeCasted() && me.Mana > sheep.ManaCost)
                || (dagon != null && dagon.CanBeCasted() && me.Mana > dagon.ManaCost)
                || (laser != null && laser.CanBeCasted() && me.Mana > laser.ManaCost)
                || (missile != null && missile.CanBeCasted() && me.Mana > missile.ManaCost)
                || (veil != null && veil.CanBeCasted() && me.Mana > veil.ManaCost && !target.HasModifier("modifier_item_veil_of_discord_debuff"))
                || (shivas != null && shivas.CanBeCasted() && me.Mana > shivas.ManaCost)
                || (ethereal != null && ethereal.CanBeCasted() && me.Mana > ethereal.ManaCost)
                || (rearm != null && rearm.CanBeCasted() && me.Mana > rearm.ManaCost)
                )
                return true;
            return false;
        }

        private static void TargetIndicator(EventArgs args)
        {
            if (!drawTarget.GetValue<bool>())
            {
                if (circle == null) return;
                circle.Dispose();
                circle = null;
                return;
            }
            if (target != null && target.IsValid && !target.IsIllusion && target.IsAlive && target.IsVisible &&
                me.IsAlive)
            {
                DrawTarget();
            }
            else if (circle != null)
            {
                circle.Dispose();
                circle = null;
            }
        }

        private static void DrawTarget()
        {
            heroIcon = Drawing.GetTexture("materials/ensage_ui/miniheroes/tinker");
            iconSize = new Vector2(HUDInfo.GetHpBarSizeY() * 2);

            if (
                !Drawing.WorldToScreen(target.Position + new Vector3(0, 0, target.HealthBarOffset / 3), out screenPosition))
                return;

            screenPosition += new Vector2(-iconSize.X, 0);
            Drawing.DrawRect(screenPosition, iconSize, heroIcon);

            if (circle == null)
            {
                circle = new ParticleEffect(@"particles\ui_mouseactions\range_finder_tower_aoe.vpcf", target);
                circle.SetControlPoint(2, me.Position);
                circle.SetControlPoint(6, new Vector3(1, 0, 0));
                circle.SetControlPoint(7, target.Position);
            }
            else
            {
                circle.SetControlPoint(2, me.Position);
                circle.SetControlPoint(6, new Vector3(1, 0, 0));
                circle.SetControlPoint(7, target.Position);
            }
        }

        private static void CastAbility(Ability ability, float range)
        {
            if (ability == null || !ability.CanBeCasted() || ability.IsInAbilityPhase ||
                !target.IsValidTarget(range, true, me.NetworkPosition) || target.IsMagicImmune() ||
                !Menu.Item("abilities").GetValue<AbilityToggler>().IsEnabled(ability.Name)) return;            

            if (ability.Name.Contains("missile") && IsFullDebuffed() && Utils.SleepCheck("TinkerPROebsleep"))
            {
                ability.UseAbility();
                return;
            }

            if (ability.Name.Contains("laser"))
            {
                ability.UseAbility(target);
                return;
            }
        }

        private static void CastRearm()
        {
            if (target.NetworkPosition.Distance2D(me) > 1000 && target.NetworkPosition.Distance2D(me) < missile.CastRange && Utils.SleepCheck("TinkerPROrearmSleep"))
            {
                if (missile.CanBeCasted() && !target.IsInvul())
                {
                    missile.UseAbility();
                }
                rearm.UseAbility();
                if (rearm.Level == 1)
                    Utils.Sleep(3010, "TinkerPROrearmSleep");
                if (rearm.Level == 2)
                    Utils.Sleep(1510, "TinkerPROrearmSleep");
                if (rearm.Level == 3)
                    Utils.Sleep(760, "TinkerPROrearmSleep");
                return;
            }

            else if (rearm == null
                || !Menu.Item("abilities").GetValue<AbilityToggler>().IsEnabled(rearm.Name)
                || !rearm.CanBeCasted()
                || me.IsChanneling()
                || !AllOnCooldown()
                || !target.IsAlive
                || !Utils.SleepCheck("TinkerPROrearmSleep")
                )
            {
                return;
            }
            else
            {
                rearm.UseAbility();
                if (rearm.Level == 1)
                    Utils.Sleep(3010, "TinkerPROrearmSleep");
                if (rearm.Level == 2)
                    Utils.Sleep(1510, "TinkerPROrearmSleep");
                if (rearm.Level == 3)
                    Utils.Sleep(760, "TinkerPROrearmSleep");
            }


        }

        private static void UseDagon()
        {
            if (dagon == null
                || !dagon.CanBeCasted()
                || target.IsMagicImmune()
                || !(target.NetworkPosition.Distance2D(me) - target.RingRadius <= dagon.CastRange)
                || !Menu.Item("items").GetValue<AbilityToggler>().IsEnabled("item_dagon")
                || !IsFullDebuffed()
                || !Utils.SleepCheck("TinkerPROebsleep")) return;
            dagon.UseAbility(target);
        }

        private static void UseItem(Item item, float range, int speed = 0)
        {
            if (item == null || !item.CanBeCasted() || target.IsMagicImmune() || target.MovementSpeed < speed ||
                target.HasModifier(item.Name) || me.IsChanneling() || !target.IsValidTarget(range, true, me.NetworkPosition) ||
                me.Spellbook.Spells.Any(x => x.IsInAbilityPhase) || !Menu.Item("items").GetValue<AbilityToggler>().IsEnabled(item.Name))
                return;

            if (item.Name.Contains("veil") && !target.HasModifier("modifier_item_veil_of_discord_debuff"))
            {
                item.UseAbility(target.NetworkPosition);
                return;
            }

            if (item.Name.Contains("ethereal") && IsFullDebuffed())
            {
                item.UseAbility(target);
                Utils.Sleep(me.NetworkPosition.Distance2D(target.NetworkPosition) / 1200 * 1000, "TinkerPROebsleep");
                return;
            }

            if (item.IsAbilityBehavior(AbilityBehavior.UnitTarget) && !item.Name.Contains("item_dagon") && !item.Name.Contains("veil"))
            {
                item.UseAbility(target);
                return;
            }

            if (item.IsAbilityBehavior(AbilityBehavior.Point) && !item.Name.Contains("veil"))
            {
                item.UseAbility(target.NetworkPosition);
                return;
            }

            if (item.IsAbilityBehavior(AbilityBehavior.Immediate) && !item.Name.Contains("veil"))
            {
                item.UseAbility();
            }
        }

        private static bool IsFullDebuffed()
        {
            if (
                (veil != null && veil.CanBeCasted() &&
                 Menu.Item("items").GetValue<AbilityToggler>().IsEnabled(veil.Name) &&
                 !target.HasModifier("modifier_item_veil_of_discord_debuff"))
                ||
                (ethereal != null && ethereal.CanBeCasted() &&
                 Menu.Item("items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name))
                )
                return false;
            return true;
        }

        private static bool AllOnCooldown()
        {
            if (
                (laser.CanBeCasted() && target.NetworkPosition.Distance2D(me) <= laser.CastRange)
                || (missile.CanBeCasted() && target.NetworkPosition.Distance2D(me) <= missile.CastRange)
                || (dagon.CanBeCasted() && target.NetworkPosition.Distance2D(me) <= dagon.CastRange)
                || (sheep.CanBeCasted() && target.NetworkPosition.Distance2D(me) <= sheep.CastRange)
                || (veil.CanBeCasted() && target.NetworkPosition.Distance2D(me) <= veil.CastRange && !target.HasModifier("modifier_item_veil_of_discord_debuff"))
                || (ethereal.CanBeCasted() && target.NetworkPosition.Distance2D(me) <= ethereal.CastRange)
                || (shivas.CanBeCasted() && target.NetworkPosition.Distance2D(me) <= shivas.CastRange)
                )
                return false;
            return true;
        }

        private static void Orbwalk()
        {
            switch (moveMode.GetValue<bool>())
            {
                case true:
                    Orbwalking.Orbwalk(target);
                    break;
                case false:
                    break;
            }
        }

        private static void UseBlink()
        {

            if (!useBlink.GetValue<bool>() || blink == null || !blink.CanBeCasted() ||
                target.Distance2D(me.Position) < 600 //|| !Utils.SleepCheck("TinkerPROblink")
                || me.Distance2D(Game.MousePosition) < 450) return;
            blink.UseAbility(Game.MousePosition);
            Utils.Sleep(200, "TinkerPROblink");
            //Utils.Sleep(500, "TinkerPROblink");
        }
        
        private static float GetDistance2D(Vector3 p1, Vector3 p2)
        {
            return (float)Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }
    }

}
