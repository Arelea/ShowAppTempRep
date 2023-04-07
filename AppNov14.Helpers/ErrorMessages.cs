using System;

namespace AppNov14.Helpers
{
    public static class ErrorMessages
    {
        public const string WarehouseIdNotFound = "Не удалось найти такой материал на складе";

        public const string IndexIdNotFound = "Не удалось найти такой паспорт";

        public const string DocumentIdNotFound = "Не удалось найти такое наименование партии";

        public const string IndexIdNotFoundOrAlreadyExist = "Введенный индес уже существует";

        public const string ReplinishmentFailed = "Не удалось добавить сырье";

        public const string DocumentNumberAlreadyExist = "Введенный номер документа уже существует";

        public const string DocumentTypeAlreadyExist = "Введенное наименование партии уже существует";

        public const string CustomerNameAlreadyExist = "Введенное имя покупателя уже существует";

        public const string CustomerNotExist = "Покупатель не существует";

        public const string ZeroNotAllowed = "В поле количество должно быть число больше ноля";

        public const string WarehouseBelowZero = "Остаток на складе становится меньше ноля, введите меньшие количество. Текущий остаток: ";

        public const string IndexBelowZero = "Остаток по паспорту становится меньше ноля, введите меньшие количество. Текущий остаток: ";

        public const string RecordNotFound = "Запись не найдена";

        public const string IndexWasDeleted = "Не удалось редактировать данную запись, паспорт был удален.";

        public const string IndexWasUsedForWriteOff = "Данный паспорт уже использовался для списания, просмотрите все записи со списанием для данного паспорта.";

        public const string ReturnEditingNotAllowed = "Запрещено редактировать операции возрвата.";

        public const string NoReplinishForIndexBeforeWriteOff = "Операции пополнения не было перед этим паспортом.";

        public const string BatchNotFound = "Не удалось найти такой номер партии.";

        public const string BatchNameAlreadyExist = "Такой номер партии уже существует.";

        public const string BatchCreateDateException = "Дата завершения производства не может быть меньше даты начала производства!";





        public const string BatchPackageBelowZero = "Недопустимое количество баулов. Баулов на складе становится меньше 0!";

        public const string BatchQuantityBelowZero = "Недопустимое количество килограмм. Вес на складе становится меньше 0!";

        public const string BatchSoldDateException = "Дата отгрузки не может быть меньше даты заверешения производства!";

        public const string BatchAllMustBeNullException = "Не допустимое значение. При списании в ноль и баулы и масса должна быть равна 0!";


        public const string BatchAllMustBeZeroOrAboveException = "Не допустимое значение. Баул или вес не может быть равен нулю по отдельности!";


        public const string BatchReturnSumNotFound = "Не удалось найти сумму прошлых поставок";

        public const string BatchReturnSumPackageOverflow = "Количество баулов на возврат не должно быть больше суммые всех поставок данной партии клиенту";

        public const string BatchReturnSumQuantityOverflow = "Вес на возврат не должно быть больше суммые всех поставок данной партии клиенту";

        public const string BatchReturnDateOverflow = "Дата возврата не должна быть меньше даты отгрузки";

        public const string BatchReturnException = "Произошла ошибка при возврате";


        public const string BatchMergeException = "Произошла ошибка при добавлении";
    }
}