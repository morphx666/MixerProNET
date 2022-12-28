using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NMixerProNET;
using System.Threading;

namespace MuteSessionByName {
    public partial class FormMain : Form {
        private CMixerPro mxp;
        private CCoreAudio ca;

        public FormMain() {
            InitializeComponent();

            if(CCoreAudio.RequiresCoreAudio) {
                mxp = new CMixerPro();
                ca = new CCoreAudio(mxp);

                this.FormClosing += (object sender, FormClosingEventArgs e) => {
                    ca.Dispose();
                    mxp.Dispose();
                };

                //Thread tmp = new Thread(GetAvailableSessions);
                //tmp.Start();

                GetAvailableSessions();
            } else {
                TextBoxSessionName.Enabled = false;
                ButtonToggleMute.Enabled = false;
                MessageBox.Show("Sessions are only supported under Windows Vista, 7 and 8", "Not Supported");
            }
        }

        private void ButtonToggleMute_Click(object sender, EventArgs e) {
            String sessionName = TextBoxSessionName.Text.ToLower();

            if(ToggleSessionMute(sessionName)) {
                GetAvailableSessions();
            } else { 
                MessageBox.Show("Session '" + sessionName + "' not found!", "Invalid Session Name");
            }
        }

        private void ButtonRefresh_Click(object sender, EventArgs e) {
            GetAvailableSessions();
        }

        private bool ToggleSessionMute(string sessionName) {
            bool isValid = false;

            foreach(CCoreAudio.CMixer mixer in ca.Mixers) {
                if(mixer.Selected && mixer.DataFlow == CoreAudio.EDataFlow.eRender) {
                    foreach(CCoreAudio.CSession session in mixer.Sessions) {
                        if(session.Name.ToLower() == sessionName) {
                            session.ControlMute.Mute = !session.ControlMute.Mute;
                            isValid = true;
                        }
                    }
                }
            }

            return isValid;
        }

        private void GetAvailableSessions() {
            ListBoxSessions.Items.Clear();

            foreach(CCoreAudio.CMixer mixer in ca.Mixers) {
                if(mixer.Selected && mixer.DataFlow == CoreAudio.EDataFlow.eRender) {
                    foreach(CCoreAudio.CSession session in mixer.Sessions) {
                        ListBoxSessions.Items.Add(string.Format("{0} {1}", (session.ControlMute.Mute?"x":" "), session.Name));
                    }
                }
            }
        }

    }
}
