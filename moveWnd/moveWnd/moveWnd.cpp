// moveWnd.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

class SearchData {
private:
  CString mySearch;

public: 

  SearchData(CString& req) : mySearch(req) {
    mySearch.MakeLower();
  }

public:
  void OnWindow(HWND wnd) {
    WINDOWINFO inf;
    ZeroMemory(&inf, sizeof(inf));
    inf.cbSize = sizeof(inf);

    GetWindowInfo(wnd, &inf);

    TCHAR caption[2048];

    GetWindowText(wnd, caption, 2048);

    CString str(caption);
    str.MakeLower();

    if (str.Find(mySearch) >= 0) {
      wprintf(L"Move window '%s'. \r\n", caption);
      SetWindowPos(wnd, NULL, 0,0,0,0, SWP_NOSIZE);
    } else {
      wprintf(L"Skip window '%s'. \r\n", caption);
    }
  }
};


BOOL CALLBACK EnumWindowsProc(HWND hwnd, LPARAM lParam) {
  ((SearchData*)lParam)->OnWindow(hwnd);
  return TRUE;
}

int _tmain(int argc, _TCHAR* argv[])
{

  if (argc !=2) {
    wprintf(L"Use <program>.exe window_title");
    return -1;
  }

  TCHAR buff[2048];
  _TCHAR* data = argv[1];
  
  TCHAR* p = buff;
  for(; *data != 0; *p++ = (TCHAR)*data++);
  *p++=0;
  *p++=0;

  CString sss(buff);
  SearchData sd(sss);
  EnumWindows(*EnumWindowsProc, (LPARAM)&sd);


	return 0;
}

