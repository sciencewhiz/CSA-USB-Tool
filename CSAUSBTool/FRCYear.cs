﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DiscUtils.Iso9660;

namespace CSAUSBTool
{
    public class FRCYear
    {
        public int Year;
        public List<ControlSystemsSoftware> Software;

        public FRCYear(int year, List<ControlSystemsSoftware> software)
        {
            Year = year;
            Software = software;
        }

        public void Download(string path, DownloadProgressChangedEventHandler progress, ToolStripLabel textProgress,
            bool async)
        {
            textProgress.Text = @"Downloading Software...";
            foreach (var soft in Software)
            {
                Console.Out.WriteLine(soft.FileName + " is being downloaded...");
                if (File.Exists(path + @"\" + soft.FileName))
                {
                    if (soft.IsValid(path))
                    {
                        Console.Out.WriteLine(soft.FileName + " already exists in target directory, skipping.");
                        continue;
                    }
                    Console.Out.WriteLine("MD5 Hash for " + soft.FileName +
                                          " was not equal to the given hash. Redownloading.");
                }
                soft.Download(path, progress, async);
            }
        }

        public void BuildISO(string sourcepath, string outputPath, ToolStripProgressBar progress)
        {
            CDBuilder builder = new CDBuilder
            {
                UseJoliet = true,
                VolumeIdentifier = "CSA_USB_" + Year
            };

            foreach (var soft in Software)
            {
                progress.ProgressBar.Value += (100 / Software.Count);
                Thread.Sleep(300);
                builder.AddFile(soft.FileName, sourcepath + @"\" + soft.FileName);
            }
            builder.Build(outputPath + @"\" + builder.VolumeIdentifier + ".iso");
            progress.Value = 100;
        }
    }
}