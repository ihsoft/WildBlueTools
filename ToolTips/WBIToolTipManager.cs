﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;

/*
Source code copyrighgt 2015, by Michael Billard (Angel-125)
License: GNU General Public License Version 3
License URL: http://www.gnu.org/licenses/
If you want to use this code, give me a shout on the KSP forums! :)
Wild Blue Industries is trademarked by Michael Billard and may be used for non-commercial purposes. All other rights reserved.
Note that Wild Blue Industries is a ficticious entity 
created for entertainment purposes. It is in no way meant to represent a real entity.
Any similarity to a real entity is purely coincidental.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
namespace WildBlueIndustries
{
    [KSPScenario(ScenarioCreationOptions.AddToAllGames, GameScenes.SPACECENTER, GameScenes.EDITOR, GameScenes.FLIGHT, GameScenes.TRACKSTATION)]
    public class WBIToolTipManager : ScenarioModule
    {
        public const string kToolTip = "PART_TIP";
        public const string kName = "name";
        public const string kDescription = "description";
        public const string kTitle = "title";

        public string kAskToShowTipsTitle = "Show Tool Tips?";
        public string kAskToShowTipsText = "Do you want to see tool tips in the future? These tips contain helpful information about the parts you use.";

        private Dictionary<string, ConfigNode> toolTips = new Dictionary<string, ConfigNode>();
        private bool showToolTips = true;
        private bool askToShowTips = true;
        private List<string> tipsShown = new List<string>();

        public override void OnAwake()
        {
            base.OnAwake();
            GameEvents.onEditorPartEvent.Add(onEditorPartEvent);
        }

        public void Destroy()
        {
            GameEvents.onEditorPartEvent.Remove(onEditorPartEvent);
        }

        public override void OnLoad(ConfigNode node)
        {
            Debug.Log("[WBIToolTipManager] - OnLoad called");

            //Get the list of tooltips
            ConfigNode[] toolTipsShown = GameDatabase.Instance.GetConfigNodes(kToolTip);
            string value;
            foreach (ConfigNode toolTipNode in toolTipsShown)
            {
                if (toolTipNode.HasValue(kName) == false)
                    continue;
                value = toolTipNode.GetValue(kName);
                toolTips.Add(value, toolTipNode);
            }

            //Show tooltips flag
            if (node.HasValue("showToolTips"))
                bool.TryParse(node.GetValue("showToolTips"), out showToolTips);

            //Ask to show tips
            if (node.HasValue("askToShowTips"))
                bool.TryParse(node.GetValue("askToShowTips"), out askToShowTips);

            //List of tips already shown
            ConfigNode[] viewedTips = node.GetNodes(kToolTip);
            for (int index = 0; index < viewedTips.Length; index++)
                tipsShown.Add(viewedTips[index].GetValue(kName));
        }

        public override void OnSave(ConfigNode node)
        {
            base.OnSave(node);

            //Tooltips flag
            node.AddValue("showToolTips", showToolTips);
            node.AddValue("askToShowTips", askToShowTips);

            //List of tips shown
            int tipCount = tipsShown.Count;
            ConfigNode tipNode;
            for (int index = 0; index < tipCount; index++)
            {
                tipNode = new ConfigNode(kToolTip);
                tipNode.AddValue(kName, tipsShown[index]);
                node.AddNode(tipNode);
            }
        }

        static bool eventFired;
        public void onEditorPartEvent(ConstructionEventType eventType, Part part)
        {
            if (eventType != ConstructionEventType.PartAttached)
                return;
            if (eventFired)
            {
                eventFired = false;
                return;
            }
            eventFired = true;

            if (toolTipView == null)
                toolTipView = new WBIToolTipView();
            if (toolTipView.IsVisible())
                return;

            if (toolTips.ContainsKey(part.name))
                ShowToolTip(part.name);
        }

        public bool ToolTipViewed(string toolTipName)
        {
            return tipsShown.Contains(toolTipName);
        }

        public void SetTipViewed(string toolTipName)
        {
            if (!tipsShown.Contains(toolTipName))
                tipsShown.Add(toolTipName);
        }

        public void SetTipUnviewed(string toolTipName)
        {
            if (tipsShown.Contains(toolTipName))
                tipsShown.Remove(toolTipName);
        }

        string showToolTipName = string.Empty;
        WBIToolTipView toolTipView = null;
        public void ShowToolTip(string toolTipName)
        {
            if (!showToolTips)
                return;
            if (ToolTipViewed(toolTipName))
                return;
            if (!toolTips.ContainsKey(toolTipName))
                return;
            tipsShown.Add(toolTipName);

            //First time?
            if (askToShowTips)
            {
                askToShowTips = false;
                toolTipView.title = kAskToShowTipsTitle;
                toolTipView.toolTip = kAskToShowTipsText;
                toolTipView.okButtonText = WBIToolTipView.kYesText;
                toolTipView.showCancelButton = true;
                toolTipView.cancelButtonText = WBIToolTipView.kNoText;
                toolTipView.okDelegate = OnOkPressed;
                toolTipView.cancelDelegate = OnCancelPressed;
                toolTipView.SetVisible(true);

                showToolTipName = toolTipName;
            }

            //Just show the tool tip
            else
            {
                toolTipView.LoadConfig(toolTips[toolTipName]);
                toolTipView.showCancelButton = false;
                toolTipView.okButtonText = WBIToolTipView.kGotItText;
                toolTipView.SetVisible(true);
            }
        }

        protected void OnOkPressed()
        {
            showToolTips = true;
            if (!string.IsNullOrEmpty(showToolTipName))
            {
                WBIToolTipView toolTipView = new WBIToolTipView();
                toolTipView.LoadConfig(toolTips[showToolTipName]);
                toolTipView.showCancelButton = false;
                toolTipView.okButtonText = WBIToolTipView.kGotItText;
                toolTipView.SetVisible(true);
                showToolTipName = string.Empty;
            }
        }

        protected void OnCancelPressed()
        {
            showToolTips = false;
        }
    }
}
