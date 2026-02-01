using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GoAnimateRipper2
{
    public class AssetManager
    {
        MainControl mainControl;
        EncryptionManager encryptionManager = new EncryptionManager();
        HttpClient httpClient = new HttpClient();

        List<string> pathes = new List<string>();
        Boolean autoMode = false;
        byte[] key;

        public AssetManager(MainControl mainControl)
        {
            this.mainControl = mainControl;

            autoMode = mainControl.decryptionKey != "(auto)" ? false : true;
            key = Encoding.ASCII.GetBytes(mainControl.decryptionKey);
        }

        public async Task<bool> DownloadAssetDirect(string url, string path)
        {
            using (var response = await httpClient.GetAsync(url))
            {
                if (!(response.StatusCode == System.Net.HttpStatusCode.Found || response.StatusCode == System.Net.HttpStatusCode.OK))
                {
                    mainControl.writeMessage($"[ERROR] Server returned {response.StatusCode} at {url}...");
                    return false;
                }
                var data = await response.Content.ReadAsByteArrayAsync();
                File.WriteAllBytes(path, data);
                return true;
            }
        }

        public async Task<bool> DownloadAsset(string path, bool isSupposedToHaveEncryption)
        {
            //Setup Phase.

            bool isSwf = true;
            if (!path.ToLower().Contains(".swf"))
            {
                isSwf = false;
            }

            if (!mainControl.doDecompile && File.Exists(GetLocalPath(path)) && !mainControl.overwriteFiles)
            {
                return false;
            }

            if ((isSwf && mainControl.skipFlash) || (!isSwf && mainControl.skipNonFlash))
            {
                return false;
            }

            //Execution Phase.
            using (var response = await httpClient.GetAsync(GetPathAsUrl(path)))
            {
                if (!(response.StatusCode == System.Net.HttpStatusCode.Found || response.StatusCode == System.Net.HttpStatusCode.OK))
                {
                    mainControl.writeMessage($"[ERROR] Server returned {response.StatusCode} at {GetPathAsUrl(path)}...", true);
                    //Hack to make sure it doesn't crash when requesting invalid xml urls.
                    if (!path.ToLower().Contains(".xml")) return false;
                }
                var data = await response.Content.ReadAsByteArrayAsync();
                bool keySuccessfullyMatched = true;
                bool isFileEncrypted = !encryptionManager.IsFlashPrefix(data);
                // If it's a SWF, deal with encryption jargon.
                if (isSwf)
                {

                    // If auto mode, try and figure out the key.
                    if (isSupposedToHaveEncryption && autoMode)
                    {
                        key = encryptionManager.DetermineKey(data);
                        if (key == null) keySuccessfullyMatched = false;
                    }
                    // If the key was found, decrypt. If it isn't auto mode, it will always work.
                    if (isSupposedToHaveEncryption && mainControl.doDecryption && keySuccessfullyMatched)
                    {
                        data = encryptionManager.Decrypt(key, data);
                    }
                    // If re-encrypt is checked and everything succeeded (or a specific edge case occured where auto mode was on and the file was supposed to be encrypted but wasn't), do re-encryption.
                    if (mainControl.doReEncryption && isSupposedToHaveEncryption && (keySuccessfullyMatched || !isFileEncrypted))
                    {
                        data = encryptionManager.Decrypt(Encoding.ASCII.GetBytes(mainControl.reEncryptionKey), data);
                    }
                }

                //Output phase.
                //If we're in FFDec mode, save the path for later, and place the file in the JPEXS target folder.
                if (mainControl.doDecompile && isSwf)
                {
                    if (pathes.Contains(GetLocalPath(path))) return false; //Dupes CANNOT exist. It's kindof a whatev thing normally but in this context we need to make sure.
                    String dir = $".\\\\ffdec_working";
                    Directory.CreateDirectory(dir);
                    pathes.Add(GetLocalPath(path));
                    dir = $".\\\\ffdec_working\\" + (pathes.Count - 1) + ".swf";
                    File.WriteAllBytes(dir, data);

                }
                else
                {
                   File.WriteAllBytes(GetLocalPath(path), data);
                }

                return true;
            }
        }

        /// <summary>
        /// void <c>GetPathAsUrl</c> returns path as full URL path.
        /// </summary>
        private String GetPathAsUrl(string path)
        {
            return mainControl.domain + path.Replace("\\", "/");
        }

        /// <summary>
        /// void <c>GetLocalPath</c> returns path as full directory path.
        /// </summary>
        public String GetLocalPath(string path)
        {
            Directory.CreateDirectory(path.Substring(0, path.LastIndexOf("\\")));
            return ".\\" + path;
        }

        public List<string> getPathes()
        {
            return pathes;
        }
    }
}
