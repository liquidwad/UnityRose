// <copyright file="RosePlayer.cs" company="Wadii Bellamine">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Wadii Bellamine</author>
// <date>2/25/2015 8:37 AM </date>
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityRose.Formats;
using System.IO;
using UnityRose;
using UnityEditor;

public class RosePlayer
{
	public enum Gender { male, female }; // TODO: there are now three gender enums. Clean this shit
	private Gender gender;

    public RosePlayer(Gender gender)
	{
        //RoseData.LoadSTB();
		this.gender = gender;
		LoadPlayer(GenderType.MALE, "New", 97,97,97,2,3,231,97);
		//LoadFemale(50,50,50,2,3,231,50);
	}

    private Bounds LoadPart(string zmsPath, string texPath, ZMD skeleton, Transform parent, bool backItem = false)
    {
        // load ZMS
        zmsPath = Utils.FixPath(zmsPath);
        texPath = Utils.FixPath(texPath);

        ZMS zms = new ZMS(zmsPath);

        // Create material

        string shader = "VertexLit";
        if (backItem)
            shader = "Transparent/Cutout/VertexLit";

        Material mat = new Material(Shader.Find(shader));

        //mat = (Material)Utils.SaveReloadAsset(mat, texPath, ".mat");

        Texture2D tex = Utils.loadTex(ref texPath); // Utils.CopyReloadTexAsset(texPath);
        mat.SetTexture("_MainTex", tex);
        mat.SetColor("_Emission", new Color(0.15f, 0.15f, 0.15f));


        //AssetDatabase.SaveAssets();

        GameObject modelObject = new GameObject();
        modelObject.transform.parent = parent;
        modelObject.transform.localPosition = Vector3.zero;
        modelObject.transform.localRotation = Quaternion.identity;
        modelObject.transform.localScale = Vector3.one;
        modelObject.name = new DirectoryInfo(zmsPath).Name;

        Mesh mesh = zms.getMesh();
        //mesh = (Mesh)Utils.SaveReloadAsset(mesh, zmsPath);

        if (zms.support.bones)
        {
            SkinnedMeshRenderer renderer = modelObject.AddComponent<SkinnedMeshRenderer>();
            mesh.bindposes = skeleton.bindposes;
            renderer.sharedMesh = mesh;
            renderer.material = mat;
            renderer.bones = skeleton.boneTransforms;
        }
        else
        {
            modelObject.AddComponent<MeshFilter>().mesh = mesh;
            MeshRenderer renderer = modelObject.AddComponent<MeshRenderer>();
            renderer.material = mat;
        }

        return mesh.bounds;

    }

	public Bounds LoadObject(ZSC zsc, int id, ZMD skeleton,Transform parent, bool backItem=false )
	{
        Bounds objectBounds = new Bounds(parent.position, Vector3.zero);
		for (int i = 0; i < zsc.Objects[id].Models.Count; i++)
		{
			int ModelID = zsc.Objects[id].Models[i].ModelID;
			int TextureID = zsc.Objects[id].Models[i].TextureID;
			
			Bounds partBounds = LoadPart( zsc.Models[ModelID], zsc.Textures[TextureID].Path, skeleton, parent, backItem);
            objectBounds.Encapsulate(partBounds);
		}
        return objectBounds;
	}
	
    public GameObject LoadDefault(GenderType gender)
    {
        return LoadPlayer(gender, "New", 0, 0, 0, 0, 0, 0, 0);
    }

	public GameObject LoadPlayer(GenderType gender, string name, int chest, int arms, int foot, int hair, int face, int back, int cap)
	{
        // Get the correct resources
        bool male = (gender == GenderType.MALE);

        ResourceManager rm = ResourceManager.Instance;

        ZSC body_zsc = male ? rm.zsc_body_male : rm.zsc_body_female;
        ZSC arms_zsc = male ? rm.zsc_arms_male : rm.zsc_arms_female;
        ZSC foot_zsc = male ? rm.zsc_foot_male : rm.zsc_foot_female;
        ZSC face_zsc = male ? rm.zsc_face_male : rm.zsc_face_female;
        ZSC cap_zsc = male ? rm.zsc_cap_male : rm.zsc_cap_female;
        ZSC hair_zsc = male ? rm.zsc_hair_male : rm.zsc_hair_female;
        ZSC back_zsc = rm.zsc_back;

        ZMD skeleton = male ? rm.zmd_male : rm.zmd_female;

        GameObject player = new GameObject(name);  // TODO: look into object pooling
       
		skeleton.buildSkeleton(player, false);
        
		//load all objects
        Bounds playerBounds = new Bounds(player.transform.position, Vector3.zero);
 
        playerBounds.Encapsulate(LoadObject(body_zsc, chest, skeleton, player.transform));
        playerBounds.Encapsulate(LoadObject(arms_zsc, arms, skeleton, player.transform));
		playerBounds.Encapsulate(LoadObject (foot_zsc, foot, skeleton, player.transform));
		playerBounds.Encapsulate(LoadObject(face_zsc, face, skeleton, skeleton.findBone("b1_head").boneObject.transform));
		playerBounds.Encapsulate(LoadObject(hair_zsc, hair, skeleton, skeleton.findBone("b1_head").boneObject.transform));
		LoadObject(cap_zsc, cap, skeleton, skeleton.findDummy("p_06").boneObject.transform);
		LoadObject(back_zsc, back , skeleton, skeleton.findDummy("p_03").boneObject.transform, true);

        player.AddComponent<Animation>();

        //load animations
		ResourceManager.Instance.LoadAnimations(player, skeleton, WeaponType.EMPTY, GenderType.MALE);

        //add PlayerController script
        PlayerController controller = player.AddComponent<PlayerController>();
        controller.isMainPlayer = true;
        controller.playerInfo.tMovS = 10.0f;

        //add Character controller
        Vector3 center = player.transform.FindChild("b1_pelvis").localPosition; //playerBounds.center;
        center.y = 0.95f;
        float height = 1.7f;
        float radius = Math.Max(playerBounds.extents.x, playerBounds.extents.y) / 2.0f;
        CharacterController charController = player.AddComponent<CharacterController>();
        charController.center = center;
        charController.height = height;
        charController.radius = radius;

        //add collider
        CapsuleCollider c = player.AddComponent<CapsuleCollider>();
        c.center =  center;
        c.height = height;
        c.radius = radius;
        c.direction = 1; // direction y

        player.transform.position = new Vector3(5200.0f, 0.0f, 5653.0f);

        /*
        //Create the camera if it isn't already there
        GameObject cameraObject = GameObject.Find("Main Camera");
        if (cameraObject == null)
        {
            cameraObject = new GameObject("Main Camera");
            cameraObject.transform.position = player.transform.position + new Vector3(-6.0f, 2.0f, -6.0f);
            cameraObject.transform.LookAt(player.transform);
            CameraController cameraController = cameraObject.AddComponent<CameraController>();
            cameraController.target = player;

            Camera camera = cameraObject.AddComponent<Camera>();
            camera.farClipPlane = 300.0f;
            camera.tag = "MainCamera";
        }
        */

        return player;

	}

}
