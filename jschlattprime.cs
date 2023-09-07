using UMM;
using UnityEngine;
using HarmonyLib;
using System.IO;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace Jschlatt
{
    [UKPlugin("imnotsimon.JschlattPrime","Jschlatt Prime", "1.0.0", "Replaces ALL of Minos Prime's voicelines with Jschlatt AI renditions. \nOriginal audio: https://www.youtube.com/watch?v=UHCmyUFsFqw", true, false)]
    public class Jschlatt : UKMod
    {
        private static Harmony harmony;

        internal static AssetBundle JschlattAssetBundle;

        public override void OnModLoaded()
        {
            Debug.Log("jschlatt prime starting");

            //load the asset bundle
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "jschlattprime";
            {
                JschlattAssetBundle = AssetBundle.LoadFromFile(Path.Combine(ModPath(), resourceName));
            }

            //start harmonylib to swap assets
            harmony = new Harmony("imnotsimon.JschlattPrime");
            harmony.PatchAll();
        }

        public static string ModPath()
        {
            return Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.LastIndexOf(Path.DirectorySeparatorChar));
        }

        public override void OnModUnload()
        {
            harmony.UnpatchSelf();
            base.OnModUnload();
        }

        private static SubtitledAudioSource.SubtitleDataLine MakeLine(string subtitle, float time){
            var sub = new SubtitledAudioSource.SubtitleDataLine();
            sub.subtitle = subtitle;
            sub.time = time;
            return sub;
        }

        //replace minos prime data
        [HarmonyPatch(typeof(MinosPrime), "Start")]
        internal class Patch01
        {
            static void Postfix(MinosPrime __instance){
                Debug.Log("Replacing minos voice lines");

                AudioClip[] dropkickLines = new AudioClip[1];
                dropkickLines[0] = JschlattAssetBundle.LoadAsset<AudioClip>("jp_judgement.wav");
                __instance.dropkickVoice = dropkickLines;

                AudioClip[] comboLines = new AudioClip[1];
                comboLines[0] = JschlattAssetBundle.LoadAsset<AudioClip>("jp_prepare.wav");
                __instance.comboVoice = comboLines;

                AudioClip[] boxingLines = new AudioClip[1];
                boxingLines[0] = JschlattAssetBundle.LoadAsset<AudioClip>("jp_thyend.wav");
                __instance.boxingVoice = boxingLines;

                AudioClip[] riderkickLines = new AudioClip[1];
                riderkickLines[0] = JschlattAssetBundle.LoadAsset<AudioClip>("jp_die.wav");
                __instance.riderKickVoice = riderkickLines;

                AudioClip[] dropAttackLines = new AudioClip[1];
                dropAttackLines[0] = JschlattAssetBundle.LoadAsset<AudioClip>("jp_crush.wav");
                __instance.dropAttackVoice = dropAttackLines;

                __instance.phaseChangeVoice = JschlattAssetBundle.LoadAsset<AudioClip>("jp_weak.wav");
            }
        }


        //use map info to inject data
        [HarmonyPatch(typeof(StockMapInfo), "Awake")]
        internal class Patch03
        {
            static void Postfix(StockMapInfo __instance)
            {
                //try to find dialog in scene and replace it
                foreach (var source in Resources.FindObjectsOfTypeAll<AudioSource>())
                {
                    if (source.clip)
                    {
                        bool replaced = false;
                        var subtitles = new List<SubtitledAudioSource.SubtitleDataLine>();
                        if (source.clip.GetName() == "mp_intro2")
                        {
                            Debug.Log("Replacing minos intro");
                            source.clip = JschlattAssetBundle.LoadAsset<AudioClip>("jp_intro.wav");
                        }
                        else if (source.clip.GetName() == "mp_outro")
                        {
                            Debug.Log("Replacing minos outro");
                            source.clip = JschlattAssetBundle.LoadAsset<AudioClip>("jp_outro.wav");
                        }
                        else if (source.clip.GetName() == "mp_deathscream")
                        {
                            Debug.Log("Replacing death scream");
                            source.clip = JschlattAssetBundle.LoadAsset<AudioClip>("jp_deathscream.wav");
                        }
                        else if (source.clip.GetName() == "mp_useless")
                        {
                            Debug.Log("Replacing useless");
                            source.clip = JschlattAssetBundle.LoadAsset<AudioClip>("jp_useless.wav");
                            replaced = true;
                        }
                    }

                }//replace boss names
            }
                [HarmonyPatch(typeof(BossHealthBar), "Awake")]
        internal class Patch04
        {
            static void Prefix(BossHealthBar __instance)
            {
                if(__instance.bossName == "MINOS PRIME"){
                    __instance.bossName = "JSCHLATT PRIME";
                }
            }
        }
     }
  }
}
