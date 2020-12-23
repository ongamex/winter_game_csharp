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
		public Vector2 pos = new Vector2(0f, 0f);
		public Vector2 vel = new Vector2(0f, 0f);
		public float walkAnimEvalTime = 0f;

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
			float minJumpHeight = 12f;
			float fallingGravityMultiplier = 1f;
			float gravity = 2f * jumpHeight / (jumpTimeApex * jumpTimeApex);
			float fallingGravity = gravity * fallingGravityMultiplier;
			float maxJumpVelocity = gravity * jumpTimeApex;
			float minJumpVelocity = System.MathF.Sqrt(2f * gravity * minJumpHeight);

			// Update
			if (Keyboard.GetState().IsKeyDown(Keys.Left)) {
				vel.X = MyMath.lerpCLamp(vel.X, -140f, 350 * u.dt);
				walkAnimEvalTime -= (float)(u.dt);
			}
			if (Keyboard.GetState().IsKeyDown(Keys.Right)) {
				vel.X = MyMath.lerpCLamp(vel.X, 140f, 350 * u.dt);
				walkAnimEvalTime += (float)(u.dt);
			}

			if (Keyboard.GetState().IsKeyDown(Keys.Space) && !u.game.oldks.IsKeyDown(Keys.Space)) {
				// pressed
				vel.Y = -maxJumpVelocity;
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

			foreach (Tile tile in u.level._tiles) {
				Rectf snowmanRect = new Rectf(pos.X + 8, pos.Y, 16f, 32f);
				Vector2 depth = snowmanRect.GetIntersectionDepth(tile.GetRectWs());
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

			u.level._camera.pos = pos;
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

	public class Level
	{
		public Camera _camera = new Camera();
		public Snowman _snowman;
		public List<Tile> _tiles = new List<Tile>();
		public List<JumpSwitch> _jumpSwitch = new List<JumpSwitch>();

		public void Update(Game1 game, float dt) {
			GameUpdateSets u = new GameUpdateSets();
			u.dt = dt;
			u.level = this;
			u.game = game;

			if (_snowman != null) {
				_snowman.Update(u);
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

