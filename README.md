# AutoTrader - 자동매매 소스코드 공유

## 개요

개발 과정을 공개하는 이유는 자동매매에 관심있는 C# 프로그래머에게 도움이 되면서 한편으로 그분들과 교제를 나누고 싶은 이유였습니다. 그분들께 도움이 될 수 있도록 증권사 연동 및 기본 시그널 처리를 구현하는 과정을 공개합니다.
각 기능에 대한 확인은 콘솔로 하게 됩니다.

- AutoTrader는 개별 증권사를 Provider 형태로 결합할 수 있습니다. (현재는 키움증권 OpenAPI Provider만 존재합니다.)
- 최종적으로 AutoTrader를 이용하는 어플리케이션의 소스코드는 소스코드상에서 증권사 API를 직접 사용하지 않아도 되게 됩니다. (진행중)

## 현재 진행 상황

- [x] 키움증권 OpenAPI 관련 기능 구현중에 있습니다. 현재 `주식기본정보`, `주식분봉정보`, `주식일봉정보`를 조회 확인할 수 있습니다.
- [x] 주문
- [ ] 주문 실시간
- [x] 실시간 시세(호가잔량, 체결)
- [ ] 계좌 관련
- [ ] 시그널 관련
- ※ 키움증권의 경우 1초에 5번, 30초에 몇번, 1시간에 몇번 API 호출에 대한 제한이 있습니다. 현재는 1초에 5번의 제한만 처리되어 있습니다.

## 이후 진행 계획
1. 이베스트 xingAPI 기반 provider 구현
1. AutoTrader 기준의 interface 일반화
1. AutoTrader의 동작성을 확인하기 위한 간단한 매매프로그램 구현
