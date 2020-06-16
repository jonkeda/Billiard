using IrrKlang;
using Physics.Collisions;
using System;
using System.IO;
using System.Windows;
using Utilities;

namespace Sound
{
    class SoundManager
    {
        public readonly ISoundEngine engine = new ISoundEngine();
        public int PlayBackSpeed { get; set; } = 1;

        private readonly Random random = new Random();

        public SoundManager()
        {
            LoadSounds();
        }

        private readonly string[] ball2wall = new string[]
        {
            "ball_wall_01.wav",
            "ball_wall_02.wav",
            "ball_wall_03.wav",
        };

        private readonly string[] ball2wallSoft = new string[]
        {
            "ball_wall_01_soft.wav",
            "ball_wall_02_soft.wav",
        };

        private readonly string[] ball2ballSoft = new string[]
        {
            "ball_ball_soft_01.wav",
            "ball_ball_soft_02.wav",
            "ball_ball_soft_03.wav"
        };

        private readonly string[] ball2ballHard = new string[]
        {
                "ball_ball_hard_01.wav",
                "ball_ball_hard_02.wav",
        };

        private string SelectRandomSound(string[] array)
        {
            return array[(int)Math.Round(random.NextDouble() * (array.Length - 1))];
        }

        public void BreakSound(Vector2D p, double force)
        {
            engine.Play3D("cue_ball_01.wav", (float)((p.x - 970 / 2) / (400 - Math.Max(100, force))), 0, (float)((p.x - 970 / 2) / (400 - Math.Max(100, force))));
        }

        public void CollisionSound(object _, CollisionEvent collision)
        {
            string soundName;

            if (collision.surface == 0)
            {
                soundName = collision.force < 100 ? SelectRandomSound(ball2wallSoft) : SelectRandomSound(ball2wall);
            }
            else
            {
                soundName = collision.force < 100 ? SelectRandomSound(ball2ballSoft) : SelectRandomSound(ball2ballHard);
            }

            ISound sound = engine.Play3D(soundName, (float)(collision.position.x - 970 / 2) / 400.0f, 0, (float)(collision.position.x - 970 / 2) / 100.0f, false, false, StreamMode.NoStreaming, true);

            sound.PlaybackSpeed = 1.0f / PlayBackSpeed;
        }

        public bool ToggleSound()
        {
            engine.SoundVolume = engine.SoundVolume == 1 ? 0 : 1;

            return engine.SoundVolume == 1;
        }

        private Stream GetResourceStreamFromPath(string path)
        {
            return Application.GetResourceStream(new Uri(path)).Stream;
        }

        private void LoadSounds()
        {
            engine.AddSoundSourceFromIOStream(GetResourceStreamFromPath("pack://application:,,,/Resources/Sounds/ball_ball_soft_01.wav"), "ball_ball_soft_01.wav");
            engine.AddSoundSourceFromIOStream(GetResourceStreamFromPath("pack://application:,,,/Resources/Sounds/ball_ball_soft_02.wav"), "ball_ball_soft_02.wav");
            engine.AddSoundSourceFromIOStream(GetResourceStreamFromPath("pack://application:,,,/Resources/Sounds/ball_ball_soft_03.wav"), "ball_ball_soft_03.wav");
            engine.AddSoundSourceFromIOStream(GetResourceStreamFromPath("pack://application:,,,/Resources/Sounds/ball_ball_hard_01.wav"), "ball_ball_hard_01.wav");
            engine.AddSoundSourceFromIOStream(GetResourceStreamFromPath("pack://application:,,,/Resources/Sounds/ball_ball_hard_02.wav"), "ball_ball_hard_02.wav");

            engine.AddSoundSourceFromIOStream(GetResourceStreamFromPath("pack://application:,,,/Resources/Sounds/cue_ball_01.wav"), "cue_ball_01.wav");

            engine.AddSoundSourceFromIOStream(GetResourceStreamFromPath("pack://application:,,,/Resources/Sounds/ball_wall_01.wav"), "ball_wall_01.wav");
            engine.AddSoundSourceFromIOStream(GetResourceStreamFromPath("pack://application:,,,/Resources/Sounds/ball_wall_02.wav"), "ball_wall_02.wav");
            engine.AddSoundSourceFromIOStream(GetResourceStreamFromPath("pack://application:,,,/Resources/Sounds/ball_wall_03.wav"), "ball_wall_03.wav");

            engine.AddSoundSourceFromIOStream(GetResourceStreamFromPath("pack://application:,,,/Resources/Sounds/ball_wall_01_soft.wav"), "ball_wall_01_soft.wav");
            engine.AddSoundSourceFromIOStream(GetResourceStreamFromPath("pack://application:,,,/Resources/Sounds/ball_wall_02_soft.wav"), "ball_wall_02_soft.wav");
        }
    }
}
