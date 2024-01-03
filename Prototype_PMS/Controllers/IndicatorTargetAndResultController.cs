using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Xml.Serialization;
using Newtonsoft.Json.Serialization;
using Prototype_PMS.Models;

namespace Prototype_PMS.Controllers
{
    public class IndicatorTargetAndResultController : Controller
    {
        private PMSEntities1 db = new PMSEntities1();

        static string _SceneView = "";
        static int? _IndicatorID;
        static List<SelectListItem> _IndicatorTypeDropdownList = new List<SelectListItem>();
        static List<SelectListItem> _IndicatorUnitDropdownList = new List<SelectListItem>();
        // Static Dictionary ที่เก็บข้อมูลเกี่ยวกับช่วงเวลา
        static Dictionary<int, Dictionary<int, string>> _PeriodDict = new Dictionary<int, Dictionary<int, string>>();

        // Dictionary สำหรับเก็บข้อมูลเดือน
        Dictionary<int, string> MountDict = new Dictionary<int, string>
        {
            {0, "ต.ค."},
            {1, "พ.ย."},
            {2, "ธ.ค."},
            {3, "ม.ค."},
            {4, "ก.พ."},
            {5, "มี.ค."},
            {6, "เม.ย."},
            {7, "พ.ค."},
            {8, "มิ.ย."},
            {9, "ก.ค."},
            {10, "ส.ค."},
            {11, "ก.ย."}
        };

        // Dictionary สำหรับเก็บข้อมูลไตรมาส
        Dictionary<int, string> QuaterDict = new Dictionary<int, string>
        {
            {0, "ไตรมาส 1"},
            {1, "ไตรมาส 2"},
            {2, "ไตรมาส 3"},
            {3, "ไตรมาส 4"},
        };

        // Dictionary สำหรับเก็บข้อมูลปี
        Dictionary<int, string> YearDict = new Dictionary<int, string>
        {
            {0, "ราย6เดือน"},
            {1, "สิ้นปี"},
        };

        // List สำหรับเก็บข้อมูลที่ใช้เลือกใน dropdown list
        readonly List<SelectListItem> _predicDropdownList = new List<SelectListItem>()
        {
            new SelectListItem(){Text = "คาดการณ์หน่วย1"},
            new SelectListItem(){Text = "คาดการณ์หน่วย2"},
            new SelectListItem(){Text = "คาดการณ์หน่วย3"},
        };

        public ActionResult Index(int? Year, string Division, string Indicator)
        {
            // ล้างค่าที่ถูกเก็บไว้ในตัวแปร static (ถ้ามี)
            ClearStatic();

            // ตรวจสอบและกำหนดค่า default ให้กับ Division และ Indicator ถ้าหากมีค่าว่าง
            if (Division == "") { Division = null; }
            if (Indicator == "") { Indicator = null; }

            // ดึงข้อมูล Indicators ทั้งหมดที่ไม่ถูกลบ
            var indicators = db.Indicators.Where(s => s.isDelete == false);

            // กำหนดค่า indicatorYear ให้กับทุกรายการ Indicator ใน indicators
            foreach (var indicator in indicators) { indicator.indicatorYear = Year; };

            // กรณีมีค่า Indicator ไม่เป็น null ให้กรองเฉพาะ Indicator ที่มีชื่อ (Indicator1) ที่มี substring ตรงกับค่า Indicator
            if (Indicator != null)
            {
                indicators = indicators.Where(s => s.Indicator1.Contains(Indicator));
            }

            // กรณีมีค่า Division ไม่เป็น null ให้กรองเฉพาะ Indicator ที่มี IndicatorOwners ที่มี Division ตรงกับค่า Division
            if (Division != null)
            {
                indicators = indicators.Where(s => s.IndicatorOwners.Where(q => q.Division == Division).Count() > 0);
            }

            // กรณีมีค่า Year ไม่เป็น null ให้กรอง Indicators ที่มีวันที่สร้าง (CreateDate) อยู่ในช่วงเวลาตั้งแต่ 1 ตุลาคม ปีก่อนหน้าถึง 30 กันยายน ปีที่กำหนด
            if (Year != null)
            {
                DateTime startYear = new DateTime(Year.Value - 1, 10, 1);
                DateTime endYear = new DateTime(Year.Value, 9, 30);
                indicators = indicators.Where(i => i.CreateDate != null && i.CreateDate >= startYear && i.CreateDate <= endYear);
            }

            // เตรียมข้อมูลปีที่ใช้ใน dropdown list บน View
            selectYear();

            // เตรียมข้อมูล Division และ SelectedYear สำหรับใช้ใน View
            ViewBag.Division = indicatorOwners();
            ViewBag.SelectedYear = Year;

            // ส่งข้อมูล Indicators ที่ผ่านการกรองมาแสดงผลใน View
            return View(indicators.ToList());
        }

        public SelectList indicatorOwners()
        {
            var uniqueOwners = db.Owners
                .Select(i => new SelectListItem
                {
                    Text = i.Division,
                    Value = i.Division
                })
                .ToList();

            SelectList selectListOwner = new SelectList(uniqueOwners, "Value", "Text");

            return selectListOwner;
        }
        //public SelectList PredictOwners()
        //{
        //    var uniqueOwners = db.Owners.Select(i => new SelectListItem
        //    {
        //        Text = i.Division,
        //        Value = i.Division
        //    });

        //    SelectList selectListOwner = new SelectList(uniqueOwners, "Value", "Text");

        //    return selectListOwner;
        //}
        public void indicatorType(Indicator indicator)
        {
            List<SelectListItem> selectListType = new List<SelectListItem>();

            foreach (var u in indicator.IndicatorXIndicatorTypes)
            {
                if (u.isCheck == true)
                {
                    selectListType.Add(new SelectListItem
                    {
                        Text = u.IndicatorType.IndicatorType1,
                        Value = u.IndicatorID.ToString(),
                    });
                }
            }

            SelectList typeSelectList = new SelectList(selectListType, "Value", "Text");
            ViewBag.IndicatorType = typeSelectList;
        }
        public void indicatorUnit(Indicator indicator)
        {

            List<SelectListItem> selectListUnit = new List<SelectListItem>();

            foreach (var u in indicator.IndicatorUnits)
            {
                selectListUnit.Add(new SelectListItem
                {
                    Text = u.Unit,
                    Value = u.ID.ToString()
                });
            }

            SelectList unitSelectList = new SelectList(selectListUnit, "Value", "Text");
            ViewBag.Unit = unitSelectList;
        }

        public ActionResult Target(int? id)
        {
            ClearStatic();
            if (id == null)
            {
                return HttpNotFound();
            }
            _SceneView = "Target";
            _IndicatorID = id;
            Indicator indicator = db.Indicators.Find(id);
            indicator.ImportantIndicatorTargetMeasuerments = indicator.ImportantIndicatorTargetMeasuerments.Where(s => s.isDelete == false).ToList();
            if (indicator == null)
            {
                return HttpNotFound();
            }
            if (indicator.PredictOwners.Count == 0)
            {
                PredictOwner predicOwner = new PredictOwner();
                predicOwner.IndicatorID = indicator.ID;
                predicOwner.isDelete = false;
                indicator.PredictOwners.Add(predicOwner);
            }
            if (indicator.ImportantIndicatorTargetMeasuerments.Count > 0)
            {
                int Pointer = 0;
                indicator = ConfigureTargetDB(indicator);
                foreach (var i in indicator.IndicatorXIndicatorTypes)
                {
                    indicator.ImportantIndicatorTargetMeasuerments.ToList()[Pointer].IsDisplay = true;
                    if (i.isCheck == false)
                    {
                        indicator.ImportantIndicatorTargetMeasuerments.ToList()[Pointer].IsUnCheck = true;
                    }
                    Pointer++;
                }
            }
            else
            {
                List<ImportantIndicatorTargetMeasuerment> importants = new List<ImportantIndicatorTargetMeasuerment>();
                foreach (var i in indicator.IndicatorXIndicatorTypes)
                {
                    var TargetTypeDisplay = InitImportantTargetMeasuerment();
                    TargetTypeDisplay.IndicatorTypeID = i.IndicatorTypeID;
                    TargetTypeDisplay.IsDisplay = true;
                    TargetTypeDisplay.IndicatorID = indicator.ID;

                    if (i.isCheck == false)
                    {
                        TargetTypeDisplay.IsUnCheck = true;
                    }
                    importants.Add(TargetTypeDisplay);

                }
                importants.AddRange(indicator.ImportantIndicatorTargetMeasuerments.ToList());
                indicator.ImportantIndicatorTargetMeasuerments = importants;
            }

            selectYear();
            InitDataStatic(indicator);
            indicatorType(indicator);
            indicatorUnit(indicator);
            ViewbagData();
            return View(_SceneView, indicator);
        }


        private ImportantIndicatorTargetMeasuerment InitImportantTargetMeasuerment()
        {
            ImportantIndicatorTargetMeasuerment importantIndicatorTargetMeasurement = new ImportantIndicatorTargetMeasuerment();

            List<ImportantIndicatorTargetMeasuerment> targetList = new List<ImportantIndicatorTargetMeasuerment>();
            int range = 5;

            for (int i = 0; i < range; i++)
            {
                ImportantIndicatorTargetMeasuerment importantIndicatorTargetMeasurement1 = new ImportantIndicatorTargetMeasuerment();
                importantIndicatorTargetMeasurement1.IndicatorLevel = i + 1;
                targetList.Add(importantIndicatorTargetMeasurement1);
            }
            importantIndicatorTargetMeasurement.SubTarget = targetList;
            importantIndicatorTargetMeasurement.isDelete = false;
            return importantIndicatorTargetMeasurement;
        }

        [HttpPost]
        [ActionName("Predict")]
        public ActionResult AddpredictOwner(Indicator indicator)
        {
            PredictOwner predictOwner = new PredictOwner();
            predictOwner.CreateDate = DateTime.Now;
            predictOwner.UpdateDate = DateTime.Now;
            predictOwner.isDelete = false;

            indicator.PredictOwners.Add(predictOwner);
            ViewbagData();
            selectYear();
            return View("Target", indicator);
        }

        [HttpPost]
        public ActionResult AddTargetMeasurement(Indicator indicator)
        {
            // เพิ่ม ImportantIndicatorTargetMeasuerment ใหม่ใน Indicator
            indicator.ImportantIndicatorTargetMeasuerments.Add(CreateImportantIndicatorTargetMeasurement(indicator));

            // เตรียมข้อมูลที่ใช้ใน View
            ViewbagData();
            indicatorType(indicator);
            indicatorUnit(indicator);
            selectYear();

            // ส่ง Indicator ไปที่ View "Target"
            return View("Target", indicator);
        }

        private ImportantIndicatorTargetMeasuerment CreateImportantIndicatorTargetMeasurement(Indicator indicator)
        {
            // สร้าง ImportantIndicatorTargetMeasuerment ใหม่
            ImportantIndicatorTargetMeasuerment importantIndicatorTargetMeasurement = new ImportantIndicatorTargetMeasuerment(); //สร้างมาเพื่อไว้สร้าง subtarget
            List<ImportantIndicatorTargetMeasuerment> ListImportant = new List<ImportantIndicatorTargetMeasuerment>(); //สร้างมาเพื่อไว้เก็บข้อมูลจากการสร้าง subtarget

            int range = 5;
            for (int i = 0; i < range; i++)
            {
                // สร้าง SubTarget ใหม่
                ImportantIndicatorTargetMeasuerment importantIndicatorTargetMeasurement1 = new ImportantIndicatorTargetMeasuerment();
                importantIndicatorTargetMeasurement1.IndicatorLevel = i + 1;

                // กำหนดค่าใน ImportantIndicatorTargetMeasuerment แต่ละระดับ
                foreach (var a in indicator.ImportantIndicatorTargetMeasuerments)
                {
                    a.IndicatorTypeID = indicator.ID;
                    a.UpdateDate = DateTime.Now;
                    a.isDelete = false;
                }
                ListImportant.Add(importantIndicatorTargetMeasurement1);
            }

            // กำหนด SubTarget ใน ImportantIndicatorTargetMeasuerment
            importantIndicatorTargetMeasurement.SubTarget = ListImportant;

            return importantIndicatorTargetMeasurement;
        }


        [HttpPost]
        public ActionResult Target(Indicator indicator)
        {
            foreach (var i in indicator.PredictOwners)
            {
                i.UpdateDate = DateTime.Now;
                if (i.ID == 0)
                {
                    i.IndicatorID = indicator.ID;
                    i.CreateDate = DateTime.Now;
                    db.PredictOwners.Add(i);
                }
                else
                {
                    db.Entry(i).State = EntityState.Modified;
                }
            }

            foreach (var i in indicator.ImportantIndicatorTargetMeasuerments)
            {
                if (i.isDelete == true)
                {
                    foreach (var j in i.SubTarget)
                    {

                        db.Entry(j).State = EntityState.Deleted;
                        j.UpdateDate = DateTime.Now;
                        j.IndicatorID = i.IndicatorTypeID;
                        j.IndicatorID = i.IndicatorUnitID;
                        j.IndicatorID = i.IndicatorUnitID;
                        j.isDelete = true;

                    }
                }
                else
                {
                    foreach (var j in i.SubTarget)
                    {
                        j.UpdateDate = DateTime.Now;
                        if (j.ID == 0)
                        {
                            // Add new subtarget
                            j.IndicatorTypeID = i.IndicatorTypeID;
                            j.IndicatorUnitID = i.IndicatorUnitID;
                            j.CreateDate = DateTime.Now;
                            j.IndicatorID = indicator.ID;
                            j.isDelete = false;
                            db.ImportantIndicatorTargetMeasuerments.Add(j);
                        }
                        else
                        {
                            db.Entry(j).State = EntityState.Modified;
                            // Modify existing subtarget
                            j.IndicatorTypeID = i.IndicatorTypeID;
                            j.IndicatorUnitID = i.IndicatorUnitID;


                        }
                    }
                }
            }

            db.SaveChanges();
            return RedirectToAction("Index", new { id = indicator.CreateDate.Value.Year });
        }


        public ActionResult Result(int? id)
        
        {
            // ล้างค่าที่ถูกเก็บไว้ในตัวแปร static (ถ้ามี)
            ClearStatic();

            if (id == null)
            {
                // ถ้า id เป็น null ให้แสดง HTTP 404 Not Found
                return HttpNotFound();
            }

            // กำหนด _IndicatorID ให้เท่ากับ id ที่รับเข้ามา
            _IndicatorID = id;

            // ค้นหา Indicator จากฐานข้อมูล
            Indicator indicator = db.Indicators.Find(id);

            // กรณีไม่พบ Indicator ในฐานข้อมูล ให้แสดง HTTP 404 Not Found
            if (indicator == null)
            {
                return HttpNotFound();
            }

            // กรอง ImportantIndicatorTargetMeasurements ที่ไม่ถูกลบ
            indicator.ImportantIndicatorTargetMeasuerments = indicator.ImportantIndicatorTargetMeasuerments
                .Where(s => s.isDelete == false)
                .ToList();

            // ปรับแต่งฐานข้อมูลของ Target
            indicator = ConfigureTargetDB(indicator);

            // กรณีที่ ImportantIndicatorResultMeasurements เป็น 0
            if (indicator.ImportantIndicatorResultMeasurements.Count == 0)
            {
                // สร้าง ImportantIndicatorResultMeasurements และ ForecastPeriods สำหรับแต่ละ PMQY
                foreach (var i in db.PeriodMonthOrQuaterOrYears)
                {
                    ImportantIndicatorResultMeasurement measurement = new ImportantIndicatorResultMeasurement();

                    measurement.Year = DateTime.Now.Year;
                    measurement.PeriodMountOrQauterOrYearID = i.ID;
                    measurement.IndicatorID = indicator.ID;
                    measurement.PMQY = i.Period;

                    indicator.ImportantIndicatorResultMeasurements.Add(measurement);
                }

                // กำหนดจำนวน ForecastPeriods ตาม PMQY
                Dictionary<string, int> PMQYRangeDict = new Dictionary<string, int>
                {
                    {"รายเดือน", 12},
                    {"รายไตรมาส", 4},
                    {"รายปี", 2}
                };

                foreach (var i in indicator.ImportantIndicatorResultMeasurements)
                {
                    if (PMQYRangeDict.ContainsKey(i.PMQY))
                    {
                        int range = PMQYRangeDict[i.PMQY];
                        for (int j = 0; j < range; j++)
                        {
                            i.ForecastPeriods.Add(new ForecastPeriod());
                        }
                    }
                }

                // กำหนดค่าเริ่มต้นสำหรับ Indicator
                indicator.PeriodSelected_Index = 0;
                indicator.ForeCastPeriodSelected_Index = 0;

                // สร้าง ForecastPeriodToolAndMethods สำหรับแต่ละ ForecastPeriod
                foreach (var i in indicator.ImportantIndicatorResultMeasurements)
                {
                    foreach (var a in i.ForecastPeriods)
                    {
                        foreach (var b in db.ForecastTools)
                        {
                            if (!string.IsNullOrEmpty(b.ForecastTool1))
                            {
                                a.ForecastPeriodToolAndMethods.Add(new ForecastPeriodToolAndMethod
                                {
                                    ToolName = b.ForecastTool1,
                                    ForecastToolID = b.ID,
                                    ForecastPeriodID = i.ID,
                                });
                            }
                            else
                            {
                                // จัดการกรณีที่ ForecastTool1 เป็น empty
                            }
                        }
                        a.IsSelect = false;
                        a.ForecastPeriodToolAndMethods.Add(new ForecastPeriodToolAndMethod
                        {
                            ToolName = "อื่นๆ โปรดระบุ",
                            IsOtherTool = true
                        });
                    }
                }

                // เตรียมข้อมูล Remark และ Result Value และ Real Value
                SetResultRemark(indicator);
                SetResultValueAndRealValue(indicator);

                // เตรียมข้อมูลที่ใช้ใน View
                ViewbagData();
            }
            else
            {
                // กรณีที่ ImportantIndicatorResultMeasurements มีข้อมูลอยู่แล้ว
                var PMQYlist = db.PeriodMonthOrQuaterOrYears.ToList();
                var Toollist = db.ForecastTools.ToList();
                int PMQYIndex = 0;

                // กำหนดค่า PMQY ให้กับแต่ละ ImportantIndicatorResultMeasurement
                foreach (var y in indicator.ImportantIndicatorResultMeasurements)
                {
                    // Check if PMQYIndex is within bounds
                    if (PMQYIndex < PMQYlist.Count)
                    {
                        y.PMQY = PMQYlist[PMQYIndex].Period;
                        PMQYIndex++;

                        // กำหนดค่า ToolName ให้กับแต่ละ ForecastPeriodToolAndMethod
                        foreach (var a in y.ForecastPeriods)
                        {
                            int ToolIndex = 0;
                            foreach (var b in a.ForecastPeriodToolAndMethods)
                            {
                                if (ToolIndex + 1 > Toollist.Count)
                                {
                                    b.ToolName = "อื่นๆ โปรดระบุ";
                                    b.IsOtherTool = true;
                                }
                                else
                                {
                                    b.ToolName = Toollist[ToolIndex].ForecastTool1;
                                    ToolIndex++;
                                }
                            }
                        }
                    }
                }
            }


            // กำหนดค่าเริ่มต้นสำหรับ Static Data
            InitDataStatic(indicator);

            // กำหนดค่า IsSelect สำหรับ ForecastPeriods แรก
            indicator.ImportantIndicatorResultMeasurements.First().ForecastPeriods.First().IsSelect = true;

            // เตรียมข้อมูลที่ใช้ใน View
            ViewbagData();
            ViewbagPeriodDict(indicator);
            selectYear();

            // ส่ง Indicator ไปที่ View "Result"
            return View("Result", indicator);
        }

        private void SetResultValueAndRealValue(Indicator indicator)
        {
            foreach (var i in indicator.ImportantIndicatorResultMeasurements)
            {
                foreach (var a in i.ForecastPeriods)
                {
                    for (int p = 0; p < indicator.IndicatorUnits.Count; p++)
                    {

                        a.ForecastValueAndRealValues.Add(new ForecastValueAndRealValue()
                        {
                            ForecastPeriodID = a.ID,
                            unitIndex = p,
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now,
                            IsDelete = false,
                            IsLastDelete = false,
                        });
                    }
                }
            }
        }

        private void SetResultRemark(Indicator indicator)
        {
            foreach (var i in indicator.ImportantIndicatorResultMeasurements)
            {
                foreach (var a in i.ForecastPeriods)
                {
                    a.ForecastPeriodResultRemarks.Add(new ForecastPeriodResultRemark()
                    {
                        ForecastPeriodID = a.ID,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        IsDelete = false,
                        IsLastDelete = false,
                    });
                }

            }
        }

        private void SetIsSelect(Indicator indicator)
        {
            foreach (var i in indicator.ImportantIndicatorResultMeasurements)
            {
                foreach (var a in i.ForecastPeriods)
                {
                    a.IsSelect = false;
                }
            }
        }

        [HttpPost]
        public ActionResult ChangePeriod(Indicator indicator)
        {
            SetIsSelect(indicator);

            indicator.ImportantIndicatorResultMeasurements.ToList()[indicator.PeriodSelected_Index].ForecastPeriods.First().IsSelect = true;
            selectYear();
            ViewbagPeriodDict(indicator);
            ViewbagData();
            return View("Result", indicator);
        }
        [HttpPost]
        public ActionResult ChangeMonthQuarterHailfYear(Indicator indicator)
        {
            SetIsSelect(indicator);

            indicator.ImportantIndicatorResultMeasurements.ToList()[indicator.PeriodSelected_Index].ForecastPeriods.ToList()[indicator.ForeCastPeriodSelected_Index].IsSelect = true;
            selectYear();
            ViewbagPeriodDict(indicator);
            ViewbagData();
            return View("Result", indicator);
        }

        [HttpPost]
        public ActionResult ChangeFile(Indicator indicator)
        {
            return View("Result", indicator);
        }

        //public ActionResult Report(int? SOEPlantID)
        //{
        //    return File();
        //}
        [HttpPost]
        public ActionResult Result(Indicator indicators)
        {

            // Save the ImportantIndicatorResultMeasurements
            foreach (var resultMeasurement in indicators.ImportantIndicatorResultMeasurements)
            {
                resultMeasurement.Year = DateTime.Now.Year;
                resultMeasurement.IsLastDelete = false;
                resultMeasurement.IsDelete = false;
                if (resultMeasurement.ID == 0)
                {
                    resultMeasurement.CreateDate = DateTime.Now;
                    db.ImportantIndicatorResultMeasurements.Add(resultMeasurement);
                }
                else
                {
                    resultMeasurement.UpdateDate = DateTime.Now;
                    db.Entry(resultMeasurement).State = EntityState.Modified;
                }
                foreach (var item in resultMeasurement.ForecastPeriods)
                {
                    var c = item.ForecastPeriodCompetitorValues;
                    var a = item.ForecastPeriodToolAndMethods;
                    var d = item.ForecastPeriodResultRemarks;
                    var b = item.ForecastValueAndRealValues;
                    item.ForecastPeriodCompetitorValues = null;
                    item.ForecastPeriodToolAndMethods = null;
                    item.ForecastPeriodResultRemarks = null;
                    item.ForecastValueAndRealValues = null;

                    item.MonthOrQuaterOrYear = resultMeasurement.PMQY;
                    item.IsDelete = false;
                    item.IsLastDelete = false;
                    item.CreateDate = DateTime.Now;
                    if (item.ID == 0)
                    {
                        item.ImportantIndicatorResultMeasurementID = resultMeasurement.ID;
                        db.ForecastPeriods.Add(item);
                    }
                    else
                    {
                        item.UpdateDate = DateTime.Now;
                        db.Entry(item).State = EntityState.Modified;
                    }
                    db.SaveChanges();



                    foreach (var ToolItem in a)
                    {
                        if (ToolItem.ID == 0) { ToolItem.ForecastPeriodID = item.ID; db.ForecastPeriodToolAndMethods.Add(ToolItem); } else { ToolItem.ForecastPeriodID = item.ID; db.Entry(ToolItem).State = EntityState.Modified; }
                    }
                    foreach (var ValueItem in b)
                    {
                        if (ValueItem.ID == 0) { ValueItem.ForecastPeriodID = item.ID; db.ForecastValueAndRealValues.Add(ValueItem); } else { ValueItem.ForecastPeriodID = item.ID; db.Entry(ValueItem).State = EntityState.Modified; }
                    }
                    foreach (var CompeItem in c)
                    {
                        if (CompeItem.ID == 0) { CompeItem.ForecastPeriodID = item.ID; db.ForecastPeriodCompetitorValues.Add(CompeItem); } else { CompeItem.ForecastPeriodID = item.ID; db.Entry(CompeItem).State = EntityState.Modified; }
                    }
                    foreach (var Remarkitem in d)
                    {
                        if (Remarkitem.ID == 0) { Remarkitem.ForecastPeriodID = item.ID; db.ForecastPeriodResultRemarks.Add(Remarkitem); } else { Remarkitem.ForecastPeriodID = item.ID; db.Entry(Remarkitem).State = EntityState.Modified; }
                    }

                    //if (d.Any() && d.First().ListFilePeriodDocs.Any()) { ActionSaveFile(d.First().ListFilePeriodDocs, d.First().ID, 1); }
                    //if (d.Any() && d.First().ListFileAnalysis.Any()) { ActionSaveFile(d.First().ListFileActionPlan, d.First().ID, 2); }
                    //if (d.Any() && d.First().ListFileActionPlan.Any()) { ActionSaveFile(d.First().ListFileActionPlan, d.First().ID, 3); }
                    if (d.First().ListFilePeriodDocs != null) { ActionSaveFile(d.First().ListFilePeriodDocs, d.First().ID, 1); }
                    if (d.First().ListFileAnalysis != null) { ActionSaveFile(d.First().ListFileAnalysis, d.First().ID, 2); }
                    if (d.First().ListFileActionPlan != null) { ActionSaveFile(d.First().ListFileActionPlan, d.First().ID, 3); }
                    db.SaveChanges();


                }
            }

            return RedirectToAction("Index");
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public void selectYear()
        {
            int currentYear = DateTime.Now.Year;
            int currentMonth = DateTime.Now.Month;

            List<SelectListItem> yearList = new List<SelectListItem>();

            bool isEndOfYear = currentMonth >= 10 && currentMonth <= 12;

            int startYear = currentYear;
            int endYear = isEndOfYear ? currentYear + 1 : currentYear;

            for (int i = startYear - 10; i <= endYear + 10; i++)
            {
                SelectListItem item = new SelectListItem
                {
                    Text = i.ToString(),
                    Value = i.ToString()
                };

                if ((isEndOfYear && i == currentYear + 1) || (!isEndOfYear && i == currentYear))
                {
                    item.Selected = true;
                }

                yearList.Add(item);
            }

            ViewBag.YearList = yearList; // Assign the List<SelectListItem> directly to ViewBag.YearList
        }
        private void ViewbagData()
        {
            var DivisionList = new List<SelectListItem>();
            foreach (var i in db.Owners)
            {
                DivisionList.Add(new SelectListItem { Text = i.Division });
            }
            ViewBag.DivisionList = DivisionList;
            ViewBag.IndicatorTypeDropdownList = _IndicatorTypeDropdownList;
            ViewBag.IndicatorUnitDropdownList = _IndicatorUnitDropdownList;
            ViewBag.PredicOwnerDropdownList = _predicDropdownList;
            Dictionary<string, int> PMYID_dict = new Dictionary<string, int>();
            foreach (var i in db.PeriodMonthOrQuaterOrYears.ToList())
            {
                PMYID_dict.Add(i.Period, i.ID);
            }
            ViewBag.PMYID_dict = PMYID_dict;

        }

        private void ClearStatic()
        {
            _IndicatorID = null;
            _IndicatorTypeDropdownList.Clear();
            _IndicatorUnitDropdownList.Clear();
            _PeriodDict.Clear();
        }
        [HttpPost]
        [ActionName("PredictDel")]

        public ActionResult OwnerDelete(Indicator indicator)
        {
            ModelState.Clear();
            indicator.PredictOwners = FindFormDelete(indicator.PredictOwners);
            selectYear();
            indicatorType(indicator);
            indicatorUnit(indicator);
            ViewbagData();
            return View("Target", indicator);
        }
        private ICollection<PredictOwner> FindFormDelete(ICollection<PredictOwner> predictOwners)
        {
            foreach (var d in predictOwners)
            {
                if (d.isPredictDelete == true)
                {
                    if (d.ID == 0)
                    {
                        var io = predictOwners.ToList();
                        io.RemoveAt(io.IndexOf(d));
                        predictOwners = io;
                    }

                    else
                    {
                        d.isDelete = true;
                    }
                }
            }
            return predictOwners;
        }
        public ActionResult DeleteTarget(Indicator indicator)
        {
            ModelState.Clear();
            foreach (var i in indicator.ImportantIndicatorTargetMeasuerments)
            {
                if (i.isDeleteImportant)
                {
                    if (i.ID == 0)
                    {
                        var tempList = indicator.ImportantIndicatorTargetMeasuerments.ToList();
                        tempList.Remove(i);
                        indicator.ImportantIndicatorTargetMeasuerments = tempList;
                    }
                    else
                    {
                        i.isDelete = true;
                    }
                }
            }
            selectYear();
            indicatorType(indicator);
            indicatorUnit(indicator);
            ViewbagData();
            return View("Target", indicator);
        }
        [HttpPost]
        public ActionResult AddCompetitorValue(Indicator indicator)
        {
            foreach (var c in indicator.ImportantIndicatorResultMeasurements)
            {
                foreach (var selectedPeriod in c.ForecastPeriods)
                {
                    if (selectedPeriod.IsAddCompetitor)
                    {
                        if (selectedPeriod.ForecastPeriodCompetitorValues == null)
                        {
                            selectedPeriod.ForecastPeriodCompetitorValues = new List<ForecastPeriodCompetitorValue>();
                        }

                        ForecastPeriodCompetitorValue competitorValue = new ForecastPeriodCompetitorValue
                        {
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now,
                            IsDelete = false,
                            IsLastDelete = false,
                            ForecastPeriodID = selectedPeriod.ID,
                            ForecastPeriod = selectedPeriod
                        };

                        selectedPeriod.ForecastPeriodCompetitorValues.Add(competitorValue);
                    }
                }
            }

            ViewbagData();
            ViewbagPeriodDict(indicator);
            selectYear();
            indicatorType(indicator);
            indicatorUnit(indicator);
            //ModelState.Clear();
            return View("Result", indicator);
        }
        public ActionResult DeleteCompetitorValue(Indicator indicator)
        {
            ModelState.Clear();
            foreach (var item in indicator.ImportantIndicatorResultMeasurements.First().ForecastPeriods)
            {
                var competitorValuesToRemove = item.ForecastPeriodCompetitorValues
                    .Where(item2 => item2.IsDeleteCompetitorValue && item2.ID == 0)
                    .ToList();

                foreach (var item2 in competitorValuesToRemove)
                {
                    item.ForecastPeriodCompetitorValues.Remove(item2);
                }

                foreach (var item2 in item.ForecastPeriodCompetitorValues.Where(item2 => item2.IsDeleteCompetitorValue && item2.ID != 0))
                {
                    item2.IsDelete = true;
                }
            }
            ViewbagData();
            ViewbagPeriodDict(indicator);
            selectYear();
            indicatorType(indicator);
            indicatorUnit(indicator);
            return View("Result", indicator);
        }
        private Indicator ConfigureTargetDB(Indicator indicator)
        {
            if (indicator.ImportantIndicatorTargetMeasuerments.Count == 0) return indicator;
            {
                int row = indicator.ImportantIndicatorTargetMeasuerments.Count / 5;
                int currentPoint = 0;
                int Range = 0;
                List<ImportantIndicatorTargetMeasuerment> importantIndicatorTargetMeasuerments = new List<ImportantIndicatorTargetMeasuerment>();
                for (int i = 0; i < row; i++)
                {
                    ImportantIndicatorTargetMeasuerment important = new ImportantIndicatorTargetMeasuerment();
                    List<ImportantIndicatorTargetMeasuerment> importantList = new List<ImportantIndicatorTargetMeasuerment>();
                    Range += 5;

                    for (int j = currentPoint; j < Range; j++)
                    {
                        importantList.Add(indicator.ImportantIndicatorTargetMeasuerments.ToList()[j]);
                    }

                    important.SubTarget = importantList;

                    important.ID = important.SubTarget[0].ID;
                    important.IndicatorID = important.SubTarget[0].IndicatorID;
                    important.IndicatorUnitID = important.SubTarget[0].IndicatorUnitID;
                    important.IndicatorTypeID = important.SubTarget[0].IndicatorTypeID;
                    important.isDelete = false;

                    importantIndicatorTargetMeasuerments.Add(important);
                    currentPoint = Range;
                }
                indicator.ImportantIndicatorTargetMeasuerments = importantIndicatorTargetMeasuerments;
                return indicator;
            }
        }

        private void InitDataStatic(Indicator indicator)
        {
            foreach (var i in indicator.IndicatorXIndicatorTypes)
            {
                if (i.isCheck)
                {
                    SelectListItem selectListItem = new SelectListItem() { Text = i.IndicatorType.IndicatorType1, Value = i.IndicatorTypeID.ToString() };
                    _IndicatorTypeDropdownList.Add(selectListItem);
                }
            }

            foreach (var i in indicator.IndicatorUnits)
            {
                SelectListItem selectListItem = new SelectListItem() { Text = i.Unit, Value = i.ID.ToString() };
                _IndicatorUnitDropdownList.Add(selectListItem);
            }
            _PeriodDict.Add(0, MountDict);
            _PeriodDict.Add(1, QuaterDict);
            _PeriodDict.Add(2, YearDict);
        }

        private void ViewbagPeriodDict(Indicator indicator)
        {
            ViewBag.PeriodDict = _PeriodDict[indicator.PeriodSelected_Index];
        }

        private void ActionSaveFile(List<HttpPostedFileBase> listFile, int ID, int runnumber)
        {
            foreach (var fileitem in listFile.Where(item => item != null))
            {
                string FileName = DateTime.UtcNow.ToString("HHmmss-ddMMyyyy") + "_" + Path.GetFileName(fileitem.FileName);
                string UploadPath = Path.Combine(Server.MapPath(FileImportDirectory), FileName);

                if (!Directory.Exists(Server.MapPath(FileImportDirectory)))
                {
                    Directory.CreateDirectory(Server.MapPath(FileImportDirectory));
                }

                using (var fileStream = new FileStream(UploadPath, FileMode.Create))
                {
                    fileitem.InputStream.CopyTo(fileStream);
                }

                ForecastFilePath filePath = new ForecastFilePath
                {
                    UploadDate = DateTime.UtcNow,
                    IsDelete = false,
                    IsLastDelete = false,
                    FileName = FileName,
                    FilePath = Path.Combine(FileImportDirectory, FileName)
                };

                db.ForecastFilePaths.Add(filePath);
                db.SaveChanges();

                AddFileEntityToDatabase(runnumber, ID, filePath);
            }
        }

        private void AddFileEntityToDatabase(int runnumber, int ID, ForecastFilePath filePath)
        {
            switch (runnumber)
            {
                case 1:
                    ForecastPeriodDocFile docFile = new ForecastPeriodDocFile();
                    docFile.ForecastPeriodResultRemarkID = ID;
                    docFile.FilePathID = filePath.ID;
                    docFile.CreateDate = DateTime.UtcNow;
                    docFile.UpdateDate = DateTime.UtcNow;
                    docFile.IsDelete = false;
                    docFile.IsLastDelete = false;
                    db.ForecastPeriodDocFiles.Add(docFile);
                    break;

                case 2:
                    ForecastAnalysisResultFile analysis = new ForecastAnalysisResultFile();
                    analysis.ForecastPeriodResultRemarkID = ID;
                    analysis.FilePathID = filePath.ID;
                    analysis.CreateDate = DateTime.UtcNow;
                    analysis.UpdateDate = DateTime.UtcNow;
                    analysis.IsDelete = false;
                    analysis.IsLastDelete = false;
                    db.ForecastAnalysisResultFiles.Add(analysis);
                    break;

                case 3:
                    ForecastChangeActionPlanFile changeAction = new ForecastChangeActionPlanFile();
                    changeAction.ForecastPeriodResultRemarkID = ID;
                    changeAction.FilePathID = filePath.ID;
                    changeAction.CreateDate = DateTime.UtcNow;
                    changeAction.UpdateDate = DateTime.UtcNow;
                    changeAction.IsDelete = false;
                    changeAction.IsLastDelete = false;
                    db.ForecastChangeActionPlanFiles.Add(changeAction);
                    break;

                default:
                    throw new ArgumentException("Invalid run number");
            }
        }

        private const string FileImportDirectory = "~/FileImport/";

    }
}
