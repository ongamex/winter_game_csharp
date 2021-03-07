using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace Game1
{
	public class Walker : IHasRectWs
	{
		public Vector2 pos = new Vector2(0f, 0f);
		public bool isWalkingRight = true;
		public float animEvalTime = 0f;

		public Rectf GetRectWs() {
			Rectf res = new Rectf();
			res.X = pos.X + 8;
			res.Y = pos.Y + 16f;
			res.Width = 16f;
			res.Height = 16f;
			return res;
		}

		public void Update(GameUpdateSets u) {

			animEvalTime += u.dt;

			if (u.level.snowman.isDead) {
				return;
			}

			Rectf groundSearchRect = new Rectf();
			groundSearchRect.X = MathF.Floor(pos.X / 32f) * 32f + (isWalkingRight ? 32f : 0f );
			groundSearchRect.Y = (float)(1 + (int)(pos.Y / 32f)) * 32f;
			groundSearchRect.Width = 32f;
			groundSearchRect.Height = 32f;

			bool hasGroundTowardsNextPos = false;
			foreach (Tile tile in u.level.tiles) {
				Rectf snowmanRect = GetRectWs();
				Vector2 depth = snowmanRect.GetIntersectionDepth(tile.GetRectWs());
				hasGroundTowardsNextPos |= groundSearchRect.GetIntersectionDepth(tile.GetRectWs()) != Vector2.Zero;
				if (depth.X != 0f && MathF.Abs(depth.X) < MathF.Abs(depth.Y)) {
					pos.X += depth.X;
					isWalkingRight = !isWalkingRight;
				}
			}

			
			pos.X += (isWalkingRight ? 1f : -1f) * 32f * u.dt;

			if(hasGroundTowardsNextPos == false) {
				isWalkingRight = !isWalkingRight;
			}
		}
	}
}
