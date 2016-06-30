
<[Namespace]("user32.dll")>
Public Module User32

    <Command("SetWindowsHookExA")>
    Public Declare Function SetWindowsHookEx Lib "user32" Alias "SetWindowsHookExA" (idHook As Integer, lpfn As Integer, hmod As Integer, dwThreadId As Integer) As Integer
    <Command("SendMessageA")>
    Public Declare Function SendMessage Lib "user32" Alias "SendMessageA" (hWnd As Integer, wMsg As Integer, wParam As Integer, lParam As Object) As Integer
    <Command("SetCapture")>
    Public Declare Function SetCapture Lib "user32" (hwnd As Integer) As Integer
    <Command("ReleaseCapture")>
    Public Declare Function ReleaseCapture Lib "user32" () As Integer
    <Command("GetWindowRect")>
    Public Declare Function GetWindowRect Lib "user32" (hwnd As Integer, lpRect As RECT) As Integer
    <Command("GetCursorPos")>
    Public Declare Function GetCursorPos Lib "user32" (lpPoint As System.Drawing.Point) As Integer
    <Command("SetCursorPos")>
    Public Declare Function SetCursorPos Lib "user32" (x As Integer, y As Integer) As Integer
    <Command("DrawEdge")>
    Public Declare Function DrawEdge Lib "user32" (hdc As Integer, qrc As RECT, edge As Integer, grfFlags As Integer) As Boolean
    <Command("OffsetRect")>
    Public Declare Function OffsetRect Lib "user32" (lpRect As RECT, x As Integer, y As Integer) As Integer
    <Command("DrawTextA")>
    Public Declare Function DrawText Lib "user32" Alias "DrawTextA" (hdc As Integer, lpStr As String, nCount As Integer, lpRect As RECT, wFormat As Integer) As Integer
    <Command("GetDC")>
    Public Declare Function GetDC Lib "user32" (hwnd As Integer) As Integer
    <Command("ReleaseDC")>
    Public Declare Function ReleaseDC Lib "user32" (hwnd As Integer, hdc As Integer) As Integer
    <Command("FillRect")>
    Public Declare Function FillRect Lib "user32" (hdc As Integer, lpRect As RECT, hBrush As Integer) As Integer
    <Command("DrawStateA")>
    Public Declare Function DrawState Lib "user32" Alias "DrawStateA" (hdc As Integer, hBrush As Integer, lpDrawStateProc As Integer, lParam As Integer, wParam As Integer, n1 As Integer, n2 As Integer, n3 As Integer, n4 As Integer, un As Integer) As Integer
    <Command("GetWindowDC")>
    Public Declare Function GetWindowDC Lib "user32" (hwnd As Integer) As Integer
    <Command("IntersectRect")>
    Public Declare Function IntersectRect Lib "user32" (lpDestRect As RECT, lpSrc1Rect As RECT, lpSrc2Rect As RECT) As Integer
    <Command("SubtractRect")>
    Public Declare Function SubtractRect Lib "user32" (lprcDst As RECT, lprcSrc1 As RECT, lprcSrc2 As RECT) As Integer
    <Command("UnionRect")>
    Public Declare Function UnionRect Lib "user32" (lpDestRect As RECT, lpSrc1Rect As RECT, lpSrc2Rect As RECT) As Integer
    <Command("IsRectEmpty")>
    Public Declare Function IsRectEmpty Lib "user32" (lpRect As RECT) As Integer
    <Command("SetRect")>
    Public Declare Function SetRect Lib "user32" (lpRect As RECT, X1 As Integer, Y1 As Integer, X2 As Integer, Y2 As Integer) As Integer
    <Command("EqualRect")>
    Public Declare Function EqualRect Lib "user32" (lpRect1 As RECT, lpRect2 As RECT) As Integer

End Module
