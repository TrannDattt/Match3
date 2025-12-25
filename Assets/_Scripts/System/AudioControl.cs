using System.Collections.Generic;
using Match3.Utils;
using UnityEngine;

namespace Match3.GameSystem
{
    public class AudioControl : Singleton<AudioControl>
    {
        public enum EAudioType
        {
            BGM,
            SFX
        }

        // public enum ESfxType
        // {
        //     ButtonClick,
        //     SwapBlock,
        //     MatchBlock,
        // }

        [Header("BGM")]
        [SerializeField] private AudioSource _bgmAudio;
        [SerializeField] private AudioClip _bgmClip;

        [Header("SFX")]
        [SerializeField] private AudioSource _sfxAudio;

        // private Dictionary<ESfxType, AudioClip> _sfxMap = new();

        public void PlayBgm()
        {
            if (!_bgmAudio || !_bgmClip) return;

            _bgmAudio.clip = _bgmClip;
            _bgmAudio.Play();
        }

        // public void PlaySFX(ESfxType type)
        // {
        //     if (!_sfxMap.ContainsKey(type) || !_sfxMap.TryGetValue(type, out var clip)) return;

        //     PlaySfx(clip);
        // }

        public void PlaySfx(AudioClip clip)
        {
            if (!clip) return;

            _sfxAudio.PlayOneShot(clip);
        }

        public void SetVolume(EAudioType audioType, float value)
        {
            var audio = audioType switch
            {
                EAudioType.BGM => _bgmAudio,
                EAudioType.SFX => _sfxAudio,
                _ => null
            };
            if (!audio) return;

            audio.volume = Mathf.Clamp01(value);
        }

        public bool GetVolumeStatus(EAudioType audioType)
        {
            var audio = audioType switch
            {
                EAudioType.BGM => _bgmAudio,
                EAudioType.SFX => _sfxAudio,
                _ => null
            };
            if (!audio) return false;

            return audio.volume > 0;
        }

        // Used to copy the setting from Audio Source to audio component in other objects
        public void SyncSetting(EAudioType fromType, AudioSource toSync)
        {
            var fromAudio = fromType switch
            {
                EAudioType.BGM => _bgmAudio,
                EAudioType.SFX => _sfxAudio,
                _ => null
            };
            if (!fromAudio) return;

            toSync.volume = fromAudio.volume;
            toSync.pitch = fromAudio.pitch;
            toSync.loop = fromAudio.loop;
            toSync.playOnAwake = fromAudio.playOnAwake;
        }
    }
}