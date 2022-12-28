using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NMixerProNET;

namespace MixerTester {
    public partial class MainForm : Form {
        private CCoreAudio coreAudio;
        private CMixerPro mxp;

        private delegate void SafeSessionsUpdateDelegate(CCoreAudio.CMixer mixer, TreeNode node);

        private List<CheckBox> chkOp = new List<CheckBox>();
        private TrackBar ucsCtrl;
        private VolumeFader caFader;

        public MainForm() {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e) {
            TreeNode n1;
            TreeNode n2;

            mxp = new CMixerPro();

            if(CCoreAudio.RequiresCoreAudio) {
                coreAudio = new CCoreAudio(mxp);

                foreach(CCoreAudio.CMixer m in coreAudio.Mixers) {
                    n1 = tvMix.Nodes.Add(m.DeviceName + " (" + m.Name + ")");

                    if(!m.Enabled) n1.ForeColor = Color.FromKnownColor(KnownColor.ControlDark);
                    n1.Tag = m;

                    if(m.Line != null) AddControls(m.Line, n1);

                    n2 = n1.Nodes.Add("Lines");
                    foreach(CCoreAudio.CLine l1 in m.Lines) AddControls(l1, n2);

                    n1 = n1.Nodes.Add(m.DeviceName, "Sessions");
                    n1.Tag = m;
                    DisplaySessions(m, n1);

                    m.SessionChanged += (CCoreAudio.CMixer mixer, CCoreAudio.CSession session) => {
                        DisplaySessions(mixer, FindSessionsNode(mixer, null));
                    };
                }
            } else {
                foreach(CMixer m in mxp.Mixers) {
                    n1 = tvMix.Nodes.Add(m.DeviceName);
                    n1 = n1.Nodes.Add("Lines");
                    foreach(CLine l1 in m.LinesByLineType(CLine.LineTypeConstants.ltcDestination)) {
                        n2 = AddControls(l1, n1, m.Index);
                        n2 = n2.Nodes.Add("Source Lines");
                        foreach(CLine l2 in m.LinesByConnection(l1.ID)) {
                            AddControls(l2, n2, m.Index);
                        }
                    }
                }
            }
        }

        private TreeNode FindSessionsNode(CCoreAudio.CMixer mixer, TreeNode parentNode) {
            TreeNodeCollection nodes;
            if(parentNode == null)
                nodes = tvMix.Nodes;
            else
                nodes = parentNode.Nodes;

            foreach(TreeNode n in nodes) {
                if(n.Tag is CCoreAudio.CMixer)
                    if(((CCoreAudio.CMixer)n.Tag).Equals(mixer)) return n;
                else {
                    TreeNode foundNode = FindSessionsNode(mixer, n);
                    if(foundNode != null) return foundNode;
                }
            }

            return null;
        }

        private void DisplaySessions(CCoreAudio.CMixer mixer, TreeNode parentNode) {
            if(this.InvokeRequired)
                this.Invoke(new SafeSessionsUpdateDelegate(SafeSessionsUpdate), mixer, parentNode);
            else
                SafeSessionsUpdate(mixer, parentNode);
        }

        private void SafeSessionsUpdate(CCoreAudio.CMixer mixer, TreeNode parentNode) {
            parentNode.Nodes.Clear();
            foreach(CCoreAudio.CSession s in mixer.Sessions) AddControls(s, parentNode);
        }

        private void UpdateChildColors(TreeNode parentNode) {
            foreach(TreeNode childNode in parentNode.Nodes) {
                childNode.ForeColor = parentNode.ForeColor;
                UpdateChildColors(childNode);
            }
        }

        private TreeNode AddControls(CLine l, TreeNode n, int mIdx) {
            n = n.Nodes.Add(l.LongName);
            foreach(CControl c in l.Controls) {
                TreeNode n1 = n.Nodes.Add(c.LongName);
                n1.Tag = c;
                n1.ForeColor = (c.IsValid && c.Enabled ? Color.Black : Color.DimGray);
            }

            return n;
        }

        private TreeNode AddControls(CCoreAudio.CSession s, TreeNode n) {
            n = n.Nodes.Add(s.Name);
            n.Nodes.Add("Volume").Tag = s.ControlVolume;
            n.Nodes.Add("Mute").Tag = s.ControlMute;

            return n;
        }

        private TreeNode AddControls(CCoreAudio.CLine l, TreeNode n) {
            TreeNode n1;

            n = n.Nodes.Add(l.Name);
            foreach(CCoreAudio.CControl c in l.Controls) {
                foreach(CCoreAudio.CControl.CControlObject sc in c.SubControls) {
                    if(!(sc is CCoreAudio.CControl.CControlPeakMeter)) {
                        if(n.Text == "Master Controls")
                            n1 = n.Nodes.Add(sc.Name);
                        else
                            n1 = n.Nodes.Add(c.Name);
                        n1.Tag = sc;
                    }
                }
            }

            return n;
        }

        private void tvMix_AfterSelect(object sender, TreeViewEventArgs e) {
            if(e.Node.Tag != null) {
                if(e.Node.Tag is CCoreAudio.CControl)
                    DisplayControls((CCoreAudio.CControl)e.Node.Tag);
                else if(e.Node.Tag is CControl)
                    DisplayControls((CControl)e.Node.Tag);
                else if(e.Node.Tag is CCoreAudio.CControl.CControlObject)
                    DisplayControls((CCoreAudio.CControl.CControlObject)e.Node.Tag);
                else if(e.Node.Tag is CCoreAudio.CMixer)
                    DisplayControls((CCoreAudio.CMixer)e.Node.Tag);
            }
        }

        private void DeleteUIControls() {
            if(ucsCtrl != null) Controls.Remove(ucsCtrl);
            if(caFader != null) Controls.Remove(caFader);

            for(int i = 0; i < chkOp.Count(); i++) Controls.Remove(chkOp[i]);
            chkOp.Clear();
        }

        private void DisplayControls(CCoreAudio.CControl selCtrl) {
            if(selCtrl.ControlMute != null) DisplayControls(selCtrl.ControlMute);
            if(selCtrl.ControlVolume != null) DisplayControls(selCtrl.ControlVolume);
            if(selCtrl.ControlPeakMeter != null) DisplayControls(selCtrl.ControlPeakMeter);
            if(selCtrl.ControlLoudness != null) DisplayControls(selCtrl.ControlLoudness);
        }

        private void DisplayControls(CCoreAudio.CControl.CControlObject selCtrl) {
            int topPos = tvMix.Top + tvMix.Height + 8;
            int x = tvMix.Left;
            int y = topPos;
            bool isEnabled = (tvMix.SelectedNode.ForeColor != Color.FromKnownColor(KnownColor.ControlDark));

            DeleteUIControls();

            if(selCtrl is CCoreAudio.CControl.CControlMute) {
                CheckBox newCheckBox = new CheckBox();
                newCheckBox.Text = selCtrl.Name;
                newCheckBox.Width = tvMix.Width;
                newCheckBox.Location = new Point(x, y);
                newCheckBox.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
                y += (newCheckBox.Height + 4);

                chkOp.Add(newCheckBox);
                Controls.Add(newCheckBox);
                selCtrl.Binding.Define(newCheckBox, "Checked", "CheckStateChanged");
                newCheckBox.Enabled = isEnabled;
            } else if(selCtrl is CCoreAudio.CControl.CControlVolume) {
                caFader = new VolumeFader();
                caFader.Width = tvMix.Width;
                caFader.Location = new Point(x, y);
                caFader.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
                caFader.CoreAudioControl = selCtrl.Parent;
                caFader.IntegralChanges = false;
                y += (caFader.Height + 4);

                Controls.Add(caFader);
                caFader.Enabled = isEnabled;
            } else if(selCtrl is CCoreAudio.CControl.CControlLoudness) {
                CheckBox newCheckBox = new CheckBox();
                newCheckBox.Text = selCtrl.Name;
                newCheckBox.Width = tvMix.Width;
                newCheckBox.Location = new Point(x, y);
                newCheckBox.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
                y += (newCheckBox.Height + 4);

                chkOp.Add(newCheckBox);
                Controls.Add(newCheckBox);
                selCtrl.Binding.Define(newCheckBox, "Checked", "CheckStateChanged");
                newCheckBox.Enabled = isEnabled;
            }
        }

        private void DisplayControls(CControl selCtrl) {
            int topPos = tvMix.Top + tvMix.Height + 8;
            int x = tvMix.Left;
            int y = topPos;

            DeleteUIControls();

            if(!selCtrl.IsValid || !selCtrl.Enabled) return;

            if(selCtrl.CtrlItems.Count > 0) {
                for(int i = 0; i < selCtrl.CtrlItems.Count; i++) {
                    CheckBox newCheckBox = new CheckBox();
                    newCheckBox.Text = (string)selCtrl.CtrlItems[i + 1].ItemName;
                    newCheckBox.Width = tvMix.Width / 4;
                    newCheckBox.Location = new Point(x, y);
                    newCheckBox.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
                    x += (newCheckBox.Width + 4);
                    if(x + newCheckBox.Width > Width) {
                        x = tvMix.Left;
                        y += (newCheckBox.Height + 4);
                    }

                    chkOp.Add(newCheckBox);
                    Controls.Add(newCheckBox);
                    selCtrl.CtrlItems[i + 1].Binding.Define(newCheckBox, "Checked", "CheckStateChanged");
                }
            } else {
                if(((selCtrl.ControlClass & CControl.ControlClassConstants.cccCLASS_FADER) == CControl.ControlClassConstants.cccCLASS_FADER) ||
                ((selCtrl.ControlClass & CControl.ControlClassConstants.cccCLASS_NUMBER) == CControl.ControlClassConstants.cccCLASS_NUMBER) ||
                ((selCtrl.ControlClass & CControl.ControlClassConstants.cccCLASS_METER) == CControl.ControlClassConstants.cccCLASS_METER) ||
                ((selCtrl.ControlClass & CControl.ControlClassConstants.cccCLASS_SLIDER) == CControl.ControlClassConstants.cccCLASS_SLIDER)) {
                    ucsCtrl = new TrackBar();
                    ucsCtrl.Minimum = selCtrl.Min;
                    ucsCtrl.Maximum = selCtrl.Max;
                    ucsCtrl.TickFrequency = ucsCtrl.Maximum / 10;
                    ucsCtrl.TickStyle = TickStyle.BottomRight;
                    ucsCtrl.Width = tvMix.Width;
                    ucsCtrl.Location = new Point(x, y);
                    ucsCtrl.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
                    y += (ucsCtrl.Height + 4);

                    Controls.Add(ucsCtrl);
                    selCtrl.Binding.Define(ucsCtrl, "Value", "ValueChanged");
                } else {
                    CheckBox newCheckBox = new CheckBox();
                    newCheckBox.Text = selCtrl.LongName;
                    newCheckBox.Width = tvMix.Width;
                    newCheckBox.Location = new Point(x, y);
                    newCheckBox.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
                    y += (newCheckBox.Height + 4);
                
                    chkOp.Add(newCheckBox);
                    Controls.Add(newCheckBox);
                    selCtrl.Binding.Define(newCheckBox, "Checked", "CheckStateChanged");
                }
            }
        }

        private void DisplayControls(CCoreAudio.CMixer selCtrl) {
            int topPos = tvMix.Top + tvMix.Height + 8;
            int x = tvMix.Left;
            int y = topPos;

            DeleteUIControls();

            CheckBox newCheckBox = new CheckBox();
            newCheckBox.Text = "Default Device";
            newCheckBox.Width = tvMix.Width;
            newCheckBox.Location = new Point(x, y);
            newCheckBox.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            newCheckBox.Checked = selCtrl.Selected;
            newCheckBox.Enabled = !selCtrl.Selected;
            y += (newCheckBox.Height + 4);
        
            chkOp.Add(newCheckBox);
            Controls.Add(newCheckBox);

            newCheckBox.Click += (object sender, EventArgs e) => {
                selCtrl.Selected = true;
                newCheckBox.Enabled = false;
            };
        }
    }
}
