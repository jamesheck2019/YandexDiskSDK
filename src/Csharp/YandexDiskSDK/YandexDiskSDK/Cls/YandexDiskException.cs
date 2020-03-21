using System;

namespace YandexDiskSDK
{
    public class YandexDiskException : Exception
    {
        public YandexDiskException(string errorMessage, int errorCode) : base(errorMessage) { }
    }

    public class ExceptionCls
    {
        public static YandexDiskException CreateException(string errorMesage, int errorCode)
        {
            return new YandexDiskException(errorMesage, errorCode);
        }
    }


}
