using System;
using System.Collections.ObjectModel;

namespace PicturesSynchroniser.Models
{
    public class ResultModel
    {
        public bool IsSuccessful { get; set; }

        public int RenamedPicturesCount { get; set; }

        public string ErrorMessage { get; set; }

        public ObservableCollection<PictureModel> PicturesForRollback { get; set; }
    }
}