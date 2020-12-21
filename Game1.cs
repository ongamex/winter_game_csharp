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
	public class AsepriteAnimDescJson
	{
		public class Rect
		{
			public int x, y, w, h;
		}

		public class Frame
		{
			public Rect frame;
			public int duration; // in miliseconds
		}

		public class Meta
		{
			public string image;
		}

		public List<Frame> frames;
		public Meta meta;
	}

	public class SpriteAnimation
	{
		public class Frame
		{
			public Rectangle subImage = new Rectangle();
			public float startTime = 0f;
			public float endTime = 0f;
			public float duration = 0f; // The duration of this frame in seconds.
		}

		public static SpriteAnimation Load(string jsonFile, ContentManager content) {
			string animJsonDesc = File.ReadAllText(jsonFile);
			AsepriteAnimDescJson animDesc = JsonConvert.DeserializeObject<AsepriteAnimDescJson>(animJsonDesc);

			SpriteAnimation anim = new SpriteAnimation();

			anim._texture = content.Load<Texture2D>(Path.GetFileNameWithoutExtension(animDesc.meta.image));

			float frameStartTime = 0f;
			foreach (AsepriteAnimDescJson.Frame descFrame in animDesc.frames) {
				Frame frame = new Frame();
				frame.startTime = frameStartTime;
				frame.duration = (float)(descFrame.duration) / 1000f;
				frame.endTime = frame.startTime + frame.duration;
				frame.subImage.X = descFrame.frame.x;
				frame.subImage.Y = descFrame.frame.y;
				frame.subImage.Width = descFrame.frame.w;
				frame.subImage.Height = descFrame.frame.h;
				anim._frames.Add(frame);

				frameStartTime += frame.duration;
			}

			anim._totalAnimationTime = frameStartTime;

			return anim;
		}

		public Rectangle Evaluate(float evalTime, bool isLooping = true) {
			if (_totalAnimationTime == 0f || _frames.Count == 0) {
				return new Rectangle();
			}

			evalTime = Math.Max(0f, evalTime);
			if (isLooping) {
				int numLengthsToRemove = (int)(evalTime / _totalAnimationTime);
				evalTime -= (float)(numLengthsToRemove) * _totalAnimationTime;
				evalTime = Math.Max(0f, evalTime);
			} else {
				evalTime = Math.Clamp(evalTime, 0f, _totalAnimationTime);
			}

			foreach(Frame frame in _frames) {
				if(evalTime >= frame.startTime && evalTime <= frame.endTime) {
					return frame.subImage;
				}
			}

			return new Rectangle();
		}

		public Texture2D _texture;
		List<Frame> _frames = new List<Frame>();
		float _totalAnimationTime = 0;
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


			if (Keyboard.GetState().IsKeyDown(Keys.Left))
				snowmanVel.X = lerpCLamp(snowmanVel.X, -50, 100 * dt);

			if (Keyboard.GetState().IsKeyDown(Keys.Right))
				snowmanVel.X = lerpCLamp(snowmanVel.X, 80f, 100 * dt);

			if (Keyboard.GetState().IsKeyDown(Keys.Space) && !oldks.IsKeyDown(Keys.Space)) {
				snowmanVel.Y = -80;
			}

			snowmanPos += snowmanVel * dt;

			snowmanVel.Y += 100f * dt;

			snowmanVel.X -= snowmanVel.X * 0.005f;

			if (snowmanPos.Y > 28f) {
				snowmanPos.Y = 28f;
				snowmanVel.Y = 0f;
			}

			// TODO: Add your update logic here
			oldks = Keyboard.GetState();
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime) {
			GraphicsDevice.Clear(Color.CornflowerBlue);
			Matrix proj = _camera.GetProjectionMatrix(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
			walkAnimEvalTime += (float)(gameTime.ElapsedGameTime.TotalSeconds);
			// Draw the snowman
			{
				Rectangle subImage = _snowmanAnimWalk.Evaluate(walkAnimEvalTime);

				Matrix n2w = Matrix.CreateTranslation(snowmanPos.X, snowmanPos.Y, 0f);
				_spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, n2w * proj);
				_spriteBatch.Draw(_snowmanAnimWalk._texture, new Vector2(0, 0), subImage, Color.White);
				_spriteBatch.End();
			}

			// Draw the bauchour
			for (int t = 0; t < 10; ++t) {
				Matrix n2w = Matrix.CreateTranslation((float)t * 32f, 60f, 0f);
				_spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, RasterizerState.CullNone, null, n2w * proj);
				_spriteBatch.Draw(_tileTexture, new Vector2(0, 0), Color.White);
				_spriteBatch.End();
			}

			base.Draw(gameTime);
		}
	}
}
