using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Game1
{
	public class Game1 : Game
	{
		public enum GameState
		{
			WelcomeScreen,
			Playing,
			EndScreen,
		}

		float timeMult = 1f;

		Camera welcomScreenCamera = new Camera();
		Camera endScreenCamera = new Camera();

		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;

		private Texture2D whiteTex1;

		private Texture2D welcomeTex;
		private Texture2D gameCompleteTex;

		private Texture2D snowmanDeadTex;
		private Texture2D snowmandCrouchTex;
		private Texture2D snowmandWinTex;
		private Texture2D tileTex;
		private Texture2D cityTex;
		private Texture2D mountTex;
		private Texture2D snowflake0;
		private Texture2D jumpswitchYellowSolid;
		private Texture2D jumpswitchYellowPassable;
		private Texture2D _jumpSwitchGreenSolid;
		private Texture2D _jumpSwitchGreenPassable;
		private Texture2D _fireProjectileTex;
		private Texture2D ghostyTex;
		private Texture2D iceSpikeTex;
		private Texture2D letterBoxTex;
		private Texture2D letterBoxPromptTex;
		private Texture2D letterBoxPromptReadyTex;
		private Texture2D letterTex;

		public SpriteAnimation snowmanWalkAnim;
		public SpriteAnimation timeswitchBlueAnim;
		public SpriteAnimation timeswitchRedAnim;
		public SpriteAnimation fileAnim;
		public SpriteAnimation walkerAnim;

		private Song song;
		public SoundEffect jumpSfx;
		public SoundEffect hitSfx;
		public SoundEffect pickupSfx;
		public SoundEffect levelWinSfx;

		GameState gamestate;
		float timeSpentInEndScreen = 0;
		int currentLevel = 0;
		List<string> allLevelFilenames = new List<string>();

		private Level level;
		public KeyboardState oldks;
		public GamePadState oldgs;
		public Snow snow = new Snow(640f, 640f, 10);

		public Game1() {
			_graphics = new GraphicsDeviceManager(this);
			_graphics.PreferredBackBufferWidth = 1200;
			_graphics.PreferredBackBufferHeight = 720;
			_graphics.ApplyChanges();
			Content.RootDirectory = "Content";
			IsMouseVisible = true;

			Window.AllowUserResizing = true;

			welcomScreenCamera.viewHeigthWs = 560f;
			endScreenCamera.viewHeigthWs = 220f;
		}

		protected override void Initialize() {
			// TODO: Add your initialization logic here

			for (int t = 0; true; ++t) {
				string filename = String.Format("Content/levels/level_{0}.txt", t);
				if (File.Exists(filename)) {
					allLevelFilenames.Add(filename);
				}
				else {
					break;
				}
			}

			base.Initialize();
		}

		protected override void LoadContent() {
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			whiteTex1 = new Texture2D(GraphicsDevice, 1, 1);
			{
				Color[] data = new Color[1 * 1];
				for (int i = 0; i < data.Length; ++i) data[i] = Color.White;
				whiteTex1.SetData(data);
			}

			song = Content.Load<Song>("snd/Slow Stride Loop");
			MediaPlayer.IsRepeating = true;
			MediaPlayer.Play(song);
			jumpSfx = Content.Load<SoundEffect>("snd/jump");
			hitSfx = Content.Load<SoundEffect>("snd/hit");
			pickupSfx = Content.Load<SoundEffect>("snd/pickup");
			levelWinSfx = Content.Load<SoundEffect>("snd/levelWin");

			welcomeTex = Content.Load<Texture2D>("welcomeImage");
			gameCompleteTex = Content.Load<Texture2D>("gameComplete");

			snowmanDeadTex = Content.Load<Texture2D>("snowman_dead");
			snowmandCrouchTex = Content.Load<Texture2D>("snowman_crouch");
			snowmandWinTex = Content.Load<Texture2D>("snowman_win");
			tileTex = Content.Load<Texture2D>("tile");
			cityTex = Content.Load<Texture2D>("city");
			mountTex = Content.Load<Texture2D>("mount");
			snowflake0 = Content.Load<Texture2D>("snowflake0");
			jumpswitchYellowSolid = Content.Load<Texture2D>("yellowJumpSwitchSolid");
			jumpswitchYellowPassable = Content.Load<Texture2D>("yellowJumpSwitchPassable");
			_jumpSwitchGreenSolid = Content.Load<Texture2D>("greenJumpSwitchSolid");
			_jumpSwitchGreenPassable = Content.Load<Texture2D>("greenJumpSwitchPassable");
			_fireProjectileTex = Content.Load<Texture2D>("fireProjectile");
			ghostyTex = Content.Load<Texture2D>("ghosty");
			iceSpikeTex = Content.Load<Texture2D>("iceSpike");
			letterBoxTex = Content.Load<Texture2D>("letterBox");
			letterBoxPromptTex = Content.Load<Texture2D>("letterBoxPrompt");
			letterBoxPromptReadyTex = Content.Load<Texture2D>("letterBoxPromptReady");
			letterTex = Content.Load<Texture2D>("letter");
			snowmanWalkAnim = SpriteAnimation.Load("Content/snowman_walk.json", Content);
			timeswitchBlueAnim = SpriteAnimation.Load("Content/timeSwitchBlueAnim.json", Content);
			timeswitchRedAnim = SpriteAnimation.Load("Content/timeSwitchRedAnim.json", Content);
			fileAnim = SpriteAnimation.Load("Content/fire.json", Content);
			walkerAnim = SpriteAnimation.Load("Content/walkAndBad.json", Content);
		}

		// Logic update.
		protected override void Update(GameTime gameTime) {
			float dt = (float)(gameTime.ElapsedGameTime.TotalSeconds);

			if (gamestate == GameState.WelcomeScreen) {
				if (Keyboard.GetState().GetPressedKeyCount() != 0 || GamePad.GetState(0).IsButtonDown(Buttons.A)) {
					gamestate = GameState.Playing;
				}
			}
			else if (gamestate == GameState.EndScreen) {
				timeSpentInEndScreen += dt;

				if (timeSpentInEndScreen > 3f) {
					timeMult += 0.3f;
					timeSpentInEndScreen = 0f;
					gamestate = GameState.WelcomeScreen;
				}
			}
			else if (gamestate == GameState.Playing) {
				snow.update(dt);

				if (level == null) {

					if (currentLevel >= allLevelFilenames.Count) {
						currentLevel = 0;
						gamestate = GameState.EndScreen;
					}

					this.Window.Title = String.Format("Snowtime Delivery - Level {0}", currentLevel + 1);
					level = Level.FromFile(allLevelFilenames[currentLevel]);
				}

				if (level != null && gamestate == GameState.Playing) {
					level.Update(this, dt * timeMult);
					if (level.shouldRestart) {
						level = Level.FromFile(allLevelFilenames[currentLevel]);
					}

					if (level.isComplete && level.timeSpentComplete > 2f) {
						level = null;
						currentLevel++;
					}
				}
			}


			// Cache the input state in order to find presses and releases.
			oldks = Keyboard.GetState();
			oldgs = GamePad.GetState(0);
			base.Update(gameTime);
		}

		// Generic Draw.
		protected override void Draw(GameTime gameTime) {

			GraphicsDevice.Clear(new Color(38f / 255f, 43f / 255f, 68f / 255f));

			if (gamestate == GameState.WelcomeScreen) {

				Matrix proj = welcomScreenCamera.GetProjectionMatrix(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

				Matrix n2w = Matrix.CreateTranslation(-welcomeTex.Width * 0.5f, -welcomeTex.Height * 0.5f, 0f);
				_spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, RasterizerState.CullNone, null, n2w * proj);
				_spriteBatch.Draw(welcomeTex, new Vector2(0, 0), Color.White);
				_spriteBatch.End();

			}
			else if (gamestate == GameState.EndScreen) {

				Matrix proj = endScreenCamera.GetProjectionMatrix(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

				Matrix n2w = Matrix.CreateTranslation(-gameCompleteTex.Width * 0.5f, -gameCompleteTex.Height * 0.5f, 0f);
				_spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, RasterizerState.CullNone, null, n2w * proj);
				_spriteBatch.Draw(gameCompleteTex, new Vector2(0, 0), Color.White);
				_spriteBatch.End();

			}
			else if (gamestate == GameState.Playing) {
				DrawLevel(gameTime);
			}

			base.Draw(gameTime);
		}

		// Draw the Level.
		protected void DrawLevel(GameTime gameTime) {

			if (level == null) {
				return;
			}

			Matrix proj = level.camera.GetProjectionMatrix(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

			float totalSeconds = (float)gameTime.TotalGameTime.TotalSeconds;

			{
				float imageWidthWs = mountTex.Width * 2f;

				float parallaxShiftXWs = level.camera.pos.X * 0.6f;
				float parallaxShiftYWs = level.camera.pos.Y * 0.05f - 32f + 4f;
				parallaxShiftXWs -= MathF.Floor(parallaxShiftXWs / imageWidthWs) * imageWidthWs;



				int k = (int)(level.camera.viewRectWs.X / imageWidthWs) - 2;
				int numRepeatsNeededToCovertTheScreen = (int)(level.camera.viewRectWs.Width / imageWidthWs) + 4;

				for (int t = 0; t < numRepeatsNeededToCovertTheScreen; ++t) {
					Matrix n2w = Matrix.CreateScale(2f, 2f, 1f) * Matrix.CreateTranslation((float)(t + k) * imageWidthWs + parallaxShiftXWs, level.minYPointWs - mountTex.Height * 2f + parallaxShiftYWs, 0f);
					_spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, RasterizerState.CullNone, null, n2w * proj);
					_spriteBatch.Draw(mountTex, new Vector2(0, 0), Color.White);
					_spriteBatch.End();
				}
			}

			// City
			{
				float imageWidthWs = cityTex.Width * 2f;

				float parallaxShiftXWs = level.camera.pos.X * 0.3f;
				float parallaxShiftYWs = level.camera.pos.Y * 0.05f - 8f;
				parallaxShiftXWs -= MathF.Floor(parallaxShiftXWs / imageWidthWs) * imageWidthWs;

				int k = (int)(level.camera.viewRectWs.X / imageWidthWs) - 2;
				int numRepeatsNeededToCovertTheScreen = (int)(level.camera.viewRectWs.Width / imageWidthWs) + 4;

				for (int t = 0; t < numRepeatsNeededToCovertTheScreen; ++t) {
					Matrix n2w = Matrix.CreateScale(2f, 2f, 1f) * Matrix.CreateTranslation((float)(t + k) * imageWidthWs + parallaxShiftXWs, level.minYPointWs - cityTex.Height * 2f + parallaxShiftYWs, 0f);
					_spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, RasterizerState.CullNone, null, n2w * proj);
					_spriteBatch.Draw(cityTex, new Vector2(0, 0), Color.White);
					_spriteBatch.End();
				}
				{
					Vector2 pos = new Vector2();
					pos.X = level.camera.viewRectWs.X;
					pos.Y = level.minYPointWs;
					Matrix n2w = Matrix.CreateScale(level.camera.viewRectWs.Width, 320f, 1f) * Matrix.CreateTranslation(pos.X, pos.Y + parallaxShiftYWs, 0f);
					_spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, RasterizerState.CullNone, null, n2w * proj);
					_spriteBatch.Draw(whiteTex1, new Vector2(0, 0), new Color(32f / 255f, 26f / 255f, 20f / 255f));
					_spriteBatch.End();
				}
			}



			foreach (Fire f in level.fires) {
				Matrix n2w = Matrix.CreateTranslation((float)f.pos.X, f.pos.Y, 0f);
				_spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, RasterizerState.CullNone, null, n2w * proj);
				Rectangle frame = fileAnim.Evaluate(f.animTime);
				_spriteBatch.Draw(fileAnim._texture, new Vector2(0, 0), frame, Color.White);
				_spriteBatch.End();
			}

			foreach (Tile tile in level.tiles) {
				Matrix n2w = Matrix.CreateTranslation((float)tile.pos.X, tile.pos.Y, 0f);
				_spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, RasterizerState.CullNone, null, n2w * proj);
				_spriteBatch.Draw(tileTex, new Vector2(0, 0), Color.White);
				_spriteBatch.End();
			}

			foreach (OneWayTile tile in level.oneWayTiles) {
				Matrix n2w = Matrix.CreateTranslation((float)tile.pos.X, tile.pos.Y, 0f);
				_spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, RasterizerState.CullNone, null, n2w * proj);
				_spriteBatch.Draw(tileTex, new Vector2(0, 0), Color.White);
				_spriteBatch.End();
			}

			foreach (JumpSwitch tile in level.jumpSwitches) {
				Matrix n2w = Matrix.CreateTranslation((float)tile.pos.X, tile.pos.Y, 0f);
				_spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, RasterizerState.CullNone, null, n2w * proj);
				if (tile._isSolid)
					_spriteBatch.Draw(tile._color == JumpSwitch.Color.Yellow ? jumpswitchYellowSolid : _jumpSwitchGreenSolid, new Vector2(0, 0), Color.White);
				else
					_spriteBatch.Draw(tile._color == JumpSwitch.Color.Yellow ? jumpswitchYellowPassable : _jumpSwitchGreenPassable, new Vector2(0, 0), Color.White);

				_spriteBatch.End();
			}

			foreach (TimeSwitch tile in level.timeSwitches) {
				Matrix n2w = Matrix.CreateTranslation((float)tile.pos.X, tile.pos.Y, 0f);
				_spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, RasterizerState.CullNone, null, n2w * proj);
				if (tile._color == TimeSwitch.Color.Blue) {
					Rectangle frame = timeswitchBlueAnim.Evaluate(tile.animEvalTime, false);
					_spriteBatch.Draw(timeswitchBlueAnim._texture, new Vector2(0, 0), frame, tile.tint);
				}
				else {
					Rectangle frame = timeswitchRedAnim.Evaluate(tile.animEvalTime, false);
					_spriteBatch.Draw(timeswitchRedAnim._texture, new Vector2(0, 0), frame, tile.tint);
				}
				_spriteBatch.End();
			}

			foreach (Letter f in level.letters) {
				SpriteEffects se = new SpriteEffects();
				float scale = Math.Clamp(1f - f.timeSpentCollected, 0f, 1f);
				Matrix n2w = Matrix.CreateTranslation(-16f, -16f, 0f) * Matrix.CreateScale(scale, scale, 1f) * Matrix.CreateTranslation(16f, 16f, 0f) * Matrix.CreateTranslation((float)f.pos.X, f.pos.Y + MathF.Sin((float)gameTime.TotalGameTime.TotalSeconds * 1.5f) * 4f, 0f);
				_spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, RasterizerState.CullNone, null, n2w * proj);

				_spriteBatch.Draw(letterTex, new Vector2(0, 0), Color.White);
				_spriteBatch.End();
			}

			foreach (Ghosty g in level.ghosties) {
				Matrix n2w = Matrix.Identity;
				if (g.isLookingRight) {
					n2w = Matrix.CreateTranslation(-32f, 0f, 0f) * Matrix.CreateScale(-1f, 1f, 1f);
				}
				n2w = n2w * Matrix.CreateTranslation(g.pos.X, g.pos.Y, 0f);
				_spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, RasterizerState.CullNone, null, n2w * proj);
				_spriteBatch.Draw(ghostyTex, new Vector2(0, 0), Color.White);
				_spriteBatch.End();
			}

			foreach (IceSpike g in level.iceSpikes) {

				Matrix n2w = Matrix.CreateTranslation(g.pos.X, g.pos.Y, 0f);
				_spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, RasterizerState.CullNone, null, n2w * proj);
				_spriteBatch.Draw(iceSpikeTex, new Vector2(0, 0), Color.White);
				_spriteBatch.End();
			}

			foreach (Walker f in level.walkers) {
				Matrix n2w = Matrix.CreateTranslation((float)f.pos.X, f.pos.Y, 0f);
				_spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, RasterizerState.CullNone, null, n2w * proj);
				Rectangle frame = walkerAnim.Evaluate(f.animEvalTime);
				_spriteBatch.Draw(walkerAnim._texture, new Vector2(0, 0), frame, Color.White);
				_spriteBatch.End();
			}


			if (level.letterBox != null) {
				{
					Matrix n2w = Matrix.CreateTranslation(level.letterBox.pos.X, level.letterBox.pos.Y, 0f);
					_spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, RasterizerState.CullNone, null, n2w * proj);
					_spriteBatch.Draw(letterBoxTex, new Vector2(0, 0), Color.White);
					_spriteBatch.End();
				}

				bool areAllLeteterCollected = true;
				foreach (Letter l in level.letters) {
					areAllLeteterCollected &= l.isCollected;
				}

				if (2f * totalSeconds % 3 > 1f) {
					Matrix n2w = Matrix.CreateTranslation(level.letterBox.pos.X + 24, level.letterBox.pos.Y - 24, 0f);
					_spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, RasterizerState.CullNone, null, n2w * proj);
					_spriteBatch.Draw(areAllLeteterCollected ? letterBoxPromptReadyTex : letterBoxPromptTex, new Vector2(0, 0), Color.White);
					_spriteBatch.End();
				}
			}

			// Draw the snowman
			{

				Matrix n2w = Matrix.Identity;
				if (level.snowman._isFacingRight == false && !level.snowman.isDead && !level.isComplete) {
					n2w = Matrix.CreateTranslation(-32f, 0f, 0f) * Matrix.CreateScale(-1f, 1f, 1f);
				}
				n2w = n2w * Matrix.CreateTranslation(level.snowman.pos.X, level.snowman.pos.Y, 0f);

				if (level.isComplete) {
					_spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, RasterizerState.CullNone, null, n2w * proj);
					_spriteBatch.Draw(snowmandWinTex, new Vector2(0, 0), Color.White);
					_spriteBatch.End();
				}
				else if (level.snowman.isDead) {
					_spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, RasterizerState.CullNone, null, n2w * proj);
					_spriteBatch.Draw(snowmanDeadTex, new Vector2(0, 0), Color.White);
					_spriteBatch.End();
				}
				else if (level.snowman.isCrouched) {
					_spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, RasterizerState.CullNone, null, n2w * proj);
					_spriteBatch.Draw(snowmandCrouchTex, new Vector2(0, 0), Color.White);
					_spriteBatch.End();
				}
				else {
					Rectangle subImage = snowmanWalkAnim.Evaluate(level.snowman.walkAnimEvalTime);

					_spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, RasterizerState.CullNone, null, n2w * proj);
					_spriteBatch.Draw(snowmanWalkAnim._texture, new Vector2(0, 0), subImage, Color.White);
					_spriteBatch.End();
				}
			}

			foreach (var f in level.fireProjectiles) {
				Matrix n2w = Matrix.CreateTranslation(-8f, -8f, 0f) * Matrix.CreateRotationZ(5f * f.age) * Matrix.CreateTranslation(8f, 8f, 0f) * Matrix.CreateTranslation((float)f.pos.X, f.pos.Y, 0f);
				_spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, RasterizerState.CullNone, null, n2w * proj);
				_spriteBatch.Draw(_fireProjectileTex, new Vector2(0, 0), Color.White);
				_spriteBatch.End();
			}



			// Snow
			{
				float parallaxShiftYWs = 0f;

				int k = (int)(level.camera.viewRectWs.X / snow.simWidth) - 1;
				int numRepeatsNeededToCovertTheScreen = (int)(level.camera.viewRectWs.Width / snow.simWidth) + 3;

				for (int t = 0; t < numRepeatsNeededToCovertTheScreen; ++t) {
					Matrix n2w = Matrix.CreateTranslation((float)(t + k) * snow.simWidth, level.minYPointWs - snow.simHeight * 0.5f, 0f);

					_spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, RasterizerState.CullNone, null, n2w * proj);
					foreach (Snow.Flake f in snow.flakes) {

						SpriteEffects se = new SpriteEffects();
						_spriteBatch.Draw(snowflake0, f.pos, null, Color.White, 0f, new Vector2(), new Vector2(0.75f, 0.75f), se, 0f);
					}
					_spriteBatch.End();
				}
			}

			// Collected Mail
			{
				for (int t = 0; t < level.letters.Count; ++t) {

					Color color = level.letters[t].isCollected ? Color.White : new Color(0.5f, 0.5f, 0.5f, 1f);

					Matrix n2w = Matrix.CreateTranslation(level.camera.viewRectWs.X + 8f + t * 18f, level.camera.viewRectWs.Y, 0f);
					_spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, RasterizerState.CullNone, null, n2w * proj);
					_spriteBatch.Draw(letterTex, new Vector2(0, 0), color);
					_spriteBatch.End();
				}

			}

			if (level.timeSpentPlaying < 0.5f) {
				_spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, RasterizerState.CullNone, null);
				_spriteBatch.Draw(whiteTex1, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), new Color(0f, 0f, 0f, 1f - MathF.Pow(level.timeSpentPlaying / 0.5f, 2f)));
				_spriteBatch.End();
			}

			if (level.snowman.isDead) {
				if (level.snowman.timeSpentDead > 0.3f) {
					_spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, RasterizerState.CullNone, null);
					_spriteBatch.Draw(whiteTex1, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), new Color(0f, 0f, 0f, (level.snowman.timeSpentDead - 0.3f) / 0.7f));
					_spriteBatch.End();
				}
			}

			if (level.isComplete) {
				if (level.timeSpentComplete > 0.3f) {
					_spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, RasterizerState.CullNone, null);
					_spriteBatch.Draw(whiteTex1, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), new Color(0f, 0f, 0f, (level.timeSpentComplete - 0.3f) / 2f));
					_spriteBatch.End();
				}
			}
		}
	}
}
