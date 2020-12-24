using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace Game1
{
	public class Camera
	{
		public Vector2 pos = new Vector2(0, 0);
		public float viewHeigthWs = 300f; // The camera image height in world space

		public Matrix GetProjectionMatrix(int iviewportWidth, int iviewportHeight) {
			float viewportWidth = (float)iviewportWidth;
			float viewportHeight = (float)iviewportHeight;
			Vector2 topLeftCornerWs = 0.5f * new Vector2(viewportWidth, viewportHeight);
			float viewportWByH = viewportWidth / viewportHeight;

			float viewWidthWs = viewHeigthWs * viewportWByH;

			float camScaleX = viewportWidth / viewWidthWs;
			float camScaleY = viewportHeight / viewHeigthWs;

			Matrix proj = Matrix.CreateTranslation(-pos.X, -pos.Y, 0f) * Matrix.CreateScale(camScaleX, camScaleY, 1f) * Matrix.CreateTranslation(topLeftCornerWs.X, topLeftCornerWs.Y, 0f);

			return proj;
		}
	}
}
