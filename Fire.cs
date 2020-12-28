﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace Game1
{
	public class Fire
	{
		public Vector2 pos = new Vector2(0f, 0f);
		public float animTime = 0;
		public float timeSinceLastProjectalieSpawn = 0f;

		public Rectf GetRectWs() {
			Rectf res = new Rectf();
			res.X = pos.X + 8f;
			res.Y = pos.Y + 16f;
			res.Width = 16f;
			res.Height = 16f;
			return res;
		}

		public void Update(GameUpdateSets u) {
			animTime += u.dt;
			timeSinceLastProjectalieSpawn += u.dt;

			if (timeSinceLastProjectalieSpawn >= 2.5f && (pos - u.level._snowman.pos).Length() < 320f) {
				timeSinceLastProjectalieSpawn = 0;
				FireProjectile p = new FireProjectile();
				Vector2 dir = u.level._snowman.pos - pos;
				if (dir.Length() > 1e-3f) {
					dir.Normalize();
				}
				p.pos = pos + new Vector2(8f, 8f);
				p.vel = dir * 64f;

				u.level._fireProjectiles.Add(p);
			}
		}
	}

	public class FireProjectile
	{
		public Vector2 pos = new Vector2(0f, 0f);
		public Vector2 vel = new Vector2(0f, 0f);
		public float age = 0f;

		public Rectf GetRectWs() {
			Rectf res = new Rectf();
			res.X = pos.X + 2f;
			res.Y = pos.Y + 2f;
			res.Width = 4f;
			res.Height = 4f;
			return res;
		}

		public bool Update(GameUpdateSets u) {
			age += u.dt;
			pos += u.dt * vel;

			return age > 10f;
		}
	}

}
