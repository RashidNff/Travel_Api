using Microsoft.AspNetCore.Http;
using TRAVEL_CORE.DAL;
using TRAVEL_CORE.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;

namespace TRAVEL_CORE.Tools
{
    public class FileOperation
    {
        public void CreateFolder(string directory)
        {
            try
            {
                //If the directory does not exist, create it. 
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }
            catch (Exception)
            { }
        }


        public void RenameFolder(string rootPath, string mainFolder, string from, string to)
        {
            string directory = Path.Combine(rootPath, mainFolder);

            try
            {
                //If the directory exists, rename it. 
                if (Directory.Exists(directory + $@"\{from}"))
                    Directory.Move(directory + $@"\{from}", directory + $@"\{to}");
            }
            catch (Exception)
            { }
        }

        public UploadedFile MoveFile(string fileNameToMove, string mainFolder)
        {
            Connection connection = new Connection();
            UploadedFile uploaded = new UploadedFile();

            DateTime dateTime = DateTime.Now;
            string requestFolder = "wwwroot";
            string uploadFolder = Path.Combine("uploads", mainFolder, dateTime.ToString("yyyy/MM/dd").Replace("-", "\\"));
            string pathToSave = Path.Combine(Directory.GetCurrentDirectory(), requestFolder, uploadFolder);
            string fileName = Path.GetFileNameWithoutExtension(fileNameToMove) + "_" + dateTime.ToString("yyyyMMdd") + "_" + dateTime.ToString("HHmmssfff") + Path.GetExtension(fileNameToMove);
            string fullPath = Path.Combine(pathToSave, fileName);
            string dbPath = Path.Combine(uploadFolder, fileName);

            CreateFolder(Path.Combine(requestFolder, uploadFolder));

            File.Move(Path.Combine(Directory.GetCurrentDirectory(), requestFolder, "uploads", "Temporary", fileNameToMove), fullPath);

            //int generatedId = 0;

            //List<SqlParameter> parameters = new List<SqlParameter>();
            //parameters.Add(new SqlParameter("UF_TYPE", 1));
            //parameters.Add(new SqlParameter("UF_FOLDER", mainFolder));
            //parameters.Add(new SqlParameter("UF_FILENAME", fileName));
            //parameters.Add(new SqlParameter("UF_FILEPATH", dbPath));
            //parameters.Add(new SqlParameter("UF_U_ID", userId));

            //generatedId = connection.Execute(tableName: "TBL_UPFILES", operation: OperationType.Insert, parameters: parameters);

            //uploaded.FileId = generatedId;
            uploaded.FileName = fileName;
            uploaded.FilePath = dbPath;

            return uploaded;
        }


        public UploadedFile UploadFile(IFormFile file, UploadedFile uploadedFile, int lastFileId = -1, bool insert = true, bool autoFolderDivision = true)
        {
            Connection connection = new Connection();
            UploadedFile uploaded = new UploadedFile();

            if (lastFileId != -1)
                DeleteFile(lastFileId);

            try
            {
                DateTime dateTime = DateTime.Now;
                string requestFolder = "wwwroot";
                string uploadFolder = (!autoFolderDivision) ? Path.Combine("uploads", uploadedFile.FileFolder) : Path.Combine("uploads", uploadedFile.FileFolder, dateTime.ToString("yyyy/MM/dd").Replace("-", "\\"));
                string pathToSave = Path.Combine(Directory.GetCurrentDirectory(), requestFolder, uploadFolder);
                string fileName = Path.GetFileNameWithoutExtension(file.FileName) + "_" + dateTime.ToString("yyyyMMdd") + "_" + dateTime.ToString("HHmmssfff") + Path.GetExtension(file.FileName);
                string fullPath = Path.Combine(pathToSave, fileName);
                string dbPath = Path.Combine(uploadFolder, fileName);

                CreateFolder(Path.Combine(requestFolder, uploadFolder));

                var stream = new FileStream(fullPath, FileMode.Create);
                file.CopyTo(stream);
                
                //int generatedId = 0;

                //if (insert)
                //{
                //    List<SqlParameter> parameters = new List<SqlParameter>();
                //    parameters.Add(new SqlParameter("UF_TYPE", uploadedFile.FileType));
                //    parameters.Add(new SqlParameter("UF_FOLDER", uploadedFile.FileFolder));
                //    parameters.Add(new SqlParameter("UF_FILENAME", fileName));
                //    parameters.Add(new SqlParameter("UF_FILEPATH", dbPath));
                //    parameters.Add(new SqlParameter("UF_U_ID", uploadedFile.UserId));

                //    generatedId = connection.Execute(tableName: "TBL_UPFILES", operation: OperationType.Insert, parameters: parameters);
                //}

                //uploaded.FileId = generatedId;
                uploaded.FileName = fileName;
                uploaded.FilePath = dbPath;
                stream.Close();

            }
            catch (Exception ex)
            { }
            return uploaded;
        }

        public void DeleteFile(int fileId)
        {
            Connection connection = new Connection();
            connection.RunQuery("Update TBL_UPFILES set UF_STATUS=-1 where UF_ID=" + fileId);
        }

        public void DeleteFileFromServer(string rootDirectory, int lastFileId, string lastFilePath)
        {
            try
            {
                string directory = rootDirectory + "\\" + lastFilePath.Replace(@"/", @"\");
                var fileInfo = new FileInfo(directory);
                fileInfo.Delete();
            }
            catch (Exception ex)
            { }

            Connection connection = new Connection();
            connection.RunQuery("Delete fron TBL_UPFILES where UF_ID=" + lastFileId);
        }

        public string GetUniqueFileName(string prefix)
        {
            return prefix + "-" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss");
        }
    }
}
