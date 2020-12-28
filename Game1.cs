using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Game1
{
	public class Game1 : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;
		private Texture2D whiteTex1;

		private Texture2D _snowmanDeadTex;
		private Texture2D _tileTexture;
		private Texture2D _cityTex;
		private Texture2D _jumpSwitchYellowSolid;
		private Texture2D _jumpSwitchYellowPassable;
		private Texture2D _jumpSwitchGreenSolid;
		private Texture2D _jumpSwitchGreenPassable;
		private Texture2D _fireProjectileTex;
		private Texture2D _ghostyTex;
		private Texture2D _iceSpikeTex;
		private Level _level;
		public KeyboardState oldks;
		public GamePadState oldgs;
		public SpriteAnimation _snowmanAnimWalk;
		public SpriteAnimation _timeSwitchBlueAnim;
		public SpriteAnimation _timeSwitchRedAnim;
		public SpriteAnimation _fireAnim;
		public SpriteAnimation _walkAndBad;

		public Game1() {
			_graphics = new GraphicsDeviceManager(this);
			_graphics.PreferredBackBufferWidth = 1200;
			_graphics.PreferredBackBufferHeight = 720;
			_graphics.ApplyChanges();
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

			whiteTex1 = new Texture2D(GraphicsDevice, 1, 1);
			{
				Color[] data = new Color[1 * 1];
				for (int i = 0; i < data.Length; ++i) data[i] = Color.White;
				whiteTex1.SetData(data);
			}

			_snowmanDeadTex = Content.Load<Texture2D>("snowman_dead");
			_tileTexture = Content.Load<Texture2D>("tile");
			_cityTex = Content.Load<Texture2D>("city");
			_jumpSwitchYellowSolid = Content.Load<Texture2D>("yellowJumpSwitchSolid");
			_jumpSwitchYellowPassable = Content.Load<Texture2D>("yellowJumpSwitchPassable");
			_jumpSwitchGreenSolid = Content.Load<Texture2D>("greenJumpSwitchSolid");
			_jumpSwitchGreenPassable = Content.Load<Texture2D>("greenJumpSwitchPassable");
			_fireProjectileTex = Content.Load<Texture2D>("fireProjectile");
			_ghostyTex = Content.Load<Texture2D>("ghosty");
			_iceSpikeTex = Content.Load<Texture2D>("iceSpike");
			_snowmanAnimWalk = SpriteAnimation.Load("snowman_walk.json", Content);
			_timeSwitchBlueAnim = SpriteAnimation.Load("timeSwitchBlueAnim.json", Content);
			_timeSwitchRedAnim = SpriteAnimation.Load("timeSwitchRedAnim.json", Content);
			_fireAnim = SpriteAnimation.Load("fire.json", Content);
			_walkAndBad = SpriteAnimation.Load("walkAndBad.json", Content);

		}

		protected override void Update(GameTime gameTime) {
			float dt = (float)(gameTime.ElapsedGameTime.TotalSeconds);

			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			_level.Update(this, dt);
			if (_level.shouldRestart) {
				_level = Level.FromText(_level.creationText);
			}

			// TODO: Add your update logic here
			oldks = Keyboard.GetState();
			oldgs = GamePad.GetState(0);
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime) {
			GraphicsDevice.Clear(new Color(117f / 255f, 114f / 255f, 71f / 255f));
			Matrix proj = _level._camera.GetProjectionMatrix(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

			{
				float imageWidthWs = _cityTex.Width * 2f;

				float parallaxShiftXWs = _level._camera.pos.X * 0.3f;
				float parallaxShiftYWs = _level._camera.pos.Y * 0.05f;
				parallaxShiftXWs -= MathF.Floor(parallaxShiftXWs / imageWidthWs) * imageWidthWs;


				
				int k = (int)(_level._camera.viewRectWs.X / imageWidthWs) - 1;
				int numRepeatsNeededToCovertTheScreen = (int)(_level._camera.viewRectWs.Width / imageWidthWs) + 3;

				for (int t = 0; t < numRepeatsNeededToCovertTheScreen; ++t) {
					Matrix n2w = Matrix.CreateScale(2f, 2f, 1f) * Matrix.CreateTranslation((float)(t + k) * imageWidthWs + parallaxShiftXWs, _level.minYPointWs - _cityTex.Height * 2f + parallaxShiftYWs, 0f);
					_spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, RasterizerState.CullNone, null, n2w * proj);
					_spriteBatch.Draw(_cityTex, new Vector2(0, 0), Color.White);
					_spriteBatch.End();
				}
				{
					Vector2 pos = new Vector2();
					pos.X = _level._camera.viewRectWs.X;
					pos.Y = _level.minYPointWs;
					Matrix n2w = Matrix.CreateScale(_level._camera.viewRectWs.Width, 320f, 1f) * Matrix.CreateTranslation(pos.X, pos.Y + parallaxShiftYWs, 0f);
					_spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, RasterizerState.CullNone, null,  n2w * proj);
					_spriteBatch.Draw(whiteTex1, new Vector2(0, 0), new Color(45f / 255f, 27f / 255f, 30f / 255f));
					_spriteBatch.End();
				}
			}

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
				}
				else {
					Rectangle frame = _timeSwitchRedAnim.Evaluate(tile.animEvalTime, false);
					_spriteBatch.Draw(_timeSwitchRedAnim._texture, new Vector2(0, 0), frame, tile.tint);
				}
				_spriteBatch.End();
			}

			foreach (Ghosty g in _level.ghosties) {
				Matrix n2w = Matrix.Identity;
				if (g.isLookingRight) {
					n2w = Matrix.CreateTranslation(-32f, 0f, 0f) * Matrix.CreateScale(-1f, 1f, 1f);
				}
				n2w = n2w * Matrix.CreateTranslation(g.pos.X, g.pos.Y, 0f);
				_spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, RasterizerState.CullNone, null, n2w * proj);
				_spriteBatch.Draw(_ghostyTex, new Vector2(0, 0), Color.White);
				_spriteBatch.End();
			}

			foreach (IceSpike g in _level.iceSpikes) {

				Matrix n2w = Matrix.CreateTranslation(g.pos.X, g.pos.Y, 0f);
				_spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, RasterizerState.CullNone, null, n2w * proj);
				_spriteBatch.Draw(_iceSpikeTex, new Vector2(0, 0), Color.White);
				_spriteBatch.End();
			}

			foreach (WalkAndBad f in _level.walkAndBads) {
				Matrix n2w = Matrix.CreateTranslation((float)f.pos.X, f.pos.Y, 0f);
				_spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, RasterizerState.CullNone, null, n2w * proj);
				Rectangle frame = _walkAndBad.Evaluate(f.animEvalTime);
				_spriteBatch.Draw(_walkAndBad._texture, new Vector2(0, 0), frame, Color.White);
				_spriteBatch.End();
			}

			// Draw the snowman
			{

				Matrix n2w = Matrix.Identity;
				if (_level._snowman._isFacingRight == false && !_level._snowman.isDead) {
					n2w = Matrix.CreateTranslation(-32f, 0f, 0f) * Matrix.CreateScale(-1f, 1f, 1f);
				}
				n2w = n2w * Matrix.CreateTranslation(_level._snowman.pos.X, _level._snowman.pos.Y, 0f);
				if (_level._snowman.isDead) {
					_spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, RasterizerState.CullNone, null, n2w * proj);
					_spriteBatch.Draw(_snowmanDeadTex, new Vector2(0, 0), Color.White);
					_spriteBatch.End();
				}
				else {
					Rectangle subImage = _snowmanAnimWalk.Evaluate(_level._snowman.walkAnimEvalTime);

					_spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, RasterizerState.CullNone, null, n2w * proj);
					_spriteBatch.Draw(_snowmanAnimWalk._texture, new Vector2(0, 0), subImage, Color.White);
					_spriteBatch.End();
				}
			}

			foreach (var f in _level._fireProjectiles) {
				Matrix n2w = Matrix.CreateTranslation(-8f, -8f, 0f) * Matrix.CreateRotationZ(5f * f.age) * Matrix.CreateTranslation(8f, 8f, 0f) * Matrix.CreateTranslation((float)f.pos.X, f.pos.Y, 0f);
				_spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, RasterizerState.CullNone, null, n2w * proj);
				_spriteBatch.Draw(_fireProjectileTex, new Vector2(0, 0), Color.White);
				_spriteBatch.End();
			}

			if (_level._snowman.isDead) {
				_spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, RasterizerState.CullNone, null);
				//_spriteBatch.Draw(null, new Rectangle(0, 0, 500, 500), new Color(0f, 0f, 0f,1f - _level._snowman.timeSpentDead));
				_spriteBatch.End();

			}

			base.Draw(gameTime);
		}
	}
}
