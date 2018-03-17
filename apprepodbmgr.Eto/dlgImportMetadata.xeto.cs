using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using apprepodbmgr.Core;
using Eto.Forms;
using Eto.Serialization.Xaml;
using libexeinfo;
using Schemas;

namespace apprepodbmgr.Eto
{
    public class dlgImportMetadata : Dialog
    {
        List<string>                                 architectures;
        internal bool                                canceled;
        internal List<ArchitecturesTypeArchitecture> chosenArchitectures;
        internal List<TargetOsEntry>                 chosenOses;
        Panels                                       currentPanel;
        internal string                              description;
        internal string                              developer;
        Panels                                       maximumPanel;
        Panels                                       minimumPanel;
        List<TargetOsEntry>                          operatingSystems;
        pnlDescription                               panelDescription;
        pnlStrings                                   panelStrings;
        pnlVersions                                  panelVersions;
        internal string                              product;
        internal string                              publisher;
        List<string>                                 strings;
        internal string                              version;
        List<string>                                 versions;

        public dlgImportMetadata()
        {
            XamlReader.Load(this);

            Topmost             = true;
            canceled            = true;
            AbortButton         = btnCancel;
            DefaultButton       = btnNext;
            DisplayMode         = DialogDisplayMode.Attached;
            panelDescription    = new pnlDescription();
            panelStrings        = new pnlStrings();
            panelVersions       = new pnlVersions();
            strings             = new List<string>();
            architectures       = new List<string>();
            operatingSystems    = new List<TargetOsEntry>();
            versions            = new List<string>();
            chosenArchitectures = new List<ArchitecturesTypeArchitecture>();
            chosenOses          = new List<TargetOsEntry>();

            LoadComplete += OnLoadComplete;

            // TODO: Show relative path only
            panelDescription.treeFiles.DataStore = Context.Readmes;

            //pnlPanel.Content = panelDescription;
            //pnlPanel.Content = panelStrings;
            //pnlPanel.Content = panelVersions;

            minimumPanel = Context.Readmes?.Count > 0 ? Panels.Description : Panels.Strings;
            maximumPanel = Context.Readmes?.Count > 0 ? Panels.Description : Panels.Strings;
        }

        // TODO: Icons
        void OnLoadComplete(object sender, EventArgs eventArgs)
        {
            if(Context.Executables?.Count > 0)
            {
                lblPanelName.Text    = "Please wait while reading executables found";
                prgProgress.MaxValue = Context.Executables.Count;
                prgProgress.MinValue = 0;
                prgProgress.Value    = 0;

                foreach(string file in Context.Executables)
                {
                    FileStream  exeStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                    MZ          mzExe     = new MZ(exeStream);
                    NE          neExe     = new NE(exeStream);
                    AtariST     stExe     = new AtariST(exeStream);
                    LX          lxExe     = new LX(exeStream);
                    COFF        coffExe   = new COFF(exeStream);
                    PE          peExe     = new PE(exeStream);
                    Geos        geosExe   = new Geos(exeStream);
                    ELF         elfExe    = new ELF(exeStream);
                    IExecutable recognizedExe;

                    if(neExe.Recognized) recognizedExe        = neExe;
                    else if(lxExe.Recognized) recognizedExe   = lxExe;
                    else if(peExe.Recognized) recognizedExe   = peExe;
                    else if(mzExe.Recognized) recognizedExe   = mzExe;
                    else if(coffExe.Recognized) recognizedExe = coffExe;
                    else if(stExe.Recognized) recognizedExe   = stExe;
                    else if(elfExe.Recognized) recognizedExe  = elfExe;
                    else if(geosExe.Recognized) recognizedExe = geosExe;
                    else
                    {
                        exeStream.Close();
                        continue;
                    }

                    if(recognizedExe.Strings != null) strings.AddRange(recognizedExe.Strings);

                    foreach(Architecture exeArch in recognizedExe.Architectures)
                    {
                        ArchitecturesTypeArchitecture? arch = ExeArchToSchemaArch(exeArch);
                        if(arch.HasValue && !architectures.Contains($"{arch.Value}"))
                            architectures.Add($"{arch.Value}");
                    }

                    operatingSystems.Add(new TargetOsEntry
                    {
                        name = recognizedExe.RequiredOperatingSystem.Name,
                        version =
                            $"{recognizedExe.RequiredOperatingSystem.MajorVersion}.{recognizedExe.RequiredOperatingSystem.MinorVersion}"
                    });

                    switch(recognizedExe)
                    {
                        case NE _:
                            if(neExe.Versions != null)
                                foreach(NE.Version exeVersion in neExe.Versions)
                                {
                                    versions.Add(exeVersion.FileVersion);
                                    versions.Add(exeVersion.ProductVersion);
                                    version = exeVersion.ProductVersion;
                                    foreach(KeyValuePair<string, Dictionary<string, string>> kvp in exeVersion
                                       .StringsByLanguage)
                                    {
                                        if(kvp.Value.TryGetValue("CompanyName", out string tmpValue))
                                            developer = tmpValue;
                                        if(kvp.Value.TryGetValue("ProductName", out string tmpValue2))
                                            product = tmpValue2;
                                    }
                                }

                            break;
                        case PE _:
                            if(peExe.Versions != null)
                                foreach(PE.Version exeVersion in peExe.Versions)
                                {
                                    versions.Add(exeVersion.FileVersion);
                                    versions.Add(exeVersion.ProductVersion);
                                    version = exeVersion.ProductVersion;
                                    foreach(KeyValuePair<string, Dictionary<string, string>> kvp in exeVersion
                                       .StringsByLanguage)
                                    {
                                        if(kvp.Value.TryGetValue("CompanyName", out string tmpValue))
                                            developer = tmpValue;
                                        if(kvp.Value.TryGetValue("ProductName", out string tmpValue2))
                                            product = tmpValue2;
                                    }
                                }

                            break;
                        case LX _:
                            if(lxExe.WinVersion != null)
                            {
                                versions.Add(lxExe.WinVersion.FileVersion);
                                versions.Add(lxExe.WinVersion.ProductVersion);
                                version = lxExe.WinVersion.ProductVersion;
                                foreach(KeyValuePair<string, Dictionary<string, string>> kvp in lxExe
                                                                                               .WinVersion
                                                                                               .StringsByLanguage)
                                {
                                    if(kvp.Value.TryGetValue("CompanyName", out string tmpValue)) developer = tmpValue;
                                    if(kvp.Value.TryGetValue("ProductName", out string tmpValue2)) product  = tmpValue2;
                                }
                            }

                            break;
                    }

                    exeStream.Close();
                    prgProgress.Value++;
                }

                strings = strings.Distinct().ToList();
                strings.Sort();

                if(strings.Count == 0 && minimumPanel == Panels.Strings) minimumPanel = Panels.Versions;
                else maximumPanel                                                     = Panels.Strings;

                panelStrings.treeStrings.DataStore = strings;
                versions                           = versions.Distinct().ToList();
                versions.Sort();
                panelVersions.treeVersions.DataStore = versions;
                architectures                        = architectures.Distinct().ToList();
                architectures.Sort();
                panelVersions.treeArchs.DataStore = architectures;

                Dictionary<string, List<string>> osEntriesDictionary = new Dictionary<string, List<string>>();

                foreach(TargetOsEntry osEntry in operatingSystems)
                {
                    if(string.IsNullOrEmpty(osEntry.name)) continue;

                    osEntriesDictionary.TryGetValue(osEntry.name, out List<string> osvers);

                    if(osvers == null) osvers = new List<string>();

                    osvers.Add(osEntry.version);
                    osEntriesDictionary.Remove(osEntry.name);
                    osEntriesDictionary.Add(osEntry.name, osvers);
                }

                operatingSystems = new List<TargetOsEntry>();
                foreach(KeyValuePair<string, List<string>> kvp in osEntriesDictionary.OrderBy(t => t.Key))
                {
                    kvp.Value.Sort();
                    foreach(string s in kvp.Value.Distinct())
                        operatingSystems.Add(new TargetOsEntry {name = kvp.Key, version = s});
                }

                panelVersions.treeOs.DataStore = operatingSystems;

                if(versions.Count > 0 || architectures.Count > 0 || operatingSystems.Count > 0)
                    maximumPanel = Panels.Versions;
            }

            prgProgress.Visible = false;
            btnPrevious.Enabled = false;
            switch(minimumPanel)
            {
                case Panels.Description:
                    pnlPanel.Content = panelDescription;
                    currentPanel     = Panels.Description;
                    break;
                case Panels.Strings:
                    pnlPanel.Content = panelStrings;
                    currentPanel     = Panels.Strings;
                    break;
                case Panels.Versions:
                    pnlPanel.Content = panelVersions;
                    currentPanel     = Panels.Versions;
                    break;
            }

            if(currentPanel == maximumPanel) btnNext.Text = "Finish";
            lblPanelName.Visible = false;
        }

        static ArchitecturesTypeArchitecture? ExeArchToSchemaArch(Architecture arch)
        {
            switch(arch)
            {
                case Architecture.Aarch64:   return ArchitecturesTypeArchitecture.aarch64;
                case Architecture.Alpha:     return ArchitecturesTypeArchitecture.axp;
                case Architecture.Amd64:     return ArchitecturesTypeArchitecture.amd64;
                case Architecture.Arm:       return ArchitecturesTypeArchitecture.arm;
                case Architecture.Avr:       return ArchitecturesTypeArchitecture.avr;
                case Architecture.Avr32:     return ArchitecturesTypeArchitecture.avr32;
                case Architecture.Clipper:   return ArchitecturesTypeArchitecture.clipper;
                case Architecture.Hobbit:    return ArchitecturesTypeArchitecture.hobbit;
                case Architecture.I286:      return ArchitecturesTypeArchitecture.i86;
                case Architecture.I386:      return ArchitecturesTypeArchitecture.ia32;
                case Architecture.I8051:     return ArchitecturesTypeArchitecture.Item8051;
                case Architecture.I86:       return ArchitecturesTypeArchitecture.i86;
                case Architecture.I860:      return ArchitecturesTypeArchitecture.i860;
                case Architecture.I960:      return ArchitecturesTypeArchitecture.i960;
                case Architecture.IA64:      return ArchitecturesTypeArchitecture.ia64;
                case Architecture.M68HC05:   return ArchitecturesTypeArchitecture.m6805;
                case Architecture.M68K:      return ArchitecturesTypeArchitecture.m68k;
                case Architecture.M88K:      return ArchitecturesTypeArchitecture.m88k;
                case Architecture.Mips:      return ArchitecturesTypeArchitecture.mips32;
                case Architecture.Mips16:    return ArchitecturesTypeArchitecture.mips32;
                case Architecture.Mips3:     return ArchitecturesTypeArchitecture.mips64;
                case Architecture.NIOS2:     return ArchitecturesTypeArchitecture.nios2;
                case Architecture.OpenRisc:  return ArchitecturesTypeArchitecture.openrisc;
                case Architecture.PaRisc:    return ArchitecturesTypeArchitecture.parisc;
                case Architecture.Pdp10:     return ArchitecturesTypeArchitecture.pdp10;
                case Architecture.Pdp11:     return ArchitecturesTypeArchitecture.pdp11;
                case Architecture.Power:     return ArchitecturesTypeArchitecture.power;
                case Architecture.PowerPc:   return ArchitecturesTypeArchitecture.ppc;
                case Architecture.PowerPc64: return ArchitecturesTypeArchitecture.ppc64;
                case Architecture.Prism:     return ArchitecturesTypeArchitecture.prism;
                case Architecture.RiscV:     return ArchitecturesTypeArchitecture.riscv;
                case Architecture.S370:      return ArchitecturesTypeArchitecture.s360;
                case Architecture.S390:      return ArchitecturesTypeArchitecture.esa390;
                case Architecture.Sh2:       return ArchitecturesTypeArchitecture.sh2;
                case Architecture.Sh3:       return ArchitecturesTypeArchitecture.sh3;
                case Architecture.Sh4:       return ArchitecturesTypeArchitecture.sh4;
                case Architecture.Sh5:       return ArchitecturesTypeArchitecture.sh5;
                case Architecture.Sparc:     return ArchitecturesTypeArchitecture.sparc;
                case Architecture.Sparc64:   return ArchitecturesTypeArchitecture.sparc64;
                case Architecture.Thumb:     return ArchitecturesTypeArchitecture.arm;
                case Architecture.Thumb2:    return ArchitecturesTypeArchitecture.arm;
                case Architecture.Vax:       return ArchitecturesTypeArchitecture.vax;
                case Architecture.We32000:   return ArchitecturesTypeArchitecture.we32000;
                case Architecture.Z80:       return ArchitecturesTypeArchitecture.z80;
                default:                     return null;
            }
        }

        void OnBtnCancelClick(object sender, EventArgs eventArgs)
        {
            canceled = true;
            Close();
            //            throw new NotImplementedException();
        }

        void OnBtnPreviousClick(object sender, EventArgs eventArgs)
        {
            switch(currentPanel)
            {
                case Panels.Description:
                    // Ok...
                    break;
                case Panels.Strings:
                    if(minimumPanel == Panels.Strings) return;

                    pnlPanel.Content = panelDescription;
                    currentPanel     = Panels.Description;

                    btnPrevious.Enabled = currentPanel != minimumPanel;

                    break;
                case Panels.Versions:
                    if(minimumPanel == Panels.Versions) return;

                    pnlPanel.Content = panelStrings;
                    currentPanel     = Panels.Strings;

                    btnPrevious.Enabled = currentPanel != minimumPanel;

                    break;
            }

            if(currentPanel != maximumPanel) btnNext.Text = "Next";
        }

        void OnBtnNextClick(object sender, EventArgs eventArgs)
        {
            if(currentPanel != maximumPanel)
            {
                switch(currentPanel)
                {
                    case Panels.Description:
                        if(maximumPanel == Panels.Description) return;

                        pnlPanel.Content = panelStrings;
                        currentPanel     = Panels.Strings;

                        btnPrevious.Enabled = currentPanel != minimumPanel;

                        break;
                    case Panels.Strings:
                        if(maximumPanel == Panels.Strings) return;

                        pnlPanel.Content = panelVersions;
                        currentPanel     = Panels.Versions;

                        btnPrevious.Enabled = currentPanel != minimumPanel;

                        break;
                    case Panels.Versions:
                        if(minimumPanel == Panels.Versions) return;

                        pnlPanel.Content = panelStrings;
                        currentPanel     = Panels.Strings;

                        btnPrevious.Enabled = currentPanel != minimumPanel;

                        break;
                }

                if(currentPanel == maximumPanel) btnNext.Text = "Finish";
                return;
            }

            if(Context.Readmes?.Count > 0 && !string.IsNullOrWhiteSpace(panelDescription.description))
                description = panelDescription.description;

            if(!(Context.Executables?.Count > 0)) return;

            if(!string.IsNullOrWhiteSpace(panelStrings.txtDeveloper.Text)) developer = panelStrings.txtDeveloper.Text;
            if(!string.IsNullOrWhiteSpace(panelStrings.txtPublisher.Text)) publisher = panelStrings.txtPublisher.Text;
            if(!string.IsNullOrWhiteSpace(panelStrings.txtProduct.Text)) product     = panelStrings.txtProduct.Text;
            if(!string.IsNullOrWhiteSpace(panelStrings.txtVersion.Text)) version     = panelStrings.txtVersion.Text;

            foreach(object archsSelectedItem in panelVersions.treeArchs.SelectedItems)
            {
                if(!(archsSelectedItem is string arch)) continue;

                if(Enum.TryParse(arch, true, out ArchitecturesTypeArchitecture realArch))
                    chosenArchitectures.Add(realArch);
            }

            foreach(object osesSelectedItem in panelVersions.treeOs.SelectedItems)
            {
                if(!(osesSelectedItem is TargetOsEntry os)) continue;

                chosenOses.Add(os);
            }

            if(panelVersions.treeVersions.SelectedItem is string chosenVersion) version = chosenVersion;

            canceled = false;
            Close();
        }

        enum Panels
        {
            Description,
            Strings,
            Versions
        }

        #region XAML UI elements
        #pragma warning disable 0649
        Label       lblPanelName;
        Panel       pnlPanel;
        ProgressBar prgProgress;
        Button      btnCancel;
        Button      btnPrevious;
        Button      btnNext;
        #pragma warning restore 0649
        #endregion XAML UI elements
    }
}