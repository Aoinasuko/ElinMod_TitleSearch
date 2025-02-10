

using BepInEx;
using HarmonyLib;

namespace Mod_TitleSearch
{
	/// <summary>
	/// Mod初期化クラス
	/// </summary>
    [BepInPlugin("bep.titlesearch", "TitleSearch", "1.0.0.0")]
    public class Mod_TitleSearch : BaseUnityPlugin
    {
        private void Awake()
        {
            new Harmony("TitleSearch").PatchAll();
        }
    }
}
