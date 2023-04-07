using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.WebPages.Html;
using AppNov14.Handlers.Interfaces;
using AppNov14.Web.ViewModels.Base;
using Microsoft.AspNetCore.Mvc;

namespace AppNov14.Web.Controllers
{
    public abstract class BaseController : Controller
    {
        private readonly IBaseDataHandler BaseDataHandler;

        public BaseController(IBaseDataHandler baseDataHandler)
        {
            this.BaseDataHandler = baseDataHandler;
        }

        protected T1 GetViewModel<T1>()
            where T1 : BaseViewModel, new()
        {
            var result = new T1();

            return result;
        }

        protected T1 CreateForm<T1>()
            where T1 : BaseForm, new()
        {
            var result = new T1();

            return result;
        }

        #region Other

        /// <summary>
        /// Возвращает селект лист
        /// </summary>
        /// <param name="items">Итемы для селектлистифая</param>
        /// <returns></returns>
        protected List<SelectListItem> SelectListifyItems(List<string> items)
        {
            var list = new List<SelectListItem>();
            if (items != null)
            {
                foreach (var item in items)
                {
                    list.Add(new SelectListItem { Value = item, Text = item });
                }
            }

            return list;
        }

        /// <summary>
        /// Возвращает селект лист
        /// </summary>
        /// <param name="items">Итемы для селектлистифая из словаря</param>
        /// <returns></returns>
        protected List<SelectListItem> SelectListifyItemsFromDictionary(Dictionary<int, string> items)
        {
            var list = new List<SelectListItem>();
            foreach (var item in items)
            {
                list.Add(new SelectListItem { Value = item.Key.ToString(), Text = item.Value });
            }

            return list;
        }

        /// <summary>
        /// Возвращает селект лист
        /// </summary>
        /// <param name="items">Итемы для селектлистифая из словаря</param>
        /// <returns></returns>
        protected List<SelectListItem> SelectListifyItemsFromDictionary(Dictionary<string, string> items)
        {
            var list = new List<SelectListItem>();
            foreach (var item in items)
            {
                list.Add(new SelectListItem { Value = item.Key.ToString(), Text = item.Value });
            }

            return list;
        }

        /// <summary>
        /// Возвращает селект лист
        /// </summary>
        /// <param name="items">Итемы для селектлистифая из словаря</param>
        /// <returns></returns>
        protected List<SelectListItem> SelectListifyItemsFromDictionaryWithNull(Dictionary<int, string> items, string placeholder = null)
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem { Value = null, Text = placeholder ?? string.Empty });

            foreach (var item in items)
            {
                list.Add(new SelectListItem { Value = item.Key.ToString(), Text = item.Value });
            }

            return list;
        }

        /// <summary>
        /// Возвращает селект лист с наллом
        /// </summary>
        /// <param name="items">Итемы для селектлистифая</param>
        /// <returns></returns>
        protected List<SelectListItem> SelectListifyItemsWithNull(List<string> items)
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem { Value = null, Text = string.Empty });
            if (items != null)
            {
                list.AddRange(items.Select(item => new SelectListItem
                {
                    Value = item,
                    Text = item,
                }));
            }

            return list;
        }

        #endregion
    }
}