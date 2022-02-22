using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentEquipment.ClassHelper
{
    public class AppData
    {
        public static EF.RentEquipmentEntities Context { get; } = new EF.RentEquipmentEntities();
    }
}
