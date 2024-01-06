using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Prototype_PMS.Models;

namespace Prototype_PMS.Controllers
{
    public class SOEPlanIndicatorsController : Controller
    {
        private PMSEntities1 db = new PMSEntities1();

        public ActionResult Target(int? id, int startYear, int endYear)
        {
            StrategicObjective strategicObjective = db.StrategicObjectives.Find(id);

            if (strategicObjective != null)
            {
                ConfigureTargetDB(strategicObjective, startYear, endYear);
            }

            return View("Target", strategicObjective);
        }

        private void ConfigureTargetDB(StrategicObjective strategicObjective, int startYear, int endYear)
        {
            if (strategicObjective.Goals != null)
            {
                foreach (var goal in strategicObjective.Goals)
                {
                    if (goal.SOEPlanIndicators != null && goal.SOEPlanIndicators.Any())
                    {
                        Indicator indicator = goal.SOEPlanIndicators.First().Indicator;

                        if (indicator.ImportantIndicatorTargetMeasuerments.Count == 0) continue;

                        int row = indicator.ImportantIndicatorTargetMeasuerments.Count / (endYear - startYear + 1);
                        int currentPoint = 0;
                        int Range = 0;
                        List<ImportantIndicatorTargetMeasuerment> importantIndicatorTargetMeasuerments = new List<ImportantIndicatorTargetMeasuerment>();

                        for (int i = 0; i < row; i++)
                        {
                            ImportantIndicatorTargetMeasuerment important = new ImportantIndicatorTargetMeasuerment();
                            List<ImportantIndicatorTargetMeasuerment> importantList = new List<ImportantIndicatorTargetMeasuerment>();
                            Range += (endYear - startYear + 1);

                            for (int j = currentPoint; j < Range; j++)
                            {
                                importantList.Add(indicator.ImportantIndicatorTargetMeasuerments.ElementAtOrDefault(j));
                            }

                            important.SubTarget = importantList;

                            important.ID = important.SubTarget?.FirstOrDefault()?.ID ?? 0;
                            important.IndicatorID = important.SubTarget?.FirstOrDefault()?.IndicatorID ?? 0;
                            important.IndicatorUnitID = important.SubTarget?.FirstOrDefault()?.IndicatorUnitID ?? 0;
                            important.isDelete = false;

                            importantIndicatorTargetMeasuerments.Add(important);
                            currentPoint = Range;
                        }

                        indicator.ImportantIndicatorTargetMeasuerments = importantIndicatorTargetMeasuerments;
                    }
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
