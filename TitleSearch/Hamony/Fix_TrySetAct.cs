using HarmonyLib;
using System;
using System.Collections.Generic;

namespace Mod_TitleSearch
{
	/// <summary>
	/// ファクションボードの派閥名選択変更
    /// ファクション名選択前に検索欄を表示して、検索を開始する
    /// </summary>
    [HarmonyPatch(typeof(TraitFactionBoard), nameof(TraitFactionBoard.TrySetAct))]
    [HarmonyPatch(new Type[]
    {
        typeof(ActPlan)
    })]
    internal class Fix_TrySetAct
    {
        [HarmonyPostfix]
        public static void Postfix(ref TraitFactionBoard __instance, ActPlan p)
        {
            TraitFactionBoard instance = __instance;
            string search = "";
            p.TrySetAct("actChangeFactionName".lang() + "(Search)", delegate
            {
                Dialog.InputName("", "", delegate (bool cancel, string text)
                {
                    if (!cancel)
                    {
                        search = text;
                    }
                    EClass.ui.AddLayer<LayerList>().SetStringList(delegate
                    {
                        List<string> list = new List<string>();
                        for (int i = 0; i < 10000; i++)
                        {
                            string textAlias = WordGen.GetCombinedName(instance.GetAlias());
                            if (textAlias.Contains(search))
                            {
                                list.Add(textAlias);
                                if (list.Count >= 10)
                                {
                                    break;
                                }
                            }
                        }
                        if (list.Count < 10)
                        {
                            for (int j = list.Count; j < 10; j++)
                            {
                                list.Add(WordGen.GetCombinedName(instance.GetAlias()));
                            }
                        }
                        return list;
                    }, delegate (int a, string b)
                    {
                        EClass.Home.name = b;
                    }).SetSize().EnableReroll();
                    ;
                });
                return false;
            }, instance.owner);
            return;
        }
    }
}