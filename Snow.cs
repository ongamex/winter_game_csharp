using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Game1
{
	public class Snow
	{
		public class Flake
		{
			public Vector2 pos = new Vector2();
			public Vector2 vel = new Vector2();
			public float phase = 0;
			public float phaseShift = 0;
			public float age = 0f;
		}

		public float simWidth = -0f;
		public float simHeight = 0f;

		public List<Flake> flakes = new List<Flake>();

		public Snow(float simWidth, float simHeight, int numParticles) {
			this.simWidth = simWidth;
			this.simHeight = simHeight;
			Random rnd = new Random();

			for(int t = 0; t < numParticles; ++t) {
				Flake f = new Flake();

				f.pos.X = (float)(rnd.NextDouble()) * simWidth;
				f.pos.Y = (float)(rnd.NextDouble()) * simHeight;

				//f.vel.X = 16f + (float)(rnd.NextDouble() * 2f - 1f) * 64f;
				f.vel.Y = 16f + (float)(rnd.NextDouble()) * 16f;

				f.phase = 0.5f + 0.5f * (float)rnd.NextDouble();
				f.phaseShift = (float)rnd.NextDouble() * 3.14f * 2f;

				flakes.Add(f);
			}
		}

		public void update(float dt) {
			foreach(var f in flakes) {
				f.age += dt;
				f.pos += f.vel * dt;
				f.vel.X += MathF.Sin(f.age * f.phase + f.phaseShift) * dt * 8f;

				f.pos.X = f.pos.X % simWidth;
				f.pos.Y = f.pos.Y % simWidth;
			}
		}		
	}
}
