using System;

namespace webtrades.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);//Вывод кода ошибки
    }
}
