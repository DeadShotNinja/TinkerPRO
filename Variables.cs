using System.Collections.Generic;
using Ensage;
using Ensage.Common.Menu;
using SharpDX;

namespace TinkerPRO
{
    internal class Variables
    {
        public const string AssemblyName = "TinkerPRO";

        public static string heroName;

        public static string[] modifiersNames =
        {
            "modifier_medusa_stone_gaze_stone",
            "modifier_winter_wyvern_winters_curse",
            "modifier_item_lotus_orb_active",
            "modifier_nyx_assassin_spiked_carapace",
            "modifier_abaddon_borrowed_time",
            "modifier_naga_siren_song_of_the_siren"
        };

        public static Dictionary<string, bool> abilitiesDictionary = new Dictionary<string, bool>
        {
            {"tinker_rearm", true},
            {"tinker_march_of_the_machines", true},
            {"tinker_heat_seeking_missile", true},
            {"tinker_laser", true}
        };

        public static Dictionary<string, bool> itemsDictionary = new Dictionary<string, bool>
        {
            {"item_sheepstick", true},
            {"item_shivas_guard", true},
            {"item_dagon", true},
            {"item_ethereal_blade", true},
            {"item_ghost", true},
            {"item_veil_of_discord", true}
        };

        public static Menu Menu;

        public static Menu items;

        public static Menu abilities;

        public static Menu targetOptions;

        public static MenuItem comboKey;

        public static MenuItem drawTarget;

        public static MenuItem moveMode;

        public static MenuItem ClosestToMouseRange;

        public static MenuItem soulRing;

        public static MenuItem bladeMail;

        public static MenuItem useBlink;

        public static bool loaded;

        public static Ability rearm, march, missile, laser;

        public static Item soulring, sheep, veil, shivas, dagon, ethereal, blink, ghost;

        public static Hero me, target;

        public static Vector2 iconSize, screenPosition;

        public static DotaTexture heroIcon;

        public static ParticleEffect circle;
    }
}
