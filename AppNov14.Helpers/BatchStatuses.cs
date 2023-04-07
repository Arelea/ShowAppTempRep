using System;

namespace AppNov14.Helpers
{
    public static class BatchStatuses
    {
        public static int NoData = 1;

        public static int InManufacturingProcess = 2;

        public static int Completed = 3;

        public static int Empty = 4;

        public static string GetName(int statusId)
        {
            if (statusId == BatchStatuses.NoData)
            {
                return "Нет данных";
            }
            else if (statusId == BatchStatuses.InManufacturingProcess)
            {
                return "В процессе производства";
            }
            else if (statusId == BatchStatuses.Completed)
            {
                return "На складе";
            }
            else if (statusId == BatchStatuses.Empty)
            {
                return "Израсходована";
            }

            return null;
        }
    }
}