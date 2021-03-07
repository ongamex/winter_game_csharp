using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace Game1
{
	public class Ghosty : IHasRectWs
	{
		public Ghosty(Vector2 p) {
			pos = p;
			waitingPos = p;
		}

		enum State
		{
			WaitingForPlayer,
			ChasingPlayer
		}

		public Vector2 waitingPos = new Vector2(0f, 0f);
		public Vector2 pos = new Vector2(0f, 0f);
		public bool isLookingRight = true;
		State state = State.WaitingForPlayer;

		public Rectf GetRectWs() {
			Rectf res = new Rectf();
			res.X = pos.X + 10f;
			res.Y = pos.Y + 8f;
			res.Width = 8f;
			res.Height = 8f;
			return res;
		}

		public void Update(GameUpdateSets u) {

			if(u.level.snowman.isDead) {
				return;
			}

			Vector2 targetPos = new Vector2(0f, 0f);
			Vector2 toSnowman = u.level.snowman.pos - pos;

			if (state == State.WaitingForPlayer) {
				targetPos = waitingPos;
				if ((pos - waitingPos).Length() < 8f && toSnowman.Length() < 160f) {
					state = State.ChasingPlayer;
				}
			}
			else if (state == State.ChasingPlayer) {
				if ((pos - waitingPos).Length() > 160f) {
					state = State.WaitingForPlayer;
				}
				targetPos = u.level.snowman.pos + new Vector2(0f,12f);
			}


			Vector2 diff = targetPos - pos;
			float diffL = diff.Length();
			float speed = MathF.Max(0.75f * diffL, 48f) * u.dt;

			if (diffL < speed) {
				pos = targetPos;
			}
			else if (diffL > 1e-6f) {
				pos += diff * (speed / diffL);
			}


			isLookingRight = diff.X > 0f;
		}
	}
}
