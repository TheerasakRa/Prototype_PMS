using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Prototype_PMS.Models
{
    public class FilePathMetadata
    {
        public int ID { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
    [MetadataType(typeof(FilePathMetadata))]
    public partial class ForecastFilePath
    {

    }
}