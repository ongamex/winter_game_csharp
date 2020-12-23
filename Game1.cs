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
		SpriteAnimation _snowmanAnimWalk;

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

			// Draw the snowman
			{
				Rectangle subImage = _snowmanAnimWalk.Evaluate(_level._snowman.walkAnimEvalTime);

				Matrix n2w = Matrix.CreateTranslation(_level._snowman.pos.X, _level._snowman.pos.Y, 0f);
				_spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, n2w * proj);
				_spriteBatch.Draw(_snowmanAnimWalk._texture, new Vector2(0, 0), subImage, Color.White);
				_spriteBatch.End();
			}

		
			foreach(Tile tile in _level._tiles) {
				Matrix n2w = Matrix.CreateTranslation((float)tile.pos.X, tile.pos.Y, 0f);
				_spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, RasterizerState.CullNone, null, n2w * proj);
				_spriteBatch.Draw(_tileTexture, new Vector2(0, 0), Color.White);
				_spriteBatch.End();
			}

			foreach (JumpSwitch tile in _level._jumpSwitch) {
				Matrix n2w = Matrix.CreateTranslation((float)tile.pos.X, tile.pos.Y, 0f);
				_spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, RasterizerState.CullNone, null, n2w * proj);
				if(tile._isSolid)
					_spriteBatch.Draw(tile._color == JumpSwitch.Color.Yellow ? _jumpSwitchYellowSolid : _jumpSwitchGreenSolid, new Vector2(0, 0), Color.White);
				else
					_spriteBatch.Draw(tile._color == JumpSwitch.Color.Yellow ?  _jumpSwitchYellowPassable : _jumpSwitchGreenPassable, new Vector2(0, 0), Color.White);

				_spriteBatch.End();
			}


			base.Draw(gameTime);
		}
	}
}
