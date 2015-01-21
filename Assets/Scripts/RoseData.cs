using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityRose.Formats;

namespace UnityRose

{
    public static class RoseData
    {


        //add rest of stb's here
        public static STB g_TblAnimationList;
        public static STB g_TblAnimationType;


        public  enum AniWeaponType
        {
            EMPTY = 1,
            OHSWORD,
            OHAXE,
            OHMACE,
            OHTOOL,
            THSWORD,
            THSPEAR,
            DSW,
            THBLUNT,
            CANNON,
            BOW,
            XBOX,
            GUN,
            STAFF,
            WAND,
            BOOK,
            KATAR,
            SHIELD,
        };


        public  enum AniAction
        {
            STANDING=0,
            TIRED,
            WALK,
            RUN,
            SIT,
            SITTING,
            STANDUP,
            WARNING,
            ATTACK1,
            ATTACK2,
            ATTACK3,
            HIT,
            FALL,
            DIE,
            RAISE,
            JUMP1,
            JUMP2,
            PICKUP,
        };

        public  enum AniGender
        {
            MALE=1,
            FEMALE,
        };

 

 
        /// <summary>
        /// Load Game Motions
        /// </summary>
        public static void LoadSTB()
        {
            //Load Motion Files
            g_TblAnimationList = new STB("Assets/3Ddata/STB/FILE_MOTION.STB");
            g_TblAnimationType = new STB("Assets/3DDATA/STB/TYPE_MOTION.STB");


            string File = GetAnimationFile(AniWeaponType.KATAR, AniAction.ATTACK1, AniGender.FEMALE);
        }

        /// <summary>
        /// Get Animation ZMO File path
        /// </summary>
        /// <param name="WeaponType">Equiped Weapon</param>
        /// <param name="Animation">Player Action</param>
        /// <param name="Gender">Player Gender</param>
        /// <returns></returns>
        public static string GetAnimationFile(AniWeaponType WeaponType, AniAction Action, AniGender Gender)
        {
            string filePath = g_TblAnimationList.Cells[int.Parse(g_TblAnimationType.Cells[(int)Action][(int)WeaponType])][(int)Gender];
            
            //if no female animation then use male one
            if (filePath == "")
                filePath = g_TblAnimationList.Cells[int.Parse(g_TblAnimationType.Cells[(int)Action][(int)WeaponType])][(int)AniGender.MALE];


            return filePath;
        }
        
    }
}
