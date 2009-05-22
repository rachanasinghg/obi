using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Windows.Forms;

namespace renamer
    {
    public class DTBFilesInfo
        {
        private string m_OpfPath;
        private string m_NcxPath;
        private string m_BaseDirectory;
        private TimeSpan m_TotalTime;
        private List<string> m_SmilFilesList;
        private List<string> m_SmilPathList;
        private List<string> m_AudioFilesList;
        private List<string> m_AudioFilePathsList;


        public DTBFilesInfo ( string path )
            {
            m_AudioFilesList = new List<string> ();
            m_AudioFilePathsList = new List<string> ();
            m_SmilFilesList = new List<string> ();
            m_SmilPathList = new List<string> ();

            ExtractFileNamesFromOpf ( path );
            }

        public string OpfPath { get { return m_OpfPath; } }

        public string NcxPath { get { return m_NcxPath; } }

        public string BaseDirectory { get { return m_BaseDirectory; } }

        public TimeSpan TotalTime { get { return m_TotalTime; } }

        public List<string> SmilFilesList { get { return m_SmilFilesList; } }

        public List<string> SmilFilePathsList { get { return m_SmilPathList; } }

        public List<string> AudioFilesList { get { return m_AudioFilesList; } }

        public List<string> AudioFilePathsList { get { return m_AudioFilePathsList; } }

        public void ExtractFileNamesFromOpf ( string path   )
            {
            m_OpfPath = path;
            m_BaseDirectory = Directory.GetParent ( m_OpfPath ).FullName;
            // create xml reader and load xml document
            XmlTextReader Reader = new XmlTextReader ( m_OpfPath );
            Reader.XmlResolver = null;

            XmlDocument XmlDoc = new XmlDocument ();
            XmlDoc.XmlResolver = null;
            XmlDoc.Load ( Reader );
            Reader.Close ();
            Reader = null;

            string baseDirectoryPath = Directory.GetParent ( m_OpfPath ).FullName;

            
            // extract total time of DTD
            XmlNodeList metaNodeList = XmlDoc.GetElementsByTagName ( "meta" );
            foreach (XmlNode n in metaNodeList)
                {
                if (n.Attributes.GetNamedItem ( "name" ).Value == "dtb:totalTime")
                    {
                    string timeString = n.Attributes.GetNamedItem ( "content" ).Value;
                    m_TotalTime = TimeSpan.Parse ( timeString );
                    }
                }


            XmlNode manifestNode = XmlDoc.GetElementsByTagName ( "manifest" )[0];

            Dictionary<string, string> smilIdDictionary = new Dictionary<string, string> ();

            XmlNodeList manifestItemList = XmlDoc.GetElementsByTagName ( "item" );

            foreach (XmlNode n in manifestItemList)
                {
                if (n.Attributes.GetNamedItem ( "media-type" ).Value == "application/x-dtbncx+xml")
                    {
                    m_NcxPath = Path.Combine ( baseDirectoryPath,
                        n.Attributes.GetNamedItem ( "href" ).Value );
                    }

                if (n.Attributes.GetNamedItem ( "media-type" ).Value == "application/smil")
                    {

                    smilIdDictionary.Add ( n.Attributes.GetNamedItem ( "id" ).Value,
                        n.Attributes.GetNamedItem ( "href" ).Value );

                    }

                if (n.Attributes.GetNamedItem ( "media-type" ).Value == "audio/mpeg"
                     || n.Attributes.GetNamedItem ( "media-type" ).Value == "audio/x-wav")
                    {
                    m_AudioFilesList.Add ( n.Attributes.GetNamedItem ( "href" ).Value );
                    m_AudioFilePathsList.Add ( Path.Combine ( baseDirectoryPath,
                        n.Attributes.GetNamedItem ( "href" ).Value ) );
                    }

                }

            XmlNode spineNode = XmlDoc.GetElementsByTagName ( "spine" )[0];

            XmlNodeList spineItemRefList = spineNode.ChildNodes;
            foreach (XmlNode n in spineItemRefList)
                {
                if (n.LocalName == "itemref")
                    {
                    string idString = n.Attributes.GetNamedItem ( "idref" ).Value;
                    if (smilIdDictionary.ContainsKey ( idString ))
                        {
                        m_SmilFilesList.Add ( smilIdDictionary[idString] );
                        m_SmilPathList.Add ( Path.Combine ( baseDirectoryPath,
                            smilIdDictionary[idString] ) );
                        }

                    }
                }

            
            //foreach (String s in AudioFilePathsList)
            //MessageBox.Show ( s );
            XmlDoc = null;

            }


        }
    }
