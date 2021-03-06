﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using mizjam1.Actors;
using mizjam1.ContentLoaders;
using mizjam1.Helpers;
using mizjam1.Sound;
using mizjam1.UI;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace mizjam1.Scenes
{
    internal class MenuScene : Scene
    {
        internal int Size = 32;
        internal List<Actor> Actors;
        internal Player Player;
        internal float SoundTimer = 0;
        internal float SoundTime = 0.5f;
        internal bool CanPlay = true;
        internal Color BgColor = new Color(14 / 255f, 7 / 255f, 27 / 255f);
        internal Button Mute;
        internal List<Point> Stars;
        public override void Initialize(GameWindow window, GraphicsDevice graphicsDevice, ContentManager content, Main main)
        {
            base.Initialize(window, graphicsDevice, content, main);
            SoundPlayer.Instance.SetTheme();
            Player = new Player(new Vector2(50, 100))
            {
                BorderCollide = false,
                CanTeleport = false,
                Scale = 3,
                MaxSpeed = 100,
            };
            Mute = new Button(window, "sound on", Size * 12f, Size, Toggle, Player, false);
            Actors = new List<Actor>
            {
                new Button(window, "star pig", Size * 1, Size * 2f, PlayPig, Player, false) { PlaySound = false },
                new Button(window, "easy", Size * 4, Size, Game.NewEasyGame, Player),
                new Button(window, "normal", Size * 6.5f, Size, Game.NewNormalGame, Player),
                new Button(window, "hard", Size * 9, Size, Game.NewHardGame, Player),
                Mute,                
                new Button(window, "full screen", Size * 13.5f, Size, Game.ToggleFullScreen, Player, false),
                new Button(window, "exit", Size * 16, Size, Game.Exit, Player, false),
                Player,
            };

            Stars = new List<Point>();
            for (int i = 0; i < Window.ClientBounds.Width / 10; i++)
            {
                int x = (int)(Window.ClientBounds.Width * RandomHelper.NextFloat());
                int y = (int)(Window.ClientBounds.Height * RandomHelper.NextFloat());
                Stars.Add(new Point(x, y));
            }
            Window.ClientSizeChanged += Window_ClientSizeChanged;
        }

        private void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            Stars.Clear();
            for (int i = 0; i < Window.ClientBounds.Width / 10; i++)
            {
                int x = (int)(Window.ClientBounds.Width * RandomHelper.NextFloat());
                int y = (int)(Window.ClientBounds.Height * RandomHelper.NextFloat());
                Stars.Add(new Point(x, y));
            }
        }
        internal void Toggle()
        {
            SoundPlayer.Instance.Toggle();
            if (SoundPlayer.Instance.IsMuted)
            {
                Mute.Text = "sound off";
            } else
            {
                Mute.Text = "sound on";
            }
        }


        internal void PlayPig()
        {
            if (CanPlay)
            {
                SoundPlayer.Instance.Play("PIG");
                CanPlay = false;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(BgColor);

            SpriteBatch.Begin(samplerState: SamplerState.PointClamp, sortMode: SpriteSortMode.FrontToBack);

            foreach (var p in Stars)
            {
                SpriteBatch.DrawPoint(p.X, p.Y, Color.White, 2);
            }

            Actors.ForEach(b => b.Draw(SpriteBatch));

            SpriteBatch.End();
        }


        public override void Update(GameTime gameTime)
        {
            if (!CanPlay)
            {
                SoundTimer += gameTime.GetElapsedSeconds();
                if (SoundTimer > SoundTime)
                {
                    CanPlay = true;
                    SoundTimer = 0;
                }
            }
            Actors.ForEach(b => b.Update(gameTime));
            for (int i = 0; i < Stars.Count; i++)
            {
                Point p = Stars[i];
                int x = (int)(p.X + Math.Max(1, i / (Window.ClientBounds.Width / 50f)));
                x %= Window.ClientBounds.Width;
                Stars[i] = new Point(x, p.Y);
            }
        }
    }
}
