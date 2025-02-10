using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mod_TitleSearch
{
	/// <summary>
	/// キャラメイク時の異名選択変更
    /// 異名選択前に検索欄を表示して、検索を開始する
    /// </summary>
    [HarmonyPatch(typeof(UICharaMaker), nameof(UICharaMaker.ListAlias))]
    internal class Fix_ListAlias
    {
        [HarmonyPrefix]
        public static bool Prefix(ref UICharaMaker __instance)
        {
            UICharaMaker instance = __instance;
            string search = "";
            Dialog.InputName("", "", delegate (bool cancel, string text)
            {
                if (!cancel)
                {
                    search = text;
                }
                EMono.ui.AddLayer<LayerList>().SetStringList(delegate
                {
                    List<string> list = new List<string>();
                    for (int i = 0; i < 10000; i++)
                    {
                        string textAlias = AliasGen.GetRandomAlias();
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
                            list.Add(AliasGen.GetRandomAlias());
                        }
                    }
                    return list;
                }, delegate (int a, string b)
                {
                    instance.chara._alias = b;
                    instance.Refresh();
                }).SetSize()
                .EnableReroll()
                .SetTitles("wAlias");
            });
            return false;
        }
    }
}