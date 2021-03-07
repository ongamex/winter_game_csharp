using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Game1
{
	public class LetterBox : IHasRectWs
	{
		public Vector2 pos = new Vector2(0f, 0f);

		public Rectf GetRectWs() {
			Rectf res = new Rectf();
			res.X = pos.X+8;
			res.Y = pos.Y;
			res.Width = 16f;
			res.Height = 32f;
			return res;
		}
	}

	public class Letter : IHasRectWs
	{
		public bool isCollected = false;
		public Vector2 pos = new Vector2(0f, 0f);
		public float timeSpentCollected = 0f;

		public Rectf GetRectWs() {
			Rectf res = new Rectf();
			res.X = pos.X;
			res.Y = pos.Y;
			res.Width = 32f;
			res.Height = 32f;
			return res;
		}

		public void Update(GameUpdateSets u) {
			if(isCollected) {
				timeSpentCollected += u.dt;
				Vector2 diff = (u.level.snowman.pos - pos);
				if(diff.Length() > 1e-6f) {
					diff.Normalize();
				}
				float t = MathF.Pow(timeSpentCollected * 2f, 2f) * u.dt * 64f;
				pos.X = MyMath.lerpClamp(pos.X, u.level.snowman.pos.X, t * MathF.Abs(diff.X));
				pos.Y = MyMath.lerpClamp(pos.Y, u.level.snowman.pos.Y, t * MathF.Abs(diff.Y));
			}
		}
	}
}
