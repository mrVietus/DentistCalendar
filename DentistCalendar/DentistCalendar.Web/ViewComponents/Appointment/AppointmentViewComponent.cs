using DentistCalendar.Web.Models.Appointment;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DentistCalendar.Web.ViewComponents.Appointment
{
    public class AppointmentViewComponent : ViewComponent
    {
        private readonly string componentView = "~/Views/Shared/Components/Appointment/ScheduleAppointment.cshtml";

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(componentView, new ScheduleAppointmentModel());
        }
    }
}
