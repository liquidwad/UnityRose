﻿
#if UNITY_EDITOR

// Importer for rose maps and player
using UnityEditor;
using UnityEngine;
using UnityRose.Formats;
using System.IO;
using System.Collections.Generic;
using UnityRose.Game;

public class RoseTerrainWindow : EditorWindow {
	string m_inputDir = "";
	string m_szcPath = "";
	int objID;
	
	// Add menu named "My Window" to the Window menu
	[MenuItem ("GameObject/Create Other/Rose Object")]
	static void Init () {
		// Get existing open window or if none, make a new one:
		RoseTerrainWindow window = (RoseTerrainWindow)EditorWindow.GetWindow (typeof (RoseTerrainWindow));
	}
	
	private void ImportGalaxy()
	{
		/* TODO: Add code to loop through all Planets and load them */
		
	}
	
	private void ImportPlanet()
	{
		/* TODO: Add code to loop through all maps and load them */
	}
	
	private struct PatchNormalIndex
	{
		public int patchID;
		public int normalID;
		public PatchNormalIndex(int patchID, int normalID)
		{
			this.patchID = patchID;
			this.normalID = normalID;
		}
	}
	
	private void ImportMap(string mapName)
	{
		
		Debug.Log ("Importing map from " + m_inputDir + "...");
		bool success = true;
		
		DirectoryInfo dirs = new DirectoryInfo(m_inputDir);
		
		GameObject map = new GameObject();
		map.name = mapName;
		
		GameObject terrain = new GameObject();
		terrain.name = "Ground";
		terrain.transform.parent = map.transform;
		
		GameObject terrainObjects = new GameObject();
		terrainObjects.name = "Objects";
		terrainObjects.transform.parent = map.transform;
		
		List<RosePatch> patches = new List<RosePatch>();
		/* TODO: Add code to loop through all patches and load them */
		foreach(DirectoryInfo dir in dirs.GetDirectories())
		{
			if(!dir.Name.Contains("."))
			{
				patches.Add (ImportPatch (dir.FullName, terrain.transform, terrainObjects.transform));
			}
		} 
		
		//blend vertex normals at the seams between patches
		Dictionary<string, List<PatchNormalIndex>> patchNormalLookup = new Dictionary<string, List<PatchNormalIndex>>();
		int patchID = 0;
		// combine all normal lookups into one big lookup containing patch ID and normal ID
		foreach(RosePatch patch in patches)
		{
			
			// go through the lookup of this patch and append all normal id's to big lookup
			foreach(string vertex in patch.edgeVertexLookup.Keys)
			{
				List<PatchNormalIndex> ids = new List<PatchNormalIndex>();
				foreach(int id in patch.edgeVertexLookup[vertex])
					ids.Add(new PatchNormalIndex(patchID, id));
				
				if(!patchNormalLookup.ContainsKey(vertex))
					patchNormalLookup.Add(vertex, ids);
				else
					patchNormalLookup[vertex].AddRange(ids);	
				
			}
			
			patchID++;	
		}
		
		// go through each enttry in the big lookup and calculate avg normal, then assign to corresponding patches
		foreach(string vertex in patchNormalLookup.Keys)
		{
			Vector3 avg = Vector3.zero;
			// First pass: calculate average normal
			foreach(PatchNormalIndex entry in patchNormalLookup[vertex])
				avg += patches[entry.patchID].m_mesh.normals[entry.normalID];
			
			avg.Normalize();
			
			// Second pass: assign new normal to corresponding patches
			foreach(PatchNormalIndex entry in patchNormalLookup[vertex])
				patches[entry.patchID].m_mesh.normals[entry.normalID] = avg;
			
		}
		
		terrainObjects.transform.Rotate (90.0f, -90.0f, 0.0f);
		terrainObjects.transform.localScale = new Vector3(1.0f, 1.0f, -1.0f);
		terrainObjects.transform.position = new Vector3(5200.0f, 0.0f, 5200.0f);
		
		if(success)
			Debug.Log ("Map Import Complete");
		else
			Debug.Log ("!Map Import Failed");
	}
	
	private RosePatch ImportPatch(string inputDir, Transform terrainParent, Transform objectsParent)
	{
		// Patch consists of the following elements:
		//	- .HIM file specifying the heighmap of the terrain: 65x65 image of floats 
		//	- .TIL file specifying the texture tileset: 16x16 tiles, containing ID's that index into .ZON.Tiles, which returns index into .ZON.Textures
		//  - TODO: add other fileTypes here after researching
		
		
		bool success = true;
		
		// Patch patch = new Patch(patchPath, 
		Debug.Log ("Importing patch from " + inputDir + "...");
		
		RosePatch patch = new RosePatch(new DirectoryInfo(inputDir));
		
		success &= patch.Load();
		success &= patch.Import(terrainParent, objectsParent);
		
		
		if(success)
			Debug.Log ("Patch Import complete");
		else
			Debug.Log ("!Patch Import failed");
		
		return patch;
	}
	
	void OnGUI () {
		// Need to do the following:
		// - Specify a specific map to load (path + directory browser?)
		// - Specify an output directory (path + directory browser?)
		// - Button to begin conversion
		// - add any extra settings here ...
		
		// ----------------Example GUI elements-----------------------
		// GUILayout.Label ("Settings", EditorStyles.boldLabel);
		// myBool = EditorGUILayout.Toggle ("Toggle", myBool);
		// myFloat = EditorGUILayout.Slider ("Slider", myFloat, -3, 3);
		// -----------------------------------------------------------
		
		//=================== MAP =======================
		
		EditorGUILayout.BeginToggleGroup("Map", true);
		m_inputDir = EditorGUILayout.TextField ("Input dir: ", m_inputDir);
		if(GUILayout.Button("Import"))
		{
			// Several options based on the given path
			// 1. 3DDATA/MAPS/						-> convert rose universe
			// 2. 3DDATA/MAPS/JUNON					-> convert rose planet
			// 3. 3DDATA/MAPS/JUNON/JPT01			-> convert rose map
			// 4. 3DDATA/MAPS/JUNON/JTP01/30_30		-> convert rose patch
			// 5. Some/invalid/path
			bool notFound = false;
			DirectoryInfo inDirInfo = new DirectoryInfo(m_inputDir);
			switch (inDirInfo.Name.ToLower())
			{
			case "maps": 						// 1.
				ImportGalaxy();
				break;
			case "junon": 						// 2.
			case "eldeon":				
			case "lunar":		
				// TODO: add any new planets here...				
				ImportPlanet();
				break;
			default:
				notFound = true;
				break;	
			} // switch
			
			if(notFound)
			{
				switch (inDirInfo.Parent.Name.ToLower())
				{
				case "junon":				// 3.
				case "eldeon":
				case "lunar":
					// TODO: add any new planets here...
					ImportMap(inDirInfo.Parent.Name.ToLower());
					break;
				default:
					GameObject terrain = new GameObject();
					terrain.name = "Terrain";
					
					GameObject terrainObjects = new GameObject();
					terrainObjects.name = "Terrain Objects";
					ImportPatch(m_inputDir, terrain.transform, terrainObjects.transform);			// 4. (LoadPatch will handle 5. properly)	
					break;		
				}	// switch
			} // if
		} // if
		
		EditorGUILayout.EndToggleGroup ();
		// ======================== OBJECT ==========================
		EditorGUILayout.BeginToggleGroup("Animated Object", true);
		m_szcPath = EditorGUILayout.TextField ("ZSC: ", m_szcPath);
		objID = EditorGUILayout.IntField ("ID: ", objID);
		
		
		if(GUILayout.Button("Import"))
		{
			RosePlayer player = new RosePlayer(RosePlayer.Gender.male);  
		}
		
		EditorGUILayout.EndToggleGroup ();
	} // OnGui()
}

#endif