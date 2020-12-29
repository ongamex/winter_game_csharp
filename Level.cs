using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace Game1
{
	public class GameUpdateSets
	{
		public float dt = 0f;
		public Level level;
		public Game1 game;
	}

	public class Level
	{
		public float timeSpentPlaying = 0f;
		public bool shouldRestart = false;
		public string creationText;
		public float deathYCoord = 0; // if the player y coord get bigger than this the player should die.
		public float minYPointWs = 0;
		public Camera _camera = new Camera();
		public Snowman _snowman;
		public List<Tile> _tiles = new List<Tile>();
		public List<JumpSwitch> _jumpSwitch = new List<JumpSwitch>();
		public List<TimeSwitch> _timeSwitches = new List<TimeSwitch>();
		public List<Fire> _fires = new List<Fire>();
		public List<FireProjectile> _fireProjectiles = new List<FireProjectile>();
		public List<WalkAndBad> walkAndBads = new List<WalkAndBad>();
		public List<Ghosty> ghosties = new List<Ghosty>();
		public List<IceSpike> iceSpikes = new List<IceSpike>();

		public void Update(Game1 game, float dt) {
			GameUpdateSets u = new GameUpdateSets();
			u.dt = dt;
			u.level = this;
			u.game = game;

			timeSpentPlaying += dt;

			if (_snowman != null) {
				_snowman.Update(u);
			}

			foreach (var a in walkAndBads) {
				a.Update(u);
			}

			foreach (TimeSwitch tile in _timeSwitches) {
				tile.Update(u);
			}

			foreach (Fire f in _fires) {
				f.Update(u);
			}

			foreach (Ghosty f in ghosties) {
				f.Update(u);
			}

			foreach (IceSpike f in iceSpikes) {
				f.Update(u);
			}

			for (int t = 0; t < _fireProjectiles.Count; ++t) {
				if (_fireProjectiles[t].Update(u)) {
					_fireProjectiles.RemoveAt(t);
					t--;
				}
			}
		}

		public static Level FromFile(string filename) {
			string text = System.IO.File.ReadAllText(filename);
			return FromText(text);
		}

		public static Level FromText(string text) {
			Level level = new Level();

			level.creationText = text;

			int xOffset = 0;
			int yOffset = 0;
			foreach (char ch in text) {

				if (ch == 'p' && level._snowman == null) {
					Snowman snowman = new Snowman();

					snowman.pos.X = (float)xOffset * 32f;
					snowman.pos.Y = (float)yOffset * 32f;

					level._snowman = snowman;
				}
				else if (ch == 'f' || ch == 'F') {
					Fire f = new Fire();
					f.pos.X = (float)xOffset * 32f;
					f.pos.Y = (float)yOffset * 32f;

					level._fires.Add(f);
				}
				else if (ch == 'x' || ch == 'X') {
					Tile tile = new Tile();
					tile.pos.X = (float)xOffset * 32f;
					tile.pos.Y = (float)yOffset * 32f;

					level._tiles.Add(tile);
				}
				else if (ch == 'y' || ch == 'Y') {
					JumpSwitch tile = new JumpSwitch(JumpSwitch.Color.Yellow);
					tile.pos.X = (float)xOffset * 32f;
					tile.pos.Y = (float)yOffset * 32f;

					level._jumpSwitch.Add(tile);
				}
				else if (ch == 'g' || ch == 'G') {
					JumpSwitch tile = new JumpSwitch(JumpSwitch.Color.Green);
					tile.pos.X = (float)xOffset * 32f;
					tile.pos.Y = (float)yOffset * 32f;

					level._jumpSwitch.Add(tile);
				}
				else if (ch == 'b' || ch == 'B') {
					TimeSwitch tile = new TimeSwitch(TimeSwitch.Color.Blue);
					tile.pos.X = (float)xOffset * 32f;
					tile.pos.Y = (float)yOffset * 32f;

					level._timeSwitches.Add(tile);
				}
				else if (ch == 'r' || ch == 'R') {
					TimeSwitch tile = new TimeSwitch(TimeSwitch.Color.Red);
					tile.pos.X = (float)xOffset * 32f;
					tile.pos.Y = (float)yOffset * 32f;

					level._timeSwitches.Add(tile);
				}
				else if (ch == 'w' || ch == 'W') {
					WalkAndBad w = new WalkAndBad();
					w.pos.X = (float)xOffset * 32f;
					w.pos.Y = (float)yOffset * 32f;

					level.walkAndBads.Add(w);
				}
				else if (ch == 'd' || ch == 'D') {
					Vector2 p = new Vector2((float)xOffset * 32f, (float)yOffset * 32f);
					Ghosty g = new Ghosty(p);

					level.ghosties.Add(g);
				}
				else if (ch == 'i' || ch == 'I') {
					IceSpike w = new IceSpike();
					w.pos.X = (float)xOffset * 32f;
					w.pos.Y = (float)yOffset * 32f;

					level.iceSpikes.Add(w);
				}

				if (ch == '\n') {
					xOffset = 0;
					yOffset++;
				}
				else {
					xOffset++;
				}
			}

			level.deathYCoord = (float)(yOffset + 3) * 32f;
			level.minYPointWs = (float)(yOffset) * 32f;

			return level;
		}
	}
}

