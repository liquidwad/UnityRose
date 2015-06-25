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
	public CharModel charModel;
    private BindPoses bindPoses;
    private GameObject skeleton;
	private ResourceManager rm;
	private Bounds playerBounds;

    public RosePlayer()
	{
		charModel = new CharModel ();
		LoadPlayer (charModel);
	}

	public RosePlayer(GenderType gender)
	{
		charModel = new CharModel ();
		charModel.gender = gender;
		LoadPlayer (charModel);
	}

	public RosePlayer(CharModel charModel)
	{
        this.charModel = charModel;
		LoadPlayer (charModel);

	}
	public RosePlayer( Vector3 position )
	{
		charModel = new CharModel ();
		charModel.pos = position;
		LoadPlayer (charModel);
	}
	private void LoadPlayer(CharModel charModel)
	{
		// Get the correct resources
		bool male = (charModel.gender == GenderType.MALE);
		
		rm = ResourceManager.Instance;
		
		player = new GameObject(charModel.name);

		LoadPlayerSkeleton (charModel);
		
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
		
		player.transform.position = charModel.pos;
	}

	private void LoadPlayerSkeleton( CharModel charModel)
	{
		LoadPlayerSkeleton(
		            charModel.gender,
					charModel.weapon,
		            charModel.equip.weaponID, 
		            charModel.equip.chestID, 
		            charModel.equip.handID, 
		            charModel.equip.footID, 
		            charModel.equip.hairID, 
		            charModel.equip.faceID, 
		            charModel.equip.backID, 
		            charModel.equip.capID);
	}


	private void LoadPlayerSkeleton(GenderType gender, WeaponType weapType, int weapon, int body, int arms, int foot, int hair, int face, int back, int cap)
	{

		// First destroy any children of player
		int childs = player.transform.childCount;
		
		for (int i = childs - 1; i > 0; i--)
			Utils.Destroy(player.transform.GetChild(i).gameObject);

		Utils.Destroy (skeleton);
	
		skeleton = rm.loadSkeleton(gender, weapType);
		bindPoses = rm.loadBindPoses (skeleton, gender, weapType);
		skeleton.transform.parent = player.transform;
		skeleton.transform.localPosition = new Vector3 (0, 0, 0);
		skeleton.transform.localRotation = Quaternion.Euler (new Vector3 (0, 0, 0));


		// If player has already been initialized, make sure it knows that the skeleton has changed in order to restart its state machine with new animations
		PlayerController controller = player.GetComponent<PlayerController> ();
		if( controller != null )
			controller.OnSkeletonChange ();

		//load all objects
		playerBounds = new Bounds(player.transform.position, Vector3.zero);
		
		playerBounds.Encapsulate(LoadObject(BodyPartType.BODY, body));
		playerBounds.Encapsulate(LoadObject(BodyPartType.ARMS, arms));
		playerBounds.Encapsulate(LoadObject(BodyPartType.FOOT, foot));
		playerBounds.Encapsulate(LoadObject(BodyPartType.FACE, face));
		playerBounds.Encapsulate(LoadObject(BodyPartType.HAIR, hair));
		playerBounds.Encapsulate(LoadObject(BodyPartType.WEAPON, weapon));
		LoadObject(BodyPartType.CAP, cap);
		LoadObject(BodyPartType.BACK, back);
	}

    public void equip(BodyPartType bodyPart, int id)
    {
		if (bodyPart == BodyPartType.WEAPON) { // TODO: SUBWEAPON
			WeaponType weapType = rm.getWeaponType (id);
			charModel.equip.weaponID = id;
			// Change skeleton if weapon type is different
			if (weapType != charModel.weapon) {
				charModel.weapon = weapType;
				LoadPlayerSkeleton ( charModel );
				return;
			}
		}

		charModel.changeID(bodyPart, id);

		List<Transform> partTransforms = Utils.findChildren (player, bodyPart.ToString());

		foreach (Transform partTransform in partTransforms) 
			Utils.Destroy(partTransform.gameObject);


		LoadObject(bodyPart, id);

    }

	/*
    // TODO: expand to allow weapon effects
    private void equipWeapon(int id)
    {
		WeaponType weapType = rm.getWeaponType (id);
		charModel.equip.weaponID = id;
		// Change skeleton if weapon type is different
		if (weapType != this.weaponType) {
			LoadPlayerSkeleton ( gender, weapType, id, charModel.equip.chestID, charModel.equip.handID, charModel.equip.footID, charModel.equip.hairID, charModel.equip.faceID, charModel.equip.backID, charModel.equip.capID);
		}
		else


		// First, remove existing weapon and shield if new weapon is two-handed
		bool removeShield = !Utils.isOneHanded (weaponType);
		Transform weaponTransform = Utils.findChild (player, BodyPartType.WEAPON.ToString ());
		Transform shieldTransform = removeShield ? Utils.findChild (player, BodyPartType.SUBWEAPON.ToString ()) : null;

#if UNITY_EDITOR
		if (shieldTransform != null && removeShield)
			GameObject.DestroyImmediate (shieldTransform.gameObject);
		if (weaponTransform != null)
			GameObject.DestroyImmediate (weaponTransform.gameObject);
#else
        if (shieldTransform != null && removeShield) GameObject.Destroy(shieldTransform.gameObject);
		if (weaponTransform != null) GameObject.Destroy (weaponTransform.gameObject);
#endif
        
		// Equip new weapon
		LoadObject (BodyPartType.WEAPON, id);
		
    }
	*/

    private Bounds LoadObject(BodyPartType bodyPart, int id)
	{
		Bounds objectBounds = new Bounds(skeleton.transform.position, Vector3.zero);
		ZSC zsc = rm.getZSC(charModel.gender, bodyPart); 
		for (int i = 0; i < zsc.Objects[id].Models.Count; i++)
		{
			int ModelID = zsc.Objects[id].Models[i].ModelID;
			int TextureID = zsc.Objects[id].Models[i].TextureID;
			
			Bounds partBounds = LoadPart(bodyPart, zsc.Objects[id].Models[i].DummyIndex, zsc.Models[ModelID], zsc.Textures[TextureID].Path);
			objectBounds.Encapsulate(partBounds);
		}
		return objectBounds;
	}


	private Bounds LoadPart(BodyPartType bodyPart, ZSC.DummyType dummy, string zmsPath, string texPath)
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
            case BodyPartType.WEAPON:
				if(charModel.weapon == WeaponType.DSW || charModel.weapon == WeaponType.KATAR )
                	modelObject.transform.parent = Utils.findChild(skeleton, dummy == ZSC.DummyType.RightHand? "p_00" : "p_01");
				else
					modelObject.transform.parent = Utils.findChild(skeleton, "p_00");
                break;
            case BodyPartType.SUBWEAPON:
                modelObject.transform.parent = Utils.findChild(skeleton, "p_02");
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


}
