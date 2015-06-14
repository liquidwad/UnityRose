// <copyright file="RosePlayer.cs" company="Wadii Bellamine">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Wadii Bellamine</author>
// <date>6/13/2015 7:01 PM </date>
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityRose.Formats;
using System.IO;
using UnityRose;

#if UNITY_EDITOR
    using UnityEditor;
#endif


namespace UnityRose
{
    /// <summary>
    /// This singleton class loads common resources into memory for quick access.
    /// Avoids having to reload certain resources that are almost always needed.
    /// For example ZSC files, every time a piece of armor is equipped or appears 
    /// on another player.
    /// 
    /// TODO: This class should be improved to cache most common equipment.
    /// </summary>
    public class ResourceManager
    {
        // Male ZSC's (equipment model links)
        public ZSC zsc_body_male;
        public ZSC zsc_arms_male;
        public ZSC zsc_foot_male;
        public ZSC zsc_face_male;
        public ZSC zsc_hair_male;
        public ZSC zsc_cap_male;

        // Female ZSC's
        public ZSC zsc_body_female;
        public ZSC zsc_arms_female;
        public ZSC zsc_foot_female;
        public ZSC zsc_face_female;
        public ZSC zsc_hair_female;
        public ZSC zsc_cap_female;

        // Unisex ZSC's
        public ZSC zsc_back;
        public ZSC zsc_faceItem;

        // ZMD's (skeleton)
        public ZMD zmd_male;
        public ZMD zmd_female;

        // STB's
        public static STB stb_animation_list;
        public static STB stb_animation_type;
        // TODO: add any other common persistent resources here


        public static ResourceManager instance;

        private ResourceManager()
        {
            zsc_body_male = new ZSC("3DData/Avatar/LIST_MBODY.zsc");
            zsc_arms_male = new ZSC("3DData/Avatar/LIST_MARMS.zsc");
            zsc_foot_male = new ZSC("3DData/Avatar/LIST_MFOOT.zsc");
            zsc_face_male = new ZSC("3DData/Avatar/LIST_MFACE.zsc");
            zsc_hair_male = new ZSC("3DData/Avatar/LIST_MHAIR.zsc");
            zsc_cap_male = new ZSC("3DData/Avatar/LIST_MCAP.zsc");

            zsc_body_female = new ZSC("3DData/Avatar/LIST_WBODY.zsc");
            zsc_arms_female = new ZSC("3DData/Avatar/LIST_WARMS.zsc");
            zsc_foot_female = new ZSC("3DData/Avatar/LIST_WFOOT.zsc");
            zsc_face_female = new ZSC("3DData/Avatar/LIST_WFACE.zsc");
            zsc_hair_female = new ZSC("3DData/Avatar/LIST_WHAIR.zsc");
            zsc_cap_female = new ZSC("3DData/Avatar/LIST_WCAP.zsc");

            zsc_back = new ZSC("3DData/Avatar/LIST_BACK.zsc");
            zsc_faceItem = new ZSC("3DData/Avatar/LIST_FACEIEM.zsc");

            zmd_male = new ZMD("3DData/Avatar/MALE.ZMD");
            zmd_female = new ZMD("3DData/Avatar/FEMALE.ZMD");

            stb_animation_list = new STB("Assets/3Ddata/STB/FILE_MOTION.STB");
            stb_animation_type = new STB("Assets/3DDATA/STB/TYPE_MOTION.STB");


        }

        public static ResourceManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new ResourceManager();

                return instance;
            }
        }

        /// TODO:
        /// Add cache manager accesors
        
        /// TODO: 
        /// Add functions to populate given character GameObject reference with loaded ZMS and Skeleton structures
        /// from cache and prefabs.  No editor-only functions are allowed here because this will be used in runtime
        /// This will be used to change armor parts during runtime

#if UNITY_EDITOR

        /// TODO:
        /// Add a function which loops through all weapon types for each gender and creates an animation
        /// object will all associated animation clips then saves this as a prefab in the Assets folder.
        /// public void GenerateAllAnimationPrefabs()



        /// <summary>
        /// Loads all animations for equiped weapon type. Used only in editor to generate prefabs
        /// </summary>
        /// <param name="WeaponType"></param>
        public void LoadAnimations(GameObject player, ZMD skeleton, WeaponType weapon, GenderType gender)
        {
            List<AnimationClip> clips = new List<AnimationClip>();

            foreach (ActionType action in Enum.GetValues(typeof(ActionType)))
            {
                // Attempt to find animation asset, and if not found, load from ZMO
                string zmoPath = Utils.FixPath(ResourceManager.Instance.GetZMOPath(weapon, action, gender));
                AnimationClip clip = R2U.GetClip(zmoPath, skeleton, action.ToString());
                clip.legacy = true;
                clips.Add(clip);
            }

            Animation animation = player.GetComponent<Animation>();
            AnimationUtility.SetAnimationClips(animation, clips.ToArray());
        }
#endif

        /// <summary>
        /// Get Animation ZMO File path
        /// </summary>
        /// <param name="WeaponType">Equiped Weapon</param>
        /// <param name="Animation">Player Action</param>
        /// <param name="Gender">Player Gender</param>
        /// <returns> The file path of the given animation </returns>
        public string GetZMOPath(WeaponType WeaponType, ActionType Action, GenderType Gender)
        {
            string filePath = stb_animation_list.Cells[int.Parse(stb_animation_type.Cells[(int)Action][(int)WeaponType])][(int)Gender];

            //if no female animation then use male one
            if (filePath == "")
                filePath = stb_animation_list.Cells[int.Parse(stb_animation_type.Cells[(int)Action][(int)WeaponType])][(int)GenderType.MALE];


            return filePath;
        }

    }
}
