using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DHTMLX.Scheduler;
using DHTMLX.Common;
using DHTMLX.Scheduler.Data;
using DHTMLX.Scheduler.Controls;

using SimpleScheduler.Models;
namespace SimpleScheduler.Controllers
{
    public class CalendarController : Controller
    {
        public ActionResult Index(FormCollection form)
        {
            var scheduler = new DHXScheduler(this);

            scheduler.Skin = Request.QueryString.Count > 0 ? (DHXScheduler.Skins)Enum.Parse(typeof(DHXScheduler.Skins), Request.QueryString["id"]) : DHXScheduler.Skins.Terrace;
            
            scheduler.LoadData = true;
            scheduler.EnableDataprocessor = true;
            scheduler.Extensions.Add(SchedulerExtensions.Extension.Recurring);
            //scheduler.Lightbox.Add(new LightboxRecurringBlock("rec_type", "Repeat event"));
            return View(scheduler);
        }

        public ContentResult Data()
        {
            var data = new SchedulerAjaxData(new SampleDataContext().Events);
                    
            return (ContentResult)data;
        }

        public ContentResult Save(int? id, FormCollection actionValues)
        {
            var action = new DataAction(actionValues);
            
            try
            {
                var changedEvent = (Event)DHXEventsHelper.Bind(typeof(Event), actionValues);
                var data = new SampleDataContext();
     

                switch (action.Type)
                {
                    case DataActionTypes.Insert:
                        data.Events.InsertOnSubmit(changedEvent);
                        break;
                    case DataActionTypes.Delete:
                        changedEvent = data.Events.SingleOrDefault(e => e.id == action.SourceId);
                        data.Events.DeleteOnSubmit(changedEvent);
                        break;
                    default:// "update"                          
                        var eventToUpdate = data.Events.SingleOrDefault(e => e.id == action.SourceId);
                        DHXEventsHelper.Update(eventToUpdate, changedEvent, new List<string>() { "id" });
                        break;
                }
                data.SubmitChanges();
                action.TargetId = changedEvent.id;
            }
            catch
            {
                action.Type = DataActionTypes.Error;
            }
            return (ContentResult)new AjaxSaveResponse(action);
        }
    }
}

