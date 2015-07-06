using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.DATALOGIC.MATRIX
{
    public enum Matrix200HostModeState
    {
        DISCONNECT,
        
        TRY_HOST_MODE_ENTER, //호스트 모드 접속 중 
        TRY_PROG_MODE_ENTER, //프로그 모드 접속 중

        NOW_HOST_MODE, 
        NOW_PROG_MODE, //연결 시 디폴트 상태 

        TRY_HOST_MODE_EXIT, //호스트 모드 해제 중 
        TRY_PROG_MODE_EXIT, //프로그 모드 해제 중 

        TRY_READING, //읽기 시도 중 

       
    }
}