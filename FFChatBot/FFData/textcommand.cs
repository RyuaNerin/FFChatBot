﻿using System.Collections.Generic;
using System.Text;

namespace FFChatBot.FFData
{
    static partial class FFData
    {
        public static readonly IDictionary<int, byte[]> TextCommand = new Dictionary<int, byte[]>
        {
            { 102, Encoding.UTF8.GetBytes("/말") },
            { 103, Encoding.UTF8.GetBytes("/외") },
            { 104, Encoding.UTF8.GetBytes("/귓") },
            { 105, Encoding.UTF8.GetBytes("/파") },
            { 106, Encoding.UTF8.GetBytes("/링") },
            { 107, Encoding.UTF8.GetBytes("/링1") },
            { 108, Encoding.UTF8.GetBytes("/링2") },
            { 109, Encoding.UTF8.GetBytes("/링3") },
            { 110, Encoding.UTF8.GetBytes("/링4") },
            { 111, Encoding.UTF8.GetBytes("/링5") },
            { 112, Encoding.UTF8.GetBytes("/링6") },
            { 113, Encoding.UTF8.GetBytes("/링7") },
            { 114, Encoding.UTF8.GetBytes("/링8") },
            { 115, Encoding.UTF8.GetBytes("/자") },
            { 116, Encoding.UTF8.GetBytes("/혼") },
            { 117, Encoding.UTF8.GetBytes("/떠") },
            { 118, Encoding.UTF8.GetBytes("/대답") },
            { 119, Encoding.UTF8.GetBytes("/연") },
            { 121, Encoding.UTF8.GetBytes("/참가") },
            { 122, Encoding.UTF8.GetBytes("/거절") },
            { 123, Encoding.UTF8.GetBytes("/초대") },
            { 124, Encoding.UTF8.GetBytes("/추방") },
            { 125, Encoding.UTF8.GetBytes("/파티장") },
            { 126, Encoding.UTF8.GetBytes("/탈퇴") },
            { 131, Encoding.UTF8.GetBytes("/조사") },
            { 132, Encoding.UTF8.GetBytes("/거래") },
            { 133, Encoding.UTF8.GetBytes("/자동달리기") },
            { 134, Encoding.UTF8.GetBytes("/따라가기") },
            { 135, Encoding.UTF8.GetBytes("/전리품") },
            { 136, Encoding.UTF8.GetBytes("/마테리아요청") },
            { 141, Encoding.UTF8.GetBytes("/캐릭터") },
            { 142, Encoding.UTF8.GetBytes("/퀘스트") },
            { 143, Encoding.UTF8.GetBytes("/임무찾기") },
            { 144, Encoding.UTF8.GetBytes("/장비함") },
            { 145, Encoding.UTF8.GetBytes("/토벌수첩") },
            { 146, Encoding.UTF8.GetBytes("/제작수첩") },
            { 147, Encoding.UTF8.GetBytes("/채집수첩") },
            { 148, Encoding.UTF8.GetBytes("/낚시수첩") },
            { 149, Encoding.UTF8.GetBytes("/업적") },
            { 150, Encoding.UTF8.GetBytes("/파티명령") },
            { 151, Encoding.UTF8.GetBytes("/친구목록") },
            { 152, Encoding.UTF8.GetBytes("/차단목록") },
            { 153, Encoding.UTF8.GetBytes("/플레이어검색") },
            { 154, Encoding.UTF8.GetBytes("/지도") },
            { 155, Encoding.UTF8.GetBytes("/텔레포") },
            { 156, Encoding.UTF8.GetBytes("/표식지정") },
            { 157, Encoding.UTF8.GetBytes("/기술목록") },
            { 158, Encoding.UTF8.GetBytes("/임무대기") },
            { 159, Encoding.UTF8.GetBytes("/추천임무") },
            { 160, Encoding.UTF8.GetBytes("/소지품") },
            { 161, Encoding.UTF8.GetBytes("/자유부대명령") },
            { 162, Encoding.UTF8.GetBytes("/링크셸명령") },
            { 163, Encoding.UTF8.GetBytes("/데존") },
            { 164, Encoding.UTF8.GetBytes("/감정표현목록") },
            { 165, Encoding.UTF8.GetBytes("/도움말") },
            { 167, Encoding.UTF8.GetBytes("/캐릭터설정") },
            { 168, Encoding.UTF8.GetBytes("/시스템설정") },
            { 169, Encoding.UTF8.GetBytes("/단축키") },
            { 170, Encoding.UTF8.GetBytes("/매크로") },
            { 171, Encoding.UTF8.GetBytes("/배열") },
            { 172, Encoding.UTF8.GetBytes("/접송종료") },
            { 173, Encoding.UTF8.GetBytes("/게임종료") },
            { 174, Encoding.UTF8.GetBytes("/파티정렬") },
            { 175, Encoding.UTF8.GetBytes("/준비확인") },
            { 176, Encoding.UTF8.GetBytes("/준비됨") },
            { 177, Encoding.UTF8.GetBytes("/준비안됨") },
            { 178, Encoding.UTF8.GetBytes("/지면표식") },
            { 179, Encoding.UTF8.GetBytes("/어류도감") },
            { 180, Encoding.UTF8.GetBytes("/하우징") },
            { 181, Encoding.UTF8.GetBytes("/꼬마친구목록") },
            { 182, Encoding.UTF8.GetBytes("/탈것목록") },
            { 183, Encoding.UTF8.GetBytes("/파티찾기") },
            { 184, Encoding.UTF8.GetBytes("/피브이피") },
            { 185, Encoding.UTF8.GetBytes("/버디") },
            { 186, Encoding.UTF8.GetBytes("/임무용아이템") },
            { 187, Encoding.UTF8.GetBytes("/공략수첩") },
            { 188, Encoding.UTF8.GetBytes("/탐험수첩") },
            { 189, Encoding.UTF8.GetBytes("/골드소서") },
            { 190, Encoding.UTF8.GetBytes("/화폐목록") },
            { 191, Encoding.UTF8.GetBytes("/풍맥의샘") },
            { 193, Encoding.UTF8.GetBytes("/공식가이드") },
            { 201, Encoding.UTF8.GetBytes("/?") },
            { 202, Encoding.UTF8.GetBytes("/대기") },
            { 204, Encoding.UTF8.GetBytes("/열두각인") },
            { 206, Encoding.UTF8.GetBytes("/총사령부경례") },
            { 207, Encoding.UTF8.GetBytes("/아이콘") },
            { 208, Encoding.UTF8.GetBytes("/대기시간") },
            { 209, Encoding.UTF8.GetBytes("/장비세트") },
            { 210, Encoding.UTF8.GetBytes("/아이템정렬") },
            { 211, Encoding.UTF8.GetBytes("/주사위") },
            { 212, Encoding.UTF8.GetBytes("/인스턴스") },
            { 213, Encoding.UTF8.GetBytes("/귓속말이력삭제") },
            { 214, Encoding.UTF8.GetBytes("/매크로잠금") },
            { 215, Encoding.UTF8.GetBytes("/매크로오류") },
            { 216, Encoding.UTF8.GetBytes("/로그삭제") },
            { 217, Encoding.UTF8.GetBytes("/매크로취소") },
            { 218, Encoding.UTF8.GetBytes("/누적플레이") },
            { 219, Encoding.UTF8.GetBytes("/단체자세") },
            { 220, Encoding.UTF8.GetBytes("/아이템검색") },
            { 221, Encoding.UTF8.GetBytes("/경관카메라") },
            { 231, Encoding.UTF8.GetBytes("/다른용무중") },
            { 232, Encoding.UTF8.GetBytes("/자리비움") },
            { 233, Encoding.UTF8.GetBytes("/파티찾는중") },
            { 234, Encoding.UTF8.GetBytes("/마테리아") },
            { 235, Encoding.UTF8.GetBytes("/검색소개말") },
            { 236, Encoding.UTF8.GetBytes("/초보해제") },
            { 251, Encoding.UTF8.GetBytes("/기술시전") },
            { 252, Encoding.UTF8.GetBytes("/무기") },
            { 253, Encoding.UTF8.GetBytes("/대상지정") },
            { 254, Encoding.UTF8.GetBytes("/플레이어대상") },
            { 255, Encoding.UTF8.GetBytes("/기타대상") },
            { 256, Encoding.UTF8.GetBytes("/적대상") },
            { 257, Encoding.UTF8.GetBytes("/전투대상") },
            { 258, Encoding.UTF8.GetBytes("/지원대상") },
            { 259, Encoding.UTF8.GetBytes("/대상보기") },
            { 260, Encoding.UTF8.GetBytes("/오른쪽대상") },
            { 261, Encoding.UTF8.GetBytes("/왼쪽대상") },
            { 262, Encoding.UTF8.GetBytes("/최근대상") },
            { 263, Encoding.UTF8.GetBytes("/최근적") },
            { 264, Encoding.UTF8.GetBytes("/대상고정") },
            { 265, Encoding.UTF8.GetBytes("/주시대상") },
            { 267, Encoding.UTF8.GetBytes("/소환수시전") },
            { 268, Encoding.UTF8.GetBytes("/버디시전") },
            { 269, Encoding.UTF8.GetBytes("/카메라보기") },
            { 270, Encoding.UTF8.GetBytes("/레벨조율") },
            { 271, Encoding.UTF8.GetBytes("/탈것") },
            { 272, Encoding.UTF8.GetBytes("/보조기술") },
            { 273, Encoding.UTF8.GetBytes("/일반기술시전") },
            { 274, Encoding.UTF8.GetBytes("/꼬마친구") },
            { 275, Encoding.UTF8.GetBytes("/버프해제") },
            { 301, Encoding.UTF8.GetBytes("/자동대상고정") },
            { 302, Encoding.UTF8.GetBytes("/자동대상보기") },
            { 303, Encoding.UTF8.GetBytes("/대상원표시") },
            { 304, Encoding.UTF8.GetBytes("/대상선표시") },
            { 305, Encoding.UTF8.GetBytes("/협공선표시") },
            { 306, Encoding.UTF8.GetBytes("/자동대상") },
            { 307, Encoding.UTF8.GetBytes("/머리보이기") },
            { 308, Encoding.UTF8.GetBytes("/무기보이기") },
            { 309, Encoding.UTF8.GetBytes("/무기자동넣기") },
            { 310, Encoding.UTF8.GetBytes("/자신클릭") },
            { 311, Encoding.UTF8.GetBytes("/필드클릭") },
            { 312, Encoding.UTF8.GetBytes("/대화로그") },
            { 313, Encoding.UTF8.GetBytes("/전투효과") },
            { 314, Encoding.UTF8.GetBytes("/허드") },
            { 315, Encoding.UTF8.GetBytes("/단축바") },
            { 316, Encoding.UTF8.GetBytes("/십자단축바") },
            { 317, Encoding.UTF8.GetBytes("/배열초기화") },
            { 318, Encoding.UTF8.GetBytes("/모든창초기화") },
            { 319, Encoding.UTF8.GetBytes("/창크기") },
            { 320, Encoding.UTF8.GetBytes("/기술오류") },
            { 321, Encoding.UTF8.GetBytes("/재시전오류") },
            { 322, Encoding.UTF8.GetBytes("//행사초기화") },
            { 323, Encoding.UTF8.GetBytes("/십자단축바조작") },
            { 401, Encoding.UTF8.GetBytes("/감정표현") },
            { 402, Encoding.UTF8.GetBytes("/놀람") },
            { 403, Encoding.UTF8.GetBytes("/불만") },
            { 404, Encoding.UTF8.GetBytes("/화") },
            { 405, Encoding.UTF8.GetBytes("/수줍") },
            { 406, Encoding.UTF8.GetBytes("/인사") },
            { 407, Encoding.UTF8.GetBytes("/응원") },
            { 408, Encoding.UTF8.GetBytes("/박수") },
            { 409, Encoding.UTF8.GetBytes("/손짓") },
            { 410, Encoding.UTF8.GetBytes("/위로") },
            { 411, Encoding.UTF8.GetBytes("/엉엉") },
            { 412, Encoding.UTF8.GetBytes("/춤") },
            { 413, Encoding.UTF8.GetBytes("/추궁") },
            { 414, Encoding.UTF8.GetBytes("/졸음") },
            { 415, Encoding.UTF8.GetBytes("/분개") },
            { 416, Encoding.UTF8.GetBytes("/작별") },
            { 417, Encoding.UTF8.GetBytes("/손인사") },
            { 418, Encoding.UTF8.GetBytes("/어이") },
            { 419, Encoding.UTF8.GetBytes("/기쁨") },
            { 420, Encoding.UTF8.GetBytes("/무릎") },
            { 421, Encoding.UTF8.GetBytes("/하하") },
            { 422, Encoding.UTF8.GetBytes("/폭소") },
            { 423, Encoding.UTF8.GetBytes("/둘러보기") },
            { 424, Encoding.UTF8.GetBytes("/자신") },
            { 425, Encoding.UTF8.GetBytes("/절레") },
            { 426, Encoding.UTF8.GetBytes("/부정") },
            { 427, Encoding.UTF8.GetBytes("/당황") },
            { 428, Encoding.UTF8.GetBytes("/손가락질") },
            { 429, Encoding.UTF8.GetBytes("/쿡쿡") },
            { 430, Encoding.UTF8.GetBytes("/칭찬") },
            { 431, Encoding.UTF8.GetBytes("/힘내") },
            { 432, Encoding.UTF8.GetBytes("/경례") },
            { 433, Encoding.UTF8.GetBytes("/기겁") },
            { 434, Encoding.UTF8.GetBytes("/으쓱") },
            { 435, Encoding.UTF8.GetBytes("/격려") },
            { 436, Encoding.UTF8.GetBytes("/진정") },
            { 437, Encoding.UTF8.GetBytes("/비틀") },
            { 438, Encoding.UTF8.GetBytes("/기지개") },
            { 439, Encoding.UTF8.GetBytes("/삐침") },
            { 440, Encoding.UTF8.GetBytes("/생각") },
            { 441, Encoding.UTF8.GetBytes("/실망") },
            { 442, Encoding.UTF8.GetBytes("/환영") },
            { 443, Encoding.UTF8.GetBytes("/끄덕") },
            { 444, Encoding.UTF8.GetBytes("/엄지") },
            { 445, Encoding.UTF8.GetBytes("/내모습") },
            { 446, Encoding.UTF8.GetBytes("/자세") },
            { 447, Encoding.UTF8.GetBytes("/손키스") },
            { 448, Encoding.UTF8.GetBytes("/사죄") },
            { 449, Encoding.UTF8.GetBytes("/행복") },
            { 450, Encoding.UTF8.GetBytes("/좌절") },
            { 451, Encoding.UTF8.GetBytes("/앉기") },
            { 455, Encoding.UTF8.GetBytes("/강조") },
            { 456, Encoding.UTF8.GetBytes("/총사령부경례") },
            { 457, Encoding.UTF8.GetBytes("/총사령부경례") },
            { 458, Encoding.UTF8.GetBytes("/총사령부경례") },
            { 459, Encoding.UTF8.GetBytes("/기도") },
            { 460, Encoding.UTF8.GetBytes("/제국식경례") },
            { 461, Encoding.UTF8.GetBytes("/투구") },
            { 469, Encoding.UTF8.GetBytes("/정색") },
            { 470, Encoding.UTF8.GetBytes("/미소") },
            { 471, Encoding.UTF8.GetBytes("/웃음") },
            { 472, Encoding.UTF8.GetBytes("/자신감") },
            { 473, Encoding.UTF8.GetBytes("/음흉") },
            { 474, Encoding.UTF8.GetBytes("/눈감기") },
            { 475, Encoding.UTF8.GetBytes("/울상") },
            { 476, Encoding.UTF8.GetBytes("/공포") },
            { 477, Encoding.UTF8.GetBytes("/어리둥절") },
            { 478, Encoding.UTF8.GetBytes("/아야") },
            { 479, Encoding.UTF8.GetBytes("/불쾌") },
            { 480, Encoding.UTF8.GetBytes("/깨달음") },
            { 481, Encoding.UTF8.GetBytes("/걱정") },
            { 486, Encoding.UTF8.GetBytes("/던지기") },
            { 491, Encoding.UTF8.GetBytes("/자세변경") },
            { 502, Encoding.UTF8.GetBytes("/탭댄스") },
            { 503, Encoding.UTF8.GetBytes("/풍요의춤") },
            { 504, Encoding.UTF8.GetBytes("/궁중무용") },
            { 505, Encoding.UTF8.GetBytes("/신사의춤") },
            { 506, Encoding.UTF8.GetBytes("/쓰담") },
            { 507, Encoding.UTF8.GetBytes("/건네기") },
            { 510, Encoding.UTF8.GetBytes("/축제의춤") },
            { 511, Encoding.UTF8.GetBytes("/승리") },
            { 512, Encoding.UTF8.GetBytes("/따귀") },
            { 513, Encoding.UTF8.GetBytes("/포옹") },
            { 514, Encoding.UTF8.GetBytes("/껴안기") },
            { 515, Encoding.UTF8.GetBytes("/힐디브랜드") },
            { 516, Encoding.UTF8.GetBytes("/주먹내밀기") },
            { 517, Encoding.UTF8.GetBytes("/큰절") },
            { 519, Encoding.UTF8.GetBytes("/사베네어춤") },
            { 520, Encoding.UTF8.GetBytes("/황금의춤") },
            { 521, Encoding.UTF8.GetBytes("/태양의춤") },
            { 525, Encoding.UTF8.GetBytes("/포권") },
        };
    }
}