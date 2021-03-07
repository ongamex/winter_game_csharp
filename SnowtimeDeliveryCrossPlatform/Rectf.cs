using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Game1
{
	public static class MyMath
	{
		public static float lerpClamp(float from, float to, float vel) {
			if (from > to) from = Math.Max(from - vel, to);
			if (from < to) from = Math.Min(from + vel, to);
			return from;
		}

		public static float lerp(float t, float a, float b) {
			return (1f - t) * a + b * t;
		}
	}

	public class Rectf
	{
		public Rectf() { }
		public Rectf(float x, float y, float w, float h) {
			X = x;
			Y = y;
			Width = w;
			Height = h;
		}

		public float X = 0f;
		public float Y = 0f;
		public float Width = 0f;
		public float Height = 0f;

		public float Left { get { return X; } }
		public float Top { get { return Y; } }

		public Vector2 GetIntersectionDepth(Rectf rectB) {
			Rectf rectA = this;
			// Calculate half sizes.
			float halfWidthA = rectA.Width / 2.0f;
			float halfHeightA = rectA.Height / 2.0f;
			float halfWidthB = rectB.Width / 2.0f;
			float halfHeightB = rectB.Height / 2.0f;

			// Calculate centers.
			Vector2 centerA = new Vector2(rectA.Left + halfWidthA, rectA.Top + halfHeightA);
			Vector2 centerB = new Vector2(rectB.Left + halfWidthB, rectB.Top + halfHeightB);

			// Calculate current and minimum-non-intersecting distances between centers.
			float distanceX = centerA.X - centerB.X;
			float distanceY = centerA.Y - centerB.Y;
			float minDistanceX = halfWidthA + halfWidthB;
			float minDistanceY = halfHeightA + halfHeightB;

			// If we are not intersecting at all, return (0, 0).
			if (Math.Abs(distanceX) >= minDistanceX || Math.Abs(distanceY) >= minDistanceY)
				return Vector2.Zero;

			// Calculate and return intersection depths.
			float depthX = distanceX > 0 ? minDistanceX - distanceX : -minDistanceX - distanceX;
			float depthY = distanceY > 0 ? minDistanceY - distanceY : -minDistanceY - distanceY;
			return new Vector2(depthX, depthY);
		}
	}
}
