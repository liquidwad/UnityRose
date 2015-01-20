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


        
		
        
       
		//LoadFemale (0,0,0);
	}
	
	private void LoadPart(string zmsPath, string texPath, ZMD skeleton, Transform parent, bool backItem = false)
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
		
		//modelObject.AddComponent<MeshCollider>().sharedMesh = mesh;
	
	}
	
	public void LoadModel(ZSC zsc, int id, ZMD skeleton, bool dummy = false)
	{
	
		foreach(ZSC.Object.Model model in zsc.Objects[id].Models)
		{
			string zmsPath = zsc.Models[model.ModelID];
			ZSC.Texture texture = zsc.Textures[model.TextureID];
			string texPath = texture.Path;
			
			zmsPath = "Assets/" + zmsPath.Replace("\\","/");
			texPath = "Assets/" + texPath.Replace("\\","/");
			
			string shader = "Diffuse";
			if(texture.AlphaEnabled)
				shader = "Transparent/Cutout/Diffuse";
			
			// TODO: pick correct shader based on alpha and glow
			Material mat = new Material(Shader.Find(shader));
			Texture2D tex = Resources.LoadAssetAtPath<Texture2D>(texPath);
			mat.SetTexture("_MainTex", tex);
			
			
			GameObject modelObject = new GameObject();
			Transform parent;
			if(!dummy)
				parent = skeleton.bones[(int)model.BoneIndex].boneObject.transform;
			else
				parent = skeleton.dummies[(int)model.DummyIndex].boneObject.transform;
			
			modelObject.transform.parent = parent;
			modelObject.transform.localPosition = model.Position; //Vector3.zero;
			modelObject.transform.localRotation = model.Rotation; //Quaternion.identity;
			modelObject.transform.localScale = model.Scale;//Vector3.one;
			modelObject.name = new DirectoryInfo(zmsPath).Name;
			
			ZMS zms = new ZMS(zmsPath);
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
				
		}

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

	public void LoadObject(ZSC zsc, int id, ZMD skelton,Transform parent, bool backItem=false )
	{
		for (int i = 0; i < zsc.Objects[id].Models.Count; i++)
		{
			int ModelID = zsc.Objects[id].Models[i].ModelID;
			int TextureID = zsc.Objects[id].Models[i].TextureID;
			
			LoadPart( zsc.Models[ModelID], zsc.Textures[TextureID].Path,skelton, parent, backItem);
		}
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
		LoadObject (body_zsc, chest, skeleton, player.transform);
		LoadObject (arms_zsc, arms, skeleton, player.transform);
		LoadObject (foot_zsc, foot, skeleton, player.transform);
		LoadObject(face_zsc, face, skeleton, skeleton.findBone("b1_head").boneObject.transform);
		LoadObject(hair_zsc, hair, skeleton, skeleton.findBone("b1_head").boneObject.transform);
		LoadObject(cap_zsc, cap, skeleton, skeleton.findDummy("p_06").boneObject.transform);
		LoadObject(back_zsc, back , skeleton, skeleton.findDummy("p_03").boneObject.transform, true);	

		player.transform.Rotate(-90.0f, 0.0f, 180.0f);



        //load animations
        LoadAnimations(RoseData.AniWeaponType.EMPTY);

        //add PlayerController script
        player.AddComponent<PlayerController>();

        //add rigidBody
        Rigidbody r = player.AddComponent<Rigidbody>();

        //add collider
        CapsuleCollider c = player.AddComponent<CapsuleCollider>();
        c.center = new Vector3(0,0,1);
        c.height = 2;
        c.direction = 2; // direction z
        

        //Create the camera
        GameObject cameraObject = new GameObject("Main Camera");

        CameraController cameraController =  cameraObject.AddComponent<CameraController>();
        cameraController.target = player;

        Camera camera = cameraObject.AddComponent<Camera>();
        camera.tag = "MainCamera";

        

        
        
        



		/*
		ZMO zmo = new ZMO("Assets/3ddata/motion/avatar/dance_chacha_m1.zmo", false, true);
		Animation animation = player.AddComponent<Animation>();
		AnimationClip clip = zmo.buildAnimationClip(skeleton);
		clip.name = "dance_cha_cha_m1";
		
		//AssetDatabase.CreateAsset(clip, "Assets/Animations/dance_cha_cha_m1.asset");
		animation.AddClip(clip, "dance_cha_cha_m1");
		animation.clip = clip;
		//animation.playAutomatically = true;
		//animation.wrapMode = WrapMode.Loop;
		//AssetDatabase.CreateAsset(animation, "Assets/Animations/dance_cha_cha_m1.anim");
		
		player.transform.Rotate(-90.0f, 0.0f, 180.0f);
		
		Animator animator = player.AddComponent<Animator>();
		
		HumanDescription config = new HumanDescription();
		config.armStretch = 0.5f;
		config.feetSpacing = 0.5f;
		config.legStretch = 0.5f;
		config.lowerArmTwist = 0.5f;
		config.upperArmTwist = 0.5f;
		config.lowerLegTwist = 0.5f;
		config.upperArmTwist = 0.5f;
		
		SkeletonBone[] bones = new SkeletonBone[skeleton.bones.Count+1];
		bones[0] = new SkeletonBone();
		bones[0].name = "player";
		bones[0].position = player.transform.position;
		bones[0].rotation = player.transform.rotation;
		bones[0].scale = player.transform.localScale;
		
		for(int i=1; i<bones.Length; i++)
		{
			bones[i] = new SkeletonBone();
			bones[i].name = skeleton.bones[i-1].Name;
			bones[i].position = skeleton.boneTransforms[i].localPosition;// skeleton.bones[i].Position;
			bones[i].rotation = skeleton.boneTransforms[i].localRotation;//skeleton.bones[i].Rotation;
			bones[i].scale = skeleton.boneTransforms[i].localScale;
		}
		config.skeleton = bones;
		
		Dictionary<string, string> unity2rose = new Dictionary<string,string>(15);
		unity2rose.Add ("Hips", skeleton.findBone("b1_pelvis").Name);
		unity2rose.Add ("LeftUpperLeg", skeleton.findBone("b1_lthigh").Name);
		unity2rose.Add ("RightUpperLeg", skeleton.findBone("b1_rthigh").Name);
		unity2rose.Add ("LeftLowerLeg", skeleton.findBone("b1_lcalf").Name);
		unity2rose.Add ("RightLowerLeg", skeleton.findBone("b1_rcalf").Name);
		unity2rose.Add ("LeftFoot", skeleton.findBone("b1_lfoot").Name);
		unity2rose.Add ("RightFoot", skeleton.findBone("b1_rfoot").Name);
		unity2rose.Add ("Spine", skeleton.findBone("b1_chest").Name);
		unity2rose.Add ("Head", skeleton.findBone("b1_head").Name);
		unity2rose.Add ("LeftUpperArm", skeleton.findBone("b1_lupperarm").Name);
		unity2rose.Add ("RightUpperArm", skeleton.findBone("b1_rupperarm").Name);
		unity2rose.Add ("LeftLowerArm", skeleton.findBone("b1_lforearm").Name);
		unity2rose.Add ("RightLowerArm", skeleton.findBone("b1_rforearm").Name);
		unity2rose.Add ("LeftHand", skeleton.findBone("b1_lhand").Name);
		unity2rose.Add ("RightHand", skeleton.findBone("b1_rhand").Name);

		
		config.human = new HumanBone[15];
		int index = 0;
		foreach(KeyValuePair<string, string> pair in unity2rose)
		{
			HumanBone humanBone = new HumanBone();
			humanBone.humanName = pair.Key;
			humanBone.boneName = pair.Value;
			humanBone.limit = new HumanLimit();
			humanBone.limit.useDefaultValues = true;
			config.human[index++] = humanBone;
		}
		
		
		Avatar male = AvatarBuilder.BuildHumanAvatar(player, config);
		male.name = "MalePlayer";
		
		//animator.avatar = male;
		
		AssetDatabase.CreateAsset(male, "Assets/Animations/MaleAvatar.asset");
		*/
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

