using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityRose.Formats;
using System.IO;
using UnityEditor;


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
		this.gender = gender;
		LoadMale(0,1,0,0,0,0,17,0);
		LoadAnimations(0);
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
	
	public void LoadAnimations(int weapon)
	{
		const string sit = "Assets/3DData/Motion/Avatar/EMPTY_SIT_M1.zmo";
		const string sitting = "Assets/3DData/Motion/Avatar/EMPTY_SITING_M1.zmo";
		const string stand = "Assets/3DData/Motion/Avatar/EMPTY_STAND_M1.zmo";
		const string standing = "Assets/3DData/Motion/Avatar/EMPTY_STOP1_M1.zmo";
		const string walk = "Assets/3DData/Motion/Avatar/EMPTY_WALK_M1.zmo";
		const string attack = "Assets/3DData/Motion/Avatar/EMPTY_ATTACK01_M1.zmo";
		
		Animation animation = player.AddComponent<Animation>();
		
		animation.AddClip(new ZMO(sit).buildAnimationClip(skeleton), "sit");
		animation.AddClip(new ZMO(sitting).buildAnimationClip(skeleton), "sitting");
		animation.AddClip(new ZMO(stand).buildAnimationClip(skeleton), "stand");
		animation.AddClip(new ZMO(standing).buildAnimationClip(skeleton), "standing");
		animation.AddClip(new ZMO(walk).buildAnimationClip(skeleton), "walk");
		animation.AddClip(new ZMO(attack).buildAnimationClip(skeleton), "attack");	
		
		player.AddComponent<PlayerController>();
	}
	
	public void LoadMale(int chest, int legs, int arms, int foot, int hair, int face, int back, int cap)
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
		/*
		LoadModel(body_zsc, chest, skeleton);
		LoadModel(body_zsc, legs, skeleton);
		LoadModel(arms_zsc, arms, skeleton);
		LoadModel(foot_zsc, foot, skeleton);
		LoadModel(hair_zsc, hair, skeleton);
		LoadModel(face_zsc, face, skeleton);
		LoadModel(cap_zsc, face, skeleton, true);
		LoadModel(back_zsc, back, skeleton, true);
		*/
		
		
		LoadPart(body_zsc.Models[chest], body_zsc.Textures[chest].Path, skeleton, player.transform);
		LoadPart(body_zsc.Models[legs], body_zsc.Textures[legs].Path, skeleton, player.transform);
		LoadPart(arms_zsc.Models[arms], arms_zsc.Textures[arms].Path, skeleton, player.transform);
		LoadPart(foot_zsc.Models[foot], foot_zsc.Textures[foot].Path, skeleton, player.transform);
		LoadPart(face_zsc.Models[face], face_zsc.Textures[face].Path, skeleton, skeleton.findBone("b1_neck").boneObject.transform);
		LoadPart(hair_zsc.Models[hair], hair_zsc.Textures[hair].Path, skeleton, skeleton.findBone("b1_neck").boneObject.transform);
		LoadPart(cap_zsc.Models[cap], cap_zsc.Textures[cap].Path, skeleton, skeleton.findDummy("p_06").boneObject.transform);
		LoadPart(back_zsc.Models[back], back_zsc.Textures[back].Path, skeleton, skeleton.findDummy("p_03").boneObject.transform, false);
		
		player.transform.Rotate(-90.0f, 0.0f, 180.0f);
		
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

