using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityRose.Formats;
using System.IO;
using UnityEditor;
using UnityRose;

public class RosePlayer
{
	public enum Gender { male, female };
	public GameObject player;
	// Bone
	private ZMD skeleton;
	// Items
	// Animations
     

	private Gender gender;
	
	public RosePlayer(Gender gender)
	{
        RoseData.LoadSTB();
		this.gender = gender;
		LoadMale(97,97,97,2,3,231,97);
	}
	
	private Bounds LoadPart(string zmsPath, string texPath, ZMD skeleton, Transform parent, bool backItem = false)
	{
		// load ZMS
		zmsPath = "Assets/" + zmsPath.Replace("\\","/");
		texPath = "Assets/" + texPath.Replace("\\","/");
		ZMS zms = new ZMS(zmsPath);
		
		// Create material
		
		string shader = "Diffuse";
		if(backItem)
			shader = "Transparent/Cutout/Diffuse";
		Material mat = new Material(Shader.Find(shader));
		Texture2D tex = Resources.LoadAssetAtPath<Texture2D>(texPath);
		mat.SetTexture("_MainTex", tex);
		
		GameObject modelObject = new GameObject();
		modelObject.transform.parent = parent;
		modelObject.transform.localPosition = Vector3.zero;
		modelObject.transform.localRotation = Quaternion.identity;
		modelObject.transform.localScale = Vector3.one;
		modelObject.name =  new DirectoryInfo(zmsPath).Name;
		
		Mesh mesh = zms.getMesh();
		
		if(zms.support.bones)
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
	
	
    /// <summary>
    /// Loads all animations for equiped weapon type
    /// </summary>
    /// <param name="WeaponType"></param>
	public void LoadAnimations(RoseData.AniWeaponType WeaponType)
	{
        Animation animation = player.AddComponent<Animation>();
        foreach ( RoseData.AniAction action in Enum.GetValues(typeof(RoseData.AniAction)))
        {
            animation.AddClip(new ZMO("Assets\\" +  RoseData.GetAnimationFile(WeaponType, action, RoseData.AniGender.MALE)).buildAnimationClip(skeleton), action.ToString());
        }
	}

	public Bounds LoadObject(ZSC zsc, int id, ZMD skelton,Transform parent, bool backItem=false )
	{
        Bounds objectBounds = new Bounds(parent.position, Vector3.zero);
		for (int i = 0; i < zsc.Objects[id].Models.Count; i++)
		{
			int ModelID = zsc.Objects[id].Models[i].ModelID;
			int TextureID = zsc.Objects[id].Models[i].TextureID;
			
			Bounds partBounds = LoadPart( zsc.Models[ModelID], zsc.Textures[TextureID].Path,skelton, parent, backItem);
            objectBounds.Encapsulate(partBounds);
		}
        return objectBounds;
	}
	
	public void LoadMale(int chest, int arms, int foot, int hair, int face, int back, int cap)
	{
		const string path_body = "Assets/3DData/Avatar/LIST_MBODY.zsc";
		const string path_arms = "Assets/3DData/Avatar/LIST_MARMS.zsc";
		const string path_foot = "Assets/3DData/Avatar/LIST_MFOOT.zsc";
		const string path_face = "Assets/3DData/Avatar/LIST_MFACE.zsc";
		const string path_hair = "Assets/3DData/Avatar/LIST_MHAIR.zsc";
		const string path_cap = "Assets/3DData/Avatar/LIST_MCAP.zsc";
		const string path_back = "Assets/3DData/Avatar/LIST_BACK.zsc";
		const string path_faceItem = "Assets/3DData/Avatar/LIST_FACEIEM.zsc";
		
		
		ZSC body_zsc = new ZSC(path_body);
		ZSC arms_zsc = new ZSC(path_arms);
		ZSC foot_zsc = new ZSC(path_foot);
		ZSC face_zsc = new ZSC(path_face);
		ZSC hair_zsc = new ZSC(path_hair);
		ZSC cap_zsc = new ZSC(path_cap);
		ZSC back_zsc = new ZSC(path_back);
		


		skeleton = new ZMD("Assets/3DData/Avatar/MALE.ZMD");
		player = new GameObject("player");
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

        //load animations
        LoadAnimations(RoseData.AniWeaponType.EMPTY);

        //add PlayerController script
        player.AddComponent<PlayerController>();

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

        //Create the camera if it isn't already there
        GameObject cameraObject = GameObject.Find("Main Camera");
        if(  cameraObject == null )
            cameraObject = new GameObject("Main Camera");

        cameraObject.transform.position = player.transform.position + new Vector3(-6.0f, 2.0f, -6.0f);
        cameraObject.transform.LookAt(player.transform);
        CameraController cameraController =  cameraObject.AddComponent<CameraController>();
        cameraController.target = player;

        Camera camera = cameraObject.AddComponent<Camera>();
        camera.farClipPlane = 300.0f;
        camera.tag = "MainCamera";
	}
	
	
	public void LoadFemale(int bodyID, int hairID, int faceID)
	{
		const string path_body = "Assets/3DData/Avatar/LIST_WBODY.zsc";
		const string path_arms = "Assets/3DData/Avatar/LIST_WARMS.zsc";
		const string path_foot = "Assets/3DData/Avatar/LIST_WFOOT.zsc";
		const string path_face = "Assets/3DData/Avatar/LIST_WFACE.zsc";
		const string path_hair = "Assets/3DData/Avatar/LIST_WHAIR.zsc";
		const string path_cap = "Assets/3DData/Avatar/LIST_WCAP.zsc";
		
		ZSC body_zsc = new ZSC(path_body);
		ZSC arms_zsc = new ZSC(path_arms);
		ZSC foot_zsc = new ZSC(path_foot);
		ZSC face_zsc = new ZSC(path_face);
		ZSC hair_zsc = new ZSC(path_hair);
		ZSC cap_zsc  = new ZSC(path_cap);
		
		skeleton = new ZMD("Assets/3DData/Avatar/FEMALE.ZMD");
		
		GameObject player = new GameObject("player");
		skeleton.buildSkeleton(player, true);
		LoadPart(body_zsc.Models[bodyID], body_zsc.Textures[bodyID].Path, skeleton, player.transform);
		LoadPart(body_zsc.Models[bodyID+1], body_zsc.Textures[bodyID+1].Path, skeleton, player.transform);
		LoadPart(arms_zsc.Models[bodyID], arms_zsc.Textures[bodyID].Path, skeleton, player.transform);
		LoadPart(foot_zsc.Models[bodyID], foot_zsc.Textures[bodyID].Path, skeleton, player.transform);
		LoadPart(face_zsc.Models[faceID], face_zsc.Textures[faceID].Path, skeleton, skeleton.findBone("b1_neck").boneObject.transform);
		LoadPart(hair_zsc.Models[hairID], hair_zsc.Textures[hairID].Path, skeleton, skeleton.findBone("b1_neck").boneObject.transform);
		
		ZMO zmo = new ZMO("Assets/3ddata/motion/avatar/dance_chacha_f1.zmo", false, true);
		Animation animation = player.AddComponent<Animation>();
		AnimationClip clip = zmo.buildAnimationClip(skeleton);
		animation.AddClip(clip, "dance_cha_cha");
		animation.playAutomatically = true;
		animation.wrapMode = WrapMode.Loop;
		
		player.transform.Rotate(-90.0f, 0.0f, 180.0f);
		
	}	
}

