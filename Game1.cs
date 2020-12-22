using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace MyGame
{
	public class TempTile
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

	public class Game1 : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;
		private Texture2D _snowmanSprite;
		private Texture2D _tileTexture;
		private Vector2 snowmanPos = new Vector2(0f, 0f);
		private Vector2 snowmanVel = new Vector2(0f, 0f);
		private KeyboardState oldks = new KeyboardState();
		SpriteAnimation _snowmanAnimWalk;
		float walkAnimEvalTime = 0f;

		List<TempTile> _tiles;

		Camera _camera = new Camera();

		public static float lerpCLamp(float from, float to, float vel) {
			if (from > to) from = Math.Max(from - vel, to);
			if (from < to) from = Math.Min(from + vel, to);
			return from;
		}

		public Game1() {
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;

			Window.AllowUserResizing = true;
		}

		protected override void Initialize() {
			// TODO: Add your initialization logic here

			base.Initialize();
		}

		protected override void LoadContent() {
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			_snowmanSprite = Content.Load<Texture2D>("snowman");
			_tileTexture = Content.Load<Texture2D>("tile");

			_snowmanAnimWalk = SpriteAnimation.Load("snowman_walk.json", Content);

			_tiles = new List<TempTile>();
			for (int t = 0; t < 10; ++t) {
				if (t % 2 != 0) continue;
				TempTile tile = new TempTile();
				tile.pos.X = (float)t * 32f;
				tile.pos.Y = 64f;
				_tiles.Add(tile);
			}
			_tiles.Add(new TempTile { pos = new Vector2(96f, 32f) });
		}

		protected override void Update(GameTime gameTime) {
			float dt = (float)(gameTime.ElapsedGameTime.TotalSeconds);

			float jumpHeight = 68f;
			float jumpTimeApex = 0.6f;
			float minJumpHeight = 24f;
			float fallingGravityMultiplier = 1.3f;
			float gravity = 2f * jumpHeight / (jumpTimeApex * jumpTimeApex);
			float fallingGravity = gravity * fallingGravityMultiplier;
			float maxJumpVelocity = gravity * jumpTimeApex;
			float minJumpVelocity = MathF.Sqrt(2f * gravity * minJumpHeight);


			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			if (Keyboard.GetState().IsKeyDown(Keys.A))
				_camera.pos.X -= 50f * dt;

			if (Keyboard.GetState().IsKeyDown(Keys.D))
				_camera.pos.X += 50f * dt;

			if (Keyboard.GetState().IsKeyDown(Keys.W))
				_camera.pos.Y -= 50f * dt;

			if (Keyboard.GetState().IsKeyDown(Keys.S))
				_camera.pos.Y += 50f * dt;

			if (Keyboard.GetState().IsKeyDown(Keys.Q))
				_camera.viewHeigthWs -= 50f * dt;

			if (Keyboard.GetState().IsKeyDown(Keys.E))
				_camera.viewHeigthWs += 50f * dt;


			if (Keyboard.GetState().IsKeyDown(Keys.Left)) {
				snowmanVel.X = lerpCLamp(snowmanVel.X, -140f, 350 * dt);
			}
			if (Keyboard.GetState().IsKeyDown(Keys.Right))
				snowmanVel.X = lerpCLamp(snowmanVel.X, 140f, 350 * dt);

			if (Keyboard.GetState().IsKeyDown(Keys.Space) && !oldks.IsKeyDown(Keys.Space)) {
				// pressed
				snowmanVel.Y = -maxJumpVelocity;
			}
			if (!Keyboard.GetState().IsKeyDown(Keys.Space) && oldks.IsKeyDown(Keys.Space)) {
				// released
				if (snowmanVel.Y < -minJumpVelocity) {
					snowmanVel.Y = -minJumpVelocity;
				}
			}

			snowmanPos += snowmanVel * dt;

			if (snowmanVel.Y < 0)
				snowmanVel.Y += gravity * dt;
			else
				snowmanVel.Y += fallingGravity * dt;


			snowmanVel.X -= snowmanVel.X * 0.05f;

			foreach (TempTile tile in _tiles) {
				Rectf snowmanRect = new Rectf(snowmanPos.X + 8, snowmanPos.Y, 16f, 32f);
				Vector2 depth = snowmanRect.GetIntersectionDepth(tile.GetRectWs());
				if(depth.X != 0f && MathF.Abs(depth.X) < MathF.Abs(depth.Y)) {
					snowmanPos.X += depth.X;
					snowmanVel.X = 0;
				}
				if (depth.Y != 0f && MathF.Abs(depth.Y) < MathF.Abs(depth.X)) {
					snowmanPos.Y += depth.Y;
					if(snowmanVel.Y > 0f)
						snowmanVel.Y = 0;
				}
			}



			if (MathF.Abs(snowmanVel.X) > 1f)
				walkAnimEvalTime += (float)(gameTime.ElapsedGameTime.TotalSeconds) * MathF.Sign(snowmanVel.X);
			else
				walkAnimEvalTime = 0f;

			// TODO: Add your update logic here
			oldks = Keyboard.GetState();
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime) {
			GraphicsDevice.Clear(Color.CornflowerBlue);
			Matrix proj = _camera.GetProjectionMatrix(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

			// Draw the snowman
			{
				Rectangle subImage = _snowmanAnimWalk.Evaluate(walkAnimEvalTime);

				Matrix n2w = Matrix.CreateTranslation(snowmanPos.X, snowmanPos.Y, 0f);
				_spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, n2w * proj);
				_spriteBatch.Draw(_snowmanAnimWalk._texture, new Vector2(0, 0), subImage, Color.White);
				_spriteBatch.End();
			}

			// Draw the bauchour
			//for (int t = 0; t < 10; ++t) {
			//	Matrix n2w = Matrix.CreateTranslation((float)t * 32f, 60f, 0f);
			//	_spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, RasterizerState.CullNone, null, n2w * proj);
			//	_spriteBatch.Draw(_tileTexture, new Vector2(0, 0), Color.White);
			//	_spriteBatch.End();
			//}
			foreach(TempTile tile in _tiles) {
				Matrix n2w = Matrix.CreateTranslation((float)tile.pos.X, tile.pos.Y, 0f);
				_spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, RasterizerState.CullNone, null, n2w * proj);
				_spriteBatch.Draw(_tileTexture, new Vector2(0, 0), Color.White);
				_spriteBatch.End();
			}

			base.Draw(gameTime);
		}
	}
}
