// Launcher
// Builds launcher.exe, placed in %gamedir%\
//
// Launcher will:
//   1. Reads udntools.ini
//   2. Checks whether the required Konami.GameSystem.dll patches are applied, warns if not
//   3. Sets the environment variables the game reads
//   4. Launches game exe with the gamedir as working directory
//
// Command line:
//   launcher.exe [args passed through to the game...]
//   launcher.exe --nowait     don't wait for the game to exit, return once launched
//   launcher.exe --check      only run the environment check, print the result, and exit

#include "../common/udncommon.h"
#include <shellapi.h>
#include <stdlib.h>

static const BYTE kOnlineUpdOrig[] = { 0x28, 0x1F, 0x03, 0x00, 0x0A, 0x12, 0x00, 0x12, 0x01, 0x12, 0x02, 0x6F, 0x20, 0x03, 0x00, 0x0A, 0x06, 0x16, 0x31, 0x18, 0x07, 0x2D, 0x08, 0x17, 0x28, 0xA1, 0x04, 0x00, 0x06, 0x2B, 0x06, 0x18, 0x28, 0xA1, 0x04, 0x00, 0x06, 0x08, 0x28, 0xA3, 0x04, 0x00, 0x06, 0x2A, 0x16, 0x28, 0xA1, 0x04, 0x00, 0x06, 0x2A };
static const BYTE kOnlineUpdNew[] = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x16, 0x28, 0xA1, 0x04, 0x00, 0x06, 0x2A };
static const BYTE kRegionOrig[] = { 0x28, 0xB6, 0x0A, 0x00, 0x06, 0x12, 0x00, 0x28, 0x1F, 0x01, 0x00, 0x06, 0x2C, 0x0C, 0x06, 0x7E, 0xEC, 0x00, 0x00, 0x04, 0x28, 0x1C, 0x01, 0x00, 0x06, 0x2A, 0x16, 0x2A };
static const BYTE kRegionNew[] = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x17, 0x2A };
static const BYTE kKbdOrig[] = { 0x28, 0x23, 0x06, 0x00, 0x0A, 0x28, 0x2A, 0x06, 0x00, 0x0A, 0x2A };
static const BYTE kKbdNew[] = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x2A };
static const BYTE kTwoOrig[]  = { 0x16, 0x2A };
static const BYTE kTwoNew[]   = { 0x17, 0x2A };
static const BYTE kLogOrig[]  = { 0x28, 0x3A, 0x00, 0x00, 0x0A, 0x2A };
static const BYTE kLogNew[]   = { 0x00, 0x00, 0x00, 0x00, 0x17, 0x2A };
static const BYTE kEa3Orig[]  = { 0x17, 0x33 };   // ldc.i4.1 ; bne.un.s
static const BYTE kEa3New[]   = { 0x19, 0x34 };   // ldc.i4.3 ; bge.un.s
static const BYTE kEacoinOrig[] = { 0x28, 0x36, 0x04, 0x00, 0x0A, 0x6F, 0xAA, 0x0B, 0x00, 0x0A, 0x6F, 0xA9, 0x12, 0x00, 0x0A, 0x26 };
static const BYTE kEacoinNew[]  = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
static const BYTE kGateOrig[] = { 0x28, 0x7B, 0x0F, 0x00, 0x06, 0x6F, 0x89, 0x0F, 0x00, 0x06, 0x6F, 0x86, 0x07, 0x00, 0x0A, 0x19, 0xFE, 0x01, 0x2A };
static const BYTE kGateNew[]  = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x17, 0x2A };

struct PatchSite {
    int    file;          // 0=Konami.GameSystem.dll, 1=Assembly-CSharp.dll
    DWORD  offset;
    DWORD  len;
    const BYTE* original;
    const BYTE* patched;
    const wchar_t* name;
};

static const PatchSite kSites[] = {
    { 0, 0x0002E6CB, 2,  kTwoOrig,       kTwoNew,       L"DebugOption.IsStandaloneEnable" },
    { 0, 0x0002E70A, 2,  kTwoOrig,       kTwoNew,       L"DebugOption.ConnectDummySecKeyInEditor" },
    { 0, 0x0000BDB8, 51, kOnlineUpdOrig, kOnlineUpdNew, L"OnlinupdateObserver.Update" },
    { 0, 0x0001EF74, 28, kRegionOrig,    kRegionNew,    L"SystemInfoProvider.IsDestinationRegionJapan" },
    { 0, 0x0002D655, 2,  kEa3Orig,       kEa3New,       L"EA3Status.UpdateStatus (executeMode < 3 -> Local)" },
    { 0, 0x00024C3B, 19, kGateOrig,      kGateNew,      L"GateStatus.participationAvailable" },
    { 1, 0x0008B784, 16, kEacoinOrig,    kEacoinNew,    L"GameEntry_ModeSelect: SaveEaCoinXml" },
};


enum PatchState { PATCH_UNKNOWN, PATCH_NONE, PATCH_APPLIED, PATCH_MIXED };

static PatchState CheckPatch(const std::wstring& gsDll, const std::wstring& acDll)
{
    HANDLE h[2];
    h[0] = CreateFileW(gsDll.c_str(), GENERIC_READ, FILE_SHARE_READ, nullptr,
                       OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, nullptr);
    h[1] = CreateFileW(acDll.c_str(), GENERIC_READ, FILE_SHARE_READ, nullptr,
                       OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, nullptr);
    if (h[0] == INVALID_HANDLE_VALUE || h[1] == INVALID_HANDLE_VALUE) {
        if (h[0] != INVALID_HANDLE_VALUE) CloseHandle(h[0]);
        if (h[1] != INVALID_HANDLE_VALUE) CloseHandle(h[1]);
        return PATCH_UNKNOWN;
    }
    int applied = 0, original = 0;
    PatchState result = PATCH_UNKNOWN;
    bool bad = false;
    for (int i = 0; i < (int)_countof(kSites) && !bad; ++i) {
        HANDLE f = h[kSites[i].file];
        LARGE_INTEGER li; li.QuadPart = kSites[i].offset;
        SetFilePointerEx(f, li, nullptr, FILE_BEGIN);
        BYTE b[64] = {0}; DWORD got = 0;
        if (!ReadFile(f, b, kSites[i].len, &got, nullptr) || got != kSites[i].len) { bad = true; break; }
        if (memcmp(b, kSites[i].patched, kSites[i].len) == 0)       ++applied;
        else if (memcmp(b, kSites[i].original, kSites[i].len) == 0) ++original;
        else { bad = true; break; }
    }
    CloseHandle(h[0]); CloseHandle(h[1]);
    if (bad) return PATCH_UNKNOWN;
    if (applied == (int)_countof(kSites))  result = PATCH_APPLIED;
    else if (original == (int)_countof(kSites)) result = PATCH_NONE;
    else result = PATCH_MIXED;
    return result;
}

static bool FileExists(const std::wstring& p)
{
    DWORD a = GetFileAttributesW(p.c_str());
    return a != INVALID_FILE_ATTRIBUTES && !(a & FILE_ATTRIBUTE_DIRECTORY);
}

int APIENTRY wWinMain(HINSTANCE, HINSTANCE, LPWSTR lpCmdLine, int)
{
    using namespace udn;

    const std::wstring rootDir = DirOf(ExePath());
    const std::wstring hostExe = rootDir + L"\\game\\DANCEaROUND.exe";
    const std::wstring gsDll   = rootDir + L"\\game\\DANCEaROUND_Data\\Managed\\Konami.GameSystem.dll";
    const std::wstring acDll   = rootDir + L"\\game\\DANCEaROUND_Data\\Managed\\Assembly-CSharp.dll";

    std::wstring extraArgs = lpCmdLine ? lpCmdLine : L"";
    bool noWait  = extraArgs.find(L"--nowait") != std::wstring::npos;
    bool checkOnly = extraArgs.find(L"--check") != std::wstring::npos;
    // aren't passed through
    for (const wchar_t* flag : { L"--nowait", L"--check" }) {
        size_t p;
        while ((p = extraArgs.find(flag)) != std::wstring::npos)
            extraArgs.erase(p, wcslen(flag));
    }
    extraArgs = Trim(extraArgs);

    // environment check
    std::wstring problems;
    if (!FileExists(rootDir + L"\\game\\UnityPlayer.dll"))
        problems += L"- game\\UnityPlayer.dll not found\n";
    if (!FileExists(gsDll))
        problems += L"- game\\DANCEaROUND_Data\\Managed\\Konami.GameSystem.dll not found\n";
    if (!FileExists(rootDir + L"\\prop\\UDNAppSetting.json"))
        problems += L"- prop\\UDNAppSetting.json not found\n";
    if (!FileExists(hostExe))
        problems += L"- game\\DANCEaROUND.exe not found (the UnityHost build output; build it and copy it over first)\n";

    {
        const std::wstring resDir = rootDir + L"\\game\\DANCEaROUND_Data\\Resources\\";
        if (!FileExists(resDir + L"unity default resources")) {
            if (FileExists(resDir + L"unity-default-resources"))
                problems += L"- game\\DANCEaROUND_Data\\Resources\\unity-default-resources needs to be renamed to\n"
                            L"  \"unity default resources\" (with spaces). Unity uses this file to load its\n"
                            L"  built-in shaders; the wrong name causes a null-pointer crash at startup.\n";
            else
                problems += L"- game\\DANCEaROUND_Data\\Resources\\unity default resources not found\n";
        }
        if (!FileExists(resDir + L"unity_builtin_extra"))
            problems += L"- game\\DANCEaROUND_Data\\Resources\\unity_builtin_extra not found\n";
    }

    if (!FileExists(acDll))
        problems += L"- game\\DANCEaROUND_Data\\Managed\\Assembly-CSharp.dll not found\n";

    PatchState ps = CheckPatch(gsDll, acDll);
    if (ps == PATCH_NONE)
        problems += L"- Konami.GameSystem.dll hasn't been patched yet. The dongle check will stop the\n"
                    L"  game on an error screen. Use config.exe's \"Apply\" first.\n";
    else if (ps == PATCH_MIXED)
        problems += L"- Konami.GameSystem.dll's patch state is inconsistent; restore it and reapply.\n";
    else if (ps == PATCH_UNKNOWN)
        problems += L"- Could not confirm Konami.GameSystem.dll's patch state (the build may differ from beta1).\n";

    if (checkOnly) {
        MessageBoxW(nullptr,
            problems.empty() ? L"Check passed, ready to launch." : (L"Found the following problems:\n\n" + problems).c_str(),
            L"UDNtools environment check",
            problems.empty() ? MB_ICONINFORMATION : MB_ICONWARNING);
        return problems.empty() ? 0 : 1;
    }

    if (!problems.empty()) {
        std::wstring msg = L"Found the following problems:\n\n" + problems + L"\nLaunch anyway?";
        if (MessageBoxW(nullptr, msg.c_str(), L"UDNtools launcher",
                        MB_ICONWARNING | MB_YESNO | MB_DEFBUTTON2) != IDYES)
            return 1;
    }

    // environment variables
    IniMap ini = ReadIni(rootDir + L"\\udntools.ini");
    GameEnv env = LoadGameEnv(ini);
    ApplyGameEnv(env);

    // build command line
    std::wstring cmd = L"\"" + hostExe + L"\"";
    std::wstring forceMode = IniGet(ini, L"LAUNCH_SCREEN_ARGS", L"");
    if (!forceMode.empty()) cmd += L" " + forceMode;
    if (IniBool(ini, L"UNITY_LOG_FILE", true))
        cmd += L" -logFile \"" + rootDir + L"\\udn_unity.log\"";
    if (!extraArgs.empty()) cmd += L" " + extraArgs;

    // If -logFile isn't picked up, Unity writes to its default location instead %USERPROFILE%\AppData\LocalLow\<company>\<product>\Player.log
    // company/product come from game\DANCEaROUND_Data\app.info
    std::wstring playerLogSrc;
    {
        wchar_t up[MAX_PATH * 2] = {0};
        if (GetEnvironmentVariableW(L"USERPROFILE", up, MAX_PATH * 2))
            playerLogSrc = std::wstring(up) + L"\\AppData\\LocalLow\\konami\\DANCEaROUND\\Player.log";
    }
    const std::wstring playerLogDst = rootDir + L"\\udn_player.log";
    DeleteFileW(playerLogDst.c_str());
    DeleteFileW((rootDir + L"\\udn_unity.log").c_str());

    STARTUPINFOW si = { sizeof(si) };
    PROCESS_INFORMATION pi = { 0 };
    std::vector<wchar_t> cmdbuf(cmd.begin(), cmd.end());
    cmdbuf.push_back(L'\0');

    if (!CreateProcessW(nullptr, cmdbuf.data(), nullptr, nullptr, FALSE,
                        0, nullptr, rootDir.c_str(), &si, &pi)) {
        wchar_t num[32]; wsprintfW(num, L"%lu", GetLastError());
        MessageBoxW(nullptr, (L"Failed to launch game\\DANCEaROUND.exe, GetLastError = " + std::wstring(num)).c_str(),
                    L"UDNtools launcher", MB_ICONERROR);
        return 5;
    }

    if (noWait) {
        CloseHandle(pi.hThread); CloseHandle(pi.hProcess);
        return 0;
    }

    WaitForSingleObject(pi.hProcess, INFINITE);
    DWORD rc = 0;
    GetExitCodeProcess(pi.hProcess, &rc);
    CloseHandle(pi.hThread);
    CloseHandle(pi.hProcess);

    if (!playerLogSrc.empty())
        CopyFileW(playerLogSrc.c_str(), playerLogDst.c_str(), FALSE);
    return (int)rc;
}
