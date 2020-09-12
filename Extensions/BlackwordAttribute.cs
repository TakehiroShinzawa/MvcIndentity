using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;

namespace MvcIdentity.Extensions
{
    public class BlackwordAttribute : ValidationAttribute, IClientValidatable
    {
        private string _opts;

        public BlackwordAttribute()
        {
            string opts = "干物,乾物,違法,麻薬,毒";//web.configから取得でも良いねぇ
            this._opts = opts;
            this.ErrorMessage = "{0}には{1}を含むことはできません。";
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture,
                                 ErrorMessageString, name, this._opts);
        }

        public override bool IsValid(object value)
        {
            if (value == null) { return true; }

            string[] list = this._opts.Split(',');
            foreach (var data in list)
            {
                if (((string)value).Contains(data))
                {
                    return false;
                }
            }
            return true;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ValidationType = "blackword",
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName())
            };
            rule.ValidationParameters["opts"] = _opts;
            yield return rule;
        }
    }
}