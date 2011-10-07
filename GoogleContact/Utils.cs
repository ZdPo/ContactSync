using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Drawing;


using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;
using System.Collections;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace GoogleContact
{
    /// <summary>
    /// Some help methods
    /// </summary>
    public static class Utils
    {

        #region MD5
        /// <summary>
        /// Calculate MD5 HASH from source string
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string CountMD5(string source)
        {
            MD5 md5 = MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(source);
            byte[] hash = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        private static byte[] _salt = Encoding.ASCII.GetBytes("268dyUta31aZ");
        private const string sharedSecret = "9U6240am3f1x87d";
        /// <summary>
        /// Encrypt the given string using AES.  The string can be decrypted using 
        /// DecryptStringAES().  The sharedSecret parameters must match.
        /// </summary>
        /// <param name="plainText">The text to encrypt.</param>
        /// <param name="sharedSecret">A password used to generate a key for encryption.</param>
        public static string EncryptString(string plainText)
        {
            string outStr = null;                       // Encrypted string to return
            RijndaelManaged aesAlg = null;              // RijndaelManaged object used to encrypt the data.

            try
            {
                // generate the key from the shared secret and the salt
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, _salt);

                // Create a RijndaelManaged object
                // with the specified key and IV.
                aesAlg = new RijndaelManaged();
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {

                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                    }
                    outStr = Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
            finally
            {
                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            // Return the encrypted bytes from the memory stream.
            return outStr;
        }

        /// <summary>
        /// Decrypt the given string.  Assumes the string was encrypted using 
        /// EncryptStringAES(), using an identical sharedSecret.
        /// </summary>
        /// <param name="cipherText">The text to decrypt.</param>
        /// <param name="sharedSecret">A password used to generate a key for decryption.</param>
        public static string DecryptString(string cipherText)
        {
            // Declare the RijndaelManaged object
            // used to decrypt the data.
            RijndaelManaged aesAlg = null;

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            try
            {
                // generate the key from the shared secret and the salt
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, _salt);

                // Create a RijndaelManaged object
                // with the specified key and IV.
                aesAlg = new RijndaelManaged();
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                // Create the streams used for decryption.                
                byte[] bytes = Convert.FromBase64String(cipherText);
                using (MemoryStream msDecrypt = new MemoryStream(bytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
            finally
            {
                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            return plaintext;
        }
        #endregion

        #region Images
        /// <summary>
        /// Create name for Contact picture from Outlook
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        public static string CreateContactPictureName(Outlook.ContactItem contact)
        {
            return Path.Combine(Path.GetDirectoryName(PathToTempPicture()), string.Format(Constants.FormatImageCacheOutlook, contact.EntryID));
        }
        /// <summary>
        /// Create name for Contact picture from Google
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        public static string CreateContactPictureName(Google.Contacts.Contact contact)
        {
            Uri ur = new Uri(contact.Id);
            return Path.Combine(Path.GetDirectoryName(PathToTempPicture()), string.Format(Constants.FormatImageCacheGoogle, ur.Segments[ur.Segments.Length-1]));
        }

        /// <summary>
        /// Return file name in temporary file with Contact image
        /// Source code uses from http://www.scip.be/index.php?Page=ArticlesNET07
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        public static string GetContactPicturePath(Outlook.ContactItem contact)
        {
            //string path = PathToTempPicture();
            string picturePath = CreateContactPictureName(contact);

            if (contact.HasPicture)
            {
                foreach (Outlook.Attachment att in contact.Attachments)
                {
                    if (att.DisplayName == "ContactPicture.jpg")
                    {
                        if (File.Exists(picturePath))
                        {
                            if (File.GetCreationTime(picturePath) < contact.LastModificationTime)
                            {
                                CleanupContactPictures(picturePath);
                            }
                            else
                                continue;
                        }
                    }
                    try
                    {
                        LoggerProvider.Instance.Logger.Debug(string.Format("Try write image to:[{0}]", picturePath));
                        att.SaveAsFile(picturePath);
                    }
                    catch (ArgumentException e)
                    {
                        LoggerProvider.Instance.Logger.Error(e);
                        picturePath = "";
                    }
                    catch (System.IO.PathTooLongException lp)
                    {
                        LoggerProvider.Instance.Logger.Error(lp);
                        picturePath = "";
                    }
                }
            }
            else // if in contact not image need clear it's from temp directory
            {
                if (File.Exists(picturePath))
                    CleanupContactPictures(picturePath);
            }
            return picturePath;
        }
        /// <summary>
        /// Return creation date for saved image
        /// </summary>
        /// <param name="picturePath"></param>
        /// <returns></returns>
        public static string PictureMD5(string imagePath)
        {
            /// TODO: need found mehod for image comparsion
            if (!File.Exists(imagePath))
                return "";

            return "";    
        }

        /// <summary>
        /// Return image photo saved on HDD
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        public static string GetContactPicturePath(Google.Contacts.Contact contact)
        {
            string picturePath = CreateContactPictureName(contact);
            if (File.Exists(picturePath))
            {
                if (File.GetCreationTime(picturePath) >= contact.Updated)
                {
                    return picturePath;
                }
                CleanupContactPictures(picturePath);
            }
            Image image = GoogleProvider.GetProvider.GetImage(contact);
            if (image != null)
            {
                LoggerProvider.Instance.Logger.Debug(string.Format("Try write image to:[{0}]", picturePath));
                image.Save(picturePath);
                return picturePath;
            }
            LoggerProvider.Instance.Logger.Error("Proble read image from Google {0}", picturePath);
            return "";
        }

        /// <summary>
        /// Clear all pictures in path
        /// </summary>
        /// <param name="path"></param>
        public static void CleanupContactAllPictures()
        {
            string path = PathToTempPicture();
            foreach (string picturePath in Directory.GetFiles(path, "Contact_*.jpg"))
            {
                try
                {
                    File.Delete(picturePath);
                }
                catch (ArgumentNullException e)
                {
                    LoggerProvider.Instance.Logger.Error("Specified fiel don't exist.");
                    LoggerProvider.Instance.Logger.Error(e);
                }
            }
        }
        /// <summary>
        /// Clean up one image when delete OneContact
        /// </summary>
        /// <param name="contact"></param>
        public static void CleanupContactPictures(string picturePath)
        {
            try
            {
                if (File.Exists(picturePath))
                    File.Delete(picturePath);
            }
            catch (ArgumentNullException e)
            {
                LoggerProvider.Instance.Logger.Error("Can't delete picture {0}", picturePath);
                LoggerProvider.Instance.Logger.Error(e);
            }
        }
        /// <summary>
        /// Return name to temporary path for this aplication
        /// </summary>
        /// <returns></returns>
        public static string PathToTempPicture()
        {
            StringBuilder path = new StringBuilder(Path.GetTempPath());
            path.Append(Constants.ApplicationName);
            path.Append("\\");
            if (!Directory.Exists(path.ToString()))
                try
                {
                    Directory.CreateDirectory(path.ToString());
                    LoggerProvider.Instance.Logger.Debug("Create temporary directory for images: {0}", path.ToString());
                }
                catch (DirectoryNotFoundException de)
                {
                    LoggerProvider.Instance.Logger.Error(de);
                    path.Remove(0, path.Length);
                    path.Append(Path.GetTempPath());
                }
                catch (UnauthorizedAccessException ua)
                {
                    LoggerProvider.Instance.Logger.Error(ua);
                    path.Remove(0, path.Length);
                    path.Append(Path.GetTempPath());
                }
            return path.ToString();
        }
        #endregion

        #region Data Serializate
        /// <summary>
        /// Create name for Google cache file
        /// </summary>
        /// <returns></returns>
        private static string CreateGoogleCacheName()
        {
            return string.Format("{0}\\Contact_{1}.cache", Path.GetDirectoryName(PathToTempPicture()), "google");
        }
        /// <summary>
        /// Create name for Outlook cache file
        /// </summary>
        /// <returns></returns>
        private static string CreateOutlookCacheName()
        {
            return string.Format("{0}\\Contact_{1}.cache", Path.GetDirectoryName(PathToTempPicture()), "outlook");
        }
        /// <summary>
        /// Write data to cache
        /// </summary>
        /// <param name="outData"></param>
        /// <param name="isOutlook"></param>
        private static void SerializeToXML(List<OneContactBase> outData, bool isOutlook)
        {
            string FileName=isOutlook ? CreateOutlookCacheName() : CreateGoogleCacheName();
            try
            {
                if (File.Exists(FileName))
                {
                    File.Delete(FileName);
                    LoggerProvider.Instance.Logger.Debug("Delete current cache file {0}", FileName);
                }
                if (File.Exists(CacheDateFileName(isOutlook)))
                {
                    File.Delete(CacheDateFileName(isOutlook));
                    LoggerProvider.Instance.Logger.Debug("Delete current cache timestamp file {0}", CacheDateFileName(isOutlook));
                }
                using (FileStream fs = new FileStream(FileName, FileMode.Create))
                {
                    LoggerProvider.Instance.Logger.Debug("Serialize to XML {0}", FileName);
                    XmlSerializer serializer = new XmlSerializer(typeof(List<OneContactBase>), new Type[] { typeof(List<OneContact>) });
                    serializer.Serialize(fs, outData);
                    fs.Flush();
                    fs.Close();
                    SaveCacheDate(isOutlook);
                }
            }
            catch (Exception ex)
            {
                LoggerProvider.Instance.Logger.Error(ex);
            }

        }
        /// <summary>
        /// Try read data from cache and return list of this data
        /// </summary>
        /// <param name="isOutlook"></param>
        /// <returns></returns>
        static List<OneContactBase> DeserializeFromXML(bool isOutlook)
        {
            List<OneContactBase> inData = null;
            if (File.Exists(isOutlook ? CreateOutlookCacheName() : CreateGoogleCacheName()))
            {
                try
                {
                    using (System.IO.FileStream fs = new System.IO.FileStream(isOutlook ? CreateOutlookCacheName() : CreateGoogleCacheName(),
                        System.IO.FileMode.Open, System.IO.FileAccess.Read))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(List<OneContactBase>), new Type[] { typeof(List<OneContact>) });
                        inData = (List<OneContactBase>)serializer.Deserialize(fs);
                        fs.Flush();
                        fs.Close();
                    }
                }
                catch (Exception ex)
                {
                    LoggerProvider.Instance.Logger.Error(ex);
                }
            }
            else
                LoggerProvider.Instance.Logger.Warn("Requested cache file not exist: {0}", isOutlook ? CreateOutlookCacheName() : CreateGoogleCacheName());
            return inData;
        }
        /// <summary>
        /// Create cache for Google
        /// </summary>
        /// <param name="goCotacts"></param>
        public static void WriteGoogleToCache(Hashtable goCotacts)
        {
            LoggerProvider.Instance.Logger.Debug("Write Google to data to cache");
            List<OneContactBase> goList = new List<OneContactBase>();
            foreach (OneContactBase cb in goCotacts.Values)
            {
                cb.IsFromCache = true;
                goList.Add(cb);
            }
            SerializeToXML(goList, false);
        }

        /// <summary>
        /// Create cache for Outlook
        /// </summary>
        /// <param name="ouCotacts"></param>
        public static void WriteOutlookToCache(Hashtable ouCotacts)
        {
            LoggerProvider.Instance.Logger.Debug("Write Outlook to data to cache");
            List<OneContactBase> goList = new List<OneContactBase>();
            foreach (OneContactBase cb in ouCotacts.Values)
            {
                cb.IsFromCache = true;
                goList.Add(cb);
            }
            SerializeToXML(goList, true);
        }

        /// <summary>
        /// Return HT contains Outlook data from cache. In any error return null
        /// </summary>
        /// <returns></returns>
        public static Hashtable ReadOutlookFromCache(ref DateTime create)
        {
            List<OneContactBase> readList = DeserializeFromXML(true);
            
            Hashtable ht = new Hashtable();
            if (readList != null)
            {
                foreach (OneContactBase on in readList)
                {
                    on.IsFromCache = true;
                    on.MD5selfCount = on.MD5Actual();
                    ht.Add(on._MyID, on);
                }
                try
                {
                    FileInfo i = new FileInfo(CreateOutlookCacheName());
                    create = i.CreationTime;
                }
                catch (Exception e)
                {
                    LoggerProvider.Instance.Logger.Error(e);
                    create = DateTime.MinValue;
                }
            }
            return ht;
        }
        /// <summary>
        /// Return HT contains Google data from cache. In any error return null
        /// </summary>
        /// <returns></returns>
        public static Hashtable ReadGoogleFromCache(ref DateTime create)
        {
            List<OneContactBase> readList = DeserializeFromXML(false);
            Hashtable ht = new Hashtable();
            if (readList != null)
            {
                foreach (OneContactBase on in readList)
                {
                    on.IsFromCache = true;
                    on.MD5selfCount = on.MD5Actual();
                    ht.Add(on._MyID, on);
                }
                try
                {
                    FileInfo i = new FileInfo(CreateGoogleCacheName());
                    create = i.CreationTime;
                }
                catch (Exception e)
                {
                    LoggerProvider.Instance.Logger.Error(e);
                    create = DateTime.MinValue;
                }
            }
            return ht;
        }
        #endregion

        #region Delete data from Cache
        /// <summary>
        /// This use when need remove cache file (disable it)
        /// </summary>
        /// <param name="isOutlook"></param>
        public static void RemoveCacheFile(bool isOutlook)
        {
            string FileName = isOutlook ? CreateOutlookCacheName() : CreateGoogleCacheName();
            try
            {
                if (File.Exists(FileName))
                {
                    File.Delete(FileName);
                    LoggerProvider.Instance.Logger.Debug("Delete current cache file {0}", FileName);
                }
                if (File.Exists(CacheDateFileName(isOutlook)))
                {
                    File.Delete(CacheDateFileName(isOutlook));
                    LoggerProvider.Instance.Logger.Debug("Delete current cache timestamp file {0}", CacheDateFileName(isOutlook));
                }
            }
            catch (Exception ex)
            {
                LoggerProvider.Instance.Logger.Error(ex);
            }
            RemoveCacheImages(isOutlook);
        }

        /// <summary>
        /// Delete all image file from cache
        /// </summary>
        /// <param name="isOutlook">define what source delete</param>
        public static void RemoveCacheImages(bool isOutlook)
        {
            string FileName = Path.GetDirectoryName(isOutlook ? CreateOutlookCacheName() : CreateGoogleCacheName());
            string FoundName = isOutlook ? string.Format(Constants.FormatImageCacheOutlook, "*") : string.Format(Constants.FormatImageCacheGoogle, "*");
            string[] FileList = Directory.GetFiles(FileName, FoundName);
            foreach (string f in FileList)
            {
                if (File.Exists(f))
                {
                    try
                    {
                        File.Delete(f);
                    }
                    catch (FileNotFoundException fe)
                    {
                        LoggerProvider.Instance.Logger.Error("File name:{0}\r\n{1}", fe.FileName, fe.Message);
                    }
                    catch (UnauthorizedAccessException uae)
                    {
                        LoggerProvider.Instance.Logger.Error("File name:{0}\r\n{1}", f, uae.Message);
                    }
                }
            }

        }
        #endregion

        #region Save DateTime of last synchronization
        /// <summary>
        /// Save date when need start read changes on servers
        /// </summary>
        /// <param name="IsOutlook">True when save this for outlook</param>
        public static void SaveCacheDate(bool IsOutlook)
        {
            try
            {
                TextWriter tw = new StreamWriter(CacheDateFileName(IsOutlook));
                tw.WriteLine(DateTime.Now.ToString("yyyyMMdd HH:mm:ss"));
                tw.Close();
            }
            catch (IOException ioe)
            {
                LoggerProvider.Instance.Logger.Error("Save timestamp for cache has problem", ioe);
            }
            catch (FormatException fe)
            {
                LoggerProvider.Instance.Logger.Error("Save timestamp for cache has problem", fe);
            }
        }
        /// <summary>
        /// Load saved datetime where need start read changes from server
        /// </summary>
        /// <param name="IsOutlook">True when save this for outlook</param>
        /// <returns>Read date time or DateTime.MinValue</returns>
        public static DateTime LoadCacheDate(bool IsOutlook)
        {
            DateTime ret=DateTime.MinValue;
            if (File.Exists(CacheDateFileName(IsOutlook)))
            {
                try
                {
                    string Stamp = File.ReadAllText(CacheDateFileName(IsOutlook));
                    if (!DateTime.TryParse(Stamp, out ret))
                        ret = DateTime.MinValue;
                }
                catch (IOException ioe)
                {
                    LoggerProvider.Instance.Logger.Error("Load timestamp for cache has problem", ioe);
                }
            }
            return ret;
        }

        private static string CacheDateFileName(bool IsOutlook)
        {
            return string.Format("{0}\\Contact_{1}.time", Path.GetDirectoryName(PathToTempPicture()), IsOutlook ? "outlook" : "google");
        }
        #endregion
    }
}
