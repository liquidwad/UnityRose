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
    public GameObject player;
    public GenderType gender; 
    public WeaponType weapon; 
	public CharModel charModel;
    private BindPoses bindPoses;
    private GameObject skeleton;


    public RosePlayer()
	{
		this.gender = GenderType.MALE;
		LoadDefault (gender);
	}
	public RosePlayer(GenderType gender, WeaponType weapon)
	{
		this.gender = gender;
		LoadPlayer (gender, weapon, "New", 97, 97, 97, 2, 3, 231, 97);
	}

	public RosePlayer(CharModel charModel)
	{
		this.gender = charModel.gender;
		this.charModel = charModel;

		LoadPlayer (charModel.gender, 
		            (WeaponType)charModel.equip.weaponID, 
		            charModel.name, 
		            charModel.equip.chestID, 
		            charModel.equip.handID, 
		            charModel.equip.footID, 
		            charModel.equip.hairID, 
		            charModel.equip.faceID, 
		            charModel.equip.backID, 
		            charModel.equip.capID);

	}

    public void equip(BodyPartType bodyPart, int id)
    {
		List<Transform> partTransforms = Utils.findChildren (player, bodyPart.ToString());
		foreach (Transform partTransform in partTransforms) 
		{
#if UNITY_EDITOR
			GameObject.DestroyImmediate (partTransform.gameObject);
#else
			GameObject.Destroy (partTransform.gameObject);
#endif
		}

		LoadObject(gender, bodyPart, id);

    }

	public Bounds LoadObject(GenderType gender, BodyPartType bodyPart, int id)
	{
		Bounds objectBounds = new Bounds(skeleton.transform.position, Vector3.zero);
		ResourceManager rm = ResourceManager.Instance;
		ZSC zsc = rm.getZSC(gender, bodyPart);
		for (int i = 0; i < zsc.Objects[id].Models.Count; i++)
		{
			int ModelID = zsc.Objects[id].Models[i].ModelID;
			int TextureID = zsc.Objects[id].Models[i].TextureID;
			
			Bounds partBounds = LoadPart(bodyPart, zsc.Models[ModelID], zsc.Textures[TextureID].Path);
			objectBounds.Encapsulate(partBounds);
		}
		return objectBounds;
	}


	private Bounds LoadPart(BodyPartType bodyPart, string zmsPath, string texPath)
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

			mesh.bindposes = bindPoses.bindPoses;
            renderer.sharedMesh = mesh;
            renderer.material = mat;
			renderer.bones = bindPoses.boneTransforms;
        }
        else
        {
            modelObject.AddComponent<MeshFilter>().mesh = mesh;
            MeshRenderer renderer = modelObject.AddComponent<MeshRenderer>();
            renderer.material = mat;
        }

        return mesh.bounds;

    }

    public GameObject LoadDefault(GenderType gender)
    {
        return LoadPlayer(gender, WeaponType.EMPTY, "New", 0, 0, 0, 0, 1, 0, 0);
    }

	public GameObject LoadPlayer(GenderType gender, WeaponType weapon, string name, int body, int arms, int foot, int hair, int face, int back, int cap)
	{
        // Get the correct resources
        bool male = (gender == GenderType.MALE);

        ResourceManager rm = ResourceManager.Instance;

        player = new GameObject(name);
		skeleton = rm.loadSkeleton(gender, weapon);
		bindPoses = rm.loadBindPoses (skeleton, gender, weapon);
        skeleton.transform.parent = player.transform;

		//load all objects
        Bounds playerBounds = new Bounds(player.transform.position, Vector3.zero);
 
        playerBounds.Encapsulate(LoadObject(gender, BodyPartType.BODY, body));
        playerBounds.Encapsulate(LoadObject(gender, BodyPartType.ARMS, arms));
        playerBounds.Encapsulate(LoadObject(gender, BodyPartType.FOOT, foot));
        playerBounds.Encapsulate(LoadObject(gender, BodyPartType.FACE, face));
        playerBounds.Encapsulate(LoadObject(gender, BodyPartType.HAIR, hair));
        LoadObject(gender, BodyPartType.CAP, cap);
        LoadObject(gender, BodyPartType.BACK, back);

        //add PlayerController script
        PlayerController controller = player.AddComponent<PlayerController>();
		controller.rosePlayer = this;
		controller.playerInfo.tMovS = charModel.stats.movSpd;

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
