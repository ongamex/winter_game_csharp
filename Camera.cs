using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace Game1
{
	public class Camera
	{
		public Vector2 pos = new Vector2(0, 0);
		public float viewHeigthWs = 240f; // The camera image height in world space
		public float camScaleX = 1f;
		public float camScaleY = 1f;

		public Rectf viewRectWs = new Rectf();

		public Matrix GetProjectionMatrix(int iviewportWidth, int iviewportHeight) {
			float viewportWidth = (float)iviewportWidth;
			float viewportHeight = (float)iviewportHeight;
			Vector2 toTopLeftCorner = 0.5f * new Vector2(viewportWidth, viewportHeight);
			float viewportWByH = viewportWidth / viewportHeight;

			float viewWidthWs = viewHeigthWs * viewportWByH;

			camScaleX = viewportWidth / viewWidthWs;
			camScaleY = viewportHeight / viewHeigthWs;

			Vector2 topLeftCornerWs = pos - 0.5f * new Vector2(viewWidthWs, viewHeigthWs);

			viewRectWs.X = topLeftCornerWs.X;
			viewRectWs.Y = topLeftCornerWs.Y;
			viewRectWs.Width = viewWidthWs;
			viewRectWs.Height = viewHeigthWs;


			Matrix proj = Matrix.CreateTranslation(-pos.X, -pos.Y, 0f) * Matrix.CreateScale(camScaleX, camScaleY, 1f) * Matrix.CreateTranslation(toTopLeftCorner.X, toTopLeftCorner.Y, 0f);

			return proj;
		}
	}
}
