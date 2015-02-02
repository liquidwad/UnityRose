
using System;
using System.Collections.Generic;
using System.IO;
using System.Resources;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
    
public class Utils
{
	public static Quaternion r2uRotation(Quaternion q)
	{
	    Vector3 axis;
	    float angle;
	    q.ToAngleAxis(out angle, out axis);
	    return Quaternion.AngleAxis(angle, new Vector3(axis.x, axis.z, -axis.y));
	}
	
	public static Vector3 r2uPosition(Vector3 v)
	{
	    return new Vector3(v.x, v.z, -v.y);
	}
	
	public static Vector3 r2uVector(Vector3 v)
	{
	    return new Vector3(v.x, v.z, -v.y);
	}

	public static void addVertexToLookup(Dictionary<String,List<int>> lookup, String vertex, int index)
	{
		if(!lookup.ContainsKey(vertex))
		{
			List<int> ids = new List<int>();
			ids.Add (index);
			lookup.Add (vertex.ToString(), ids);
		}
		else
		{
			lookup[vertex].Add(index);
		}
	}
	
	public static Bounds GetMaxBounds(GameObject g)
	{
		var b = new Bounds(g.transform.position, Vector3.zero);
		foreach (Renderer r in g.GetComponentsInChildren<Renderer>())
		{
			b.Encapsulate(r.bounds);
		}
		return b;
	}
	
#if UNITY_EDITOR
	// Returns true if platform is mac
	public static bool IsMac()
	{
		return Application.platform == RuntimePlatform.OSXEditor;
	}

	// fixes the path based on platform and returns fixed string
	public static string FixPath(string file)
	{
        if (IsMac())
            return file.Replace("\\", "/");
        else
            return file.Replace("/", "\\");
	}
	

	// Converts a rose path to a unity path and creates the directory structure of non-existent
	public static DirectoryInfo r2uDir(string rosePath, string extension = ".asset")
	{
		var roseDir = new DirectoryInfo(FixPath(rosePath).ToLower());
        string unityPath = roseDir.FullName;

        if (roseDir.Extension != "")
            unityPath = unityPath.Replace(roseDir.Extension, extension);
        else
            unityPath += extension;

        if (unityPath.Contains("3ddata"))
            unityPath = unityPath.Replace("3ddata", "GameData");

		var unityDir = new DirectoryInfo(unityPath);
		
		// Creat the parent folder path if it doesn't already exist
		if( !unityDir.Parent.Exists )
			Directory.CreateDirectory(unityDir.Parent.FullName);

        unityDir.Refresh();
		return unityDir;
	}
	
	// Trim path preceding /asset
	public static string GetUnityPath( DirectoryInfo path )
	{
		string result = path.Name;
		DirectoryInfo currentPath = path.Parent;
		while( currentPath.Name.ToLower() != "assets" )
		{
			DirectoryInfo nextPath = new DirectoryInfo(currentPath.FullName);
			result = nextPath.Name + "/" + result;
			currentPath = nextPath.Parent;
		}
			
		return ("assets/" + result);
	}

    // Trim path preceding /asset
    public static string GetUnityPath(string path)
    {
        DirectoryInfo dir = new DirectoryInfo(path);
        return GetUnityPath(dir);
    }
	
	// Saves an asset into the GameData folder using AssetDatabase, then loads it back from that path and returns it
	// The rose file path structure is maintained but 3ddata is replaced by GameData
	public static UnityEngine.Object SaveReloadAsset(UnityEngine.Object asset, string rosePath, string extension = ".asset")
	{
		DirectoryInfo unityDir = r2uDir ( rosePath, extension );
		// Only create the asset if it doesn't already exist

        if( !File.Exists(  unityDir.FullName ) )
		{
			AssetDatabase.CreateAsset( asset, GetUnityPath(unityDir) );
			AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

		return AssetDatabase.LoadMainAssetAtPath( GetUnityPath(unityDir) );
	}

    // Attempts to load an asset using the unity path. If not present, returns null
    public static UnityEngine.Object LoadAsset(string rosePath, string extension = ".asset")
    {
        DirectoryInfo unityDir = r2uDir(rosePath, extension);

        if (!File.Exists(unityDir.FullName))
            return null;   

        return AssetDatabase.LoadMainAssetAtPath(GetUnityPath(unityDir));
    }

    public static string GetUnityTexPath(string rosePath, string extension)
    {
        DirectoryInfo unityDir = new DirectoryInfo( rosePath );//r2uDir(rosePath, extension);
        string texPath = "Assets/GameData/Textures/" + unityDir.Name;
        // Convert the texture name to the given extensio and create intermediate folders if  not present
        DirectoryInfo texDir = r2uDir(texPath, extension);
        return texDir.FullName;
    }
	
	// Copy the texture from 3ddata to GameData (if it doesn't already exist) then load it as a Texture2D and return it
	public static Texture2D CopyReloadTexAsset( string rosePath )
	{
		/*
        DirectoryInfo unityDir = r2uDir ( rosePath, ".dds" );
        if ( !File.Exists(unityDir.FullName))
        {
            AssetDatabase.CopyAsset(FixPath(rosePath), GetUnityPath(unityDir));
            AssetDatabase.Refresh();
        }
		
		return (Texture2D) AssetDatabase.LoadMainAssetAtPath( GetUnityPath(unityDir) );
         * */

        string texPathDDS = GetUnityTexPath(rosePath, ".dds");
        string texPathPNG = GetUnityTexPath(rosePath, ".png");

        
        if (!File.Exists(texPathPNG) && !File.Exists(texPathDDS))
        {
            // If neither dds nor png are present, move texture to textures folder as dds and load dds
            AssetDatabase.CopyAsset(FixPath(rosePath), texPathDDS);
            AssetDatabase.Refresh();
            return (Texture2D)AssetDatabase.LoadMainAssetAtPath( GetUnityPath(texPathDDS) );
        }
        else if( File.Exists(texPathPNG) )
        {
            // if both dds and png files are present in the textures folder, delete the dds
            if( File.Exists(texPathDDS) )
            {
                AssetDatabase.DeleteAsset(GetUnityPath(texPathDDS));
                AssetDatabase.Refresh();
            }

            return (Texture2D)AssetDatabase.LoadMainAssetAtPath(GetUnityPath(texPathPNG));
        }
        else if( File.Exists(texPathDDS) )
        {
            // if only dds file exists, return it
            return (Texture2D)AssetDatabase.LoadMainAssetAtPath(GetUnityPath(texPathDDS));
        }

        // in case of some failure, return null
        return null;
	}


	//****************************************************************************************************
	//  static function DrawLine(rect : Rect) : void
	//  static function DrawLine(rect : Rect, color : Color) : void
	//  static function DrawLine(rect : Rect, width : float) : void
	//  static function DrawLine(rect : Rect, color : Color, width : float) : void
	//  static function DrawLine(Vector2 pointA, Vector2 pointB) : void
	//  static function DrawLine(Vector2 pointA, Vector2 pointB, color : Color) : void
	//  static function DrawLine(Vector2 pointA, Vector2 pointB, width : float) : void
	//  static function DrawLine(Vector2 pointA, Vector2 pointB, color : Color, width : float) : void
	//  
	//  Draws a GUI line on the screen.
	//  
	//  DrawLine makes up for the severe lack of 2D line rendering in the Unity runtime GUI system.
	//  This function works by drawing a 1x1 texture filled with a color, which is then scaled
	//   and rotated by altering the GUI matrix.  The matrix is restored afterwards.
	//****************************************************************************************************
	
	public static Texture2D lineTex;
	
	public static void DrawLine(Rect rect) { DrawLine(rect, GUI.contentColor, 1.0f); }
	public static void DrawLine(Rect rect, Color color) { DrawLine(rect, color, 1.0f); }
	public static void DrawLine(Rect rect, float width) { DrawLine(rect, GUI.contentColor, width); }
	public static void DrawLine(Rect rect, Color color, float width) { DrawLine(new Vector2(rect.x, rect.y), new Vector2(rect.x + rect.width, rect.y + rect.height), color, width); }
	public static void DrawLine(Vector2 pointA, Vector2 pointB) { DrawLine(pointA, pointB, GUI.contentColor, 1.0f); }
	public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color) { DrawLine(pointA, pointB, color, 1.0f); }
	public static void DrawLine(Vector2 pointA, Vector2 pointB, float width) { DrawLine(pointA, pointB, GUI.contentColor, width); }
	public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width)
	{
		// Save the current GUI matrix, since we're going to make changes to it.
		Matrix4x4 matrix = GUI.matrix;
		
		// Generate a single pixel texture if it doesn't exist
		if (!lineTex) { lineTex = new Texture2D(1, 1); }
		
		// Store current GUI color, so we can switch it back later,
		// and set the GUI color to the color parameter
		Color savedColor = GUI.color;
		GUI.color = color;
		
		// Determine the angle of the line.
		float angle = Vector3.Angle(pointB - pointA, Vector2.right);
		
		// Vector3.Angle always returns a positive number.
		// If pointB is above pointA, then angle needs to be negative.
		if (pointA.y > pointB.y) { angle = -angle; }
		
		// Use ScaleAroundPivot to adjust the size of the line.
		// We could do this when we draw the texture, but by scaling it here we can use
		//  non-integer values for the width and length (such as sub 1 pixel widths).
		// Note that the pivot point is at +.5 from pointA.y, this is so that the width of the line
		//  is centered on the origin at pointA.
		GUIUtility.ScaleAroundPivot(new Vector2((pointB - pointA).magnitude, width), new Vector2(pointA.x, pointA.y + 0.5f));
		
		// Set the rotation for the line.
		//  The angle was calculated with pointA as the origin.
		GUIUtility.RotateAroundPivot(angle, pointA);
		
		// Finally, draw the actual line.
		// We're really only drawing a 1x1 texture from pointA.
		// The matrix operations done with ScaleAroundPivot and RotateAroundPivot will make this
		//  render with the proper width, length, and angle.
		GUI.DrawTexture(new Rect(pointA.x, pointA.y, 1, 1), lineTex);
		
		// We're done.  Restore the GUI matrix and GUI color to whatever they were before.
		GUI.matrix = matrix;
		GUI.color = savedColor;
	}

#endif
}