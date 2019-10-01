using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Data", menuName = "TerrainTile", order = 1)]
public class TerrainTile : ScriptableObject
{
	public TileBase land;
	public TileBase landsEnd;
	public LandType landType;

	public enum LandType
	{
		Normal,
		Ice,
		Desert,
		Hell,
		Heaven
	}
}