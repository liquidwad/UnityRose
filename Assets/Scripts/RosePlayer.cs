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
		this.gender = gender;
		LoadPlayer(GenderType.MALE, WeaponType.EMPTY, "New", 97,97,97,2,3,231,97);
	}

	private Bounds LoadPart(GameObject skeleton, BindPoses poses, BodyPartType bodyPart, string zmsPath, string texPath)
    {
        zmsPath = Utils.FixPath(zmsPath);
		texPath = Utils.FixPath (texPath).Replace ("dds", "png");

        // Cached load of ZMS and texture
        ResourceManager rm = ResourceManager.Instance;
        ZMS zms = (ZMS)rm.cachedLoad(zmsPath);
        Texture2D tex = (Texture2D)rm.cachedLoad(texPath);

        // Create material
        string shader = "VertexLit";
        if (bodyPart == BodyPartType.BACK)
            shader = "Transparent/Cutout/VertexLit";

        Material mat = new Material(Shader.Find(shader));

        mat.SetTexture("_MainTex", tex);
        mat.SetColor("_Emission", new Color(0.15f, 0.15f, 0.15f));

        GameObject modelObject = new GameObject();

        switch ( bodyPart )
        {
            case BodyPartType.FACE:
            case BodyPartType.HAIR:
				modelObject.transform.parent = Utils.findChild(skeleton, "b1_head");
                break;
            case BodyPartType.CAP:
				modelObject.transform.parent = Utils.findChild(skeleton, "p_06");
                break;
            case BodyPartType.BACK:
				modelObject.transform.parent = Utils.findChild(skeleton, "p_03");
                break;
            default:
				modelObject.transform.parent = skeleton.transform.parent.transform;
                break;
        }
        
        modelObject.transform.localPosition = Vector3.zero;
        modelObject.transform.localRotation = Quaternion.identity;
        modelObject.transform.localScale = Vector3.one;
		modelObject.name = bodyPart.ToString ();
        Mesh mesh = zms.getMesh();
        if (zms.support.bones)
        {
            SkinnedMeshRenderer renderer = modelObject.AddComponent<SkinnedMeshRenderer>();

			mesh.bindposes = poses.bindPoses;
            renderer.sharedMesh = mesh;
            renderer.material = mat;
			renderer.bones = poses.boneTransforms;
        }
        else
        {
            modelObject.AddComponent<MeshFilter>().mesh = mesh;
            MeshRenderer renderer = modelObject.AddComponent<MeshRenderer>();
            renderer.material = mat;
        }

        return mesh.bounds;

    }

    public Bounds LoadObject(GenderType gender, BodyPartType bodyPart, int id, GameObject skeleton, BindPoses poses)
    {
        Bounds objectBounds = new Bounds(skeleton.transform.position, Vector3.zero);
        ResourceManager rm = ResourceManager.Instance;
        ZSC zsc = rm.getZSC(gender, bodyPart);
        for (int i = 0; i < zsc.Objects[id].Models.Count; i++)
        {
            int ModelID = zsc.Objects[id].Models[i].ModelID;
            int TextureID = zsc.Objects[id].Models[i].TextureID;

            Bounds partBounds = LoadPart(skeleton, poses, bodyPart, zsc.Models[ModelID], zsc.Textures[TextureID].Path);
            objectBounds.Encapsulate(partBounds);
        }
        return objectBounds;
    }

    public GameObject LoadDefault(GenderType gender)
    {
        return LoadPlayer(gender, WeaponType.EMPTY, "New", 0, 0, 0, 0, 0, 0, 0);
    }

	public GameObject LoadPlayer(GenderType gender, WeaponType weapon, string name, int body, int arms, int foot, int hair, int face, int back, int cap)
	{
        // Get the correct resources
        bool male = (gender == GenderType.MALE);

        ResourceManager rm = ResourceManager.Instance;

        GameObject player = new GameObject(name);
		GameObject skeleton = rm.loadSkeleton(gender, weapon);
		BindPoses poses = rm.loadBindPoses (skeleton, gender, weapon);
        skeleton.transform.parent = player.transform;

		//load all objects
        Bounds playerBounds = new Bounds(player.transform.position, Vector3.zero);
 
        playerBounds.Encapsulate(LoadObject(gender, BodyPartType.BODY, body, skeleton, poses));
        playerBounds.Encapsulate(LoadObject(gender, BodyPartType.ARMS, arms, skeleton, poses));
        playerBounds.Encapsulate(LoadObject(gender, BodyPartType.FOOT, foot, skeleton, poses));
        playerBounds.Encapsulate(LoadObject(gender, BodyPartType.FACE, face, skeleton, poses));
        playerBounds.Encapsulate(LoadObject(gender, BodyPartType.HAIR, hair, skeleton, poses));
        LoadObject(gender, BodyPartType.CAP, cap, skeleton, poses);
        LoadObject(gender, BodyPartType.BACK, back, skeleton, poses);

        //add PlayerController script
        PlayerController controller = player.AddComponent<PlayerController>();
        controller.isMainPlayer = true;
        controller.playerInfo.tMovS = 10.0f;

        //add Character controller
        Vector3 center = skeleton.transform.FindChild("b1_pelvis").localPosition;
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

        return player;

	}

}
