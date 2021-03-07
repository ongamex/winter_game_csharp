using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace Game1
{
	public class IceSpike : IHasRectWs
	{
		public Vector2 pos = new Vector2(0f, 0f);
		public Vector2 vel = new Vector2(0f, 0f);
		public bool isFalling = false;

		public Rectf GetRectWs() {
			Rectf res = new Rectf();
			res.X = pos.X + 10;
			res.Y = pos.Y;
			res.Width = 14f;
			res.Height = 28f;
			return res;
		}

		public void Update(GameUpdateSets u) {

			if (u.level.snowman.isDead) {
				return;
			}

			if (MathF.Abs(u.level.snowman.pos.X - pos.X) <= 32f) {
				isFalling = true;
			}

			if(isFalling && pos.Y < 1000f) {
				vel.Y += 800f * u.dt;
				pos += vel * u.dt;
			}
		}
	}
}
