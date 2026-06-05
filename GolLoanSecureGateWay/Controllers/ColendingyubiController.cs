using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static GoldLoanSecureGateWay.Model.ColendingybiRequest;
using static GoldLoanSecureGateWay.Model.ColendingyubiResponse;
using System.IO;
using Renci.SshNet;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GoldLoanSecureGateWay.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]

    public class ColendingyubiController : Controller
    {
        // GET: /<controller>/
        //public IActionResult Index()
        //{
        //    return View();
        //}
        [HttpPost]

        public ActionResult<ColendingybiAgreementsignResponse> PostColendingLoanAgreementSigning([FromBody] ColendingybiAgreementsignRequest request)
        {
            ColendingybiAgreementsignResponse response = new ColendingybiAgreementsignResponse();
            if (ModelState.IsValid)
            {
                

                string hostname = "sftp.asirvad.com";
               // string hostname = "";
                int port = 22;
                string sftp_username = "yubi";
                string sftp_password = "Asir@12321#";
                //DateTime dtValue = Convert.ToDateTime(dt.Rows[0][0]);
                // string Maturity_date = dtValue.ToString("dd-MM-yyyy");

                string quotation_no = request.Quotation_no;

                string base64String = request.Document;
                string custKycimage = request.kycDocument;

                byte[] pdfBytes = Convert.FromBase64String(base64String);
                byte[] pdfBytes1 = Convert.FromBase64String(custKycimage);
                string fileName = quotation_no;
                //string fileName = "03250872433888";
                string filepath = request.path;
                string filepath1 = request.path1;
                string filepath2 = request.path2;

                int res = UploadToSftp(pdfBytes, fileName, hostname, 22, sftp_username, sftp_password, "/ASIRVA_IDFC_GOLD_1/", filepath, pdfBytes1, filepath1, filepath2);
                if (res == 1)
                {
                    response.result = 1;
                    response.flag = 1;
                    response.code = 1;
                }
                else
                {
                    response.result = 0;
                    response.flag = 0;
                    response.code = 0;
                }

                    return response;
                
            }
            else
            {
            }
            return response;
        }

        public static int UploadToSftp(byte[] fileBytes, string SubfolderName, string sftpHost, int sftpPort, string sftpUser, string sftpPassword, string remoteDirectory, string path, byte[] fileBytes1, string path1, string path2)
        {

            int result = 0;
            SftpClient sftp = null;
            string newDirectory = "";
            string fileName = path1 + ".pdf";
            try
            {

                sftp = new SftpClient(sftpHost, sftpPort, sftpUser, sftpPassword);
                sftp.Connect();
                newDirectory = remoteDirectory + path + "/" + path1;



                string[] folderStack = newDirectory.Trim('/').Split('/');
                string index = "";
                foreach (string folder in folderStack)
                {
                    index = index + "/" + folder;
                    if (!sftp.Exists(index))
                        sftp.CreateDirectory(index);
                }
                string remoteFilePath1 = newDirectory.TrimEnd('/') + "/" + fileName;
                using (var ms1 = new MemoryStream(fileBytes))
                {
                    ms1.Position = 0;
                    sftp.UploadFile(ms1, remoteFilePath1, true);
                }
                //--------- File 1 upload complete

                //start File 2 upload

                newDirectory = remoteDirectory + path + "/" + path2;

                folderStack = newDirectory.Trim('/').Split('/');
                index = "";
                foreach (string folder in folderStack)
                {
                    index = index + "/" + folder;
                    if (!sftp.Exists(index))
                        sftp.CreateDirectory(index);
                }
                remoteFilePath1 = newDirectory.TrimEnd('/') + "/" + path2 + DetectFileType(fileBytes1);
                using (var ms1 = new MemoryStream(fileBytes1))
                {
                    ms1.Position = 0;
                    sftp.UploadFile(ms1, remoteFilePath1, true);
                }


                //--------------------------

                // Create a nested folder inside the base directory
                //string nestedFolder = newDirectory.TrimEnd('/') ;
                //if (!sftp.Exists(nestedFolder))
                //    sftp.CreateDirectory(nestedFolder);
                //string extension = DetectFileType(fileBytes1); 
                //// Upload the second file into the nested folder
                //string remoteFilePath2 = nestedFolder + path2 +"/" + path2 + DetectFileType(fileBytes1);
                //using (var ms2 = new MemoryStream(fileBytes1))
                //{
                //    ms2.Position = 0;
                //    sftp.UploadFile(ms2, remoteFilePath2, true);
                //}
                result = 1;
            }
            catch (Exception ex)
            {
                string path3 = @"C:\Users\101006\Desktop\CO-LENDING YUBI\errorlog.txt";
                System.IO.File.AppendAllText(path1, ex.ToString());
                result = 0;
                throw;

            }
            finally
            {
                if (sftp != null && sftp.IsConnected)
                    sftp.Disconnect();
            }
            return result;
        }

        private static string DetectFileType(byte[] fileBytes)
        {
            if (fileBytes.Length > 4)
            {
                // PDF
                if (fileBytes[0] == 0x25 && fileBytes[1] == 0x50 &&
                    fileBytes[2] == 0x44 && fileBytes[3] == 0x46)
                    return ".pdf";

                // JPEG
                if (fileBytes[0] == 0xFF && fileBytes[1] == 0xD8)
                    return ".jpg";

                // PNG
                if (fileBytes[0] == 0x89 && fileBytes[1] == 0x50 &&
                    fileBytes[2] == 0x4E && fileBytes[3] == 0x47)
                    return ".png";
            }

            // Default fallback
            return "bin";
        }


    }
}
