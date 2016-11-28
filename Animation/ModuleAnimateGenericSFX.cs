﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;
using ModuleWheels;

/*
Source code copyright 2016, by Michael Billard (Angel-125)
License: GNU General Public License Version 3
License URL: http://www.gnu.org/licenses/
Wild Blue Industries is trademarked by Michael Billard and may be used for non-commercial purposes. All other rights reserved.
Note that Wild Blue Industries is a ficticious entity 
created for entertainment purposes. It is in no way meant to represent a real entity.
Any similarity to a real entity is purely coincidental.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
namespace WildBlueIndustries
{
    public class ModuleAnimateGenericSFX : ModuleAnimateGeneric
    {
        [KSPField]
        public string startSoundURL = string.Empty;

        [KSPField]
        public float startSoundPitch = 1.0f;

        [KSPField]
        public float startSoundVolume = 0.5f;

        [KSPField]
        public string loopSoundURL = string.Empty;

        [KSPField]
        public float loopSoundPitch = 1.0f;

        [KSPField]
        public float loopSoundVolume = 0.5f;

        [KSPField]
        public string stopSoundURL = string.Empty;

        [KSPField]
        public float stopSoundPitch = 1.0f;

        [KSPField]
        public float stopSoundVolume = 0.5f;
        
        protected AudioSource loopSound = null;
        protected AudioSource startSound = null;
        protected AudioSource stopSound = null;
        protected bool isMoving = false;

        public void playStart()
        {
            if (startSound != null)
                startSound.Play();

            if (loopSound != null)
                loopSound.Play();
        }

        public void playEnd()
        {
            if (stopSound != null)
                stopSound.Play();

            if (loopSound != null)
                loopSound.Stop();
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            if (!string.IsNullOrEmpty(startSoundURL))
            {
                startSound = gameObject.AddComponent<AudioSource>();
                startSound.clip = GameDatabase.Instance.GetAudioClip(startSoundURL);
                startSound.pitch = startSoundPitch;
                startSound.volume = GameSettings.SHIP_VOLUME * startSoundVolume;
            }

            if (!string.IsNullOrEmpty(loopSoundURL))
            {
                loopSound = gameObject.AddComponent<AudioSource>();
                loopSound.clip = GameDatabase.Instance.GetAudioClip(loopSoundURL);
                loopSound.loop = true;
                loopSound.pitch = loopSoundPitch;
                loopSound.volume = GameSettings.SHIP_VOLUME * loopSoundVolume;
            }

            if (!string.IsNullOrEmpty(stopSoundURL))
            {
                stopSound = gameObject.AddComponent<AudioSource>();
                stopSound.clip = GameDatabase.Instance.GetAudioClip(stopSoundURL);
                stopSound.pitch = stopSoundPitch;
                stopSound.volume = GameSettings.SHIP_VOLUME * stopSoundVolume;
            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            //Play start
            if (aniState == animationStates.MOVING && isMoving == false)
            {
                isMoving = true;
                playStart();
            }

            //Play end
            else if (aniState == animationStates.LOCKED && isMoving)
            {
                isMoving = false;
                playEnd();
            }
        }
    }
}
