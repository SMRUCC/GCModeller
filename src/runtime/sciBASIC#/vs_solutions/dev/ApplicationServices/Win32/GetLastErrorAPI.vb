﻿#Region "Microsoft.VisualBasic::dd20f5bcd1d85cfbb30980c437ecdbb1, Microsoft.VisualBasic.Core\ApplicationServices\Tools\Win32\GetLastErrorAPI.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    '     Module GetLastErrorAPI
    ' 
    '         Function: GetLastErrorCode
    '         Enum ErrorCodes
    ' 
    ' 
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Win32

    ''' <summary>
    ''' Wrapper for the returns value of api <see cref="GetLastError"/>
    ''' </summary>
    ''' <remarks>
    ''' <see cref="ErrorCodes"/> reference msdn:
    ''' 
    ''' + https://support.microsoft.com/en-us/kb/155011
    ''' + https://support.microsoft.com/EN-US/kb/155012
    ''' </remarks>
    Public Module GetLastErrorAPI

        ''' <summary>
        ''' 针对之前调用的api函数，用这个函数取得扩展错误信息（在vb里使用：在vb中，用Err对象的``GetLastError``
        ''' 属性获取``GetLastError``的值。这样做是必要的，因为在api调用返回以及vb调用继续执行期间，
        ''' vb有时会重设``GetLastError``的值）
        ''' 
        ''' ``GetLastError``返回的值通过在api函数中调用``SetLastError``或``SetLastErrorEx``设置。函数
        ''' 并无必要设置上一次错误信息，所以即使一次``GetLastError``调用返回的是零值，也不能
        ''' 担保函数已成功执行。只有在函数调用返回一个错误结果时，这个函数指出的错误结果
        ''' 才是有效的。通常，只有在函数返回一个错误结果，而且已知函数会设置``GetLastError``
        ''' 变量的前提下，才应访问``GetLastError``；这时能保证获得有效的结果。``SetLastError``函
        ''' 数主要在对api函数进行模拟的dll函数中使用。
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' ``GetLastError``返回的值通过在api函数中调用``SetLastError``或``SetLastErrorEx``设置。函数并无必要设置上一次错误信息，
        ''' 所以即使一次``GetLastError``调用返回的是零值，也不能担保函数已成功执行。只有在函数调用返回一个错误结果时，
        ''' 这个函数指出的错误结果才是有效的。通常，只有在函数返回一个错误结果，而且已知函数会设置``GetLastError``变量的前提下，
        ''' 才应访问``GetLastError``；这时能保证获得有效的结果。``SetLastError``函数主要在对api函数进行模拟的dll函数中使用，
        ''' 所以对vb应用程序来说是没有意义的
        ''' </remarks>
        Public Declare Function GetLastError Lib "kernel32" Alias "GetLastError" () As Integer

        ''' <summary>
        ''' Retrieves the calling thread's last-error code value. The last-error code is maintained on a per-thread basis. 
        ''' Multiple threads do not overwrite each other's last-error code.
        ''' </summary>
        ''' <returns>
        ''' The return value is the calling thread's last-error code.
        ''' 
        ''' The Return Value section of the documentation for each function that sets the last-error code notes the conditions 
        ''' under which the function sets the last-error code. Most functions that set the thread's last-error code set it 
        ''' when they fail. However, some functions also set the last-error code when they succeed. If the function is not 
        ''' documented to set the last-error code, the value returned by this function is simply the most recent last-error 
        ''' code to have been set; some functions set the last-error code to 0 on success and others do not.
        ''' </returns>
        ''' <remarks>
        ''' Functions executed by the calling thread set this value by calling the SetLastError function. You should call the 
        ''' GetLastError function immediately when a function's return value indicates that such a call will return useful data. 
        ''' That is because some functions call SetLastError with a zero when they succeed, wiping out the error code set by 
        ''' the most recently failed function.
        ''' To obtain an error string for system error codes, use the FormatMessage function. For a complete list of error codes 
        ''' provided by the operating system, see System Error Codes.
        ''' The error codes returned by a function are Not part of the Windows API specification And can vary by operating system 
        ''' Or device driver. For this reason, we cannot provide the complete list of error codes that can be returned by each 
        ''' function. There are also many functions whose documentation does Not include even a partial list of error codes that 
        ''' can be returned.
        ''' Error codes are 32-bit values (bit 31 Is the most significant bit). Bit 29 Is reserved For application-defined Error 
        ''' codes; no system Error code has this bit Set. If you are defining an Error code For your application, Set this bit 
        ''' To one. That indicates that the Error code has been defined by an application, And ensures that your Error code does 
        ''' Not conflict With any Error codes defined by the system.
        ''' 
        ''' To convert a system error into an HRESULT value, use the HRESULT_FROM_WIN32 macro.
        ''' </remarks>
        Public Function GetLastErrorCode() As ErrorCodes
            Return CType(GetLastError, ErrorCodes)
        End Function

        ''' <summary>
        ''' This article lists the error codes you may encounter in Windows NT. For the remaining error codes, 
        ''' please see the following article(s) in the Microsoft Knowledge Base:
        ''' 
        ''' + [155011](https://support.microsoft.com/en-us/kb/155011) Error Codes in Windows NT Part 1 of 2
        ''' + [155012](https://support.microsoft.com/EN-US/kb/155012) Error Codes in Windows NT Part 2 of 2
        ''' (<see cref="GetLastError"/>的返回值的含义)
        ''' </summary>
        Public Enum ErrorCodes As Integer

            '''<summary>
            ''' Compression algorithm not
            ''' recognized.
            '''</summary>
            LZERROR_UNKNOWNALG = -8

            '''<summary>
            ''' Input parameter out of
            ''' acceptable range.
            '''</summary>
            LZERROR_BADVALUE = -7

            '''<summary>
            ''' Bad global handle.
            '''</summary>
            LZERROR_GLOBLOCK = -6

            '''<summary>
            ''' Insufficient memory for LZFile
            ''' structure.
            '''</summary>
            LZERROR_GLOBALLOC = -5

            '''<summary>
            ''' Out of space for output file.
            '''</summary>
            LZERROR_WRITE = -4

            '''<summary>
            ''' Corrupt compressed file
            ''' format.
            '''</summary>
            LZERROR_READ = -3

            '''<summary>
            ''' Invalid output handle.
            '''</summary>
            LZERROR_BADOUTHANDLE = -2

            '''<summary>
            ''' Invalid input handle.
            '''</summary>
            LZERROR_BADINHANDLE = -1

            '''<summary>
            ''' No error.
            '''</summary>
            NO_ERROR = 0L

            '''<summary>
            ''' The operation was successfully
            ''' completed.
            '''</summary>
            ERROR_SUCCESS = 0L

            '''<summary>
            ''' The function is incorrect.
            '''</summary>
            ERROR_INVALID_FUNCTION = 1L

            '''<summary>
            ''' The system cannot find the
            ''' file specified.
            '''</summary>
            ERROR_FILE_NOT_FOUND = 2L

            '''<summary>
            ''' The system cannot find the
            ''' specified path.
            '''</summary>
            ERROR_PATH_NOT_FOUND = 3L

            '''<summary>
            ''' The system cannot open the
            ''' file.
            '''</summary>
            ERROR_TOO_MANY_OPEN_FILES = 4L

            '''<summary>
            ''' Access is denied.
            '''</summary>
            ERROR_ACCESS_DENIED = 5L

            '''<summary>
            ''' The internal file identifier
            ''' is incorrect.
            '''</summary>
            ERROR_INVALID_HANDLE = 6L

            '''<summary>
            ''' The storage control blocks
            ''' were destroyed.
            '''</summary>
            ERROR_ARENA_TRASHED = 7L

            '''<summary>
            ''' Not enough storage is
            ''' available to process this
            ''' command.
            '''</summary>
            ERROR_NOT_ENOUGH_MEMORY = 8L

            '''<summary>
            ''' The storage control block
            ''' address is invalid.
            '''</summary>
            ERROR_INVALID_BLOCK = 9L

            '''<summary>
            ''' The environment is incorrect.
            '''</summary>
            ERROR_BAD_ENVIRONMENT = 10L

            '''<summary>
            ''' An attempt was made to load a
            ''' program with an incorrect
            ''' format.
            '''</summary>
            ERROR_BAD_FORMAT = 11L

            '''<summary>
            ''' The access code is invalid.
            '''</summary>
            ERROR_INVALID_ACCESS = 12L

            '''<summary>
            ''' The data is invalid.
            '''</summary>
            ERROR_INVALID_DATA = 13L

            '''<summary>
            ''' Not enough storage is
            ''' available to complete this
            ''' operation.
            '''</summary>
            ERROR_OUTOFMEMORY = 14L

            '''<summary>
            ''' The system cannot find the
            ''' specified drive.
            '''</summary>
            ERROR_INVALID_DRIVE = 15L

            '''<summary>
            ''' The directory cannot be
            ''' removed.
            '''</summary>
            ERROR_CURRENT_DIRECTORY = 16L

            '''<summary>
            ''' The system cannot move the
            ''' file to a different disk
            ''' drive.
            '''</summary>
            ERROR_NOT_SAME_DEVICE = 17L

            '''<summary>
            ''' There are no more files.
            '''</summary>
            ERROR_NO_MORE_FILES = 18L

            '''<summary>
            ''' The media is write protected.
            '''</summary>
            ERROR_WRITE_PROTECT = 19L

            '''<summary>
            ''' The system cannot find the
            ''' specified device.
            '''</summary>
            ERROR_BAD_UNIT = 20L

            '''<summary>
            ''' The drive is not ready.
            '''</summary>
            ERROR_NOT_READY = 21L

            '''<summary>
            ''' The device does not recognize
            ''' the command.
            '''</summary>
            ERROR_BAD_COMMAND = 22L

            '''<summary>
            ''' Data error (cyclic redundancy
            ''' check).
            '''</summary>
            ERROR_CRC = 23L

            '''<summary>
            ''' The program issued a command
            ''' but the command length is
            ''' incorrect.
            '''</summary>
            ERROR_BAD_LENGTH = 24L

            '''<summary>
            ''' The drive cannot locate a
            ''' specific area or track on the
            ''' disk.
            '''</summary>
            ERROR_SEEK = 25L

            '''<summary>
            ''' The specified disk cannot be
            ''' accessed.
            '''</summary>
            ERROR_NOT_DOS_DISK = 26L

            '''<summary>
            ''' The drive cannot find the
            ''' requested sector.
            '''</summary>
            ERROR_SECTOR_NOT_FOUND = 27L

            '''<summary>
            ''' The printer is out of paper.
            '''</summary>
            ERROR_OUT_OF_PAPER = 28L

            '''<summary>
            ''' The system cannot write to the
            ''' specified device.
            '''</summary>
            ERROR_WRITE_FAULT = 29L

            '''<summary>
            ''' The system cannot read from
            ''' the specified device.
            '''</summary>
            ERROR_READ_FAULT = 30L

            '''<summary>
            ''' A device attached to the
            ''' system is not functioning.
            '''</summary>
            ERROR_GEN_FAILURE = 31L

            '''<summary>
            ''' The process cannot access the
            ''' file because it is being used
            ''' by another process.
            '''</summary>
            ERROR_SHARING_VIOLATION = 32L

            '''<summary>
            ''' The process cannot access the
            ''' file because another process
            ''' has locked a portion of the
            ''' file.
            '''</summary>
            ERROR_LOCK_VIOLATION = 33L

            '''<summary>
            ''' The wrong disk is in the
            ''' drive. Insert %2 (Volume
            ''' Serial Number: %3) into drive
            ''' %1.
            '''</summary>
            ERROR_WRONG_DISK = 34L

            '''<summary>
            ''' Too many files opened for
            ''' sharing.
            '''</summary>
            ERROR_SHARING_BUFFER_EXCEEDED = 36L

            '''<summary>
            ''' Reached End Of File.
            '''</summary>
            ERROR_HANDLE_EOF = 38L

            '''<summary>
            ''' The disk is full.
            '''</summary>
            ERROR_HANDLE_DISK_FULL = 39L

            '''<summary>
            ''' The network request is not
            ''' supported.
            '''</summary>
            ERROR_NOT_SUPPORTED = 50L

            '''<summary>
            ''' The remote computer is not
            ''' available.
            '''</summary>
            ERROR_REM_NOT_LIST = 51L

            '''<summary>
            ''' A duplicate name exists on the
            ''' network.
            ''' 
            '''</summary>
            ERROR_DUP_NAME = 52L

            '''<summary>
            ''' The network path was not
            ''' found.
            '''</summary>
            ERROR_BAD_NETPATH = 53L

            '''<summary>
            ''' The network is busy.
            '''</summary>
            ERROR_NETWORK_BUSY = 54L

            '''<summary>
            ''' The specified network resource
            ''' is no longer available.
            '''</summary>
            ERROR_DEV_NOT_EXIST = 55L

            '''<summary>
            ''' The network BIOS command limit
            ''' has been reached.
            '''</summary>
            ERROR_TOO_MANY_CMDS = 56L

            '''<summary>
            ''' A network adapter hardware
            ''' error occurred.
            '''</summary>
            ERROR_ADAP_HDW_ERR = 57L

            '''<summary>
            ''' The specified server cannot
            ''' perform the requested
            ''' operation.
            '''</summary>
            ERROR_BAD_NET_RESP = 58L

            '''<summary>
            ''' An unexpected network error
            ''' occurred.
            '''</summary>
            ERROR_UNEXP_NET_ERR = 59L

            '''<summary>
            ''' The remote adapter is not
            ''' compatible.
            '''</summary>
            ERROR_BAD_REM_ADAP = 60L

            '''<summary>
            ''' The printer queue is full.
            '''</summary>
            ERROR_PRINTQ_FULL = 61L

            '''<summary>
            ''' Space to store the file
            ''' waiting to be printed is not
            ''' available on the server.
            '''</summary>
            ERROR_NO_SPOOL_SPACE = 62L

            '''<summary>
            ''' File waiting to be printed was
            ''' deleted.
            '''</summary>
            ERROR_PRINT_CANCELLED = 63L

            '''<summary>
            ''' The specified network name is
            ''' no longer available.
            '''</summary>
            ERROR_NETNAME_DELETED = 64L

            '''<summary>
            ''' Network access is denied.
            '''</summary>
            ERROR_NETWORK_ACCESS_DENIED = 65L

            '''<summary>
            ''' The network resource type is
            ''' incorrect.
            '''</summary>
            ERROR_BAD_DEV_TYPE = 66L

            '''<summary>
            ''' The network name cannot be
            ''' found.
            '''</summary>
            ERROR_BAD_NET_NAME = 67L

            '''<summary>
            ''' The name limit for the local
            ''' computer network adapter card
            ''' exceeded.
            '''</summary>
            ERROR_TOO_MANY_NAMES = 68L

            '''<summary>
            ''' The network BIOS session limit
            ''' exceeded.
            '''</summary>
            ERROR_TOO_MANY_SESS = 69L

            '''<summary>
            ''' The remote server is paused or
            ''' is in the process of being
            ''' started.
            '''</summary>
            ERROR_SHARING_PAUSED = 70L

            '''<summary>
            ''' The network request was not
            ''' accepted.
            '''</summary>
            ERROR_REQ_NOT_ACCEP = 71L

            '''<summary>
            ''' The specified printer or disk
            ''' device has been paused.
            '''</summary>
            ERROR_REDIR_PAUSED = 72L

            '''<summary>
            ''' The file exists.
            '''</summary>
            ERROR_FILE_EXISTS = 80L

            '''<summary>
            ''' The directory or file cannot
            ''' be created.
            '''</summary>
            ERROR_CANNOT_MAKE = 82L

            '''<summary>
            ''' Fail on INT 24.
            '''</summary>
            ERROR_FAIL_I24 = 83L

            '''<summary>
            ''' Storage to process this
            ''' request is not available.
            '''</summary>
            ERROR_OUT_OF_STRUCTURES = 84L

            '''<summary>
            ''' The local device name is
            ''' already in use.
            '''</summary>
            ERROR_ALREADY_ASSIGNED = 85L

            '''<summary>
            ''' The specified network password
            ''' is incorrect.
            '''</summary>
            ERROR_INVALID_PASSWORD = 86L

            '''<summary>
            ''' The parameter is incorrect.
            '''</summary>
            ERROR_INVALID_PARAMETER = 87L

            '''<summary>
            ''' A write fault occurred on the
            ''' network.
            '''</summary>
            ERROR_NET_WRITE_FAULT = 88L

            '''<summary>
            ''' The system cannot start
            ''' another process at this time.
            '''</summary>
            ERROR_NO_PROC_SLOTS = 89L

            '''<summary>
            ''' Cannot create another system
            ''' semaphore.
            '''</summary>
            ERROR_TOO_MANY_SEMAPHORES = 100L

            '''<summary>
            ''' The exclusive semaphore is
            ''' owned by another process.
            '''</summary>
            ERROR_EXCL_SEM_ALREADY_OWNED = 101L

            '''<summary>
            ''' The semaphore is set and
            ''' cannot be closed.
            '''</summary>
            ERROR_SEM_IS_SET = 102L

            '''<summary>
            ''' The semaphore cannot be set
            ''' again.
            '''</summary>
            ERROR_TOO_MANY_SEM_REQUESTS = 103L

            '''<summary>
            ''' Cannot request exclusive
            ''' semaphores at interrupt time.
            '''</summary>
            ERROR_INVALID_AT_INTERRUPT_TIME = 104L

            '''<summary>
            ''' The previous ownership of this
            ''' semaphore has ended.
            '''</summary>
            ERROR_SEM_OWNER_DIED = 105L

            '''<summary>
            ''' Insert the disk for drive 1.
            '''</summary>
            ERROR_SEM_USER_LIMIT = 106L

            '''<summary>
            ''' Program stopped because
            ''' alternate disk was not
            ''' inserted.
            '''</summary>
            ERROR_DISK_CHANGE = 107L

            '''<summary>
            ''' The disk is in use or locked
            ''' by another process.
            '''</summary>
            ERROR_DRIVE_LOCKED = 108L

            '''<summary>
            ''' The pipe was ended.
            '''</summary>
            ERROR_BROKEN_PIPE = 109L

            '''<summary>
            ''' The system cannot open the
            ''' specified device or file.
            '''</summary>
            ERROR_OPEN_FAILED = 110L

            '''<summary>
            ''' The file name is too long.
            '''</summary>
            ERROR_BUFFER_OVERFLOW = 111L

            '''<summary>
            ''' There is not enough space on
            ''' the disk.
            '''</summary>
            ERROR_DISK_FULL = 112L

            '''<summary>
            ''' No more internal file
            ''' identifiers available.
            '''</summary>
            ERROR_NO_MORE_SEARCH_HANDLES = 113L

            '''<summary>
            ''' The target internal file
            ''' identifier is incorrect.
            '''</summary>
            ERROR_INVALID_TARGET_HANDLE = 114L

            '''<summary>
            ''' The IOCTL call made by the
            ''' application program is
            ''' incorrect.
            '''</summary>
            ERROR_INVALID_CATEGORY = 117L

            '''<summary>
            ''' The verify-on-write switch
            ''' parameter value is incorrect.
            '''</summary>
            ERROR_INVALID_VERIFY_SWITCH = 118L

            '''<summary>
            ''' The system does not support
            ''' the requested command.
            '''</summary>
            ERROR_BAD_DRIVER_LEVEL = 119L

            '''<summary>
            ''' The Application Program
            ''' Interface (API) entered will
            ''' only work in Windows/NT mode.
            '''</summary>
            ERROR_CALL_NOT_IMPLEMENTED = 120L

            '''<summary>
            ''' The semaphore timeout period
            ''' has expired.
            '''</summary>
            ERROR_SEM_TIMEOUT = 121L

            '''<summary>
            ''' The data area passed to a
            ''' system call is too small.
            '''</summary>
            ERROR_INSUFFICIENT_BUFFER = 122L

            '''<summary>
            ''' The file name, directory name,
            ''' or volume label is
            ''' syntactically incorrect.
            '''</summary>
            ERROR_INVALID_NAME = 123L

            '''<summary>
            ''' The system call level is
            ''' incorrect.
            '''</summary>
            ERROR_INVALID_LEVEL = 124L

            '''<summary>
            ''' The disk has no volume label.
            '''</summary>
            ERROR_NO_VOLUME_LABEL = 125L

            '''<summary>
            ''' The specified module cannot be
            ''' found.
            '''</summary>
            ERROR_MOD_NOT_FOUND = 126L

            '''<summary>
            ''' The specified procedure could
            ''' not be found.
            '''</summary>
            ERROR_PROC_NOT_FOUND = 127L

            '''<summary>
            ''' There are no child processes
            ''' to wait for.
            '''</summary>
            ERROR_WAIT_NO_CHILDREN = 128L

            '''<summary>
            ''' The %1 application cannot be
            ''' run in Windows mode.
            '''</summary>
            ERROR_CHILD_NOT_COMPLETE = 129L

            '''<summary>
            ''' Attempt to use a file handle
            ''' to an open disk partition for
            ''' an operation other than raw
            ''' disk I/O.
            '''</summary>
            ERROR_DIRECT_ACCESS_HANDLE = 130L

            '''<summary>
            ''' An attempt was made to move
            ''' the file pointer before the
            ''' beginning of the file.
            '''</summary>
            ERROR_NEGATIVE_SEEK = 131L

            '''<summary>
            ''' The file pointer cannot be set
            ''' on the specified device or
            ''' file.
            '''</summary>
            ERROR_SEEK_ON_DEVICE = 132L

            '''<summary>
            ''' A JOIN or SUBST command cannot
            ''' be used for a drive that
            ''' contains previously joined
            ''' drives.
            '''</summary>
            ERROR_IS_JOIN_TARGET = 133L

            '''<summary>
            ''' An attempt was made to use a
            ''' JOIN or SUBST command on a
            ''' drive that is already joined.
            '''</summary>
            ERROR_IS_JOINED = 134L

            '''<summary>
            ''' An attempt was made to use a
            ''' JOIN or SUBST command on a
            ''' drive already substituted.
            '''</summary>
            ERROR_IS_SUBSTED = 135L

            '''<summary>
            ''' The system attempted to delete
            ''' the JOIN of a drive not
            ''' previously joined.
            '''</summary>
            ERROR_NOT_JOINED = 136L

            '''<summary>
            ''' The system attempted to delete
            ''' the substitution of a drive
            ''' not previously substituted.
            '''</summary>
            ERROR_NOT_SUBSTED = 137L

            '''<summary>
            ''' The system tried to join a
            ''' drive to a directory on a
            ''' joined drive.
            '''</summary>
            ERROR_JOIN_TO_JOIN = 138L

            '''<summary>
            ''' The system attempted to
            ''' substitute a drive to a
            ''' directory on a substituted
            ''' drive.
            '''</summary>
            ERROR_SUBST_TO_SUBST = 139L

            '''<summary>
            ''' The system tried to join a
            ''' drive to a directory on a
            ''' substituted drive.
            '''</summary>
            ERROR_JOIN_TO_SUBST = 140L

            '''<summary>
            ''' The system attempted to SUBST
            ''' a drive to a directory on a
            ''' joined drive.
            '''</summary>
            ERROR_SUBST_TO_JOIN = 141L

            '''<summary>
            ''' The system cannot perform a
            ''' JOIN or SUBST at this time.
            '''</summary>
            ERROR_BUSY_DRIVE = 142L

            '''<summary>
            ''' The system cannot join or
            ''' substitute a drive to or for a
            ''' directory on the same drive.
            '''</summary>
            ERROR_SAME_DRIVE = 143L

            '''<summary>
            ''' The directory is not a
            ''' subdirectory of the root
            ''' directory.
            '''</summary>
            ERROR_DIR_NOT_ROOT = 144L

            '''<summary>
            ''' The directory is not empty.
            '''</summary>
            ERROR_DIR_NOT_EMPTY = 145L

            '''<summary>
            ''' The path specified is being
            ''' used in a substitute.
            '''</summary>
            ERROR_IS_SUBST_PATH = 146L

            '''<summary>
            ''' Not enough resources are
            ''' available to process this
            ''' command.
            '''</summary>
            ERROR_IS_JOIN_PATH = 147L

            '''<summary>
            ''' The specified path cannot be
            ''' used at this time.
            '''</summary>
            ERROR_PATH_BUSY = 148L

            '''<summary>
            ''' An attempt was made to join or
            ''' substitute a drive for which a
            ''' directory on the drive is the
            ''' target of a previous
            ''' substitute.
            '''</summary>
            ERROR_IS_SUBST_TARGET = 149L

            '''<summary>
            ''' System trace information not
            ''' specified in your CONFIG.SYS
            ''' file, or tracing is not
            ''' allowed.
            '''</summary>
            ERROR_SYSTEM_TRACE = 150L

            '''<summary>
            ''' The number of specified
            ''' semaphore events is incorrect.
            '''</summary>
            ERROR_INVALID_EVENT_COUNT = 151L

            '''<summary>
            ''' Too many semaphores are
            ''' already set.
            '''</summary>
            ERROR_TOO_MANY_MUXWAITERS = 152L

            '''<summary>
            ''' The list is not correct.
            '''</summary>
            ERROR_INVALID_LIST_FORMAT = 153L

            '''<summary>
            ''' The volume label entered
            ''' exceeds the 11 character
            ''' limit. The first 11 characters
            ''' were written to disk. Any
            ''' characters that exceeded the
            ''' 11 character limit were
            ''' automatically deleted.
            '''</summary>
            ERROR_LABEL_TOO_LONG = 154L

            '''<summary>
            ''' Cannot create another thread.
            '''</summary>
            ERROR_TOO_MANY_TCBS = 155L

            '''<summary>
            ''' The recipient process has
            ''' refused the signal.
            '''</summary>
            ERROR_SIGNAL_REFUSED = 156L

            '''<summary>
            ''' The segment is already
            ''' discarded and cannot be
            ''' locked.
            '''</summary>
            ERROR_DISCARDED = 157L

            '''<summary>
            ''' The segment is already
            ''' unlocked.
            '''</summary>
            ERROR_NOT_LOCKED = 158L

            '''<summary>
            ''' The address for the thread ID
            ''' is incorrect.
            '''</summary>
            ERROR_BAD_THREADID_ADDR = 159L

            '''<summary>
            ''' The argument string passed to
            ''' DosExecPgm is incorrect.
            '''</summary>
            ERROR_BAD_ARGUMENTS = 160L

            '''<summary>
            ''' The specified path name is
            ''' invalid.
            '''</summary>
            ERROR_BAD_PATHNAME = 161L

            '''<summary>
            ''' A signal is already pending.
            '''</summary>
            ERROR_SIGNAL_PENDING = 162L

            '''<summary>
            ''' No more threads can be created
            ''' in the system.
            '''</summary>
            ERROR_MAX_THRDS_REACHED = 164L

            '''<summary>
            ''' Attempt to lock a region of a
            ''' file failed.
            '''</summary>
            ERROR_LOCK_FAILED = 167L

            '''<summary>
            ''' The requested resource is in
            ''' use.
            '''</summary>
            ERROR_BUSY = 170L

            '''<summary>
            ''' A lock request was not
            ''' outstanding for the supplied
            ''' cancel region.
            '''</summary>
            ERROR_CANCEL_VIOLATION = 173L

            '''<summary>
            ''' The file system does not
            ''' support atomic changing of the
            ''' lock type.
            '''</summary>
            ERROR_ATOMIC_LOCKS_NOT_SUPPORTED = 174L

            '''<summary>
            ''' The system detected a segment
            ''' number that is incorrect.
            '''</summary>
            ERROR_INVALID_SEGMENT_NUMBER = 180L

            '''<summary>
            ''' The operating system cannot
            ''' run %1.
            '''</summary>
            ERROR_INVALID_ORDINAL = 182L

            '''<summary>
            ''' Attempt to create file that
            ''' already exists.
            '''</summary>
            ERROR_ALREADY_EXISTS = 183L

            '''<summary>
            ''' The flag passed is incorrect.
            '''</summary>
            ERROR_INVALID_FLAG_NUMBER = 186L

            '''<summary>
            ''' The specified system semaphore
            ''' name was not found.
            '''</summary>
            ERROR_SEM_NOT_FOUND = 187L

            '''<summary>
            ''' The operating system cannot
            ''' run %1.
            '''</summary>
            ERROR_INVALID_STARTING_CODESEG = 188L

            '''<summary>
            ''' The operating system cannot
            ''' run %1.
            '''</summary>
            ERROR_INVALID_STACKSEG = 189L

            '''<summary>
            ''' The operating system cannot
            ''' run %1.
            '''</summary>
            ERROR_INVALID_MODULETYPE = 190L

            '''<summary>
            ''' %1 cannot be run in Windows/NT
            ''' mode.
            '''</summary>
            ERROR_INVALID_EXE_SIGNATURE = 191L

            '''<summary>
            ''' The operating system cannot
            ''' run %1.
            '''</summary>
            ERROR_EXE_MARKED_INVALID = 192L

            '''<summary>
            ''' %1 is not a valid Windows-
            ''' based application.
            '''</summary>
            ERROR_BAD_EXE_FORMAT = 193L

            '''<summary>
            ''' The operating system cannot
            ''' run %1.
            '''</summary>
            ERROR_ITERATED_DATA_EXCEEDS_64k = 194L

            '''<summary>
            ''' The operating system cannot
            ''' run %1.
            '''</summary>
            ERROR_INVALID_MINALLOCSIZE = 195L

            '''<summary>
            ''' The operating system cannot
            ''' run this application program.
            '''</summary>
            ERROR_DYNLINK_FROM_INVALID_RING = 196L

            '''<summary>
            ''' The operating system is not
            ''' presently configured to run
            ''' this application.
            '''</summary>
            ERROR_IOPL_NOT_ENABLED = 197L

            '''<summary>
            ''' The operating system cannot
            ''' run %1.
            '''</summary>
            ERROR_INVALID_SEGDPL = 198L

            '''<summary>
            ''' The operating system cannot
            ''' run this application program.
            '''</summary>
            ERROR_AUTODATASEG_EXCEEDS_64k = 199L

            '''<summary>
            ''' The code segment cannot be
            ''' greater than or equal to 64KB.
            '''</summary>
            ERROR_RING2SEG_MUST_BE_MOVABLE = 200L

            '''<summary>
            ''' The operating system cannot
            ''' run %1.
            '''</summary>
            ERROR_RELOC_CHAIN_XEEDS_SEGLIM = 201L

            '''<summary>
            ''' The operating system cannot
            ''' run %1.
            '''</summary>
            ERROR_INFLOOP_IN_RELOC_CHAIN = 202L

            '''<summary>
            ''' The system could not find the
            ''' environment option entered.
            '''</summary>
            ERROR_ENVVAR_NOT_FOUND = 203L

            '''<summary>
            ''' No process in the command
            ''' subtree has a signal handler.
            '''</summary>
            ERROR_NO_SIGNAL_SENT = 205L

            '''<summary>
            ''' The file name or extension is
            ''' too long.
            '''</summary>
            ERROR_FILENAME_EXCED_RANGE = 206L

            '''<summary>
            ''' The ring 2 stack is in use.
            '''</summary>
            ERROR_RING2_STACK_IN_USE = 207L

            '''<summary>
            ''' The global filename characters
            ''' * or ? are entered
            ''' incorrectly, or too many
            ''' global filename characters are
            ''' specified.
            '''</summary>
            ERROR_META_EXPANSION_TOO_LONG = 208L

            '''<summary>
            ''' The signal being posted is
            ''' incorrect.
            '''</summary>
            ERROR_INVALID_SIGNAL_NUMBER = 209L

            '''<summary>
            ''' The signal handler cannot be
            ''' set.
            '''</summary>
            ERROR_THREAD_1_INACTIVE = 210L

            '''<summary>
            ''' The segment is locked and
            ''' cannot be reallocated.
            '''</summary>
            ERROR_LOCKED = 212L

            '''<summary>
            ''' Too many dynamic link modules
            ''' are attached to this program
            ''' or dynamic link module.
            '''</summary>
            ERROR_TOO_MANY_MODULES = 214L

            '''<summary>
            ''' Can't nest calls to
            ''' LoadModule.
            '''</summary>
            ERROR_NESTING_NOT_ALLOWED = 215L

            '''<summary>
            ''' The pipe state is invalid.
            '''</summary>
            ERROR_BAD_PIPE = 230L

            '''<summary>
            ''' All pipe instances busy.
            '''</summary>
            ERROR_PIPE_BUSY = 231L

            '''<summary>
            ''' Pipe close in progress.
            '''</summary>
            ERROR_NO_DATA = 232L

            '''<summary>
            ''' No process on other end of
            ''' pipe.
            '''</summary>
            ERROR_PIPE_NOT_CONNECTED = 233L

            '''<summary>
            ''' More data is available.
            '''</summary>
            ERROR_MORE_DATA = 234L

            '''<summary>
            ''' The session was canceled.
            '''</summary>
            ERROR_VC_DISCONNECTED = 240L

            '''<summary>
            ''' The specified EA name is
            ''' invalid.
            '''</summary>
            ERROR_INVALID_EA_NAME = 254L

            '''<summary>
            ''' The EAs are inconsistent.
            '''</summary>
            ERROR_EA_LIST_INCONSISTENT = 255L

            '''<summary>
            ''' No more data is available.
            '''</summary>
            ERROR_NO_MORE_ITEMS = 259L

            '''<summary>
            ''' The Copy API cannot be used.
            '''</summary>
            ERROR_CANNOT_COPY = 266L

            '''<summary>
            ''' The directory name is invalid.
            '''</summary>
            ERROR_DIRECTORY = 267L

            '''<summary>
            ''' The EAs did not fit in the
            ''' buffer.
            '''</summary>
            ERROR_EAS_DIDNT_FIT = 275L

            '''<summary>
            ''' The EA file on the mounted
            ''' file system is damaged.
            '''</summary>
            ERROR_EA_FILE_CORRUPT = 276L

            '''<summary>
            ''' The EA table in the EA file on
            ''' the mounted file system is
            ''' full.
            '''</summary>
            ERROR_EA_TABLE_FULL = 277L

            '''<summary>
            ''' The specified EA handle is
            ''' invalid.
            '''</summary>
            ERROR_INVALID_EA_HANDLE = 278L

            '''<summary>
            ''' The mounted file system does
            ''' not support extended
            ''' attributes.
            '''</summary>
            ERROR_EAS_NOT_SUPPORTED = 282L

            '''<summary>
            ''' Attempt to release mutex not
            ''' owned by caller.
            '''</summary>
            ERROR_NOT_OWNER = 288L

            '''<summary>
            ''' Too many posts made to a
            ''' semaphore.
            '''</summary>
            ERROR_TOO_MANY_POSTS = 298L

            '''<summary>
            ''' Only part of a
            ''' Read/WriteProcessMemory
            ''' request was completed.
            '''</summary>
            ERROR_PARTIAL_COPY = 299L

            '''<summary>
            ''' The system cannot find message
            ''' for message number 0x%1 in
            ''' message file for %2.
            '''</summary>
            ERROR_MR_MID_NOT_FOUND = 317L

            '''<summary>
            ''' Attempt to access invalid
            ''' address.
            '''</summary>
            ERROR_INVALID_ADDRESS = 487L

            '''<summary>
            ''' Arithmetic result exceeded 32-
            ''' bits.
            '''</summary>
            ERROR_ARITHMETIC_OVERFLOW = 534L

            '''<summary>
            ''' There is a process on other
            ''' end of the pipe.
            '''</summary>
            ERROR_PIPE_CONNECTED = 535L

            '''<summary>
            ''' Waiting for a process to open
            ''' the other end of the pipe.
            '''</summary>
            ERROR_PIPE_LISTENING = 536L

            '''<summary>
            ''' Access to the EA is denied.
            '''</summary>
            ERROR_EA_ACCESS_DENIED = 994L

            '''<summary>
            ''' The I/O operation was aborted
            ''' due to either thread exit or
            ''' application request.
            '''</summary>
            ERROR_OPERATION_ABORTED = 995L

            '''<summary>
            ''' Overlapped IO event not in
            ''' signaled state.
            '''</summary>
            ERROR_IO_INCOMPLETE = 996L

            '''<summary>
            ''' Overlapped IO operation in
            ''' progress.
            '''</summary>
            ERROR_IO_PENDING = 997L

            '''<summary>
            ''' Invalid access to memory
            ''' location.
            '''</summary>
            ERROR_NOACCESS = 998L

            '''<summary>
            ''' Error accessing paging file.
            '''</summary>
            ERROR_SWAPERROR = 999L

            '''<summary>
            ''' Recursion too deep, stack
            ''' overflowed.
            '''</summary>
            ERROR_STACK_OVERFLOW = 1001L

            '''<summary>
            ''' Window can't handle sent
            ''' message.
            '''</summary>
            ERROR_INVALID_MESSAGE = 1002L

            '''<summary>
            ''' Cannot complete function for
            ''' some reason.
            '''</summary>
            ERROR_CAN_NOT_COMPLETE = 1003L

            '''<summary>
            ''' The flags are invalid.
            '''</summary>
            ERROR_INVALID_FLAGS = 1004L

            '''<summary>
            ''' The volume does not contain a
            ''' recognized file system. Make
            ''' sure that all required file
            ''' system drivers are loaded and
            ''' the volume is not damaged.
            '''</summary>
            ERROR_UNRECOGNIZED_VOLUME = 1005L

            '''<summary>
            ''' The volume for a file was
            ''' externally altered and the
            ''' opened file is no longer
            ''' valid.
            '''</summary>
            ERROR_FILE_INVALID = 1006L

            '''<summary>
            ''' The requested operation cannot
            ''' be performed in full-screen
            ''' mode.
            '''</summary>
            ERROR_FULLSCREEN_MODE = 1007L

            '''<summary>
            ''' An attempt was made to
            ''' reference a token that does
            ''' not exist.
            '''</summary>
            ERROR_NO_TOKEN = 1008L

            '''<summary>
            ''' The configuration registry
            ''' database is damaged.
            '''</summary>
            ERROR_BADDB = 1009L

            '''<summary>
            ''' The configuration registry key
            ''' is invalid.
            '''</summary>
            ERROR_BADKEY = 1010L

            '''<summary>
            ''' The configuration registry key
            ''' cannot be opened.
            '''</summary>
            ERROR_CANTOPEN = 1011L

            '''<summary>
            ''' The configuration registry key
            ''' cannot be read.
            '''</summary>
            ERROR_CANTREAD = 1012L

            '''<summary>
            ''' The configuration registry key
            ''' cannot be written.
            '''</summary>
            ERROR_CANTWRITE = 1013L

            '''<summary>
            ''' One of the files containing
            ''' the system's registry data had
            ''' to be recovered by use of a
            ''' log or alternate copy. The
            ''' recovery succeeded.
            '''</summary>
            ERROR_REGISTRY_RECOVERED = 1014L

            '''<summary>
            ''' The registry is damaged. The
            ''' structure of one of the files
            ''' that contains registry data is
            ''' damaged, or the system's in
            ''' memory image of the file is
            ''' damaged, or the file could not
            ''' be recovered because its
            ''' alternate copy or log was
            ''' absent or damaged.
            '''</summary>
            ERROR_REGISTRY_CORRUPT = 1015L

            '''<summary>
            ''' The registry initiated an I/O
            ''' operation that had an
            ''' unrecoverable failure. The
            ''' registry could not read in, or
            ''' write out, or flush, one of
            ''' the files that contain the
            ''' system's image of the
            ''' registry.
            '''</summary>
            ERROR_REGISTRY_IO_FAILED = 1016L

            '''<summary>
            ''' The system attempted to load
            ''' or restore a file into the
            ''' registry, and the specified
            ''' file is not in the format of a
            ''' registry file.
            '''</summary>
            ERROR_NOT_REGISTRY_FILE = 1017L

            '''<summary>
            ''' Illegal operation attempted on
            ''' a registry key that has been
            ''' marked for deletion.
            '''</summary>
            ERROR_KEY_DELETED = 1018L

            '''<summary>
            ''' System could not allocate
            ''' required space in a registry
            ''' log.
            '''</summary>
            ERROR_NO_LOG_SPACE = 1019L

            '''<summary>
            ''' An attempt was made to create
            ''' a symbolic link in a registry
            ''' key that already has subkeys
            ''' or values.
            '''</summary>
            ERROR_KEY_HAS_CHILDREN = 1020L

            '''<summary>
            ''' An attempt was made to create
            ''' a stable subkey under a
            ''' volatile parent key.
            '''</summary>
            ERROR_CHILD_MUST_BE_VOLATILE = 1021L

            '''<summary>
            ''' This indicates that a notify
            ''' change request is being
            ''' completed and the information
            ''' is not being returned in the
            ''' caller's buffer. The caller
            ''' now needs to enumerate the
            ''' files to find the changes.
            '''</summary>
            ERROR_NOTIFY_ENUM_DIR = 1022L

            '''<summary>
            ''' A stop control has been sent
            ''' to a service which other
            ''' running services are dependent
            ''' on.
            '''</summary>
            ERROR_DEPENDENT_SERVICES_RUNNING = 1051L

            '''<summary>
            ''' The requested control is not
            ''' valid for this service.
            '''</summary>
            ERROR_INVALID_SERVICE_CONTROL = 1052L

            '''<summary>
            ''' The service did not respond to
            ''' the start or control request
            ''' in a timely fashion.
            '''</summary>
            ERROR_SERVICE_REQUEST_TIMEOUT = 1053L

            '''<summary>
            ''' A thread could not be created
            ''' for the service.
            '''</summary>
            ERROR_SERVICE_NO_THREAD = 1054L

            '''<summary>
            ''' The service database is
            ''' locked.
            '''</summary>
            ERROR_SERVICE_DATABASE_LOCKED = 1055L

            '''<summary>
            ''' An instance of the service is
            ''' already running.
            '''</summary>
            ERROR_SERVICE_ALREADY_RUNNING = 1056L

            '''<summary>
            ''' The account name is invalid or
            ''' does not exist.
            '''</summary>
            ERROR_INVALID_SERVICE_ACCOUNT = 1057L

            '''<summary>
            ''' The specified service is
            ''' disabled and cannot be
            ''' started.
            '''</summary>
            ERROR_SERVICE_DISABLED = 1058L

            '''<summary>
            ''' Circular service dependency
            ''' was specified.
            '''</summary>
            ERROR_CIRCULAR_DEPENDENCY = 1059L

            '''<summary>
            ''' The specified service does not
            ''' exist as an installed service.
            '''</summary>
            ERROR_SERVICE_DOES_NOT_EXIST = 1060L

            '''<summary>
            ''' The service cannot accept
            ''' control messages at this time.
            '''</summary>
            ERROR_SERVICE_CANNOT_ACCEPT_CTRL = 1061L

            '''<summary>
            ''' The service has not been
            ''' started.
            '''</summary>
            ERROR_SERVICE_NOT_ACTIVE = 1062L

            '''<summary>
            ''' The service process could
            ''' not connect to the service
            ''' controller.
            '''</summary>
            ERROR_FAILED_SERVICE_CONTROLLER_CONNECT = 1063L

            '''<summary>
            ''' An exception occurred in the
            ''' service when handling the
            ''' control request.
            '''</summary>
            ERROR_EXCEPTION_IN_SERVICE = 1064L

            '''<summary>
            ''' The database specified does
            ''' not exist.
            '''</summary>
            ERROR_DATABASE_DOES_NOT_EXIST = 1065L

            '''<summary>
            ''' The service has returned a
            ''' service-specific error code.
            '''</summary>
            ERROR_SERVICE_SPECIFIC_ERROR = 1066L

            '''<summary>
            ''' The process terminated
            ''' unexpectedly.
            '''</summary>
            ERROR_PROCESS_ABORTED = 1067L

            '''<summary>
            ''' The dependency service or
            ''' group failed to start.
            '''</summary>
            ERROR_SERVICE_DEPENDENCY_FAIL = 1068L

            '''<summary>
            ''' The service did not start due
            ''' to a logon failure.
            '''</summary>
            ERROR_SERVICE_LOGON_FAILED = 1069L

            '''<summary>
            ''' After starting, the service
            ''' hung in a start-pending state.
            '''</summary>
            ERROR_SERVICE_START_HANG = 1070L

            '''<summary>
            ''' The specified service database
            ''' lock is invalid.
            '''</summary>
            ERROR_INVALID_SERVICE_LOCK = 1071L

            '''<summary>
            ''' The specified service has been
            ''' marked for deletion.
            '''</summary>
            ERROR_SERVICE_MARKED_FOR_DELETE = 1072L

            '''<summary>
            ''' The specified service already
            ''' exists.
            '''</summary>
            ERROR_SERVICE_EXISTS = 1073L

            '''<summary>
            ''' The system is currently
            ''' running with the last-known-
            ''' good configuration.
            '''</summary>
            ERROR_ALREADY_RUNNING_LKG = 1074L

            '''<summary>
            ''' The dependency service does
            ''' not exist or has been marked
            ''' for deletion.
            '''</summary>
            ERROR_SERVICE_DEPENDENCY_DELETED = 1075L

            '''<summary>
            ''' The current boot has already
            ''' been accepted for use as the
            ''' last-known-good control set.
            '''</summary>
            ERROR_BOOT_ALREADY_ACCEPTED = 1076L

            '''<summary>
            ''' No attempts to start the
            ''' service have been made since
            ''' the last boot.
            '''</summary>
            ERROR_SERVICE_NEVER_STARTED = 1077L

            '''<summary>
            ''' The name is already in use as
            ''' either a service name or a
            ''' service display name.
            '''</summary>
            ERROR_DUPLICATE_SERVICE_NAME = 1078L

            '''<summary>
            ''' The account specified for this
            ''' service is different from the
            ''' account specified for other
            ''' services running in the same
            ''' process.
            '''</summary>
            ERROR_DIFFERENT_SERVICE_ACCOUNT = 1079L

            '''<summary>
            ''' The physical end of the tape
            ''' has been reached.
            '''</summary>
            ERROR_END_OF_MEDIA = 1100L

            '''<summary>
            ''' A tape access reached a
            ''' filemark.
            '''</summary>
            ERROR_FILEMARK_DETECTED = 1101L

            '''<summary>
            ''' The beginning of the tape or
            ''' partition was encountered.
            '''</summary>
            ERROR_BEGINNING_OF_MEDIA = 1102L

            '''<summary>
            ''' A tape access reached a
            ''' setmark.
            '''</summary>
            ERROR_SETMARK_DETECTED = 1103L

            '''<summary>
            ''' During a tape access, the end
            ''' of the data marker was
            ''' reached.
            '''</summary>
            ERROR_NO_DATA_DETECTED = 1104L

            '''<summary>
            ''' Tape could not be partitioned.
            '''</summary>
            ERROR_PARTITION_FAILURE = 1105L

            '''<summary>
            ''' When accessing a new tape of a
            ''' multivolume partition, the
            ''' current block size is
            ''' incorrect.
            '''</summary>
            ERROR_INVALID_BLOCK_LENGTH = 1106L

            '''<summary>
            ''' Tape partition information
            ''' could not be found when
            ''' loading a tape.
            '''</summary>
            ERROR_DEVICE_NOT_PARTITIONED = 1107L

            '''<summary>
            ''' Attempt to lock the eject
            ''' media mechanism failed.
            '''</summary>
            ERROR_UNABLE_TO_LOCK_MEDIA = 1108L

            '''<summary>
            ''' Unload media failed.
            '''</summary>
            ERROR_UNABLE_TO_UNLOAD_MEDIA = 1109L

            '''<summary>
            ''' Media in drive may have
            ''' changed.
            '''</summary>
            ERROR_MEDIA_CHANGED = 1110L

            '''<summary>
            ''' The I/O bus was reset.
            '''</summary>
            ERROR_BUS_RESET = 1111L

            '''<summary>
            ''' Tape query failed because of
            ''' no media in drive.
            '''</summary>
            ERROR_NO_MEDIA_IN_DRIVE = 1112L

            '''<summary>
            ''' No mapping for the Unicode
            ''' character exists in the target
            ''' multi-byte code page.
            '''</summary>
            ERROR_NO_UNICODE_TRANSLATION = 1113L

            '''<summary>
            ''' A DLL initialization routine
            ''' failed.
            '''</summary>
            ERROR_DLL_INIT_FAILED = 1114L

            '''<summary>
            ''' A system shutdown is in
            ''' progress.
            '''</summary>
            ERROR_SHUTDOWN_IN_PROGRESS = 1115L

            '''<summary>
            ''' An attempt to abort the
            ''' shutdown of the system failed
            ''' because no shutdown was in
            ''' progress.
            '''</summary>
            ERROR_NO_SHUTDOWN_IN_PROGRESS = 1116L

            '''<summary>
            ''' The request could not be
            ''' performed because of an I/O
            ''' device error.
            '''</summary>
            ERROR_IO_DEVICE = 1117L

            '''<summary>
            ''' No serial device was
            ''' successfully initialized. The
            ''' serial driver will unload.
            '''</summary>
            ERROR_SERIAL_NO_DEVICE = 1118L

            '''<summary>
            ''' Unable to open a device that
            ''' was sharing an interrupt
            ''' request (IRQ) with other
            ''' devices. At least one other
            ''' device that uses that IRQ was
            ''' already opened.
            '''</summary>
            ERROR_IRQ_BUSY = 1119L

            '''<summary>
            ''' A serial I/O operation was
            ''' completed by another write to
            ''' the serial port. (The
            ''' IOCTL_SERIAL_XOFF_COUNTER
            ''' reached zero.)
            '''</summary>
            ERROR_MORE_WRITES = 1120L

            '''<summary>
            ''' A serial I/O operation
            ''' completed because the time-out
            ''' period expired. (The
            ''' IOCTL_SERIAL_XOFF_COUNTER did
            ''' not reach zero.)
            '''</summary>
            ERROR_COUNTER_TIMEOUT = 1121L

            '''<summary>
            ''' No ID address mark was found
            ''' on the floppy disk.
            '''</summary>
            ERROR_FLOPPY_ID_MARK_NOT_FOUND = 1122L

            '''<summary>
            ''' Mismatch between the floppy
            ''' disk sector ID field and the
            ''' floppy disk controller track
            ''' address.
            '''</summary>
            ERROR_FLOPPY_WRONG_CYLINDER = 1123L

            '''<summary>
            ''' The floppy disk controller
            ''' reported an error that is not
            ''' recognized by the floppy disk
            ''' driver.
            '''</summary>
            ERROR_FLOPPY_UNKNOWN_ERROR = 1124L

            '''<summary>
            ''' The floppy disk controller
            ''' returned inconsistent results
            ''' in its registers.
            '''</summary>
            ERROR_FLOPPY_BAD_REGISTERS = 1125L

            '''<summary>
            ''' While accessing the hard disk,
            ''' a recalibrate operation
            ''' failed, even after retries.
            '''</summary>
            ERROR_DISK_RECALIBRATE_FAILED = 1126L

            '''<summary>
            ''' While accessing the hard disk,
            ''' a disk operation failed even
            ''' after retries.
            '''</summary>
            ERROR_DISK_OPERATION_FAILED = 1127L

            '''<summary>
            ''' While accessing the hard disk,
            ''' a disk controller reset was
            ''' needed, but even that failed.
            '''</summary>
            ERROR_DISK_RESET_FAILED = 1128L

            '''<summary>
            ''' Physical end of tape
            ''' encountered.
            '''</summary>
            ERROR_EOM_OVERFLOW = 1129L

            '''<summary>
            ''' Not enough server storage is
            ''' available to process this
            ''' command.
            '''</summary>
            ERROR_NOT_ENOUGH_SERVER_MEMORY = 1130L

            '''<summary>
            ''' A potential deadlock condition
            ''' has been detected.
            '''</summary>
            ERROR_POSSIBLE_DEADLOCK = 1131L

            '''<summary>
            ''' The base address or the file
            ''' offset specified does not have
            ''' the proper alignment.
            '''</summary>
            ERROR_MAPPED_ALIGNMENT = 1132L

            '''<summary>
            ''' An attempt to change the
            ''' system power state was vetoed
            ''' by another application or
            ''' driver.
            '''</summary>
            ERROR_SET_POWER_STATE_VETOED = 1140L

            '''<summary>
            ''' The system BIOS failed an
            ''' attempt to change the system
            ''' power state.
            '''</summary>
            ERROR_SET_POWER_STATE_FAILED = 1141L

            '''<summary>
            ''' An attempt was made to create
            ''' more links on a file than the
            ''' file system supports.
            '''</summary>
            ERROR_TOO_MANY_LINKS = 1142L

            '''<summary>
            ''' The specified program requires
            ''' a newer version of Windows.
            '''</summary>
            ERROR_OLD_WIN_VERSION = 1150L

            '''<summary>
            ''' The specified program is not a
            ''' Windows or MS-DOS program.
            '''</summary>
            ERROR_APP_WRONG_OS = 1151L

            '''<summary>
            ''' Cannot start more than one
            ''' instance of the specified
            ''' program.
            '''</summary>
            ERROR_SINGLE_INSTANCE_APP = 1152L

            '''<summary>
            ''' The specified program was
            ''' written for an older version
            ''' of Windows.
            '''</summary>
            ERROR_RMODE_APP = 1153L

            '''<summary>
            ''' One of the library files
            ''' needed to run this application
            ''' is damaged.
            '''</summary>
            ERROR_INVALID_DLL = 1154L

            '''<summary>
            ''' No application is associated
            ''' with the specified file for
            ''' this operation.
            '''</summary>
            ERROR_NO_ASSOCIATION = 1155L

            '''<summary>
            ''' An error occurred in sending
            ''' the command to the
            ''' application.
            '''</summary>
            ERROR_DDE_FAIL = 1156L

            '''<summary>
            ''' One of the library files
            ''' needed to run this application
            ''' cannot be found.
            '''</summary>
            ERROR_DLL_NOT_FOUND = 1157L

            '''<summary>
            ''' The specified device name is
            ''' invalid.
            '''</summary>
            ERROR_BAD_DEVICE = 1200L

            '''<summary>
            ''' The device is not currently
            ''' connected but is a remembered
            ''' connection.
            '''</summary>
            ERROR_CONNECTION_UNAVAIL = 1201L

            '''<summary>
            ''' An attempt was made to
            ''' remember a device that was
            ''' previously remembered.
            '''</summary>
            ERROR_DEVICE_ALREADY_REMEMBERED = 1202L

            '''<summary>
            ''' No network provider accepted
            ''' the given network path.
            '''</summary>
            ERROR_NO_NET_OR_BAD_PATH = 1203L

            '''<summary>
            ''' The specified network provider
            ''' name is invalid.
            '''</summary>
            ERROR_BAD_PROVIDER = 1204L

            '''<summary>
            ''' Unable to open the network
            ''' connection profile.
            '''</summary>
            ERROR_CANNOT_OPEN_PROFILE = 1205L

            '''<summary>
            ''' The network connection profile
            ''' is damaged.
            '''</summary>
            ERROR_BAD_PROFILE = 1206L

            '''<summary>
            ''' Cannot enumerate a non-
            ''' container.
            '''</summary>
            ERROR_NOT_CONTAINER = 1207L

            '''<summary>
            ''' An extended error has
            ''' occurred.
            '''</summary>
            ERROR_EXTENDED_ERROR = 1208L

            '''<summary>
            ''' The format of the specified
            ''' group name is invalid.
            '''</summary>
            ERROR_INVALID_GROUPNAME = 1209L

            '''<summary>
            ''' The format of the specified
            ''' computer name is invalid.
            '''</summary>
            ERROR_INVALID_COMPUTERNAME = 1210L

            '''<summary>
            ''' The format of the specified
            ''' event name is invalid.
            '''</summary>
            ERROR_INVALID_EVENTNAME = 1211L

            '''<summary>
            ''' The format of the specified
            ''' domain name is invalid.
            '''</summary>
            ERROR_INVALID_DOMAINNAME = 1212L

            '''<summary>
            ''' The format of the specified
            ''' service name is invalid.
            '''</summary>
            ERROR_INVALID_SERVICENAME = 1213L

            '''<summary>
            ''' The format of the specified
            ''' network name is invalid.
            '''</summary>
            ERROR_INVALID_NETNAME = 1214L

            '''<summary>
            ''' The format of the specified
            ''' share name is invalid.
            '''</summary>
            ERROR_INVALID_SHARENAME = 1215L

            '''<summary>
            ''' The format of the specified
            ''' password is invalid.
            '''</summary>
            ERROR_INVALID_PASSWORDNAME = 1216L

            '''<summary>
            ''' The format of the specified
            ''' message name is invalid.
            '''</summary>
            ERROR_INVALID_MESSAGENAME = 1217L

            '''<summary>
            ''' The format of the specified
            ''' message destination is
            ''' invalid.
            '''</summary>
            ERROR_INVALID_MESSAGEDEST = 1218L

            '''<summary>
            ''' The credentials supplied
            ''' conflict with an existing set
            ''' of credentials.
            '''</summary>
            ERROR_SESSION_CREDENTIAL_CONFLICT = 1219L

            '''<summary>
            ''' An attempt was made to
            ''' establish a session to a LAN
            ''' Manager server, but there are
            ''' already too many sessions
            ''' established to that server.
            '''</summary>
            ERROR_REMOTE_SESSION_LIMIT_EXCEEDED = 1220L

            '''<summary>
            ''' The workgroup or domain name
            ''' is already in use by another
            ''' computer on the network.
            '''</summary>
            ERROR_DUP_DOMAINNAME = 1221L

            '''<summary>
            ''' The network is not present or
            ''' not started.
            '''</summary>
            ERROR_NO_NETWORK = 1222L

            '''<summary>
            ''' The operation was cancelled by
            ''' the user.
            '''</summary>
            ERROR_CANCELLED = 1223L

            '''<summary>
            ''' The requested operation cannot
            ''' be performed on a file with a
            ''' user mapped section open.
            '''</summary>
            ERROR_USER_MAPPED_FILE = 1224L

            '''<summary>
            ''' The remote system refused the
            ''' network connection.
            '''</summary>
            ERROR_CONNECTION_REFUSED = 1225L

            '''<summary>
            ''' The network connection was
            ''' gracefully closed.
            '''</summary>
            ERROR_GRACEFUL_DISCONNECT = 1226L

            '''<summary>
            ''' The network transport endpoint
            ''' already has an address
            ''' associated with it.
            '''</summary>
            ERROR_ADDRESS_ALREADY_ASSOCIATED = 1227L

            '''<summary>
            ''' An address has not yet been
            ''' associated with the network
            ''' endpoint.
            '''</summary>
            ERROR_ADDRESS_NOT_ASSOCIATED = 1228L

            '''<summary>
            ''' An operation was attempted on
            ''' a non-existent network
            ''' connection.
            '''</summary>
            ERROR_CONNECTION_INVALID = 1229L

            '''<summary>
            ''' An invalid operation was
            ''' attempted on an active network
            ''' connection.
            '''</summary>
            ERROR_CONNECTION_ACTIVE = 1230L

            '''<summary>
            ''' The remote network is not
            ''' reachable by the transport.
            '''</summary>
            ERROR_NETWORK_UNREACHABLE = 1231L

            '''<summary>
            ''' The remote system is not
            ''' reachable by the transport.
            '''</summary>
            ERROR_HOST_UNREACHABLE = 1232L

            '''<summary>
            ''' The remote system does not
            ''' support the transport
            ''' protocol.
            '''</summary>
            ERROR_PROTOCOL_UNREACHABLE = 1233L

            '''<summary>
            ''' No service is operating at the
            ''' destination network endpoint
            ''' on the remote system.
            '''</summary>
            ERROR_PORT_UNREACHABLE = 1234L

            '''<summary>
            ''' The request was aborted.
            '''</summary>
            ERROR_REQUEST_ABORTED = 1235L

            '''<summary>
            ''' The network connection was
            ''' aborted by the local system.
            '''</summary>
            ERROR_CONNECTION_ABORTED = 1236L

            '''<summary>
            ''' The operation could not be
            ''' completed. A retry should be
            ''' performed.
            '''</summary>
            ERROR_RETRY = 1237L

            '''<summary>
            ''' A connection to the server
            ''' could not be made because the
            ''' limit on the number of
            ''' concurrent connections for
            ''' this account has been reached.
            '''</summary>
            ERROR_CONNECTION_COUNT_LIMIT = 1238L

            '''<summary>
            ''' Attempting to login during an
            ''' unauthorized time of day for
            ''' this account.
            '''</summary>
            ERROR_LOGIN_TIME_RESTRICTION = 1239L

            '''<summary>
            ''' The account is not authorized
            ''' to login from this station.
            '''</summary>
            ERROR_LOGIN_WKSTA_RESTRICTION = 1240L

            '''<summary>
            ''' The network address could not
            ''' be used for the operation
            ''' requested.
            '''</summary>
            ERROR_INCORRECT_ADDRESS = 1241L

            '''<summary>
            ''' The service is already
            ''' registered.
            '''</summary>
            ERROR_ALREADY_REGISTERED = 1242L

            '''<summary>
            ''' The specified service does not
            ''' exist.
            '''</summary>
            ERROR_SERVICE_NOT_FOUND = 1243L

            '''<summary>
            ''' The operation being requested
            ''' was not performed because the
            ''' user has not been
            ''' authenticated.
            '''</summary>
            ERROR_NOT_AUTHENTICATED = 1244L

            '''<summary>
            ''' The operation being requested
            ''' was not performed because the
            ''' user has not logged on to the
            ''' network.
            '''</summary>
            ERROR_NOT_LOGGED_ON = 1245L

            '''<summary>
            ''' Return that wants caller to
            ''' continue with work in
            ''' progress.
            '''</summary>
            ERROR_CONTINUE = 1246L

            '''<summary>
            ''' An attempt was made to perform
            ''' an initialization operation
            ''' when initialization has
            ''' already been completed.
            '''</summary>
            ERROR_ALREADY_INITIALIZED = 1247L

            '''<summary>
            ''' No more local devices.
            '''</summary>
            ERROR_NO_MORE_DEVICES = 1248L


            '''<summary>
            ''' Indicates not all privileges
            ''' referenced are assigned to the
            ''' caller. This allows, for
            ''' example, all privileges to be
            ''' disabled without having to
            ''' know exactly which privileges
            ''' are assigned.
            '''</summary>
            ERROR_NOT_ALL_ASSIGNED = 1300L

            '''<summary>
            ''' Some of the information to be
            ''' mapped has not been
            ''' translated.
            '''</summary>
            ERROR_SOME_NOT_MAPPED = 1301L

            '''<summary>
            ''' No system quota limits are
            ''' specifically set for this
            ''' account.
            '''</summary>
            ERROR_NO_QUOTAS_FOR_ACCOUNT = 1302L

            '''<summary>
            ''' A user session key was
            ''' requested for a local RPC
            ''' connection. The session key
            ''' returned is a constant value
            ''' and not unique to this
            ''' connection.
            '''</summary>
            ERROR_LOCAL_USER_SESSION_KEY = 1303L

            '''<summary>
            ''' The Windows NT password is too
            ''' complex to be converted to a
            ''' Windows-networking password.
            ''' The Windows-networking
            ''' password returned is a NULL
            ''' string.
            '''</summary>
            ERROR_NULL_LM_PASSWORD = 1304L

            '''<summary>
            ''' Indicates an encountered or
            ''' specified revision number is
            ''' not one known by the service.
            ''' The service may not be aware
            ''' of a more recent revision.
            '''</summary>
            ERROR_UNKNOWN_REVISION = 1305L

            '''<summary>
            ''' Indicates two revision levels
            ''' are incompatible.
            '''</summary>
            ERROR_REVISION_MISMATCH = 1306L

            '''<summary>
            ''' Indicates a particular
            ''' Security ID cannot be assigned
            ''' as the owner of an object.
            '''</summary>
            ERROR_INVALID_OWNER = 1307L

            '''<summary>
            ''' Indicates a particular
            ''' Security ID cannot be assigned
            ''' as the primary group of an
            ''' object.
            '''</summary>
            ERROR_INVALID_PRIMARY_GROUP = 1308L

            '''<summary>
            ''' An attempt was made to operate
            ''' on an impersonation token by a
            ''' thread was not currently
            ''' impersonating a client.
            '''</summary>
            ERROR_NO_IMPERSONATION_TOKEN = 1309L

            '''<summary>
            ''' A mandatory group cannot be
            ''' disabled.
            '''</summary>
            ERROR_CANT_DISABLE_MANDATORY = 1310L

            '''<summary>
            ''' There are currently no logon
            ''' servers available to service
            ''' the logon request.
            '''</summary>
            ERROR_NO_LOGON_SERVERS = 1311L

            '''<summary>
            ''' A specified logon session does
            ''' not exist. It may already have
            ''' been terminated.
            '''</summary>
            ERROR_NO_SUCH_LOGON_SESSION = 1312L

            '''<summary>
            ''' A specified privilege does not
            ''' exist.
            '''</summary>
            ERROR_NO_SUCH_PRIVILEGE = 1313L

            '''<summary>
            ''' A required privilege is not
            ''' held by the client.
            '''</summary>
            ERROR_PRIVILEGE_NOT_HELD = 1314L

            '''<summary>
            ''' The name provided is not a
            ''' properly formed account name.
            '''</summary>
            ERROR_INVALID_ACCOUNT_NAME = 1315L

            '''<summary>
            ''' The specified user already
            ''' exists.
            '''</summary>
            ERROR_USER_EXISTS = 1316L

            '''<summary>
            ''' The specified user does not
            ''' exist.
            '''</summary>
            ERROR_NO_SUCH_USER = 1317L

            '''<summary>
            ''' The specified group already
            ''' exists.
            ''' 
            '''</summary>
            ERROR_GROUP_EXISTS = 1318L

            '''<summary>
            ''' The specified group does not
            ''' exist.
            '''</summary>
            ERROR_NO_SUCH_GROUP = 1319L

            '''<summary>
            ''' The specified user account is
            ''' already in the specified group
            ''' account. Also used to indicate
            ''' a group can not be deleted
            ''' because it contains a member.
            '''</summary>
            ERROR_MEMBER_IN_GROUP = 1320L

            '''<summary>
            ''' The specified user account is
            ''' not a member of the specified
            ''' group account.
            '''</summary>
            ERROR_MEMBER_NOT_IN_GROUP = 1321L

            '''<summary>
            ''' Indicates the requested
            ''' operation would disable or
            ''' delete the last remaining
            ''' administration account. This
            ''' is not allowed to prevent
            ''' creating a situation where the
            ''' system will not be
            ''' administrable.
            '''</summary>
            ERROR_LAST_ADMIN = 1322L

            '''<summary>
            ''' When trying to update a
            ''' password, this return status
            ''' indicates the value provided
            ''' as the current password is
            ''' incorrect.
            '''</summary>
            ERROR_WRONG_PASSWORD = 1323L

            '''<summary>
            ''' When trying to update a
            ''' password, this return status
            ''' indicates the value provided
            ''' for the new password contains
            ''' values not allowed in
            ''' passwords.
            '''</summary>
            ERROR_ILL_FORMED_PASSWORD = 1324L

            '''<summary>
            ''' When trying to update a
            ''' password, this status
            ''' indicates that some password
            ''' update rule was violated. For
            ''' example, the password may not
            ''' meet length criteria.
            '''</summary>
            ERROR_PASSWORD_RESTRICTION = 1325L

            '''<summary>
            ''' The attempted logon is
            ''' invalid. This is due to either
            ''' a bad user name or
            ''' authentication information.
            '''</summary>
            ERROR_LOGON_FAILURE = 1326L

            '''<summary>
            ''' Indicates a referenced user
            ''' name and authentication
            ''' information are valid, but
            ''' some user account restriction
            ''' has prevented successful
            ''' authentication (such as time-
            ''' of-day restrictions).
            '''</summary>
            ERROR_ACCOUNT_RESTRICTION = 1327L

            '''<summary>
            ''' The user account has time
            ''' restrictions and cannot be
            ''' logged onto at this time.
            '''</summary>
            ERROR_INVALID_LOGON_HOURS = 1328L

            '''<summary>
            ''' The user account is restricted
            ''' and cannot be used to log on
            ''' from the source workstation.
            '''</summary>
            ERROR_INVALID_WORKSTATION = 1329L

            '''<summary>
            ''' The user account's password
            ''' has expired.
            '''</summary>
            ERROR_PASSWORD_EXPIRED = 1330L

            '''<summary>
            ''' The referenced account is
            ''' currently disabled and cannot
            ''' be logged on to.
            '''</summary>
            ERROR_ACCOUNT_DISABLED = 1331L

            '''<summary>
            ''' None of the information to be
            ''' mapped has been translated.
            '''</summary>
            ERROR_NONE_MAPPED = 1332L

            '''<summary>
            ''' The number of LUID requested
            ''' cannot be allocated with a
            ''' single allocation.
            '''</summary>
            ERROR_TOO_MANY_LUIDS_REQUESTED = 1333L

            '''<summary>
            ''' Indicates there are no more
            ''' LUID to allocate.
            '''</summary>
            ERROR_LUIDS_EXHAUSTED = 1334L

            '''<summary>
            ''' Indicates the sub-authority
            ''' value is invalid for the
            ''' particular use.
            '''</summary>
            ERROR_INVALID_SUB_AUTHORITY = 1335L

            '''<summary>
            ''' Indicates the ACL structure is
            ''' not valid.
            '''</summary>
            ERROR_INVALID_ACL = 1336L

            '''<summary>
            ''' Indicates the SID structure is
            ''' invalid.
            '''</summary>
            ERROR_INVALID_SID = 1337L

            '''<summary>
            ''' Indicates the
            ''' SECURITY_DESCRIPTOR structure
            ''' is invalid.
            '''</summary>
            ERROR_INVALID_SECURITY_DESCR = 1338L

            '''<summary>
            ''' Indicates that an attempt to
            ''' build either an inherited ACL
            ''' or ACE did not succeed. One of
            ''' the more probable causes is
            ''' the replacement of a CreatorId
            ''' with an SID that didn't fit
            ''' into the ACE or ACL.
            '''</summary>
            ERROR_BAD_INHERITANCE_ACL = 1340L

            '''<summary>
            ''' The GUID allocation server is
            ''' already disabled at the
            ''' moment.
            '''</summary>
            ERROR_SERVER_DISABLED = 1341L

            '''<summary>
            ''' The GUID allocation server is
            ''' already enabled at the moment.
            '''</summary>
            ERROR_SERVER_NOT_DISABLED = 1342L

            '''<summary>
            ''' The value provided is an
            ''' invalid value for an
            ''' identifier authority.
            '''</summary>
            ERROR_INVALID_ID_AUTHORITY = 1343L

            '''<summary>
            ''' When a block of memory is
            ''' allotted for future updates,
            ''' such as the memory allocated
            ''' to hold discretionary access
            ''' control and primary group
            ''' information, successive
            ''' updates may exceed the amount
            ''' of memory originally allotted.
            ''' Since quota may already have
            ''' been charged to several
            ''' processes that have handles of
            ''' the object, it is not
            ''' reasonable to alter the size
            ''' of the allocated memory.
            ''' Instead, a request that
            ''' requires more memory than has
            ''' been allotted must fail and
            ''' the
            ''' ERROR_ALLOTTED_SPACE_EXCEEDED
            ''' error returned.
            '''</summary>
            ERROR_ALLOTTED_SPACE_EXCEEDED = 1344L

            '''<summary>
            ''' The specified attributes are
            ''' invalid, or incompatible with
            ''' the attributes for the group
            ''' as a whole.
            '''</summary>
            ERROR_INVALID_GROUP_ATTRIBUTES = 1345L

            '''<summary>
            ''' A specified impersonation
            ''' level is invalid. Also used to
            ''' indicate a required
            ''' impersonation level was not
            ''' provided.
            '''</summary>
            ERROR_BAD_IMPERSONATION_LEVEL = 1346L

            '''<summary>
            ''' An attempt was made to open an
            ''' anonymous level token.
            ''' Anonymous tokens cannot be
            ''' opened.
            '''</summary>
            ERROR_CANT_OPEN_ANONYMOUS = 1347L

            '''<summary>
            ''' The requested validation
            ''' information class is invalid.
            '''</summary>
            ERROR_BAD_VALIDATION_CLASS = 1348L

            '''<summary>
            ''' The type of token object is
            ''' inappropriate for its
            ''' attempted use.
            '''</summary>
            ERROR_BAD_TOKEN_TYPE = 1349L

            '''<summary>
            ''' Indicates an attempt was made
            ''' to operate on the security of
            ''' an object that does not have
            ''' security associated with it.
            '''</summary>
            ERROR_NO_SECURITY_ON_OBJECT = 1350L

            '''<summary>
            ''' Indicates a domain controller
            ''' could not be contacted or that
            ''' objects within the domain are
            ''' protected and necessary
            ''' information could not be
            ''' retrieved.
            '''</summary>
            ERROR_CANT_ACCESS_DOMAIN_INFO = 1351L

            '''<summary>
            ''' Indicates the Sam Server was
            ''' in the wrong state to perform
            ''' the desired operation.
            '''</summary>
            ERROR_INVALID_SERVER_STATE = 1352L

            '''<summary>
            ''' Indicates the domain is in the
            ''' wrong state to perform the
            ''' desired operation.
            '''</summary>
            ERROR_INVALID_DOMAIN_STATE = 1353L

            '''<summary>
            ''' Indicates the requested
            ''' operation cannot be completed
            ''' with the domain in its present
            ''' role.
            '''</summary>
            ERROR_INVALID_DOMAIN_ROLE = 1354L

            '''<summary>
            ''' The specified domain does not
            ''' exist.
            '''</summary>
            ERROR_NO_SUCH_DOMAIN = 1355L

            '''<summary>
            ''' The specified domain already
            ''' exists.
            '''</summary>
            ERROR_DOMAIN_EXISTS = 1356L

            '''<summary>
            ''' An attempt to exceed the limit
            ''' on the number of domains per
            ''' server for this release.
            '''</summary>
            ERROR_DOMAIN_LIMIT_EXCEEDED = 1357L

            '''<summary>
            ''' This error indicates the
            ''' requested operation cannot be
            ''' completed due to a
            ''' catastrophic media failure or
            ''' on-disk data structure
            ''' corruption.
            '''</summary>
            ERROR_INTERNAL_DB_CORRUPTION = 1358L

            '''<summary>
            ''' This error indicates the SAM
            ''' server has encounterred an
            ''' internal consistency error in
            ''' its database. This
            ''' catastrophic failure prevents
            ''' further operation of SAM.
            '''</summary>
            ERROR_INTERNAL_ERROR = 1359L

            '''<summary>
            ''' Indicates generic access types
            ''' were contained in an access
            ''' mask that should already be
            ''' mapped to non-generic access
            ''' types.
            '''</summary>
            ERROR_GENERIC_NOT_MAPPED = 1360L

            '''<summary>
            ''' Indicates a security
            ''' descriptor is not in the
            ''' required format (absolute or
            ''' self-relative).
            '''</summary>
            ERROR_BAD_DESCRIPTOR_FORMAT = 1361L

            '''<summary>
            ''' The requested action is
            ''' restricted for use by logon
            ''' processes only. The calling
            ''' process has not registered as
            ''' a logon process.
            '''</summary>
            ERROR_NOT_LOGON_PROCESS = 1362L

            '''<summary>
            ''' An attempt was made to start a
            ''' new session manager or LSA
            ''' logon session with an ID
            ''' already in use.
            '''</summary>
            ERROR_LOGON_SESSION_EXISTS = 1363L

            '''<summary>
            ''' A specified authentication
            ''' package is unknown.
            '''</summary>
            ERROR_NO_SUCH_PACKAGE = 1364L

            '''<summary>
            ''' The logon session is not in a
            ''' state consistent with the
            ''' requested operation.
            '''</summary>
            ERROR_BAD_LOGON_SESSION_STATE = 1365L

            '''<summary>
            ''' The logon session ID is
            ''' already in use.
            '''</summary>
            ERROR_LOGON_SESSION_COLLISION = 1366L

            '''<summary>
            ''' Indicates an invalid value has
            ''' been provided for LogonType
            ''' has been requested.
            '''</summary>
            ERROR_INVALID_LOGON_TYPE = 1367L

            '''<summary>
            ''' Indicates that an attempt was
            ''' made to impersonate via a
            ''' named pipe was not yet read
            ''' from.
            '''</summary>
            ERROR_CANNOT_IMPERSONATE = 1368L

            '''<summary>
            ''' Indicates that the transaction
            ''' state of a registry sub-tree
            ''' is incompatible with the
            ''' requested operation. For
            ''' example, a request has been
            ''' made to start a new
            ''' transaction with one already
            ''' in progress, or a request to
            ''' apply a transaction when one
            ''' is not currently in progress.
            ''' This status value is returned
            ''' by the runtime library (RTL)
            ''' registry transaction package
            ''' (RXact).
            '''</summary>
            ERROR_RXACT_INVALID_STATE = 1369L

            '''<summary>
            ''' Indicates an error occurred
            ''' during a registry transaction
            ''' commit. The database has been
            ''' left in an unknown state. The
            ''' state of the registry
            ''' transaction is left as
            ''' COMMITTING. This status value
            ''' is returned by the runtime
            ''' library (RTL) registry
            ''' transaction package (RXact).
            '''</summary>
            ERROR_RXACT_COMMIT_FAILURE = 1370L

            '''<summary>
            ''' Indicates an operation was
            ''' attempted on a built-in
            ''' (special) SAM account that is
            ''' incompatible with built-in
            ''' accounts. For example, built-
            ''' in accounts cannot be renamed
            ''' or deleted.
            '''</summary>
            ERROR_SPECIAL_ACCOUNT = 1371L

            '''<summary>
            ''' The requested operation cannot
            ''' be performed on the specified
            ''' group because it is a built-in
            ''' special group.
            '''</summary>
            ERROR_SPECIAL_GROUP = 1372L

            '''<summary>
            ''' The requested operation cannot
            ''' be performed on the specified
            ''' user because it is a built-in
            ''' special user.
            '''</summary>
            ERROR_SPECIAL_USER = 1373L

            '''<summary>
            ''' Indicates a member cannot be
            ''' removed from a group because
            ''' the group is currently the
            ''' member's primary group.
            '''</summary>
            ERROR_MEMBERS_PRIMARY_GROUP = 1374L

            '''<summary>
            ''' An attempt was made to
            ''' establish a token for use as a
            ''' primary token but the token is
            ''' already in use. A token can
            ''' only be the primary token of
            ''' one process at a time.
            '''</summary>
            ERROR_TOKEN_ALREADY_IN_USE = 1375L

            '''<summary>
            ''' The specified alias does not
            ''' exist.
            '''</summary>
            ERROR_NO_SUCH_ALIAS = 1376L

            '''<summary>
            ''' The specified account name is
            ''' not a member of the alias.
            '''</summary>
            ERROR_MEMBER_NOT_IN_ALIAS = 1377L

            '''<summary>
            ''' The specified account name is
            ''' not a member of the alias.
            '''</summary>
            ERROR_MEMBER_IN_ALIAS = 1378L

            '''<summary>
            ''' The specified alias already
            ''' exists.
            '''</summary>
            ERROR_ALIAS_EXISTS = 1379L

            '''<summary>
            ''' A requested type of logon,
            ''' such as Interactive, Network,
            ''' or Service, is not granted by
            ''' the target system's local
            ''' security policy. The system
            ''' administrator can grant the
            ''' required form of logon.
            '''</summary>
            ERROR_LOGON_NOT_GRANTED = 1380L

            '''<summary>
            ''' The maximum number of secrets
            ''' that can be stored in a single
            ''' system was exceeded.
            '''</summary>
            ERROR_TOO_MANY_SECRETS = 1381L

            '''<summary>
            ''' The length of a secret exceeds
            ''' the maximum length allowed.
            '''</summary>
            ERROR_SECRET_TOO_LONG = 1382L

            '''<summary>
            ''' The Local Security Authority
            ''' (LSA) database contains in
            ''' internal inconsistency.
            '''</summary>
            ERROR_INTERNAL_DB_ERROR = 1383L

            '''<summary>
            ''' During a logon attempt, the
            ''' user's security context
            ''' accumulated too many security
            ''' IDs. Remove the user from some
            ''' groups or aliases to reduce
            ''' the number of security ids to
            ''' incorporate into the security
            ''' context.
            '''</summary>
            ERROR_TOO_MANY_CONTEXT_IDS = 1384L

            '''<summary>
            ''' A user has requested a type of
            ''' logon, such as interactive or
            ''' network, that was not granted.
            ''' An administrator has control
            ''' over who may logon
            ''' interactively and through the
            ''' network.
            '''</summary>
            ERROR_LOGON_TYPE_NOT_GRANTED = 1385L

            '''<summary>
            ''' An attempt was made to change
            ''' a user password in the
            ''' security account manager
            ''' without providing the
            ''' necessary NT cross-encrypted
            ''' password.
            '''</summary>
            ERROR_NT_CROSS_ENCRYPTION_REQUIRED = 1386L

            '''<summary>
            ''' A new member cannot be added
            ''' to an alias because the member
            ''' does not exist.
            '''</summary>
            ERROR_NO_SUCH_MEMBER = 1387L

            '''<summary>
            ''' A new member could not be
            ''' added to an alias because the
            ''' member has the wrong account
            ''' type.
            '''</summary>
            ERROR_INVALID_MEMBER = 1388L

            '''<summary>
            ''' Too many SIDs specified.
            '''</summary>
            ERROR_TOO_MANY_SIDS = 1389L

            '''<summary>
            ''' An attempt was made to change
            ''' a user password in the
            ''' security account manager
            ''' without providing the required
            ''' LM cross-encrypted password.
            '''</summary>
            ERROR_LM_CROSS_ENCRYPTION_REQUIRED = 1390L

            '''<summary>
            ''' Indicates an ACL contains no
            ''' inheritable components.
            '''</summary>
            ERROR_NO_INHERITANCE = 1391L

            '''<summary>
            ''' The file or directory is
            ''' damaged and nonreadable.
            '''</summary>
            ERROR_FILE_CORRUPT = 1392L

            '''<summary>
            ''' The disk structure is damaged
            ''' and nonreadable.
            '''</summary>
            ERROR_DISK_CORRUPT = 1393L

            '''<summary>
            ''' There is no user session key
            ''' for the specified logon
            ''' session.
            '''</summary>
            ERROR_NO_USER_SESSION_KEY = 1394L

            '''<summary>
            ''' The service being accessed is
            ''' licensed for a particular
            ''' number of connections. No more
            ''' connections can be made to the
            ''' service at this time because
            ''' there are already as many
            ''' connections as the service can
            ''' accept.
            '''</summary>
            ERROR_LICENSE_QUOTA_EXCEEDED = 1395L

            '''<summary>
            ''' The window handle invalid.
            '''</summary>
            ERROR_INVALID_WINDOW_HANDLE = 1400L

            '''<summary>
            ''' The menu handle is invalid.
            '''</summary>
            ERROR_INVALID_MENU_HANDLE = 1401L

            '''<summary>
            ''' The cursor handle is invalid.
            '''</summary>
            ERROR_INVALID_CURSOR_HANDLE = 1402L

            '''<summary>
            ''' Invalid accelerator-table
            ''' handle.
            '''</summary>
            ERROR_INVALID_ACCEL_HANDLE = 1403L

            '''<summary>
            ''' The hook handle is invalid.
            '''</summary>
            ERROR_INVALID_HOOK_HANDLE = 1404L

            '''<summary>
            ''' The DeferWindowPos handle is
            ''' invalid.
            '''</summary>
            ERROR_INVALID_DWP_HANDLE = 1405L

            '''<summary>
            ''' CreateWindow failed, creating
            ''' top-level window with WS_CHILD
            ''' style.
            '''</summary>
            ERROR_TLW_WITH_WSCHILD = 1406L

            '''<summary>
            ''' Cannot find window class.
            '''</summary>
            ERROR_CANNOT_FIND_WND_CLASS = 1407L

            '''<summary>
            ''' Invalid window, belongs to
            ''' other thread.
            '''</summary>
            ERROR_WINDOW_OF_OTHER_THREAD = 1408L

            '''<summary>
            ''' Hotkey is already registered.
            '''</summary>
            ERROR_HOTKEY_ALREADY_REGISTERED = 1409L

            '''<summary>
            ''' Class already exists.
            '''</summary>
            ERROR_CLASS_ALREADY_EXISTS = 1410L

            '''<summary>
            ''' Class does not exist.
            '''</summary>
            ERROR_CLASS_DOES_NOT_EXIST = 1411L

            '''<summary>
            ''' Class still has open windows.
            '''</summary>
            ERROR_CLASS_HAS_WINDOWS = 1412L

            '''<summary>
            ''' The index is invalid.
            '''</summary>
            ERROR_INVALID_INDEX = 1413L

            '''<summary>
            ''' The icon handle is invalid.
            '''</summary>
            ERROR_INVALID_ICON_HANDLE = 1414L

            '''<summary>
            ''' Using private DIALOG window
            ''' words.
            '''</summary>
            ERROR_PRIVATE_DIALOG_INDEX = 1415L

            '''<summary>
            ''' List box ID not found.
            '''</summary>
            ERROR_LISTBOX_ID_NOT_FOUND = 1416L

            '''<summary>
            ''' No wildcard characters found.
            '''</summary>
            ERROR_NO_WILDCARD_CHARACTERS = 1417L

            '''<summary>
            ''' Thread doesn't have clipboard
            ''' open.
            '''</summary>
            ERROR_CLIPBOARD_NOT_OPEN = 1418L

            '''<summary>
            ''' Hotkey is not registered.
            '''</summary>
            ERROR_HOTKEY_NOT_REGISTERED = 1419L

            '''<summary>
            ''' The window is not a valid
            ''' dialog window.
            '''</summary>
            ERROR_WINDOW_NOT_DIALOG = 1420L

            '''<summary>
            ''' Control ID not found.
            '''</summary>
            ERROR_CONTROL_ID_NOT_FOUND = 1421L

            '''<summary>
            ''' Invalid Message, combo box
            ''' doesn't have an edit control.
            '''</summary>
            ERROR_INVALID_COMBOBOX_MESSAGE = 1422L

            '''<summary>
            ''' The window is not a combo box.
            '''</summary>
            ERROR_WINDOW_NOT_COMBOBOX = 1423L

            '''<summary>
            ''' Height must be less than 256.
            '''</summary>
            ERROR_INVALID_EDIT_HEIGHT = 1424L

            '''<summary>
            ''' Invalid HDC passed to
            ''' ReleaseDC.
            '''</summary>
            ERROR_DC_NOT_FOUND = 1425L

            '''<summary>
            ''' The hook filter type is
            ''' invalid.
            '''</summary>
            ERROR_INVALID_HOOK_FILTER = 1426L

            '''<summary>
            ''' The filter proc is invalid.
            '''</summary>
            ERROR_INVALID_FILTER_PROC = 1427L

            '''<summary>
            ''' Cannot set non-local hook
            ''' without an module handle.
            '''</summary>
            ERROR_HOOK_NEEDS_HMOD = 1428L

            '''<summary>
            ''' This hook can only be set
            ''' globally.
            '''</summary>
            ERROR_GLOBAL_ONLY_HOOK = 1429L

            '''<summary>
            ''' The journal hook is already
            ''' installed.
            '''</summary>
            ERROR_JOURNAL_HOOK_SET = 1430L

            '''<summary>
            ''' Hook is not installed.
            '''</summary>
            ERROR_HOOK_NOT_INSTALLED = 1431L

            '''<summary>
            ''' The message for single-
            ''' selection list box is invalid.
            '''</summary>
            ERROR_INVALID_LB_MESSAGE = 1432L

            '''<summary>
            ''' LB_SETCOUNT sent to non-lazy
            ''' list box.
            '''</summary>
            ERROR_SETCOUNT_ON_BAD_LB = 1433L

            '''<summary>
            ''' This list box doesn't support
            ''' tab stops.
            '''</summary>
            ERROR_LB_WITHOUT_TABSTOPS = 1434L

            '''<summary>
            ''' Cannot destroy object created
            ''' by another thread.
            '''</summary>
            ERROR_DESTROY_OBJECT_OF_OTHER_THREAD = 1435L

            '''<summary>
            ''' Child windows can't have
            ''' menus.
            '''</summary>
            ERROR_CHILD_WINDOW_MENU = 1436L

            '''<summary>
            ''' Window does not have system
            ''' menu.
            '''</summary>
            ERROR_NO_SYSTEM_MENU = 1437L

            '''<summary>
            ''' The message box style is
            ''' invalid.
            '''</summary>
            ERROR_INVALID_MSGBOX_STYLE = 1438L

            '''<summary>
            ''' The SPI_* parameter is
            ''' invalid.
            '''</summary>
            ERROR_INVALID_SPI_VALUE = 1439L

            '''<summary>
            ''' Screen already locked.
            '''</summary>
            ERROR_SCREEN_ALREADY_LOCKED = 1440L

            '''<summary>
            ''' All DeferWindowPos HWNDs must
            ''' have same parent.
            '''</summary>
            ERROR_HWNDS_HAVE_DIFFERENT_PARENT = 1441L

            '''<summary>
            ''' Window is not a child window.
            '''</summary>
            ERROR_NOT_CHILD_WINDOW = 1442L

            '''<summary>
            ''' The GW_* command is invalid.
            '''</summary>
            ERROR_INVALID_GW_COMMAND = 1443L

            '''<summary>
            ''' The thread ID is invalid.
            '''</summary>
            ERROR_INVALID_THREAD_ID = 1444L

            '''<summary>
            ''' DefMDIChildProc called with a
            ''' non-MDI child window.
            '''</summary>
            ERROR_NON_MDICHILD_WINDOW = 1445L

            '''<summary>
            ''' Pop-up menu already active.
            '''</summary>
            ERROR_POPUP_ALREADY_ACTIVE = 1446L

            '''<summary>
            ''' Window does not have scroll
            ''' bars.
            '''</summary>
            ERROR_NO_SCROLLBARS = 1447L

            '''<summary>
            ''' Scrollbar range greater than
            ''' 0x7FFF.
            '''</summary>
            ERROR_INVALID_SCROLLBAR_RANGE = 1448L

            '''<summary>
            ''' The ShowWindow command is
            ''' invalid.
            '''</summary>
            ERROR_INVALID_SHOWWIN_COMMAND = 1449L

            '''<summary>
            ''' Insufficient system resources
            ''' exist to complete the
            ''' requested service.
            '''</summary>
            ERROR_NO_SYSTEM_RESOURCES = 1450L

            '''<summary>
            ''' Insufficient system resources
            ''' exist to complete the
            ''' requested service.
            '''</summary>
            ERROR_NONPAGED_SYSTEM_RESOURCES = 1451L

            '''<summary>
            ''' Insufficient system resources
            ''' exist to complete the
            ''' requested service.
            '''</summary>
            ERROR_PAGED_SYSTEM_RESOURCES = 1452L

            '''<summary>
            ''' Insufficient quota to complete
            ''' the requested service.
            '''</summary>
            ERROR_WORKING_SET_QUOTA = 1453L

            '''<summary>
            ''' Insufficient quota to complete
            ''' the requested service.
            '''</summary>
            ERROR_PAGEFILE_QUOTA = 1454L

            '''<summary>
            ''' The paging file is too small
            ''' for this operation to
            ''' complete.
            '''</summary>
            ERROR_COMMITMENT_LIMIT = 1455L

            '''<summary>
            ''' A menu item was not found.
            '''</summary>
            ERROR_MENU_ITEM_NOT_FOUND = 1456L

            '''<summary>
            ''' Invalid keyboard layout
            ''' handle.
            '''</summary>
            ERROR_INVALID_KEYBOARD_HANDLE = 1457L

            '''<summary>
            ''' Hook type not allowed.
            '''</summary>
            ERROR_HOOK_TYPE_NOT_ALLOWED = 1458L

            '''<summary>
            ''' One of the Eventlog logfiles
            ''' is damaged.
            '''</summary>
            ERROR_EVENTLOG_FILE_CORRUPT = 1500L

            '''<summary>
            ''' No event log file could be
            ''' opened, so the event logging
            ''' service did not start.
            '''</summary>
            ERROR_EVENTLOG_CANT_START = 1501L

            '''<summary>
            ''' The event log file is full.
            '''</summary>
            ERROR_LOG_FILE_FULL = 1502L

            '''<summary>
            ''' The event log file has changed
            ''' between reads.
            '''</summary>
            ERROR_EVENTLOG_FILE_CHANGED = 1503L

            '''<summary>
            ''' The string binding is invalid.
            '''</summary>
            RPC_S_INVALID_STRING_BINDING = 1700L

            '''<summary>
            ''' The binding handle is the
            ''' incorrect type.
            '''</summary>
            RPC_S_WRONG_KIND_OF_BINDING = 1701L

            '''<summary>
            ''' The binding handle is invalid.
            '''</summary>
            RPC_S_INVALID_BINDING = 1702L

            '''<summary>
            ''' The RPC protocol sequence is
            ''' not supported.
            '''</summary>
            RPC_S_PROTSEQ_NOT_SUPPORTED = 1703L

            '''<summary>
            ''' The RPC protocol sequence is
            ''' invalid.
            '''</summary>
            RPC_S_INVALID_RPC_PROTSEQ = 1704L

            '''<summary>
            ''' The string UUID is invalid.
            '''</summary>
            RPC_S_INVALID_STRING_UUID = 1705L

            '''<summary>
            ''' The endpoint format is
            ''' invalid.
            '''</summary>
            RPC_S_INVALID_ENDPOINT_FORMAT = 1706L

            '''<summary>
            ''' The network address is
            ''' invalid.
            '''</summary>
            RPC_S_INVALID_NET_ADDR = 1707L

            '''<summary>
            ''' No endpoint was found.
            '''</summary>
            RPC_S_NO_ENDPOINT_FOUND = 1708L

            '''<summary>
            ''' The timeout value is invalid.
            '''</summary>
            RPC_S_INVALID_TIMEOUT = 1709L

            '''<summary>
            ''' The object UUID was not found.
            '''</summary>
            RPC_S_OBJECT_NOT_FOUND = 1710L

            '''<summary>
            ''' The object UUID already
            ''' registered.
            '''</summary>
            RPC_S_ALREADY_REGISTERED = 1711L

            '''<summary>
            ''' The type UUID is already
            ''' registered.
            '''</summary>
            RPC_S_TYPE_ALREADY_REGISTERED = 1712L

            '''<summary>
            ''' The server is already
            ''' listening.
            '''</summary>
            RPC_S_ALREADY_LISTENING = 1713L

            '''<summary>
            ''' No protocol sequences were
            ''' registered.
            '''</summary>
            RPC_S_NO_PROTSEQS_REGISTERED = 1714L

            '''<summary>
            ''' The server is not listening.
            '''</summary>
            RPC_S_NOT_LISTENING = 1715L

            '''<summary>
            ''' The manager type is unknown.
            '''</summary>
            RPC_S_UNKNOWN_MGR_TYPE = 1716L

            '''<summary>
            ''' The interface is unknown.
            '''</summary>
            RPC_S_UNKNOWN_IF = 1717L

            '''<summary>
            ''' There are no bindings.
            '''</summary>
            RPC_S_NO_BINDINGS = 1718L

            '''<summary>
            ''' There are no protocol
            ''' sequences.
            '''</summary>
            RPC_S_NO_PROTSEQS = 1719L

            '''<summary>
            ''' The endpoint cannot be
            ''' created.
            '''</summary>
            RPC_S_CANT_CREATE_ENDPOINT = 1720L

            '''<summary>
            ''' Not enough resources are
            ''' available to complete this
            ''' operation.
            '''</summary>
            RPC_S_OUT_OF_RESOURCES = 1721L

            '''<summary>
            ''' The server is unavailable.
            '''</summary>
            RPC_S_SERVER_UNAVAILABLE = 1722L

            '''<summary>
            ''' The server is too busy to
            ''' complete this operation.
            '''</summary>
            RPC_S_SERVER_TOO_BUSY = 1723L

            '''<summary>
            ''' The network options are
            ''' invalid.
            '''</summary>
            RPC_S_INVALID_NETWORK_OPTIONS = 1724L

            '''<summary>
            ''' There is not a remote
            ''' procedure call active in this
            ''' thread.
            '''</summary>
            RPC_S_NO_CALL_ACTIVE = 1725L

            '''<summary>
            ''' The remote procedure call
            ''' failed.
            '''</summary>
            RPC_S_CALL_FAILED = 1726L

            '''<summary>
            ''' The remote procedure call
            ''' failed and did not execute.
            '''</summary>
            RPC_S_CALL_FAILED_DNE = 1727L

            '''<summary>
            ''' An RPC protocol error
            ''' occurred.
            '''</summary>
            RPC_S_PROTOCOL_ERROR = 1728L

            '''<summary>
            ''' The transfer syntax is not
            ''' supported by the server.
            '''</summary>
            RPC_S_UNSUPPORTED_TRANS_SYN = 1730L

            '''<summary>
            ''' The server has insufficient
            ''' memory to complete this
            ''' operation.
            '''</summary>
            RPC_S_SERVER_OUT_OF_MEMORY = 1731L

            '''<summary>
            ''' The type UUID is not
            ''' supported.
            '''</summary>
            RPC_S_UNSUPPORTED_TYPE = 1732L

            '''<summary>
            ''' The tag is invalid.
            '''</summary>
            RPC_S_INVALID_TAG = 1733L

            '''<summary>
            ''' The array bounds are invalid.
            '''</summary>
            RPC_S_INVALID_BOUND = 1734L

            '''<summary>
            ''' The binding does not contain
            ''' an entry name.
            '''</summary>
            RPC_S_NO_ENTRY_NAME = 1735L

            '''<summary>
            ''' The name syntax is invalid.
            '''</summary>
            RPC_S_INVALID_NAME_SYNTAX = 1736L

            '''<summary>
            ''' The name syntax is not
            ''' supported.
            '''</summary>
            RPC_S_UNSUPPORTED_NAME_SYNTAX = 1737L

            '''<summary>
            ''' No network address is
            ''' available to use to construct
            ''' a UUID.
            '''</summary>
            RPC_S_UUID_NO_ADDRESS = 1739L

            '''<summary>
            ''' The endpoint is a duplicate.
            '''</summary>
            RPC_S_DUPLICATE_ENDPOINT = 1740L

            '''<summary>
            ''' The authentication type is
            ''' unknown.
            '''</summary>
            RPC_S_UNKNOWN_AUTHN_TYPE = 1741L

            '''<summary>
            ''' The maximum number of calls is
            ''' too small.
            '''</summary>
            RPC_S_MAX_CALLS_TOO_SMALL = 1742L

            '''<summary>
            ''' The string is too long.
            '''</summary>
            RPC_S_STRING_TOO_LONG = 1743L

            '''<summary>
            ''' The RPC protocol sequence was
            ''' not found.
            '''</summary>
            RPC_S_PROTSEQ_NOT_FOUND = 1744L

            '''<summary>
            ''' The procedure number is out of
            ''' range.
            '''</summary>
            RPC_S_PROCNUM_OUT_OF_RANGE = 1745L

            '''<summary>
            ''' The binding does not contain
            ''' any authentication
            ''' information.
            '''</summary>
            RPC_S_BINDING_HAS_NO_AUTH = 1746L

            '''<summary>
            ''' The authentication service is
            ''' unknown.
            '''</summary>
            RPC_S_UNKNOWN_AUTHN_SERVICE = 1747L

            '''<summary>
            ''' The authentication level is
            ''' unknown.
            '''</summary>
            RPC_S_UNKNOWN_AUTHN_LEVEL = 1748L

            '''<summary>
            ''' The security context is
            ''' invalid.
            '''</summary>
            RPC_S_INVALID_AUTH_IDENTITY = 1749L

            '''<summary>
            ''' The authorization service is
            ''' unknown.
            '''</summary>
            RPC_S_UNKNOWN_AUTHZ_SERVICE = 1750L

            '''<summary>
            ''' The entry is invalid.
            '''</summary>
            EPT_S_INVALID_ENTRY = 1751L

            '''<summary>
            ''' The operation cannot be
            ''' performed.
            '''</summary>
            EPT_S_CANT_PERFORM_OP = 1752L

            '''<summary>
            ''' There are no more endpoints
            ''' available from the endpoint
            ''' mapper.
            '''</summary>
            EPT_S_NOT_REGISTERED = 1753L

            '''<summary>
            ''' The entry name is incomplete.
            '''</summary>
            RPC_S_INCOMPLETE_NAME = 1755L

            '''<summary>
            ''' The version option is invalid.
            '''</summary>
            RPC_S_INVALID_VERS_OPTION = 1756L

            '''<summary>
            ''' There are no more members.
            '''</summary>
            RPC_S_NO_MORE_MEMBERS = 1757L

            '''<summary>
            ''' There is nothing to unexport.
            '''</summary>
            RPC_S_NOT_ALL_OBJS_UNEXPORTED = 1758L

            '''<summary>
            ''' The interface was not found.
            '''</summary>
            RPC_S_INTERFACE_NOT_FOUND = 1759L

            '''<summary>
            ''' The entry already exists.
            '''</summary>
            RPC_S_ENTRY_ALREADY_EXISTS = 1760L

            '''<summary>
            ''' The entry is not found.
            '''</summary>
            RPC_S_ENTRY_NOT_FOUND = 1761L

            '''<summary>
            ''' The name service is
            ''' unavailable.
            '''</summary>
            RPC_S_NAME_SERVICE_UNAVAILABLE = 1762L

            '''<summary>
            ''' The requested operation is not
            ''' supported.
            '''</summary>
            RPC_S_CANNOT_SUPPORT = 1764L

            '''<summary>
            ''' No security context is
            ''' available to allow
            ''' impersonation.
            '''</summary>
            RPC_S_NO_CONTEXT_AVAILABLE = 1765L

            '''<summary>
            ''' An internal error occurred in
            ''' RPC.
            '''</summary>
            RPC_S_INTERNAL_ERROR = 1766L

            '''<summary>
            ''' The server attempted an
            ''' integer divide by zero.
            '''</summary>
            RPC_S_ZERO_DIVIDE = 1767L

            '''<summary>
            ''' An addressing error occurred
            ''' in the server.
            '''</summary>
            RPC_S_ADDRESS_ERROR = 1768L

            '''<summary>
            ''' A floating point operation at
            ''' the server caused a divide by
            ''' zero.
            '''</summary>
            RPC_S_FP_DIV_ZERO = 1769L

            '''<summary>
            ''' A floating point underflow
            ''' occurred at the server.
            '''</summary>
            RPC_S_FP_UNDERFLOW = 1770L

            '''<summary>
            ''' A floating point overflow
            ''' occurred at the server.
            '''</summary>
            RPC_S_FP_OVERFLOW = 1771L

            '''<summary>
            ''' The list of servers available
            ''' for auto_handle binding was
            ''' exhausted.
            '''</summary>
            RPC_X_NO_MORE_ENTRIES = 1772L

            '''<summary>
            ''' The file designated by
            ''' DCERPCCHARTRANS cannot be
            ''' opened.
            '''</summary>
            RPC_X_SS_CHAR_TRANS_OPEN_FAIL = 1773L

            '''<summary>
            ''' The file containing the
            ''' character translation table
            ''' has fewer than 512 bytes.
            '''</summary>
            RPC_X_SS_CHAR_TRANS_SHORT_FILE = 1774L

            '''<summary>
            ''' A null context handle is
            ''' passed as an [in] parameter.
            '''</summary>
            RPC_X_SS_IN_NULL_CONTEXT = 1775L

            '''<summary>
            ''' The context handle does not
            ''' match any known context
            ''' handles.
            '''</summary>
            RPC_X_SS_CONTEXT_MISMATCH = 1776L

            '''<summary>
            ''' The context handle changed
            ''' during a call.
            '''</summary>
            RPC_X_SS_CONTEXT_DAMAGED = 1777L

            '''<summary>
            ''' The binding handles passed to
            ''' a remote procedure call do not
            ''' match.
            '''</summary>
            RPC_X_SS_HANDLES_MISMATCH = 1778L

            '''<summary>
            ''' The stub is unable to get the
            ''' call handle.
            '''</summary>
            RPC_X_SS_CANNOT_GET_CALL_HANDLE = 1779L

            '''<summary>
            ''' A null reference pointer was
            ''' passed to the stub.
            '''</summary>
            RPC_X_NULL_REF_POINTER = 1780L

            '''<summary>
            ''' The enumeration value is out
            ''' of range.
            '''</summary>
            RPC_X_ENUM_VALUE_OUT_OF_RANGE = 1781L

            '''<summary>
            ''' The byte count is too small.
            '''</summary>
            RPC_X_BYTE_COUNT_TOO_SMALL = 1782L

            '''<summary>
            ''' The stub received bad data.
            '''</summary>
            RPC_X_BAD_STUB_DATA = 1783L

            '''<summary>
            ''' The supplied user buffer is
            ''' invalid for the requested
            ''' operation.
            '''</summary>
            ERROR_INVALID_USER_BUFFER = 1784L

            '''<summary>
            ''' The disk media is not
            ''' recognized. It may not be
            ''' formatted.
            '''</summary>
            ERROR_UNRECOGNIZED_MEDIA = 1785L

            '''<summary>
            ''' The workstation does not have
            ''' a trust secret.
            '''</summary>
            ERROR_NO_TRUST_LSA_SECRET = 1786L

            '''<summary>
            ''' The domain controller does not
            ''' have an account for this
            ''' workstation.
            '''</summary>
            ERROR_NO_TRUST_SAM_ACCOUNT = 1787L

            '''<summary>
            ''' The trust relationship between
            ''' the primary domain and the
            ''' trusted domain failed.
            '''</summary>
            ERROR_TRUSTED_DOMAIN_FAILURE = 1788L

            '''<summary>
            ''' The trust relationship between
            ''' this workstation and the
            ''' primary domain failed.
            '''</summary>
            ERROR_TRUSTED_RELATIONSHIP_FAILURE = 1789L

            '''<summary>
            ''' The network logon failed.
            '''</summary>
            ERROR_TRUST_FAILURE = 1790L

            '''<summary>
            ''' A remote procedure call is
            ''' already in progress for this
            ''' thread.
            '''</summary>
            RPC_S_CALL_IN_PROGRESS = 1791L

            '''<summary>
            ''' An attempt was made to logon,
            ''' but the network logon service
            ''' was not started.
            '''</summary>
            ERROR_NETLOGON_NOT_STARTED = 1792L

            '''<summary>
            ''' The user's account has
            ''' expired.
            '''</summary>
            ERROR_ACCOUNT_EXPIRED = 1793L

            '''<summary>
            ''' The redirector is in use and
            ''' cannot be unloaded.
            '''</summary>
            ERROR_REDIRECTOR_HAS_OPEN_HANDLES = 1794L

            '''<summary>
            ''' The specified printer driver
            ''' is already installed.
            '''</summary>
            ERROR_PRINTER_DRIVER_ALREADY_INSTALLED = 1795L

            '''<summary>
            ''' The specified port is unknown.
            '''</summary>
            ERROR_UNKNOWN_PORT = 1796L

            '''<summary>
            ''' The printer driver is unknown.
            '''</summary>
            ERROR_UNKNOWN_PRINTER_DRIVER = 1797L

            '''<summary>
            ''' The print processor is
            ''' unknown.
            '''</summary>
            ERROR_UNKNOWN_PRINTPROCESSOR = 1798L

            '''<summary>
            ''' The specified separator file
            ''' is invalid.
            '''</summary>
            ERROR_INVALID_SEPARATOR_FILE = 1799L

            '''<summary>
            ''' The specified priority is
            ''' invalid.
            '''</summary>
            ERROR_INVALID_PRIORITY = 1800L

            '''<summary>
            ''' The printer name is invalid.
            '''</summary>
            ERROR_INVALID_PRINTER_NAME = 1801L

            '''<summary>
            ''' The printer already exists.
            '''</summary>
            ERROR_PRINTER_ALREADY_EXISTS = 1802L

            '''<summary>
            ''' The printer command is
            ''' invalid.
            '''</summary>
            ERROR_INVALID_PRINTER_COMMAND = 1803L

            '''<summary>
            ''' The specified datatype is
            ''' invalid.
            '''</summary>
            ERROR_INVALID_DATATYPE = 1804L

            '''<summary>
            ''' The Environment specified is
            ''' invalid.
            '''</summary>
            ERROR_INVALID_ENVIRONMENT = 1805L

            '''<summary>
            ''' There are no more bindings.
            '''</summary>
            RPC_S_NO_MORE_BINDINGS = 1806L

            '''<summary>
            ''' The account used is an
            ''' interdomain trust account. Use
            ''' your normal user account or
            ''' remote user account to access
            ''' this server.
            '''</summary>
            ERROR_NOLOGON_INTERDOMAIN_TRUST_ACCOUNT = 1807L

            '''<summary>
            ''' The account used is a
            ''' workstation trust account. Use
            ''' your normal user account or
            ''' remote user account to access
            ''' this server.
            '''</summary>
            ERROR_NOLOGON_WORKSTATION_TRUST_ACCOUNT = 1808L

            '''<summary>
            ''' The account used is an server
            ''' trust account. Use your normal
            ''' user account or remote user
            ''' account to access this server.
            '''</summary>
            ERROR_NOLOGON_SERVER_TRUST_ACCOUNT = 1809L

            '''<summary>
            ''' The name or security ID (SID)
            ''' of the domain specified is
            ''' inconsistent with the trust
            ''' information for that domain.
            '''</summary>
            ERROR_DOMAIN_TRUST_INCONSISTENT = 1810L

            '''<summary>
            ''' The server is in use and
            ''' cannot be unloaded.
            '''</summary>
            ERROR_SERVER_HAS_OPEN_HANDLES = 1811L

            '''<summary>
            ''' The specified image file did
            ''' not contain a resource
            ''' section.
            '''</summary>
            ERROR_RESOURCE_DATA_NOT_FOUND = 1812L

            '''<summary>
            ''' The specified resource type
            ''' can not be found in the image
            ''' file.
            '''</summary>
            ERROR_RESOURCE_TYPE_NOT_FOUND = 1813L

            '''<summary>
            ''' The specified resource name
            ''' can not be found in the image
            ''' file.
            '''</summary>
            ERROR_RESOURCE_NAME_NOT_FOUND = 1814L

            '''<summary>
            ''' The specified resource
            ''' language ID cannot be found in
            ''' the image file.
            '''</summary>
            ERROR_RESOURCE_LANG_NOT_FOUND = 1815L

            '''<summary>
            ''' Not enough quota is available
            ''' to process this command.
            '''</summary>
            ERROR_NOT_ENOUGH_QUOTA = 1816L

            '''<summary>
            ''' 
            '''</summary>
            RPC_S_NO_INTERFACES = 1817L

            '''<summary>
            ''' The server was altered while
            ''' processing this call.
            '''</summary>
            RPC_S_CALL_CANCELLED = 1818L

            '''<summary>
            ''' The binding handle does not
            ''' contain all required
            ''' information.
            '''</summary>
            RPC_S_BINDING_INCOMPLETE = 1819L

            '''<summary>
            ''' Communications failure.
            '''</summary>
            RPC_S_COMM_FAILURE = 1820L

            '''<summary>
            ''' The requested authentication
            ''' level is not supported.
            '''</summary>
            RPC_S_UNSUPPORTED_AUTHN_LEVEL = 1821L

            '''<summary>
            ''' No principal name registered.
            '''</summary>
            RPC_S_NO_PRINC_NAME = 1822L

            '''<summary>
            ''' The error specified is not a
            ''' valid Windows RPC error code.
            '''</summary>
            RPC_S_NOT_RPC_ERROR = 1823L

            '''<summary>
            ''' A UUID that is valid only on
            ''' this computer has been
            ''' allocated.
            '''</summary>
            RPC_S_UUID_LOCAL_ONLY = 1824L

            '''<summary>
            ''' A security package specific
            ''' error occurred.
            '''</summary>
            RPC_S_SEC_PKG_ERROR = 1825L

            '''<summary>
            ''' Thread is not cancelled.
            '''</summary>
            RPC_S_NOT_CANCELLED = 1826L

            '''<summary>
            ''' Invalid operation on the
            ''' encoding/decoding handle.
            '''</summary>
            RPC_X_INVALID_ES_ACTION = 1827L

            '''<summary>
            ''' Incompatible version of the
            ''' serializing package.
            '''</summary>
            RPC_X_WRONG_ES_VERSION = 1828L

            '''<summary>
            ''' Incompatible version of the
            ''' RPC stub.
            '''</summary>
            RPC_X_WRONG_STUB_VERSION = 1829L

            '''<summary>
            ''' The idl pipe object is invalid
            ''' or corrupted.
            '''</summary>
            RPC_X_INVALID_PIPE_OBJECT = 1830L

            '''<summary>
            ''' The operation is invalid for a
            ''' given idl pipe object.
            '''</summary>
            RPC_X_INVALID_PIPE_OPERATION = 1831L

            '''<summary>
            ''' The idl pipe version is not
            ''' supported.
            '''</summary>
            RPC_X_WRONG_PIPE_VERSION = 1832L

            '''<summary>
            ''' The group member was not
            ''' found.
            '''</summary>
            RPC_S_GROUP_MEMBER_NOT_FOUND = 1898L

            '''<summary>
            ''' The endpoint mapper database
            ''' could not be created.
            '''</summary>
            EPT_S_CANT_CREATE = 1899L

            '''<summary>
            ''' The object universal unique
            ''' identifier (UUID) is the nil
            ''' UUID.
            '''</summary>
            RPC_S_INVALID_OBJECT = 1900L

            '''<summary>
            ''' The specified time is invalid.
            '''</summary>
            ERROR_INVALID_TIME = 1901L

            '''<summary>
            ''' The specified Form name is
            ''' invalid.
            '''</summary>
            ERROR_INVALID_FORM_NAME = 1902L

            '''<summary>
            ''' The specified Form size is
            ''' invalid.
            '''</summary>
            ERROR_INVALID_FORM_SIZE = 1903L

            '''<summary>
            ''' The specified Printer handle
            ''' is already being waited on.
            '''</summary>
            ERROR_ALREADY_WAITING = 1904L

            '''<summary>
            ''' The specified Printer has been
            ''' deleted.
            '''</summary>
            ERROR_PRINTER_DELETED = 1905L

            '''<summary>
            ''' The state of the Printer is
            ''' invalid.
            '''</summary>
            ERROR_INVALID_PRINTER_STATE = 1906L

            '''<summary>
            ''' The user must change his
            ''' password before he logs on the
            ''' first time.
            '''</summary>
            ERROR_PASSWORD_MUST_CHANGE = 1907L

            '''<summary>
            ''' Could not find the domain
            ''' controller for this domain.
            '''</summary>
            ERROR_DOMAIN_CONTROLLER_NOT_FOUND = 1908L

            '''<summary>
            ''' The referenced account is
            ''' currently locked out and may
            ''' not be logged on to.
            '''</summary>
            ERROR_ACCOUNT_LOCKED_OUT = 1909L

            '''<summary>
            ''' The object exporter specified
            ''' was not found.
            '''</summary>
            OR_INVALID_OXID = 1910L

            '''<summary>
            ''' The object specified was not
            ''' found.
            '''</summary>
            OR_INVALID_OID = 1911L

            '''<summary>
            ''' The object resolver set
            ''' specified was not found.
            '''</summary>
            OR_INVALID_SET = 1912L

            '''<summary>
            ''' Some data remains to be sent
            ''' in the request buffer.
            '''</summary>
            RPC_S_SEND_INCOMPLETE = 1913L

            '''<summary>
            ''' The pixel format is invalid.
            '''</summary>
            ERROR_INVALID_PIXEL_FORMAT = 2000L

            '''<summary>
            ''' The specified driver is
            ''' invalid.
            '''</summary>
            ERROR_BAD_DRIVER = 2001L

            '''<summary>
            ''' The window style or class
            ''' attribute is invalid for this
            ''' operation.
            '''</summary>
            ERROR_INVALID_WINDOW_STYLE = 2002L

            '''<summary>
            ''' The requested metafile
            ''' operation is not supported.
            '''</summary>
            ERROR_METAFILE_NOT_SUPPORTED = 2003L

            '''<summary>
            ''' The requested transformation
            ''' operation is not supported.
            '''</summary>
            ERROR_TRANSFORM_NOT_SUPPORTED = 2004L

            '''<summary>
            ''' The requested clipping
            ''' operation is not supported.
            '''</summary>
            ERROR_CLIPPING_NOT_SUPPORTED = 2005L

            '''<summary>
            ''' The network is not present or
            ''' not started.
            '''</summary>
            ERROR_NO_NETWORK2 = 2138L

            '''<summary>
            ''' The specified user name is
            ''' invalid.
            '''</summary>
            ERROR_BAD_USERNAME = 2202L

            '''<summary>
            ''' This network connection does
            ''' not exist.
            '''</summary>
            ERROR_NOT_CONNECTED = 2250L

            '''<summary>
            ''' There are open files or
            ''' requests pending on this
            ''' connection.
            '''</summary>
            ERROR_OPEN_FILES = 2401L

            '''<summary>
            ''' Active connections still
            ''' exist.
            '''</summary>
            ERROR_ACTIVE_CONNECTIONS = 2402L

            '''<summary>
            ''' The device is in use by an
            ''' active process and cannot be
            ''' disconnected.
            '''</summary>
            ERROR_DEVICE_IN_USE = 2404L

            '''<summary>
            ''' The specified print monitor is
            ''' unknown.
            '''</summary>
            ERROR_UNKNOWN_PRINT_MONITOR = 3000L

            '''<summary>
            ''' The specified printer driver
            ''' is currently in use.
            '''</summary>
            ERROR_PRINTER_DRIVER_IN_USE = 3001L

            '''<summary>
            ''' The spool file was not found.
            '''</summary>
            ERROR_SPOOL_FILE_NOT_FOUND = 3002L

            '''<summary>
            ''' A StartDocPrinter call was not
            ''' issued.
            '''</summary>
            ERROR_SPL_NO_STARTDOC = 3003L

            '''<summary>
            ''' An AddJob call was not issued.
            '''</summary>
            ERROR_SPL_NO_ADDJOB = 3004L

            '''<summary>
            ''' The specified print
            ''' processor has already been
            ''' installed.
            '''</summary>
            ERROR_PRINT_PROCESSOR_ALREADY_INSTALLED = 3005L

            '''<summary>
            ''' The specified print monitor
            ''' has already been installed.
            '''</summary>
            ERROR_PRINT_MONITOR_ALREADY_INSTALLED = 3006L

            '''<summary>
            ''' The specified print monitor
            ''' does not have the required
            ''' functions.
            '''</summary>
            ERROR_INVALID_PRINT_MONITOR = 3007L

            '''<summary>
            ''' The specified print monitor is
            ''' currently in use.
            '''</summary>
            ERROR_PRINT_MONITOR_IN_USE = 3008L

            '''<summary>
            ''' The requested operation is not
            ''' allowed when there are jobs
            ''' queued to the printer.
            '''</summary>
            ERROR_PRINTER_HAS_JOBS_QUEUED = 3009L

            '''<summary>
            ''' The requested operation is
            ''' successful. Changes will not
            ''' be effective until the system
            ''' is rebooted.
            '''</summary>
            ERROR_SUCCESS_REBOOT_REQUIRED = 3010L

            '''<summary>
            ''' The requested operation is
            ''' successful. Changes will not
            ''' be effective until the service
            ''' is restarted.
            '''</summary>
            ERROR_SUCCESS_RESTART_REQUIRED = 3011L

            '''<summary>
            ''' WINS encountered an error
            ''' while processing the command.
            '''</summary>
            ERROR_WINS_INTERNAL = 4000L

            '''<summary>
            ''' The local WINS can not be
            ''' deleted.
            '''</summary>
            ERROR_CAN_NOT_DEL_LOCAL_WINS = 4001L

            '''<summary>
            ''' The importation from the file
            ''' failed.
            '''</summary>
            ERROR_STATIC_INIT = 4002L

            '''<summary>
            ''' The backup failed. Was a full
            ''' backup done before?
            '''</summary>
            ERROR_INC_BACKUP = 4003L

            '''<summary>
            ''' The backup failed. Check the
            ''' directory that you are backing
            ''' the database to.
            '''</summary>
            ERROR_FULL_BACKUP = 4004L

            '''<summary>
            ''' The name does not exist in the
            ''' WINS database.
            '''</summary>
            ERROR_REC_NON_EXISTENT = 4005L

            '''<summary>
            ''' Replication with a non-
            ''' configured partner is not
            ''' allowed.
            '''</summary>
            ERROR_RPL_NOT_ALLOWED = 4006L

            '''<summary>
            ''' The list of servers for this
            ''' workgroup is not currently
            ''' available.
            '''</summary>
            ERROR_NO_BROWSER_SERVERS_FOUND = 6118L
        End Enum

        '(1)-功能错误。
        '(2)- 系统找不到指定的文件。
        '(3)-系统找不到指定的路径。
        '(4)-系统无法打开文件。
        '(5)-拒绝访问。
        '(6)-句柄无 效。
        '(7)-存储控制块被损坏。
        '(8)-存储空间不足，无法处理此命令。
        '(9)-存储控制块地址无效。
        '(10)-环境错 误。
        '(11)-试图加载格式错误的程序。
        '(12)-访问码无效。
        '(13)-数据无效。
        '(14)-存储器不足，无法完成此 操作。
        '(15)-系统找不到指定的驱动器。
        '(16)-无法删除目录。
        '(17)-系统无法将文件移到不同的驱动器。
        '(18)- 没有更多文件。
        '(19)-介质受写入保护。
        '(20)-系统找不到指定的设备。
        '(21)-设备未就绪。
        '(22)-设备不识 别此命令。
        '(23)-数据错误 (循环冗余检查)。
        '(24)-程序发出命令，但命令长度不正确。
        '(25)-驱动器无法找出磁盘上 特定区域或磁道的位置。
        '(26)-无法访问指定的磁盘或软盘。
        '(27)-驱动器找不到请求的扇区。
        '(28)-打印机缺纸。
        '(29)- 系统无法写入指定的设备。
        '(30)-系统无法从指定的设备上读取。
        '(31)-连到系统上的设备没有发挥作用。
        '(32)-进程无法 访问文件，因为另一个程序正在使用此文件。
        '(33)-进程无法访问文件，因为另一个程序已锁定文件的一部分。
        '(36)-用来共享的打开文 件过多。
        '(38)-到达文件结尾。
        '(39)-磁盘已满。
        '(50)-不支持网络请求。
        '(51)-远程计算机不可用 。
        '(52)- 在网络上已有重复的名称。
        '(53)-找不到网络路径。
        '(54)-网络忙。
        '(55)-指定的网络资源或设备不再可用。
        '(56)- 已到达网络 BIOS 命令限制。
        '(57)-网络适配器硬件出错。
        '(58)-指定的服务器无法运行请求的操作。
        '(59)-发生意 外的网络错误。
        '(60)-远程适配器不兼容。
        '(61)-打印机队列已满。
        '(62)-无法在服务器上获得用于保存待打印文件的空 间。
        '(63)-删除等候打印的文件。
        '(64)-指定的网络名不再可用。
        '(65)-拒绝网络访问。
        '(66)-网络资源类型 错误。
        '(67)-找不到网络名。
        '(68)-超过本地计算机网卡的名称限制。
        '(69)-超出网络 BIOS 会话限制。
        '(70)- 远程服务器已暂停，或正在启动过程中。
        '(71)-当前已无法再同此远程计算机连接，因为已达到计算机的连接数目极限。
        '(72)-已暂停指 定的打印机或磁盘设备。
        '(80)-文件存在。
        '(82)-无法创建目录或文件。
        '(83)-INT 24 失败。
        '(84)- 无法取得处理此请求的存储空间。
        '(85)-本地设备名已在使用中。
        '(86)-指定的网络密码错误。
        '(87)-参数错误。
        '(88)- 网络上发生写入错误。
        '(89)-系统无法在此时启动另一个进程。
        '(100)-无法创建另一个系统信号灯。
        '(101)-另一个进程 拥有独占的信号灯。
        '(102)-已设置信号灯且无法关闭。
        '(103)-无法再设置信号灯。
        '(104)-无法在中断时请求独占的信 号灯。
        '(105)-此信号灯的前一个所有权已结束。
        '(107)-程序停止，因为替代的软盘未插入。
        '(108)-磁盘在使用中，或 被另一个进程锁定。
        '(109)-管道已结束。
        '(110)-系统无法打开指定的设备或文件。
        '(111)-文件名太长。
        '(112)- 磁盘空间不足。
        '(113)-无法再获得内部文件的标识。
        '(114)-目标内部文件的标识不正确。 (117)-应用程序制作的 IOCTL 调用错误。
        '(118)-验证写入的切换参数值错误。
        '(119)-系统不支持请求的命令。
        '(120)-此功能只被此系 统支持。
        '(121)-信号灯超时时间已到。
        '(122)-传递到系统调用的数据区太小。
        '(123)-文件名、目录名或卷标语法不正 确。
        '(124)-系统调用级别错误。
        '(125)-磁盘没有卷标。
        '(126)-找不到指定的模块。
        '(127)-找不到指定 的程序。
        '(128)-没有等候的子进程。
        '(130)-试图使用操作(而非原始磁盘 I/O)的已打开磁盘分区的文件句柄。
        '(131)- 试图移动文件指针到文件开头之前。
        '(132)-无法在指定的设备或文件上设置文件指针。
        '(133)-包含先前加入驱动器的驱动器无法使用 JOIN 或 SUBST 命令。
        '(134)-试图在已被合并的驱动器上使用 JOIN 或 SUBST 命令。
        '(135)-试图在已 被合并的驱动器上使用 JOIN 或 SUBST 命令。
        '(136)-系统试图解除未合并驱动器的 JOIN。
        '(137)-系统试图解除 未替代驱动器的 SUBST。
        '(138)-系统试图将驱动器合并到合并驱动器上的目录。
        '(139)-系统试图将驱动器替代为替代驱动器上 的目录。
        '(140)-系统试图将驱动器合并到替代驱动器上的目录。
        '(141)-系统试图替代驱动器为合并驱动器上的目录。
        '(142)- 系统无法在此时运行 JOIN 或 SUBST。
        '(143)-系统无法将驱动器合并到或替代为相同驱动器上的目录。
        '(144)-目录并非 根目录下的子目录。
        '(145)-目录非空。
        '(146)-指定的路径已在替代中使用。
        '(147)-资源不足，无法处理此命令。
        '(148)- 指定的路径无法在此时使用。
        '(149)-企图将驱动器合并或替代为驱动器上目录是上一个替代的目标的驱动器。
        '(150)-系统跟踪信息未 在 CONFIG.SYS 文件中指定，或不允许跟踪。
        '(151)-为 DosMuxSemWait 指定的信号灯事件个数错误。
        '(152)-DosMuxSemWait 不可运行。已设置过多的信号灯。
        '(153)-DosMuxSemWait 清单错误。
        '(154)-输入的卷标超过目标文件系统的长度限 制
        '(155)-无法创建另一个线程。
        '(156)-接收进程已拒绝此信号。
        '(157)-段已被放弃且无法锁定。
        '(158)- 段已解除锁定。
        '(159)-线程标识的地址错误。
        '(160)-传递到 DosExecPgm 的参数字符串错误。
        '(161)-指 定的路径无效。
        '(162)-信号已暂停。
        '(164)-无法在系统中创建更多的线程。
        '(167)-无法锁定文件区域。
        '(170)- 请求的资源在使用中。
        '(173)-对于提供取消区域进行锁定的请求不明显。
        '(174)-文件系统不支持锁定类型的最小单元更改。
        '(180)- 系统检测出错误的段号。
        '(183)-当文件已存在时，无法创建该文件。
        '(186)-传递的标志错误。
        '(187)-找不到指定的系 统信号灯名称。
        '(196)-操作系统无法运行此应用程序。
        '(197)-操作系统当前的配置不能运行此应用程序。
        '(199)-操作 系统无法运行此应用程序。
        '(200)-代码段不可大于或等于 64K。
        '(203)-操作系统找不到已输入的环境选项。
        '(205)- 命令子树中的进程没有信号处理程序。
        '(206)-文件名或扩展名太长。
        '(207)-第 2 环堆栈已被占用。
        '(208)-没有正 确输入文件名通配符 * 或 ?，或指定过多的文件名通配符。
        '(209)-正在发送的信号错误。(210)-无法设置信号处理程序。
        '(212)- 段已锁定且无法重新分配。
        '(214)-连到该程序或动态链接模块的动态链接模块太多。
        '(215)-无法嵌套调用 LoadModule。
        '(230)- 管道状态无效。
        '(231)-所有的管道实例都在使用中。
        '(232)-管道正在关闭中。
        '(233)-管道的另一端上无任何进程。
        '(234)- 更多数据可用。
        '(240)-取消会话。
        '(254)-指定的扩展属性名无效。
        '(255)-扩展属性不一致。
        '(258)-等 待的操作过时。
        '(259)-没有可用的数据了。
        '(266)-无法使用复制功能。
        '(267)-目录名无效。
        '(275)-扩 展属性在缓冲区中不适用。
        '(276)-装在文件系统上的扩展属性文件已损坏。
        '(277)-扩展属性表格文件已满。
        '(278)-指 定的扩展属性句柄无效。
        '(282)-装入的文件系统不支持扩展属性。
        '(288)-企图释放并非呼叫方所拥有的多用户终端运行程序。
        '(298)- 发向信号灯的请求过多。
        '(299)-仅完成部分的 ReadProcessMemoty 或 WriteProcessMemory 请求。
        '(300)- 操作锁定请求被拒绝。
        '(301)-系统接收了一个无效的操作锁定确认。
        '(487)-试图访问无效的地址。
        '(534)-算术结果超 过 32 位。
        '(535)-管道的另一端有一进程。
        '(536)-等候打开管道另一端的进程。
        '(994)-拒绝访问扩展属性。
        '(995)- 由于线程退出或应用程序请求，已放弃 I/O 操作。
        '(996)-重叠 I/O 事件不在信号状态中。
        '(997)-重叠 I/O 操作在进行中。
        '(998)-内存分配访问无效。
        '(999)-错误运行页内操作。
        '(1001)-递归太深；栈溢出。
        '(1002)- 窗口无法在已发送的消息上操作。
        '(1003)-无法完成此功能。
        '(1004)-无效标志。
        '(1005)-此卷不包含可识别的文件 系统。请确定所有请求的文件系统驱动程序已加载，且此卷未损坏。
        '(1006)-文件所在的卷已被外部改变，因此打开的文件不再有效。
        '(1007)- 无法在全屏幕模式下运行请求的操作。
        '(1008)-试图引用不存在的令牌。
        '(1009)-配置注册表数据库损坏。
        '(1010)- 配置注册表项无效。
        '(1011)-无法打开配置注册表项。
        '(1012)-无法读取配置注册表项。
        '(1013)-无法写入配置注册 表项。
        '(1014)-注册表数据库中的某一文件必须使用记录或替代复制来恢复。恢复成功完成。
        '(1015)-注册表损坏。包含注册表数据 的某一文件结构损坏，或系统的文件内存映像损坏，或因为替代副本、日志缺少或损坏而无法恢复文件。
        '(1016)-由注册表启动的 I/O 操作恢复失败。注册表无法读入、写出或清除任意一个包含注册表系统映像的文件。
        '(1017)-系统试图加载或还原文件到注册表，但指定的文件并非 注册表文件格式。
        '(1018)-试图在标记为删除的注册表项上运行不合法的操作。
        '(1019)-系统无法配置注册表日志中所请求的空间。
        '(1020)- 无法在已有子项或值的注册表项中创建符号链接。
        '(1021)-无法在易变父项下创建稳定子项。
        '(1022)-通知更改请求正在完成中，且 信息并未返回到呼叫方的缓冲区中。当前呼叫方必须枚举文件来查找更改。
        '(1051)-已发送停止控制到服务，该服务被其它正在运行的服务所依赖。
        '(1052)- 请求的控件对此服务无效
        '(1053)-服务并未及时响应启动或控制请求。
        '(1054)-无法创建此服务的线程。
        '(1055)-锁 定服务数据库。
        '(1056)-服务的实例已在运行中。
        '(1057)-帐户名无效或不存在，或者密码对于指定的帐户名无效。
        '(1058)- 无法启动服务，原因可能是它被禁用或与它相关联的设备没有启动。
        '(1059)-指定了循环服务依存。
        '(1060)-指定的服务并未以已安 装的服务存在。
        '(1061)-服务无法在此时接受控制信息。
        '(1062)-服务未启动。
        '(1063)-服务进程无法连接到服务控 制器上。
        '(1064)-当处理控制请求时，在服务中发生异常。
        '(1065)-指定的数据库不存在。
        '(1066)-服务已返回特定 的服务错误码。
        '(1067)-进程意外终止。
        '(1068)-依存服务或组无法启动。
        '(1069)-由于登录失败而无法启动服务。
        '(1070)- 启动后，服务停留在启动暂停状态。
        '(1071)-指定的服务数据库锁定无效。
        '(1072)-指定的服务已标记为删除。
        '(1073)- 指定的服务已存在。
        '(1074)-系统当前以最新的有效配置运行。
        '(1075)-依存服务不存在，或已被标记为删除。
        '(1076)- 已接受使用当前引导作为最后的有效控制设置。
        '(1077)-上次启动之后，仍未尝试引导服务。
        '(1078)-名称已用作服务名或服务显示 名。
        '(1079)-此服务的帐户不同于运行于同一进程上的其它服务的帐户。
        '(1080)-只能为 Win32 服务设置失败操作，不能为驱动程序设置。
        '(1081)-这个服务所运行的处理和服务控制管理器相同。所以，如果服务处理程序意外中止的话，服务控 制管理器无法进行任何操作。
        '(1082)-这个服务尚未设置恢复程序。
        '(1083)-配置成在该可执行程序中运行的这个服务不能执行该服 务。
        '(1100)-已达磁带的实际结尾。
        '(1101)-磁带访问已达文件标记。
        '(1102)-已达磁带或磁盘分区的开头。
        '(1103)- 磁带访问已达一组文件的结尾。
        '(1104)-磁带上不再有任何数据。
        '(1105)-磁带无法分区。
        '(1106)-在访问多卷分区 的新磁带时，当前的块大小不正确。
        '(1107)-当加载磁带时，找不到分区信息。
        '(1108)-无法锁定媒体弹出功能。
        '(1109)- 无法卸载介质。
        '(1110)-驱动器中的介质可能已更改。
        '(1111)-复位 I/O 总线。
        '(1112)-驱动器中没有媒体。
        '(1113)- 在多字节的目标代码页中，没有此 Unicode 字符可以映射到的字符。
        '(1114)-动态链接库 (DLL) 初始化例程失败。
        '(1115)- 系统关机正在进行。
        '(1116)-因为没有任何进行中的关机过程，所以无法中断系统关机。
        '(1117)-因为 I/O 设备错误，所以无法运行此项请求。
        '(1118)-没有串行设备被初始化成功。串行驱动程序将卸载。
        '(1119)-无法打开正在与其他设备 共享中断请求(IRQ)的设备。至少有一个使用该 IRQ 的其他设备已打开。
        '(1120)-序列 I/O 操作已由另一个串行口的写入完成。(IOCTL_SERIAL_XOFF_COUNTER 已达零。)
        '(1121)-因为已过超时时间，所以串行 I/O 操作完成。(IOCTL_SERIAL_XOFF_COUNTER 未达零。)
        '(1122)-在软盘上找不到 ID 地址标记。
        '(1123)- 软盘扇区 ID 字符域与软盘控制器磁道地址不相符。
        '(1124)-软盘控制器报告软盘驱动程序不能识别的错误。
        '(1125)-软盘控制 器返回与其寄存器中不一致的结果。

        '(1126)-当访问硬盘时，重新校准操作失败，重试仍然失败。
        '(1127)-当访问硬盘时，磁盘操作失败，重试仍然失败。
        '(1128)- 当访问硬盘时，即使失败，仍须复位磁盘控制器。
        '(1129)-已达磁带结尾。
        '(1130)-服务器存储空间不足，无法处理此命令。
        '(1131)- 检测出潜在的死锁状态。
        '(1132)-指定的基址或文件偏移量没有适当对齐。
        '(1140)-改变系统供电状态的尝试被另一应用程序或驱动 程序否决。
        '(1141)-系统 BIOS 改变系统供电状态的尝试失败。
        '(1142)-试图在一文件上创建超过系统允许数额的链接。
        '(1150)- 指定程序要求更新的 Windows 版本。
        '(1151)-指定程序不是 Windows 或 MS-DOS 程序。
        '(1152)-只能 启动该指定程序的一个实例。
        '(1153)-该指定程序适用于旧的 Windows 版本。
        '(1154)-执行该应用程序所需的库文件之一 被损坏。
        '(1155)-没有应用程序与此操作的指定文件有关联。
        '(1156)-在输送指令到应用程序的过程中出现错误。　
        '(1157)- 执行该应用程序所需的库文件之一无法找到。
        '(1158)-当前程序已使用了 Window 管理器对象的系统允许的所有句柄。
        '(1159)- 消息只能与同步操作一起使用。
        '(1160)-指出的源元素没有媒体。
        '(1161)-指出的目标元素已包含媒体。
        '(1162)-指 出的元素不存在。
        '(1163)-指出的元素是未显示的存储资源的一部分。
        '(1164)-显示设备需要重新初始化，因为硬件有错误。
        '(1165)- 设备显示在尝试进一步操作之前需要清除。
        '(1166)-设备显示它的门仍是打开状态。
        '(1167)-设备没有连接。
        '(1168)- 找不到元素。
        '(1169)-索引中没有同指定项相匹配的项。
        '(1170)-在对象上不存在指定的属性集。
        '(1171)-传递到 GetMouseMovePoints 的点不在缓冲区中。
        '(1172)-跟踪(工作站)服务没运行。
        '(1173)-找不到卷 ID。
        '(1175)- 无法删除要被替换的文件。
        '(1176)-无法将替换文件移到要被替换的文件。要被替换的文件保持原来的名称。
        '(1177)-无法将替换文 件移到要被替换的文件。要被替换的文件已被重新命名为备份名称。
        '(1178)-卷更改记录被删除。
        '(1179)-卷更改记录服务不处于活 动中。
        '(1180)-找到一份文件，但是可能不是正确的文件。
        '(1181)-日志项从日志中被删除。
        '(1200)-指定的设备名 无效。
        '(1201)-设备当前未连接上，但其为一个记录连接。
        '(1202)-企图记录先前已被记录的设备。
        '(1203)-无任何 网络提供程序接受指定的网络路径。
        '(1204)-指定的网络提供程序名称无效。
        '(1205)-无法打开网络连接配置文件。
        '(1206)- 网络连接配置文件损坏。
        '(1207)-无法枚举空载体。
        '(1208)-发生扩展错误。
        '(1209)-指定的组名格式无效。
        '(1210)- 指定的计算机名格式无效。
        '(1211)-指定的事件名格式无效。
        '(1212)-指定的域名格式无效。
        '(1213)-指定的服务名 格式无效。
        '(1214)-指定的网络名格式无效。
        '(1215)-指定的共享名格式无效。
        '(1216)-指定的密码格式无效。
        '(1217)- 指定的消息名格式无效。
        '(1218)-指定的消息目标格式无效。
        '(1219)-提供的凭据与已存在的凭据集冲突。
        '(1220)- 企图创建网络服务器的会话，但已对该服务器创建过多的会话。

        '(1221)-工作组或域名已由网络上的另一部计算机使用。
        '(1222)-网络未连接或启动。
        '(1223)-操作已被用户取消。
        '(1224)- 请求的操作无法在使用用户映射区域打开的文件上执行。
        '(1225)-远程系统拒绝网络连接。
        '(1226)-网络连接已被适当地关闭了。
        '(1227)- 网络传输终结点已有与其关联的地址。
        '(1228)-地址仍未与网络终结点关联。
        '(1229)-企图在不存在的网络连接上进行操作。
        '(1230)- 企图在使用中的网络连接上进行无效的操作。
        '(1231)-不能访问网络位置。有关网络排除故障的信息，请参阅 Windows 帮助。
        '(1232)- 不能访问网络位置。有关网络排除故障的信息，请参阅 Windows 帮助。
        '(1233)-不能访问网络位置。有关网络排除故障的信息，请参阅 Windows 帮助。
        '(1234)-没有任何服务正在远程系统上的目标网络终结点上操作。
        '(1235)-请求被终止。
        '(1236)- 由本地系统终止网络连接。
        '(1237)-操作无法完成。应该重试。
        '(1238)-因为已达到此帐户的最大同时连接数限制，所以无法连接服 务器。
        '(1239)-试图在这个帐户未被授权的时间内登录。
        '(1240)-此帐户并未得到从这个工作站登录的授权。
        '(1241)- 请求的操作不能使用这个网络地址。
        '(1242)-服务器已经注册。
        '(1243)-指定的服务不存在。
        '(1244)-因为用户还未 被验证，不能执行所要求的操作。
        '(1245)-因为用户还未登录网络，不能执行所要求的操作。指定的服务不存在。
        '(1246)-正在继续 工作。
        '(1247)-试图进行初始操作，但是初始化已完成。
        '(1248)-没有更多的本地设备。　
        '(1249)-指定的站点不存 在。
        '(1250)-具有指定名称的域控制器已经存在。
        '(1251)-只有连接到服务器上时，该操作才受支持。
        '(1252)-即使 没有改动，组策略框架也应该调用扩展。
        '(1253)-指定的用户没有一个有效的配置文件。
        '(1254)-Microsoft Small Business Server 不支持此操作。
        '(1300)-并非所有被引用的特权都指派给呼叫方。
        '(1301)-帐户名和安全标识 间的某些映射未完成。
        '(1302)-没有为该帐户特别设置系统配额限制。
        '(1303)-没有可用的加密密钥。返回了一个已知加密密钥。
        '(1304)- 密码太复杂，无法转换成 LAN Manager 密码。返回的 LAN Manager 密码为空字符串。
        '(1305)-修订级别未知。
        '(1306)- 表明两个修订级别是不兼容的。
        '(1307)-这个安全标识不能指派为此对象的所有者。
        '(1308)-这个安全标识不能指派为对象的主要 组。
        '(1309)-当前并未模拟客户的线程试图操作模拟令牌。
        '(1310)-组可能未被禁用。
        '(1311)-当前没有可用的登录 服务器来服务登录请求。
        '(1312)-指定的登录会话不存在。可能已被终止。
        '(1313)-指定的特权不存在。
        '(1314)-客 户没有所需的特权。
        '(1315)-提供的名称并非正确的帐户名形式。
        '(1316)-指定的用户已存在。
        '(1317)-指定的用户 不存在。
        '(1318)-指定的组已存在。
        '(1319)-指定的组不存在。
        '(1320)-指定的用户帐户已是指定组的成员，或是因 为组包含成员所以无法删除指定的组。
        '(1321)-指定的用户帐户不是指定组帐户的成员。
        '(1322)-无法禁用或删除最后剩余的系统管 理帐户。

        '(1323)-无法更新密码。提供作为当前密码的值不正确。
        '(1324)-无法更新密码。提供给新密码的值包含密码中不允许的值。
        '(1325)- 无法更新密码。为新密码提供的值不符合字符域的长度、复杂性或历史要求。
        '(1326)-登录失败: 未知的用户名或错误密码。
        '(1327)- 登录失败: 用户帐户限制。
        '(1328)-登录失败: 违反帐户登录时间限制。
        '(1329)-登录失败: 不允许用户登录到此计算机。
        '(1330)- 登录失败: 指定的帐户密码已过期。
        '(1331)-登录失败: 禁用当前的帐户。
        '(1332)-帐户名与安全标识间无任何映射完成。
        '(1333)- 一次请求过多的本地用户标识符(LUIDs)。
        '(1334)-无更多可用的本地用户标识符(LUIDs)。
        '(1335)-对于该特别用 法，安全 ID 的次级授权部分无效。
        '(1336)-访问控制列表(ACL)结构无效。
        '(1337)-安全 ID 结构无效。
        '(1338)- 安全描述符结构无效。
        '(1340)-无法创建固有的访问控制列表(ACL)或访问控制项目(ACE)。
        '(1341)-服务器当前已禁用。
        '(1342)- 服务器当前已启用。
        '(1343)-提供给识别代号颁发机构的值为无效值。
        '(1344)-无更多可用的内存以更新安全信息。
        '(1345)- 指定属性无效，或与整个群体的属性不兼容。
        '(1346)-指定的模拟级别无效， 或所提供的模拟级别无效。
        '(1347)-无法打开匿名级 安全令牌。
        '(1348)-请求的验证信息类别无效。
        '(1349)-令牌的类型对其尝试使用的方法不适当。
        '(1350)-无法在与 安全性无关联的对象上运行安全性操作。
        '(1351)-未能从域控制器读取配置信息，或者是因为机器不可使用，或者是访问被拒绝。
        '(1352)- 安全帐户管理器(SAM)或本地安全颁发机构(LSA)服务器处于运行安全操作的错误状态。
        '(1353)-域处于运行安全操作的错误状态。
        '(1354)- 此操作只对域的主要域控制器可行。
        '(1355)-指定的域不存在，或无法联系。
        '(1356)-指定的域已存在。
        '(1357)-试 图超出每服务器域个数的限制。
        '(1358)-无法完成请求操作，因为磁盘上的严重介质失败或数据结构损坏。
        '(1359)-出现了内部错 误。
        '(1360)-通用访问类型包含于已映射到非通用类型的访问掩码中。
        '(1361)-安全描述符格式不正确 (绝对或自相关的)。
        '(1362)- 请求操作只限制在登录进程中使用。调用进程未注册为一个登录进程。
        '(1363)-无法使用已在使用中的标识启动新的会话。
        '(1364)- 未知的指定验证数据包。
        '(1365)-登录会话并非处于与请求操作一致的状态中。
        '(1366)-登录会话标识已在使用中。
        '(1367)- 登录请求包含无效的登录类型值。
        '(1368)-在使用命名管道读取数据之前，无法经由该管道模拟。
        '(1369)-注册表子树的事务处理状 态与请求状态不一致。
        '(1370)-安全性数据库内部出现损坏。
        '(1371)-无法在内置帐户上运行此操作。
        '(1372)-无法 在内置特殊组上运行此操作。
        '(1373)-无法在内置特殊用户上运行此操作。
        '(1374)-无法从组中删除用户，因为当前组为用户的主要 组。
        '(1375)-令牌已作为主要令牌使用。
        '(1376)-指定的本地组不存在。
        '(1377)-指定的帐户名不是本地组的成员。
        '(1378)- 指定的帐户名已是本地组的成员。
        '(1379)-指定的本地组已存在。

        '(1380)-登录失败: 未授予用户在此计算机上的请求登录类型。
        '(1381)-已超过在单一系统中可保存机密的最大个数。
        '(1382)- 机密的长度超过允许的最大长度。
        '(1383)-本地安全颁发机构数据库内部包含不一致性。
        '(1384)-在尝试登录的过程中，用户的安全 上下文积累了过多的安全标识。
        '(1385)-登录失败: 未授予用户在此计算机上的请求登录类型。
        '(1386)-更改用户密码时需要交叉 加密密码。
        '(1387)-由于成员不存在，无法将成员添加到本地组中，也无法从本地组将其删除。
        '(1388)-无法将新成员加入到本地组 中，因为成员的帐户类型错误。
        '(1389)-已指定过多的安全标识。
        '(1390)-更改此用户密码时需要交叉加密密码。
        '(1391)- 表明 ACL 未包含任何可承继的组件。
        '(1392)-文件或目录损坏且无法读取。
        '(1393)-磁盘结构损坏且无法读取。
        '(1394)- 无任何指定登录会话的用户会话项。
        '(1395)-正在访问的服务有连接数目标授权限制。这时候已经无法再连接，原因是已经到达可接受的连接数目上 限。
        '(1396)-登录失败: 该目标帐户名称不正确。
        '(1397)-相互身份验证失败。该服务器在域控制器的密码过期。
        '(1398)- 在客户机和服务器之间有一个时间差。
        '(1400)-无效的窗口句柄。
        '(1401)-无效的菜单句柄。
        '(1402)-无效的光标句 柄。
        '(1403)-无效的加速器表句柄。
        '(1404)-无效的挂钩句柄。
        '(1405)-无效的多重窗口位置结构句柄。
        '(1406)- 无法创建最上层子窗口。
        '(1407)-找不到窗口类别。
        '(1408)-无效窗口；它属于另一线程。
        '(1409)-热键已注册。
        '(1410)- 类别已存在。
        '(1411)-类别不存在。
        '(1412)-类别仍有打开的窗口。
        '(1413)-无效索引。
        '(1414)-无 效的图标句柄。
        '(1415)-使用专用 DIALOG 窗口字。
        '(1416)-找不到列表框标识。
        '(1417)-找不到通配字 符。
        '(1418)-线程没有打开的剪贴板。
        '(1419)-没有注册热键。
        '(1420)-窗口不是合法的对话窗口。
        '(1421)- 找不到控件 ID。
        '(1422)-因为没有编辑控制，所以组合框的消息无效。
        '(1423)-窗口不是组合框。
        '(1424)-高度 必须小于 256。
        '(1425)-无效的设备上下文(DC)句柄。
        '(1426)-无效的挂接程序类型。
        '(1427)-无效的挂接 程序。
        '(1428)-没有模块句柄无法设置非本机的挂接。
        '(1429)-此挂接程序只可整体设置。
        '(1430)-Journal Hook 程序已安装。
        '(1431)-挂接程序尚未安装。
        '(1432)-单一选择列表框的无效消息。
        '(1433)-LB_SETCOUNT 发送到非被动的列表框。
        '(1434)-此列表框不支持 Tab 键宽度。
        '(1435)-无法毁坏由另一个线程创建的对象。
        '(1436)- 子窗口没有菜单。
        '(1437)-窗口没有系统菜单。
        '(1438)-无效的消息对话框样式。
        '(1439)-无效的系统范围内的 (SPI_*) 参数。
        '(1440)-已锁定屏幕。
        '(1441)-多重窗口位置结构中窗口的所有句柄必须具有相同的上层。
        '(1442)- 窗口不是子窗口。
        '(1443)-无效的 GW_* 命令。
        '(1444)-无效的线程标识。
        '(1445)-无法处理非多重文档界面 (MDI) 窗口中的消息。
        '(1446)-弹出式菜单已经激活。
        '(1447)-窗口没有滚动条。
        '(1448)-滚动条范围不可 大于 MAXLONG。

        '(1449)-无法以指定的方式显示或删除窗口。
        '(1450)-系统资源不足，无法完成请求的服务。
        '(1451)-系统资源不足， 无法完成请求的服务。
        '(1452)-系统资源不足，无法完成请求的服务。
        '(1453)-配额不足，无法完成请求的服务。
        '(1454)- 配额不足，无法完成请求的服务。
        '(1455)-页面文件太小，无法完成操作。
        '(1456)-找不到菜单项。
        '(1457)-键盘布 局句柄无效。
        '(1458)-不允许使用挂钩类型。
        '(1459)-该操作需要交互式窗口工作站。
        '(1460)-由于超时时间已过， 该操作返回。
        '(1461)-无效监视器句柄。
        '(1500)-事件日志文件损坏。
        '(1501)-无法打开事件日志文件，事件日志服 务没有启动。
        '(1502)-事件日志文件已满。
        '(1503)-事件日志文件已在读?涓摹?
        '(1601)-无法访问 Windows 安装服务。请与技术支持人员联系，确认 Windows 安装服务是否注册正确。
        '(1602)-用户取消了安装。
        '(1603)- 安装时发生严重错误
        '(1604)-安装已挂起，未完成。
        '(1605)-这个操作只对当前安装的产品有效。
        '(1606)-功能 ID 未注册。
        '(1607)-组件 ID 并未注册。
        '(1608)-未知属性。
        '(1609)-句柄处于不正确的状态。
        '(1610)- 这个产品的配置数据已损坏。请与技术支持人员联系。
        '(1611)-组件限制语不存在。
        '(1612)-这个产品的安装来源无法使用。请验证 来源是否存在，是否可以访问。
        '(1613)-Windows 安装服务无法安装这个安装程序包。您必须安装含有 Windows 安装服务新版本的 Windows Service Park。
        '(1614)-没有卸载产品。
        '(1615)-SQL 查询语法不正确或不被支持。
        '(1616)-记录字符域不存在。
        '(1617)-设备已被删除.
        '(1618)-正在进行另一个安装操 作。请在继续这个安装操作之前完成那个操作。
        '(1619)-未能打开这个安装程序包。请验证程序包是否存在，是否可以访问；或者与应用程序供应商 联系，验证这是否是有效的 Windows 安装服务程序包。
        '(1620)-未能打开这个安装程序包。请与应用程序供应商联系，验证这是否是有效 的 Windows 安装服务程序包。
        '(1621)-启动 Windows 安装服务用户界面时有错误。请与技术支持人员联系。
        '(1622)- 打开安装日志文件的错误。请验证指定的日志文件位置是否存在，是否可以写入。
        '(1623)-安装程序包的语言不受系统支持。
        '(1624)- 应用变换时的错误。请验证指定的变换路径是否有效。
        '(1625)-系统策略禁止这个安装。请与系统管理员联系。
        '(1626)-无法执行函 数。
        '(1627)-执行期间，函数出了问题。
        '(1628)-指定了无效的或未知的表格。
        '(1629)-提供的数据类型不对。
        '(1630)- 这个类型的数据不受支持。
        '(1631)-Windows 安装服务未能启动。请与技术支持人员联系。
        '(1632)-临时文件夹已满或无法 使用。请验证临时文件夹是否存在，是否可以写入。
        '(1633)-这个处理器类型不支持该安装程序包。请与产品供应商联系。
        '(1634)- 组件没有在这台计算机上使用。
        '(1635)-无法打开修补程序包。请验证修补程序包是否存在，是否可以访问；或者与应用程序供应商联系，验证这是 否是 Windows 安装服务的修补程序包。
        '(1636)-无法打开修补程序包。请与应用程序供应商联系，验证这是否是 Windows 安装服务的修补程序包。
        '(1637)-Windows 安装服务无法处理这个插入程序包。您必须安装含有 Windows 安装服务新版本的 Windows Service Pack。
        '(1638)-已安装这个产品的另一个版本。这个版本的安装无法继续。要配置或删除这个产品的现有版 本，请用“控制面板”上的“添加/删除程序”。
        '(1639)-无效的命令行参数。有关详细的命令行帮助，请查阅 Windows 安装服务的 SDK。
        '(1640)-在终端服务远程会话期间，只有管理员有添加、删除或配置服务器软件的权限。如果您要在服务器上安装或配置软件，请与网络管 理员联系。
        '(1641)-要求的操作已成功结束。要使改动生效，必须重新启动系统。
        '(1642)-Windows 安装服务无法安装升级修补程序，因为被升级的程序可能会丢失或是升级修补程序可能更新此程序的一个不同版本。请确认要被升级的程序在您的计算机上且您的升 级修补程序是正确的。
        '(1700)-串绑定无效。
        '(1701)-绑定句柄类型不正确。
        '(1702)-绑定句柄无效。
        '(1703)- 不支持 RPC 协议序列。
        '(1704)-RPC 协议序列无效。
        '(1705)-字符串通用唯一标识 (UUID) 无效。
        '(1706)- 终结点格式无效。
        '(1707)-网络地址无效。
        '(1708)-找不到终结点。
        '(1709)-超时值无效。
        '(1710)- 找不到对象通用唯一标识(UUID)。
        '(1711)-对象通用唯一标识 (UUID)已注册。
        '(1712)-类型通用唯一标识 (UUID)已注册。
        '(1713)-RPC 服务器已在侦听。
        '(1714)-未登记任何协议序列。
        '(1715)-RPC 服务器未在侦听。
        '(1716)-未知的管理器类型。
        '(1717)-未知的界面。
        '(1718)-没有任何链接。
        '(1719)- 无任何协议顺序。
        '(1720)-无法创建终结点。
        '(1721)-资源不足，无法完成此操作。
        '(1722)-RPC 服务器不可用。
        '(1723)-RPC 服务器过忙以致无法完成此操作。
        '(1724)-网络选项无效。
        '(1725)-在此线程中， 没有使用中的远程过程调用。
        '(1726)-远程过程调用失败。
        '(1727)-远程过程调用失败且未运行。
        '(1728)-远程过程 调用(RPC)协议出错。
        '(1730)-RPC 服务器不支持传送语法。
        '(1732)-不支持通用唯一标识(UUID)类型。
        '(1733)- 标记无效。
        '(1734)-数组边界无效。
        '(1735)-链接不包含项目名称。
        '(1736)-名称语法无效。
        '(1737)- 不支持名称语法。
        '(1739)-没有可用来创建通用唯一标识 (UUID)的网络地址。
        '(1740)-终结点是一份备份。
        '(1741)- 未知的验证类型。
        '(1742)-调用的最大个数太小。
        '(1743)-字符串太长。
        '(1744)-找不到 RPC 协议顺序。
        '(1745)- 过程号超出范围。
        '(1746)-绑定不包含任何验证信息。
        '(1747)-未知的验证服务。
        '(1748)-未知的验证级别。
        '(1749)- 安全上下文无效。
        '(1750)-未知的授权服务。
        '(1751)-项目无效。
        '(1752)-服务器终结点无法运行操作。
        '(1753)- 终结点映射表中无更多的可用终结点。
        '(1754)-未导出任何界面。
        '(1755)-项目名称不完整。
        '(1756)-版本选项无 效。
        '(1757)-没有其他成员。
        '(1758)-没有内容未导出。
        '(1759)-接口没有找到。
        '(1760)-项目已存 在。
        '(1761)-找不到项目。
        '(1762)-无可用的名称服务。
        '(1763)-网络地址族无效。

        '(1764)-不支持请求的操作。
        '(1765)-无可用的安全上下文以允许模拟。
        '(1766)-远程过程调用(RPC)中发生内部 错误。
        '(1767)-RPC 服务器试图以零除整数。
        '(1768)-RPC 服务器中发生地址错误。
        '(1769)-RPC 服务器上的浮点操作导至以零做除数。
        '(1770)-RPC 服务器上发生浮点下溢。
        '(1771)-RPC 服务器上发生浮点上溢。
        '(1772)- 自动句柄绑定的可用 RPC 服务器列表已用完。
        '(1773)-无法打开字符翻译表文件。
        '(1774)-包含字符翻译表的文件少于512 字节。
        '(1775)-在远程过程调用时，将空的上下文句柄从客户传递到主机。
        '(1777)-在远程过程调用时，上下文句柄已更改。
        '(1778)- 传递到远程过程调用的绑定句柄不相符。
        '(1779)-承接体无法获得远程过程调用句柄。
        '(1780)-传递空引用指针到承接体。
        '(1781)- 列举值超出范围。
        '(1782)-字节计数太小。
        '(1783)-承接体接收到坏数据。
        '(1784)-提供给请求操作的用户缓冲区无 效。
        '(1785)-磁盘媒体无法识别。可能未被格式化。
        '(1786)-工作站没有信任机密。
        '(1787)-服务器上的安全数据库 没有此工作站信任关系的计算机帐户。
        '(1788)-主域和受信域间的信任关系失败。
        '(1789)-此工作站和主域间的信任关系失败。
        '(1790)- 网络登录失败。
        '(1791)-此线程的远程过程调用已在进行中。
        '(1792)-试图登录，但是网络登录服务没有启动。
        '(1793)- 用户帐户到期。
        '(1794)-转发程序已被占用且无法卸载。
        '(1795)-指定的打印机驱动程序已安装。
        '(1796)-指定的端 口未知。
        '(1797)-未知的打印机驱动程序。
        '(1798)-未知的打印机处理器。
        '(1799)-指定的分隔页文件无效。
        '(1800)- 指定的优先级无效。
        '(1801)-打印机名无效。
        '(1802)-打印机已存在。
        '(1803)-打印机命令无效。
        '(1804)- 指定的数据类型无效。
        '(1805)-指定的环境无效。
        '(1806)-没有更多的绑定。
        '(1807)-所用帐户为域间信任帐户。请 使用您的全局用户帐户或本地用户帐户来访问这台服务器。
        '(1808)-所用帐户是一个计算机帐户。使用您的全局用户帐户或本地用户帐户来访问此服 务器。
        '(1809)-已使用的帐户为服务器信任帐户。使用您的全局用户帐户或本地用户帐户来访问此服务器。
        '(1810)-指定域的名称或 安全标识(SID)与该域的信任信息不一致。
        '(1811)-服务器在使用中且无法卸载。
        '(1812)-指定的映像文件不包含资源区域。
        '(1813)- 找不到映像文件中指定的资源类型。
        '(1814)-找不到映像文件中指定的资源名。
        '(1815)-找不到映像文件中指定的资源语言标识。
        '(1816)- 配额不足，无法处理此命令。
        '(1817)-未登记任何界面。
        '(1818)-远程过程调用被取消。
        '(1819)-绑定句柄不包含所 有需要的信息。
        '(1820)-在远程过程调用过程中通讯失败。
        '(1821)-不支持请求的验证级别。
        '(1822)-未登记任何主 名称。
        '(1823)-指定的错误不是有效的 Windows RPC 错误码。
        '(1824)-已配置一个只在这部计算机上有效的 UUID。
        '(1825)-发生一个安全包特有的错误。
        '(1826)-线程未取消。
        '(1827)-无效的编码/解码句柄操作。
        '(1828)- 序列化包装的版本不兼容。

        '(1829)-RPC 承接体的版本不兼容。
        '(1830)-RPC 管道对象无效或已损坏。
        '(1831)-试图在 RPC 管道物件上进行无效操作。
        '(1832)-不被支持的 RPC 管道版本。
        '(1898)-找不到该组成员。
        '(1899)-无法创建 终结点映射表数据库项。
        '(1900)-对象通用唯一标识 (UUID) 为 nil UUID。
        '(1901)-指定的时间无效。
        '(1902)- 指定的格式名称无效。
        '(1903)-指定的格式大小无效。
        '(1904)-指定的打印机句柄正等候在
        '(1905)-已删除指定的打 印机。
        '(1906)-打印机的状态无效。
        '(1907)-在第一次登录之前，必须更改用户密码。
        '(1908)-找不到此域的域控制 器。
        '(1909)-引用的帐户当前已锁定，且可能无法登录。
        '(1910)-没有发现指定的此对象导出者
        '(1911)-没有发现指 定的对象。
        '(1912)-没有发现指定的对象解析器。
        '(1913)-一些待发数据仍停留在请求缓冲区内。
        '(1914)-无效的异 步远程过程调用句柄。
        '(1915)-这个操作的异步 RPC 调用句柄不正确。
        '(1916)-RPC 管道对象已经关闭。
        '(1917)- 在 RPC 调用完成之前全部的管道都已处理完成。
        '(1918)-没有其他可用的数据来自 RPC 管道。
        '(1919)-这个机器没有可 用的站点名。
        '(1920)-系统无法访问此文件。
        '(1921)-系统无法辨识文件名。
        '(1922)-项目不是所要的类型。
        '(1923)- 无法将所有对象的 UUID 导出到指定的项。
        '(1924)-无法将界面导出到指定的项。
        '(1925)-无法添加指定的配置文件项。
        '(1926)- 无法添加指定的配置文件元素。
        '(1927)-无法删除指定的配置文件元素。
        '(1928)-无法添加组元素。
        '(1929)-无法删 除组元素。
        '(2000)-无效的像素格式。
        '(2001)-指定的驱动程序无效。
        '(2002)-窗口样式或类别属性对此操作无效。
        '(2003)- 不支持请求的图元操作。
        '(2004)-不支持请求的变换操作。
        '(2005)-不支持请求的剪切操作。
        '(2010)-指定的颜色管 理模块无效。
        '(2011)-制定的颜色文件配置无效。
        '(2012)-找不到指定的标识。
        '(2013)-找不到所需的标识。
        '(2014)- 指定的标识已经存在。
        '(2015)-指定的颜色文件配置与任何设备都不相关。
        '(2016)-找不到该指定的颜色文件配置
        '(2017)- 指定的颜色空间无效。
        '(2018)-图像颜色管理没有启动。
        '(2019)-在删除该颜色传输时有一个错误。
        '(2020)-该指定 的颜色传输无效。
        '(2021)-该指定的变换与位图的颜色空间不匹配。
        '(2022)-该指定的命名颜色索引在配置文件中不存在。
        '(2102)- 没有安装工作站驱动程序。
        '(2103)-无法定位服务器。
        '(2104)-发生内部错误，网络无法访问共享内存段。
        '(2105)- 网络资源不足。
        '(2106)-工作站不支持这项操作。
        '(2107)-设备没有连接。
        '(2108)-网络连接已成功，但需要提示用 户输入一个不同于原始指定的密码。
        '(2114)-没有启动服务器服务。
        '(2115)-队列空。
        '(2116)-设备或目录不存在。
        '(2117)- 无法在重定向的资源上执行这项操作。
        '(2118)-名称已经共享。
        '(2119)-服务器当前无法提供所需的资源。
        '(2121)- 额外要求的项目超过允许的上限。
        '(2122)-对等服务只支持两个同时操作的用户 。
        '(2123)-API 返回缓冲区太小。

        '(2127)-远程 API 错误。
        '(2131)-打开或读取配置文件时出错。
        '(2136)-发生一般网络错误。
        '(2137)- 工作站服务的状态不一致。重新启动工作站服务之前，请先重新启动计算机。
        '(2138)-工作站服务没有启动。
        '(2139)-所需信息不可 用。
        '(2140)-发生 Windows 2000 内部错误。
        '(2141)-服务器没有设置事务处理。
        '(2142)-远程服务 器不支持请求的 API。
        '(2143)-事件名无效。
        '(2144)-网络上已经有此计算机名。请更名后重新启动。
        '(2146)- 配置信息中找不到指定的组件。
        '(2147)-配置信息中找不到指定的参数。
        '(2149)-配置文件中有一个命令行太长。
        '(2150)- 打印机不存在。
        '(2151)-打印作业不存在。
        '(2152)-打印机目标找不到。
        '(2153)-打印机目标已经存在。
        '(2154)- 打印机队列已经存在。
        '(2155)-无法添加其它的打印机。
        '(2156)-无法添加其它的打印作业。
        '(2157)-无法添加其它 的打印机目标。
        '(2158)-此打印机目标处于空闲中，不接受控制操作。
        '(2159)-此“打印机目标请求”包含无效的控制函数。
        '(2160)- 打印处理程序没有响应。
        '(2161)-后台处理程序没有运行。
        '(2162)-打印目标当前的状况，无法执行这项操作。
        '(2163)- 打印机队列当前的状况，无法执行这项操作。
        '(2164)-打印作业当前的状况，无法执行这项操作。
        '(2165)-无法为后台处理程序分配 内存。
        '(2166)-设备驱动程序不存在。
        '(2167)-打印处理程序不支持这种数据类型。
        '(2168)-没有安装打印处理程 序。
        '(2180)-锁定服务数据库。
        '(2181)-服务表已满。
        '(2182)-请求的服务已经启动。
        '(2183)-这项 服务没有响应控制操作。
        '(2184)-服务仍未启动。
        '(2185)-服务名无效。
        '(2186)-服务没有响应控制功能。
        '(2187)- 服务控制处于忙碌状态。
        '(2188)-配置文件包含无效的服务程序名。
        '(2189)-在当前的状况下无法控制服务。
        '(2190)- 服务异常终止。
        '(2191)-这项服务无法接受请求的 “暂停” 或 “停止” 操作。
        '(2192)-服务控制“计划程序”在“计划表” 中找不到服务名。
        '(2193)-无法读取服务控制计划程序管道。
        '(2194)-无法创建新服务的线程。
        '(2200)-此工作站已 经登录到局域网。
        '(2201)-工作站没有登录到局域网。
        '(2202)-指定的用户名无效。
        '(2203)-密码参数无效。
        '(2204)- 登录处理器没有添加消息别名。
        '(2205)-登录处理器没有添加消息别名。
        '(2206)-注销处理器没有删除消息别名。
        '(2207)- 注销处理器没有删除消息别名。
        '(2209)-暂停网络登录。
        '(2210)-中心登录服务器发生冲突。
        '(2211)-服务器没有设 置正确的用户路径。
        '(2212)-加载或运行登录脚本时出错。
        '(2214)-没有指定登录服务器，计算机的登录状态是单机操作。
        '(2215)- 登录服务器找不到。
        '(2216)-此计算机已经有一个登录域。
        '(2217)-登录服务器无法验证登录。
        '(2219)-安全数据库 找不到。
        '(2220)-组名找不到。
        '(2221)-用户名找不到。
        '(2222)-资源名找不到。
        '(2223)-组已经存 在。
        '(2224)-帐户已经存在。
        '(2225)-资源使用权限清单已经存在。
        '(2226)-此操作只能在该域的主域控制器上执 行。

        '(2227)-安全数据库没有启动。
        '(2228)-用户帐户数据库中的名称太多。
        '(2229)-磁盘 I/O 失败。
        '(2230)- 已经超过每个资源 64 个项目的限制。
        '(2231)-不得删除带会话的用户。
        '(2232)-上层目录找不到。
        '(2233)-无 法添加到安全数据库会话高速缓存段。
        '(2234)-这项操作不能在此特殊的组上执行。
        '(2235)-用户帐户数据库会话高速缓存没有记录 此用户。
        '(2236)-用户已经属于此组。
        '(2237)-用户不属于此组。
        '(2238)-此用户帐户尚未定义。
        '(2239)- 此用户帐户已过期。
        '(2240)-此用户不得从此工作站登录网络。
        '(2241)-这时候不允许用户登录网络。
        '(2242)-此用 户的密码已经过期。
        '(2243)-此用户的密码无法更改。
        '(2244)-现在无法使用此密码。
        '(2245)-密码不满足密码策略 的需要。检查最小密码长度、密码复杂性和密码历史的需求。
        '(2246)-此用户的密码最近才启用，现在不能更改。
        '(2247)-安全数据 库已损坏。
        '(2248)-不需要更新此副本复制的网络/本地安全数据库。
        '(2249)-此副本复制的数据库已过时；请同步处理其中的数 据。
        '(2250)-此网络连接不存在。
        '(2251)-此 asg_type 无效。
        '(2252)-此设备当前正在共享中。
        '(2270)- 计算机名无法作为消息别名添加。网络上可能已经有此名称。
        '(2271)-信使服务已经启动。
        '(2272)-信使服务启动失败。
        '(2273)- 网络上找不到此消息别名。
        '(2274)-此消息别名已经转发出去。
        '(2275)-已经添加了此消息别名，但是仍被转发。
        '(2276)- 此消息别名已在本地存在。
        '(2277)-添加的消息别名已经超过数目上限。
        '(2278)-无法删除计算机名。
        '(2279)-消息 无法转发回到同一个工作站。
        '(2280)-域消息处理器出错。
        '(2281)-消息已经发送出去，但是收件者已经暂停信使服务。
        '(2282)- 消息已经发送出去，但尚未收到。
        '(2283)-消息别名当前正在使用中。请稍候片刻再试。
        '(2284)-信使服务尚未启动。
        '(2285)- 该名称不在本地计算机上。
        '(2286)-网络上找不到转发的消息别名。
        '(2287)-远程通讯站的消息别名表已经满了。
        '(2288)- 此别名的消息当前没有在转发中。
        '(2289)-广播的消息被截断。
        '(2294)-设备名无效。
        '(2295)-写入出错。
        '(2297)- 网络上的消息别名重复。
        '(2298)-此消息别名会在稍后删除。
        '(2299)-没有从所有的网络删除消息别名。
        '(2300)-这 项操作无法在使用多种网络的计算机上执行。
        '(2310)-此共享的资源不存在。
        '(2311)-设备没有共享。
        '(2312)-带此 计算机名的会话不存在。
        '(2314)-没有用此识别号打开的文件。
        '(2315)-执行远程管理命令失败。
        '(2316)-打开远程 临时文件失败。
        '(2317)-从远程管理命令返回的数据已经被截断成 64K。
        '(2318)-此设备无法同时共享为后台处理资源和非后台 处理资源。
        '(2319)-服务器清单中的信息可能不正确
        '(2320)-计算机在此域未处于活动状态
        '(2321)-在删除共享之 前，需要将该共享从分布式文件系统中删除。
        '(2331)-无法在此设备执行这项操作
        '(2332)-此设备无法共享。
        '(2333)- 此设备未打开。
        '(2334)-此设备名清单无效。
        '(2335)-队列优先级无效。

        '(2337)-没有任何共享的通讯设备。
        '(2338)-指定的队列不存在。
        '(2340)-此设备清单无效。
        '(2341)- 请求的设备无效。
        '(2342)-后台处理程序正在使用此设备。
        '(2343)-此设备已经被当成通讯设备来使用。
        '(2351)-此 计算机名无效。
        '(2354)-指定的字符串及前缀太长。
        '(2356)-此路径组成部分无效。
        '(2357)-无法判断输入类型。
        '(2362)- 类型缓冲区不够大。
        '(2370)-配置文件不得超过 64K。
        '(2371)-初始偏移量越界。
        '(2372)-系统无法删除当前到 网络资源的连接。
        '(2373)-系统无法分析此文件中的命令行。
        '(2374)-加载配置文件时出错。\
        '(2375)-保存配置文 件时出错，只部份地保存了配置文件。
        '(2378)-此日志文件在前后两次读取之间已经发生变化。
        '(2380)-资源路径不可以是目录。
        '(2381)- 资源路径无效。
        '(2382)-目标路径无效。
        '(2383)-源路径及目标路径分属不同的服务器。
        '(2385)-请求的 Run 服务器现在暂停。
        '(2389)-与 Run 服务器通讯时出错。
        '(2391)-启动后台处理时出错。
        '(2392)-找不到您连接 的共享资源。
        '(2400)-LAN 适配器号码无效。
        '(2401)-此网络连接有文件打开或请求挂起。
        '(2402)-使用中的连 接仍存在。
        '(2403)-此共享名或密码无效。
        '(2404)-设备正由活动进程使用，无法断开。
        '(2405)-此驱动器号已在本 地使用。
        '(2430)-指定的客户已经在指定的事件注册。
        '(2431)-警报表已满。
        '(2432)-发出的警报名称无效或不存 在。
        '(2433)-警报接收者无效。
        '(2434)-用户的登录时间长短不再合法。所以已经删除用户与该服务器的会话。
        '(2440)- 日志文件中没有请求的记录号。
        '(2450)-用户帐户数据库没有正确配置。
        '(2451)-当 Netlogon 服务正在运行时，不允许执行这项操作。
        '(2452)-这项操作无法在最后的管理帐户上执行。
        '(2453)-找不到此域的域控制器。
        '(2454)- 无法设置此用户的登录信息。
        '(2455)-Netlogon 服务尚未启动。
        '(2456)-无法添加到用户帐户数据库。
        '(2457)- 此服务器的时钟与主域控制器的时钟不一致。
        '(2458)-检测到密码不匹配。
        '(2460)-服务器识别码没有指定有效的服务器。
        '(2461)- 会话标识没有指定有效的会话。
        '(2462)-连接识别码没有指定有效的连接。
        '(2463)-可用服务器表中无法再加上其它项。
        '(2464)- 服务器已经到了支持的会话数目上限。
        '(2465)-服务器已经到了支持的连接数目上限。
        '(2466)-服务器打开的文件到了上限，无法打 开更多文件。
        '(2467)-这台服务器没有登记替换的服务器。
        '(2470)-请用低级的 API (远程管理协议)。
        '(2480)-UPS 服务无法访问 UPS 驱动程序。
        '(2481)-UPS 服务设置错误。
        '(2482)-UPS 服务无法访问指定通讯端口 (Comm Port)。
        '(2483)-UPS 显示线路中断或电池不足，服务没有启动。
        '(2484)-UPS 服务无法执行系统关机的操作。
        '(2500)- 下面的程序返回一个 MS-DOS 错误码:
        '(2501)-下面的程序需要更多的内存:
        '(2502)-下面程序调用了不支持的 MS-DOS 函数:
        '(2503)-工作站无法启动。
        '(2504)-下面的文件已损坏。
        '(2505)-启动块定义文件中没有指定 引导程序。

        '(2506)-NetBIOS 返回错误: NCB 及 SMB 数据转储。
        '(2507)-磁盘 I/O 错误。
        '(2508)-无 法替换映像参数。
        '(2509)-跨越磁盘扇区范围的映像参数太多。
        '(2510)-不是从用 /S 格式化的 MS-DOS软盘产生的映像。
        '(2511)-稍后会从远程重新启动。
        '(2512)-无法调用远程启动服务器。
        '(2513)-无法 连接到远程启动服务器。
        '(2514)-无法打开远程启动服务器上的映像文件。
        '(2515)-正在连接到远程启动服务器…
        '(2516)- 正在连接到远程启动服务器…
        '(2517)-远程启动服务已经停止，请检测错误记录文件，查明出错的原因。
        '(2518)-远程启动失败，请 检查错误日志文件，查明出错的原因。
        '(2519)-不允许第二个远程启动 (Remoteboot) 资源连接。
        '(2550)-浏览服务 设置成 MaintainServerList=No。
        '(2610)-因为没有网卡与这项服务一起启动，所以无法启动服务。
        '(2611)- 因为注册表中的启动信息不正确，所以无法启动服务。
        '(2612)-无法启动服务，原因是它的数据库找不到或损坏。
        '(2613)-因为找不 到 RPLFILES 共享的资源，所以无法启动服务。
        '(2614)-因为找不到 RPLUSER 组，所以无法启动服务。
        '(2615)- 无法枚举服务记录。
        '(2616)-工作站记录信息已损坏。
        '(2617)-工作站记录找不到。
        '(2618)-其它的工作站正在使用 此工作站名。
        '(2619)-配置文件记录已损坏。
        '(2620)-配置文件记录找不到。
        '(2621)-其它的配置文件正在使用此名 称。
        '(2622)-有很多工作站正在使用此配置文件。
        '(2623)-配置记录已损坏。
        '(2624)-配置记录找不到。
        '(2625)- 适配器识别记录已损坏。
        '(2626)-内部服务出错。
        '(2627)-供应商识别记录已损坏。
        '(2628)-启动块记录已损坏。
        '(2629)- 找不到此工作站的用户帐户记录。
        '(2630)-RPLUSER 本地组找不到。
        '(2631)-找不到启动块记录。
        '(2632)- 所选的配置文件与此工作站不兼容。
        '(2633)-其它的工作站正在使用所选的网卡。
        '(2634)-有些配置文件正在使用此配置。
        '(2635)- 有数个工作站、配置文件或配置正在使用此启动块。
        '(2636)-服务无法制作远程启动数据库的备份。
        '(2637)-找不到适配器记录。
        '(2638)- 找不到供应商记录。
        '(2639)-其它供应商记录正在使用此供应商名称。
        '(2640)-其它的启动区记录正在使用启动名称或供应商识别记 录。
        '(2641)-其它的配置正在使用此配置名称。
        '(2660)-由 Dfs 服务所维护的内部数据库已损坏
        '(2661)-内部 数据库中的一条记录已 损坏
        '(2662)-输入项路径与卷路径不匹配
        '(2663)-给定卷名已存在
        '(2664)-指定的服务器共 享已在 Dfs 中共享
        '(2665)-所显示的服务器共享不支持所显示的 Dfs 卷
        '(2666)-此操作在非叶卷上无效。
        '(2667)- 此操作在叶卷上无效。
        '(2668)-此操作不明确，因为该卷存在多服务器。
        '(2669)-无法创建连接点
        '(2670)-该服务器 不是 Dfs 可识别的
        '(2671)-指定的重命名目标路径无效。
        '(2672)-指定 Dfs 卷脱线
        '(2673)-指定的服务 器不为此卷服务
        '(2674)-检测到 Dfs 名中的环路
        '(2675)-在基于服务器的 Dfs 上不支持该操作
        '(2676)- 这个卷已经受该指定服务器共享支持

        '(2677)-无法删除这个卷的上一个服务器共享支持
        '(2678)-Inter-Dfs 卷不支持该操作
        '(2679)-Dfs 服务的内部状态已经变得不一致
        '(2680)-Dfs 服务已经安装在指定的服务器上
        '(2681)-被协调的 Dfs 数据是一样的
        '(2682)- 无法删除 Dfs 根目录卷 – 如需要请卸载 Dfs
        '(2683)-该共享的子目录或父目录已经存在在一个 Dfs 中
        '(2690)-Dfs 内部错误
        '(2691)-这台机器已经加入域 。
        '(2692)-这个机器目前未加入域。
        '(2693)-这台机器是域控制器，而且 无法从域中退出。
        '(2694)-目标域控制器不支持在 OU 中创建的机器帐户。
        '(2695)-指定的工作组名无效
        '(2696)- 指定的计算机名与域控制器上使用的默认语言不兼容。
        '(2697)-找不到指定的计算机帐户。
        '(2999)-这是 NERR 范围内的最后一个错误。
        '(3000)-指定了未知的打?嗍悠鳌?
        '(3001)-指定的打印机驱动程序当前正在使用。
        '(3002)- 找不到缓冲文件。
        '(3003)-未发送 StartDocPrinter 调用。
        '(3004)-未发送 AddJob 调用。
        '(3005)- 指定的打印处理器已经安装。
        '(3006)-指定的打?嗍悠饕丫沧啊?
        '(3007)-该指定的打?嗍悠鞑痪弑杆蟮墓δ堋！?
        '(3008)- 该指定的打?嗍悠髡谑褂弥小?
        '(3009)-当打印机有作业排成队列时此操作请求是不允许的。
        '(3010)-请求的操作成功。直到重新 启动系统前更改将不会生效。
        '(3011)-请求的操作成功。直到重新启动服务前更改将不会生效。
        '(3012)-找不到打印机。
        '(3023)- 用户指定的关机命令文件，它的配置有问题。不过 UPS 服务已经启动。
        '(3029)-因为用户帐户数据库 (NET.ACC) 找不到或损坏，而且也没有可用的备份数据库，所以不能启动本地安全机制。系统不安全！
        '(3037)-@I *登录小时数
        '(3039)-已 经超过一个目录中文件的副本复制的限制。
        '(3040)-已经超过副本复制的目录树深度限制。
        '(3046)-无法登录。用户当前已经登录， 同时参数 TRYUSER设置为 NO。
        '(3052)-命令行或配置文件中没有提供必要的参数。
        '(3054)-无法满足资源的请求。
        '(3055)- 系统配置有问题。
        '(3056)-系统出错。
        '(3057)-发生内部一致性的错误。
        '(3058)-配置文件或命令行的选项不明确。
        '(3059)- 配置文件或命令行的参数重复。
        '(3060)-服务没有响应控制，DosKillProc 函数已经停止服务。
        '(3061)-运行服务程序 时出错。
        '(3062)-无法启动次级服务。
        '(3064)-文件有问题。
        '(3070)-内存
        '(3071)-磁盘空间
        '(3072)- 线程
        '(3073)-过程
        '(3074)-安全性失败。
        '(3075)-LAN Manager 根目录不正确或找不到。
        '(3076)- 未安装网络软件。
        '(3077)-服务器未启动。
        '(3078)-服务器无法访问用户帐户数据库 (NET.ACC)。
        '(3079)-LANMAN 树中安装的文件不兼容。
        '(3080)-LANMAN\LOGS 目录无效。
        '(3081)-指定的域无法使用。
        '(3082)-另 一计算机正将此计算机名当作消息别名使用。
        '(3083)-宣布服务器名失败。
        '(3084)-用户帐户数据库没有正确配置。
        '(3085)- 服务器没有运行用户级安全功能。
        '(3087)-工作站设置不正确。
        '(3088)-查看您的错误日志文件以了解详细信息。

        '(3089)-无法写入此文件。
        '(3090)-ADDPAK 文件损坏。请删除 LANMAN\NETPROG\ADDPAK.SER后重新应用所有的 ADDPAK。
        '(3091)-因为没有运行 CACHE.EXE，所以无法启动 LM386 服务器。
        '(3092)-安全数据库中找不到这台计算机的帐户。
        '(3093)-这台计算机 不是 SERVERS 组的成员。
        '(3094)-SERVERS 组没有在本地安全数据库中。
        '(3095)-此 Windows NT 计算机被设置为某个组的成员，并不是域的成员。此种配置下不需要运行 Netlogon 服务。
        '(3096)-找不到此域的 Windows NT 域控制器。
        '(3098)-服务无法与主域控制器进行验证。
        '(3099)-安全数据库文件创建日期或序号有问题。
        '(3100)- 因为网络软件出错，所以无法执行操作。
        '(3102)-这项服务无法长期锁定网络控制块 (NCB) 的段。错误码就是相关数据。
        '(3103)- 这项服务无法解除网络控制块 (NCB) 段的长期锁定。错误码就是相关数据。
        '(3106)-收到意外的网络控制块 (NCB)。NCB 就是相关数据。
        '(3107)-网络没有启动。
        '(3108)-NETWKSTA.SYS 的 DosDevIoctl 或 DosFsCtl 调用失败。显示的数据为以下格式:DWORD 值代表调用 Ioctl 或 FsCtl 的 CS:IP WORD 错误代码WORD Ioctl 或 FsCtl 号
        '(3111)-发生意外的 NetBIOS 错误。错误码就是相关数据。
        '(3112)-收到的服务器消 息块 (SMB) 无效。SMB 就是相关数据。
        '(3114)-因为缓冲区溢出，所以错误日志文件中部份的项目丢失。
        '(3120)-控制 网络缓冲区以外资源用量的初始化参数被设置大小，因此需要的内存太多。
        '(3121)-服务器无法增加内存段的大小。
        '(3124)-服务器 启动失败。三个 chdev参数必须同时为零或者同时不为零。
        '(3129)-服务器无法更新 AT 计划文件。文件损坏。
        '(3130)- 服务器调用 NetMakeLMFileName 时出错。错误码就是相关数据。
        '(3132)-无法长期锁定服务器缓冲区。请检查交换磁盘的可用 空间，然后重新启动系统以启动服务器。
        '(3140)-因为多次连续出现网络控制块 (NCB) 错误，所以停止服务。最后一个坏的 NCB 以原始数据形式出现。
        '(3141)-因为消息服务器共享的数据段被锁住，所以消息服务器已经停止运行。
        '(3151)-因为 VIO 调用出错，所以无法弹出显示消息。错误码就是相关数据。
        '(3152)-收到的服务器消息块 (SMB) 无效。SMB 就是相关数据。
        '(3160)- 工作站信息段大于 64K。大小如下(以 DWORD 值的格式):
        '(3161)-工作站无法取得计算机的名称号码。
        '(3162)-工作 站无法初始化 Async NetBIOS 线程。错误码就是相关数据。
        '(3163)-工作站无法打开最前面的共享段。错误码就是相关数据。
        '(3164)- 工作站主机表已满。
        '(3165)-收到的邮筒服务器消息块 (SMB) 有问题，SMB 就是相关数据。
        '(3166)-工作站启动用户帐 户数据库时出错。错误码就是相关数据。
        '(3167)-工作站响应 SSI 重新验证请求时出错。函数码及错误码就是相关数据。
        '(3174)- 服务器无法读取 AT 计划文件。
        '(3175)-服务器发现错误的 AT 计划记录。
        '(3176)-服务器找不到 AT 计划文件，所以创建一个计划文件。
        '(3185)-因为用户帐户数据库 (NET.ACC) 找不到或损坏，而且也没有可用的备份数据库，所以不能启动本地安全机制。系统不安全！

        '(3204)-服务器无法创建线程。CONFIG.SYS 中的 THREADS 参数必须加大。
        '(3213)-已经超过一个目录中文件的 副本复制的限制。
        '(3214)-已经超过副本复制的目录树深度限制。
        '(3215)-邮筒中收到的消息无法识别。
        '(3217)-无 法登录。用户当前已经登录，同时参数 TRYUSER设置为 NO。
        '(3230)-检测到服务器的电源中断。
        '(3231)-UPS 服务已经关掉服务器。
        '(3232)-UPS 服务没有完成执行用户指定的关机命令文件。
        '(3233)-无法打开 UPS 驱动程序。错误码就是相关数据。
        '(3234)-电源已经恢复。
        '(3235)-用户指定的关机命令文件有问题。
        '(3256)-该项 服务的动态链接库发生无法修复的错误。
        '(3257)-系统返回意外的错误码。错误码就是相关数据。
        '(3258)-容错错误日志文件 – LANROOT\LOGS\FT.LOG超过 64K。
        '(3259)-容错错误日志文件 – LANROOT\LOGS\FT.LOG，在被打开时就已设置更新进度位，这表示上次使用错误日志时，系统死机。
        '(3301)-Remote IPC
        '(3302)-Remote Admin
        '(3303)-Logon server share
        '(3304)-网络出错。
        '(3400)- 内存不足，无法启动工作站服务。
        '(3401)-读取 LAMAN.INI 文件的 NETWORKS 项目出错。
        '(3404)-LAMAN.INI 文件中的 NETWORKS 项目太多。
        '(3408)-程序无法用在此操作系统。
        '(3409)-已经安装转发程序。
        '(3411)- 安装 NETWKSTA.SYS 时出错。请按 ENTER 继续。
        '(3412)-求解程序链接问题。
        '(3419)-您已经打开文件或设 备，强制断开会造成数据丢失。
        '(3420)-内部用的默认共享
        '(3421)-信使服务
        '(3500)-命令成功完成。
        '(3501)- 使用的选项无效。
        '(3503)-命令包含无效的参数个数。
        '(3504)-命令运行完毕，但发生一个或多个错误。
        '(3505)-使 用的选项数值不正确。
        '(3510)-命令使用了冲突的选项。
        '(3512)-软件需要新版的操作系统。
        '(3513)-数据多于 Windows 2000 所能够返回的。
        '(3515)-此命令只能用在 Windows 2000 域控制器。
        '(3516)-这个指令 不能用于一个 Windows 2000 域控制器。
        '(3520)-已经启动以下 Windows 2000 服务:
        '(3525)-停止 工作站服务也会同时停止服务器服务 。
        '(3526)-工作站有打开的文件。
        '(3533)-服务正在启动或停止中，请稍候片刻后再试一次。
        '(3534)- 服务没有报告任何错误。
        '(3535)-正在控制设备时出错。
        '(3660)-这些工作站在这台服务器上有会话:
        '(3661)-这些 工作站有会话打开了此台服务器上的文件:
        '(3666)-消息别名已经转发出去。
        '(3670)-您有以下的远程连接:
        '(3671)- 继续运行会取消连接。
        '(3676)-会记录新的网络连接。
        '(3677)-不记录新的网络连接。
        '(3678)-保存配置文件时出 错，原先记录的网络连接状态没有更改。
        '(3679)-读取配置文件时出错。
        '(3682)-没有启动任何网络服务。
        '(3683)- 清单是空的。
        '(3689)-工作站服务已经在运行中，Windows 2000 会忽略工作站的命令选项。
        '(3694)-在打印作业正在 后台处理到队列时，无法删除共享的队列。
        '(3710)-打开帮助文件时出错。
        '(3711)-帮助文件是空的。
        '(3712)-帮助 文件已经损坏。
        '(3714)-这是专为那些安装旧版软件的系统提供的操作。
        '(3716)-设备类型未知。
        '(3717)-日志文件 已经损坏。
        '(3718)-程序文件名后必须以 .EXE 结束。
        '(3719)-找不到匹配的共享，因此没有删除。
        '(3720)- 用户记录中的 “单位/星期” 的值不正确。
        '(3725)-删除共享时出错。
        '(3726)-用户名无效。
        '(3727)-密码无 效。
        '(3728)-密码不匹配。
        '(3729)-永久连接没有完全还原。
        '(3730)-计算机名或域名错误。
        '(3732)- 无法设置该资源的默认权限。
        '(3734)-没有输入正确的密码。
        '(3735)-没有输入正确的名称。
        '(3736)-该资源无法共 享。
        '(3737)-权限字符串包含无效的权限。
        '(3738)-您只能在打印机或通讯设备上执行这项操作。
        '(3743)-服务器没 有设置远程管理的功能。
        '(3752)-这台服务器上没有用户的会话。
        '(3756)-响应无效。
        '(3757)-没有提供有效的响 应。
        '(3758)-提供的目标清单与打印机队列目标清单不匹配。
        '(3761)-指定的时间范围中结束的时间比开始的时间早。
        '(3764)- 提供的时间不是整点。
        '(3765)-12 与 24 小时格式不能混用。
        '(3767)-提供的日期格式无效。
        '(3768)-提供 的日期范围无效。
        '(3769)-提供的时间范围无效。
        '(3770)-NET USER 的参数无效。请检查最短的密码长度和/或提供参数。
        '(3771)-ENABLESCRIPT 的值必须是 YES。
        '(3773)-提供的 国家(地区)代码无效。
        '(3774)-用户已经创建成功，但是无法添加到USERS 本地组中。
        '(3775)-提供的用户上下文无效。
        '(3777)- 文件发送功能已不再支持。
        '(3778)-您可能没有指定 ADMIN$ 及 IPC$ 共享的路径。
        '(3784)-只有磁盘共享可以标记 为可以缓存。
        '(3802)-此计划日期无效。
        '(3803)-LANMAN 根目录无法使用。
        '(3804)-SCHED.LOG 文件无法打开。
        '(3805)-服务器服务尚未启动。
        '(3806)-AT 作业标识不存在。
        '(3807)-AT 计划文件已损坏。
        '(3808)- 因为 AT 计划文件发生问题，所以无法运行删除操作。
        '(3809)-命令行不得超过 259 个字符。
        '(3810)-因为磁盘已满，所 以 AT 计划文件无法更新。
        '(3812)-AT 计划文件无效。请删除此文件并创建新的文件。
        '(3813)-AT 计划文件已经删除。
        '(3814)- 此命令的语法是:AT [id] [/DELETE]AT 时间 [/EVERY:日期 | /NEXT:日期] 命令AT 命令会在以后的指定日期及时间，安排程序在服务器上运行。它也会显示安排运行的程序及命令的清单。您可以将日期指定为M、T、W、Th、F、Sa、Su 或 1-31 的格式。您可以将时间指定为HH:MM的二十四小时格式。
        '(3815)-AT 命令已经超时。请稍后再试一次。
        '(3816)- 用户帐户的密码使用最短期限不得大于密码最长使用期限。
        '(3817)-指定的数值与安装下层软件的服务器不兼容。请指定较小的值。
        '(3901)-****
        '(3902)-**** 意外到达消息的结尾 ****
        '(3905)-请按 ESC 退出
        '(3906)-…
        '(3912)-找不到时间服务器。
        '(3915)- 无法判断用户的主目录。
        '(3916)-没有指定用户的主目录。
        '(3920)-已经没有可用的驱动器号。
        '(3936)-这台计算机 目前没有配置成使用一个指定的 SNTP 服务器。
        '(3953)-语法错误。
        '(3960)-指定的文件号码无效。
        '(3961)- 指定的打印作业号码无效。
        '(3963)-指定的用户或组帐户找不到。
        '(3965)-已添加用户，但 NetWare 的文件和打印服务无法启用。
        '(3966)-没有安装 NetWare 的文件和打印服务。
        '(3967)-无法为 NetWare 的文件和打印服务设置用户属性。
        '(3969)-NetWare 兼容登录
        '(4000)-WINS 在处理命令时遇到错误。
        '(4001)- 本地的 WINS 不能删除。
        '(4002)-文件导入操作失败。
        '(4003)-备份操作失败。是否先前已作过完整备份?
        '(4004)- 备份操作失败。请检查您备份数据库的目录。
        '(4005)-WINS 数据库中没有这个名称。
        '(4006)-不允许复制一个尚未配置的伙 伴。
        '(4100)-DHCP 客户获得一个在网上已被使用的 IP 地址。 直到 DHCP 客户可以获得新的地址前，本地接口将被禁用。
        '(4200)- 无法识别传来的 GUID 是否为有效的 WMI 数据提供程序。
        '(4201)-无法识别传来的实例名是否为有效的 WMI 数据提供程序。
        '(4202)- 无法识别传来的数据项目标识符是否为有效的 WMI 数据提供程序。
        '(4203)-无法完成 WMI 请求，应该重试一次。
        '(4204)- 找不到 WMI 数据提供程序。
        '(4205)-WMI 数据提供程序引用到一个未注册的实例组。
        '(4206)-WMI 数据块或事件通知已启用。
        '(4207)-WMI 数据块不再可用。
        '(4208)-WMI 数据服务无法使用。
        '(4209)-WMI 数据提供程序无法完成要求。
        '(4210)-WMI MOF 信息无效。
        '(4211)-WMI 注册信息无效。
        '(4212)-WMI 数据块或事件通知已禁用。
        '(4213)-WMI 数据项目或数据块为只读。
        '(4214)-WMI 数据项目或数据块不能更改。
        '(4300)- 媒体标识符没有表示一个有效的媒体。
        '(4301)-库标识符没有表示一个有效的库。
        '(4302)-媒体缓冲池标识符没有表示一个有效的媒 体缓冲池。
        '(4303)-驱动器和媒体不兼容或位于不同的库中。
        '(4304)-媒体目前在脱机库中，您必须联机才能运行这个操作。
        '(4305)- 操作无法在脱机库中运行。
        '(4306)-库、驱动器或媒体缓冲池是空的。
        '(4307)-库、磁盘或媒体缓冲池必须是空的，才能运行这个操 作。
        '(4308)-在这个媒体缓冲池或库中目前没有可用的媒体。
        '(4309)-这个操作所需的资源已禁用。
        '(4310)-媒体标 识符没有表示一个有效的清洗器。
        '(4311)-无法清洗驱动器或不支持清洗。
        '(4312)-对象标识符没有表示一个有效的对象。
        '(4313)- 无法读取或写入数据库。
        '(4314)-数据库已满。
        '(4315)-媒体与设备或媒体缓冲池不兼容。
        '(4316)-这个操作所需的 资源不存在。
        '(4317)-操作标识符不正确。
        '(4318)-媒体未被安装，或未就绪。
        '(4319)-设备未就绪。
        '(4320)- 操作员或系统管理员拒绝了请求。
        '(4321)-驱动器标识符不代表一个有效的驱动器。
        '(4322)-程序库已满。没有可使用的插槽。
        '(4323)- 传输程序不能访问媒体。
        '(4324)-无法将媒体加载到驱动器中。
        '(4325)-无法检索有关驱动器的状态。
        '(4326)-无法 检索有关插槽的状态。
        '(4327)-无法检索传输的状态。
        '(4328)-因为传输已在使用中，所以无法使用。
        '(4329)-无法 打开或关闭弹入/弹出端口。
        '(4330)-因为媒体在驱动器中，无法将其弹出。
        '(4331)-清洗器插槽已被保留。
        '(4332)- 没有保留清洗器插槽。
        '(4333)-清洗器墨盒已进行了最大次数的驱动器清洗。
        '(4334)-意外媒体标识号。
        '(4335)-在 这个组或源中最后剩下的项目不能被删除。
        '(4336)-提供的消息超过了这个参数所允许的最大尺寸。
        '(4337)-该卷含有系统和页面文 件。
        '(4338)-由于库中至少有一个驱动器可以支持该媒体类型，不能从库中删除媒体类型。
        '(4339)-由于没有可以使用的已被启动的 驱动器，无法将该脱机媒体装入这个系统。
        '(4340)-(Y/N) [Y]
        '(4341)-(Y/N) [N]
        '(4342)-错误
        '(4343)-OK
        '(4344)-Y
        '(4345)-N
        '(4346)- 任何
        '(4347)-A
        '(4348)-P
        '(4349)-(找不到)
        '(4350)-远程存储服务无法撤回文件。
        '(4351)- 远程存储服务此时不可操作。
        '(4352)-远程存储服务遇到一个媒体错误。
        '(4354)-请键入密码:
        '(4358)-请键入用户 的密码:
        '(4359)-请键入共享资源的密码:
        '(4360)-请键入您的密码:
        '(4361)-请再键入一次密码以便确认:
        '(4362)- 请键入用户的旧密码:
        '(4363)-请键入用户的新密码:
        '(4364)-请键入您的新密码:
        '(4365)-请键入复制器服务密 码:
        '(4368)-请键入您的用户名:
        '(4372)-打印作业详细信息
        '(4378)-控制下列正在运行的服务:
        '(4379)- 统计数据可用于正在运行的下列服务:
        '(4381)-此命令的语法是:
        '(4382)-此命令的选项是:
        '(4383)-请键入主域控 制器的名称:
        '(4385)-Sunday
        '(4386)-Monday
        '(4387)-Tuesday
        '(4388)-Wednesday
        '(4389)-Thursday
        '(4390)- 此文件或目录不是一个重解析点。
        '(4391)-重解析点的属性不能被设置，因为它与已有的属性冲突。
        '(4392)-在重解析点缓冲区中的 数据无效。
        '(4393)-在重解析点缓冲区中的标签无效。
        '(4394)-请求中指定的标签
        '(4395)-W
        '(4396)-Th
        '(4397)-F
        '(4398)-S
        '(4399)-Sa
        '(4401)- 组名
        '(4402)-注释
        '(4403)-成员
        '(4406)-别名
        '(4407)-注释
        '(4408)-成员
        '(4411)- 用户名
        '(4412)-全名
        '(4413)-注释
        '(4414)-用户的注释
        '(4415)-参数
        '(4416)-国家 (地区)代码
        '(4417)-权限等级
        '(4418)-操作员权限
        '(4419)-帐户启用
        '(4420)-帐户到期
        '(4421)- 上次设置密码
        '(4422)-密码到期
        '(4423)-密码可更改
        '(4424)-允许的工作站
        '(4425)-磁盘空间上限
        '(4426)- 无限制
        '(4427)-本地组会员
        '(4428)-域控制器
        '(4429)-登录脚本
        '(4430)-上次登录
        '(4431)- 全局组成员
        '(4432)-可允许的登录小时数
        '(4433)-全部
        '(4434)-无
        '(4436)-主目录
        '(4437)- 需要密码
        '(4438)-用户可以更改密码
        '(4439)-用户配置文件
        '(4440)-已锁定
        '(4450)-计算机名
        '(4451)- 用户名
        '(4452)-软件版本
        '(4453)-工作站活动在
        '(4454)-Windows NT 根目录
        '(4455)-工 作站域
        '(4456)-登录域
        '(4457)-其它域
        '(4458)-COM 打开超时 (秒)
        '(4459)-COM 发送计数 (字节)
        '(4460)-COM 发送超时 (毫秒)
        '(4461)-DOS 会话打印超时 (秒)
        '(4462)-错误日 志文件大小上限 (K)
        '(4463)-高速缓存上限 (K)
        '(4464)-网络缓冲区数
        '(4465)-字符缓冲区数
        '(4466)- 域缓冲区大小
        '(4467)-字符缓冲区大小
        '(4468)-计算机全名
        '(4469)-工作站域 DNS 名称
        '(4470)-Windows 2000
        '(4481)-服务器名称
        '(4482)-服务器注释
        '(4483)-发送管理警报到
        '(4484)-软件版本
        '(4485)- 对等服务器
        '(4486)-Windows NT
        '(4487)-服务器等级
        '(4488)-Windows NT Server
        '(4489)- 服务器正运行于
        '(4492)-服务器已隐藏
        '(4500)-零备份存储在这个卷上不可用。
        '(4506)-登录的用户数量上限
        '(4507)- 同时可并存的管理员数量上限
        '(4508)-资源共享数量上限
        '(4509)-资源连接数量上限
        '(4510)-服务器打开的文件数量 上限
        '(4511)-每个会话打开的文件数量上限
        '(4512)-文件锁定数量上限
        '(4520)-空闲的会话时间 (分)
        '(4526)- 共享等级
        '(4527)-用户等级
        '(4530)-未限制的服务器
        '(4570)-强制用户在时间到期之后多久必须注销?:
        '(4571)- 多少次密码不正确后锁住帐户?:
        '(4572)-密码最短使用期限 (天):
        '(4573)-密码最长使用期限 (天):
        '(4574)- 密码长度下限:
        '(4575)-保持的密码历史记录长度:
        '(4576)-计算机角色:
        '(4577)-工作站域的主域控制器:
        '(4578)- 锁定阈值:
        '(4579)-锁定持续时间(分):
        '(4580)-锁定观测窗口(分):
        '(4600)-统计开始于
        '(4601)- 接受的会话
        '(4602)-会话超时
        '(4603)-会话出错
        '(4604)-发送的 KB
        '(4605)-接收的 KB
        '(4606)- 平均响应时间 (毫秒)
        '(4607)-网络错误
        '(4608)-访问的文件
        '(4609)-后台处理的打印作业
        '(4610)- 系统出错
        '(4611)-密码违规
        '(4612)-权限违规
        '(4613)-访问的通讯设备
        '(4614)-会话已启动
        '(4615)- 重新连接的会话
        '(4616)-会话启动失败
        '(4617)-断开的会话
        '(4618)-网络 I/O 执行
        '(4619)-文 件及管道被访问
        '(4620)-时间缓冲区耗尽
        '(4621)-大缓冲区
        '(4622)-请求缓冲区
        '(4626)-已做连接
        '(4627)- 连接失败
        '(4630)-接收的字节数
        '(4631)-接收的服务器消息块 (SMB)
        '(4632)-传输的字节数
        '(4633)- 传输的服务器消息块 (SMB)
        '(4634)-读取操作
        '(4635)-写入操作
        '(4636)-拒绝原始读取
        '(4637)- 拒绝原始写入
        '(4638)-网络错误
        '(4639)-已做连接
        '(4640)-重新连接
        '(4641)-服务器断开
        '(4642)- 会话已启动
        '(4643)-会话挂起
        '(4644)-失败的会话
        '(4645)-操作失败
        '(4646)-使用计数
        '(4647)- 使用计数失败
        '(4655)-消息名称转发已经取消。
        '(4661)-密码已经更改成功。
        '(4664)-消息已经发给网络上所有的用 户。
        '(4666)-消息已经送到此服务器上的所有用户。
        '(4696)-Windows NT Server
        '(4697)-Windows NT Workstation
        '(4698)-MS-DOS 增强型工作站
        '(4700)-服务器名称 注释
        '(4701)-资源共 享名 类型 用途 注释
        '(4702)-(UNC)
        '(4703)-…
        '(4704)-Domain
        '(4706)-其它可用的 网络:
        '(4710)-Disk
        '(4711)-Print
        '(4712)-Comm
        '(4713)-IPC
        '(4714)- 状态 本地 远程 网络
        '(4715)-OK
        '(4716)-休止
        '(4717)-已暂停
        '(4718)-断开
        '(4719)- 错误
        '(4720)-正在连接
        '(4721)-正在重新连接
        '(4722)-状态
        '(4723)-本地名称
        '(4724)- 远程名称
        '(4725)-资源类型
        '(4726)-# 打开
        '(4727)-# 连接
        '(4728)-不可用
        '(4730)- 共享名 资源 注释
        '(4731)-共享名
        '(4732)-资源
        '(4733)-后台处理
        '(4734)-权限
        '(4735)- 最多用户
        '(4736)-无限制
        '(4737)-用户
        '(4740)-识别码 路径 用户名 # 锁定
        '(4741)-文件识别 码
        '(4742)-锁定
        '(4743)-权限
        '(4750)-计算机 用户名 客户类型 打开空闲时间
        '(4751)-计算机
        '(4752)- 会话时间
        '(4753)-空闲时间
        '(4754)-资源共享名 类型 # 打开
        '(4755)-客户类型
        '(4756)-来宾登 录
        '(4770)-脱机缓存被启用:手动恢复
        '(4771)-脱机缓存被启用:自动恢复
        '(4772)-脱机缓存被启用:用户之间没有 共享
        '(4773)-脱机缓存被停用
        '(4774)-自动
        '(4775)-手动
        '(4800)-名称
        '(4801)-转发 到
        '(4802)-已经从下列位置转发给您
        '(4803)-这台服务器的用户
        '(4804)-用户已经按 Ctrl+Break 中断网络发送。
        '(4810)-名称 作业编号 大小 状态
        '(4811)-作业
        '(4812)-打印
        '(4813)-名称
        '(4814)- 作业 #
        '(4815)-大小
        '(4816)-状态
        '(4817)-分隔文件
        '(4818)-注释
        '(4819)-优先级
        '(4820)- 打印后于
        '(4821)-打印直到
        '(4822)-打印处理程序
        '(4823)-附加信息
        '(4824)-参数
        '(4825)- 打印设备
        '(4826)-打印机活动中
        '(4827)-打印机搁置
        '(4828)-打印机出错
        '(4829)-正在删除打印机
        '(4830)- 打印机状态未知
        '(4841)-作业 #
        '(4842)-正在提交用户
        '(4843)-通知
        '(4844)-作业数据类型
        '(4845)- 作业参数
        '(4846)-正在等候
        '(4847)-搁置于队列
        '(4848)-正在后台处理
        '(4849)-已暂停
        '(4850)- 脱机
        '(4851)-错误
        '(4852)-缺纸
        '(4853)-需要干预
        '(4854)-正在打印
        '(4855)-on
        '(4862)- 驱动程序
        '(4930)-用户名 类型 日期
        '(4931)-锁定
        '(4932)-服务
        '(4933)-服务器
        '(4934)- 服务器已启动
        '(4935)-服务器已暂停
        '(4936)-服务器已继续操作
        '(4937)-服务器已停止
        '(4938)-会话
        '(4939)- 登录来宾
        '(4940)-登录用户
        '(4941)-登录管理员
        '(4942)-正常注销
        '(4943)-登录
        '(4944)- 注销错误
        '(4945)-注销自动断开
        '(4946)-注销管理员断开
        '(4947)-注销受登录限制
        '(4948)-服务
        '(4957)- 帐户
        '(4964)-已修改帐户系统设置
        '(4965)-登录限制
        '(4966)-超过限制: 未知
        '(4967)-超过限制: 登录时间
        '(4968)-超过限制: 帐户过期
        '(4969)-超过限制: 工作站识别码无效
        '(4970)-超过限制: 帐户停用
        '(4971)- 超过限制: 帐户已删除
        '(4972)-资源
        '(4978)-密码不正确
        '(4979)-需要管理员特权
        '(4980)-访问
        '(4984)- 拒绝访问
        '(4985)-未知
        '(4986)-其它
        '(4987)-持续时间:
        '(4988)-持续时间: 无效
        '(4989)- 持续时间: 1 秒以下
        '(4990)-(无)
        '(4994)-访问结束
        '(4995)-登录到网络
        '(4996)-拒绝登录
        '(4997)- 程序 消息 时间
        '(4999)-管理员已解除帐户的锁定状态
        '(5000)-注销网络
        '(5001)-因为其它资源需要它，不能将群 集资源移到另一个组。
        '(5002)-找不到此群集资源的依存。
        '(5003)-因为已经处于依存状态，此群集资源不能依存于指定的资源。
        '(5004)- 此群集资源未联机。
        '(5005)-此操作没有可用的群集节点。
        '(5006)-没有群集资源。
        '(5007)-找不到群集资源。
        '(5008)- 正在关闭群集。
        '(5009)-因为联机，群集节点无法从群集中脱离。
        '(5010)-对象已存在。
        '(5011)-此对象已在列表 中。
        '(5012)-新请求没有可用的群集组。
        '(5013)-找不到群集组。
        '(5014)-因为群集组未联机，此操作不能完成。
        '(5015)- 群集节点不是此资源的所有者。
        '(5016)-群集节点不是此资源的所有者。
        '(5017)-群集资源不能在指定的资源监视器中创建。
        '(5018)- 群集资源不能通过资源监视器来联机。
        '(5019)-因为群集资源联机，此操作不能完成。
        '(5020)-由于是仲裁资源，群集资源不能被删 除或脱机。
        '(5021)-由于没有能力成为仲裁资源，此群集不能使指定资源成为仲裁资源。
        '(5022)-群集软件正关闭。
        '(5023)- 组或资源的状态不是执行请求操作的正确状态。
        '(5024)-属性已被存储，但在下次资源联机前，不是所有的修改将生效。
        '(5025)-由 于不属于共享存储类别，群集不能使指定资源成为仲裁资源。
        '(5026)-由于是内核资源，无法删除群集资源。
        '(5027)-仲裁资源联机 失败。
        '(5028)-无法成功创建或装入仲裁日志。
        '(5029)-群集日志损坏。
        '(5030)-由于该日志已超出最大限量，无法 将记录写入群集日志。
        '(5031)-群集日志已超出最大限量。
        '(5032)-群集日志没有发现检查点记录。
        '(5033)-不满足 日志所需的最小磁盘空间。
        '(5034)-群集节点未能控制仲裁资源，因为它被另一个活动节点拥有。
        '(5035)-这个操作的群集网络无 效。
        '(5036)-此操作没有可用的群集节点。
        '(5037)-所有群集节点都必须运行才能执行这个操作。
        '(5038)-群集资源 失败。
        '(5039)-该群集节点无效。
        '(5040)-该群集节点已经存在。
        '(5041)-一个节点正在加入该群集。
        '(5042)- 找不到群集节点。
        '(5043)-找不到群集本地节点信息。
        '(5044)-群集网络已经存在。
        '(5045)-找不到群集网络。
        '(5046)- 群集网络界面已经存在。
        '(5047)-找不到群集网络界面。
        '(5048)-群集请求在这个对象中无效。
        '(5049)-群集网络提 供程序无效。
        '(5050)-群集节点坏了。
        '(5051)-无法连接到群集节点。
        '(5052)-该群集节点不是群集的一个成员。
        '(5053)- 群集加入操作正在进行中。
        '(5054)-该群集网络无效。
        '(5055)-Mar
        '(5056)-该群集节点可以使用。
        '(5057)- 该群集 IP 地址已在使用中。
        '(5058)-该群集节点没有中止。
        '(5059)-没有有效的群集安全上下文。
        '(5060)-该 群集网络不是为内部群集通讯配置的。
        '(5061)-群集节点已经开始。
        '(5062)-群集节点已经坏了。
        '(5063)-群集网络 已经联机。
        '(5064)-群集网络已经脱机。
        '(5065)-群集节点已经是该群集的成员。
        '(5066)-该群集网络是唯一个为两 个或更多活动群集节点进行内部群集通讯的配置。不能从网络上删除内部通讯能力。
        '(5067)-一个或更多的群集资源依靠网络来向客户提供服务。不 能从网络上删除客户访问能力。
        '(5068)-该操作不能在群集资源上作为仲裁资源执行。您不能将仲裁资源脱机或修改它的所有者名单。
        '(5069)- 该群集仲裁资源不允许有任何依存关系。
        '(5070)-该群集节点暂停。
        '(5071)-群集资源不能联机。所有者节点不能在这个资源上运 行。
        '(5072)-群集节点没有准备好，不能执行所请求的操作。
        '(5073)-群集节点正在关闭。
        '(5074)-放弃群集节点加 入操作。
        '(5075)-由于加入节点和支持者之间的软件版本不兼容，该群集加入操作失败。
        '(5076)-由于该群集已经到达其所能监督的 资源限制，不能创建这个资源。
        '(5077)-系统配置在群集加入或形成操作时已更改。放弃加入或形成操作。
        '(5078)-找不到指定的资 源种类。
        '(5079)-指定的节点不支持这种资源，这也许是由于版本不一致或是由于在这个节点上没有资源 DLL。
        '(5080)-该资源 DLL 不支持指定的资源名称。这可能是由于一个提供给源 DLL 名称是错误的(或经过更改的)。
        '(5081)-不能在 RPC 服务器上注册任何身份验证包。
        '(5082)-由于组的所有者不在组的首选列表中，不能将组联机。要改变组的所有者节点，请移动组。
        '(5083)- 群集数据库的系列号已改变，或者与锁定程序节点不相容，因此加入操作没有成功。如果在加入操作期间群集数据库有任何改动，这都可能发生。
        '(5084)- 资源在其当前状态下，资源监视器不允许执行失败操作。资源处于挂起状态时，这都可能发生。
        '(5085)-非锁定程序代码收到一个为全局更新保留锁 定的请求。
        '(5086)-群集服务找不到仲裁磁盘。
        '(5087)-已备份的群集数据库可能已损坏。
        '(5088)-DFS 根目录已在这个群集节点中。
        '(5089)-由于与另一个现有属性冲突，未能修改资源属性。
        '(5090)-西班牙
        '(5091)-丹 麦
        '(5092)-瑞典
        '(5093)-挪威
        '(5094)-德国
        '(5095)-澳大利亚
        '(5096)-日本
        '(5097)- 韩国
        '(5098)-中国
        '(5099)-台湾
        '(5100)-亚洲
        '(5101)-葡萄牙
        '(5102)-芬兰
        '(5103)- 阿拉伯
        '(5104)-希伯莱
        '(5153)-UPS 服务即将执行最后的关机操作。
        '(5170)-工作站必须用 NET START 才能启动。
        '(5175)-远程 IPC
        '(5176)-远程管理
        '(5177)-默认共享
        '(5291)-永不
        '(5292)- 永不
        '(5293)-永不
        '(5295)-NETUS.HLP
        '(5296)-NET.HLP
        '(5300)-网络控制块 (NCB) 请求运行成功。NCB 是相关数据。
        '(5301)-SEND DATAGRAM、SEND BROADCAST、ADAPTER STATUS 或 SESSION STATUS 的网络控制块 (NCB) 缓冲区长度无效。NCB 是相关数据。
        '(5302)-网络控制块 (NCB) 指定的数据描述数组无效。NCB 是相关数据。
        '(5303)-网络控制块 (NCB) 指定的命令无效。NCB 是相关数据。
        '(5304)- 网络控制块 (NCB) 指定的消息交换码无效。NCB 是相关数据。
        '(5305)-网络控制块 (NCB) 命令超时。会话可能异常终止。NCB 是相关数据。
        '(5306)-接收的网络控制块 (NCB) 消息不完整。NCB 是相关数据。
        '(5307)- 网络控制块 (NCB) 指定的缓冲区无效。NCB 是相关数据。
        '(5308)-网络控制块 (NCB) 指定的会话号码没有作用。NCB 是相关数据。
        '(5309)-网卡没有任何资源可用。网络控制块 (NCB) 请求被拒绝。NCB 是相关数据。
        '(5310)-网络控制块 (NCB) 指定的会话已经关闭。NCB 是相关数据。
        '(5311)-网络控制块 (NCB) 命令已经取消。NCB 是相关数据。
        '(5312)- 网络控制块 (NCB) 指定的消息块不合逻辑。NCB 是相关数据。
        '(5313)-该名称已经存在于本地适配器名称表中。网络控制块 (NCB) 请求被拒绝。NCB 是相关数据。
        '(5314)-网卡名称表已满。网络控制块 (NCB) 请求被拒绝。NCB 是相关数据。
        '(5315)- 网络名称已经有活动的会话，现在取消注册。网络控制块 (NCB) 命令运行完毕。NCB 是相关数据。
        '(5316)-先前发出的 Receive Lookahead 命令对此会话仍起作用。网络控制块 (NCB) 命令被拒绝。NCB 是相关数据。
        '(5317)-本地会话 表已满。网络控制块 (NCB) 请求被拒绝。NCB 是相关数据。
        '(5318)-拒绝打开网络控制块 (NCB) 会话，远程计算机上没有侦听命令在执行。NCB 是相关数据。
        '(5319)-网络控制块 (NCB) 指定的名称号码无效。NCB 是相关数据。
        '(5320)- 网络控制块 (NCB) 中指定的调用名称找不到，或者没有应答。NCB 是相关数据。
        '(5321)-网络控制块 (NCB) 中指定的名称找不到。无法将“*”或00h 填入 NCB 名称。NCB 是相关数据。
        '(5322)-网络控制块 (NCB) 中指定的名称正用于远程适配器。NCB 是相关数据。
        '(5323)-网络控制块 (NCB) 中指定的名称已经删除。NCB 是相关数据。
        '(5324)- 网络控制块 (NCB) 中指定的会话异常终止。NCB 是相关数据。
        '(5325)-网络协议在网络上检测两个或数个相同的名称。 网络控制块 (NCB) 是相关数据。
        '(5326)-收到意外的协议数据包。远程设备可能不兼容。网络控制块 (NCB) 是相关数据。
        '(5333)-NetBIOS 界面正忙。网络控制块 (NCB) 请求被拒绝。NCB 是相关数据。
        '(5334)-未完成的网络控制块 (NCB) 命令太多。NCB 请求被拒绝。NCB 是相关数据。
        '(5335)-网络控制块 (NCB) 中指定的适配器号无效。NCB 是相关数据。
        '(5336)-网 络控制块 (NCB) 命令在取消的同时运行完毕。NCB 是相关数据。
        '(5337)-网络控制块 (NCB) 指定的名称已经保留。NCB 是相关数据。
        '(5338)-网络控制块 (NCB) 命令无法取消。NCB 是相关数据。
        '(5351)-同一个会话有多个网络控制块 (NCB)。NCB 请求被拒绝。NCB 是相关数据。
        '(5352)-网卡出错。唯一可能发出的 NetBIOS 命令是 NCB RESET。网络控制块 (NCB) 是相关数据。
        '(5354)-超过应用程序数目上限。网络控制区 (NCB) 请求被拒绝，NCB 是相关数据。
        '(5356)-请求的资源无法使用。网络控制块 (NCB) 请求被拒绝。NCB 是相关数据。
        '(5364)-系统出错。网 络控制块 (NCB) 请求被拒绝。NCB 即为数据。
        '(5365)-“ROM 校验和”失败。网络控制块 (NCB) 请求被拒绝。NCB 是相关数据。
        '(5366)-RAM 测试失败。网络控制块 (NCB) 请求被拒绝。NCB 是相关数据。
        '(5367)-数字式环回失 败。网络控制块 (NCB) 请求被拒绝。NCB 是相关数据。
        '(5368)-模拟式环回失败。网络控制块 (NCB) 请求被拒绝。NCB 是相关数据。
        '(5369)-界面失败。网络控制块 (NCB) 请求被拒绝。NCB 是相关数据。
        '(5370)-收到的网络控制块 (NCB) 返回码无法识别。NCB 是相关数据。
        '(5380)-网卡故障。网络控制块 (NCB) 请求被拒绝。NCB 是相关数据。
        '(5381)- 网络控制块 (NCB) 命令仍然处于搁置状态。NCB 是相关数据。
        '(5509)-Windows 2000 无法按指定的配置启动，将换用先前可工作的配置。
        '(5600)-无法共享用户或脚本路径。
        '(5601)-计算机的密码在本地安全数据库中 找不到。
        '(5602)-访问计算机的本地或网络安全数据库时，发生内部错误。
        '(5705)-Netlogon 服务用于记录数据库更改数据的日志高速缓存已损坏。Netlogon 服务正在复位更改日志文件。
        '(5728)-无法加载任何传输。
        '(5739)- 此域的全局组数目超过可以复制到 LanMan BDC 的限制。请删除部分的全局组或删除域中的 LanManBDC。
        '(5742)-服务无法 检索必要的消息，所以无法运行远程启动客户。
        '(5743)-服务发生严重的错误，无法从远程启动3Com 3Start 远程启动客户。
        '(5744)- 服务发生严重的系统错误，即将关机。
        '(5760)-服务在分析 RPL 配置时出错。
        '(5761)-服务在创建所有配置的 RPL 配置文件时出错。
        '(5762)-服务在访问注册表时出错。
        '(5763)-服务在替换可能过期的 RPLDISK.SYS 时出错。
        '(5764)- 服务在添加安全帐户或设置文件权限时出错。这些帐户是独立 RPL 工作站的 RPLUSER 本地组以及用户帐户。.
        '(5765)-服务无法备 份它的数据库。
        '(5766)-服务无法从它的数据库初始化。数据库可能找不到或损坏。服务会试图从备份数据库恢复该数据库。
        '(5767)- 服务无法从备份数据库还原它的数据库。服务将不启动。
        '(5768)-服务无法从备份数据库还原它的数据库。
        '(5769)-服务无法从它还 原的数据库初始化。服务将不启动。
        '(5771)-远程启动数据库采用 NT 3.5 / NT 3.51 格式。NT 正在转换其为 NT 4.0 格式。完成转换后，JETCONV 转换器将写出应用事件日志。
        '(5773)-该 DC 的 DNS 服务器不支持动态 DNS。将文件 $SystemRootSystem32Config etlogon.dns$中的 DNS 记录添加到伺服那个文件中引用的域的 DNS 服务器。
        '(5781)- 由于没有可以使用的 DNS 服务器，一个或更多 DNS 记录的动态注册和注销未成功。
        '(6000)-无法加密指定的文件。
        '(6001)- 指定的文件无法解密。
        '(6002)-指定的文件已加密，而且用户没有能力解密。
        '(6003)-这个系统没有有效的加密恢复策略配置。
        '(6004)- 所需的加密驱动程序并未加载到系统中。
        '(6005)-文件加密所使用的加密驱动程序与目前加载的加密驱动程序不同。
        '(6006)-没有为 用户定义的 EFS 关键字。
        '(6007)-指定的文件并未加密。
        '(6008)-指定的文件不是定义的 EFS 导出格式。
        '(6009)- 指定的文件是只读文件。
        '(6010)-已为加密而停用目录。
        '(6011)-不信任服务器来进行远程加密操作。
        '(6118)-此工 作组的服务器列表当前无法使用
        '(6200)-要正常运行，任务计划程序服务的配置必须在系统帐户中运行。单独的任务可以被配置成在其他帐户中运 行。
        '(7001)-指定的会话名称无效。
        '(7002)-指定的协议驱动程序无效。
        '(7003)-在系统路径上找不到指定的协议驱 动程序。
        '(7004)-在系统路径上找不到指定的终端连接。
        '(7005)-不能为这个会话创建一个事件日志的注册键。
        '(7006)- 同名的一个服务已经在系统中存在。
        '(7007)-在会话上一个关闭操作挂起。
        '(7008)-没有可用的输出缓冲器。
        '(7009)- 找不到 MODEM.INF 文件。
        '(7010)-在 MODEM.INF 中没有找到调制解调器名称。
        '(7011)-调制解调器没有接 受发送给它的指令。验证配置的调制解调器与连接的调制解调器是否匹配。
        '(7012)-调制解调器没有接受发送给它的指令。验证该调制解调器是否接 线正确并且打开了电源开关。
        '(7013)-运载工具检测失败或者由于断开连接，运载工具已被丢弃。
        '(7014)-在要求的时间内没有发现 拨号音。 确定电话线连接正确并可使用。
        '(7015)-在远程站点回叫时检测到了占线信号。
        '(7016)-在回叫时远程站点上检测到了声 音。
        '(7017)-传输驱动程序错误
        '(7022)-找不到指定的会话。
        '(7023)-指定的会话名称已处于使用中。
        '(7024)- 由于终端连接目前正在忙于处理一个连接、断开连接、复位或删除操作，无法完成该请求的操作。
        '(7025)-试图连接到其视频模式不受当前客户支持 的会话。
        '(7035)-应用程序尝试启动 DOS 图形模式。不支持 DOS 图形模式。
        '(7037)-您的交互式登录权限已被禁用。请 与您的管理员联系。
        '(7038)-该请求的操作只能在系统控制台上执行。这通常是一个驱动程序或系统 DLL 要求直接控制台访问的结果。
        '(7040)- 客户未能对服务器连接消息作出响应。
        '(7041)-不支持断开控制台会话。
        '(7042)-不支持重新将一个断开的会话连接到控制台。
        '(7044)- 远程控制另一个会话的请求被拒绝。
        '(7045)-拒绝请求的会话访问。
        '(7049)-指定的终端连接驱动程序无效。
        '(7050)- 不能远程控制该请求的会话。这也许是由于该会话被中断或目前没有一个用户登录。而且，您不能从该系统控制台远程控制一个会话或远程控制系统控制台。并且， 您不能远程控制您自己的当前会话。
        '(7051)-该请求的会话没有配置成允许远程控制。
        '(7052)-拒绝连接到这个终端服务器。终端服 务器客户许可证目前正在被另一个用户使用。请与系统管理员联系，获取一份新的终端服务器客户，其许可证号码必须是有效的、唯一的。
        '(7053)- 拒绝连接到这个终端服务器。还没有为这份终端服务器客户输入您的终端服务器客户许可证号码。请与系统管理员联系，为该终端服务器客户输入一个有效的、唯一 的许可证号码。
        '(7054)-系统已达到其授权的登录限制。请以后再试一次。
        '(7055)-您正在使用的客户没有使用该系统的授权。您的 登录请求被拒绝。
        '(7056)-系统许可证已过期。您的登录请求被拒绝。
        '(8001)-文件复制服务 API 被错误调用。
        '(8002)- 无法启动文件复制服务。
        '(8003)-无法停止文件复制服务。
        '(8004)-文件复制服务 API 终止了请求。事件日志可能有详细信息。
        '(8005)-该文件复制服务中断了该请求。事件日志可能有详细信息。
        '(8006)-无法联系文件 复制服务。事件日志可能有详细信息。
        '(8007)-由于该用户没有足够特权，文件复制服务不能满足该请求。事件日志可能有详细信息。
        '(8008)- 由于验证的 RPC 无效，文件复制服务不能满足该请求。事件日志可能有详细信息。
        '(8009)-由于该用户在域控制器上没有足够特权，文件复制 服务不能满足该请求。事件日志可能有详细信息。
        '(8010)-由于在域控制器上的验证的 RPC 无效，文件复制服务不能满足该请求。事件日志可能有详细信息。
        '(8011)-该文件复制服务无法与在域控制器上的文件复制服务通讯。事件日志可能 有详细信息。
        '(8012)-在域控制器上的文件复制服务无法与这台计算机上的文件复制服务通讯。事件日志可能有详细信息。
        '(8013)- 由于内部错误，该文件复制服务不能进入该系统卷中。事件日志可能有详细信息。
        '(8014)-由于内部超时，该文件复制服务不能进入该系统卷中。事 件日志可能有详细信息。
        '(8015)-该文件复制服务无法处理此请求。该系统卷仍在忙于前一个请求。
        '(8016)-由于内部错误，该文件 复制服务无法停止复制该系统卷。事件日志可能有详细信息。
        '(8017)-该文件复制服务检测到一个无效参数。
        '(8200)-在安装目录服 务时出现一个错误。有关详细信息，请查看事件日志。
        '(8201)-目录服务在本地评估组成员身份。
        '(8202)-指定的目录服务属性或值 不存在。
        '(8203)-指定给目录服务的属性语法无效。
        '(8204)-指定给目录服务的属性类型未定义。
        '(8205)-指定的目 录服务属性或值已经存在。
        '(8206)-目录服务忙。
        '(8207)-该目录服务无效。
        '(8208)-目录服务无法分配相对标识 号。
        '(8209)-目录服务已经用完了相对标识号池。
        '(8210)-由于目录服务不是该类操作的主控，未能执行操作。
        '(8211)- 目录服务无法初始化分配相对标识号的子系统。
        '(8212)-该请求的操作没有满足一个或多个与该对象的类别相关的约束。
        '(8213)-目 录服务只可以在一个页状对象上运行要求的操作。
        '(8214)-目录服务不能在一个对象的 RDN 属性上执行该请求的操作。
        '(8215)- 目录服务检测出修改对象类别的尝试。
        '(8216)-不能执行请求的通过域的移动操作。
        '(8217)-无法联系全局编录服务器。
        '(8218)- 策略对象是共享的并只可在根目录上修改。
        '(8219)-策略对象不存在。
        '(8220)-请求的策略信息只在目录服务中。
        '(8221)- 域控制器升级目前正在使用中。
        '(8222)-域控制器升级目前不在使用中
        '(8224)-出现一个操作错误。
        '(8225)-出现一 个协议错误。
        '(8226)-已经超过这个请求的时间限制。
        '(8227)-已经超过这个请求的大小限制。
        '(8228)-已经超过这 个请求的管理限制。
        '(8229)-比较的响应为假。
        '(8230)-比较的响应为真。
        '(8231)-这个服务器不支持请求的身份验 证方式。
        '(8232)-这台服务器需要一个更安全的身份验证方式。
        '(8233)-不适当的身份验证。
        '(8234)-未知的身份验 证机制。
        '(8235)-从服务器返回了一个建议。
        '(8236)-该服务器不支持该请求的关键扩展。
        '(8237)-这个请求需要一 个安全的连接。
        '(8238)-不恰当的匹配。
        '(8239)-出现一个约束冲突。
        '(8240)-在服务器上没有这样一个对象。
        '(8241)- 有一个别名问题。
        '(8242)-指定了一个无效的 dn 语法。
        '(8243)-该对象为叶对象。
        '(8244)-有一个别名废弃问 题。
        '(8245)-该服务器不愿意处理该请求。
        '(8246)-检查到一个循环。
        '(8247)-有一个命名冲突。
        '(8248)- 结果设置太大。
        '(8249)-该操作会影响到多个 DSA。
        '(8250)-该服务器不可操作。
        '(8251)-出现一个本地错误。
        '(8252)- 出现一个编码错误。
        '(8253)-出现一个解码错误。
        '(8254)-无法识别寻找筛选器。
        '(8255)-一个或多个参数非法。
        '(8256)- 不支持指定的方式。
        '(8257)-没有返回结果。
        '(8258)-该服务器不支持该指定的控制。
        '(8259)-客户检测到一个参考 循环。
        '(8260)-超过当前的参考限制。
        '(8301)-根目录对象必须是一个命名上下文的头。该根目录对象不能有实例父类。
        '(8302)- 不能执行添加副本操作。名称上下文必须可写才能创建副本。
        '(8303)-出现一个对架构中未定义的一个属性的参考。
        '(8304)-超过了 一个对象的最大尺寸。
        '(8305)-尝试向目录中添加一个已在使用中的名称的对象。
        '(8306)-尝试添加一个对象，该对象属于那类在架 构中没有一个 RDN 定义的类别。
        '(8307)-尝试添加一个使用 RDN 的对象，但该 RDN 不是一个在架构中定义的 RDN 。
        '(8308)- 在对象中找不到任何请求的属性。
        '(8309)-用户缓冲区太小。
        '(8310)-在操作中指定的属性不出现在对象上。
        '(8311)- 修改操作非法。不允许该修改的某个方面。
        '(8312)-指定的对象太大。
        '(8313)-指定的实例类别无效。
        '(8314)-操作 必须在主控 DSA 执行。
        '(8315)-必须指定对象类别属性。
        '(8316)-一个所需的属性丢失。
        '(8317)-尝试修改一 个对象，将一个对该类别来讲是非法的属性包括进来。
        '(8318)-在对象上指定的属性已经存在。
        '(8320)-指定的属性不存在或没有 值。
        '(8321)-为只有一个值的属性指定了多个值。
        '(8322)-属性值不在接受范围内。
        '(8323)-指定的值已存在。
        '(8324)- 由于不存在于对象上，不能删除该属性。
        '(8325)-由于不存在于对象上，不能删除该属性值。
        '(8326)-指定的根对象不能是子参考。
        '(8327)- 不允许链接。
        '(8328)-不允许链接的评估。
        '(8329)-由于对象的父类不是未实例化就是被删除了，所以不能执行操作。
        '(8330)- 不允许有一个用别名的父类。别名是叶对象。
        '(8331)-对象和父类必须是同一种类，不是都是原件就是都是副本。
        '(8332)-由于子对 象存在，操作不能执行。这个操作只能在叶对象上执行。
        '(8333)-没有找到目录对象。
        '(8334)-别名对象丢失。
        '(8335)- 对象名语法不对。
        '(8336)-不允许一个别名参考另一个别名。
        '(8337)-别名不能解除参考。
        '(8338)-操作超出范围。
        '(8340)- 不能删除 DSA 对象。
        '(8341)-出现一个目录服务错误。
        '(8342)-操作只能在内部主控 DSA 对象上执行。
        '(8343)- 对象必须为 DSA 类别。
        '(8344)-访问权不够不能执行该操作。
        '(8345)-由于父类不在可能的上级列表上，不能添加该对象。
        '(8346)- 由于该属性处于“安全帐户管理器” (SAM)，不允许访问该属性。
        '(8347)-名称有太多部分。
        '(8348)-名称太长。
        '(8349)- 名称值太长。
        '(8350)-目录服务遇到了一个错误分列名称。
        '(8351)-目录服务找不到一个名称的属性种类。
        '(8352)- 该名称不能识别一个对象; 该名称识别一个幻象。
        '(8353)-安全描述符太短。
        '(8354)-安全描述符无效。
        '(8355)- 为删除的对象创建名称失败。
        '(8356)-一个新子参考的父类必须存在。
        '(8357)-该对象必须是一个命名上下文。
        '(8358)- 不允许添加一个不属于系统的属性。
        '(8359)-对象的类别必须是有结构的; 您不能实例化一个抽象的类别。
        '(8360)-找不到架构的 对象。
        '(8361)-有这个 GUID (非活动的的或活动的)的本地对象已经存在。
        '(8362)-操作不能在一个后部链接上执行。
        '(8363)- 找不到指定的命名上下文的互交参考。
        '(8364)-由于目录服务关闭，操作不能执行。
        '(8365)-目录服务请求无效。
        '(8366)- 无法读?巧姓呤粜浴?
        '(8367)-请求的 FSMO 操作失败。不能连接当前的 FSMO 盒。
        '(8368)-不允许跨过一个命名 上下文修改 DN。
        '(8369)-由于属于系统，不能修改该属性。
        '(8370)-只有复制器可以执行这个功能。
        '(8371)-指 定的类别没有定义。
        '(8372)-指定的类别不是一个子类别。
        '(8373)-名称参考无效。
        '(8374)-交叉参考已经存在。
        '(8375)- 不允许删除一个主控交叉参考。
        '(8376)-只在 NC 头上支持子目录树通知。
        '(8377)-通知筛选器太复杂。
        '(8378)- 架构更新失败: 重复的 RDN。
        '(8379)-架构更新失败: 重复的 OID。
        '(8380)-架构更新失败: 重复的 MAPI 识别符。
        '(8381)-架构更新失败: 复制架构 id GUID。
        '(8382)-架构更新失败: 重复的 LDAP 显示名称。
        '(8383)- 架构更新失败: 范围下部少于范围上部。
        '(8384)-架构更新失败: 语法不匹配。
        '(8385)-架构更新失败: 属性在必须包含中使用。
        '(8386)-架构更新失败: 属性在可能包含中使用。
        '(8387)-架构更新失败: 可能包含中的属性不存在。
        '(8388)- 架构更新失败:必须包含中的属性不存在。
        '(8389)-架构更新失败: 在辅助类别列表中的类别不存在或不是一个辅助类别。
        '(8390)- 架构更新失败: poss-superior 中的类别不存在。
        '(8391)-架构更新失败: 在 subclassof 列表中的类别不存在或不能满足等级规则。
        '(8392)-架构更新失败: Rdn-Att-Id 语法不对。
        '(8393)-架构更新失败: 类别作为辅助类别使用。
        '(8394)-架构更新失败: 类别作为子类别使用。
        '(8395)-架构更新失败: 类别作为 poss superior 使用。
        '(8396)-架构更新在重新计算验证缓存时失败。
        '(8397)-目录树删除没有完成。要继续删除目录树，必须 再次发出请求。
        '(8398)-不能执行请求的删除操作。
        '(8399)-不能读?芄辜锹脊芾砝啾鹗侗鸱?
        '(8400)-属性架构 语法不对。
        '(8401)-不能缓存属性。
        '(8402)-不能缓存类别。
        '(8403)-不能从缓存删除属性。
        '(8404)- 无法从缓存中删除类别。
        '(8405)-无法读取特殊名称的属性。
        '(8406)-丢失一个所需的子参考。
        '(8407)-不能检索范 例种类属性。
        '(8408)-出现一个内部错误。
        '(8409)-出现一个数据错误。
        '(8410)-丢失一个属性 GOVERNSID。
        '(8411)-丢失一个所需要的属性。
        '(8412)-指定的命名上下文丢失了一个交叉参考。
        '(8413)- 出现一个安全检查错误。
        '(8414)-没有加载架构。
        '(8415)-架构分配失败。请检查机器内存是否不足。
        '(8416)-为属 性架构获得所需语法失败。
        '(8417)-全局编录验证失败。全局编录无效或不支持操作。目录的某些部分目前无效。
        '(8418)-由于有关 服务器之间的架构不匹配，复制操作失败。
        '(8419)-找不到 DSA 对象。
        '(8420)-找不到命名上下文。
        '(8421)- 在缓存中找不到命名上下文。
        '(8422)-无法检索子对象。
        '(8423)-由于安全原因不允许修改。
        '(8424)-操作不能替换 该隐藏的记录。
        '(8425)-等级无效。
        '(8426)-尝试建立等级表失败。
        '(8427)-目录配置参数在注册中丢失。
        '(8428)- 尝试计算地址簿索引失败。
        '(8429)-等级表的分配失败。
        '(8430)-目录服务遇到一个内部失败。
        '(8431)-目录服务遇 到一个未知失败。
        '(8432)-根对象需要一个 $top$ 类别。
        '(8433)-这个目录服务器已关闭，并且不能接受新上浮单一主机操 作角色的所有权。
        '(8434)-目录服务没有必需的配置信息，并且不能决定新上浮单一主机操作角色的所有权。
        '(8435)-该目录服务无 法将一个或多个上浮单一主机操作角色传送给其它服务器。
        '(8436)-复制操作失败。
        '(8437)-为这个复制操作指定了一个无效的参 数。
        '(8438)-目录服务太忙，现在无法完成这个复制操作。
        '(8439)-为这个复制操作指定的单一名称无效。
        '(8440)- 为这一个复制操作所指定的命名上下文无效。
        '(8441)-为这个复制操作指定的单一名称已经存在。
        '(8442)-复制系统遇到一个内部错 误。
        '(8443)-复制操作遇到数据库不一致问题。
        '(8444)-不能连接到为这个复制操作指定的服务器上。
        '(8445)-复制 操作遇到一个有无效范例类型的对象。
        '(8446)-复制操作无法分配内存。
        '(8447)-复制操作遇到一个邮件系统错误。
        '(8448)- 目标服务器的复制参考信息已经存在。
        '(8449)-目标服务器的复制参考信息不存在。
        '(8450)-由于是由另一台服务器上复制的，因此 不能删除命名上下文。
        '(8451)-复制操作遇到一个数据库错误。
        '(8452)-命名上下文要被删除或没有从指定的服务器上复制。
        '(8453)- 复制访问被拒绝。
        '(8454)-这个版本的目录服务不支持请求的操作。
        '(8455)-取消复制远程过程呼叫。
        '(8456)-源服 务器目前拒绝复制请求。
        '(8457)-目标服务器当前拒绝复制请求。
        '(8458)-由于对象名称冲突，复制操作失败。
        '(8459)- 复制源已被重新安装。
        '(8460)-由于一个所需父对象丢失，复制操作失败。
        '(8461)-复制操作被抢先。
        '(8462)-由于 缺乏更新，放弃复制同步尝试。
        '(8463)-由于系统正在关闭，复制操作被中断了。
        '(8464)-由于目标部分属性设置不是一个源部分属 性设置的子设置，复制同步尝试失败。
        '(8465)-由于主复制尝试从部分复制同步，复制同步尝试失败。
        '(8466)-已经与为这个复制操 作的指定的服务器联系，但是该服务器无法与完成这个操作所需的另外一个服务器联系。
        '(8467)-在副本安装时，检测到一个使用的源和内部版本之 间的架构不匹配，不能安装该副本。
        '(8468)-架构更新失败: 有同一连接标识符的属性已经存在。
        '(8469)-名称翻译: 常见处理错误。
        '(8470)-名称翻译: 不能找到该名称或权限不够，不能看到名称。
        '(8471)-名称翻译: 输入名称映射到多个输出名称。
        '(8472)-名称翻译: 找到输出名称，但是找不到相应的输出格式。
        '(8473)-名称翻译: 不能完全解析，只找到了域。
        '(8474)-名称翻译: 不接到线上，无法在客户机上执行纯粹的语法映射。
        '(8475)-不允许一个构造 att 修改。
        '(8476)-指定的 OM-Object 类别对指定语法的一个属性是不正确的。
        '(8477)-复制请求已暂停; 等待回答。
        '(8478)-要求的操作需要一个目录服务，但没有可用的。
        '(8479)-类别或属性的 LDAP 显示名称含有非 ASCII 字符。
        '(8480)-请求的查找操作只支持基本查找。
        '(8481)-查找未能从数据库检索属性。
        '(8482)-架构 更新操作试图添加一个反向链接，但该反向链接没有相应的正向链接。
        '(8483)-跨域移动的来源和目标在对象日期上不一致。或者是来源，或者是目 标没有对象的最后一个版本。
        '(8484)-跨域移动的来源和目标在对象当前的名称上不一致。或者是来源，或者是目标没有对象的最后一个版本。
        '(8485)- 域间移动的来源和目标是一样的。调用程序应该使用本地移动操作，而不是域间移动操作。
        '(8486)-域间移动的来源和目标与目录林中的命名上下文 不一致。来源或目标没有分区容器的最近版本。
        '(8487)-跨域移动的目标不是目标命名上下文的权威。
        '(8488)-跨域移动的来源和目 标提供的来源对象的身份不一样。 来源或目标没有来源对象的最近版本。
        '(8489)-跨域移动的对象应该已经被目标服务器删除。来源服务器没有来 源对象的最近版本。
        '(8490)-要求对 PDC FSMO 的专门访问权的另一个操作正在进行中。
        '(8491)-跨域移动没有成功，导 致被移动对象有两个版本 – 一个在来源域，一个在目标域。需要删除目标对象，将系统还原到一致状态。
        '(8492)-因为不允许这个类别的跨域移 动，或者对象有一些特点，如: 信任帐户或防止移动的受限制的 RID；所以不能将该对象跨域移动。
        '(8493)-一旦移动，不能将带有成员身份 的对象跨域移动，这会侵犯帐户组的成员身份条件。从帐户组成员身份删除对象，再试一次。
        '(8494)-命名上下文标题必须是另一个命名上下文标题 的直接子标题，而不是一个内节点的子标题。
        '(8495)-因为目录没有提议的命名上下文上面的命名上下文的副本，所以无法验证所提议的命名上下文 的名称。请保证充当域命名主机的服务器已配置成全局编录服务器，并且服务器及其复制伙伴是最新的。
        '(8496)-目标域必须在本机模式中。
        '(8497)- 因为服务器在指定域中没有基?峁谷萜鳎晕薹ㄖ葱胁僮鳌?
        '(8498)-不允许跨域移动帐户组。
        '(8499)-不允许跨域移动资源组。
        '(8500)- 属性的搜索标志无效。ANR 位只在 Unicode 或 Teletex 字符串的属性上有效。
        '(8501)-不允许在将 NC 头作为子体的对象开始删除目录树。
        '(8502)-因为目录树在使用中，目录服务未能为删除目录树而将其锁定。
        '(8503)-删除目录树 时，目录服务未能识别要删除的对象列表。
        '(8505)-只有管理员才能修改管理组的成员列表。
        '(8506)-不能改变域控制器帐户的主要 组 ID。
        '(8507)-试图修改基?芄埂?
        '(8508)-不允许进行下列操作: 为现有类别添加新的强制属性；从现有类别删除强制属性；为没有向回链接属性的特殊类别 “Top” 添加可选属性，向回链接属性指的是直接或通过继承。例如: 添加或删除附属类别。
        '(8509)-该域控制器上不允许架构更新。没有设置注册表项， 或者 DC 不是架构 FSMO 角色所有者。
        '(8510)-无法在架构容器下创建这个类别的对象。在架构容器下，您只能创建属性架构和类别架构 对象。
        '(8511)-副本/子项安装未能获取源 DC 上的架构容器的 objectVersion 属性。架构容器上的属性不存在，或者提供的凭据没有读取属性的权限。
        '(8512)-副本/子项安装未能读取 system32 目录中的文件 schema.ini 的 SCHEMA 段中的 objectVersion 属性。
        '(8513)-指定的组类型无效。
        '(8514)- 如果域是安全启用的，在混合型域中不能嵌套全局组。
        '(8515)-如果域是安全启用的，在混合型域中不能嵌套本地组。
        '(8516)-全局 组不能将本地组作为成员。
        '(8517)-全局组不能将通用组作为成员。
        '(8518)-通用组不能将本地组作为成员。
        '(8519)- 全局组不能有跨域成员。
        '(8520)-本地组不能将另一个跨域本地组作为成员。
        '(8521)-包含主要成员的组不能改变为安全停用的组。
        '(8522)- 架构缓冲加载未能转换类架构对象上的字符串默认值 SD。
        '(8523)-只有配置成全局编录服务器的 DSAs 才能充当域命名主机 FSMO 的角色。
        '(8524)-由于 DNS 查找故障，DSA 操作无法进行。
        '(8525)-处理一个对象的 DNS 主机名改动时，服务主要名称数值无法保持同步。
        '(8526)-未能读取安全描述符属性。
        '(8527)-没有找到请求的对象，但找到了具有 那个密钥的对象。
        '(8528)-正在添加的链接属性的语法不正确。正向链接只能有语法 2.5.5.1、2.5.5.7 和 2.5.5.14，而反向链接只能有语法 2.5.5.1
        '(8529)-安全帐户管理员需要获得启动密码。
        '(8530)-安全帐户管理员 需要从软盘获得启动密钥。
        '(8531)-目录服务无法启动。
        '(8532)-未能启动目录服务。
        '(8533)-客户和服务器之间的 连接要求数据包保密性。
        '(8534)-来源域跟目标域不在同一个目录林中。
        '(8535)-目标域必须在目录林中。
        '(8536)- 该操作要求启用目标域审核。
        '(8537)-该操作无法为来源域找到 DC。
        '(8538)-来源对象必须是一个组或用户。
        '(8539)- 来源对象的 SID 已经在目标目录林中。
        '(8540)-来源对象和目标对象必须属于同一类型。
        '(8542)-在复制请求中不能包括架构 信息。
        '(8543)-由于架构不兼容性，无法完成复制操作。
        '(8544)-由于前一个架构的不兼容性，无法完成复制操作。
        '(8545)- 因为源和目标都没有收到有关最近跨域启动操作的信息，所以无法应用复制更新。
        '(8546)-因为还有主控这个域的域控制器，所以无法删除请求的 域。
        '(8547)-只能在全局编录服务器上执行请求的操作。
        '(8548)-本地组只能是同一个域中其他本地组的成员。
        '(8549)- 外部安全主要成员不能是通用组的成员。
        '(8550)-出于安全，无法将属性复制到 GC。
        '(8551)-由于目前正在处理的修改太多，无 法采取 PDC 的检查点。
        '(8552)-操作需要启用那个源域审核。
        '(8553)-安全主要对象仅能在域命名环境菜单中创建。
        '(8554)- 服务主要名称(SPN) 无法建造，因为提供的主机名格式不适合。
        '(8555)-筛选器已传递建造的属性。
        '(8556)-unicodePwd 属性值必须括在双引号中。
        '(8557)-您的计算机无法加入域。已超出此域上允许创建的计算机帐户的最大值。请同系统管理员联系，复位或增加此 限定值。
        '(8558)-由于安全原因，操作必须在目标 DC 上运行。
        '(8559)-由于安全原因，源 DC 必须是 Service Pack 4 或更新版本。
        '(8560)-在树目录删除的操作中不能删除“关键目录服务系统”对象。数目录删除操作可能只进行了一部分。
        '(9001)-DNS 服务器无法解释格式。
        '(9002)-DNS 服务器失败。
        '(9003)-DNS 名称不存在。
        '(9004)-名称服务器不支持 DNS 请求。
        '(9005)-拒绝 DNS 操作。
        '(9006)-不应该存在的 DNS 名称仍然存在。
        '(9007)-不应该 存在的 DNS RR 集仍然存在。
        '(9008)-应该存在的 DNS RR 集不存在。
        '(9009)-DNS 服务器对区域没有权威。
        '(9010)- 在更新或 prereq 中的 DNS 名称不在区域中。
        '(9016)-DNS 签名验证失败。
        '(9017)-DNS 不正确密钥。
        '(9018)-DNS 签名验证过期。
        '(9501)-为 DNS 查询找不到记录。
        '(9502)-无效 DNS 包。
        '(9503)-没有 DNS 包。
        '(9504)-DNS 错误，请检查 rcode。
        '(9505)-为保险的 DNS 包。
        '(9551)-无效的 DNS 种类。
        '(9552)-无效的 IP 地址。
        '(9553)-无效的属性。
        '(9554)-稍后再试一次 DNS 操作。
        '(9555)- 给出的记录名称和种类不是单一的。
        '(9556)-DNS 名称不符合 RFC 说明。
        '(9557)-DNS 名称是一个完全合格的 DNS 名称。
        '(9558)-DNS 名称以“.”分隔(多标签)。
        '(9559)-DNS 名称是单一部分名称。
        '(9560)-DNS 名称含有无效字符。
        '(9561)-DNS 名称完全是数字的。
        '(9601)-DNS 区域不存在。
        '(9602)-DNS 区域信息无效。
        '(9603)-DNS 区域无效操作。
        '(9604)-无效 DNS 区域配置。
        '(9605)-DNS 区域没有颁发机构记录的开始(SOA)。
        '(9606)-DNS 区域没有“名称服务器” (NS)的记录。
        '(9607)-DNS 区域已锁定。
        '(9608)-DNS 区域创建失败。
        '(9609)-DNS 区域已经存在。
        '(9610)-DNS 自动区域已经存在。
        '(9611)-无效的 DNS 区域种类。
        '(9612)-次要 DNS 区域需要主 IP 地址。
        '(9613)-DNS 区域不是次要的。
        '(9614)-需要一个次要 IP 地址
        '(9615)-WINS 初始化失败。
        '(9616)-需要 WINS 服务器。
        '(9617)-NBTSTAT 初始化呼叫失败。
        '(9618)-颁发机构起始(SOA)删除无效
        '(9651)-主要 DNS 区域需要数据文件。
        '(9652)-DNS 区域的无效数据文件名称。
        '(9653)-为 DNS 区域打开数据文件失败。
        '(9654)- 为 DNS 区域写数据文件失败。
        '(9655)-为 DNS 区域读取数据文件时失败。
        '(9701)-DNS 记录不存在。
        '(9702)-DNS 记录格式错误。
        '(9703)-DNS 中节点创建失败。
        '(9704)-未知 DNS 记录类型。
        '(9705)-DNS 记录超时。
        '(9706)-名称不在 DNS 区域。
        '(9707)-检测到 CNAME 循环。
        '(9708)-节点为一个 CNAME DNS 记录。
        '(9709)-指定名称的 CNAME 记录已经存在。
        '(9710)-记录不在 DNS 区域根目录。
        '(9711)-DNS 记录已经存在。
        '(9712)-次要 DNS 区域数据错误。
        '(9713)-不能创建 DNS 缓存数据。
        '(9714)-DNS 名称不存在。
        '(9715)-不能创建指针(PTR)记录。
        '(9716)-DNS 域没有被删除。
        '(9717)-该目录服务无 效。
        '(9718)-DNS 区域已经在目录服务中存在。
        '(9719)-DNS 服务器没有为目录服务集合 DNS 区域创建或读取启动文件。
        '(9751)-完成 DNS AXFR (区域复制)。
        '(9752)-DNS 区域复制失败。
        '(9753)- 添加了本地 WINS 服务器。
        '(9801)-安全更新呼叫需要继续更新请求。
        '(9851)-TCP/IP 没有安装网络协议。
        '(9852)- 没有为本地系统配置 DNS 服务器。
        '(10004)-一个封锁操作被对 WSACancelBlockingCall 的调用中断。
        '(10009)- 提供的文件句柄无效。
        '(10013)-以一种访问权限不允许的方式做了一个访问套接字的尝试。
        '(10014)-系统检测到在一个调用中尝 试使用指针参数时的无效指针地址。
        '(10022)-提供了一个无效的参数。
        '(10024)-打开的套接字太多。
        '(10035)- 无法立即完成一个非阻挡性套接字操作。
        '(10036)-目前正在执行一个阻挡性操作。
        '(10037)-在一个非阻挡套接字上尝试了一个已 经在进行的操作。
        '(10038)-在一个非套接字上尝试了一个操作。
        '(10039)-请求的地址在一个套接字中从操作中忽略。
        '(10040)- 一个在数据报套接字上发送的消息大于内部消息缓冲器或其它一些网络限制，或该用户用于接收数据报的缓冲器比数据报小。
        '(10041)-在套接字函 数调用中指定的一个协议不支持请求的套接字类别的语法。
        '(10042)-在 getsockopt 或 setsockopt 调用中指定的一个未知的、无效的或不受支持的选项或层次。
        '(10043)-请求的协议还没有在系统中配置，或者没有它存在的迹象。
        '(10044)- 在这个地址家族中不存在对指定的插槽种类的支持。
        '(10045)-参考的对象种类不支持尝试的操作。
        '(10046)-协议家族尚未配置到 系统中或没有它的存在迹象。
        '(10047)-使用了与请求的协议不兼容的地址。
        '(10048)-通常每个套接字地址 (协议/网络地址/端口)只允许使用一次。
        '(10049)-在其上下文中，该请求的地址无效。
        '(10050)-套接字操作遇到了一个已死 的网络。
        '(10051)-向一个无法连接的网络尝试了一个套接字操作。
        '(10052)-当该操作在进行中，由于保持活动的操作检测到一个 故障，该连接中断。
        '(10053)-您的主机中的软件放弃了一个已建立的连接。
        '(10054)-远程主机强迫关闭了一个现有的连接。
        '(10055)- 由于系统缓冲区空间不足或列队已满，不能执行套接字上的操作。
        '(10056)-在一个已经连接的套接字上做了一个连接请求。
        '(10057)- 由于套接字没有连接并且 (当使用一个 sendto 调用发送数据报套接字时) 没有提供地址，发送或接收数据的请求没有被接受。
        '(10058)- 由于以前的关闭调用，套接字在那个方向已经关闭，发送或接收数据的请求没有被接受。
        '(10059)-对某个内核对象的引用过多。
        '(10060)- 由于连接方在一段时间后没有正确的答复或连接的主机没有反应，连接尝试失败。
        '(10061)-不能做任何连接，因为目标机器积极地拒绝它。
        '(10062)- 无法翻译名称。
        '(10063)-名称组件或名称太长。
        '(10064)-由于目标主机坏了，套接字操作失败。
        '(10065)-套接 字操作尝试一个无法连接的主机。
        '(10066)-不能删除目录，除非它是空的。
        '(10067)-一个 Windows 套接字操作可能在可以同时使用的应用程序数目上有限制。
        '(10068)-超过限额。
        '(10069)-超过磁盘限额。
        '(10070)- 文件句柄引用不再有效。
        '(10071)-项目在本地不可用。
        '(10091)-因为它使用提供网络服务的系统目前无 效，WSAStartup 目前不能正常工作。
        '(10092)-不支持请求的 Windows 套接字版本
        '(10093)-应用程序没有 调用 WSAStartup，或者 WSAStartup 失败。
        '(10101)-由 WSARecv 或 WSARecvFrom 返回表示远程方面已经开始了关闭步骤。
        '(10102)-WSALookupServiceNext 不能返回更多的结果。
        '(10103)- 在处理这个调用时，就开始调用 WSALookupServiceEnd。该调用被删除。
        '(10104)-过程调用无效。
        '(10105)- 请求的服务提供程序无效。
        '(10106)-没有加载或初始化请求的服务提供程序。
        '(10107)-从来不应失败的系统调用失败了。
        '(10108)- 没有已知的此服务。在指定的名称空间中找不这个服务。
        '(10109)-找不到指定的类别。
        '(10110)-WSALookupServiceNext 不能返回更多的结果。
        '(10111)-在处理这个调用时，就开始调用 WSALookupServiceEnd。该调用被删除。
        '(10112)- 由于被拒绝，数据查询失败。
        '(11001)-不知道这样的主机。
        '(11002)-这是在主机名解析时常出现的暂时错误，并且意味着本地服 务器没有从权威服务器上收到响应。
        '(11003)-在数据寻找中出现一个不可恢复的错误。
        '(11004)-请求的名称有效并且是在数据库 中找到，但是它没有相关的正确的数据。
        '(11005)-至少到达了一个保留。
        '(11006)-至少到达了一个路径。
        '(11007)- 没有发送方。
        '(11008)-没有接受方。
        '(11009)-保留已经确认。
        '(11010)-错误是由于资源不足造成。
        '(11011)- 由于管理原因被拒绝 – 无效凭据。
        '(11012)-未知或有冲突类型。
        '(11013)-某一部分的 filterspec 或 providerspecific 缓冲区有问题。
        '(11014)-flowspec 的某部分有问题。
        '(11015)-一般性 QOS 错误。
        '(11016)-在流程规格中发现一个无效的或不可识别的服务类型。
        '(11017)-在 QOS 结构中发现一个无效的或不一致的流程规格。
        '(11018)-无效的 QOS 提供程序特定缓冲区。
        '(11019)-使用了无效的 QOS 筛选器样式。
        '(11020)-使用了无效的 QOS 筛选器类型。
        '(11021)-FLOWDESCRIPTOR 中指定的 QOS FILTERSPEC 数量不正确。
        '(11022)-在 QOS 提供程序特定缓冲区中指定了一个 ObjectLength 字符域无效的对象。
        '(11023)-QOS 结构中指定的流程描述符数量不正确。
        '(11024)-在 QOS 提供程序特定缓冲区中发现一个不可识别的对象。
        '(11025)-在 QOS 提供程序特定缓冲区中发现一个无效的策略对象。
        '(11026)- 在流程描述符列表中发现一个无效的 QOS 流程描述符。
        '(11027)-在 QOS 提供程序特定缓冲区中发现一个无效的或不一致的流程规格。
        '(11028)- 在 QOS 提供程序特定缓冲区中发现一个无效的 FILTERSPEC。
        '(11029)-在 QOS 提供程序特定缓冲区中发现一个无效的波形丢弃模式对象。
        '(11030)-在 QOS 提供程序特定缓冲区中发现一个无效的成形速率对象。
        '(11031)- 在 QOS 提供程序特定缓冲区中发现一个保留的策略因素

    End Module
End Namespace
