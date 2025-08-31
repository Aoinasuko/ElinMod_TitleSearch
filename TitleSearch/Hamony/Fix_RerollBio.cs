using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mod_TitleSearch
{

	/// <summary>
    /// ID登録用クラス
    /// </summary>
    public class IDName
    {
        public int id = -1;
        public int idAdv = -1;

        public IDName(int id, int idAdv)
        {
            this.id = id;
            this.idAdv = idAdv;
        }
    }

	/// <summary>
	/// キャラメイク時の生まれリロール変更
    /// リロール時に固定入力できるようにする
    /// </summary>
    [HarmonyPatch(typeof(UICharaMaker), nameof(UICharaMaker.RerollBio))]
    [HarmonyPatch(new Type[]
    {
    })]
    internal class Fix_RerollBio
    {
        [HarmonyPrefix]
        public static bool Prefix(ref UICharaMaker __instance)
        {
            UICharaMaker instance = __instance;
			Chara chara = __instance.chara;
            Biography bio = __instance.chara.bio;
            string desc = "";
            // 父親元ID
            IDName id_dad = new IDName(bio.idDad, bio.idAdvDad);
            IDName id_mom = new IDName(bio.idMom, bio.idAdvMom);
            IDName id_loc = new IDName(bio.idHome, bio.idLoc);

            if (Lang.isJP)
            {
                desc = "誕生日の月は？";
            } else
            {
                desc = "What is your birthday month?";
            }
            Dialog.InputName(desc, "", delegate (bool cancel_month, string text_month)
            {
                if (!cancel_month)
                {
                    if (ValCheck(text_month, 1, out int retbirthMonth))
                    {
                        bio.birthMonth = retbirthMonth;
                        instance.Refresh();
                    }
                    if (Lang.isJP)
                    {
                        desc = "誕生日の日は？";
                    }
                    else
                    {
                        desc = "What is your birthday day?";
                    }
                    Dialog.InputName(desc, "", delegate (bool cancel_day, string text_day)
                    {
                        if (!cancel_day)
                        {
                            if (ValCheck(text_day, 2, out int retbirthDay))
                            {
                                bio.birthDay = retbirthDay;
                                instance.Refresh();
                            }
                            if (Lang.isJP)
                            {
                                desc = "年齢は？";
                            }
                            else
                            {
                                desc = "What is your age?";
                            }
                            Dialog.InputName(desc, "", delegate (bool cancel_age, string text_age)
                            {
                                if (!cancel_age)
                                {
                                    if (ValCheck(text_age, 0, out int retbirthAge))
                                    {
                                        bio.SetAge(chara, retbirthAge);
                                        instance.Refresh();
                                    }
                                    if (Lang.isJP)
                                    {
                                        desc = "身長は？";
                                    }
                                    else
                                    {
                                        desc = "What is your height?";
                                    }
                                    Dialog.InputName(desc, "", delegate (bool cancel_height, string text_height)
                                    {
                                        if (!cancel_height)
                                        {
                                            if (ValCheck(text_height, 0, out int retHeight))
                                            {
                                                bio.height = retHeight;
                                                instance.Refresh();
                                            }
                                            if (Lang.isJP)
                                            {
                                                desc = "体重は？";
                                            }
                                            else
                                            {
                                                desc = "What is your weight?";
                                            }
                                            Dialog.InputName(desc, "", delegate (bool cancel_weight, string text_weight)
                                            {
                                                if (!cancel_weight)
                                                {
                                                    if (ValCheck(text_weight, 0, out int retWeight))
                                                    {
                                                        bio.weight = retWeight;
                                                        instance.Refresh();
                                                    }
                                                    // 父親
                                                    if (Lang.isJP)
                                                    {
                                                        desc = "父親は？";
                                                    }
                                                    else
                                                    {
                                                        desc = "What is your father?";
                                                    }
                                                    Dialog.InputName(desc, "", delegate (bool cancel_father, string text_father)
                                                    {
                                                        string search_father = "";
                                                        if (!cancel_father)
                                                        {
                                                            search_father = text_father;
                                                            List<string> list_father_name = new List<string>();
                                                            List<IDName> list_father_id = new List<IDName>();
                                                            // ロール検索
                                                            EMono.ui.AddLayer<LayerList>().SetStringList(delegate
                                                            {
                                                                list_father_name = new List<string>();
                                                                list_father_id = new List<IDName>();
                                                                for (int i = 0; i < 10000; i++)
                                                                {
                                                                    bio.GenerateDad();
                                                                    int idDad = bio.idDad;
                                                                    int idAdvDad = bio.idAdvDad;
                                                                    string textDadName = bio.nameDad;
                                                                    if (textDadName.Contains(search_father))
                                                                    {
                                                                        list_father_name.Add(textDadName);
                                                                        list_father_id.Add(new IDName(idDad, idAdvDad));
                                                                        if (list_father_name.Count >= 10)
                                                                        {
                                                                            break;
                                                                        }
                                                                    }
                                                                }
                                                                if (list_father_name.Count < 10)
                                                                {
                                                                    for (int j = list_father_name.Count; j < 10; j++)
                                                                    {
                                                                        bio.GenerateDad();
                                                                        int idDad = bio.idDad;
                                                                        int idAdvDad = bio.idAdvDad;
                                                                        string textDadName = bio.nameDad;
                                                                        list_father_name.Add(textDadName);
                                                                        list_father_id.Add(new IDName(idDad, idAdvDad));
                                                                    }
                                                                }
                                                                IDName retDad = id_dad;
                                                                bio.idDad = retDad.id;
                                                                bio.idAdvDad = retDad.idAdv;
                                                                return list_father_name;
                                                            }, delegate (int a, string b)
                                                            {
                                                                IDName retDad = list_father_id.ElementAt(a);
                                                                bio.idDad = retDad.id;
                                                                bio.idAdvDad = retDad.idAdv;
                                                                instance.Refresh();
                                                                // 母親
                                                                if (Lang.isJP)
                                                                {
                                                                    desc = "母親は？";
                                                                }
                                                                else
                                                                {
                                                                    desc = "What is your mother?";
                                                                }
                                                                Dialog.InputName(desc, "", delegate (bool cancel_mother, string text_mother)
                                                                {
                                                                    string search_mother = "";
                                                                    if (!cancel_mother)
                                                                    {
                                                                        search_mother = text_mother;
                                                                        List<string> list_mother_name = new List<string>();
                                                                        List<IDName> list_mother_id = new List<IDName>();
                                                                        // ロール検索
                                                                        EMono.ui.AddLayer<LayerList>().SetStringList(delegate
                                                                        {
                                                                            list_mother_name = new List<string>();
                                                                            list_mother_id = new List<IDName>();
                                                                            for (int i = 0; i < 10000; i++)
                                                                            {
                                                                                bio.GenerateMom();
                                                                                int idMom = bio.idMom;
                                                                                int idAdvMom = bio.idAdvMom;
                                                                                string textMomName = bio.nameMom;
                                                                                if (textMomName.Contains(search_mother))
                                                                                {
                                                                                    list_mother_name.Add(textMomName);
                                                                                    list_mother_id.Add(new IDName(idMom, idAdvMom));
                                                                                    if (list_mother_name.Count >= 10)
                                                                                    {
                                                                                        break;
                                                                                    }
                                                                                }
                                                                            }
                                                                            if (list_mother_name.Count < 10)
                                                                            {
                                                                                for (int j = list_mother_name.Count; j < 10; j++)
                                                                                {
                                                                                    bio.GenerateMom();
                                                                                    int idMom = bio.idMom;
                                                                                    int idAdvMom = bio.idAdvMom;
                                                                                    string textMomName = bio.nameMom;
                                                                                    list_mother_name.Add(textMomName);
                                                                                    list_mother_id.Add(new IDName(idMom, idAdvMom));
                                                                                }
                                                                            }
                                                                            IDName retMom = id_mom;
                                                                            bio.idMom = retMom.id;
                                                                            bio.idAdvMom = retMom.idAdv;
                                                                            return list_mother_name;
                                                                        }, delegate (int c, string d)
                                                                        {
                                                                            IDName retMom = list_mother_id.ElementAt(c);
                                                                            bio.idMom = retMom.id;
                                                                            bio.idAdvMom = retMom.idAdv;
                                                                            instance.Refresh();
                                                                            // 生まれた場所
                                                                            if (Lang.isJP)
                                                                            {
                                                                                desc = "生まれた場所は？";
                                                                            }
                                                                            else
                                                                            {
                                                                                desc = "What is your birth location?";
                                                                            }
                                                                            Dialog.InputName(desc, "", delegate (bool cancel_birth, string text_birth)
                                                                            {
                                                                                string search_birth = "";
                                                                                if (!cancel_birth)
                                                                                {
                                                                                    search_birth = text_birth;
                                                                                    List<string> list_birth_name = new List<string>();
                                                                                    List<IDName> list_birth_id = new List<IDName>();
                                                                                    // ロール検索
                                                                                    EMono.ui.AddLayer<LayerList>().SetStringList(delegate
                                                                                    {
                                                                                        list_birth_name = new List<string>();
                                                                                        list_birth_id = new List<IDName>();
                                                                                        for (int i = 0; i < 10000; i++)
                                                                                        {
                                                                                            bio.idHome = WordGen.GetID("home");
                                                                                            bio.idLoc = WordGen.GetID("loc");
                                                                                            int idHome = bio.idHome;
                                                                                            int idLoc = bio.idLoc;
                                                                                            string textLocName = bio.nameBirthplace;
                                                                                            if (textLocName.Contains(search_birth))
                                                                                            {
                                                                                                list_birth_name.Add(textLocName);
                                                                                                list_birth_id.Add(new IDName(idHome, idLoc));
                                                                                                if (list_birth_name.Count >= 10)
                                                                                                {
                                                                                                    break;
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                        if (list_birth_name.Count < 10)
                                                                                        {
                                                                                            for (int j = list_birth_name.Count; j < 10; j++)
                                                                                            {
                                                                                                bio.idHome = WordGen.GetID("home");
                                                                                                bio.idLoc = WordGen.GetID("loc");
                                                                                                int idHome = bio.idHome;
                                                                                                int idLoc = bio.idLoc;
                                                                                                string textLocName = bio.nameBirthplace;
                                                                                                list_birth_name.Add(textLocName);
                                                                                                list_birth_id.Add(new IDName(idHome, idLoc));
                                                                                            }
                                                                                        }
                                                                                        IDName retLoc = id_loc;
                                                                                        bio.idHome = retLoc.id;
                                                                                        bio.idLoc = retLoc.idAdv;
                                                                                        return list_birth_name;
                                                                                    }, delegate (int e, string f)
                                                                                    {
                                                                                        IDName retLoc = list_birth_id.ElementAt(e);
                                                                                        bio.idHome = retLoc.id;
                                                                                        bio.idLoc = retLoc.idAdv;
                                                                                        instance.Refresh();
                                                                                    }).SetSize()
                                                                                    .EnableReroll();
                                                                                    EMono.ui.langHint = "lore_unknown".lang();
                                                                                }
                                                                            });
                                                                        }).SetSize()
                                                                        .EnableReroll();
                                                                    }
                                                                });
                                                            }).SetSize()
                                                            .EnableReroll();
                                                        }
                                                    });
                                                }
                                            });
                                        }
                                    });
                                }
                            });
                        }
                    });
                }
            });
            return false;
        }

        /// <summary>
        /// 値チェック
		/// - type引数 -
        /// 0:数値か
        /// 1:数値か(1～12)
        /// 2:数値か(1～30）
        /// </summary>
        public static bool ValCheck(string text, int type, out int outvalue)
        {
            if (type == 0)
            {
                if (int.TryParse(text, out int value))
                {
                    outvalue = value;
                    return true;
                }
            }
            if (type == 1)
            {
                if (int.TryParse(text, out int value))
                {
                    if (value >= 1 && value <= 12)
                    {
                        outvalue = value;
                        return true;
                    }
                }
            }
            if (type == 2)
            {
                if (int.TryParse(text, out int value))
                {
                    if (value >= 1 && value <= 30)
                    {
                        outvalue = value;
                        return true;
                    }
                }
            }
            outvalue = -1;
            return false;
        }
    }
}