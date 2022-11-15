using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TRAVEL_CORE.Entities;
using TRAVEL_CORE.Repositories.Abstract;
using TRAVEL_CORE.Repositories.Concrete;
using TRAVEL_CORE.Tools;

namespace TRAVEL_CORE.Controllers
{
    [Route("api/[controller]/[action]")]
    [Authorize]
    [ApiController]
    public class CommonController : ControllerBase
    {
        private readonly ICommonRepository _commonRepository;

        public CommonController(ICommonRepository commonRepository)
        {
            _commonRepository = commonRepository;
        }

        [HttpPost]
        public IActionResult UploadFile()
        {
            IFormFile file = null;

            try
            {
                file = Request.Form.Files[0];
            }
            catch (Exception)
            { }

            if (file == null)
                return BadRequest(new { message = "No files to upload!" });

            UploadedFile uploadedFile = new UploadedFile();
            FileOperation fileOperation = new FileOperation();
            UploadedFile uploaded = new UploadedFile();
            uploadedFile.FileType = 1;

            uploadedFile.FileFolder = "Temporary";
            uploaded = fileOperation.UploadFile(file, uploadedFile, insert: false, autoFolderDivision: false);

            return Ok(uploaded);
        }

        [HttpGet]
        public IActionResult GetSpecode(string type)
        {
            try
            {
                return Ok(JsonConvert.SerializeObject(_commonRepository.GetSpecode(type)));
            }
            catch (System.Exception)
            {
                return BadRequest(new { message = "Unexpected error occurred!" });
            }
        }

        /// <summary>
        /// Get Template Costs Id And Text
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetTemplateCosts()
        {
            try
            {
                return Ok(JsonConvert.SerializeObject(_commonRepository.GetTemplateCosts()));

            }
            catch (Exception)
            {
                return BadRequest(new { message = "Unexpected error occurred!" });
            }
        }

        /// <summary>
        /// Get Firms Id And Text
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetFirms()
        {
            try
            {
                return Ok(JsonConvert.SerializeObject(_commonRepository.GetFirms()));

            }
            catch (Exception)
            {
                return BadRequest(new { message = "Unexpected error occurred!" });
            }
        }

    }

}
