/*
  Rogue Legacy Enhanced

  This project is based on modified disassembly of Rogue Legacy's engine, with permission to do so by its creators.
  Therefore, former creators copyright notice applies to original disassembly. 

  Disassembled source Copyright(C) 2011-2015, Cellar Door Games Inc.
  Rogue Legacy(TM) is a trademark or registered trademark of Cellar Door Games Inc. All Rights Reserved.
*/

using System;
using DS2DEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tweener;
using Tweener.Ease;

namespace RogueCastle
{
    public class DeathDefiedScreen : Screen
    {
        private Vector2 m_cameraPos;
        private PlayerObj m_player;
        private string m_songName;
        private SpriteObj m_spotlight;
        private float m_storedMusicVolume;
        private SpriteObj m_title;
        private SpriteObj m_titlePlate;
        public float BackBufferOpacity { get; set; }

        public override void LoadContent()
        {
            new Vector2(2f, 2f);
            new Color(255, 254, 128);
            m_spotlight = new SpriteObj("GameOverSpotlight_Sprite");
            m_spotlight.Rotation = 90f;
            m_spotlight.ForceDraw = true;
            m_spotlight.Position = new Vector2(660f, 40 + m_spotlight.Height);
            m_titlePlate = new SpriteObj("SkillUnlockTitlePlate_Sprite");
            m_titlePlate.Position = new Vector2(660f, 160f);
            m_titlePlate.ForceDraw = true;
            m_title = new SpriteObj("DeathDefyText_Sprite");
            m_title.Position = m_titlePlate.Position;
            m_title.Y -= 40f;
            m_title.ForceDraw = true;
            base.LoadContent();
        }

        public override void OnEnter()
        {
            m_cameraPos = new Vector2(Camera.X, Camera.Y);
            if (m_player == null)
            {
                m_player = (ScreenManager as RCScreenManager).Player;
            }
            m_titlePlate.Scale = Vector2.Zero;
            m_title.Scale = Vector2.Zero;
            m_title.Opacity = 1f;
            m_titlePlate.Opacity = 1f;
            m_storedMusicVolume = SoundManager.GlobalMusicVolume;
            m_songName = SoundManager.GetCurrentMusicName();
            Tween.To(typeof (SoundManager), 1f, Tween.EaseNone, "GlobalMusicVolume",
                (m_storedMusicVolume*0.1f).ToString());
            SoundManager.PlaySound("Player_Death_FadeToBlack");
            m_player.Visible = true;
            m_player.Opacity = 1f;
            m_spotlight.Opacity = 0f;
            Tween.To(this, 0.5f, Linear.EaseNone, "BackBufferOpacity", "1");
            Tween.To(m_spotlight, 0.1f, Linear.EaseNone, "delay", "1", "Opacity", "1");
            Tween.AddEndHandlerToLastTween(typeof (SoundManager), "PlaySound", "Player_Death_Spotlight");
            Tween.To(Camera, 1f, Quad.EaseInOut, "X", m_player.AbsX.ToString(), "Y",
                (m_player.Bounds.Bottom - 10).ToString(), "Zoom", "1");
            Tween.RunFunction(2f, this, "PlayerLevelUpAnimation");
            base.OnEnter();
        }

        public void PlayerLevelUpAnimation()
        {
            m_player.ChangeSprite("PlayerLevelUp_Character");
            m_player.PlayAnimation(false);
            Tween.To(m_titlePlate, 0.5f, Back.EaseOut, "ScaleX", "1", "ScaleY", "1");
            Tween.To(m_title, 0.5f, Back.EaseOut, "delay", "0.1", "ScaleX", "0.8", "ScaleY", "0.8");
            Tween.RunFunction(0.1f, typeof (SoundManager), "PlaySound", "GetItemStinger3");
            Tween.RunFunction(2f, this, "ExitTransition");
        }

        public void ExitTransition()
        {
            Tween.To(typeof (SoundManager), 1f, Tween.EaseNone, "GlobalMusicVolume", m_storedMusicVolume.ToString());
            Tween.To(Camera, 1f, Quad.EaseInOut, "X", m_cameraPos.X.ToString(), "Y", m_cameraPos.Y.ToString());
            Tween.To(m_spotlight, 0.5f, Tween.EaseNone, "Opacity", "0");
            Tween.To(m_titlePlate, 0.5f, Tween.EaseNone, "Opacity", "0");
            Tween.To(m_title, 0.5f, Tween.EaseNone, "Opacity", "0");
            Tween.To(this, 0.2f, Tween.EaseNone, "delay", "1", "BackBufferOpacity", "0");
            Tween.AddEndHandlerToLastTween(ScreenManager, "HideCurrentScreen");
        }

        public override void Draw(GameTime gameTime)
        {
            Camera.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null,
                Camera.GetTransformation());
            Camera.Draw(Game.GenericTexture,
                new Rectangle((int) Camera.TopLeftCorner.X - 10, (int) Camera.TopLeftCorner.Y - 10, 1340, 740),
                Color.Black*BackBufferOpacity);
            m_player.Draw(Camera);
            Camera.End();
            Camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null, null);
            m_spotlight.Draw(Camera);
            m_titlePlate.Draw(Camera);
            m_title.Draw(Camera);
            Camera.End();
            base.Draw(gameTime);
        }

        public override void Dispose()
        {
            if (!IsDisposed)
            {
                Console.WriteLine("Disposing Death Defied Screen");
                m_player = null;
                m_spotlight.Dispose();
                m_spotlight = null;
                m_title.Dispose();
                m_title = null;
                m_titlePlate.Dispose();
                m_titlePlate = null;
                base.Dispose();
            }
        }
    }
}