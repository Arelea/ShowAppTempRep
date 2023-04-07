using System;

namespace AppNov14.Helpers
{
    public static class BatchHistoryTexts
    {
        public static string GetTextByAction(int actionTypeId, string name = null, string secondName = null)
        {
            if (actionTypeId == BatchHistoryActions.CreateAction)
            {
                return $"Начато производство партии «{name}».";
            }
            else if (actionTypeId == BatchHistoryActions.CompleteAction)
            {
                return $"Производство партии «{name}» завершено.";
            }
            else if (actionTypeId == BatchHistoryActions.SellAction)
            {
                return $"Партия «{name}» отгружена покупателю со склада.";
            }
            else if (actionTypeId == BatchHistoryActions.ReturnFromCustomerAction)
            {
                return $"Партия «{name}» возвращена на склад от покупателя.";
            }
            else if (actionTypeId == BatchHistoryActions.MergeToOtherAction)
            {
                return $"Партия «{name}» добавлена в партию «{secondName}».";
            }
            else if (actionTypeId == BatchHistoryActions.MergingFromOtherAction)
            {
                return $"В пратию «{secondName}» была добавлена партия «{name}».";
            }
            else if (actionTypeId == BatchHistoryActions.UtilizationAction)
            {
                return $"Партия «{name}» была утилизирована.";
            }

            return null;
        }
    }

    public static class BatchHistoryActions
    {
        public static int CreateAction = 1;

        public static int CompleteAction = 2;

        public static int SellAction = 3;

        public static int ReturnFromCustomerAction = 4;

        public static int MergeToOtherAction = 5;

        public static int MergingFromOtherAction = 6;

        public static int UtilizationAction = 7;

        public static int NoneAction = 8;

        public static string GetActionName(int actionTypeId)
        {
            if (actionTypeId == BatchHistoryActions.CreateAction)
            {
                return "Создание";
            }
            else if (actionTypeId == BatchHistoryActions.CompleteAction)
            {
                return "Производство завершено, отправлена на склад";
            }
            else if (actionTypeId == BatchHistoryActions.SellAction)
            {
                return "Отгрузка";
            }
            else if (actionTypeId == BatchHistoryActions.ReturnFromCustomerAction)
            {
                return "Возврат от покупателя";
            }
            else if (actionTypeId == BatchHistoryActions.MergeToOtherAction)
            {
                return "Добавление в другую партию";
            }
            else if (actionTypeId == BatchHistoryActions.MergingFromOtherAction)
            {
                return "Принятие другой партии";
            }
            else if (actionTypeId == BatchHistoryActions.UtilizationAction)
            {
                return "Утилизация";
            }
            else if (actionTypeId == BatchHistoryActions.NoneAction)
            {
                return "Нет данных";
            }

            return null;
        }
    }

    public static class BatchHistoryOperations
    {
        public static int UsualOperation = 1;

        public static int EditOperation = 2;

        public static int InformationOperation = 3;

        public static string GetOperationName(int operationTypeId)
        {
            if (operationTypeId == BatchHistoryOperations.UsualOperation)
            {
                return "Действие";
            }
            else if (operationTypeId == BatchHistoryOperations.EditOperation)
            {
                return "Редактирование";
            }
            else if (operationTypeId == BatchHistoryOperations.InformationOperation)
            {
                return "Информация";
            }

            return null;
        }
    }
}