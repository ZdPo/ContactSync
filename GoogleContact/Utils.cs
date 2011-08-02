using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Drawing;


using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;

namespace GoogleContact
{
    /// <summary>
    /// Some help methods
    /// </summary>
    class Utils
    {
        /// <summary>
        /// Based of FxCop recomendation
        /// </summary>
        private Utils()
        {}
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

        /// <summary>
        /// Create name for Contact picture from Outlook
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        public static string CreateContactPictureName(Outlook.ContactItem contact)
        {
            return string.Format("{0}\\Contact_{1}.jpg", Path.GetDirectoryName(PathToTempPicture()), contact.EntryID);
        }
        /// <summary>
        /// Create name for Contact picture from Google
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        public static string CreateContactPictureName(Google.Contacts.Contact contact)
        {
            Uri ur = new Uri(contact.Id);
            return string.Format("{0}\\Contact_{1}.jpg", Path.GetDirectoryName(PathToTempPicture()), ur.Segments[ur.Segments.Length-1]);
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
    }
}
