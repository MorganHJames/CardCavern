using System.Linq;
using System.Reflection;
using UnityEngine;

namespace UnityEditor
{
    [CustomEditor(typeof(HexagonalRuleTile), true)]
    [CanEditMultipleObjects]
    internal class HexagonalRuleTileEditor : RuleTileEditor
    {
        private static readonly Vector2[] s_PointedTopPositions =
        {
            new Vector2(2f, 1f), new Vector2(1.5f, 2f), new Vector2(0.5f, 2f), new Vector2(0f, 1f), new Vector2(0.5f, 0f), new Vector2(1.5f, 0f)
        };
        private static readonly int[] s_PointedTopArrows = {5, 8, 6, 3, 0, 2};
        private static readonly Vector2[] s_FlatTopPositions =
        {
            new Vector2(1f, 0f), new Vector2(2f, 0.5f), new Vector2(2f, 1.5f), new Vector2(1f, 2f), new Vector2(0f, 1.5f), new Vector2(0f, 0.5f)
        };
        private static readonly int[] s_FlatTopArrows = {1, 2, 8, 7, 6, 0};

        internal override void RuleMatrixOnGUI(RuleTile ruleTile, Rect rect, Rect sibRect, RuleTile.TilingRule tilingRule)
        {
			var hexTile = (HexagonalRuleTile) ruleTile;
            bool flatTop = hexTile.m_FlatTop;

            Handles.color = EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.2f) : new Color(0f, 0f, 0f, 0.2f);
            float w = rect.width / 3f;
            float h = rect.height / 3f;

			float w2 = sibRect.width / 3f;
			float h2 = sibRect.height / 3f;
			// Grid
			if (flatTop)
            {
                for (int x = 0; x <= 3; x++)
                {
                    float left = rect.xMin + x * w;
                    float offset = x % 3 > 0 ? 0 : h / 2;
                    Handles.DrawLine(new Vector3(left, rect.yMin + offset), new Vector3(left, rect.yMax - offset));

                    if (x < 3)
                    {
                        bool noOffset = x % 2 > 0;
                        for (int y = 0; y < (noOffset ? 4 : 3); y++)
                        {
                            float top = rect.yMin + y * h + (noOffset ? 0 : h / 2);
                            Handles.DrawLine(new Vector3(left, top), new Vector3(left + w, top));
                        }
                    }
					if (tilingRule.m_CheckSiblings)
					{
						float left2 = sibRect.xMin + x * w;
						float offset2 = x % 3 > 0 ? 0 : h / 2;
						Handles.DrawLine(new Vector3(left2, sibRect.yMin + offset2), new Vector3(left2, sibRect.yMax - offset2));

						if (x < 3)
						{
							bool noOffset = x % 2 > 0;
							for (int y = 0; y < (noOffset ? 4 : 3); y++)
							{
								float top = sibRect.yMin + y * h + (noOffset ? 0 : h / 2);
								Handles.DrawLine(new Vector3(left2, top), new Vector3(left2 + w, top));
							}
						}
					}
                }
            }
            else
            {
                for (int y = 0; y <= 3; y++)
                {
                    float top = rect.yMin + y * h;
                    float offset = y % 3 > 0 ? 0 : w / 2;
                    Handles.DrawLine(new Vector3(rect.xMin + offset, top), new Vector3(rect.xMax - offset, top));

                    if (y < 3)
                    {
                        bool noOffset = y % 2 > 0;
                        for (int x = 0; x < (noOffset ? 4 : 3); x++)
                        {
                            float left = rect.xMin + x * w + (noOffset ? 0 : w / 2);
                            Handles.DrawLine(new Vector3(left, top), new Vector3(left, top + h));
                        }
                    }

					if (tilingRule.m_CheckSiblings)
					{
						float top2 = sibRect.yMin + y * h;
						float offset2 = y % 3 > 0 ? 0 : w / 2;
						Handles.DrawLine(new Vector3(sibRect.xMin + offset2, top2), new Vector3(sibRect.xMax - offset2, top2));

						if (y < 3)
						{
							bool noOffset = y % 2 > 0;
							for (int x = 0; x < (noOffset ? 4 : 3); x++)
							{
								float left = sibRect.xMin + x * w + (noOffset ? 0 : w / 2);
								Handles.DrawLine(new Vector3(left, top2), new Vector3(left, top2 + h));
							}
						}
					}
				}
            }
            
            // Icons
            Handles.color = Color.white;
            for (int index = 0; index < hexTile.neighborCount; ++index)
            {
                Vector2 position = flatTop ? s_FlatTopPositions[index] : s_PointedTopPositions[index];
                int arrowIndex = flatTop ? s_FlatTopArrows[index] : s_PointedTopArrows[index];
                Rect r = new Rect(rect.xMin + position.x * w, rect.yMin + position.y * h, w - 1, h - 1);
				Rect r2 = new Rect(rect.xMin + position.x * w, rect.yMin + position.y * h, w - 1, h - 1);
				RuleOnGUI(r, arrowIndex, tilingRule.m_Neighbors[index]);
				RuleOnGUI(r2, arrowIndex, tilingRule.m_SiblingNeighbors[index]);
				RuleNeighborUpdate(r, r2, tilingRule, index);
            }
            // Center
            {
                Rect r = new Rect(rect.xMin + w, rect.yMin + h, w - 1, h - 1);
				Rect r2 = new Rect(rect.xMin + w, rect.yMin + h, w - 1, h - 1);
				RuleTransformOnGUI(r, tilingRule.m_RuleTransform);
				RuleTransformOnGUI(r2, tilingRule.m_RuleTransform);
                RuleTransformUpdate(r, r2, tilingRule);
			}
        }
    }
}
