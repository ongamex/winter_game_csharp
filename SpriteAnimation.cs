using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace Game1
{
	public class AsepriteAnimDescJson
	{
		public class Rect
		{
			public int x { get; set; } = 0;
			public int y { get; set; } = 0;
			public int w { get; set; } = 0;
			public int h { get; set; } = 0;
		}

		public class Frame
		{
			public Rect frame { get; set; }
			public int duration { get; set; } // in miliseconds
		}

		public class Meta
		{
			public string image { get; set; }
		}

		public List<Frame> frames { get; set; }
		public Meta meta { get; set; }
	}

	public class SpriteAnimation
	{
		public class Frame
		{
			public Rectangle subImage { get; set; } = new Rectangle();
			public float startTime { get; set; } = 0f;
			public float endTime { get; set; } = 0f;
			public float duration { get; set; } = 0f; // The duration of this frame in seconds.
		}

		public static SpriteAnimation Load(string jsonFile, ContentManager content) {
			string animJsonDesc = File.ReadAllText(jsonFile);
			AsepriteAnimDescJson animDesc = System.Text.Json.JsonSerializer.Deserialize<AsepriteAnimDescJson>(animJsonDesc);

			SpriteAnimation anim = new SpriteAnimation();

			anim._texture = content.Load<Texture2D>(Path.GetFileNameWithoutExtension(animDesc.meta.image));

			float frameStartTime = 0f;
			foreach (AsepriteAnimDescJson.Frame descFrame in animDesc.frames) {
				Frame frame = new Frame();
				frame.startTime = frameStartTime;
				frame.duration = (float)(descFrame.duration) / 1000f;
				frame.endTime = frame.startTime + frame.duration;
				Rectangle f = new Rectangle();
				f.X = descFrame.frame.x;
				f.Y = descFrame.frame.y;
				f.Width = descFrame.frame.w;
				f.Height = descFrame.frame.h;
				frame.subImage = f;
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
			}
			else {
				evalTime = Math.Clamp(evalTime, 0f, _totalAnimationTime);
			}

			foreach (Frame frame in _frames) {
				if (evalTime >= frame.startTime && evalTime <= frame.endTime) {
					return frame.subImage;
				}
			}

			return new Rectangle();
		}

		public Texture2D _texture;
		List<Frame> _frames { get; set; } = new List<Frame>();
		public float _totalAnimationTime { get; set; } = 0;
	}
}
