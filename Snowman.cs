using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace Game1
{
	public class Snowman
	{
		public bool _isFacingRight = true;
		public float timeSpentInAir = 0f;
		public Vector2 pos = new Vector2(0f, 0f);
		public Vector2 vel = new Vector2(0f, 0f);
		public float walkAnimEvalTime = 0f;
		public bool wasGrounded = false;
		public bool isDead = false;
		public float timeSpentDead = 0f;
		private int jumpCounter = 0;

		public Rectf GetRectWs() {
			Rectf res = new Rectf();
			res.X = pos.X + 8f;
			res.Y = pos.Y + 4f;
			res.Width = 16f;
			res.Height = 28f;
			return res;
		}

		public void Update(GameUpdateSets u) {
			float jumpHeight = 36f;
			float jumpTimeApex = 0.3f;
			float minJumpHeight = 4f;
			float fallingGravityMultiplier = 1f;
			float gravity = 2f * jumpHeight / (jumpTimeApex * jumpTimeApex);
			float fallingGravity = gravity * fallingGravityMultiplier;
			float maxJumpVelocity = gravity * jumpTimeApex;
			float minJumpVelocity = System.MathF.Sqrt(2f * gravity * minJumpHeight);

			// Update
			if (isDead) {
				vel = Vector2.Zero;
				timeSpentDead += u.dt;

				if (timeSpentDead > 1f) {
					u.level.shouldRestart = true;
				}
				return;
			}

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

			bool isJumpBtnPressed = (Keyboard.GetState().IsKeyDown(Keys.Space) && !u.game.oldks.IsKeyDown(Keys.Space)) || (u.game.oldgs.IsButtonUp(Buttons.A) && GamePad.GetState(0).IsButtonDown(Buttons.A));
			bool isJumpBtnReleased = !Keyboard.GetState().IsKeyDown(Keys.Space) && u.game.oldks.IsKeyDown(Keys.Space) || (u.game.oldgs.IsButtonDown(Buttons.A) && GamePad.GetState(0).IsButtonUp(Buttons.A));

			if ((timeSpentInAir < 0.15f || jumpCounter == 1) && isJumpBtnPressed) {
				// pressed
				vel.Y = -maxJumpVelocity;
				jumpCounter++;
				foreach (JumpSwitch tile in u.level._jumpSwitch) {
					tile._isSolid = !tile._isSolid;
				}
			}

			if (isJumpBtnReleased) {
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
			Rectf snowmanRectGroundCheck = new Rectf(pos.X + 8, pos.Y + 4f, 16f, 30f);

			foreach (Tile tile in u.level._tiles) {
				Rectf snowmanRect = GetRectWs();
				Vector2 depth = snowmanRect.GetIntersectionDepth(tile.GetRectWs());
				isGrounded |= snowmanRectGroundCheck.GetIntersectionDepth(tile.GetRectWs()).Y < 0f;
				if (depth.X != 0f && MathF.Abs(depth.X) < MathF.Abs(depth.Y)) {
					pos.X += depth.X;
					//vel.X = 0;
				}
				if (depth.Y != 0f && MathF.Abs(depth.Y) < MathF.Abs(depth.X)) {
					pos.Y += depth.Y;
					if (vel.Y > 0f)
						vel.Y = 0;
				}
			}

			foreach (JumpSwitch tile in u.level._jumpSwitch) {

				if (tile._isSolid == false) continue;

				Rectf snowmanRect = GetRectWs();
				Vector2 depth = snowmanRect.GetIntersectionDepth(tile.GetRectWs());
				isGrounded |= snowmanRectGroundCheck.GetIntersectionDepth(tile.GetRectWs()).Y < 0f;
				if (depth.X != 0f && MathF.Abs(depth.X) < MathF.Abs(depth.Y)) {
					pos.X += depth.X;
					//vel.X = 0;
				}
				if (depth.Y != 0f && MathF.Abs(depth.Y) < MathF.Abs(depth.X)) {
					pos.Y += depth.Y;
					if (vel.Y > 0f)
						vel.Y = 0;
				}
			}

			foreach (TimeSwitch tile in u.level._timeSwitches) {
				if (tile._isSolid == false) continue;

				Rectf snowmanRect = GetRectWs();
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

			foreach (var f in u.level.walkAndBads) {
				Rectf snowmanRect = GetRectWs();
				Vector2 depth = snowmanRect.GetIntersectionDepth(f.GetRectWs());
				if (depth != Vector2.Zero) {
					isDead = true;
					//u.level.shouldRestart = true;
				}
			}

			foreach (var f in u.level._fires) {
				Rectf snowmanRect = GetRectWs();
				Vector2 depth = snowmanRect.GetIntersectionDepth(f.GetRectWs());
				if (depth != Vector2.Zero) {
					isDead = true;
					//u.level.shouldRestart = true;
				}
			}

			foreach (FireProjectile fp in u.level._fireProjectiles) {
				Rectf snowmanRect = GetRectWs();
				Vector2 depth = snowmanRect.GetIntersectionDepth(fp.GetRectWs());
				if (depth != Vector2.Zero) {
					isDead = true;
					//u.level.shouldRestart = true;
				}
			}

			foreach (Ghosty f in u.level.ghosties) {
				Rectf snowmanRect = GetRectWs();
				Vector2 depth = snowmanRect.GetIntersectionDepth(f.GetRectWs());
				if (depth != Vector2.Zero) {
					isDead = true;
					//u.level.shouldRestart = true;
				}
			}

			foreach (IceSpike f in u.level.iceSpikes) {
				Rectf snowmanRect = GetRectWs();
				Vector2 depth = snowmanRect.GetIntersectionDepth(f.GetRectWs());
				if (depth != Vector2.Zero) {
					isDead = true;
					//u.level.shouldRestart = true;
				}
			}

			if (pos.Y >= u.level.deathYCoord) {
				isDead = true;
				//u.level.shouldRestart = true;
			}




			wasGrounded = isGrounded;
			if (isGrounded) {
				jumpCounter = 0;
				timeSpentInAir = 0f;
			}
			else {
				timeSpentInAir += u.dt;
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
