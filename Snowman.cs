using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace Game1
{
	public class Snowman
	{
		public bool _isFacingRight = true;
		public Vector2 pos = new Vector2(0f, 0f);
		public Vector2 vel = new Vector2(0f, 0f);
		public float walkAnimEvalTime = 0f;
		public bool wasGrounded = false;
		public bool isDead = false;
		private int jumpCounter = 0;
		public float timeSpentDead = 0f;

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
			if (Keyboard.GetState().IsKeyDown(Keys.Left) || GamePad.GetState(0).IsButtonDown(Buttons.DPadLeft)) {
				vel.X = MyMath.lerpCLamp(vel.X, -140f, 350 * u.dt);
				walkAnimEvalTime += (float)(u.dt);
				_isFacingRight = false;
			}
			if (Keyboard.GetState().IsKeyDown(Keys.Right) || GamePad.GetState(0).IsButtonDown(Buttons.DPadRight)) {
				vel.X = MyMath.lerpCLamp(vel.X, 140f, 350 * u.dt);
				walkAnimEvalTime += (float)(u.dt);
				_isFacingRight = true;
			}

			if (MathF.Abs(GamePad.GetState(0).ThumbSticks.Left.X) > 0.01f) {
				float k = GamePad.GetState(0).ThumbSticks.Left.X;
				vel.X = MyMath.lerpCLamp(vel.X, 140f * k, 350 * u.dt);
				walkAnimEvalTime += (float)(u.dt);
				_isFacingRight = k > 0f;
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

			foreach (var f in u.level._fires) {
				Rectf snowmanRect = new Rectf(pos.X + 8, pos.Y, 16f, 32f);
				Vector2 depth = snowmanRect.GetIntersectionDepth(f.GetRectWs());
				if (depth != Vector2.Zero) {
					isDead = true;
					//u.level.shouldRestart = true;
				}
			}

			foreach (FireProjectile fp in u.level._fireProjectiles) {
				Rectf snowmanRect = new Rectf(pos.X + 8, pos.Y, 16f, 32f);
				Vector2 depth = snowmanRect.GetIntersectionDepth(fp.GetRectWs());
				if (depth != Vector2.Zero) {
					isDead = true;
					//u.level.shouldRestart = true;
				}
			}

			if (pos.Y >= u.level.deathYCoord) {
				isDead = true;
				//u.level.shouldRestart = true;
			}

			if (isDead) {
				vel = Vector2.Zero;
				timeSpentDead += u.dt;
			}
			if (timeSpentDead > 1f) {
				u.level.shouldRestart = true;
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

}
