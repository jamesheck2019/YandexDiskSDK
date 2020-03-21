using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace YandexDiskSDK
{
    public  class utilitiez
    {
        public static string AsQueryString(Dictionary<string, string> parameters)
        {
            if (!parameters.Any()) { return string.Empty; }

            var builder = new System.Text.StringBuilder("?");
            var separator = string.Empty;
            foreach (var kvp in parameters.Where(P => !string.IsNullOrEmpty(P.Value)))
            {
                builder.AppendFormat("{0}{1}={2}", separator, System.Net.WebUtility.UrlEncode(kvp.Key), System.Net.WebUtility.UrlEncode(kvp.Value.ToString()));
                separator = "&";
            }
            return builder.ToString();
        }

        #region EnumUtils
        public static string stringValueOf(Enum value)
        {
            System.Reflection.FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            else
            {
                return value.ToString();
            }
        }

        public static object enumValueOf(string value, Type enumType)
        {
            string[] names = Enum.GetNames(enumType);

            foreach (string name in names)
            {
                if (stringValueOf((Enum)Enum.Parse(enumType, name)).Equals(value))
                {
                    return Enum.Parse(enumType, name);
                }
            }
            throw  new ArgumentException("The string is not a description or value of the specified enum.");
    }
        #endregion

        public enum UploadTypes{FilePath,Stream,BytesArry}


        public enum DestinationType
        {
            disk,
            app,
            trash
        }

        public enum ResponseType
        {
            token,
            code
        }

        public enum Fields
        {
            nothing,
            name,
            _embedded,
            exif,
            created,
            modified,
            path,
            comment_ids,
            type,
            revision,
            items
        }

        public enum SortEnum
        {
            nothing,
            name,
            created,
            modified,
            path,
            size
        }

        public enum FilterEnum
        {
            audio,
            backup,
            book,
            compressed,
            data,
            development,
            diskimage,
            document,
            encoded,
            executable,
            flash,
            font,
            image,
            settings,
            spreadsheet,
            text,
            unknown,
            video,
            web
        }

        public enum PrivacyEnum
        {
            Public,
            Private
        }

        public enum ItemTypeEnum
        {
            both,
            file,
            dir
        }

        public enum PreviewSizeEnum
        {
            [Description("S")]
            S_150,
            [Description("M")]
            S_300,
            [Description("L")]
            S_500,
            [Description("XL")]
            S_800,
            [Description("XXL")]
            S_1024,
            [Description("XXXL")]
            S_1280
        }



    }

}
