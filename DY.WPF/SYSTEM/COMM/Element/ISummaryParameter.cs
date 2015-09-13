using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.WPF.SYSTEM.COMM
{
    /// <summary>
    /// CommDataGrid를 위해 통신 객체의 요약 문자열을 리턴
    /// </summary>
    public interface ISummary
    {
        string GetSummary();
    }
}