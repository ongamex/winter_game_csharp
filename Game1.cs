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
	public class Game1 : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;
		private Texture2D _snowmanSprite;
		private Texture2D _tileTexture;
		private Texture2D _jumpSwitchYellowSolid;
		private Texture2D _jumpSwitchYellowPassable;
		private Texture2D _jumpSwitchGreenSolid;
		private Texture2D _jumpSwitchGreenPassable;
		private Level _level;
		public KeyboardState oldks;
		Camera _camera = new Camera();
		public SpriteAnimation _snowmanAnimWalk;
		public SpriteAnimation _timeSwitchBlueAnim;
		public SpriteAnimation _timeSwitchRedAnim;
		public SpriteAnimation _fireAnim;

		public Game1() {
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;

			Window.AllowUserResizing = true;
		}

		protected override void Initialize() {
			// TODO: Add your initialization logic here

			base.Initialize();
			_level = Level.FromFile("level_1.txt");
		}

		protected override void LoadContent() {
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			_snowmanSprite = Content.Load<Texture2D>("snowman");
			_tileTexture = Content.Load<Texture2D>("tile");
			_jumpSwitchYellowSolid = Content.Load<Texture2D>("yellowJumpSwitchSolid");
			_jumpSwitchYellowPassable = Content.Load<Texture2D>("yellowJumpSwitchPassable");
			_jumpSwitchGreenSolid = Content.Load<Texture2D>("greenJumpSwitchSolid");
			_jumpSwitchGreenPassable = Content.Load<Texture2D>("greenJumpSwitchPassable");
			_snowmanAnimWalk = SpriteAnimation.Load("snowman_walk.json", Content);
			_timeSwitchBlueAnim = SpriteAnimation.Load("timeSwitchBlueAnim.json", Content);
			_timeSwitchRedAnim = SpriteAnimation.Load("timeSwitchRedAnim.json", Content);
			_fireAnim = SpriteAnimation.Load("fire.json", Content);
		}

		protected override void Update(GameTime gameTime) {
			float dt = (float)(gameTime.ElapsedGameTime.TotalSeconds);

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

			_level.Update(this, dt);

			// TODO: Add your update logic here
			oldks = Keyboard.GetState();
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime) {
			GraphicsDevice.Clear(Color.CornflowerBlue);
			Matrix proj = _level._camera.GetProjectionMatrix(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);


			foreach (Fire f in _level._fires) {
				Matrix n2w = Matrix.CreateTranslation((float)f.pos.X, f.pos.Y, 0f);
				_spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, RasterizerState.CullNone, null, n2w * proj);
				Rectangle frame = _fireAnim.Evaluate(f.animTime);
				_spriteBatch.Draw(_fireAnim._texture, new Vector2(0, 0), frame, Color.White);
				_spriteBatch.End();
			}

			foreach (Tile tile in _level._tiles) {
				Matrix n2w = Matrix.CreateTranslation((float)tile.pos.X, tile.pos.Y, 0f);
				_spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, RasterizerState.CullNone, null, n2w * proj);
				_spriteBatch.Draw(_tileTexture, new Vector2(0, 0), Color.White);
				_spriteBatch.End();
			}

			foreach (JumpSwitch tile in _level._jumpSwitch) {
				Matrix n2w = Matrix.CreateTranslation((float)tile.pos.X, tile.pos.Y, 0f);
				_spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, RasterizerState.CullNone, null, n2w * proj);
				if (tile._isSolid)
					_spriteBatch.Draw(tile._color == JumpSwitch.Color.Yellow ? _jumpSwitchYellowSolid : _jumpSwitchGreenSolid, new Vector2(0, 0), Color.White);
				else
					_spriteBatch.Draw(tile._color == JumpSwitch.Color.Yellow ? _jumpSwitchYellowPassable : _jumpSwitchGreenPassable, new Vector2(0, 0), Color.White);

				_spriteBatch.End();
			}

			foreach (TimeSwitch tile in _level._timeSwitches) {
				Matrix n2w = Matrix.CreateTranslation((float)tile.pos.X, tile.pos.Y, 0f);
				_spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, RasterizerState.CullNone, null, n2w * proj);
				if (tile._color == TimeSwitch.Color.Blue) {
					Rectangle frame = _timeSwitchBlueAnim.Evaluate(tile.animEvalTime, false);
					_spriteBatch.Draw(_timeSwitchBlueAnim._texture, new Vector2(0, 0), frame, tile.tint);
				} else {
					Rectangle frame = _timeSwitchRedAnim.Evaluate(tile.animEvalTime, false);
					_spriteBatch.Draw(_timeSwitchRedAnim._texture, new Vector2(0, 0), frame, tile.tint);
				}
				_spriteBatch.End();
			}

			// Draw the snowman
			{
				Rectangle subImage = _snowmanAnimWalk.Evaluate(_level._snowman.walkAnimEvalTime);

				Matrix n2w = Matrix.Identity;
				if (_level._snowman._isFacingRight == false) {
					n2w = Matrix.CreateTranslation(-32f, 0f, 0f) * Matrix.CreateScale(-1f, 1f, 1f);
				}
				n2w = n2w * Matrix.CreateTranslation(_level._snowman.pos.X, _level._snowman.pos.Y, 0f);
				_spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, RasterizerState.CullNone, null, n2w * proj);
				_spriteBatch.Draw(_snowmanAnimWalk._texture, new Vector2(0, 0), subImage, Color.White);
				_spriteBatch.End();
			}



			base.Draw(gameTime);
		}
	}
}
