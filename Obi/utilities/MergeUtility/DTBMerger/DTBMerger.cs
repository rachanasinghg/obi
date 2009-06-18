using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace DTBMerger
    {
    public class DTBMerger
        {
        private String[] m_InputPaths;
        private string m_OutputDirectory;
        private int m_ProgressInfo;
        private PageMergeOptions m_PageMergeOptions;

        public DTBMerger ( string[] inputPaths, string outputDirectory, PageMergeOptions pageOptions )
            {
            m_ProgressInfo = 0;

            if (inputPaths.Length == 0)
                throw new System.Exception ( "No input DTBs" );

            for (int i = 0; i < inputPaths.Length; i++)
                {
                if (!File.Exists ( inputPaths[i] ))
                    {
                    throw new System.IO.FileNotFoundException ( inputPaths[i] + " do not exists" );
                    }
                }
            m_InputPaths = inputPaths;

            if (Directory.Exists ( outputDirectory ))
                {
                Directory.Delete ( outputDirectory, true );
                }
            Directory.CreateDirectory ( outputDirectory );
            m_OutputDirectory = outputDirectory;

            m_PageMergeOptions = pageOptions;
            }


        public int ProgressInfo { get { return m_ProgressInfo; } }

        public void MergeDTDs ()
            {
            m_ProgressInfo = 0;
            List<string> inputParameterList = CopyAllDTDsToOutputDirectory ( true );

            for (int i = 0; i < inputParameterList.Count; i++)
                {
                string prefix = Convert.ToChar ( ((int)'a') + i ).ToString ();
                prefix = prefix + "_";
                //MessageBox.Show ( prefix.ToString () );

                DTBRenamer renamer = new DTBRenamer ( inputParameterList[i], prefix );
                renamer.RenameDTBFilesSet ();
                }

            m_ProgressInfo = 70;
            DTBIntegrator integrator = new DTBIntegrator ( inputParameterList, m_PageMergeOptions );
            integrator.IntegrateDTBs ();

            m_ProgressInfo = 90;
            // delete temporary directories, all directories excluding first directory in list
            for (int i = 1; i < inputParameterList.Count; i++)
                {
                string dirPathToDelete = Directory.GetParent ( inputParameterList[i] ).FullName;
                Directory.Delete ( dirPathToDelete, true );
                }
            m_ProgressInfo = 100;
            }


        private List<string> CopyAllDTDsToOutputDirectory ( bool isDAISY3 )
            {
            List<string> inputParameterList = new List<string> ();
            // copy first DTB to output directory
            string opfPath = CopyDTBFiles (
                Directory.GetParent ( m_InputPaths[0] ).FullName,
                m_OutputDirectory,
                isDAISY3 );

            inputParameterList.Add ( opfPath );

            m_ProgressInfo += (60 / m_InputPaths.Length);
            // copy all remaining DTBs in their folders
            for (int i = 1; i < m_InputPaths.Length; i++)
                {
                string copyToDirectory = Path.Combine ( m_OutputDirectory, i.ToString () );
                Directory.CreateDirectory ( copyToDirectory );

                opfPath = CopyDTBFiles (
                Directory.GetParent ( m_InputPaths[i] ).FullName,
                copyToDirectory,
                isDAISY3 );

                inputParameterList.Add ( opfPath );
                m_ProgressInfo += (60 / m_InputPaths.Length);
                }

            // for debugging purpose
            foreach (string s in inputParameterList)
                {
                if (!File.Exists ( s ))
                    System.Diagnostics.Debug.Fail ( "input parameter filepath is invalid", "problem in copy before starting merge operation" );
                }

            return inputParameterList;

            }

        private string CopyDTBFiles ( string sourceDirectory, string destinationDirectory, bool isDAISY3 )
            {
            string[] sourceFilePaths = Directory.GetFiles ( sourceDirectory, "*.*", SearchOption.TopDirectoryOnly );

            string opfPath = "";

            for (int i = 0; i < sourceFilePaths.Length; i++)
                {
                FileInfo f = new FileInfo ( sourceFilePaths[i] );
                string destinationPath = Path.Combine ( destinationDirectory, f.Name );
                f.CopyTo ( destinationPath );

                if (isDAISY3 && f.Extension == ".opf")
                    {
                    opfPath = destinationPath;
                    }
                else if (!isDAISY3 &&
                    (string.Compare ( f.Name, "ncc.html", true ) == 0 || string.Compare ( f.Name, "ncc.htm", true ) == 0))
                    {
                    opfPath = destinationPath;
                    }
                }
            return opfPath;
            }

        
        public void MergeDAISY2DTDs ()
            {
            m_ProgressInfo = 0;
            List<string> inputParameterList = CopyAllDTDsToOutputDirectory ( false );
            //foreach (string s in inputParameterList)
            //MessageBox.Show ( s );

            for (int i = 0; i < inputParameterList.Count; i++)
                {
                string prefix = Convert.ToChar ( ((int)'a') + i ).ToString ();
                prefix = prefix + "_";
                //MessageBox.Show ( prefix.ToString () );

                DTBRenamer renamer = new DTBRenamer ( inputParameterList[i], prefix );
                renamer.Rename2_02DTBFilesSet ();
                }


            m_ProgressInfo = 70;
            DTBIntegrator integrator = new DTBIntegrator ( inputParameterList, m_PageMergeOptions );
            integrator.IntegrateDAISY2DTBs ();

            m_ProgressInfo = 90;
            // delete temporary directories, all directories excluding first directory in list
            for (int i = 1; i < inputParameterList.Count; i++)
                {
                string dirPathToDelete = Directory.GetParent ( inputParameterList[i] ).FullName;
                Directory.Delete ( dirPathToDelete, true );
                }
            m_ProgressInfo = 100;
            }


        }
    }
