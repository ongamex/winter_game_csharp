using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace Game1
{
	public class Tile : IHasRectWs
	{
		public Vector2 pos = new Vector2(0f, 0f);

		public Rectf GetRectWs() {
			Rectf res = new Rectf();
			res.X = pos.X;
			res.Y = pos.Y;
			res.Width = 32f;
			res.Height = 32f;
			return res;
		}
	}

	public class OneWayTile : IHasRectWs
	{
		public Vector2 pos = new Vector2(0f, 0f);

		public Rectf GetRectWs() {
			Rectf res = new Rectf();
			res.X = pos.X;
			res.Y = pos.Y;
			res.Width = 32f;
			res.Height = 4f;
			return res;
		}
	}

	public class JumpSwitch : IHasRectWs
	{
		public enum Color { Yellow, Green };

		public JumpSwitch(Color c) {
			_color = c;
			_isSolid = _color == Color.Yellow;
		}

		public Rectf GetRectWs() {
			Rectf res = new Rectf();
			res.X = pos.X;
			res.Y = pos.Y;
			res.Width = 32f;
			res.Height = 32f;
			return res;
		}

		public Color _color = Color.Yellow;
		public bool _isSolid = false;
		public Vector2 pos = new Vector2();
	}

	public class TimeSwitch : IHasRectWs
	{
		public enum Color { Blue, Red };

		public TimeSwitch(Color c) {
			_color = c;
			_isSolid = _color == Color.Blue;
		}

		public Rectf GetRectWs() {
			Rectf res = new Rectf();
			res.X = pos.X;
			res.Y = pos.Y;
			res.Width = 32f;
			res.Height = 32f;
			return res;
		}

		public void Update(GameUpdateSets u) {
			float animLength = u.game.timeswitchBlueAnim._totalAnimationTime;

			if (timeInCurrentState >= 2f) {
				_isSolid = !_isSolid;
				timeInCurrentState = 0f;
			}
			timeInCurrentState += u.dt;


			if (timeInCurrentState > 1.5f) {
				int k = (int)((timeInCurrentState - 2.5f) / 0.1f);
				if (k % 2 == 0) {
					tint = new Microsoft.Xna.Framework.Color(0.5f, 0.5f, 0.5f);
				}
				else {
					tint = new Microsoft.Xna.Framework.Color(1f, 1f, 1f);
				}
			}
			else {
				tint = new Microsoft.Xna.Framework.Color(1f, 1f, 1f);
			}

			animEvalTime += _isSolid ? -u.dt : u.dt;
			animEvalTime = Math.Clamp(animEvalTime, 0f, animLength);
		}

		public Color _color = Color.Blue;
		public bool _isSolid = false;
		public Vector2 pos = new Vector2();
		public Microsoft.Xna.Framework.Color tint = new Microsoft.Xna.Framework.Color(1f, 1f, 1f);

		public float animEvalTime = 0f;
		public float timeInCurrentState = 0;
	}
}
