﻿using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace apprepodbmgr.Core
{
    public static class IO
    {
        public static List<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption,
                                                  bool followLinks = true, bool symlinks = true)
        {
            if(followLinks)
                return new List<string>(Directory.EnumerateFiles(path, searchPattern, searchOption));

            List<string> files       = new List<string>();
            List<string> directories = new List<string>();

            foreach(string file in Directory.EnumerateFiles(path, searchPattern))
            {
                var fi = new FileInfo(file);

                if(fi.Attributes.HasFlag(FileAttributes.ReparsePoint) && symlinks)
                    files.Add(file);
                else if(!fi.Attributes.HasFlag(FileAttributes.ReparsePoint))
                    files.Add(file);
            }

            if(searchOption != SearchOption.AllDirectories)
                return files;

            foreach(string directory in Directory.EnumerateDirectories(path, searchPattern))
            {
                var di = new DirectoryInfo(directory);

                if(!di.Attributes.HasFlag(FileAttributes.ReparsePoint))
                    files.AddRange(EnumerateFiles(directory, searchPattern, searchOption, followLinks, symlinks));
            }

            return files;
        }

        public static List<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption,
                                                        bool followLinks = true, bool symlinks = true)
        {
            if(followLinks)
                return new List<string>(Directory.EnumerateDirectories(path, searchPattern, searchOption));

            List<string> directories = new List<string>();

            if(searchOption != SearchOption.AllDirectories)
                return directories;

            directories.AddRange(from directory in Directory.EnumerateDirectories(path, searchPattern) let di =
                                     new DirectoryInfo(directory)
                                 where !di.Attributes.HasFlag(FileAttributes.ReparsePoint) select directory);

            List<string> newDirectories = new List<string>();

            foreach(string directory in directories)
                newDirectories.AddRange(EnumerateDirectories(directory, searchPattern, searchOption, followLinks,
                                                             symlinks));

            directories.AddRange(newDirectories);

            return directories;
        }

        public static List<string> EnumerateSymlinks(string path, string searchPattern, SearchOption searchOption)
        {
            List<string> directories = new List<string>();

            List<string> links = (from file in Directory.EnumerateFiles(path, searchPattern) let fi = new FileInfo(file)
                                  where fi.Attributes.HasFlag(FileAttributes.ReparsePoint) select file).ToList();

            if(searchOption != SearchOption.AllDirectories)
                return links;

            foreach(string directory in Directory.EnumerateDirectories(path, searchPattern))
            {
                var di = new DirectoryInfo(directory);

                if(!di.Attributes.HasFlag(FileAttributes.ReparsePoint))
                    directories.Add(directory);
                else //if (!links.Contains(directory))
                    links.Add(directory);
            }

            foreach(string directory in directories)
                links.AddRange(EnumerateSymlinks(directory, searchPattern, searchOption));

            return links;
        }
    }
}