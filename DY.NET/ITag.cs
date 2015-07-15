/*
 * 작성자: CHILD	
 * 기능: 최상위 부모 클래스
 * 날짜: 2015-03-25
 */

using System;

namespace DY.NET
{
    /// <summary>
    /// 객체에 식별 가능한 태그나 설명, 또는 유저 정의 객체를 보관하기 위한 인터페이스입니다.
    /// </summary>
    public interface ITag
    {
        int Tag { get; set; }
        string Description { get; set; }
        object UserData { get; set; }
    }
}