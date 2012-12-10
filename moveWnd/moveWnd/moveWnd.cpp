// moveWnd.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"


class SearchData {
private:
  CString mySearch;
  vector<HWND> myWindows;

public: 
  SearchData(CString& req) : mySearch(req) {
    mySearch.MakeLower();
  }

public:
  void OnWindow(HWND wnd) {
    TCHAR caption[2048];
    GetWindowText(wnd, caption, 2000);

    CString str(caption);
    str.MakeLower();

    if (str.Find(mySearch) >= 0) {
      myWindows.push_back(wnd);
    } else {
      wprintf(L"Skip window '%s'. \r\n", caption);
    }
  }

  void processWindows() {
    for(vector<HWND>::iterator it = myWindows.begin(); it != myWindows.end(); it++) {
          TCHAR caption[2048];
          HWND wnd = *it;
          GetWindowText(wnd, caption, 2000);

          wprintf(L"Move window '%s'. \r\n", caption);
          SetWindowPos(wnd, NULL, 0,0,0,0, SWP_NOSIZE);
    }
  }

};


BOOL CALLBACK ListWindowsProc(HWND hwnd, LPARAM lParam) {
  TCHAR buff[2048];
  GetWindowText(hwnd, buff, 2000);
  wprintf(L"Window: %s\n", buff);
  return TRUE;
}


BOOL CALLBACK EnumWindowsProc(HWND hwnd, LPARAM lParam) {
  ((SearchData*)lParam)->OnWindow(hwnd);
  return TRUE;
}

int _tmain(int argc, _TCHAR* argv[])
{  
  wprintf(L"Use: \n");
  wprintf(L"  <program>.exe window_title\n");
  wprintf(L"     to move windows with name containing window_title\n");
  wprintf(L"  <program>.exe\n");
  wprintf(L"     to list all window\n\n");
  
  if (argc !=2) {    
    EnumWindows(*ListWindowsProc, NULL);      
    return 1;
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
  sd.processWindows();

	return 0;
}

