﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Xamarin.Forms;

namespace qController
{
    public class QController
    {

        QUpdater qUpdater;
        QCue selectedCue;
        public QClient qClient;
        public QWorkSpace qWorkspace;
        public string playbackPosition;
        public Stopwatch sw;
		public QController(string address, int port)
        {
            qClient = new QClient(address, port);
            qUpdater = new QUpdater(this);
            qUpdater.Start();
            qClient.qParser.SelectedCueUpdated += this.OnSelectedCueUpdated;
            qClient.qParser.WorkspaceUpdated += this.OnWorkspaceUpdated;
            qClient.qParser.PlaybackPositionUpdated += this.OnPlaybackPositionUpdated;
            qClient.qParser.CueInfoUpdated += this.OnCueUpdateReceived;
            qClient.qParser.AudioLevelsUpdated += this.OnAudioLevelsUpdated;
            qClient.qParser.ChildrenUpdated += this.OnChildrenUpdated;
            qClient.sendArgsUDP("/updates", 1);
            qClient.sendArgsUDP("/updates", 1);
            qClient.sendAndReceiveString("/cueLists");

        }

        private void OnChildrenUpdated(object source, ChildrenEventArgs args)
        {
            Console.WriteLine("Children Updated in QController: " + args.cue_id);
            qWorkspace.UpdateChildren(args.cue_id, args.children);

            foreach (var cueList in qWorkspace.data)
            {
                foreach (var cue in cueList.cues)
                {
                    if (cue.type == "Group")
                    {
                        if (cue.cues == null)
                        {
                            Console.WriteLine("Next Group Cue Found");
                            qClient.sendStringUDP("/cue_id/" + cue.uniqueID + "/children");
                            break;
                        }
                    }
                }
            }
        }

        private void OnAudioLevelsUpdated(object source, AudioLevelArgs args)
        {
            if(qWorkspace != null)
                qWorkspace.UpdateLevels(args.cue_id, args.levels);
        }
        public void Connect()
        {
            qClient.sendStringUDP("/connect");
        }

        public void Disconnect()
        {
            qClient.sendStringUDP("/disconnect");
        }

        public void ConnectWithPass(string pass)
        {
            qClient.sendArgsUDP("/connect", pass);
        }

        public void updateCueValue(QCue cue, string property, object newValue){
            //qClient.sendArgs("/cue_id/"+cue.uniqueID+"/"+property, newValue);
        }

        public void RefreshGroupCues()
        {
            if (qWorkspace.ChildrenPopulated())
                return;
            foreach (var cueList in qWorkspace.data)
            {
                foreach (var cue in cueList.cues)
                {
                    if (cue.type == "Group")
                    {
                        Console.WriteLine("First Group Cue Found");
                        qClient.sendStringUDP("/cue_id/" + cue.uniqueID + "/children");
                        break;
                    }
                }
            }
        }

        public void OnSelectedCueUpdated(object source, CueEventArgs args)
        {
            selectedCue = args.Cue;
            Console.WriteLine("Selected cue updated: " + selectedCue.uniqueID);
        }

        public void OnWorkspaceUpdated(object source, WorkspaceEventArgs args)
        {
            qWorkspace = args.UpdatedWorkspace;
            Console.WriteLine("Workspace updated in QController: " + qWorkspace.workspace_id);
            qWorkspace.PrintStats();
            playbackPosition = null;
            qClient.UpdateSelectedCue();
        }

        public void OnPlaybackPositionUpdated(object source, PlaybackPositionArgs args)
        {
            playbackPosition = args.PlaybackPosition;
            Console.WriteLine("Playback Position updated in QController: " + playbackPosition);
        }

        public void OnCueUpdateReceived(object source, CueEventArgs args)
        {
            qWorkspace.UpdateCue(args.Cue);
            Console.WriteLine("Cue updated in QController: " + args.Cue.uniqueID );
            if (playbackPosition == null)
            {
                playbackPosition = args.Cue.uniqueID;
            }
            if(args.Cue.type == "Audio" || args.Cue.type == "Mic" || args.Cue.type == "Video")
            {
                Console.WriteLine("Cue has audio levels...sending request");
                qClient.sendStringUDP("/cue_id/" + args.Cue.uniqueID + "/levels");
            }
        }

        public void Kill(){
            qUpdater.Kill();
            qClient.Close();
        }
    }
}
