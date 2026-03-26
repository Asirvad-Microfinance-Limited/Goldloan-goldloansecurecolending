using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static GoldLoanSecureGateWay.Model.ColendingybiRequest;
using static GoldLoanSecureGateWay.Model.ColendingyubiResponse;
using Renci.SshNet;
using System.IO;


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
                
                string quotation_no =request.Quotation_no;

                string base64String = request.Document;

                byte[] pdfBytes = Convert.FromBase64String(base64String);
                string fileName = quotation_no;

                string filepath=request.path;

                int res = UploadToSftp(pdfBytes, fileName, hostname, 22, sftp_username, sftp_password, "/ASIRVA_IDFC_GOLD_1/", filepath);
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

        public static int UploadToSftp(byte[] fileBytes, string SubfolderName, string sftpHost, int sftpPort, string sftpUser, string sftpPassword, string remoteDirectory,string path)
        {

            int result = 0;
            SftpClient sftp = null;
            string newDirectory = "";
            string fileName = SubfolderName + ".pdf";
            try
            {

                sftp = new SftpClient(sftpHost, sftpPort, sftpUser, sftpPassword);
                sftp.Connect();
                newDirectory = remoteDirectory + path;

                string[] folderStack=newDirectory.Trim('/').Split('/');
                string index="";
                foreach (string folder in folderStack)
                {
                    index = index + "/" + folder;
                    if (!sftp.Exists(index))
                        sftp.CreateDirectory(index);
                }
                

                string remoteFilePath = newDirectory.TrimEnd('/') + "/" + fileName;

                using (var memoryStream = new MemoryStream(fileBytes))
                {
                    memoryStream.Position = 0;
                    sftp.UploadFile(memoryStream, remoteFilePath, true);
                }

                result = 1;
            }
            catch (Exception ex)
            {
                string path1 = @"C:\Users\101006\Desktop\CO-LENDING YUBI\errorlog.txt";
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

         
                
            
    }
}
