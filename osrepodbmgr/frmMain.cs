﻿//
//  Author:
//    Natalia Portillo claunia@claunia.com
//
//  Copyright (c) 2017, © Claunia.com
//
//  All rights reserved.
//
//  Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
//
//     * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in
//       the documentation and/or other materials provided with the distribution.
//     * Neither the name of the [ORGANIZATION] nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
//
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
//  "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
//  LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
//  A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
//  CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
//  EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
//  PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
//  PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
//  LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
//  NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Gtk;
using osrepodbmgr.Core;

namespace osrepodbmgr
{
    public partial class frmMain : Window
    {
        ListStore osView;
        ListStore fileView;
        Thread thdPulseProgress;
        Thread thdPopulateOSes;
        Thread thdCompressTo;
        Thread thdSaveAs;
        Thread thdPopulateFiles;
        bool populatingFiles;
        Thread thdScanFile;
        TreeIter outIter;

        public frmMain() :
                base(WindowType.Toplevel)
        {
            Build();

            Workers.InitDB();

            CellRendererText developerCell = new CellRendererText();
            CellRendererText productCell = new CellRendererText();
            CellRendererText versionCell = new CellRendererText();
            CellRendererText languagesCell = new CellRendererText();
            CellRendererText architectureCell = new CellRendererText();
            CellRendererText machineCell = new CellRendererText();
            CellRendererText formatCell = new CellRendererText();
            CellRendererText descriptionCell = new CellRendererText();
            CellRendererToggle oemCell = new CellRendererToggle();
            CellRendererToggle upgradeCell = new CellRendererToggle();
            CellRendererToggle updateCell = new CellRendererToggle();
            CellRendererToggle sourceCell = new CellRendererToggle();
            CellRendererToggle filesCell = new CellRendererToggle();
            CellRendererToggle netinstallCell = new CellRendererToggle();
            CellRendererText pathCell = new CellRendererText();

            TreeViewColumn developerColumn = new TreeViewColumn("Developer", developerCell, "text", 0, "background", 14, "foreground", 15);
            TreeViewColumn productColumn = new TreeViewColumn("Product", productCell, "text", 1, "background", 14, "foreground", 15);
            TreeViewColumn versionColumn = new TreeViewColumn("Version", versionCell, "text", 2, "background", 14, "foreground", 15);
            TreeViewColumn languagesColumn = new TreeViewColumn("Languages", languagesCell, "text", 3, "background", 14, "foreground", 15);
            TreeViewColumn architectureColumn = new TreeViewColumn("Architecture", architectureCell, "text", 4, "background", 14, "foreground", 15);
            TreeViewColumn machineColumn = new TreeViewColumn("Machine", machineCell, "text", 5, "background", 14, "foreground", 15);
            TreeViewColumn formatColumn = new TreeViewColumn("Format", formatCell, "text", 6, "background", 14, "foreground", 15);
            TreeViewColumn descriptionColumn = new TreeViewColumn("Description", descriptionCell, "text", 7, "background", 14, "foreground", 15);
            TreeViewColumn oemColumn = new TreeViewColumn("OEM?", oemCell, "active", 8);
            TreeViewColumn upgradeColumn = new TreeViewColumn("Upgrade?", upgradeCell, "active", 9);
            TreeViewColumn updateColumn = new TreeViewColumn("Update?", updateCell, "active", 10);
            TreeViewColumn sourceColumn = new TreeViewColumn("Source?", sourceCell, "active", 11);
            TreeViewColumn filesColumn = new TreeViewColumn("Files?", filesCell, "active", 12);
            TreeViewColumn netinstallColumn = new TreeViewColumn("NetInstall?", netinstallCell, "active", 13);
            TreeViewColumn pathColumn = new TreeViewColumn("Path in repo", pathCell, "text", 16, "background", 14, "foreground", 15);

            osView = new ListStore(typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string),
                                     typeof(bool), typeof(bool), typeof(bool), typeof(bool), typeof(bool), typeof(bool), typeof(string), typeof(string), typeof(string),
                                   typeof(long), typeof(string));

            treeOSes.Model = osView;
            treeOSes.AppendColumn(developerColumn);
            treeOSes.AppendColumn(productColumn);
            treeOSes.AppendColumn(versionColumn);
            treeOSes.AppendColumn(languagesColumn);
            treeOSes.AppendColumn(architectureColumn);
            treeOSes.AppendColumn(machineColumn);
            treeOSes.AppendColumn(formatColumn);
            treeOSes.AppendColumn(descriptionColumn);
            treeOSes.AppendColumn(oemColumn);
            treeOSes.AppendColumn(upgradeColumn);
            treeOSes.AppendColumn(updateColumn);
            treeOSes.AppendColumn(sourceColumn);
            treeOSes.AppendColumn(filesColumn);
            treeOSes.AppendColumn(netinstallColumn);
            treeOSes.AppendColumn(pathColumn);

            treeOSes.Selection.Mode = SelectionMode.Single;

            CellRendererText hashCell = new CellRendererText();
            CellRendererText lengthCell = new CellRendererText();
            CellRendererToggle crackCell = new CellRendererToggle();
            CellRendererToggle virscanCell = new CellRendererToggle();
            CellRendererText clamtimeCell = new CellRendererText();
            CellRendererText vtottimeCell = new CellRendererText();
            CellRendererText virusCell = new CellRendererText();

            TreeViewColumn hashColumn = new TreeViewColumn("SHA256", hashCell, "text", 0, "background", 7, "foreground", 8);
            TreeViewColumn lengthColumn = new TreeViewColumn("Length", lengthCell, "text", 1, "background", 7, "foreground", 8);
            TreeViewColumn crackColumn = new TreeViewColumn("Crack?", crackCell, "active", 2);
            TreeViewColumn virscanColumn = new TreeViewColumn("Scanned for virus?", virscanCell, "active", 3, "inconsistent", 9);
            TreeViewColumn clamtimeColumn = new TreeViewColumn("Last scanned with clamd", clamtimeCell, "text", 4, "background", 7, "foreground", 8);
            TreeViewColumn vtottimeColumn = new TreeViewColumn("Last checked on VirusTotal", vtottimeCell, "text", 5, "background", 7, "foreground", 8);
            TreeViewColumn virusColumn = new TreeViewColumn("Virus", virusCell, "text", 6, "background", 7, "foreground", 8);

            fileView = new ListStore(typeof(string), typeof(long), typeof(bool), typeof(bool), typeof(string), typeof(string),
                                     typeof(string), typeof(string), typeof(string), typeof(bool), typeof(bool));

            treeFiles.Model = fileView;
            treeFiles.AppendColumn(hashColumn);
            treeFiles.AppendColumn(lengthColumn);
            treeFiles.AppendColumn(crackColumn);
            treeFiles.AppendColumn(virscanColumn);
            treeFiles.AppendColumn(clamtimeColumn);
            treeFiles.AppendColumn(vtottimeColumn);
            treeFiles.AppendColumn(virusColumn);

            treeFiles.Selection.Mode = SelectionMode.Single;
            treeFiles.Selection.Changed += treeFilesSelectionChanged;

            thdPulseProgress = new Thread(() =>
            {
                while(true)
                {
                    Application.Invoke(delegate
                    {
                        prgProgress.Pulse();
                    });
                    Thread.Sleep(66);
                }
            });
            Workers.Failed += LoadOSesFailed;
            Workers.Finished += LoadOSesFinished;
            Workers.UpdateProgress += UpdateProgress;
            Workers.AddOS += AddOS;
            thdPopulateOSes = new Thread(Workers.GetAllOSes);
            thdPopulateOSes.Start();
        }

        void LoadOSesFailed(string text)
        {
            Application.Invoke(delegate
            {
                MessageDialog dlgMsg = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok,
                                                         string.Format("Error {0} when populating OSes tree, exiting...", text));
                dlgMsg.Run();
                dlgMsg.Destroy();
                Workers.Failed -= LoadOSesFailed;
                Workers.Finished -= LoadOSesFinished;
                Workers.UpdateProgress -= UpdateProgress;
                if(thdPulseProgress != null)
                {
                    thdPulseProgress.Abort();
                    thdPulseProgress = null;
                }
                if(thdPopulateOSes != null)
                {
                    thdPopulateOSes.Abort();
                    thdPopulateOSes = null;
                }
                Application.Quit();
            });
        }

        void LoadOSesFinished()
        {
            Application.Invoke(delegate
            {
                Workers.Failed -= LoadOSesFailed;
                Workers.Finished -= LoadOSesFinished;
                Workers.UpdateProgress -= UpdateProgress;
                if(thdPulseProgress != null)
                {
                    thdPulseProgress.Abort();
                    thdPulseProgress = null;
                }
                if(thdPopulateOSes != null)
                {
                    thdPopulateOSes.Abort();
                    thdPopulateOSes = null;
                }
                lblProgress.Visible = false;
                prgProgress.Visible = false;
                treeOSes.Sensitive = true;
                btnAdd.Visible = true;
                btnRemove.Visible = true;
                btnCompress.Visible = true;
                btnSave.Visible = true;
                btnHelp.Visible = true;
                btnSettings.Visible = true;
            });
        }

        public void UpdateProgress(string text, string inner, long current, long maximum)
        {
            Application.Invoke(delegate
            {
                lblProgress.Text = text;
                prgProgress.Text = inner;
                if(maximum > 0)
                    prgProgress.Fraction = current / (double)maximum;
                else
                    prgProgress.Pulse();
            });
        }

        public void UpdateProgress2(string text, string inner, long current, long maximum)
        {
            Application.Invoke(delegate
            {
                lblProgress2.Text = text;
                prgProgress2.Text = inner;
                if(maximum > 0)
                    prgProgress2.Fraction = current / (double)maximum;
                else
                    prgProgress2.Pulse();
            });
        }

        void AddOS(DBEntry os, bool existsInRepo, string pathInRepo)
        {
            Application.Invoke(delegate
            {
                if(thdPulseProgress != null)
                {
                    thdPulseProgress.Abort();
                    thdPulseProgress = null;
                }

                string color = existsInRepo ? "green" : "red";
                osView.AppendValues(os.developer, os.product, os.version, os.languages, os.architecture, os.machine,
                                    os.format, os.description, os.oem, os.upgrade, os.update, os.source,
                                    os.files, os.netinstall, color, "black", pathInRepo, os.id, os.mdid);
            });
        }

        protected void OnBtnAddClicked(object sender, EventArgs e)
        {
            dlgAdd _dlgAdd = new dlgAdd();
            _dlgAdd.OnAddedOS += (os, existsInRepo, pathInRepo) =>
            {
                string color = existsInRepo ? "green" : "red";
                osView.AppendValues(os.developer, os.product, os.version, os.languages, os.architecture, os.machine,
                                    os.format, os.description, os.oem, os.upgrade, os.update, os.source,
                                    os.files, os.netinstall, color, "black", pathInRepo, os.id, os.mdid);
            };
            _dlgAdd.Run();
            _dlgAdd.Destroy();
        }

        protected void OnBtnRemoveClicked(object sender, EventArgs e)
        {
            TreeIter osIter;
            if(treeOSes.Selection.GetSelected(out osIter))
            {
                MessageDialog dlgMsg = new MessageDialog(this, DialogFlags.Modal, MessageType.Question, ButtonsType.YesNo,
                             "Are you sure you want to remove the selected OS?");

                if(dlgMsg.Run() == (int)ResponseType.Yes)
                {
                    Workers.RemoveOS((long)osView.GetValue(osIter, 17), (string)osView.GetValue(osIter, 18));
                    osView.Remove(ref osIter);
                }

                dlgMsg.Destroy();
            }
        }

        protected void OnBtnSaveClicked(object sender, EventArgs e)
        {
            TreeIter osIter;
            if(treeOSes.Selection.GetSelected(out osIter))
            {
                Context.dbInfo.id = (long)osView.GetValue(osIter, 17);

                FileChooserDialog dlgFolder = new FileChooserDialog("Save to...", this, FileChooserAction.SelectFolder,
                                                     "Cancel", ResponseType.Cancel, "Choose", ResponseType.Accept);
                dlgFolder.SelectMultiple = false;

                if(dlgFolder.Run() == (int)ResponseType.Accept)
                {
                    Context.path = dlgFolder.Filename;

                    dlgFolder.Destroy();

                    lblProgress.Visible = true;
                    prgProgress.Visible = true;
                    lblProgress2.Visible = true;
                    prgProgress2.Visible = true;
                    treeOSes.Sensitive = false;
                    btnAdd.Visible = false;
                    btnRemove.Visible = false;
                    btnCompress.Visible = false;
                    btnSave.Visible = false;
                    btnHelp.Visible = false;
                    btnSettings.Visible = false;
                    btnStop.Visible = true;

                    if(thdPulseProgress != null)
                    {
                        thdPulseProgress.Abort();
                        thdPulseProgress = null;
                    }
                    Workers.Failed += SaveAsFailed;
                    Workers.Finished += SaveAsFinished;
                    Workers.UpdateProgress += UpdateProgress;
                    Workers.UpdateProgress2 += UpdateProgress2;
                    thdSaveAs = new Thread(Workers.SaveAs);
                    thdSaveAs.Start();
                }
                else
                    dlgFolder.Destroy();
            }
        }

        public void SaveAsFailed(string text)
        {
            Application.Invoke(delegate
            {
                MessageDialog dlgMsg = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, text);
                dlgMsg.Run();
                dlgMsg.Destroy();
                lblProgress.Visible = false;
                prgProgress.Visible = false;
                lblProgress2.Visible = false;
                prgProgress2.Visible = false;
                treeOSes.Sensitive = true;
                btnAdd.Visible = true;
                btnRemove.Visible = true;
                btnCompress.Visible = true;
                btnSave.Visible = true;
                btnHelp.Visible = true;
                btnSettings.Visible = true;
                btnStop.Visible = false;

                Workers.Failed -= SaveAsFailed;
                Workers.Finished -= SaveAsFinished;
                Workers.UpdateProgress -= UpdateProgress;
                Workers.UpdateProgress2 -= UpdateProgress2;

                if(thdPulseProgress != null)
                {
                    thdPulseProgress.Abort();
                    thdPulseProgress = null;
                }
                if(thdSaveAs != null)
                {
                    thdSaveAs.Abort();
                    thdSaveAs = null;
                }

                Context.path = null;
            });
        }

        public void SaveAsFinished()
        {
            Application.Invoke(delegate
            {
                lblProgress.Visible = false;
                prgProgress.Visible = false;
                lblProgress2.Visible = false;
                prgProgress2.Visible = false;
                treeOSes.Sensitive = true;
                btnAdd.Visible = true;
                btnRemove.Visible = true;
                btnCompress.Visible = true;
                btnSave.Visible = true;
                btnHelp.Visible = true;
                btnSettings.Visible = true;
                btnStop.Visible = false;

                Workers.Failed -= SaveAsFailed;
                Workers.Finished -= SaveAsFinished;
                Workers.UpdateProgress -= UpdateProgress;

                if(thdPulseProgress != null)
                {
                    thdPulseProgress.Abort();
                    thdPulseProgress = null;
                }
                if(thdSaveAs != null)
                {
                    thdSaveAs.Abort();
                    thdSaveAs = null;
                }

                MessageDialog dlgMsg = new MessageDialog(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok,
                                                         string.Format("Correctly saved to {0}", Context.path));
                dlgMsg.Run();
                dlgMsg.Destroy();

                Context.path = null;
            });
        }

        protected void OnBtnHelpClicked(object sender, EventArgs e)
        {
            dlgHelp _help = new dlgHelp();
            _help.Run();
            _help.Destroy();
        }

        protected void OnBtnSettingsClicked(object sender, EventArgs e)
        {
            dlgSettings _dlgSettings = new dlgSettings();
            _dlgSettings.Run();
            _dlgSettings.Destroy();
        }

        protected void OnBtnQuitClicked(object sender, EventArgs e)
        {
            OnBtnStopClicked(sender, e);
            Application.Quit();
        }

        protected void OnBtnStopClicked(object sender, EventArgs e)
        {
            Workers.Failed -= CompressToFailed;
            Workers.Failed -= LoadOSesFailed;
            Workers.Failed -= SaveAsFailed;
            Workers.Finished -= CompressToFinished;
            Workers.Finished -= LoadOSesFinished;
            Workers.Finished -= SaveAsFinished;
            Workers.UpdateProgress -= UpdateProgress;
            Workers.UpdateProgress2 -= UpdateProgress2;

            if(thdPulseProgress != null)
            {
                thdPulseProgress.Abort();
                thdPulseProgress = null;
            }
            if(thdPopulateOSes != null)
            {
                thdPopulateOSes.Abort();
                thdPopulateOSes = null;
            }
            if(thdCompressTo != null)
            {
                thdPopulateOSes.Abort();
                thdPopulateOSes = null;
            }
            if(thdSaveAs != null)
            {
                thdSaveAs.Abort();
                thdSaveAs = null;
            }
        }

        protected void OnDeleteEvent(object sender, DeleteEventArgs e)
        {
            OnBtnStopClicked(sender, e);
        }

        protected void OnBtnCompressClicked(object sender, EventArgs e)
        {
            TreeIter osIter;
            if(treeOSes.Selection.GetSelected(out osIter))
            {
                Context.dbInfo.id = (long)osView.GetValue(osIter, 17);

                FileChooserDialog dlgFolder = new FileChooserDialog("Compress to...", this, FileChooserAction.Save,
                                                     "Cancel", ResponseType.Cancel, "Choose", ResponseType.Accept);
                dlgFolder.SelectMultiple = false;

                if(dlgFolder.Run() == (int)ResponseType.Accept)
                {
                    Context.path = dlgFolder.Filename;

                    dlgFolder.Destroy();

                    lblProgress.Visible = true;
                    prgProgress.Visible = true;
                    lblProgress2.Visible = true;
                    prgProgress2.Visible = true;
                    treeOSes.Sensitive = false;
                    btnAdd.Visible = false;
                    btnRemove.Visible = false;
                    btnCompress.Visible = false;
                    btnSave.Visible = false;
                    btnHelp.Visible = false;
                    btnSettings.Visible = false;
                    btnStop.Visible = true;

                    if(thdPulseProgress != null)
                    {
                        thdPulseProgress.Abort();
                        thdPulseProgress = null;
                    }
                    Workers.Failed += CompressToFailed;
                    Workers.Finished += CompressToFinished;
                    Workers.UpdateProgress += UpdateProgress;
                    Workers.UpdateProgress2 += UpdateProgress2;
                    thdCompressTo = new Thread(Workers.CompressTo);
                    thdCompressTo.Start();
                }
                else
                    dlgFolder.Destroy();
            }
        }

        public void CompressToFailed(string text)
        {
            Application.Invoke(delegate
            {
                MessageDialog dlgMsg = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, text);
                dlgMsg.Run();
                dlgMsg.Destroy();

                lblProgress.Visible = false;
                lblProgress2.Visible = false;
                prgProgress.Visible = false;
                prgProgress2.Visible = false;
                treeOSes.Sensitive = true;
                btnAdd.Visible = true;
                btnRemove.Visible = true;
                btnCompress.Visible = true;
                btnSave.Visible = true;
                btnHelp.Visible = true;
                btnSettings.Visible = true;
                btnStop.Visible = false;

                Workers.Failed -= CompressToFailed;
                Workers.Finished -= CompressToFinished;
                Workers.UpdateProgress -= UpdateProgress;
                Workers.UpdateProgress2 -= UpdateProgress2;

                if(thdPulseProgress != null)
                {
                    thdPulseProgress.Abort();
                    thdPulseProgress = null;
                }
                if(thdCompressTo != null)
                {
                    thdCompressTo.Abort();
                    thdCompressTo = null;
                }

                Context.path = null;
            });
        }

        public void CompressToFinished()
        {
            Application.Invoke(delegate
            {
                lblProgress.Visible = false;
                lblProgress2.Visible = false;
                prgProgress.Visible = false;
                prgProgress2.Visible = false;
                treeOSes.Sensitive = true;
                btnAdd.Visible = true;
                btnRemove.Visible = true;
                btnCompress.Visible = true;
                btnSave.Visible = true;
                btnHelp.Visible = true;
                btnSettings.Visible = true;
                btnStop.Visible = false;

                Workers.Failed -= CompressToFailed;
                Workers.Finished -= CompressToFinished;
                Workers.UpdateProgress -= UpdateProgress;
                Workers.UpdateProgress2 -= UpdateProgress2;

                if(thdPulseProgress != null)
                {
                    thdPulseProgress.Abort();
                    thdPulseProgress = null;
                }
                if(thdCompressTo != null)
                {
                    thdCompressTo.Abort();
                    thdCompressTo = null;
                }

                MessageDialog dlgMsg = new MessageDialog(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok,
                                                         string.Format("Correctly compressed as {0}", Context.path));
                dlgMsg.Run();
                dlgMsg.Destroy();

                Context.path = null;
            });
        }

        protected void OnBtnStopFilesClicked(object sender, EventArgs e)
        {
            if(populatingFiles)
            {
                Workers.Failed -= LoadFilesFailed;
                Workers.Finished -= LoadFilesFinished;
                Workers.UpdateProgress -= UpdateFileProgress;
                Workers.AddFile -= AddFile;

                if(thdPulseProgress != null)
                {
                    thdPulseProgress.Abort();
                    thdPulseProgress = null;
                }
                if(thdPopulateFiles != null)
                {
                    thdPopulateFiles.Abort();
                    thdPopulateFiles = null;
                }

                fileView.Clear();
                btnStopFiles.Visible = false;
                btnPopulateFiles.Visible = true;
            }
        }

        protected void OnBtnToggleCrackClicked(object sender, EventArgs e)
        {
            TreeIter fileIter;
            if(treeFiles.Selection.GetSelected(out fileIter))
            {
                string hash = (string)fileView.GetValue(fileIter, 0);
                long length = (long)fileView.GetValue(fileIter, 1);
                bool crack = !(bool)fileView.GetValue(fileIter, 2);
                bool hasvirus = (bool)fileView.GetValue(fileIter, 3);
                string clamtime = (string)fileView.GetValue(fileIter, 4);
                string vttime = (string)fileView.GetValue(fileIter, 5);
                string virus = (string)fileView.GetValue(fileIter, 6);
                string color = (string)fileView.GetValue(fileIter, 7);
                bool viruschecked = (bool)fileView.GetValue(fileIter, 9);

                Workers.ToggleCrack(hash, crack);

                fileView.Remove(ref fileIter);
                fileView.AppendValues(hash, length, crack, hasvirus, clamtime, vttime, virus, color, "black", viruschecked);
            }
        }

        protected void OnBtnScanWithClamdClicked(object sender, EventArgs e)
        {
            if(treeFiles.Selection.GetSelected(out outIter))
            {
                DBFile file = Workers.GetDBFile((string)fileView.GetValue(outIter, 0));

                if(file == null)
                {
                    MessageDialog dlgMsg = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok,
                                                 "Cannot get file from database");
                    dlgMsg.Run();
                    dlgMsg.Destroy();
                    return;
                }

                treeFiles.Sensitive = false;
                btnToggleCrack.Sensitive = false;
                btnScanWithClamd.Sensitive = false;
                btnCheckInVirusTotal.Sensitive = false;
                prgProgress.Visible = true;
                Workers.Failed += ClamdFailed;
                Workers.ScanFinished += ClamdFinished;

                prgProgress.Text = "Scanning file with clamd.";
                thdPulseProgress = new Thread(() =>
                {
                    while(true)
                    {
                        Application.Invoke(delegate
                        {
                            prgProgress.Pulse();
                        });
                        Thread.Sleep(66);
                    }
                });

                thdScanFile = new Thread(() => Workers.ClamScanFileFromRepo(file));
                thdScanFile.Start();
            }
        }

        void ClamdFailed(string text)
        {
            Application.Invoke(delegate
            {
                treeFiles.Sensitive = true;
                btnToggleCrack.Sensitive = true;
                btnScanWithClamd.Sensitive = true;
                btnCheckInVirusTotal.Sensitive = true;
                prgProgress.Visible = false;
                Workers.Failed -= ClamdFailed;
                Workers.ScanFinished -= ClamdFinished;
                prgProgress.Text = "";
                if(thdPulseProgress != null)
                {
                    thdPulseProgress.Abort();
                    thdPulseProgress = null;
                }
                if(thdScanFile != null)
                {
                    thdScanFile.Abort();
                    thdScanFile = null;
                }
            });
        }

        void ClamdFinished(DBFile file)
        {
            Application.Invoke(delegate
            {
                treeFiles.Sensitive = true;
                btnToggleCrack.Sensitive = true;
                btnScanWithClamd.Sensitive = true;
                btnCheckInVirusTotal.Sensitive = true;
                Workers.Failed -= ClamdFailed;
                Workers.ScanFinished -= ClamdFinished;
                prgProgress.Text = "";
                prgProgress.Visible = false;
                if(thdPulseProgress != null)
                {
                    thdPulseProgress.Abort();
                    thdPulseProgress = null;
                }
                if(thdScanFile != null)
                    thdScanFile = null;

                fileView.Remove(ref outIter);
                AddFile(file);
            });
        }

        protected void OnBtnCheckInVirusTotalClicked(object sender, EventArgs e)
        {
        }

        protected void OnBtnPopulateFilesClicked(object sender, EventArgs e)
        {
            // TODO: Implement
            btnCheckInVirusTotal.Sensitive = false;

            notebook1.GetNthPage(0).Sensitive = false;
            btnStopFiles.Visible = true;
            btnPopulateFiles.Visible = false;

            thdPulseProgress = new Thread(() =>
            {
                while(true)
                {
                    Application.Invoke(delegate
                    {
                        prgProgressFiles1.Pulse();
                    });
                    Thread.Sleep(66);
                }
            });
            lblProgressFiles1.Text = "Loading files from database";
            lblProgressFiles1.Visible = true;
            lblProgressFiles2.Visible = true;
            prgProgressFiles1.Visible = true;
            prgProgressFiles2.Visible = true;
            Workers.Failed += LoadFilesFailed;
            Workers.Finished += LoadFilesFinished;
            Workers.UpdateProgress += UpdateFileProgress;
            Workers.AddFile += AddFile;
            Workers.AddFiles += AddFiles;
            populatingFiles = true;
            thdPulseProgress.Start();
            thdPopulateFiles = new Thread(Workers.GetFilesFromDb);
            thdPopulateFiles.Start();
        }

        public void UpdateFileProgress(string text, string inner, long current, long maximum)
        {
            Application.Invoke(delegate
            {
                lblProgressFiles2.Text = text;
                prgProgressFiles2.Text = inner;
                if(maximum > 0)
                    prgProgressFiles2.Fraction = current / (double)maximum;
                else
                    prgProgressFiles2.Pulse();
            });
        }

        void AddFile(DBFile file)
        {
            Application.Invoke(delegate
            {
                if(thdPulseProgress != null)
                {
                    thdPulseProgress.Abort();
                    thdPulseProgress = null;
                }

                string color;

                if(file.HasVirus.HasValue)
                {
                    color = file.HasVirus.Value ? "red" : "green";
                }
                else
                    color = "yellow";

                fileView.AppendValues(file.Sha256, file.Length, file.Crack, file.HasVirus.HasValue ? file.HasVirus.Value : false,
                                      file.ClamTime == null ? "Never" : file.ClamTime.Value.ToString(),
                                      file.VirusTotalTime == null ? "Never" : file.VirusTotalTime.Value.ToString(),
                                      file.Virus, color, "black", !file.HasVirus.HasValue);
            });
        }

        void AddFiles(List<DBFile> files)
        {
            Application.Invoke(delegate
            {
                if(thdPulseProgress != null)
                {
                    thdPulseProgress.Abort();
                    thdPulseProgress = null;
                }

                foreach(DBFile file in files)
                {
                    string color;

                    if(file.HasVirus.HasValue)
                    {
                        color = file.HasVirus.Value ? "red" : "green";
                    }
                    else
                        color = "yellow";

                    fileView.AppendValues(file.Sha256, file.Length, file.Crack, file.HasVirus.HasValue ? file.HasVirus.Value : false,
                                          file.ClamTime == null ? "Never" : file.ClamTime.Value.ToString(),
                                          file.VirusTotalTime == null ? "Never" : file.VirusTotalTime.Value.ToString(),
                                          file.Virus, color, "black", !file.HasVirus.HasValue);
                }
            });
        }

        void LoadFilesFailed(string text)
        {
            Application.Invoke(delegate
            {
                MessageDialog dlgMsg = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok,
                                             string.Format("Error {0} when populating files, exiting...", text));
                dlgMsg.Run();
                dlgMsg.Destroy();
                Workers.Failed -= LoadFilesFailed;
                Workers.Finished -= LoadFilesFinished;
                Workers.UpdateProgress -= UpdateFileProgress;
                if(thdPulseProgress != null)
                {
                    thdPulseProgress.Abort();
                    thdPulseProgress = null;
                }
                if(thdPopulateFiles != null)
                {
                    thdPopulateFiles.Abort();
                    thdPopulateFiles = null;
                }
                notebook1.GetNthPage(0).Sensitive = true;
                fileView.Clear();
                btnStopFiles.Visible = false;
                btnPopulateFiles.Visible = true;
                populatingFiles = false;
            });
        }

        void LoadFilesFinished()
        {
            Application.Invoke(delegate
            {
                Workers.Failed -= LoadFilesFailed;
                Workers.Finished -= LoadFilesFinished;
                Workers.UpdateProgress -= UpdateFileProgress;
                if(thdPulseProgress != null)
                {
                    thdPulseProgress.Abort();
                    thdPulseProgress = null;
                }
                if(thdPopulateFiles != null)
                {
                    thdPopulateFiles.Abort();
                    thdPopulateFiles = null;
                }
                lblProgressFiles1.Visible = false;
                lblProgressFiles2.Visible = false;
                prgProgressFiles1.Visible = false;
                prgProgressFiles2.Visible = false;
                btnToggleCrack.Visible = true;
                btnScanWithClamd.Visible = true;
                btnCheckInVirusTotal.Visible = true;
                btnStopFiles.Visible = false;
                btnPopulateFiles.Visible = false;
                populatingFiles = false;
                treeFiles.Sensitive = true;
                notebook1.GetNthPage(0).Sensitive = true;
            });
        }

        void treeFilesSelectionChanged(object sender, EventArgs e)
        {
            TreeIter fileIter;
            if(treeFiles.Selection.GetSelected(out fileIter))
            {
                if((bool)fileView.GetValue(fileIter, 2))
                    btnToggleCrack.Label = "Mark as not crack";
                else
                    btnToggleCrack.Label = "Mark as crack";
            }
        }
    }
}