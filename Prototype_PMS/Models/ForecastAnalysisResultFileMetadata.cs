﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Prototype_PMS.Models
{
    public class ForecastAnalysisResultFileMetadata
    {
        public int ID { get; set; }
        public Nullable<int> ForecastPeriodResultRemarkID { get; set; }
        public Nullable<int> FilePathID { get; set; }
    }
    [MetadataType(typeof(ForecastAnalysisResultFileMetadata))]
    public partial class ForecastAnalysisResultFile
    {

    }
}