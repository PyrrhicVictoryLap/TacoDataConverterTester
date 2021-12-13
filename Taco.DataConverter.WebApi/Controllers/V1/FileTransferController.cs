using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.WebUtilities;
using System.IO;
using Taco.DataConverter.WebApi.Helpers;

namespace Taco.DataConverter.WebApi.Controllers.V1
{
    [Route("v1/filetransfers")]
    [ApiController]
    public class FileTransferController : ControllerBase
    {

        #region Globals and Constants

        #endregion Globals and Constants

        #region Constructors

        #endregion Constructors

        #region Exposed Methods

        //Route: v1/filetransfers/upload/creators/{createdBy}/locations/{tacoLocation}.
        //Example: v1/filetransfers/upload/creators/Automated%20System/locations/Files.
        /// <summary>
        /// Uploads a new file to the server.
        /// </summary>
        /// <param name="cancellationToken">Optional token that can end tasks.</param>
        /// <returns></returns>
        [DisableFormValueModelBinding]
        [DisableRequestSizeLimit]
        [HttpPost("upload")] //The second parameter is optional.
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UploadFilesAsync(CancellationToken cancellationToken = default)
        {
            //Much of this code comes from Microsoft's large file upload sample.
            //https://github.com/dotnet/AspNetCore.Docs/tree/main/aspnetcore/mvc/models/file-uploads/samples/5.x

            var request = HttpContext.Request;
            //The MultipartReader does not play well with model binding, so the route values need to be gathered manually.
            var routeValues = Request.RouteValues;

            //TODO: Verify file is excel

            if (!request.HasFormContentType ||
                !MediaTypeHeaderValue.TryParse(request.ContentType, out var mediaTypeHeader) ||
                string.IsNullOrEmpty(mediaTypeHeader.Boundary.Value))
            {
                return new UnsupportedMediaTypeResult();
            }

            var reader = new MultipartReader(mediaTypeHeader.Boundary.Value, request.Body);
            var section = await reader.ReadNextSectionAsync();

            //This will check all files that have been included in the request.
            while (section != null)
            {
                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition,
                    out var contentDisposition);

                //Check that the form data is a file. Other types of form data will be skipped.
                if (hasContentDispositionHeader && contentDisposition.DispositionType.Equals("form-data") &&
                    !string.IsNullOrEmpty(contentDisposition.FileName.Value))
                {
                    //There should probably be some logic here to evaluate file types and whatnot.
                    //Our risks are not too high since this is internal, but it would still probably be a good idea.

                    var fileName = $"{DateTime.Now.Ticks}_{Guid.NewGuid()}.xlsx";
                    var fullPath = Path.Combine("C:\\APITest", fileName);
                    //var hashToSave = string.Empty;

                    //The file is copied before the data is created.
                    //This means that there is the possibility that orphan files can exist if something goes wrong with data creation.
                    //Add a method to clean orphan files at some point.
                    using (var stream = System.IO.File.Create(fullPath))
                    {
                        await section.Body.CopyToAsync(stream, cancellationToken);

                    }

                }

                section = await reader.ReadNextSectionAsync(cancellationToken);
            }

            //BusinessLogic called here
            

            //Set badrequest condition
            if (true)
            {
                return Ok();
            }
            else
            {
                return BadRequest("No file data in the request.");
            }
        }

        #endregion
    }
}
