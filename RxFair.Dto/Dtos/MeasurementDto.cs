using System.ComponentModel.DataAnnotations;

namespace RxFair.Dto.Dtos
{
    public class MeasurementView : BaseModel
    {
        public string MeasurementTypeName { get; set; }

        public short MeasurementType { get; set; }

        public string MeasurementUnit { get; set; }

    }
}
