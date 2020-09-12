using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.UI.WebControls;

namespace MvcIdentity.SiteHelper
{
    public static class SiteHelpers
    {
        //要素に対するビューヘルパー

        //ルビ要素
        public static IHtmlString Ruby(this HtmlHelper helper, string mainText, string rubyText)
        {
            return MvcHtmlString.Create("<ruby>" + mainText + "<rp>(</rp><rt>" + rubyText + "</rt><rp>)</rp></ruby>");
        }



        //image要素
        public static IHtmlString Image(this HtmlHelper helper, string src)
        {
            TagBuilder builder = new TagBuilder("img");
            builder.MergeAttribute("alt", "画像取得ミス");
            builder.MergeAttribute(
                "src", UrlHelper.GenerateContentUrl(src, helper.ViewContext.HttpContext));
            return MvcHtmlString.Create(
                builder.ToString(TagRenderMode.SelfClosing));
        }
        public static IHtmlString Image(this HtmlHelper helper, string src, string alt)
        {
            TagBuilder builder = new TagBuilder("img");
            builder.MergeAttribute("alt", alt);
            builder.MergeAttribute(
                "src", UrlHelper.GenerateContentUrl(src, helper.ViewContext.HttpContext));
            return MvcHtmlString.Create(
                builder.ToString(TagRenderMode.SelfClosing));
        }
        public static IHtmlString Image(this HtmlHelper helper, string src, string alt, object htmlAttrs)
        {
            TagBuilder builder = new TagBuilder("img");
            builder.MergeAttribute("alt", alt);
            builder.MergeAttribute(
                "src", UrlHelper.GenerateContentUrl(src, helper.ViewContext.HttpContext));
            builder.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttrs));
            return MvcHtmlString.Create(
                builder.ToString(TagRenderMode.SelfClosing));

        }
        public static IHtmlString RadioButtonListFor<TModel, Tproperty>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, Tproperty>> exp,
            IEnumerable<SelectListItem> list, object htmlAttrs)
        {
            StringBuilder sb = new StringBuilder();
            var name = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(
                ExpressionHelper.GetExpressionText(exp));
            var value = ModelMetadata.FromLambdaExpression(exp, helper.ViewData).Model.ToString();

            int i = 1;
            foreach (var item in list)
            {
                string id = $"{name}_{i++}";
                TagBuilder label = new TagBuilder("label");
                label.MergeAttributes(
                    HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttrs));
                label.InnerHtml = helper.RadioButton(name, item.Value, (item.Value == value), new { id = id, memberid = i }).ToString();
                label.InnerHtml += (item.Text + "\r\n");
                sb.Append(label.ToString(TagRenderMode.Normal));
            }
            return MvcHtmlString.Create(sb.ToString());
        }


        //　チェック時に、htmlattのバリュー値を出力するチェックボックスを生成。
        //ラムダ式版
        public static IHtmlString CheckboxNameFor<TModel, Tproperty>(this HtmlHelper<TModel> helper,
           Expression<Func<TModel, Tproperty>> exp,
           string name, object htmlAttrs)
        {
            StringBuilder sb = new StringBuilder();
            var value = ModelMetadata.FromLambdaExpression(exp, helper.ViewData).Model.ToString();
            TagBuilder input = new TagBuilder("input");
            input.MergeAttribute("name", name);
            input.MergeAttribute("type", "checkbox");
            //input.MergeAttribute("value", "true");
            if (value == "True") input.MergeAttribute("checked", "checked");

            input.MergeAttributes(
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttrs));
            sb.Append(input.ToString(TagRenderMode.SelfClosing));

            input = new TagBuilder("input");
            input.MergeAttribute("name", name);
            input.MergeAttribute("type", "hidden");
            input.MergeAttribute("value", "false");
            sb.Append(input.ToString(TagRenderMode.SelfClosing));
            return MvcHtmlString.Create(sb.ToString());
        }

        //　ラムダ式の値を直接出力し、同時にHiddenInputを生成。
        public static IHtmlString DisplayItemFor<TModel, Tproperty>(this HtmlHelper<TModel> helper,
           Expression<Func<TModel, Tproperty>> exp)
        {
            StringBuilder sb = new StringBuilder();
            var metaData = ModelMetadata.FromLambdaExpression(exp, helper.ViewData);
            var value = metaData.Model.ToString();
            sb.Append(value);   //表示する値
            TagBuilder input = new TagBuilder("input");
            input.MergeAttribute("name", metaData.PropertyName);
            input.MergeAttribute("id", metaData.PropertyName);
            input.MergeAttribute("type", "hidden");
            input.MergeAttribute("value", value); // hiddenで出力

            sb.Append(input.ToString(TagRenderMode.SelfClosing));

            return MvcHtmlString.Create(sb.ToString());
        }

        //　ラムダ式の値を直接出力し、同時にHiddenInputを生成。(名前指定でアトリビュート付き)
        public static IHtmlString DisplayItemFor<TModel, Tproperty>(this HtmlHelper<TModel> helper,
           Expression<Func<TModel, Tproperty>> exp,
           string name, object htmlAttrs)
        {
            StringBuilder sb = new StringBuilder();
            var value = ModelMetadata.FromLambdaExpression(exp, helper.ViewData).Model.ToString();
            sb.Append(value);   //表示する値

            TagBuilder input = new TagBuilder("input");
            input.MergeAttribute("name", name);
            input.MergeAttribute("type", "hidden");
            input.MergeAttribute("value", value); // hiddenで出力

            input.MergeAttributes(
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttrs));
            sb.Append(input.ToString(TagRenderMode.SelfClosing));

            return MvcHtmlString.Create(sb.ToString());
        }

        //　ラムダ式の値をHiddenInputで、モデルプロパティー名で生成。
        public static IHtmlString HiddenItemFor<TModel, Tproperty>(this HtmlHelper<TModel> helper,
           Expression<Func<TModel, Tproperty>> exp)
        {
            StringBuilder sb = new StringBuilder();
            var metaData = ModelMetadata.FromLambdaExpression(exp, helper.ViewData);
            var value = metaData.Model.ToString();
            TagBuilder input = new TagBuilder("input");
            input.MergeAttribute("name", metaData.PropertyName);
            input.MergeAttribute("id", metaData.PropertyName);
            input.MergeAttribute("type", "hidden");
            input.MergeAttribute("value", value); // hiddenで出力

            sb.Append(input.ToString(TagRenderMode.SelfClosing));

            return MvcHtmlString.Create(sb.ToString());
        }

        //　ラムダ式の値を直接出力し、aタグを生成。(名前指定でアトリビュート付き)
        public static IHtmlString DisplayLinkFor<TModel, Tproperty>(this HtmlHelper<TModel> helper,
           Expression<Func<TModel, Tproperty>> exp,
           string url, object htmlAttrs)
        {
            StringBuilder sb = new StringBuilder();
            var metaData = ModelMetadata.FromLambdaExpression(exp, helper.ViewData);
            var value = metaData.Model.ToString();
            if (string.IsNullOrWhiteSpace(url))
            {
                sb.Append(value);
            }
            else
            {
                TagBuilder input = new TagBuilder("a");
                input.MergeAttribute("href", url); // url
                input.MergeAttributes(
                    HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttrs));
                input.InnerHtml = value;//表示する値
                sb.Append(input.ToString(TagRenderMode.Normal));
            }

            return MvcHtmlString.Create(sb.ToString());
        }
        //　ラムダ式の値を直接出力し、Actionaタグを生成。(名前指定でアトリビュート付き)
        public static IHtmlString DisplayActionLinkFor<TModel, Tproperty>(this HtmlHelper<TModel> helper,
           Expression<Func<TModel, Tproperty>> exp,
           string actionName, string controllerName, object routeValues, object htmlAttrs)
        {
            StringBuilder sb = new StringBuilder();
            var metaData = ModelMetadata.FromLambdaExpression(exp, helper.ViewData);
            var value = metaData.Model.ToString();
            TagBuilder input = new TagBuilder("a");
            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);
            input.MergeAttribute("href", urlHelper.Action(actionName, controllerName, routeValues)); // url
            input.MergeAttributes(
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttrs));
            input.InnerHtml = value;//表示する値
            sb.Append(input.ToString(TagRenderMode.Normal));

            return MvcHtmlString.Create(sb.ToString());
        }

        //　ラムダ式の値を直接出力し、Int のテキストボックスを生成(アトリビュート付き)
        public static IHtmlString IntEditorFor<TModel, Tproperty>(this HtmlHelper<TModel> helper,
           Expression<Func<TModel, Tproperty>> exp, object htmlAttrs)
        {
            StringBuilder sb = new StringBuilder();
            var metaData = ModelMetadata.FromLambdaExpression(exp, helper.ViewData);
            var value = "0";
            if (metaData.Model != null)
            {
                value = metaData.Model.ToString();
            }
            var displayName = metaData.DisplayName;
            //Class を検索
            var attClass = GetObjValue(htmlAttrs, "class");
            TagBuilder input = new TagBuilder("input");
            input.MergeAttribute("name", metaData.PropertyName);
            input.MergeAttribute("id", metaData.PropertyName);
            input.MergeAttribute("value", value);
            var ff = input.Attributes;
            input.MergeAttribute("class", "text-box single-line " + attClass);
            input.MergeAttribute("data-val", "true");
            input.MergeAttribute("data-val-number", $"フィールド {displayName} には数字を指定してください。");
            input.MergeAttribute("data-val-required", $"{displayName} フィールドが必要です。");
            input.MergeAttribute("type", "number");
            input.MergeAttributes(
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttrs));
            sb.Append(input.ToString(TagRenderMode.SelfClosing));
            return MvcHtmlString.Create(sb.ToString());
        }

        //グローバルリソースを取得するビューヘルパー
        public static string GlobalResesource(
            this HtmlHelper helper, string type, string key, params object[] args)
        {
            //var context = helper.ViewContext.HttpContext;
            return string.Format(
                (string)helper.ViewContext.HttpContext.GetGlobalResourceObject(type, key), args);
        }



        // ローカルリソースを取得するビューヘルパー
        public static string LocalResource(
            this HtmlHelper helper, string key, params object[] args)
        {
            var view = helper.ViewContext.View as RazorView;
            return (
                string.Format((string)helper.ViewContext.HttpContext.GetLocalResourceObject(((RazorView)helper.ViewContext.View).ViewPath, key), args)
                );
        }
        // シェアーローカルリソースを取得するビューヘルパー
 
        static string shareDir = ConfigurationManager.AppSettings["SharedViewDir"];
        public static string LocalResource(
            this HtmlHelper helper, string key, string partialName, params object[] args)
        {
            //var view = helper.ViewContext.View as RazorView;
            var view = shareDir + partialName + ".cshtml";
            return (
                string.Format((string)helper.ViewContext.HttpContext.GetLocalResourceObject(view, key), args)
                );
        }
        // get value from htmlattribute objects
        static string GetObjValue(object obj, string targetName)
        {
            var attClass = obj.ToString();
            int iPos = attClass.IndexOf(" " + targetName + " = ");
            if (iPos > 0)
            {
                var att = attClass.Substring(iPos + 9);
                if ((iPos = att.IndexOf(",")) < 0)
                    iPos = att.Length - 2;
                attClass = att.Substring(0, iPos);
            }
            else
                attClass = "";
            return attClass;
        }
    }
}