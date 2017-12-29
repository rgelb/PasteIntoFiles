﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PasteAsFile
{
    public partial class frmMain : Form
    {
        public string CurrentLocation { get; set; }
        public bool IsText { get; set; }
        public frmMain()
        {
            InitializeComponent();
        }
        public frmMain(string location)
        {
            InitializeComponent();
            this.CurrentLocation = location;
        }
        private void frmMain_Load(object sender, EventArgs e)
        {
            bool skipUI = true; // assume we skip the UI

            if (Registry.GetValue(@"HKEY_CLASSES_ROOT\Directory\Background\shell\Paste As File\command", "", null) == null)
            {
                if (MessageBox.Show("Seems that you are running this application for the first time,\nDo you want to Register it with your system Context Menu ?", "Paste As File", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Program.RegisterApp();
                    skipUI = false;
                }
            }

            // bring up the UI if Shift Key is pressed.
            if (Control.ModifierKeys == Keys.Shift)
            {
                skipUI = false;
            }

            txtFilename.Text = DateTime.Now.ToString("dd-MM-yyyy HH-mm-ss");
            txtCurrentLocation.Text = CurrentLocation ?? @"C:\";

            if (Clipboard.ContainsText())
            {
                lblType.Text = "Text File";
                comExt.SelectedItem = "txt";
                IsText = true;
                txtContent.Text = Clipboard.GetText();
            }
            else if (Clipboard.ContainsImage())
            {
                lblType.Text = "Image";
                comExt.SelectedItem = "png";
                imgContent.Image = Clipboard.GetImage();
            }
            else
            {
                lblType.Text = "Unknown File";
                btnSave.Enabled = false;
                skipUI = false;
            }

            if (skipUI)
            {
                btnSave_Click(null, null);
                Environment.Exit(0);
            }
            else
            {
                // since Shift key was pressed - it's likely that the app opened in background
                this.WindowState = FormWindowState.Minimized;
                this.Show();
                this.WindowState = FormWindowState.Normal;
            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string location = txtCurrentLocation.Text;
            location = location.EndsWith("\\") ? location : location + "\\";
            string filename = txtFilename.Text + "." + comExt.SelectedItem.ToString() ;
            if (IsText)
            {

                File.WriteAllText(location+filename,txtContent.Text,Encoding.UTF8);
                this.Text += " : File Saved :)";
            }
            else
            {
                switch (comExt.SelectedItem.ToString())
                {
                    case "png":
                        imgContent.Image.Save(location + filename, ImageFormat.Png);
                        break;
                    case "ico":
                        imgContent.Image.Save(location + filename, ImageFormat.Icon);
                        break;
                    case "jpg":
                        imgContent.Image.Save(location + filename, ImageFormat.Jpeg);
                        break;
                    case "bmp":
                        imgContent.Image.Save(location + filename, ImageFormat.Bmp);
                        break;
                    case "gif":
                        imgContent.Image.Save(location + filename, ImageFormat.Gif);
                        break;
                    default:
                        imgContent.Image.Save(location + filename, ImageFormat.Png);
                        break;
                }
                
                this.Text += " : Image Saved :)";
            }

            //Task.Factory.StartNew(() =>
            //{
            //    Thread.Sleep(1000);
            //    Environment.Exit(0);
            //});
        }

        private void btnBrowseForFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Select a folder for saving this file ";
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                txtCurrentLocation.Text = fbd.SelectedPath;
            }
        }

        private void lblWebsite_Click(object sender, EventArgs e)
        {
            Process.Start("http://eslamx.com");
        }

        private void lblMe_Click(object sender, EventArgs e)
        {
            Process.Start("http://twitter.com/EslaMx7");
        }

        private void lblHelp_Click(object sender, EventArgs e)
        {
            string msg = "Paste As File helps you paste any text or images in your system clipboard into a file directly instead of creating new file yourslef";
            msg += "\n--------------------\nTo Register the application to your system Context Menu run the program as Administrator with this argument : /reg";
            msg += "\nto Unregister the application use this argument : /unreg\n";
            msg += "\n--------------------\nSend Feedback to : EslaMx7@Gmail.Com\n\nThanks :)";
            MessageBox.Show(msg,"Paste As File Help",MessageBoxButtons.OK,MessageBoxIcon.Information);


           
        }
    }
}
