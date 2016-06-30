Attribute VB_Name = "modFileEx"

Public Const REG_SZ As Long = 1
Public Const REG_DWORD As Long = 4
Public Const HKEY_CLASSES_ROOT = &H80000000
Public Const HKEY_CURRENT_USER = &H80000001
Public Const HKEY_LOCAL_MACHINE = &H80000002
Public Const HKEY_USERS = &H80000003
Public Const ERROR_NONE = 0
Public Const ERROR_BADDB = 1
Public Const ERROR_BADKEY = 2
Public Const ERROR_CANTOPEN = 3
Public Const ERROR_CANTREAD = 4
Public Const ERROR_CANTWRITE = 5
Public Const ERROR_OUTOFMEMORY = 6
Public Const ERROR_INVALID_PARAMETER = 7
Public Const ERROR_ACCESS_DENIED = 8
Public Const ERROR_INVALID_PARAMETERS = 87
Public Const ERROR_NO_MORE_ITEMS = 259
Public Const KEY_ALL_ACCESS = &H3F
Public Const REG_OPTION_NON_VOLATILE = 0

Public Declare Function RegCloseKey Lib "advapi32.dll" (ByVal hKey As Long) As Long
Public Declare Function RegCreateKeyEx Lib "advapi32.dll" Alias "RegCreateKeyExA" (ByVal hKey As Long, ByVal lpSubKey As String, ByVal Reserved As Long, ByVal lpClass As String, ByVal dwOptions As Long, ByVal samDesired As Long, ByVal lpSecurityAttributes As Long, phkResult As Long, lpdwDisposition As Long) As Long
Public Declare Function RegOpenKeyEx Lib "advapi32.dll" Alias "RegOpenKeyExA" (ByVal hKey As Long, ByVal lpSubKey As String, ByVal ulOptions As Long, ByVal samDesired As Long, phkResult As Long) As Long
Public Declare Function RegSetValueExString Lib "advapi32.dll" Alias "RegSetValueExA" (ByVal hKey As Long, ByVal lpValueName As String, ByVal Reserved As Long, ByVal dwType As Long, ByVal lpValue As String, ByVal cbData As Long) As Long
Public Declare Function RegSetValueExLong Lib "advapi32.dll" Alias "RegSetValueExA" (ByVal hKey As Long, ByVal lpValueName As String, ByVal Reserved As Long, ByVal dwType As Long, lpValue As Long, ByVal cbData As Long) As Long

Public Sub CreateAssociation(strAsso As String, strDescription As String, strExeName As String, strIcon As String)
    Dim sPath As String

    CreateNewKey strAsso, HKEY_CLASSES_ROOT
    SetKeyValue strAsso, "", strDescription, REG_SZ
      
    CreateNewKey strDescription & "\shell\open\command", HKEY_CLASSES_ROOT
    SetKeyValue strDescription, "", strDescription, REG_SZ
    sPath = strExeName & " %1"
    SetKeyValue strDescription & "\shell\open\command", "", sPath, REG_SZ
   
    CreateNewKey strDescription & "\DefaultIcon", HKEY_CLASSES_ROOT
    SetKeyValue strDescription & "\DefaultIcon", "", strIcon, REG_SZ
End Sub
  
Public Function SetValueEx(ByVal hKey As Long, sValueName As String, lType As Long, vValue As Variant) As Long
    Dim nValue As Long
    Dim sValue As String
    
    Select Case lType
        Case REG_SZ
            sValue = vValue & Chr$(0)
            SetValueEx = RegSetValueExString(hKey, sValueName, 0&, lType, sValue, Len(sValue))
        Case REG_DWORD
            nValue = vValue
            SetValueEx = RegSetValueExLong(hKey, sValueName, 0&, lType, nValue, 4)
    End Select
End Function

Public Sub CreateNewKey(sNewKeyName As String, lPredefinedKey As Long)
    Dim hKey As Long
    Dim r As Long
    
    r = RegCreateKeyEx(lPredefinedKey, sNewKeyName, 0&, vbNullString, REG_OPTION_NON_VOLATILE, KEY_ALL_ACCESS, 0&, hKey, r)
    Call RegCloseKey(hKey)
End Sub

Public Sub SetKeyValue(sKeyName As String, sValueName As String, vValueSetting As Variant, lValueType As Long)
    Dim r As Long
    Dim hKey As Long

    r = RegOpenKeyEx(HKEY_CLASSES_ROOT, sKeyName, 0, KEY_ALL_ACCESS, hKey)
    r = SetValueEx(hKey, sValueName, lValueType, vValueSetting)
    Call RegCloseKey(hKey)
End Sub
