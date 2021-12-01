using System;
using System.Collections.Generic;

namespace EClinic.Models.ViewModels
{
    public class MeetingManageViewModel
    {
        public List<MettingForManagingListViewModel> ModelsList { get; set; }
        public int? doctorTypeId { get; set; }
        public string doctorName { get; set; }
        public DateTime? date { get; set; }
    }
}
