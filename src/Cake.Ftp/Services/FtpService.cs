﻿using System;
using System.IO;
using System.Net;
using System.Text;
using Cake.Core.Diagnostics;
using Cake.Core.IO;

namespace Cake.Ftp.Services {
    /// <summary>
    /// The FTP Service.
    /// </summary>
    public class FtpService : IFtpService {
        private readonly ICakeLog _log;

        /// <summary>
        /// Intializes a new instance of the <see cref="FtpService"/> class. 
        /// </summary>
        /// <param name="log"></param>
        public FtpService(ICakeLog log) {
            _log = log;
        }

        /// <summary>
        /// Uploads a file.
        /// </summary>
        /// <param name="serverUri">The URI for the FTP server.</param>
        /// <param name="uploadFile">The file to upload.</param>
        /// <param name="username">The FTP username.</param>
        /// <param name="password">The FTP password.</param>
        public void UploadFile(Uri serverUri, IFile uploadFile, string username, string password) {
            // Adding verbose logging for the URI being used.
            _log.Verbose("Uploading file to {0}", serverUri);
            // Creating the request
            var request = (FtpWebRequest) WebRequest.Create(serverUri);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(username, password);

            using (var streamReader = new StreamReader(uploadFile.OpenRead())) {
                // Get the file contents.
                var fileContents = Encoding.UTF8.GetBytes(streamReader.ReadToEnd());
                request.ContentLength = fileContents.Length;

                // Writing the file to the request stream.
                var requestStream = request.GetRequestStream();
                requestStream.Write(fileContents, 0, fileContents.Length);
                requestStream.Close();

                // Getting the response from the FTP server.
                var response = (FtpWebResponse) request.GetResponse();

                // Logging if it completed and the description of the status returned.
                _log.Information("File upload complete, status {0}", response.StatusDescription);
                response.Close();
            }
        }

        /// <summary>
        /// Deletes a file.
        /// </summary>
        /// <param name="serverUri">The URI for the FTP server.</param>
        /// <param name="username">The FTP username.</param>
        /// <param name="password">The FTP password.</param>
        public void DeleteFile(Uri serverUri, string username, string password) {
            var request = (FtpWebRequest)WebRequest.Create(serverUri);
            request.Method = WebRequestMethods.Ftp.DeleteFile;

            // Adding verbose logging for credentials used.
            _log.Verbose("Using the following credentials {0}, {1}", username, password);
            request.Credentials = new NetworkCredential(username, password);

            var response = (FtpWebResponse)request.GetResponse();
            // Logging if it completed and the description of the status returned.
            _log.Information("File upload complete, status {0}", response.StatusDescription);
            response.Close();
        }
    }
}
