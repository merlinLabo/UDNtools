// UDN UnityHost
// -----------------------------------------------------------------------------
// The build output must be named DANCEaROUND.exe, placed in %GAME_DIR%\game\.
//

#include "../common/udncommon.h"
#include <stdint.h>
#include <stdlib.h>
#include <wchar.h>

// Static import
extern "C" __declspec(dllimport) int __stdcall UnityMain(HINSTANCE, HINSTANCE, LPSTR, int);

static std::wstring g_log;
static volatile LONG g_exCount = 0;
static LONG g_exLimit = 40;

// crash diagnostics

static std::wstring ModuleOf(const void* addr, uintptr_t* offset)
{
    HMODULE hm = nullptr;
    if (GetModuleHandleExW(GET_MODULE_HANDLE_EX_FLAG_FROM_ADDRESS
                         | GET_MODULE_HANDLE_EX_FLAG_UNCHANGED_REFCOUNT,
                           (LPCWSTR)addr, &hm) && hm) {
        wchar_t path[MAX_PATH * 2] = {0};
        GetModuleFileNameW(hm, path, MAX_PATH * 2);
        *offset = (uintptr_t)addr - (uintptr_t)hm;
        const wchar_t* base = wcsrchr(path, L'\\');
        return base ? base + 1 : path;
    }
    *offset = (uintptr_t)addr;
    return L"<not inside any loaded module>";
}

static const wchar_t* ExceptionName(DWORD code)
{
    switch (code) {
    case EXCEPTION_ACCESS_VIOLATION:      return L"ACCESS_VIOLATION";
    case EXCEPTION_IN_PAGE_ERROR:         return L"IN_PAGE_ERROR";
    case EXCEPTION_ILLEGAL_INSTRUCTION:   return L"ILLEGAL_INSTRUCTION";
    case EXCEPTION_PRIV_INSTRUCTION:      return L"PRIV_INSTRUCTION";
    case EXCEPTION_STACK_OVERFLOW:        return L"STACK_OVERFLOW";
    case EXCEPTION_INT_DIVIDE_BY_ZERO:    return L"INT_DIVIDE_BY_ZERO";
    case EXCEPTION_FLT_DIVIDE_BY_ZERO:    return L"FLT_DIVIDE_BY_ZERO";
    case 0xE06D7363:                      return L"C++ exception";
    case 0x406D1388:                      return L"thread naming (harmless)";
    default:                              return L"other";
    }
}

static void DescribeException(const wchar_t* tag, PEXCEPTION_POINTERS ep)
{
    EXCEPTION_RECORD* er = ep->ExceptionRecord;
    uintptr_t off = 0;
    std::wstring mod = ModuleOf(er->ExceptionAddress, &off);

    wchar_t buf[512];
    wsprintfW(buf, L"%s code=0x%08X (%s) address=0x%p module=%s+0x%IX",
              tag, er->ExceptionCode, ExceptionName(er->ExceptionCode),
              er->ExceptionAddress, mod.c_str(), off);
    std::wstring line = buf;

    if (er->ExceptionCode == EXCEPTION_ACCESS_VIOLATION && er->NumberParameters >= 2) {
        const wchar_t* kind = er->ExceptionInformation[0] == 0 ? L"read"
                            : er->ExceptionInformation[0] == 1 ? L"write"
                            : er->ExceptionInformation[0] == 8 ? L"execute" : L"?";
        wchar_t buf2[128];
        wsprintfW(buf2, L"  op=%s target=0x%p", kind, (void*)er->ExceptionInformation[1]);
        line += buf2;
    }
    udn::LogLine(g_log, line);
}


static void LogStack(PCONTEXT ctxIn)
{
    CONTEXT ctx = *ctxIn;
    UNWIND_HISTORY_TABLE hist;
    ZeroMemory(&hist, sizeof(hist));
    udn::LogLine(g_log, L"    ---- call stack ----");
    for (int i = 0; i < 32 && ctx.Rip; ++i) {
        uintptr_t off = 0;
        std::wstring mod = ModuleOf((const void*)ctx.Rip, &off);
        wchar_t b[320];
        wsprintfW(b, L"    #%02d  %s+0x%IX", i, mod.c_str(), off);
        udn::LogLine(g_log, b);

        DWORD64 imageBase = 0;
        PRUNTIME_FUNCTION rf = RtlLookupFunctionEntry(ctx.Rip, &imageBase, &hist);
        if (rf) {
            PVOID handlerData = nullptr;
            DWORD64 establisher = 0;
            RtlVirtualUnwind(UNW_FLAG_NHANDLER, imageBase, ctx.Rip, rf,
                             &ctx, &handlerData, &establisher, nullptr);
        } else {
            DWORD64 ret = 0; SIZE_T got = 0;
            if (!ctx.Rsp || !ReadProcessMemory(GetCurrentProcess(), (LPCVOID)ctx.Rsp,
                                               &ret, sizeof(ret), &got) || got != sizeof(ret))
                break;
            ctx.Rip = ret;
            ctx.Rsp += 8;
        }
    }
}

static LONG CALLBACK VehHandler(PEXCEPTION_POINTERS ep)
{
    DWORD code = ep->ExceptionRecord->ExceptionCode;
    switch (code) {
    case EXCEPTION_ACCESS_VIOLATION:
    case EXCEPTION_IN_PAGE_ERROR:
    case EXCEPTION_ILLEGAL_INSTRUCTION:
    case EXCEPTION_PRIV_INSTRUCTION:
    case EXCEPTION_STACK_OVERFLOW:
    case EXCEPTION_INT_DIVIDE_BY_ZERO:
        break;
    default:
        return EXCEPTION_CONTINUE_SEARCH;
    }
    LONG n = InterlockedIncrement((LONG*)&g_exCount);
    if (n > g_exLimit) {
        if (n == g_exLimit + 1)
            udn::LogLine(g_log, L"VEH: exception logging cap reached, no longer logging further ones (Mono uses access violations for null-reference checks, this is normal)");
        return EXCEPTION_CONTINUE_SEARCH;
    }
    wchar_t tag[64];
    wsprintfW(tag, L"VEH #%d:", n);
    DescribeException(tag, ep);
    if (n <= 5) LogStack(ep->ContextRecord);
    return EXCEPTION_CONTINUE_SEARCH;
}

static LONG WINAPI TopLevelFilter(PEXCEPTION_POINTERS ep)
{
    udn::LogLine(g_log, L"==== unhandled exception, process is terminating ====");
    DescribeException(L"FATAL:", ep);
    LogStack(ep->ContextRecord);
    return EXCEPTION_CONTINUE_SEARCH;
}

// entry point

static LPSTR CmdLineWithoutExeName()
{
    LPSTR full = GetCommandLineA();
    LPSTR p = full;
    if (*p == '"') {
        ++p;
        while (*p && *p != '"') ++p;
        if (*p == '"') ++p;
    } else {
        while (*p && *p != ' ' && *p != '\t') ++p;
    }
    while (*p == ' ' || *p == '\t') ++p;
    return p;
}

int APIENTRY wWinMain(HINSTANCE hInstance, HINSTANCE, LPWSTR, int nShowCmd)
{
    using namespace udn;

    const std::wstring exe     = ExePath();
    const std::wstring gameDir = DirOf(exe);
    const std::wstring rootDir = DirOf(gameDir);
    g_log = rootDir + L"\\udntools.log";

    IniMap ini = ReadIni(rootDir + L"\\udntools.ini");
    bool setCwd     = IniBool(ini, L"HOST_SET_CWD", true);
    bool setDllDir  = IniBool(ini, L"HOST_SET_DLL_DIR", false);
    bool logEx      = IniBool(ini, L"HOST_LOG_EXCEPTIONS", true);
    {
        std::wstring lim = IniGet(ini, L"HOST_EXCEPTION_LIMIT", L"40");
        int v = _wtoi(lim.c_str());
        if (v > 0) g_exLimit = v;
    }

    if (logEx) {
        AddVectoredExceptionHandler(1, VehHandler);
        SetUnhandledExceptionFilter(TopLevelFilter);
    }

    if (GetEnvironmentVariableW(L"CONNECT_DUMMY_BI2A", nullptr, 0) == 0)
        ApplyGameEnv(LoadGameEnv(ini));

    if (setDllDir)
        SetDllDirectoryW(gameDir.c_str());

    // games dir 
    // Konami.GameSystem uses relative paths when not going through AVS (prop/UDNAppSetting.json, dev/raw/..., etc.)
    // relative to the process's working directory

    if (setCwd)
        SetCurrentDirectoryW(rootDir.c_str());

    std::wstring cmdMode = IniGet(ini, L"HOST_CMDLINE_MODE", L"full");
    LPSTR cmdline = (cmdMode == L"args") ? CmdLineWithoutExeName() : GetCommandLineA();

    int rc = UnityMain(hInstance, nullptr, cmdline, nShowCmd);
    return rc;
}
