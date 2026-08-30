// UDNtools-beta1 — shared helper code
// Goal: standalone launching of the DANCE aROUND (UDN-001) hard drive backup
#pragma once
#include <windows.h>
#include <string>
#include <vector>
#include <map>

namespace udn {

// ---------------------------------------------------------------- paths

inline std::wstring ExePath()
{
    wchar_t buf[MAX_PATH * 2] = {0};
    GetModuleFileNameW(nullptr, buf, MAX_PATH * 2);
    return buf;
}

inline std::wstring DirOf(const std::wstring& path)
{
    size_t p = path.find_last_of(L"\\/");
    return (p == std::wstring::npos) ? L"." : path.substr(0, p);
}

// ---------------------------------------------------------------- ini reading
//
// udntools.ini is the simplest possible key=value file, no sections, lines starting
// with # or ; are comments. Deliberately not using GetPrivateProfileString, to avoid
// its UTF-8 handling issues.

typedef std::map<std::wstring, std::wstring> IniMap;

inline std::wstring Trim(const std::wstring& s)
{
    size_t a = s.find_first_not_of(L" \t\r\n");
    if (a == std::wstring::npos) return L"";
    size_t b = s.find_last_not_of(L" \t\r\n");
    return s.substr(a, b - a + 1);
}

inline IniMap ReadIni(const std::wstring& file)
{
    IniMap m;
    HANDLE h = CreateFileW(file.c_str(), GENERIC_READ, FILE_SHARE_READ, nullptr,
                           OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, nullptr);
    if (h == INVALID_HANDLE_VALUE) return m;
    DWORD size = GetFileSize(h, nullptr);
    std::vector<char> raw(size + 1, 0);
    DWORD got = 0;
    ReadFile(h, raw.data(), size, &got, nullptr);
    CloseHandle(h);

    // Interpret as UTF-8 (with or without BOM)
    const char* start = raw.data();
    if (got >= 3 && (unsigned char)start[0] == 0xEF && (unsigned char)start[1] == 0xBB
        && (unsigned char)start[2] == 0xBF) { start += 3; got -= 3; }
    int wlen = MultiByteToWideChar(CP_UTF8, 0, start, (int)got, nullptr, 0);
    std::wstring text(wlen, L'\0');
    MultiByteToWideChar(CP_UTF8, 0, start, (int)got, &text[0], wlen);

    size_t pos = 0;
    while (pos <= text.size()) {
        size_t nl = text.find(L'\n', pos);
        std::wstring line = Trim(text.substr(pos, (nl == std::wstring::npos ? text.size() : nl) - pos));
        pos = (nl == std::wstring::npos) ? text.size() + 1 : nl + 1;
        if (line.empty() || line[0] == L'#' || line[0] == L';' || line[0] == L'[') continue;
        size_t eq = line.find(L'=');
        if (eq == std::wstring::npos) continue;
        m[Trim(line.substr(0, eq))] = Trim(line.substr(eq + 1));
    }
    return m;
}

inline std::wstring IniGet(const IniMap& m, const wchar_t* key, const wchar_t* def)
{
    IniMap::const_iterator it = m.find(key);
    return (it == m.end() || it->second.empty()) ? std::wstring(def) : it->second;
}

inline bool IniBool(const IniMap& m, const wchar_t* key, bool def)
{
    std::wstring v = IniGet(m, key, def ? L"true" : L"false");
    return (v == L"1" || v == L"true" || v == L"True" || v == L"TRUE" || v == L"yes");
}

// ---------------------------------------------------------------- logging

inline void LogLine(const std::wstring& logFile, const std::wstring& msg)
{
    HANDLE h = CreateFileW(logFile.c_str(), FILE_APPEND_DATA, FILE_SHARE_READ, nullptr,
                           OPEN_ALWAYS, FILE_ATTRIBUTE_NORMAL, nullptr);
    if (h == INVALID_HANDLE_VALUE) return;
    SYSTEMTIME st; GetLocalTime(&st);
    wchar_t stamp[64];
    wsprintfW(stamp, L"[%02d:%02d:%02d] ", st.wHour, st.wMinute, st.wSecond);
    std::wstring line = std::wstring(stamp) + msg + L"\r\n";
    int n = WideCharToMultiByte(CP_UTF8, 0, line.c_str(), (int)line.size(), nullptr, 0, nullptr, nullptr);
    std::vector<char> buf(n);
    WideCharToMultiByte(CP_UTF8, 0, line.c_str(), (int)line.size(), buf.data(), n, nullptr, nullptr);
    DWORD written = 0;
    WriteFile(h, buf.data(), (DWORD)n, &written, nullptr);
    CloseHandle(h);
}

// ---------------------------------------------------------------- environment variables
//
// Konami.GameSystem's UDNApplicationEnvironmentVariable.ReadEnvironmentVariables()
// reads these names directly with Environment.GetEnvironmentVariable.
// Values are parsed with bool.TryParse, so they must be "true" / "false" (case-insensitive).
// SET_FRONT_VOLUME / SET_WOOFER_VOLUME go through float.TryParse.

struct GameEnv {
    bool  dummyBi2a;      // CONNECT_DUMMY_BI2A     skip the BI2A main I/O board
    bool  dummyIcca;      // CONNECT_DUMMY_ICCA     skip the ICCA card reader
    bool  dummyCamera;    // CONNECT_DUMMY_CAMERA   skip the RealSense camera
    bool  standalone;     // STANDALONE_ENABLE      standalone mode (needs the DLL patch to take effect)
    bool  agingMode;      // AGING_MODE
    bool  gotoQcMode;     // GOTO_QCMODE
    bool  rebootTest;     // REBOOT_TEST
    bool  disableWatchdog;// DISABLE_WATCHDOG
    bool  allLevelPlayable;// ALL_LEVEL_PLAYABLE
    bool  ignoreEntryCalib;// IGNORE_ENTRY_CALIB
    std::wstring frontVolume;  // SET_FRONT_VOLUME  empty = don't set
    std::wstring wooferVolume; // SET_WOOFER_VOLUME
};

inline GameEnv LoadGameEnv(const IniMap& ini)
{
    GameEnv e;
    e.dummyBi2a        = IniBool(ini, L"CONNECT_DUMMY_BI2A",  true);
    e.dummyIcca        = IniBool(ini, L"CONNECT_DUMMY_ICCA",  true);
    e.dummyCamera      = IniBool(ini, L"CONNECT_DUMMY_CAMERA", true);
    e.standalone       = IniBool(ini, L"STANDALONE_ENABLE",   true);
    e.agingMode        = IniBool(ini, L"AGING_MODE",          false);
    e.gotoQcMode       = IniBool(ini, L"GOTO_QCMODE",         false);
    e.rebootTest       = IniBool(ini, L"REBOOT_TEST",         false);
    e.disableWatchdog  = IniBool(ini, L"DISABLE_WATCHDOG",    true);
    e.allLevelPlayable = IniBool(ini, L"ALL_LEVEL_PLAYABLE",  false);
    e.ignoreEntryCalib = IniBool(ini, L"IGNORE_ENTRY_CALIB",  false);
    e.frontVolume      = IniGet(ini, L"SET_FRONT_VOLUME",  L"");
    e.wooferVolume     = IniGet(ini, L"SET_WOOFER_VOLUME", L"");
    return e;
}

inline void ApplyGameEnv(const GameEnv& e)
{
    #define SETB(name, val) SetEnvironmentVariableW(L##name, (val) ? L"true" : L"false")
    SETB("CONNECT_DUMMY_BI2A",   e.dummyBi2a);
    SETB("CONNECT_DUMMY_ICCA",   e.dummyIcca);
    SETB("CONNECT_DUMMY_CAMERA", e.dummyCamera);
    SETB("STANDALONE_ENABLE",    e.standalone);
    SETB("AGING_MODE",           e.agingMode);
    SETB("GOTO_QCMODE",          e.gotoQcMode);
    SETB("REBOOT_TEST",          e.rebootTest);
    SETB("DISABLE_WATCHDOG",     e.disableWatchdog);
    SETB("ALL_LEVEL_PLAYABLE",   e.allLevelPlayable);
    SETB("IGNORE_ENTRY_CALIB",   e.ignoreEntryCalib);
    #undef SETB
    if (!e.frontVolume.empty())  SetEnvironmentVariableW(L"SET_FRONT_VOLUME",  e.frontVolume.c_str());
    if (!e.wooferVolume.empty()) SetEnvironmentVariableW(L"SET_WOOFER_VOLUME", e.wooferVolume.c_str());
}

} // namespace udn
