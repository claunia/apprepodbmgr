//
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
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Serialization;
using apprepodbmgr.Core;
using Eto.Drawing;
using Eto.Forms;
using Eto.Serialization.Xaml;
using Newtonsoft.Json;
using Schemas;

namespace apprepodbmgr.Eto
{
    public class dlgAdd : Dialog
    {
        public delegate void OnAddedAppDelegate(DbEntry app);

        readonly ObservableCollection<DBEntryForEto> appView;

        readonly ObservableCollection<FileEntry> fileView;
        int                                      knownFiles;
        bool                                     stopped;
        Thread                                   thdAddFiles;
        Thread                                   thdCheckFiles;
        Thread                                   thdExtractArchive;
        Thread                                   thdFindFiles;
        Thread                                   thdHashFiles;
        Thread                                   thdOpenArchive;
        Thread                                   thdPackFiles;
        Thread                                   thdRemoveTemp;

        public dlgAdd()
        {
            XamlReader.Load(this);

            Context.UnarChangeStatus += UnarChangeStatus;
            Context.CheckUnar();

            fileView = new ObservableCollection<FileEntry>();

            treeFiles.DataStore = fileView;

            treeFiles.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell
                {
                    Binding = Binding.Property<FileEntry, string>(r => r.Path)
                },
                HeaderText = "Path"
            });

            treeFiles.Columns.Add(new GridColumn
            {
                DataCell = new CheckBoxCell
                {
                    Binding = Binding.Property<FileEntry, bool?>(r => r.IsCrack)
                },
                HeaderText = "Crack?"
            });

            treeFiles.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell
                {
                    Binding = Binding.Property<FileEntry, string>(r => r.Hash)
                },
                HeaderText = "SHA256"
            });

            treeFiles.Columns.Add(new GridColumn
            {
                DataCell = new CheckBoxCell
                {
                    Binding = Binding.Property<FileEntry, bool?>(r => r.Known)
                },
                HeaderText = "Known?"
            });

            treeFiles.AllowMultipleSelection = false;

            treeFiles.CellFormatting += (sender, e) =>
            {
                e.BackgroundColor = ((FileEntry)e.Item).Known ? Colors.Green : Colors.Red;
            };

            appView = new ObservableCollection<DBEntryForEto>();

            treeApps.DataStore = appView;

            treeApps.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell
                {
                    Binding = Binding.Property<DBEntryForEto, string>(r => r.developer)
                },
                HeaderText = "Developer"
            });

            treeApps.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell
                {
                    Binding = Binding.Property<DBEntryForEto, string>(r => r.product)
                },
                HeaderText = "Product"
            });

            treeApps.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell
                {
                    Binding = Binding.Property<DBEntryForEto, string>(r => r.version)
                },
                HeaderText = "Version"
            });

            treeApps.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell
                {
                    Binding = Binding.Property<DBEntryForEto, string>(r => r.languages)
                },
                HeaderText = "Languages"
            });

            treeApps.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell
                {
                    Binding = Binding.Property<DBEntryForEto, string>(r => r.architecture)
                },
                HeaderText = "Architecture"
            });

            treeApps.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell
                {
                    Binding = Binding.Property<DBEntryForEto, string>(r => r.targetos)
                },
                HeaderText = "Target OS"
            });

            treeApps.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell
                {
                    Binding = Binding.Property<DBEntryForEto, string>(r => r.format)
                },
                HeaderText = "Format"
            });

            treeApps.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell
                {
                    Binding = Binding.Property<DBEntryForEto, string>(r => r.description)
                },
                HeaderText = "Description"
            });

            treeApps.Columns.Add(new GridColumn
            {
                DataCell = new CheckBoxCell
                {
                    Binding = Binding.Property<DBEntryForEto, bool?>(r => r.oem)
                },
                HeaderText = "OEM?"
            });

            treeApps.Columns.Add(new GridColumn
            {
                DataCell = new CheckBoxCell
                {
                    Binding = Binding.Property<DBEntryForEto, bool?>(r => r.upgrade)
                },
                HeaderText = "Upgrade?"
            });

            treeApps.Columns.Add(new GridColumn
            {
                DataCell = new CheckBoxCell
                {
                    Binding = Binding.Property<DBEntryForEto, bool?>(r => r.update)
                },
                HeaderText = "Update?"
            });

            treeApps.Columns.Add(new GridColumn
            {
                DataCell = new CheckBoxCell
                {
                    Binding = Binding.Property<DBEntryForEto, bool?>(r => r.source)
                },
                HeaderText = "Source?"
            });

            treeApps.Columns.Add(new GridColumn
            {
                DataCell = new CheckBoxCell
                {
                    Binding = Binding.Property<DBEntryForEto, bool?>(r => r.files)
                },
                HeaderText = "Files?"
            });

            treeApps.Columns.Add(new GridColumn
            {
                DataCell = new CheckBoxCell
                {
                    Binding = Binding.Property<DBEntryForEto, bool?>(r => r.Installer)
                },
                HeaderText = "Installer?"
            });

            treeApps.AllowMultipleSelection = false;

            txtArchitecture.ToolTip =
                "This field contains a comma separated list of architectures the application can run on. To edit its contents use the metadata editor.";

            txtDescription.ToolTip = "This field contains a free-form text description of this application.";

            txtDeveloper.ToolTip =
                "This field contains the developer of the application. To edit its contents use the metadata editor.";

            txtFormat.ToolTip =
                "This field is contains the name of the format of the disk images, when it is not a byte-by-byte format like .iso or .img.";

            txtLanguages.ToolTip =
                "This field contains a comma separated list of languages the application includes. To edit its contents use the metadata editor.";

            txtProduct.ToolTip =
                "This field contains the application name. To edit its contents use the metadata editor.";

            txtTargetOs.ToolTip =
                "This field contains a comma separated list of operating systems this application can run on. To edit its contents use the metadata editor.";

            txtVersion.ToolTip =
                "This field contains the application version. To edit its contents use the metadata editor.";

            chkFiles.ToolTip = "If this field is checked it indicates the application is already installed.";

            chkInstaller.ToolTip =
                "If this field is checked it indicates the application comes as an installer (one or several files), but it's not installed neither disk images.";

            chkOem.ToolTip =
                "If this field is checked it indicates the application came bundled with hardware (aka OEM distributed).";

            chkSource.ToolTip = "If this field is checked it indicates this is the source code for the application.";

            chkUpdate.ToolTip =
                "If this field is checked it indicates this version is a minor version update that requires a previous version of the application already installed.";

            chkUpgrade.ToolTip =
                "If this field is checked it indicates this version is a major version upgrade that requires a previous version of the application already installed.";

            txtArchitecture.ReadOnly = true;
            txtDeveloper.ReadOnly    = true;
            txtLanguages.ReadOnly    = true;
            txtProduct.ReadOnly      = true;
            txtTargetOs.ReadOnly     = true;
            txtVersion.ReadOnly      = true;
        }

        public event OnAddedAppDelegate OnAddedApp;

        void UnarChangeStatus() => Application.Instance.Invoke(delegate
        {
            btnArchive.Enabled = Context.UnarUsable;
        });

        protected void OnDeleteEvent(object sender, CancelEventArgs e)
        {
            if(btnStop.Visible)
                btnStop.PerformClick();

            if(btnClose.Enabled)
                btnClose.PerformClick();
        }

        protected void OnBtnFolderClicked(object sender, EventArgs e)
        {
            var dlgFolder = new SelectFolderDialog
            {
                Title = "Open folder"
            };

            if(dlgFolder.ShowDialog(this) != DialogResult.Ok)
                return;

            knownFiles          =  0;
            stopped             =  false;
            lblProgress.Text    =  "Finding files";
            lblProgress.Visible =  true;
            prgProgress.Visible =  true;
            btnExit.Enabled     =  false;
            btnFolder.Visible   =  false;
            btnArchive.Visible  =  false;
            thdFindFiles        =  new Thread(Workers.FindFiles);
            Context.Path        =  dlgFolder.Directory;
            Workers.Failed      += FindFilesFailed;
            Workers.Finished    += FindFilesFinished;
            btnStop.Visible     =  true;
            thdFindFiles.Start();
        }

        void FindFilesFailed(string text) => Application.Instance.Invoke(delegate
        {
            if(!stopped)
                MessageBox.Show(text, MessageBoxType.Error);

            lblProgress.Visible =  false;
            prgProgress.Visible =  false;
            btnExit.Enabled     =  true;
            btnFolder.Visible   =  true;
            btnArchive.Visible  =  true;
            btnStop.Visible     =  false;
            Workers.Failed      -= FindFilesFailed;
            Workers.Finished    -= FindFilesFinished;
            thdFindFiles        =  null;
        });

        void FindFilesFinished() => Application.Instance.Invoke(delegate
        {
            Workers.Failed   -= FindFilesFailed;
            Workers.Finished -= FindFilesFinished;

            lblProgress.Visible  = true;
            prgProgress.Visible  = true;
            lblProgress2.Visible = true;
            prgProgress2.Visible = true;

            thdFindFiles            =  null;
            thdHashFiles            =  new Thread(Workers.HashFiles);
            Workers.Failed          += HashFilesFailed;
            Workers.Finished        += HashFilesFinished;
            Workers.UpdateProgress  += UpdateProgress;
            Workers.UpdateProgress2 += UpdateProgress2;
            thdHashFiles.Start();
        });

        void HashFilesFailed(string text) => Application.Instance.Invoke(delegate
        {
            if(!stopped)
                MessageBox.Show(text, MessageBoxType.Error);

            lblProgress.Visible     =  false;
            prgProgress.Visible     =  false;
            lblProgress2.Visible    =  false;
            prgProgress2.Visible    =  false;
            Workers.Failed          -= HashFilesFailed;
            Workers.Finished        -= HashFilesFinished;
            Workers.UpdateProgress  -= UpdateProgress;
            Workers.UpdateProgress2 -= UpdateProgress2;
            btnExit.Enabled         =  true;
            btnFolder.Visible       =  true;
            btnArchive.Visible      =  true;
            btnStop.Visible         =  false;
            thdHashFiles            =  null;
        });

        void HashFilesFinished() => Application.Instance.Invoke(delegate
        {
            lblProgress.Visible     =  false;
            prgProgress.Visible     =  false;
            lblProgress2.Visible    =  false;
            prgProgress2.Visible    =  false;
            Workers.Failed          -= HashFilesFailed;
            Workers.Finished        -= HashFilesFinished;
            Workers.UpdateProgress  -= UpdateProgress;
            Workers.UpdateProgress2 -= UpdateProgress2;
            thdHashFiles            =  null;

            lblProgress.Visible = true;
            prgProgress.Visible = true;

            thdCheckFiles           =  new Thread(Workers.CheckDbForFiles);
            Workers.Failed          += ChkFilesFailed;
            Workers.Finished        += ChkFilesFinished;
            Workers.UpdateProgress  += UpdateProgress;
            Workers.UpdateProgress2 += UpdateProgress2;
            Workers.AddFileForApp   += AddFile;
            Workers.AddApp          += AddApp;
            thdCheckFiles.Start();
        });

        void ChkFilesFailed(string text) => Application.Instance.Invoke(delegate
        {
            if(!stopped)
                MessageBox.Show(text, MessageBoxType.Error);

            prgProgress.Visible     =  false;
            btnStop.Visible         =  false;
            btnClose.Visible        =  false;
            btnExit.Enabled         =  true;
            Workers.Failed          -= ChkFilesFailed;
            Workers.Finished        -= ChkFilesFinished;
            Workers.UpdateProgress  -= UpdateProgress;
            Workers.UpdateProgress2 -= UpdateProgress2;
            Workers.AddFileForApp   -= AddFile;
            Workers.AddApp          -= AddApp;
            thdCheckFiles?.Abort();
            thdHashFiles = null;
            fileView?.Clear();

            if(appView == null)
                return;

            tabApps.Visible = false;
            appView.Clear();
        });

        void ChkFilesFinished() => Application.Instance.Invoke(delegate
        {
            Workers.Failed          -= ChkFilesFailed;
            Workers.Finished        -= ChkFilesFinished;
            Workers.UpdateProgress  -= UpdateProgress;
            Workers.UpdateProgress2 -= UpdateProgress2;
            Workers.AddFileForApp   -= AddFile;
            Workers.AddApp          -= AddApp;

            thdCheckFiles?.Abort();

            thdHashFiles        = null;
            lblProgress.Visible = false;
            prgProgress.Visible = false;
            btnStop.Visible     = false;

            if(Context.Executables?.Count > 0 ||
               Context.Readmes?.Count     > 0)
            {
                var importMetadataDlg = new dlgImportMetadata();
                importMetadataDlg.ShowModal(this);

                if(!importMetadataDlg.canceled &&
                   (importMetadataDlg.chosenArchitectures.Count > 0 || importMetadataDlg.chosenOses.Count > 0 ||
                    !string.IsNullOrWhiteSpace(importMetadataDlg.description) ||
                    !string.IsNullOrWhiteSpace(importMetadataDlg.developer) ||
                    !string.IsNullOrWhiteSpace(importMetadataDlg.product) ||
                    !string.IsNullOrWhiteSpace(importMetadataDlg.publisher) ||
                    !string.IsNullOrWhiteSpace(importMetadataDlg.version)))
                {
                    if(Context.Metadata == null &&
                       (importMetadataDlg.chosenArchitectures.Count > 0 || importMetadataDlg.chosenOses.Count > 0 ||
                        !string.IsNullOrWhiteSpace(importMetadataDlg.developer) ||
                        !string.IsNullOrWhiteSpace(importMetadataDlg.product) ||
                        !string.IsNullOrWhiteSpace(importMetadataDlg.publisher) ||
                        !string.IsNullOrWhiteSpace(importMetadataDlg.version)))
                        Context.Metadata = new CICMMetadataType();

                    if(!string.IsNullOrWhiteSpace(importMetadataDlg.description))
                        txtDescription.Text = importMetadataDlg.description;

                    if(!string.IsNullOrWhiteSpace(importMetadataDlg.product))
                        Context.Metadata.Name = importMetadataDlg.product;

                    if(!string.IsNullOrWhiteSpace(importMetadataDlg.publisher))
                        Context.Metadata.Publisher = new[]
                        {
                            importMetadataDlg.publisher
                        };

                    if(!string.IsNullOrWhiteSpace(importMetadataDlg.developer))
                        Context.Metadata.Developer = new[]
                        {
                            importMetadataDlg.developer
                        };

                    if(importMetadataDlg.chosenArchitectures.Count > 0)
                        Context.Metadata.Architectures = importMetadataDlg.chosenArchitectures.ToArray();

                    if(importMetadataDlg.chosenOses.Count > 0)
                    {
                        List<RequiredOperatingSystemType> reqOs = new List<RequiredOperatingSystemType>();

                        foreach(TargetOsEntry osEntry in importMetadataDlg.chosenOses)
                            reqOs.Add(new RequiredOperatingSystemType
                            {
                                Name = osEntry.name,
                                Version = new[]
                                {
                                    osEntry.version
                                }
                            });

                        Context.Metadata.RequiredOperatingSystems = reqOs.ToArray();
                    }
                }
            }

            btnClose.Visible       = true;
            btnExit.Enabled        = true;
            btnPack.Visible        = true;
            btnPack.Enabled        = true;
            btnRemoveFile.Visible  = true;
            btnToggleCrack.Visible = true;
            btnRemoveFile.Enabled  = true;
            btnToggleCrack.Enabled = true;

            txtFormat.ReadOnly      = false;
            txtDescription.ReadOnly = false;
            chkOem.Enabled          = true;
            chkFiles.Enabled        = true;
            chkUpdate.Enabled       = true;
            chkUpgrade.Enabled      = true;
            chkInstaller.Enabled    = true;
            chkSource.Enabled       = true;

            btnMetadata.Visible = true;

            if(Context.Metadata != null)
            {
                if(Context.Metadata.Developer != null)
                    foreach(string developer in Context.Metadata.Developer)
                    {
                        if(!string.IsNullOrWhiteSpace(txtDeveloper.Text))
                            txtDeveloper.Text += ",";

                        txtDeveloper.Text += developer;
                    }

                if(!string.IsNullOrWhiteSpace(Context.Metadata.Name))
                    txtProduct.Text = Context.Metadata.Name;

                if(!string.IsNullOrWhiteSpace(Context.Metadata.Version))
                    txtVersion.Text = Context.Metadata.Version;

                if(Context.Metadata.Languages != null)
                    foreach(LanguagesTypeLanguage language in Context.Metadata.Languages)
                    {
                        if(!string.IsNullOrWhiteSpace(txtLanguages.Text))
                            txtLanguages.Text += ",";

                        txtLanguages.Text += language;
                    }

                if(Context.Metadata.Architectures != null)
                    foreach(ArchitecturesTypeArchitecture architecture in Context.Metadata.Architectures)
                    {
                        if(!string.IsNullOrWhiteSpace(txtArchitecture.Text))
                            txtArchitecture.Text += ",";

                        txtArchitecture.Text += architecture;
                    }

                if(Context.Metadata.RequiredOperatingSystems != null)
                    foreach(string targetos in Context.Metadata.RequiredOperatingSystems.Select(os => os.Name).
                                                       Distinct())
                    {
                        if(!string.IsNullOrWhiteSpace(txtTargetOs.Text))
                            txtTargetOs.Text += ",";

                        txtTargetOs.Text += targetos;
                    }

                btnMetadata.BackgroundColor = Colors.Green;
            }
            else
                btnMetadata.BackgroundColor = Colors.Red;

            lblStatus.Visible = true;
            lblStatus.Text    = $"{fileView.Count} files ({knownFiles} already known)";
        });

        void AddFile(string filename, string hash, bool known, bool isCrack) => Application.Instance.Invoke(delegate
        {
            fileView.Add(new FileEntry
            {
                Path    = filename,
                Hash    = hash,
                Known   = known,
                IsCrack = isCrack
            });

            btnPack.Enabled |= !known;

            if(known)
                knownFiles++;
        });

        void AddApp(DbEntry app) => Application.Instance.Invoke(delegate
        {
            tabApps.Visible = true;
            appView.Add(new DBEntryForEto(app));
        });

        protected void OnBtnExitClicked(object sender, EventArgs e)
        {
            if(btnClose.Enabled)
                OnBtnCloseClicked(sender, e);

            Close();
        }

        protected void OnBtnCloseClicked(object sender, EventArgs e)
        {
            btnFolder.Visible      = true;
            btnArchive.Visible     = true;
            Context.Path           = "";
            Context.Files          = null;
            Context.Hashes         = null;
            Context.Executables    = null;
            Context.Readmes        = null;
            btnStop.Visible        = false;
            btnPack.Visible        = false;
            btnClose.Visible       = false;
            btnRemoveFile.Visible  = false;
            btnToggleCrack.Visible = false;
            fileView?.Clear();

            if(appView != null)
            {
                tabApps.Visible = false;
                appView.Clear();
            }

            txtFormat.ReadOnly      = true;
            txtDescription.ReadOnly = true;
            chkOem.Enabled          = false;
            chkFiles.Enabled        = false;
            chkUpdate.Enabled       = false;
            chkUpgrade.Enabled      = false;
            chkInstaller.Enabled    = false;
            chkSource.Enabled       = false;
            txtFormat.Text          = "";
            txtTargetOs.Text        = "";
            txtProduct.Text         = "";
            txtVersion.Text         = "";
            txtLanguages.Text       = "";
            txtDeveloper.Text       = "";
            txtDescription.Text     = "";
            txtArchitecture.Text    = "";
            chkOem.Checked          = false;
            chkFiles.Checked        = false;
            chkUpdate.Checked       = false;
            chkUpgrade.Checked      = false;
            chkInstaller.Checked    = false;
            chkSource.Checked       = false;

            if(Context.TmpFolder != null)
            {
                btnStop.Visible           =  false;
                prgProgress.Visible       =  true;
                lblProgress.Visible       =  true;
                lblProgress.Text          =  "Removing temporary files";
                prgProgress.Indeterminate =  true;
                Workers.Failed            += RemoveTempFilesFailed;
                Workers.Finished          += RemoveTempFilesFinished;
                thdRemoveTemp             =  new Thread(Workers.RemoveTempFolder);
                thdRemoveTemp.Start();
            }

            btnMetadata.Visible = false;
            Context.Metadata    = null;
            lblStatus.Visible   = false;
        }

        void UpdateProgress(string text, string inner, long current, long maximum) =>
            Application.Instance.Invoke(delegate
            {
                if(!string.IsNullOrWhiteSpace(text) &&
                   !string.IsNullOrWhiteSpace(inner))
                    lblProgress.Text = $"{text}: {inner}";
                else if(!string.IsNullOrWhiteSpace(inner))
                    lblProgress.Text = inner;
                else
                    lblProgress.Text = text;

                if(maximum > 0)
                {
                    if(current < int.MinValue ||
                       current > int.MaxValue ||
                       maximum < int.MinValue ||
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
                else
                    prgProgress.Indeterminate = true;
            });

        void UpdateProgress2(string text, string inner, long current, long maximum) =>
            Application.Instance.Invoke(delegate
            {
                if(!string.IsNullOrWhiteSpace(text) &&
                   !string.IsNullOrWhiteSpace(inner))
                    lblProgress2.Text = $"{text}: {inner}";
                else if(!string.IsNullOrWhiteSpace(inner))
                    lblProgress2.Text = inner;
                else
                    lblProgress2.Text = text;

                if(maximum > 0)
                {
                    if(current < int.MinValue ||
                       current > int.MaxValue ||
                       maximum < int.MinValue ||
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
                else
                    prgProgress2.Indeterminate = true;
            });

        protected void OnBtnStopClicked(object sender, EventArgs e)
        {
            stopped = true;

            Workers.AddFileForApp    -= AddFile;
            Workers.AddApp           -= AddApp;
            Workers.Failed           -= AddFilesToDbFailed;
            Workers.Failed           -= ChkFilesFailed;
            Workers.Failed           -= ExtractArchiveFailed;
            Workers.Failed           -= FindFilesFailed;
            Workers.Failed           -= HashFilesFailed;
            Workers.Failed           -= OpenArchiveFailed;
            Workers.Failed           -= PackFilesFailed;
            Workers.Failed           -= RemoveTempFilesFailed;
            Workers.Finished         -= AddFilesToDbFinished;
            Workers.Finished         -= ChkFilesFinished;
            Workers.Finished         -= ExtractArchiveFinished;
            Workers.Finished         -= FindFilesFinished;
            Workers.Finished         -= HashFilesFinished;
            Workers.Finished         -= OpenArchiveFinished;
            Workers.Finished         -= RemoveTempFilesFinished;
            Workers.FinishedWithText -= PackFilesFinished;
            Workers.UpdateProgress   -= UpdateProgress;
            Workers.UpdateProgress2  -= UpdateProgress2;

            if(thdFindFiles != null)
            {
                thdFindFiles.Abort();
                thdFindFiles = null;
            }

            if(thdHashFiles != null)
            {
                thdHashFiles.Abort();
                thdHashFiles = null;
            }

            if(thdCheckFiles != null)
            {
                thdCheckFiles.Abort();
                thdCheckFiles = null;
            }

            if(thdAddFiles != null)
            {
                thdAddFiles.Abort();
                thdAddFiles = null;
            }

            if(thdPackFiles != null)
            {
                thdPackFiles.Abort();
                thdPackFiles = null;
            }

            if(thdOpenArchive != null)
            {
                thdOpenArchive.Abort();
                thdOpenArchive = null;
            }

            if(Context.UnarProcess != null)
            {
                Context.UnarProcess.Kill();
                Context.UnarProcess = null;
            }

            if(Context.TmpFolder != null)
            {
                btnStop.Visible           =  false;
                lblProgress.Text          =  "Removing temporary files";
                prgProgress.Indeterminate =  true;
                Workers.Failed            += RemoveTempFilesFailed;
                Workers.Finished          += RemoveTempFilesFinished;
                thdRemoveTemp             =  new Thread(Workers.RemoveTempFolder);
                thdRemoveTemp.Start();
            }
            else
                RestoreUi();
        }

        void RestoreUi()
        {
            lblProgress.Visible      =  false;
            prgProgress.Visible      =  false;
            lblProgress2.Visible     =  false;
            prgProgress2.Visible     =  false;
            btnExit.Enabled          =  true;
            btnFolder.Visible        =  true;
            btnArchive.Visible       =  true;
            lblProgress.Visible      =  false;
            prgProgress.Visible      =  false;
            btnExit.Enabled          =  true;
            btnFolder.Visible        =  true;
            btnArchive.Visible       =  true;
            Workers.Failed           -= FindFilesFailed;
            Workers.Failed           -= HashFilesFailed;
            Workers.Failed           -= ChkFilesFailed;
            Workers.Failed           -= OpenArchiveFailed;
            Workers.Failed           -= AddFilesToDbFailed;
            Workers.Failed           -= PackFilesFailed;
            Workers.Failed           -= ExtractArchiveFailed;
            Workers.Failed           -= RemoveTempFilesFailed;
            Workers.Finished         -= FindFilesFinished;
            Workers.Finished         -= HashFilesFinished;
            Workers.Finished         -= ChkFilesFinished;
            Workers.Finished         -= OpenArchiveFinished;
            Workers.Finished         -= AddFilesToDbFinished;
            Workers.Finished         -= ExtractArchiveFinished;
            Workers.Finished         -= RemoveTempFilesFinished;
            Workers.FinishedWithText -= PackFilesFinished;
            Workers.UpdateProgress   -= UpdateProgress;
            Workers.UpdateProgress2  -= UpdateProgress2;
            btnStop.Visible          =  false;
            fileView?.Clear();

            if(appView == null)
                return;

            tabApps.Visible = false;
            appView.Clear();
        }

        void RemoveTempFilesFailed(string text) => Application.Instance.Invoke(delegate
        {
            MessageBox.Show(text, MessageBoxType.Error);
            Workers.Failed    -= RemoveTempFilesFailed;
            Workers.Finished  -= RemoveTempFilesFinished;
            Context.Path      =  null;
            Context.TmpFolder =  null;
            RestoreUi();
        });

        void RemoveTempFilesFinished() => Application.Instance.Invoke(delegate
        {
            Workers.Failed    -= RemoveTempFilesFailed;
            Workers.Finished  -= RemoveTempFilesFinished;
            Context.Path      =  null;
            Context.TmpFolder =  null;
            RestoreUi();
        });

        void AddToDatabase()
        {
            btnRemoveFile.Enabled   = false;
            btnToggleCrack.Enabled  = false;
            btnPack.Enabled         = false;
            btnClose.Enabled        = false;
            prgProgress.Visible     = true;
            txtFormat.ReadOnly      = true;
            txtDescription.ReadOnly = true;
            chkOem.Enabled          = false;
            chkFiles.Enabled        = false;
            chkUpdate.Enabled       = false;
            chkUpgrade.Enabled      = false;
            chkInstaller.Enabled    = false;
            chkSource.Enabled       = false;

            Workers.UpdateProgress += UpdateProgress;
            Workers.Finished       += AddFilesToDbFinished;
            Workers.Failed         += AddFilesToDbFailed;

            Context.DbInfo.Architecture = txtArchitecture.Text;
            Context.DbInfo.Description  = txtDescription.Text;
            Context.DbInfo.Developer    = txtDeveloper.Text;
            Context.DbInfo.Format       = txtFormat.Text;
            Context.DbInfo.Languages    = txtLanguages.Text;
            Context.DbInfo.TargetOs     = txtTargetOs.Text;
            Context.DbInfo.Product      = txtProduct.Text;
            Context.DbInfo.Version      = txtVersion.Text;
            Context.DbInfo.Files        = chkFiles.Checked.Value;
            Context.DbInfo.Installer    = chkInstaller.Checked.Value;
            Context.DbInfo.Oem          = chkOem.Checked.Value;
            Context.DbInfo.Source       = chkSource.Checked.Value;
            Context.DbInfo.Update       = chkUpdate.Checked.Value;
            Context.DbInfo.Upgrade      = chkUpgrade.Checked.Value;

            if(Context.Metadata != null)
            {
                var ms = new MemoryStream();
                var xs = new XmlSerializer(typeof(CICMMetadataType));
                xs.Serialize(ms, Context.Metadata);
                Context.DbInfo.Xml = ms.ToArray();
                var js = new JsonSerializer();
                ms = new MemoryStream();
                var sw = new StreamWriter(ms);
                js.Serialize(sw, Context.Metadata, typeof(CICMMetadataType));
                Context.DbInfo.Json = ms.ToArray();
            }
            else
            {
                Context.DbInfo.Xml  = null;
                Context.DbInfo.Json = null;
            }

            thdAddFiles = new Thread(Workers.AddFilesToDb);
            thdAddFiles.Start();
        }

        void AddFilesToDbFinished() => Application.Instance.Invoke(delegate
        {
            Workers.UpdateProgress -= UpdateProgress;
            Workers.Finished       -= AddFilesToDbFinished;
            Workers.Failed         -= AddFilesToDbFailed;

            thdAddFiles?.Abort();

            long counter = 0;
            fileView.Clear();

            foreach(KeyValuePair<string, DbAppFile> kvp in Context.Hashes)
            {
                UpdateProgress(null, "Updating table", counter, Context.Hashes.Count);

                fileView.Add(new FileEntry
                {
                    Path  = kvp.Key,
                    Hash  = kvp.Value.Sha256,
                    Known = true
                });

                counter++;
            }

            // TODO: Update application table

            OnAddedApp?.Invoke(Context.DbInfo);

            lblProgress.Visible = false;
            prgProgress.Visible = false;
            btnClose.Enabled    = true;
        });

        void AddFilesToDbFailed(string text) => Application.Instance.Invoke(delegate
        {
            if(!stopped)
                MessageBox.Show(text, MessageBoxType.Error);

            Workers.UpdateProgress -= UpdateProgress;
            Workers.Finished       -= AddFilesToDbFinished;
            Workers.Failed         -= AddFilesToDbFailed;

            thdAddFiles?.Abort();

            ChkFilesFinished();
        });

        protected void OnBtnPackClicked(object sender, EventArgs e)
        {
            btnRemoveFile.Enabled   = false;
            btnToggleCrack.Enabled  = false;
            btnPack.Enabled         = false;
            btnClose.Enabled        = false;
            prgProgress.Visible     = true;
            prgProgress2.Visible    = true;
            lblProgress.Visible     = true;
            lblProgress2.Visible    = true;
            txtFormat.ReadOnly      = true;
            txtDescription.ReadOnly = true;
            chkOem.Enabled          = false;
            chkFiles.Enabled        = false;
            chkUpdate.Enabled       = false;
            chkUpgrade.Enabled      = false;
            chkInstaller.Enabled    = false;
            chkSource.Enabled       = false;

            Workers.UpdateProgress   += UpdateProgress;
            Workers.UpdateProgress2  += UpdateProgress2;
            Workers.FinishedWithText += PackFilesFinished;
            Workers.Failed           += PackFilesFailed;

            Context.DbInfo.Architecture = txtArchitecture.Text;
            Context.DbInfo.Description  = txtDescription.Text;
            Context.DbInfo.Developer    = txtDeveloper.Text;
            Context.DbInfo.Format       = txtFormat.Text;
            Context.DbInfo.Languages    = txtLanguages.Text;
            Context.DbInfo.TargetOs     = txtTargetOs.Text;
            Context.DbInfo.Product      = txtProduct.Text;
            Context.DbInfo.Version      = txtVersion.Text;
            Context.DbInfo.Files        = chkFiles.Checked.Value;
            Context.DbInfo.Installer    = chkInstaller.Checked.Value;
            Context.DbInfo.Oem          = chkOem.Checked.Value;
            Context.DbInfo.Source       = chkSource.Checked.Value;
            Context.DbInfo.Update       = chkUpdate.Checked.Value;
            Context.DbInfo.Upgrade      = chkUpgrade.Checked.Value;

            thdPackFiles = new Thread(Workers.CompressFiles);
            thdPackFiles.Start();
        }

        void PackFilesFinished(string text) => Application.Instance.Invoke(delegate
        {
            Workers.UpdateProgress   -= UpdateProgress;
            Workers.UpdateProgress2  -= UpdateProgress2;
            Workers.FinishedWithText -= PackFilesFinished;
            Workers.Failed           -= PackFilesFailed;
            prgProgress2.Visible     =  false;
            lblProgress2.Visible     =  false;

            thdPackFiles?.Abort();

            AddToDatabase();

            MessageBox.Show(text);
        });

        void PackFilesFailed(string text) => Application.Instance.Invoke(delegate
        {
            if(!stopped)
                MessageBox.Show(text, MessageBoxType.Error);

            Workers.UpdateProgress   -= UpdateProgress;
            Workers.UpdateProgress2  -= UpdateProgress2;
            Workers.FinishedWithText -= PackFilesFinished;
            Workers.Failed           -= PackFilesFailed;

            thdPackFiles?.Abort();

            btnRemoveFile.Enabled   = true;
            btnToggleCrack.Enabled  = true;
            btnPack.Enabled         = true;
            btnClose.Enabled        = true;
            prgProgress.Visible     = false;
            prgProgress2.Visible    = false;
            lblProgress.Visible     = false;
            lblProgress2.Visible    = false;
            txtFormat.ReadOnly      = false;
            txtDescription.ReadOnly = false;
            chkOem.Enabled          = true;
            chkFiles.Enabled        = true;
            chkUpdate.Enabled       = true;
            chkUpgrade.Enabled      = true;
            chkInstaller.Enabled    = true;
            chkSource.Enabled       = true;
        });

        protected void OnBtnArchiveClicked(object sender, EventArgs e)
        {
            if(!Context.UnarUsable)
            {
                MessageBox.Show("Cannot open archives without a working unar installation.", MessageBoxType.Error);

                return;
            }

            var dlgFile = new OpenFileDialog
            {
                Title       = "Open archive",
                MultiSelect = false
            };

            if(dlgFile.ShowDialog(this) != DialogResult.Ok)
                return;

            knownFiles                = 0;
            stopped                   = false;
            lblProgress.Text          = "Opening archive";
            lblProgress.Visible       = false;
            prgProgress.Visible       = true;
            btnExit.Enabled           = false;
            btnFolder.Visible         = false;
            btnArchive.Visible        = false;
            prgProgress.Indeterminate = true;

            thdOpenArchive   =  new Thread(Workers.OpenArchive);
            Context.Path     =  dlgFile.FileName;
            Workers.Failed   += OpenArchiveFailed;
            Workers.Finished += OpenArchiveFinished;
            btnStop.Visible  =  true;
            thdOpenArchive.Start();
        }

        void OpenArchiveFailed(string text) => Application.Instance.Invoke(delegate
        {
            if(!stopped)
                MessageBox.Show(text, MessageBoxType.Error);

            lblProgress.Visible =  false;
            prgProgress.Visible =  false;
            btnExit.Enabled     =  true;
            btnFolder.Visible   =  true;
            btnArchive.Visible  =  true;
            btnStop.Visible     =  false;
            Workers.Failed      -= OpenArchiveFailed;
            Workers.Finished    -= OpenArchiveFinished;
            thdOpenArchive      =  null;
        });

        void OpenArchiveFinished() => Application.Instance.Invoke(delegate
        {
            stopped                 =  false;
            lblProgress.Text        =  "Extracting archive";
            prgProgress.Visible     =  true;
            prgProgress2.Visible    =  true;
            btnExit.Enabled         =  false;
            btnFolder.Visible       =  false;
            btnArchive.Visible      =  false;
            Workers.UpdateProgress  += UpdateProgress;
            lblProgress.Visible     =  true;
            lblProgress2.Visible    =  true;
            Workers.Failed          -= OpenArchiveFailed;
            Workers.Finished        -= OpenArchiveFinished;
            thdOpenArchive          =  null;
            Workers.Failed          += ExtractArchiveFailed;
            Workers.Finished        += ExtractArchiveFinished;
            Workers.UpdateProgress2 += UpdateProgress2;
            thdExtractArchive       =  new Thread(Workers.ExtractArchive);
            thdExtractArchive.Start();
        });

        void ExtractArchiveFailed(string text) => Application.Instance.Invoke(delegate
        {
            if(!stopped)
                MessageBox.Show(text, MessageBoxType.Error);

            lblProgress2.Visible    =  false;
            prgProgress2.Visible    =  false;
            btnExit.Enabled         =  true;
            btnFolder.Visible       =  true;
            btnArchive.Visible      =  true;
            Workers.Failed          -= ExtractArchiveFailed;
            Workers.Finished        -= ExtractArchiveFinished;
            Workers.UpdateProgress  -= UpdateProgress;
            Workers.UpdateProgress2 -= UpdateProgress2;
            thdExtractArchive       =  null;

            if(Context.TmpFolder == null)
                return;

            btnStop.Visible           =  false;
            lblProgress.Text          =  "Removing temporary files";
            prgProgress.Indeterminate =  true;
            Workers.Failed            += RemoveTempFilesFailed;
            Workers.Finished          += RemoveTempFilesFinished;
            thdRemoveTemp             =  new Thread(Workers.RemoveTempFolder);
            thdRemoveTemp.Start();
        });

        void ExtractArchiveFinished() => Application.Instance.Invoke(delegate
        {
            stopped                   =  false;
            lblProgress.Text          =  "Finding files";
            lblProgress.Visible       =  true;
            lblProgress2.Visible      =  false;
            prgProgress.Visible       =  true;
            btnExit.Enabled           =  false;
            btnFolder.Visible         =  false;
            btnArchive.Visible        =  false;
            Workers.Failed            -= ExtractArchiveFailed;
            Workers.Finished          -= ExtractArchiveFinished;
            Workers.UpdateProgress    -= UpdateProgress;
            Workers.UpdateProgress2   -= UpdateProgress2;
            prgProgress.Indeterminate =  true;

            thdExtractArchive =  null;
            thdFindFiles      =  new Thread(Workers.FindFiles);
            Workers.Failed    += FindFilesFailed;
            Workers.Finished  += FindFilesFinished;
            btnStop.Visible   =  true;
            thdFindFiles.Start();
        });

        protected void OnBtnMetadataClicked(object sender, EventArgs e)
        {
            var _dlgMetadata = new dlgMetadata
            {
                Metadata = Context.Metadata
            };

            _dlgMetadata.FillFields();

            _dlgMetadata.ShowModal(this);

            if(!_dlgMetadata.Modified)
                return;

            Context.Metadata = _dlgMetadata.Metadata;

            if(string.IsNullOrWhiteSpace(txtDeveloper.Text))
                if(Context.Metadata.Developer != null)
                    foreach(string developer in Context.Metadata.Developer)
                    {
                        if(!string.IsNullOrWhiteSpace(txtDeveloper.Text))
                            txtDeveloper.Text += ",";

                        txtDeveloper.Text += developer;
                    }

            if(string.IsNullOrWhiteSpace(txtProduct.Text))
                if(!string.IsNullOrWhiteSpace(Context.Metadata.Name))
                    txtProduct.Text = Context.Metadata.Name;

            if(string.IsNullOrWhiteSpace(txtVersion.Text))
                if(!string.IsNullOrWhiteSpace(Context.Metadata.Version))
                    txtVersion.Text = Context.Metadata.Version;

            if(string.IsNullOrWhiteSpace(txtLanguages.Text))
                if(Context.Metadata.Languages != null)
                    foreach(LanguagesTypeLanguage language in Context.Metadata.Languages)
                    {
                        if(!string.IsNullOrWhiteSpace(txtLanguages.Text))
                            txtLanguages.Text += ",";

                        txtLanguages.Text += language;
                    }

            if(string.IsNullOrWhiteSpace(txtArchitecture.Text))
                if(Context.Metadata.Architectures != null)
                    foreach(ArchitecturesTypeArchitecture architecture in Context.Metadata.Architectures)
                    {
                        if(!string.IsNullOrWhiteSpace(txtArchitecture.Text))
                            txtArchitecture.Text += ",";

                        txtArchitecture.Text += architecture;
                    }

            if(string.IsNullOrWhiteSpace(txtTargetOs.Text))
                if(Context.Metadata.RequiredOperatingSystems != null)
                    foreach(string targetos in Context.Metadata.RequiredOperatingSystems.Select(os => os.Name).
                                                       Distinct())
                    {
                        if(!string.IsNullOrWhiteSpace(txtTargetOs.Text))
                            txtTargetOs.Text += ",";

                        txtTargetOs.Text += targetos;
                    }

            btnMetadata.BackgroundColor = Colors.Green;
        }

        protected void OnBtnRemoveFileClicked(object sender, EventArgs e)
        {
            if(treeFiles.SelectedItem == null)
                return;

            string name = ((FileEntry)treeFiles.SelectedItem).Path;
            string filesPath;

            if(!string.IsNullOrEmpty(Context.TmpFolder) &&
               Directory.Exists(Context.TmpFolder))
                filesPath = Context.TmpFolder;
            else
                filesPath = Context.Path;

            Context.Hashes.Remove(name);
            Context.Files.Remove(Path.Combine(filesPath, name));
            fileView.Remove((FileEntry)treeFiles.SelectedItem);
        }

        protected void OnBtnToggleCrackClicked(object sender, EventArgs e)
        {
            if(treeFiles.SelectedItem == null)
                return;

            string name  = ((FileEntry)treeFiles.SelectedItem).Path;
            bool   known = ((FileEntry)treeFiles.SelectedItem).Known;

            if(!Context.Hashes.TryGetValue(name, out DbAppFile appFile))
                return;

            appFile.Crack = !appFile.Crack;
            Context.Hashes.Remove(name);
            Context.Hashes.Add(name, appFile);
            ((FileEntry)treeFiles.SelectedItem).IsCrack = appFile.Crack;
            fileView.Remove((FileEntry)treeFiles.SelectedItem);

            fileView.Add(new FileEntry
            {
                Path    = name,
                Hash    = appFile.Sha256,
                Known   = known,
                IsCrack = appFile.Crack
            });
        }

        void treeFilesSelectionChanged(object sender, EventArgs e)
        {
            if(treeFiles.SelectedItem == null)
                return;

            btnToggleCrack.Text = ((FileEntry)treeFiles.SelectedItem).IsCrack ? "Mark as not crack" : "Mark as crack";
        }

        class FileEntry
        {
            public string Path    { get; set; }
            public string Hash    { get; set; }
            public bool   Known   { get; set; }
            public bool   IsCrack { get; set; }
        }

        #region XAML UI elements
        #pragma warning disable 0649
        TextBox     txtDeveloper;
        TextBox     txtProduct;
        TextBox     txtVersion;
        TextBox     txtLanguages;
        TextBox     txtArchitecture;
        TextBox     txtTargetOs;
        TextBox     txtFormat;
        TextArea    txtDescription;
        CheckBox    chkOem;
        CheckBox    chkUpdate;
        CheckBox    chkUpgrade;
        CheckBox    chkFiles;
        CheckBox    chkSource;
        CheckBox    chkInstaller;
        GridView    treeFiles;
        TabPage     tabApps;
        GridView    treeApps;
        Label       lblProgress;
        ProgressBar prgProgress;
        Label       lblProgress2;
        ProgressBar prgProgress2;
        Button      btnRemoveFile;
        Button      btnMetadata;
        Button      btnStop;
        Button      btnFolder;
        Button      btnArchive;
        Button      btnPack;
        Button      btnClose;
        Button      btnExit;
        Button      btnToggleCrack;
        Label       lblStatus;
        #pragma warning restore 0649
        #endregion XAML UI elements
    }
}