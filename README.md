FDDaaMI
=======
Floppy Disk Drive as a Musical Instrument

입력
--
MIDI 파일을 입력으로 사용하려면
[MIDIFile2Text](http://midifile2text.sourceforge.net/)을 이용하여 파일을 변환해야 합니다.

파일 경로는 소스에 하드코딩되어 있으니 잘 찾아보세요.

입력 파일 형식은 다음 정보가 탭으로 구분되어 한 줄에 하나씩 저장된 텍스트 파일입니다.

+ cumTime : 누적 시간
+ delTime : 델타 시간
+ event : 노트 이벤트, non 또는 noff
+ pitch : 미디 노트 피치
+ vel : 미디 노트 벨로시티
+ chan : 미디 채널
+ value : na

기능
--
호스트 프로그램은 직렬 포트로 지정된 프로토콜로 플로피 디스크 동작 패킷을 실시간 전송합니다.
직렬 포트 설정은 어떤 클래스 생성자에서 변경할 수 있고, 프로토콜은 간단해서 소스를 보면 쉽게 알 수 있습니다.

플로피 디스크 드라이브를 직접 제어하는 부분은 Arduino Uno로 개발했고,
만들 당시 플로피 디스크 드라이브가 4개밖에 없었기 때문에 4개까지 지원합니다.

더 많은 플로피 디스크 드라이브 지원이 필요하다면 직접 수정하거나, 저에게 플로피 디스크 드라이브를 선물해 주세요.
고맙습니다.

GPLv3 최고!
--

Copyright (C) 2013 류형욱

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
