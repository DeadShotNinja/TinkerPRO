using Ensage.Common.Menu;

namespace TinkerPRO
{
    internal class Options : Variables
    {
        public static void MenuInit()
        {
            heroName = "npc_dota_hero_tinker";
            Menu = new Menu(AssemblyName, AssemblyName, true, heroName, true);
            comboKey = new MenuItem("comboKey", "Combo Key").SetValue(new KeyBind(70, KeyBindType.Press)).SetTooltip("Full combo in logical order.");
            useBlink = new MenuItem("useBlink", "Use Blink Dagger").SetValue(false).SetTooltip("Will auto blink with combo key held down.");
            soulRing = new MenuItem("soulRing", "Soulring").SetValue(true).SetTooltip("Will use soul ring before combo.");
            bladeMail = new MenuItem("bladeMail", "Check for BladeMail").SetValue(false).SetTooltip("Will not combo if target used blademail.");
            drawTarget = new MenuItem("drawTarget", "Target indicator").SetValue(true).SetTooltip("Shows red circle around your target.");
            moveMode = new MenuItem("moveMode", "Orbwalk").SetValue(true).SetTooltip("Will orbwalk to mouse while combo key is held down.");
            ClosestToMouseRange = new MenuItem("ClosestToMouseRange", "Closest to mouse range").SetValue(new Slider(800, 500, 1200)).SetTooltip("Will look for enemy in selected range around your mouse pointer.");
                        
            items = new Menu("Items", "Items");
            abilities = new Menu("Abilities", "Abilities");
            targetOptions = new Menu("Target Options", "Target Options");

            Menu.AddItem(comboKey);

            Menu.AddSubMenu(items);
            Menu.AddSubMenu(abilities);
            Menu.AddSubMenu(targetOptions);

            items.AddItem(new MenuItem("items", "Items").SetValue(new AbilityToggler(itemsDictionary)));
            items.AddItem(useBlink);
            items.AddItem(soulRing);
            items.AddItem(bladeMail);
            abilities.AddItem(new MenuItem("abilities", "Abilities").SetValue(new AbilityToggler(abilitiesDictionary)));
            targetOptions.AddItem(moveMode);
            targetOptions.AddItem(ClosestToMouseRange);
            targetOptions.AddItem(drawTarget);

            Menu.AddToMainMenu();
        }

    }
}