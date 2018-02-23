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
using System.Collections.ObjectModel;
using System.Threading;
using apprepodbmgr.Core;
using Eto.Drawing;
using Eto.Forms;
using Eto.Serialization.Xaml;

namespace apprepodbmgr.Eto
{
    public class frmMain : Form
    {
        int infectedFiles;

        ObservableCollection<DBEntryForEto> lstApps;
        ObservableCollection<DbFile>        lstFiles;
        DbFile                              outIter;
        bool                                populatingFiles;
        bool                                scanningFiles;
        Thread                              thdCleanFiles;
        Thread                              thdCompressTo;
        Thread                              thdPopulateApps;
        Thread                              thdPopulateFiles;
        Thread                              thdSaveAs;
        Thread                              thdScanFile;

        public frmMain()
        {
            XamlReader.Load(this);

            Workers.InitDB();

            lstApps = new ObservableCollection<DBEntryForEto>();

            treeApps.DataStore = lstApps;
            treeApps.Columns.Add(new GridColumn
            {
                DataCell   = new TextBoxCell {Binding = Binding.Property<DBEntryForEto, string>(r => r.developer)},
                HeaderText = "Developer"
            });
            treeApps.Columns.Add(new GridColumn
            {
                DataCell   = new TextBoxCell {Binding = Binding.Property<DBEntryForEto, string>(r => r.product)},
                HeaderText = "Product"
            });
            treeApps.Columns.Add(new GridColumn
            {
                DataCell   = new TextBoxCell {Binding = Binding.Property<DBEntryForEto, string>(r => r.version)},
                HeaderText = "Version"
            });
            treeApps.Columns.Add(new GridColumn
            {
                DataCell   = new TextBoxCell {Binding = Binding.Property<DBEntryForEto, string>(r => r.languages)},
                HeaderText = "Languages"
            });
            treeApps.Columns.Add(new GridColumn
            {
                DataCell   = new TextBoxCell {Binding = Binding.Property<DBEntryForEto, string>(r => r.architecture)},
                HeaderText = "Architecture"
            });
            treeApps.Columns.Add(new GridColumn
            {
                DataCell   = new TextBoxCell {Binding = Binding.Property<DBEntryForEto, string>(r => r.machine)},
                HeaderText = "Machine"
            });
            treeApps.Columns.Add(new GridColumn
            {
                DataCell   = new TextBoxCell {Binding = Binding.Property<DBEntryForEto, string>(r => r.format)},
                HeaderText = "Format"
            });
            treeApps.Columns.Add(new GridColumn
            {
                DataCell   = new TextBoxCell {Binding = Binding.Property<DBEntryForEto, string>(r => r.description)},
                HeaderText = "Description"
            });
            treeApps.Columns.Add(new GridColumn
            {
                DataCell   = new CheckBoxCell {Binding = Binding.Property<DBEntryForEto, bool?>(r => r.oem)},
                HeaderText = "OEM?"
            });
            treeApps.Columns.Add(new GridColumn
            {
                DataCell   = new CheckBoxCell {Binding = Binding.Property<DBEntryForEto, bool?>(r => r.upgrade)},
                HeaderText = "Upgrade?"
            });
            treeApps.Columns.Add(new GridColumn
            {
                DataCell   = new CheckBoxCell {Binding = Binding.Property<DBEntryForEto, bool?>(r => r.update)},
                HeaderText = "Update?"
            });
            treeApps.Columns.Add(new GridColumn
            {
                DataCell   = new CheckBoxCell {Binding = Binding.Property<DBEntryForEto, bool?>(r => r.source)},
                HeaderText = "Source?"
            });
            treeApps.Columns.Add(new GridColumn
            {
                DataCell   = new CheckBoxCell {Binding = Binding.Property<DBEntryForEto, bool?>(r => r.files)},
                HeaderText = "Files?"
            });
            treeApps.Columns.Add(new GridColumn
            {
                DataCell   = new CheckBoxCell {Binding = Binding.Property<DBEntryForEto, bool?>(r => r.netinstall)},
                HeaderText = "NetInstall?"
            });

            treeApps.AllowMultipleSelection = false;

            lstFiles = new ObservableCollection<DbFile>();

            treeFiles.DataStore = lstFiles;
            treeFiles.Columns.Add(new GridColumn
            {
                DataCell   = new TextBoxCell {Binding = Binding.Property<DbFile, string>(r => r.Sha256)},
                HeaderText = "SHA256"
            });
            treeFiles.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell
                {
                    Binding = Binding.Property<DbFile, long>(r => r.Length).Convert(s => s.ToString())
                },
                HeaderText = "Length"
            });
            treeFiles.Columns.Add(new GridColumn
            {
                DataCell   = new CheckBoxCell {Binding = Binding.Property<DbFile, bool?>(r => r.Crack)},
                HeaderText = "Crack?"
            });
            treeFiles.Columns.Add(new GridColumn
            {
                DataCell   = new CheckBoxCell {Binding = Binding.Property<DbFile, bool?>(r => r.HasVirus)},
                HeaderText = "Has virus?"
            });
            treeFiles.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell
                {
                    Binding = Binding.Property<DbFile, DateTime?>(r => r.ClamTime)
                                     .Convert(s => s == null ? "Never" : s.Value.ToString())
                },
                HeaderText = "Last scanned with clamd"
            });
            treeFiles.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell
                {
                    Binding = Binding.Property<DbFile, DateTime?>(r => r.VirusTotalTime)
                                     .Convert(s => s == null ? "Never" : s.Value.ToString())
                },
                HeaderText = "Last checked on VirusTotal"
            });
            treeFiles.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell
                {
                    Binding = Binding.Property<DbFile, string>(r => r.Virus).Convert(s => s ?? "None")
                },
                HeaderText = "Virus"
            });

            treeFiles.AllowMultipleSelection =  false;
            treeFiles.CellFormatting         += (sender, e) =>
            {
                if(((DbFile)e.Item).HasVirus.HasValue)
                    e.BackgroundColor  = ((DbFile)e.Item).HasVirus.Value ? Colors.Red : Colors.Green;
                else e.BackgroundColor = Colors.Yellow;

                e.ForegroundColor = Colors.Black;
            };

            prgProgress.Indeterminate = true;

            if(!Context.UsableDotNetZip)
            {
                btnCompress.Visible = false;
                mnuCompress.Enabled = false;
            }

            Workers.Failed         += LoadAppsFailed;
            Workers.Finished       += LoadAppsFinished;
            Workers.UpdateProgress += UpdateProgress;
            Workers.AddApp         += AddApp;
            Workers.AddFile        += AddFile;
            Workers.AddFiles       += AddFiles;
            thdPopulateApps        =  new Thread(Workers.GetAllApps);
            thdPopulateApps.Start();
        }

        void LoadAppsFailed(string text)
        {
            Application.Instance.Invoke(delegate
            {
                MessageBox.Show($"Error {text} when populating applications, exiting...", MessageBoxButtons.OK,
                                MessageBoxType.Error, MessageBoxDefaultButton.OK);
                if(thdPopulateApps != null)
                {
                    thdPopulateApps.Abort();
                    thdPopulateApps = null;
                }

                Workers.Failed         -= LoadAppsFailed;
                Workers.Finished       -= LoadAppsFinished;
                Workers.UpdateProgress -= UpdateProgress;
                Application.Instance.Quit();
            });
        }

        void LoadAppsFinished()
        {
            Application.Instance.Invoke(delegate
            {
                Workers.Failed         -= LoadAppsFailed;
                Workers.Finished       -= LoadAppsFinished;
                Workers.UpdateProgress -= UpdateProgress;
                Workers.AddApp         -= AddApp;
                if(thdPopulateApps != null)
                {
                    thdPopulateApps.Abort();
                    thdPopulateApps = null;
                }

                lblProgress.Visible  = false;
                prgProgress.Visible  = false;
                treeApps.Enabled     = true;
                btnAdd.Visible       = true;
                btnRemove.Visible    = true;
                btnCompress.Visible  = Context.UsableDotNetZip;
                btnSave.Visible      = true;
                btnSettings.Enabled  = true;
                lblAppStatus.Visible = true;
                lblAppStatus.Text    = $"{lstApps.Count} applications";
            });
        }

        void UpdateProgress(string text, string inner, long current, long maximum)
        {
            Application.Instance.Invoke(delegate
            {
                if(!string.IsNullOrWhiteSpace(text) && !string.IsNullOrWhiteSpace(inner))
                    lblProgress.Text = $"{text}: {inner}";
                else if(!string.IsNullOrWhiteSpace(inner))
                    lblProgress.Text = inner;
                else
                    lblProgress.Text = text;
                if(maximum > 0)
                {
                    if(current < int.MinValue || current > int.MaxValue || maximum < int.MinValue ||
                       maximum > int.MaxValue)
                    {
                        current /= 100;
                        maximum /= 100;
                    }

                    prgProgress.Indeterminate = false;
                    prgProgress.MinValue      = 0;
                    prgProgress.MaxValue      = (int)maximum;
                    prgProgress.Value         = (int)current;
                }
                else prgProgress.Indeterminate = true;
            });
        }

        void UpdateProgress2(string text, string inner, long current, long maximum)
        {
            Application.Instance.Invoke(delegate
            {
                if(!string.IsNullOrWhiteSpace(text) && !string.IsNullOrWhiteSpace(inner))
                    lblProgress2.Text = $"{text}: {inner}";
                else if(!string.IsNullOrWhiteSpace(inner))
                    lblProgress2.Text = inner;
                else
                    lblProgress2.Text = text;
                if(maximum > 0)
                {
                    if(current < int.MinValue || current > int.MaxValue || maximum < int.MinValue ||
                       maximum > int.MaxValue)
                    {
                        current /= 100;
                        maximum /= 100;
                    }

                    prgProgress2.Indeterminate = false;
                    prgProgress2.MinValue      = 0;
                    prgProgress2.MaxValue      = (int)maximum;
                    prgProgress2.Value         = (int)current;
                }
                else prgProgress2.Indeterminate = true;
            });
        }

        void AddApp(DbEntry app)
        {
            Application.Instance.Invoke(delegate { lstApps.Add(new DBEntryForEto(app)); });
        }

        protected void OnBtnAddClicked(object sender, EventArgs e)
        {
            dlgAdd dlgAdd     = new dlgAdd();
            dlgAdd.OnAddedApp += app => { lstApps.Add(new DBEntryForEto(app)); };
            dlgAdd.ShowModal(this);
        }

        protected void OnBtnRemoveClicked(object sender, EventArgs e)
        {
            if(treeApps.SelectedItem == null) return;
            if(MessageBox.Show("Are you sure you want to remove the selected application?", MessageBoxButtons.YesNo,
                               MessageBoxType.Question, MessageBoxDefaultButton.No) != DialogResult.Yes) return;

            Workers.RemoveApp(((DBEntryForEto)treeApps.SelectedItem).id, ((DBEntryForEto)treeApps.SelectedItem).mdid);
            lstApps.Remove((DBEntryForEto)treeApps.SelectedItem);
        }

        protected void OnBtnSaveClicked(object sender, EventArgs e)
        {
            if(treeApps.SelectedItem == null) return;

            SelectFolderDialog dlgFolder = new SelectFolderDialog {Title = "Save to..."};
            if(dlgFolder.ShowDialog(this) != DialogResult.Ok) return;

            Context.DbInfo.Id = ((DBEntryForEto)treeApps.SelectedItem).id;
            Context.Path      = dlgFolder.Directory;

            lblProgress.Visible  = true;
            prgProgress.Visible  = true;
            lblProgress2.Visible = true;
            prgProgress2.Visible = true;
            treeApps.Enabled     = false;
            btnAdd.Visible       = false;
            btnRemove.Visible    = false;
            btnCompress.Visible  = false;
            btnSave.Visible      = false;
            btnSettings.Enabled  = false;
            btnStop.Visible      = true;

            Workers.Failed          += SaveAsFailed;
            Workers.Finished        += SaveAsFinished;
            Workers.UpdateProgress  += UpdateProgress;
            Workers.UpdateProgress2 += UpdateProgress2;
            thdSaveAs               =  new Thread(Workers.SaveAs);
            thdSaveAs.Start();
        }

        void SaveAsFailed(string text)
        {
            Application.Instance.Invoke(delegate
            {
                MessageBox.Show(text, MessageBoxButtons.OK, MessageBoxType.Error);

                lblProgress.Visible  = false;
                prgProgress.Visible  = false;
                lblProgress2.Visible = false;
                prgProgress2.Visible = false;
                treeApps.Enabled     = true;
                btnAdd.Visible       = true;
                btnRemove.Visible    = true;
                btnCompress.Visible  = Context.UsableDotNetZip;
                btnSave.Visible      = true;
                btnSettings.Enabled  = true;
                btnStop.Visible      = false;

                Workers.Failed          -= SaveAsFailed;
                Workers.Finished        -= SaveAsFinished;
                Workers.UpdateProgress  -= UpdateProgress;
                Workers.UpdateProgress2 -= UpdateProgress2;

                if(thdSaveAs != null)
                {
                    thdSaveAs.Abort();
                    thdSaveAs = null;
                }

                Context.Path = null;
            });
        }

        void SaveAsFinished()
        {
            Application.Instance.Invoke(delegate
            {
                lblProgress.Visible  = false;
                prgProgress.Visible  = false;
                lblProgress2.Visible = false;
                prgProgress2.Visible = false;
                treeApps.Enabled     = true;
                btnAdd.Visible       = true;
                btnRemove.Visible    = true;
                btnCompress.Visible  = Context.UsableDotNetZip;
                btnSave.Visible      = true;
                btnSettings.Enabled  = true;
                btnStop.Visible      = false;

                Workers.Failed         -= SaveAsFailed;
                Workers.Finished       -= SaveAsFinished;
                Workers.UpdateProgress -= UpdateProgress;

                if(thdSaveAs != null)
                {
                    thdSaveAs.Abort();
                    thdSaveAs = null;
                }

                MessageBox.Show($"Correctly saved to {Context.Path}");

                Context.Path = null;
            });
        }

        protected void OnBtnSettingsClicked(object sender, EventArgs e)
        {
            dlgSettings _dlgSettings = new dlgSettings();
            _dlgSettings.ShowModal();
        }

        protected void OnBtnQuitClicked(object sender, EventArgs e)
        {
            OnBtnStopClicked(sender, e);
            Application.Instance.Quit();
        }

        protected void OnBtnStopClicked(object sender, EventArgs e)
        {
            Workers.AddApp          -= AddApp;
            Workers.Failed          -= CompressToFailed;
            Workers.Failed          -= LoadAppsFailed;
            Workers.Failed          -= SaveAsFailed;
            Workers.Finished        -= CompressToFinished;
            Workers.Finished        -= LoadAppsFinished;
            Workers.Finished        -= SaveAsFinished;
            Workers.UpdateProgress  -= UpdateProgress;
            Workers.UpdateProgress2 -= UpdateProgress2;

            if(thdPopulateApps != null)
            {
                thdPopulateApps.Abort();
                thdPopulateApps = null;
            }

            if(thdCompressTo != null)
            {
                thdPopulateApps.Abort();
                thdPopulateApps = null;
            }

            if(thdSaveAs == null) return;

            thdSaveAs.Abort();
            thdSaveAs = null;
        }

        protected void OnDeleteEvent(object sender, EventArgs e)
        {
            OnBtnStopClicked(sender, e);
        }

        protected void OnBtnCompressClicked(object sender, EventArgs e)
        {
            if(treeApps.SelectedItem == null) return;

            SaveFileDialog dlgFile = new SaveFileDialog {Title = "Compress to..."};

            if(dlgFile.ShowDialog(this) != DialogResult.Ok) return;

            Context.DbInfo.Id = ((DBEntryForEto)treeApps.SelectedItem).id;
            Context.Path      = dlgFile.FileName;

            lblProgress.Visible  = true;
            prgProgress.Visible  = true;
            lblProgress2.Visible = true;
            prgProgress2.Visible = true;
            treeApps.Enabled     = false;
            btnAdd.Visible       = false;
            btnRemove.Visible    = false;
            btnCompress.Visible  = false;
            btnSave.Visible      = false;
            btnSettings.Enabled = false;
            btnStop.Visible     = true;

            Workers.Failed          += CompressToFailed;
            Workers.Finished        += CompressToFinished;
            Workers.UpdateProgress  += UpdateProgress;
            Workers.UpdateProgress2 += UpdateProgress2;
            thdCompressTo           =  new Thread(Workers.CompressTo);
            thdCompressTo.Start();
        }

        void CompressToFailed(string text)
        {
            Application.Instance.Invoke(delegate
            {
                MessageBox.Show(text, MessageBoxButtons.OK, MessageBoxType.Error);
                lblProgress.Visible  = false;
                lblProgress2.Visible = false;
                prgProgress.Visible  = false;
                prgProgress2.Visible = false;
                treeApps.Enabled     = true;
                btnAdd.Visible       = true;
                btnRemove.Visible    = true;
                btnCompress.Visible  = Context.UsableDotNetZip;
                btnSave.Visible      = true;
                btnSettings.Enabled  = true;
                btnStop.Visible      = false;

                Workers.Failed          -= CompressToFailed;
                Workers.Finished        -= CompressToFinished;
                Workers.UpdateProgress  -= UpdateProgress;
                Workers.UpdateProgress2 -= UpdateProgress2;

                if(thdCompressTo != null)
                {
                    thdCompressTo.Abort();
                    thdCompressTo = null;
                }

                Context.Path = null;
            });
        }

        void CompressToFinished()
        {
            Application.Instance.Invoke(delegate
            {
                lblProgress.Visible  = false;
                lblProgress2.Visible = false;
                prgProgress.Visible  = false;
                prgProgress2.Visible = false;
                treeApps.Enabled     = true;
                btnAdd.Visible       = true;
                btnRemove.Visible    = true;
                btnCompress.Visible  = Context.UsableDotNetZip;
                btnSave.Visible      = true;
                btnSettings.Enabled  = true;
                btnStop.Visible      = false;

                Workers.Failed          -= CompressToFailed;
                Workers.Finished        -= CompressToFinished;
                Workers.UpdateProgress  -= UpdateProgress;
                Workers.UpdateProgress2 -= UpdateProgress2;

                if(thdCompressTo != null)
                {
                    thdCompressTo.Abort();
                    thdCompressTo = null;
                }

                MessageBox.Show($"Correctly compressed as {Context.Path}");

                Context.Path = null;
            });
        }

        protected void OnBtnStopFilesClicked(object sender, EventArgs e)
        {
            if(populatingFiles)
            {
                Workers.Failed         -= LoadFilesFailed;
                Workers.Finished       -= LoadFilesFinished;
                Workers.UpdateProgress -= UpdateFileProgress2;
                Workers.AddFile        -= AddFile;
                Workers.AddFiles       -= AddFiles;

                if(thdPopulateFiles != null)
                {
                    thdPopulateFiles.Abort();
                    thdPopulateFiles = null;
                }

                lstFiles.Clear();
                btnStopFiles.Visible     = false;
                btnPopulateFiles.Visible = true;
            }

            if(scanningFiles)
                if(thdScanFile != null)
                {
                    thdScanFile.Abort();
                    thdScanFile = null;
                }

            AllClamdFinished();
        }

        protected void OnBtnToggleCrackClicked(object sender, EventArgs e)
        {
            if(treeFiles.SelectedItem == null) return;

            DbFile file  = (DbFile)treeFiles.SelectedItem;
            bool   crack = !file.Crack;

            Workers.ToggleCrack(file.Sha256, crack);

            lstFiles.Remove(file);
            file.Crack = crack;
            lstFiles.Add(file);
        }

        protected void OnBtnScanWithClamdClicked(object sender, EventArgs e)
        {
            if(treeFiles.SelectedItem == null) return;

            DbFile file = Workers.GetDBFile(((DbFile)treeFiles.SelectedItem).Sha256);
            outIter     = (DbFile)treeFiles.SelectedItem;

            if(file == null)
            {
                MessageBox.Show("Cannot get file from database", MessageBoxType.Error);
                return;
            }

            treeFiles.Enabled            =  false;
            btnToggleCrack.Enabled       =  false;
            btnScanWithClamd.Enabled     =  false;
            btnCheckInVirusTotal.Enabled =  false;
            prgProgressFiles1.Visible    =  true;
            lblProgressFiles1.Visible    =  true;
            Workers.Failed               += ClamdFailed;
            Workers.ScanFinished         += ClamdFinished;
            Workers.UpdateProgress       += UpdateVirusProgress;

            lblProgressFiles1.Text          = "Scanning file with clamd.";
            prgProgressFiles1.Indeterminate = true;

            thdScanFile = new Thread(() => Workers.ClamScanFileFromRepo(file));
            thdScanFile.Start();
        }

        void ClamdFailed(string text)
        {
            Application.Instance.Invoke(delegate
            {
                treeFiles.Enabled            =  true;
                btnToggleCrack.Enabled       =  true;
                btnScanWithClamd.Enabled     =  true;
                btnCheckInVirusTotal.Enabled =  true;
                prgProgressFiles1.Visible    =  false;
                lblProgressFiles1.Visible    =  false;
                Workers.Failed               -= ClamdFailed;
                Workers.ScanFinished         -= ClamdFinished;
                Workers.UpdateProgress       -= UpdateVirusProgress;
                lblProgressFiles1.Text       =  "";
                if(thdScanFile == null) return;

                thdScanFile.Abort();
                thdScanFile = null;
            });
        }

        void ClamdFinished(DbFile file)
        {
            Application.Instance.Invoke(delegate
            {
                treeFiles.Enabled                   =  true;
                btnToggleCrack.Enabled              =  true;
                btnScanWithClamd.Enabled            =  true;
                btnCheckInVirusTotal.Enabled        =  true;
                Workers.Failed                      -= ClamdFailed;
                Workers.ScanFinished                -= ClamdFinished;
                Workers.UpdateProgress              -= UpdateVirusProgress;
                lblProgressFiles1.Text              =  "";
                prgProgressFiles1.Visible           =  false;
                lblProgressFiles1.Visible           =  false;
                if(thdScanFile != null) thdScanFile =  null;

                if((!outIter.HasVirus.HasValue || outIter.HasVirus.HasValue && !outIter.HasVirus.Value) &&
                   file.HasVirus.HasValue                                   && file.HasVirus.Value) infectedFiles++;

                lstFiles.Remove(outIter);
                AddFile(file);

                lblFileStatus.Text = $"{lstFiles.Count} files ({infectedFiles} infected)";
            });
        }

        protected void OnBtnCheckInVirusTotalClicked(object sender, EventArgs e)
        {
            if(treeFiles.SelectedItem == null) return;

            DbFile file = Workers.GetDBFile(((DbFile)treeFiles.SelectedItem).Sha256);
            outIter     = (DbFile)treeFiles.SelectedItem;

            if(file == null)
            {
                MessageBox.Show("Cannot get file from database", MessageBoxType.Error);
                return;
            }

            treeFiles.Enabled            =  false;
            btnToggleCrack.Enabled       =  false;
            btnScanWithClamd.Enabled     =  false;
            btnCheckInVirusTotal.Enabled =  false;
            prgProgressFiles1.Visible    =  true;
            lblProgressFiles1.Visible    =  true;
            Workers.Failed               += VirusTotalFailed;
            Workers.ScanFinished         += VirusTotalFinished;
            Workers.UpdateProgress       += UpdateVirusProgress;

            lblProgressFiles1.Text          = "Scanning file with VirusTotal.";
            prgProgressFiles1.Indeterminate = true;

            thdScanFile = new Thread(() => Workers.VirusTotalFileFromRepo(file));
            thdScanFile.Start();
        }

        void VirusTotalFailed(string text)
        {
            Application.Instance.Invoke(delegate
            {
                treeFiles.Enabled                   =  true;
                btnToggleCrack.Enabled              =  true;
                btnScanWithClamd.Enabled            =  true;
                btnCheckInVirusTotal.Enabled        =  true;
                prgProgressFiles1.Visible           =  false;
                Workers.Failed                      -= VirusTotalFailed;
                Workers.ScanFinished                -= VirusTotalFinished;
                Workers.UpdateProgress              -= UpdateVirusProgress;
                lblProgressFiles1.Text              =  "";
                if(thdScanFile != null) thdScanFile =  null;
                MessageBox.Show(text, MessageBoxType.Error);
            });
        }

        void VirusTotalFinished(DbFile file)
        {
            Application.Instance.Invoke(delegate
            {
                treeFiles.Enabled                   =  true;
                btnToggleCrack.Enabled              =  true;
                btnScanWithClamd.Enabled            =  true;
                btnCheckInVirusTotal.Enabled        =  true;
                Workers.Failed                      -= VirusTotalFailed;
                Workers.ScanFinished                -= VirusTotalFinished;
                Workers.UpdateProgress              -= UpdateVirusProgress;
                lblProgressFiles1.Text              =  "";
                prgProgressFiles1.Visible           =  false;
                if(thdScanFile != null) thdScanFile =  null;

                if((!outIter.HasVirus.HasValue || outIter.HasVirus.HasValue && !outIter.HasVirus.Value) &&
                   file.HasVirus.HasValue                                   && file.HasVirus.Value) infectedFiles++;

                lstFiles.Remove(outIter);
                AddFile(file);

                lblFileStatus.Text = $"{lstFiles.Count} files ({infectedFiles} infected)";
            });
        }

        void UpdateVirusProgress(string text, string inner, long current, long maximum)
        {
            Application.Instance.Invoke(delegate { lblProgressFiles1.Text = text; });
        }

        protected void OnBtnPopulateFilesClicked(object sender, EventArgs e)
        {
            lstFiles.Clear();
            tabApps.Enabled          = false;
            btnStopFiles.Visible     = true;
            btnPopulateFiles.Visible = false;

            lblProgressFiles1.Text          =  "Loading files from database";
            lblProgressFiles1.Visible       =  true;
            lblProgressFiles2.Visible       =  true;
            prgProgressFiles1.Visible       =  true;
            prgProgressFiles2.Visible       =  true;
            prgProgressFiles1.Indeterminate =  true;
            Workers.Failed                  += LoadFilesFailed;
            Workers.Finished                += LoadFilesFinished;
            Workers.UpdateProgress          += UpdateFileProgress2;
            populatingFiles                 =  true;
            infectedFiles                   =  0;
            thdPopulateFiles                =  new Thread(Workers.GetFilesFromDb);
            thdPopulateFiles.Start();
        }

        void UpdateFileProgress(string text, string inner, long current, long maximum)
        {
            Application.Instance.Invoke(delegate
            {
                if(!string.IsNullOrWhiteSpace(text) && !string.IsNullOrWhiteSpace(inner))
                    lblProgressFiles1.Text = $"{text}: {inner}";
                else if(!string.IsNullOrWhiteSpace(inner))
                    lblProgressFiles1.Text = inner;
                else
                    lblProgressFiles1.Text = text;
                if(maximum > 0)
                {
                    if(current < int.MinValue || current > int.MaxValue || maximum < int.MinValue ||
                       maximum > int.MaxValue)
                    {
                        current /= 100;
                        maximum /= 100;
                    }

                    prgProgressFiles1.Indeterminate = false;
                    prgProgressFiles1.MinValue      = 0;
                    prgProgressFiles1.MaxValue      = (int)maximum;
                    prgProgressFiles1.Value         = (int)current;
                }
                else prgProgressFiles1.Indeterminate = true;
            });
        }

        void UpdateFileProgress2(string text, string inner, long current, long maximum)
        {
            Application.Instance.Invoke(delegate
            {
                if(!string.IsNullOrWhiteSpace(text) && !string.IsNullOrWhiteSpace(inner))
                    lblProgressFiles2.Text = $"{text}: {inner}";
                else if(!string.IsNullOrWhiteSpace(inner))
                    lblProgressFiles2.Text = inner;
                else
                    lblProgressFiles2.Text = text;
                if(maximum > 0)
                {
                    if(current < int.MinValue || current > int.MaxValue || maximum < int.MinValue ||
                       maximum > int.MaxValue)
                    {
                        current /= 100;
                        maximum /= 100;
                    }

                    prgProgressFiles2.Indeterminate = false;
                    prgProgressFiles2.MinValue      = 0;
                    prgProgressFiles2.MaxValue      = (int)maximum;
                    prgProgressFiles2.Value         = (int)current;
                }
                else prgProgressFiles2.Indeterminate = true;
            });
        }

        void AddFile(DbFile file)
        {
            Application.Instance.Invoke(delegate
            {
                if(file.HasVirus.HasValue && file.HasVirus.Value) infectedFiles++;

                lstFiles.Add(file);
            });
        }

        void AddFiles(List<DbFile> files)
        {
            Application.Instance.Invoke(delegate
            {
                List<DbFile> foo = new List<DbFile>();
                foo.AddRange(lstFiles);
                foo.AddRange(files);
                lstFiles = new ObservableCollection<DbFile>(foo);

                foreach(DbFile file in files)
                    if(file.HasVirus.HasValue && file.HasVirus.Value)
                        infectedFiles++;
            });
        }

        void LoadFilesFailed(string text)
        {
            Application.Instance.Invoke(delegate
            {
                MessageBox.Show($"Error {text} when populating files, exiting...", MessageBoxType.Error);
                Workers.Failed         -= LoadFilesFailed;
                Workers.Finished       -= LoadFilesFinished;
                Workers.UpdateProgress -= UpdateFileProgress2;
                if(thdPopulateFiles != null)
                {
                    thdPopulateFiles.Abort();
                    thdPopulateFiles = null;
                }

                tabApps.Enabled = true;
                lstFiles.Clear();
                btnStopFiles.Visible     = false;
                btnPopulateFiles.Visible = true;
                populatingFiles          = false;
            });
        }

        void LoadFilesFinished()
        {
            Application.Instance.Invoke(delegate
            {
                Workers.Failed         -= LoadFilesFailed;
                Workers.Finished       -= LoadFilesFinished;
                Workers.UpdateProgress -= UpdateFileProgress2;
                if(thdPopulateFiles != null)
                {
                    thdPopulateFiles.Abort();
                    thdPopulateFiles = null;
                }

                treeFiles.DataStore          = lstFiles;
                lblProgressFiles1.Visible    = false;
                lblProgressFiles2.Visible    = false;
                prgProgressFiles1.Visible    = false;
                prgProgressFiles2.Visible    = false;
                btnToggleCrack.Visible       = true;
                btnScanWithClamd.Visible     = true;
                btnCheckInVirusTotal.Visible = true;
                btnStopFiles.Visible         = false;
                btnPopulateFiles.Visible     = false;
                populatingFiles              = false;
                treeFiles.Enabled            = true;
                tabApps.Enabled              = true;
                btnScanAllPending.Visible    = true;
                btnCleanFiles.Visible        = true;
                lblFileStatus.Visible        = true;
                lblFileStatus.Text           = $"{lstFiles.Count} files ({infectedFiles} infected)";
            });
        }

        void treeFilesSelectionChanged(object sender, EventArgs e)
        {
            if(treeFiles.SelectedItem == null) return;

            btnToggleCrack.Text = ((DbFile)treeFiles.SelectedItem).Crack ? "Mark as not crack" : "Mark as crack";
        }

        protected void OnBtnScanAllPendingClicked(object sender, EventArgs e)
        {
            treeFiles.Enabled            =  false;
            btnToggleCrack.Enabled       =  false;
            btnScanWithClamd.Enabled     =  false;
            btnCheckInVirusTotal.Enabled =  false;
            lblProgressFiles1.Visible    =  true;
            lblProgressFiles2.Visible    =  true;
            prgProgressFiles1.Visible    =  true;
            prgProgressFiles2.Visible    =  true;
            btnScanAllPending.Enabled    =  false;
            Workers.Finished             += AllClamdFinished;
            Workers.UpdateProgress       += UpdateVirusProgress2;
            Workers.UpdateProgress2      += UpdateFileProgress;
            btnStopFiles.Visible         =  true;
            scanningFiles                =  true;

            lblProgressFiles2.Text          = "Scanning file with clamd.";
            prgProgressFiles2.Indeterminate = true;

            thdScanFile = new Thread(Workers.ClamScanAllFiles);
            thdScanFile.Start();
        }

        void AllClamdFinished()
        {
            Application.Instance.Invoke(delegate
            {
                treeFiles.Enabled                   =  true;
                btnToggleCrack.Enabled              =  true;
                btnScanWithClamd.Enabled            =  true;
                btnCheckInVirusTotal.Enabled        =  true;
                btnScanAllPending.Enabled           =  true;
                Workers.Finished                    -= AllClamdFinished;
                Workers.UpdateProgress              -= UpdateVirusProgress2;
                Workers.UpdateProgress2             -= UpdateFileProgress;
                lblProgressFiles1.Text              =  "";
                prgProgressFiles1.Visible           =  false;
                lblProgressFiles2.Text              =  "";
                prgProgressFiles2.Visible           =  false;
                btnStopFiles.Visible                =  false;
                scanningFiles                       =  false;
                if(thdScanFile != null) thdScanFile =  null;

                OnBtnPopulateFilesClicked(null, new EventArgs());
            });
        }

        void UpdateVirusProgress2(string text, string inner, long current, long maximum)
        {
            Application.Instance.Invoke(delegate { lblProgressFiles2.Text = text; });
        }

        protected void OnBtnCleanFilesClicked(object sender, EventArgs e)
        {
            DialogResult result =
                MessageBox.Show("This option will search the database for any known file that doesn't\n" + "belong to any application and remove it from the database.\n\n" + "It will then search the repository for any file not on the database and remove it.\n\n" + "THIS OPERATION MAY VERY LONG, CANNOT BE CANCELED AND REMOVES DATA ON DISK.\n\n" + "Are you sure to continue?",
                                MessageBoxButtons.YesNo, MessageBoxType.Question);
            if(result != DialogResult.Yes) return;

            btnCleanFiles.Visible        =  false;
            btnToggleCrack.Visible       =  false;
            btnScanWithClamd.Visible     =  false;
            btnScanAllPending.Visible    =  false;
            btnCheckInVirusTotal.Visible =  false;
            tabApps.Enabled              =  false;
            treeFiles.Enabled            =  false;
            lblProgressFiles1.Visible    =  true;
            lblProgressFiles2.Visible    =  true;
            Workers.Finished             += CleanFilesFinished;
            Workers.UpdateProgress       += UpdateFileProgress;
            Workers.UpdateProgress2      += UpdateFileProgress2;
            lblProgressFiles1.Text       =  "";
            prgProgressFiles1.Visible    =  true;
            lblProgressFiles2.Text       =  "";
            prgProgressFiles2.Visible    =  true;
            btnStopFiles.Visible         =  false;
            prgProgress2.Indeterminate   =  true;
            btnSettings.Enabled          =  false;
            mnuCompress.Enabled          =  false;
            btnQuit.Enabled              =  false;
            mnuFile.Enabled              =  false;
            thdCleanFiles                =  new Thread(Workers.CleanFiles);
            thdCleanFiles.Start();
        }

        void CleanFilesFinished()
        {
            Application.Instance.Invoke(delegate
            {
                btnCleanFiles.Visible                   =  true;
                btnToggleCrack.Visible                  =  true;
                btnScanWithClamd.Visible                =  true;
                btnScanAllPending.Visible               =  true;
                btnCheckInVirusTotal.Visible            =  true;
                tabApps.Enabled                         =  true;
                treeFiles.Enabled                       =  true;
                Workers.Finished                        -= CleanFilesFinished;
                Workers.UpdateProgress                  -= UpdateFileProgress;
                Workers.UpdateProgress2                 -= UpdateFileProgress2;
                lblProgressFiles1.Text                  =  "";
                prgProgressFiles1.Visible               =  false;
                lblProgressFiles2.Text                  =  "";
                prgProgressFiles2.Visible               =  false;
                btnSettings.Enabled                     =  true;
                mnuCompress.Enabled                     =  true;
                btnQuit.Enabled                         =  true;
                mnuFile.Enabled                         =  true;
                if(thdCleanFiles != null) thdCleanFiles =  null;

                OnBtnPopulateFilesClicked(null, new EventArgs());
            });
        }

        #region XAML UI elements
        #pragma warning disable 0649
        GridView       treeApps;
        Label          lblProgress;
        ProgressBar    prgProgress;
        Label          lblProgress2;
        ProgressBar    prgProgress2;
        Button         btnAdd;
        Button         btnRemove;
        Button         btnCompress;
        Button         btnSave;
        Button         btnStop;
        ButtonMenuItem btnSettings;
        ButtonMenuItem mnuCompress;
        GridView       treeFiles;
        Label          lblProgressFiles1;
        ProgressBar    prgProgressFiles1;
        Label          lblProgressFiles2;
        ProgressBar    prgProgressFiles2;
        Button         btnStopFiles;
        Button         btnToggleCrack;
        Button         btnScanWithClamd;
        Button         btnCheckInVirusTotal;
        Button         btnPopulateFiles;
        TabPage        tabApps;
        Button         btnScanAllPending;
        Button         btnCleanFiles;
        ButtonMenuItem btnQuit;
        ButtonMenuItem mnuFile;
        Label          lblAppStatus;
        Label          lblFileStatus;
        #pragma warning restore 0649
        #endregion XAML UI elements
    }
}