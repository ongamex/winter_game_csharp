using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace MyGame
{
	public class GameUpdateSets
	{
		public float dt = 0f;
		public Level level;
		public Game1 game;
	}

	public class Snowman
	{
		public bool _isFacingRight = true;
		public Vector2 pos = new Vector2(0f, 0f);
		public Vector2 vel = new Vector2(0f, 0f);
		public float walkAnimEvalTime = 0f;
		public bool wasGrounded = false;

		private int jumpCounter = 0;

		public Rectf GetRectWs() {
			Rectf res = new Rectf();
			res.X = pos.X + 8;
			res.Y = pos.Y;
			res.Width = 16f;
			res.Height = 32f;
			return res;
		}

		public void Update(GameUpdateSets u) {
			float jumpHeight = 36f;
			float jumpTimeApex = 0.3f;
			float minJumpHeight = 8f;
			float fallingGravityMultiplier = 1f;
			float gravity = 2f * jumpHeight / (jumpTimeApex * jumpTimeApex);
			float fallingGravity = gravity * fallingGravityMultiplier;
			float maxJumpVelocity = gravity * jumpTimeApex;
			float minJumpVelocity = System.MathF.Sqrt(2f * gravity * minJumpHeight);

			// Update
			if (Keyboard.GetState().IsKeyDown(Keys.Left)) {
				vel.X = MyMath.lerpCLamp(vel.X, -140f, 350 * u.dt);
				walkAnimEvalTime += (float)(u.dt);
				_isFacingRight = false;
			}
			if (Keyboard.GetState().IsKeyDown(Keys.Right)) {
				vel.X = MyMath.lerpCLamp(vel.X, 140f, 350 * u.dt);
				walkAnimEvalTime += (float)(u.dt);
				_isFacingRight = true;
			}

			if ((wasGrounded || jumpCounter == 1) && Keyboard.GetState().IsKeyDown(Keys.Space) && !u.game.oldks.IsKeyDown(Keys.Space)) {
				// pressed
				vel.Y = -maxJumpVelocity;
				jumpCounter++;
				foreach (JumpSwitch tile in u.level._jumpSwitch) {
					tile._isSolid = !tile._isSolid;
				}
			}

			if (!Keyboard.GetState().IsKeyDown(Keys.Space) && u.game.oldks.IsKeyDown(Keys.Space)) {
				// released
				if (vel.Y < -minJumpVelocity) {
					vel.Y = -minJumpVelocity;
				}
			}

			pos += vel * u.dt;

			if (vel.Y < 0)
				vel.Y += gravity * u.dt;
			else
				vel.Y += fallingGravity * u.dt;


			vel.X -= vel.X * 0.05f;

			bool isGrounded = false;
			Rectf snowmanRectGroundCheck = new Rectf(pos.X + 8, pos.Y, 16f, 34f);

			foreach (Tile tile in u.level._tiles) {
				Rectf snowmanRect = new Rectf(pos.X + 8, pos.Y, 16f, 32f);
				Vector2 depth = snowmanRect.GetIntersectionDepth(tile.GetRectWs());
				isGrounded |= snowmanRectGroundCheck.GetIntersectionDepth(tile.GetRectWs()).Y < 0f;
				if (depth.X != 0f && MathF.Abs(depth.X) < MathF.Abs(depth.Y)) {
					pos.X += depth.X;
					vel.X = 0;
				}
				if (depth.Y != 0f && MathF.Abs(depth.Y) < MathF.Abs(depth.X)) {
					pos.Y += depth.Y;
					if (vel.Y > 0f)
						vel.Y = 0;
				}
			}

			foreach (JumpSwitch tile in u.level._jumpSwitch) {

				if (tile._isSolid == false) continue;

				Rectf snowmanRect = new Rectf(pos.X + 8, pos.Y, 16f, 32f);
				Vector2 depth = snowmanRect.GetIntersectionDepth(tile.GetRectWs());
				isGrounded |= snowmanRectGroundCheck.GetIntersectionDepth(tile.GetRectWs()).Y < 0f;
				if (depth.X != 0f && MathF.Abs(depth.X) < MathF.Abs(depth.Y)) {
					pos.X += depth.X;
					vel.X = 0;
				}
				if (depth.Y != 0f && MathF.Abs(depth.Y) < MathF.Abs(depth.X)) {
					pos.Y += depth.Y;
					if (vel.Y > 0f)
						vel.Y = 0;
				}
			}

			foreach (TimeSwitch tile in u.level._timeSwitches) {

				if (tile._isSolid == false) continue;

				Rectf snowmanRect = new Rectf(pos.X + 8, pos.Y, 16f, 32f);
				Vector2 depth = snowmanRect.GetIntersectionDepth(tile.GetRectWs());
				isGrounded |= snowmanRectGroundCheck.GetIntersectionDepth(tile.GetRectWs()).Y < 0f;
				if (depth.X != 0f && MathF.Abs(depth.X) < MathF.Abs(depth.Y)) {
					pos.X += depth.X;
					vel.X = 0;
				}
				if (depth.Y != 0f && MathF.Abs(depth.Y) < MathF.Abs(depth.X)) {
					pos.Y += depth.Y;
					if (vel.Y > 0f)
						vel.Y = 0;
				}
			}

			wasGrounded = isGrounded;
			if (isGrounded) {
				jumpCounter = 0;
			}

			Camera cam = u.level._camera;

			if (cam.pos.X - pos.X < -50f) {
				cam.pos.X = pos.X - 50f;
			}

			if (cam.pos.X - pos.X > 150f) {
				cam.pos.X = pos.X + 150f;
			}

			if (cam.pos.Y - (pos.Y + 50f) < -50f) {
				cam.pos.Y = (pos.Y + 50f) - 50f;
			}

			if (cam.pos.Y - pos.Y > 100f) {
				cam.pos.Y = pos.Y + 100f;
			}

			//cam.pos.Y = pos.Y;
		}
	}

	public class Fire
	{
		public Vector2 pos = new Vector2(0f, 0f);
		public float animTime = 0;

		public Rectf GetRectWs() {
			Rectf res = new Rectf();
			res.X = pos.X + 8;
			res.Y = pos.Y;
			res.Width = 24f;
			res.Height = 32f;
			return res;
		}

		public void Update(GameUpdateSets u) {
			animTime += u.dt;
		}
	}

	public class Tile
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

	public class JumpSwitch
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

	public class TimeSwitch
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
			float animLength = u.game._timeSwitchBlueAnim._totalAnimationTime;

			if (timeInCurrentState >= 3f) {
				_isSolid = !_isSolid;
				timeInCurrentState = 0f;
			}
			timeInCurrentState += u.dt;


			if (timeInCurrentState > 2.5f) {
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

	public class Level
	{
		public Camera _camera = new Camera();
		public Snowman _snowman;
		public List<Tile> _tiles = new List<Tile>();
		public List<JumpSwitch> _jumpSwitch = new List<JumpSwitch>();
		public List<TimeSwitch> _timeSwitches = new List<TimeSwitch>();
		public List<Fire> _fires = new List<Fire>();

		public void Update(Game1 game, float dt) {
			GameUpdateSets u = new GameUpdateSets();
			u.dt = dt;
			u.level = this;
			u.game = game;

			if (_snowman != null) {
				_snowman.Update(u);
			}

			foreach (TimeSwitch tile in _timeSwitches) {
				tile.Update(u);
			}

			foreach (Fire f in _fires) {
				f.Update(u);
			}

		}

		public static Level FromFile(string filename) {
			string text = System.IO.File.ReadAllText(filename);
			return FromText(text);
		}

		public static Level FromText(string text) {
			Level level = new Level();

			int xOffset = 0;
			int yOffset = 0;
			foreach (char ch in text) {

				if (ch == 'p' && level._snowman == null) {
					Snowman snowman = new Snowman();

					snowman.pos.X = (float)xOffset * 32f;
					snowman.pos.Y = (float)yOffset * 32f;

					level._snowman = snowman;
				}
				else if (ch == 'f' || ch == 'F') {
					Fire f = new Fire();
					f.pos.X = (float)xOffset * 32f;
					f.pos.Y = (float)yOffset * 32f;

					level._fires.Add(f);
				}
				else if (ch == 'x' || ch == 'X') {
					Tile tile = new Tile();
					tile.pos.X = (float)xOffset * 32f;
					tile.pos.Y = (float)yOffset * 32f;

					level._tiles.Add(tile);
				}
				else if (ch == 'y' || ch == 'Y') {
					JumpSwitch tile = new JumpSwitch(JumpSwitch.Color.Yellow);
					tile.pos.X = (float)xOffset * 32f;
					tile.pos.Y = (float)yOffset * 32f;

					level._jumpSwitch.Add(tile);
				}
				else if (ch == 'g' || ch == 'G') {
					JumpSwitch tile = new JumpSwitch(JumpSwitch.Color.Green);
					tile.pos.X = (float)xOffset * 32f;
					tile.pos.Y = (float)yOffset * 32f;

					level._jumpSwitch.Add(tile);
				}
				else if (ch == 'b' || ch == 'B') {
					TimeSwitch tile = new TimeSwitch(TimeSwitch.Color.Blue);
					tile.pos.X = (float)xOffset * 32f;
					tile.pos.Y = (float)yOffset * 32f;

					level._timeSwitches.Add(tile);
				}
				else if (ch == 'r' || ch == 'R') {
					TimeSwitch tile = new TimeSwitch(TimeSwitch.Color.Red);
					tile.pos.X = (float)xOffset * 32f;
					tile.pos.Y = (float)yOffset * 32f;

					level._timeSwitches.Add(tile);
				}

				if (ch == '\n') {
					xOffset = 0;
					yOffset++;
				}
				else {
					xOffset++;
				}
			}

			return level;
		}
	}
}

