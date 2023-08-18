import pymem
import win32gui
from pymem.exception import CouldNotOpenProcess
from win32process import GetWindowThreadProcessId

import ctypes, sys


def is_admin():
    try:
        return ctypes.windll.shell32.IsUserAnAdmin()
    except:
        return False


def enum_windows_callback(hwnd: int, hwnd_list: list):
    hwnd_list.append(hwnd)


if not is_admin():
    ctypes.windll.shell32.ShellExecuteW(None, "runas", sys.executable, __file__, None, 1)

HWND_LIST = []
win32gui.EnumWindows(enum_windows_callback, HWND_LIST)

MINIGRAM_PIDS = []

MINIGRAM_TITLE = '羊了个羊'

for hwnd in HWND_LIST:
    title = win32gui.GetWindowText(hwnd)
    if MINIGRAM_TITLE in title:
        # print(title, hwnd)
        MINIGRAM_PIDS = GetWindowThreadProcessId(hwnd)
        break

# for hwnd in HWND_LIST:
#     # foreach all WeChatAppEx process
#     thread_id, process_id = win32process.GetWindowThreadProcessId(hwnd)
#     proc = psutil.Process(pid=process_id)
#     if "WeChatAppEx" in proc.name():
#         MINIGRAM_PIDS.append(process_id)
#         print(process_id, proc.name())


if len(MINIGRAM_PIDS) == 0:
    print(f"{MINIGRAM_TITLE} not found")
    exit(0)

for pid in MINIGRAM_PIDS:
    try:
        pm = pymem.Pymem(pid)
        bytes_pattern = r'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9'.encode('utf-8')
        character_count_address = pymem.pattern.pattern_scan_all(pm.process_handle, bytes_pattern,
                                                                 return_multiple=True)
        # print(character_count_address)
        for address in character_count_address:
            try:
                text = pm.read_string(address, 500)
                print(text)
            except:
                continue

    except CouldNotOpenProcess as e:
        print(e)
