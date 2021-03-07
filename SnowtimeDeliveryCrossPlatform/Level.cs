using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Linq;

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
		public bool isComplete = false;
		public float timeSpentComplete = 0f;
		public float timeSpentPlaying = 0f;
		public bool shouldRestart = false;
		public string creationText;
		public float deathYCoord = 0; // if the player y coord get bigger than this the player should die.
		public float minYPointWs = 0;
		public Camera camera = new Camera();
		public Snowman snowman;
		public LetterBox letterBox;
		public List<Tile> tiles = new List<Tile>();
		public List<OneWayTile> oneWayTiles = new List<OneWayTile>();
		public List<JumpSwitch> jumpSwitches = new List<JumpSwitch>();
		public List<TimeSwitch> timeSwitches = new List<TimeSwitch>();
		public List<Fire> fires = new List<Fire>();
		public List<FireProjectile> fireProjectiles = new List<FireProjectile>();
		public List<Walker> walkers = new List<Walker>();
		public List<Ghosty> ghosties = new List<Ghosty>();
		public List<IceSpike> iceSpikes = new List<IceSpike>();
		public List<Letter> letters = new List<Letter>();

		public void Update(Game1 game, float dt) {
			GameUpdateSets u = new GameUpdateSets();
			u.dt = dt;
			u.level = this;
			u.game = game;

			timeSpentPlaying += dt;

			if(isComplete) {
				timeSpentComplete += dt;
			}

			if (snowman != null) {
				snowman.Update(u);
			}

			foreach (var a in walkers) {
				a.Update(u);
			}

			foreach (TimeSwitch tile in timeSwitches) {
				tile.Update(u);
			}

			foreach (Fire f in fires) {
				f.Update(u);
			}

			foreach (Ghosty f in ghosties) {
				f.Update(u);
			}

			foreach (IceSpike f in iceSpikes) {
				f.Update(u);
			}

			foreach (Letter f in letters) {
				f.Update(u);
			}

			for (int t = 0; t < fireProjectiles.Count; ++t) {
				if (fireProjectiles[t].Update(u)) {
					fireProjectiles.RemoveAt(t);
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

				if ((ch == 'p' || ch == 'P') && level.snowman == null) {
					Snowman snowman = new Snowman();

					snowman.pos.X = (float)xOffset * 32f;
					snowman.pos.Y = (float)yOffset * 32f;

					level.snowman = snowman;
				}
				else if (ch == 'f' || ch == 'F') {
					Fire f = new Fire();
					f.pos.X = (float)xOffset * 32f;
					f.pos.Y = (float)yOffset * 32f;

					level.fires.Add(f);
				}
				else if (ch == 'x' || ch == 'X') {
					Tile tile = new Tile();
					tile.pos.X = (float)xOffset * 32f;
					tile.pos.Y = (float)yOffset * 32f;

					level.tiles.Add(tile);
				}
				else if (ch == 'J') {
					JumpSwitch tile = new JumpSwitch(JumpSwitch.Color.Yellow);
					tile.pos.X = (float)xOffset * 32f;
					tile.pos.Y = (float)yOffset * 32f;

					level.jumpSwitches.Add(tile);
				}
				else if (ch == 'j') {
					JumpSwitch tile = new JumpSwitch(JumpSwitch.Color.Green);
					tile.pos.X = (float)xOffset * 32f;
					tile.pos.Y = (float)yOffset * 32f;

					level.jumpSwitches.Add(tile);
				}
				else if (ch == 'T') {
					TimeSwitch tile = new TimeSwitch(TimeSwitch.Color.Blue);
					tile.pos.X = (float)xOffset * 32f;
					tile.pos.Y = (float)yOffset * 32f;

					level.timeSwitches.Add(tile);
				}
				else if (ch == 't') {
					TimeSwitch tile = new TimeSwitch(TimeSwitch.Color.Red);
					tile.pos.X = (float)xOffset * 32f;
					tile.pos.Y = (float)yOffset * 32f;

					level.timeSwitches.Add(tile);
				}
				else if (ch == 'w') {
					Walker w = new Walker();
					w.pos.X = (float)xOffset * 32f;
					w.pos.Y = (float)yOffset * 32f;

					level.walkers.Add(w);
				}
				else if (ch == 'W') {
					Walker w = new Walker();
					w.pos.X = (float)xOffset * 32f;
					w.pos.Y = (float)yOffset * 32f;
					w.isWalkingRight = false;

					level.walkers.Add(w);
				}
				else if (ch == 'g' || ch == 'G') {
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
				else if (ch == 'L') {
					LetterBox w = new LetterBox();
					w.pos.X = (float)xOffset * 32f;
					w.pos.Y = (float)yOffset * 32f;

					level.letterBox = w;
				}
				else if (ch == 'l') {
					Letter w = new Letter();
					w.pos.X = (float)xOffset * 32f;
					w.pos.Y = (float)yOffset * 32f;

					level.letters.Add(w);
				}

				if (ch == '\n') {
					xOffset = 0;
					yOffset++;
				}
				else {
					xOffset++;
				}
			}

			level.letters = level.letters.OrderBy(a => a.pos.X).ThenBy(a => a.pos.Y).ToList();

			level.deathYCoord = (float)(yOffset + 3) * 32f;
			level.minYPointWs = (float)(yOffset) * 32f;

			return level;
		}
	}
}

