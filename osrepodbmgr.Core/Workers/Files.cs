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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using DiscImageChef.Checksums;
using Newtonsoft.Json;
using Schemas;
using SharpCompress.Compressors.BZip2;
using SharpCompress.Compressors.Deflate;
using SharpCompress.Compressors.LZMA;

namespace osrepodbmgr.Core
{
    public static partial class Workers
    {
        public static void FindFiles()
        {
            string filesPath;

            if(!string.IsNullOrEmpty(Context.tmpFolder) && Directory.Exists(Context.tmpFolder))
                filesPath = Context.tmpFolder;
            else
                filesPath = Context.path;

            if(string.IsNullOrEmpty(filesPath))
            {
                if(Failed != null)
                    Failed("Path is null or empty");
            }

            if(!Directory.Exists(filesPath))
            {
                if(Failed != null)
                    Failed("Directory not found");
            }

            try
            {
                Context.files = new List<string>(Directory.EnumerateFiles(filesPath, "*", SearchOption.AllDirectories));
                Context.files.Sort();
                Context.folders = new List<string>(Directory.EnumerateDirectories(filesPath, "*", SearchOption.AllDirectories));
                Context.folders.Sort();
                if(Finished != null)
                    Finished();
            }
            catch(Exception ex)
            {
                if(Debugger.IsAttached)
                    throw;
                if(Failed != null)
                    Failed(string.Format("Exception {0}\n{1}", ex.Message, ex.InnerException));
            }
        }

        public static void HashFiles()
        {
            try
            {
                Context.hashes = new Dictionary<string, DBOSFile>();
                Context.foldersDict = new Dictionary<string, DBFolder>();
                List<string> alreadyMetadata = new List<string>();
                bool foundMetadata = false;

                // For metadata
                List<ArchitecturesTypeArchitecture> architectures = new List<ArchitecturesTypeArchitecture>();
                List<BarcodeType> barcodes = new List<BarcodeType>();
                List<BlockMediaType> disks = new List<BlockMediaType>();
                List<string> categories = new List<string>();
                List<string> keywords = new List<string>();
                List<LanguagesTypeLanguage> languages = new List<LanguagesTypeLanguage>();
                List<OpticalDiscType> discs = new List<OpticalDiscType>();
                List<string> subcategories = new List<string>();
                List<string> systems = new List<string>();
                bool releaseDateSpecified = false;
                DateTime releaseDate = DateTime.MinValue;
                CICMMetadataTypeReleaseType releaseType = CICMMetadataTypeReleaseType.Retail;
                bool releaseTypeSpecified = false;
                List<string> authors = new List<string>();
                List<string> developers = new List<string>();
                List<string> performers = new List<string>();
                List<string> publishers = new List<string>();
                string metadataName = null;
                string metadataPartNo = null;
                string metadataSerial = null;
                string metadataVersion = null;

                // End for metadata

                long counter = 1;
                foreach(string file in Context.files)
                {
                    // An already known metadata file, skip it
                    if(alreadyMetadata.Contains(file))
                    {
                        counter++;
                        continue;
                    }

                    if(Path.GetExtension(file).ToLowerInvariant() == ".xml")
                    {
                        FileStream xrs = new FileStream(file, FileMode.Open, FileAccess.Read);
                        XmlReader xr = XmlReader.Create(xrs);
                        XmlSerializer xs = new XmlSerializer(typeof(CICMMetadataType));
                        if(xs.CanDeserialize(xr))
                        {
                            CICMMetadataType thisMetadata = (CICMMetadataType)xs.Deserialize(xr);
                            if(thisMetadata.Architectures != null)
                                architectures.AddRange(thisMetadata.Architectures);
                            if(thisMetadata.Barcodes != null)
                                barcodes.AddRange(thisMetadata.Barcodes);
                            if(thisMetadata.BlockMedia != null)
                                disks.AddRange(thisMetadata.BlockMedia);
                            if(thisMetadata.Categories != null)
                                categories.AddRange(thisMetadata.Categories);
                            if(thisMetadata.Keywords != null)
                                keywords.AddRange(thisMetadata.Keywords);
                            if(thisMetadata.Languages != null)
                                languages.AddRange(thisMetadata.Languages);
                            if(thisMetadata.OpticalDisc != null)
                                discs.AddRange(thisMetadata.OpticalDisc);
                            if(thisMetadata.Subcategories != null)
                                subcategories.AddRange(thisMetadata.Subcategories);
                            if(thisMetadata.Systems != null)
                                systems.AddRange(thisMetadata.Systems);
                            if(thisMetadata.Author != null)
                                authors.AddRange(thisMetadata.Author);
                            if(thisMetadata.Developer != null)
                                developers.AddRange(thisMetadata.Developer);
                            if(thisMetadata.Performer != null)
                                performers.AddRange(thisMetadata.Performer);
                            if(thisMetadata.Publisher != null)
                                publishers.AddRange(thisMetadata.Publisher);
                            if(string.IsNullOrWhiteSpace(metadataName) && !string.IsNullOrWhiteSpace(thisMetadata.Name))
                                metadataName = thisMetadata.Name;
                            if(string.IsNullOrWhiteSpace(metadataPartNo) && !string.IsNullOrWhiteSpace(thisMetadata.PartNumber))
                                metadataPartNo = thisMetadata.PartNumber;
                            if(string.IsNullOrWhiteSpace(metadataSerial) && !string.IsNullOrWhiteSpace(thisMetadata.SerialNumber))
                                metadataSerial = thisMetadata.SerialNumber;
                            if(string.IsNullOrWhiteSpace(metadataVersion) && !string.IsNullOrWhiteSpace(thisMetadata.Version))
                                metadataVersion = thisMetadata.Version;
                            if(thisMetadata.ReleaseDateSpecified)
                            {
                                if(thisMetadata.ReleaseDate > releaseDate)
                                {
                                    releaseDateSpecified = true;
                                    releaseDate = thisMetadata.ReleaseDate;
                                }
                            }
                            if(thisMetadata.ReleaseTypeSpecified)
                            {
                                releaseTypeSpecified = true;
                                releaseType = thisMetadata.ReleaseType;
                            }

                            foundMetadata = true;

                            string metadataFileWithoutExtension = Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file));
                            alreadyMetadata.Add(metadataFileWithoutExtension + ".xml");
                            alreadyMetadata.Add(metadataFileWithoutExtension + ".xmL");
                            alreadyMetadata.Add(metadataFileWithoutExtension + ".xMl");
                            alreadyMetadata.Add(metadataFileWithoutExtension + ".xML");
                            alreadyMetadata.Add(metadataFileWithoutExtension + ".Xml");
                            alreadyMetadata.Add(metadataFileWithoutExtension + ".XmL");
                            alreadyMetadata.Add(metadataFileWithoutExtension + ".XMl");
                            alreadyMetadata.Add(metadataFileWithoutExtension + ".XML");
                            alreadyMetadata.Add(metadataFileWithoutExtension + ".json");
                            alreadyMetadata.Add(metadataFileWithoutExtension + ".jsoN");
                            alreadyMetadata.Add(metadataFileWithoutExtension + ".jsOn");
                            alreadyMetadata.Add(metadataFileWithoutExtension + ".jsON");
                            alreadyMetadata.Add(metadataFileWithoutExtension + ".jSon");
                            alreadyMetadata.Add(metadataFileWithoutExtension + ".jSoN");
                            alreadyMetadata.Add(metadataFileWithoutExtension + ".jSOn");
                            alreadyMetadata.Add(metadataFileWithoutExtension + ".jSON");
                            alreadyMetadata.Add(metadataFileWithoutExtension + ".Json");
                            alreadyMetadata.Add(metadataFileWithoutExtension + ".JsoN");
                            alreadyMetadata.Add(metadataFileWithoutExtension + ".JsOn");
                            alreadyMetadata.Add(metadataFileWithoutExtension + ".JsON");
                            alreadyMetadata.Add(metadataFileWithoutExtension + ".JSon");
                            alreadyMetadata.Add(metadataFileWithoutExtension + ".JSoN");
                            alreadyMetadata.Add(metadataFileWithoutExtension + ".JSOn");
                            alreadyMetadata.Add(metadataFileWithoutExtension + ".JSON");

                            xr.Close();
                            xrs.Close();
                            continue;
                        }

                        xr.Close();
                        xrs.Close();
                    }
                    else if(Path.GetExtension(file).ToLowerInvariant() == ".json")
                    {
                        FileStream jrs = new FileStream(file, FileMode.Open, FileAccess.Read);
                        TextReader jr = new StreamReader(jrs);
                        JsonSerializer js = new JsonSerializer();

                        try
                        {
                            CICMMetadataType thisMetadata = (CICMMetadataType)js.Deserialize(jr, typeof(CICMMetadataType));
                            if(thisMetadata.Architectures != null)
                                architectures.AddRange(thisMetadata.Architectures);
                            if(thisMetadata.Barcodes != null)
                                barcodes.AddRange(thisMetadata.Barcodes);
                            if(thisMetadata.BlockMedia != null)
                                disks.AddRange(thisMetadata.BlockMedia);
                            if(thisMetadata.Categories != null)
                                categories.AddRange(thisMetadata.Categories);
                            if(thisMetadata.Keywords != null)
                                keywords.AddRange(thisMetadata.Keywords);
                            if(thisMetadata.Languages != null)
                                languages.AddRange(thisMetadata.Languages);
                            if(thisMetadata.OpticalDisc != null)
                                discs.AddRange(thisMetadata.OpticalDisc);
                            if(thisMetadata.Subcategories != null)
                                subcategories.AddRange(thisMetadata.Subcategories);
                            if(thisMetadata.Systems != null)
                                systems.AddRange(thisMetadata.Systems);
                            if(thisMetadata.Author != null)
                                authors.AddRange(thisMetadata.Author);
                            if(thisMetadata.Developer != null)
                                developers.AddRange(thisMetadata.Developer);
                            if(thisMetadata.Performer != null)
                                performers.AddRange(thisMetadata.Performer);
                            if(thisMetadata.Publisher != null)
                                publishers.AddRange(thisMetadata.Publisher);
                            if(string.IsNullOrWhiteSpace(metadataName) && !string.IsNullOrWhiteSpace(thisMetadata.Name))
                                metadataName = thisMetadata.Name;
                            if(string.IsNullOrWhiteSpace(metadataPartNo) && !string.IsNullOrWhiteSpace(thisMetadata.PartNumber))
                                metadataPartNo = thisMetadata.PartNumber;
                            if(string.IsNullOrWhiteSpace(metadataSerial) && !string.IsNullOrWhiteSpace(thisMetadata.SerialNumber))
                                metadataSerial = thisMetadata.SerialNumber;
                            if(string.IsNullOrWhiteSpace(metadataVersion) && !string.IsNullOrWhiteSpace(thisMetadata.Version))
                                metadataVersion = thisMetadata.Version;
                            if(thisMetadata.ReleaseDateSpecified)
                            {
                                if(thisMetadata.ReleaseDate > releaseDate)
                                {
                                    releaseDateSpecified = true;
                                    releaseDate = thisMetadata.ReleaseDate;
                                }
                            }
                            if(thisMetadata.ReleaseTypeSpecified)
                            {
                                releaseTypeSpecified = true;
                                releaseType = thisMetadata.ReleaseType;
                            }

                            foundMetadata = true;

                            string metadataFileWithoutExtension = Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file));
                            alreadyMetadata.Add(metadataFileWithoutExtension + ".xml");
                            alreadyMetadata.Add(metadataFileWithoutExtension + ".xmL");
                            alreadyMetadata.Add(metadataFileWithoutExtension + ".xMl");
                            alreadyMetadata.Add(metadataFileWithoutExtension + ".xML");
                            alreadyMetadata.Add(metadataFileWithoutExtension + ".Xml");
                            alreadyMetadata.Add(metadataFileWithoutExtension + ".XmL");
                            alreadyMetadata.Add(metadataFileWithoutExtension + ".XMl");
                            alreadyMetadata.Add(metadataFileWithoutExtension + ".XML");
                            alreadyMetadata.Add(metadataFileWithoutExtension + ".json");
                            alreadyMetadata.Add(metadataFileWithoutExtension + ".jsoN");
                            alreadyMetadata.Add(metadataFileWithoutExtension + ".jsOn");
                            alreadyMetadata.Add(metadataFileWithoutExtension + ".jsON");
                            alreadyMetadata.Add(metadataFileWithoutExtension + ".jSon");
                            alreadyMetadata.Add(metadataFileWithoutExtension + ".jSoN");
                            alreadyMetadata.Add(metadataFileWithoutExtension + ".jSOn");
                            alreadyMetadata.Add(metadataFileWithoutExtension + ".jSON");
                            alreadyMetadata.Add(metadataFileWithoutExtension + ".Json");
                            alreadyMetadata.Add(metadataFileWithoutExtension + ".JsoN");
                            alreadyMetadata.Add(metadataFileWithoutExtension + ".JsOn");
                            alreadyMetadata.Add(metadataFileWithoutExtension + ".JsON");
                            alreadyMetadata.Add(metadataFileWithoutExtension + ".JSon");
                            alreadyMetadata.Add(metadataFileWithoutExtension + ".JSoN");
                            alreadyMetadata.Add(metadataFileWithoutExtension + ".JSOn");
                            alreadyMetadata.Add(metadataFileWithoutExtension + ".JSON");

                            jr.Close();
                            jrs.Close();
                            continue;
                        }
                        catch
                        {
                            jr.Close();
                            jrs.Close();
                        }
                    }

                    string filesPath;
                    FileInfo fi = new FileInfo(file);

                    if(!string.IsNullOrEmpty(Context.tmpFolder) && Directory.Exists(Context.tmpFolder))
                        filesPath = Context.tmpFolder;
                    else
                        filesPath = Context.path;

                    string relpath = file.Substring(filesPath.Length + 1);
                    if(UpdateProgress != null)
                        UpdateProgress(string.Format("Hashing file {0} of {1}", counter, Context.files.Count), null, counter, Context.files.Count);
                    FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);

                    byte[] dataBuffer = new byte[bufferSize];
                    SHA256Context sha256Context = new SHA256Context();
                    sha256Context.Init();

                    if(fileStream.Length > bufferSize)
                    {
                        long offset;
                        long remainder = fileStream.Length % bufferSize;
                        for(offset = 0; offset < (fileStream.Length - remainder); offset += (int)bufferSize)
                        {
                            if(UpdateProgress2 != null)
                                UpdateProgress2(string.Format("{0:P}", offset / (double)fileStream.Length), relpath, offset, fileStream.Length);
                            dataBuffer = new byte[bufferSize];
                            fileStream.Read(dataBuffer, 0, (int)bufferSize);
                            sha256Context.Update(dataBuffer);
                        }
                        if(UpdateProgress2 != null)
                            UpdateProgress2(string.Format("{0:P}", offset / (double)fileStream.Length), relpath, offset, fileStream.Length);
                        dataBuffer = new byte[remainder];
                        fileStream.Read(dataBuffer, 0, (int)remainder);
                        sha256Context.Update(dataBuffer);
                    }
                    else
                    {
                        if(UpdateProgress2 != null)
                            UpdateProgress2(string.Format("{0:P}", 0 / (double)fileStream.Length), relpath, 0, fileStream.Length);
                        dataBuffer = new byte[fileStream.Length];
                        fileStream.Read(dataBuffer, 0, (int)fileStream.Length);
                        sha256Context.Update(dataBuffer);
                    }

                    fileStream.Close();
                    string hash = stringify(sha256Context.Final());

                    DBOSFile dbFile = new DBOSFile();
                    dbFile.Attributes = fi.Attributes;
                    dbFile.CreationTimeUtc = fi.CreationTimeUtc;
                    dbFile.LastAccessTimeUtc = fi.LastAccessTimeUtc;
                    dbFile.LastWriteTimeUtc = fi.LastWriteTimeUtc;
                    dbFile.Length = fi.Length;
                    dbFile.Path = relpath;
                    dbFile.Sha256 = hash;

                    // TODO: Add common cracker group names?
                    dbFile.Crack |= (relpath.ToLowerInvariant().Contains("crack") || // Typical crack
                       relpath.ToLowerInvariant().Contains("crack") || // Typical keygen
                       relpath.ToLowerInvariant().Contains("[k]"));

                    Context.hashes.Add(relpath, dbFile);
                    counter++;
                }

                counter = 1;
                foreach(string folder in Context.folders)
                {

                    string filesPath;
                    DirectoryInfo di = new DirectoryInfo(folder);

                    if(!string.IsNullOrEmpty(Context.tmpFolder) && Directory.Exists(Context.tmpFolder))
                        filesPath = Context.tmpFolder;
                    else
                        filesPath = Context.path;

                    string relpath = folder.Substring(filesPath.Length + 1);
                    if(UpdateProgress != null)
                        UpdateProgress(string.Format("Checking folder {0} of {1}", counter, Context.folders.Count), null, counter, Context.folders.Count);

                    DBFolder dbFolder = new DBFolder();
                    dbFolder.Attributes = di.Attributes;
                    dbFolder.CreationTimeUtc = di.CreationTimeUtc;
                    dbFolder.LastAccessTimeUtc = di.LastAccessTimeUtc;
                    dbFolder.LastWriteTimeUtc = di.LastWriteTimeUtc;
                    dbFolder.Path = relpath;

                    Context.foldersDict.Add(relpath, dbFolder);
                    counter++;
                }

                if(foundMetadata)
                {
                    Context.metadata = new CICMMetadataType();
                    if(architectures.Count > 0)
                        Context.metadata.Architectures = architectures.Distinct().ToArray();
                    if(authors.Count > 0)
                        Context.metadata.Author = authors.Distinct().ToArray();
                    // TODO: Check for uniqueness
                    if(barcodes.Count > 0)
                        Context.metadata.Barcodes = barcodes.ToArray();
                    if(disks.Count > 0)
                        Context.metadata.BlockMedia = disks.ToArray();
                    if(categories.Count > 0)
                        Context.metadata.Categories = categories.Distinct().ToArray();
                    if(developers.Count > 0)
                        Context.metadata.Developer = developers.Distinct().ToArray();
                    if(keywords.Count > 0)
                        Context.metadata.Keywords = keywords.Distinct().ToArray();
                    if(languages.Count > 0)
                        Context.metadata.Languages = languages.Distinct().ToArray();
                    Context.metadata.Name = metadataName;
                    if(discs.Count > 0)
                        Context.metadata.OpticalDisc = discs.ToArray();
                    Context.metadata.PartNumber = metadataPartNo;
                    if(performers.Count > 0)
                        Context.metadata.Performer = performers.Distinct().ToArray();
                    if(publishers.Count > 0)
                        Context.metadata.Publisher = publishers.Distinct().ToArray();
                    if(releaseDateSpecified)
                    {
                        Context.metadata.ReleaseDate = releaseDate;
                        Context.metadata.ReleaseDateSpecified = true;
                    }
                    if(releaseTypeSpecified)
                    {
                        Context.metadata.ReleaseType = releaseType;
                        Context.metadata.ReleaseTypeSpecified = true;
                    }
                    Context.metadata.SerialNumber = metadataSerial;
                    if(subcategories.Count > 0)
                        Context.metadata.Subcategories = subcategories.Distinct().ToArray();
                    if(systems.Count > 0)
                        Context.metadata.Systems = systems.Distinct().ToArray();
                    Context.metadata.Version = metadataVersion;

                    foreach(string metadataFile in alreadyMetadata)
                        Context.files.Remove(metadataFile);
                }
                else
                    Context.metadata = null;
                if(Finished != null)
                    Finished();
            }
            catch(Exception ex)
            {
                if(Debugger.IsAttached)
                    throw;
                if(Failed != null)
                    Failed(string.Format("Exception {0}\n{1}", ex.Message, ex.InnerException));
            }
        }

        public static void RemoveTempFolder()
        {
            try
            {
                if(Directory.Exists(Context.tmpFolder))
                {
                    Directory.Delete(Context.tmpFolder, true);
                    if(Finished != null)
                        Finished();
                }
            }
            catch(System.IO.IOException)
            {
                // Could not delete temporary files, do not crash.
                if(Finished != null)
                    Finished();
            }
            catch(Exception ex)
            {
                if(Debugger.IsAttached)
                    throw;
                if(Failed != null)
                    Failed(string.Format("Exception {0}\n{1}", ex.Message, ex.InnerException));
            }
        }

        public static void CopyFile()
        {
            try
            {
                if(!File.Exists(Context.path))
                {
                    if(Failed != null)
                        Failed("Specified file cannot be found");
                    return;
                }

                if(string.IsNullOrWhiteSpace(Context.tmpFolder))
                {
                    if(Failed != null)
                        Failed("Destination cannot be empty");
                    return;
                }

                if(Directory.Exists(Context.tmpFolder))
                {
                    if(Failed != null)
                        Failed("Destination cannot be a folder");
                    return;
                }

                FileStream inFs = new FileStream(Context.path, FileMode.Open, FileAccess.Read);
                FileStream outFs = new FileStream(Context.tmpFolder, FileMode.Create, FileAccess.Write);

                byte[] buffer = new byte[bufferSize];

                while((inFs.Position + bufferSize) <= inFs.Length)
                {
                    if(UpdateProgress != null)
                        UpdateProgress("Copying file...", string.Format("{0} / {1} bytes", inFs.Position, inFs.Length), inFs.Position, inFs.Length);

                    inFs.Read(buffer, 0, buffer.Length);
                    outFs.Write(buffer, 0, buffer.Length);
                }

                buffer = new byte[inFs.Length - inFs.Position];
                if(UpdateProgress != null)
                    UpdateProgress("Copying file...", string.Format("{0} / {1} bytes", inFs.Position, inFs.Length), inFs.Position, inFs.Length);

                inFs.Read(buffer, 0, buffer.Length);
                outFs.Write(buffer, 0, buffer.Length);

                inFs.Close();
                outFs.Close();

                if(Finished != null)
                    Finished();
            }
            catch(Exception ex)
            {
                if(Debugger.IsAttached)
                    throw;
                if(Failed != null)
                    Failed(string.Format("Exception {0}\n{1}", ex.Message, ex.InnerException));
            }
        }

        public static void SaveAs()
        {
            try
            {
                if(string.IsNullOrWhiteSpace(Context.path))
                {
                    if(Failed != null)
                        Failed("Destination cannot be empty");
                    return;
                }

                if(File.Exists(Context.path))
                {
                    if(Failed != null)
                        Failed("Destination cannot be a file");
                    return;
                }

                if(Context.dbInfo.id == 0)
                {
                    if(Failed != null)
                        Failed("Operating system must be set");
                    return;
                }

                List<DBOSFile> files;
                List<DBFolder> folders;
                long counter;

                if(UpdateProgress != null)
                    UpdateProgress("", "Asking DB for files...", 1, 100);

                dbCore.DBOps.GetAllFilesInOS(out files, Context.dbInfo.id);

                if(UpdateProgress != null)
                    UpdateProgress("", "Asking DB for folders...", 2, 100);

                dbCore.DBOps.GetAllFolders(out folders, Context.dbInfo.id);

                if(UpdateProgress != null)
                    UpdateProgress("", "Creating folders...", 3, 100);

                counter = 0;
                foreach(DBFolder folder in folders)
                {
                    if(UpdateProgress2 != null)
                        UpdateProgress2("", folder.Path, counter, folders.Count);

                    DirectoryInfo di = Directory.CreateDirectory(Path.Combine(Context.path, folder.Path));
                    di.Attributes = folder.Attributes;
                    di.CreationTimeUtc = folder.CreationTimeUtc;
                    di.LastAccessTimeUtc = folder.LastAccessTimeUtc;
                    di.LastWriteTimeUtc = folder.LastWriteTimeUtc;

                    counter++;
                }

                counter = 3;
                foreach(DBOSFile file in files)
                {
                    if(UpdateProgress != null)
                        UpdateProgress("", string.Format("Creating {0}...", file.Path), counter, 3 + files.Count);

                    Stream zStream = null;
                    string repoPath;
                    AlgoEnum algorithm;

                    if(File.Exists(Path.Combine(Settings.Current.RepositoryPath, file.Sha256[0].ToString(),
                                                file.Sha256[1].ToString(), file.Sha256[2].ToString(),
                                                file.Sha256[3].ToString(), file.Sha256[4].ToString(),
                                                file.Sha256 + ".gz")))
                    {
                        repoPath = Path.Combine(Settings.Current.RepositoryPath, file.Sha256[0].ToString(),
                                                file.Sha256[1].ToString(), file.Sha256[2].ToString(),
                                                file.Sha256[3].ToString(), file.Sha256[4].ToString(),
                                                file.Sha256 + ".gz");
                        algorithm = AlgoEnum.GZip;
                    }
                    else if(File.Exists(Path.Combine(Settings.Current.RepositoryPath, file.Sha256[0].ToString(),
                                                file.Sha256[1].ToString(), file.Sha256[2].ToString(),
                                                file.Sha256[3].ToString(), file.Sha256[4].ToString(),
                                                file.Sha256 + ".bz2")))
                    {
                        repoPath = Path.Combine(Settings.Current.RepositoryPath, file.Sha256[0].ToString(),
                                                file.Sha256[1].ToString(), file.Sha256[2].ToString(),
                                                file.Sha256[3].ToString(), file.Sha256[4].ToString(),
                                                file.Sha256 + ".bz2");
                        algorithm = AlgoEnum.BZip2;
                    }
                    else if(File.Exists(Path.Combine(Settings.Current.RepositoryPath, file.Sha256[0].ToString(),
                                                file.Sha256[1].ToString(), file.Sha256[2].ToString(),
                                                file.Sha256[3].ToString(), file.Sha256[4].ToString(),
                                                file.Sha256 + ".lzma")))
                    {
                        repoPath = Path.Combine(Settings.Current.RepositoryPath, file.Sha256[0].ToString(),
                                                file.Sha256[1].ToString(), file.Sha256[2].ToString(),
                                                file.Sha256[3].ToString(), file.Sha256[4].ToString(),
                                                file.Sha256 + ".lzma");
                        algorithm = AlgoEnum.LZMA;
                    }
                    else
                    {
                        if(Failed != null)
                            Failed(string.Format("Cannot find file with hash {0} in the repository", file.Sha256));
                        return;
                    }

                    FileStream inFs = new FileStream(repoPath, FileMode.Open, FileAccess.Read);
                    FileStream outFs = new FileStream(Path.Combine(Context.path, file.Path), FileMode.CreateNew, FileAccess.Write);

                    switch(algorithm)
                    {
                        case AlgoEnum.GZip:
                            zStream = new GZipStream(inFs, SharpCompress.Compressors.CompressionMode.Decompress);
                            break;
                        case AlgoEnum.BZip2:
                            zStream = new BZip2Stream(inFs, SharpCompress.Compressors.CompressionMode.Decompress);
                            break;
                        case AlgoEnum.LZMA:
                            byte[] properties = new byte[5];
                            inFs.Read(properties, 0, 5);
                            inFs.Seek(8, SeekOrigin.Current);
                            zStream = new LzmaStream(properties, inFs);
                            break;
                    }

                    byte[] buffer = new byte[bufferSize];

                    while((outFs.Position + bufferSize) <= file.Length)
                    {
                        if(UpdateProgress2 != null)
                            UpdateProgress2(string.Format("{0:P}", outFs.Position / (double)file.Length),
                                            string.Format("{0} / {1} bytes", outFs.Position, file.Length),
                                            outFs.Position, file.Length);

                        zStream.Read(buffer, 0, buffer.Length);
                        outFs.Write(buffer, 0, buffer.Length);
                    }

                    buffer = new byte[file.Length - outFs.Position];
                    if(UpdateProgress2 != null)
                        UpdateProgress2(string.Format("{0:P}", outFs.Position / (double)file.Length),
                                        string.Format("{0} / {1} bytes", outFs.Position, file.Length),
                                        outFs.Position, file.Length);

                    zStream.Read(buffer, 0, buffer.Length);
                    outFs.Write(buffer, 0, buffer.Length);

                    if(UpdateProgress2 != null)
                        UpdateProgress2(string.Format("{0:P}", file.Length / (double)file.Length),
                                        "Finishing...", inFs.Length, inFs.Length);

                    zStream.Close();
                    outFs.Close();

                    FileInfo fi = new FileInfo(Path.Combine(Context.path, file.Path));
                    fi.Attributes = file.Attributes;
                    fi.CreationTimeUtc = file.CreationTimeUtc;
                    fi.LastAccessTimeUtc = file.LastAccessTimeUtc;
                    fi.LastWriteTimeUtc = file.LastWriteTimeUtc;

                    counter++;
                }

                if(Finished != null)
                    Finished();
            }
            catch(Exception ex)
            {
                if(Debugger.IsAttached)
                    throw;
                if(Failed != null)
                    Failed(string.Format("Exception {0}\n{1}", ex.Message, ex.InnerException));
            }
        }
    }
}
