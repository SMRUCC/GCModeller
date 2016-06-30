Imports System.Drawing

<[Namespace]("gdi32.dll")>
Public Module Gdi32

    ''' <summary>
    ''' The Escape function allows applications to access capabilities of a particular device not directly available through GDI. Escape calls made by an application are translated and sent to the driver 
    ''' </summary>
    ''' <param name="hdc">Identifies the device context.</param>
    ''' <param name="nEscape">Specifies the escape function to be performed. This parameter must be one of the predefined escape values. Use the ExtEscape function if your application defines a private escape value.</param>
    ''' <param name="nCount">Specifies the number of bytes of data pointed to by the lpvInData parameter.</param>
    ''' <param name="lpInData">Points to the input structure required for the specified escape.</param>
    ''' <param name="lpOutData">
    ''' Points to the structure that receives output from this escape. This parameter should be NULL if no data is returned
    ''' 
    ''' If the function succeeds, the return value is greater than zero, except with the QUERYESCSUPPORT printer escape, which checks for implementation only. If the escape is not implemented, the return value is zero. 
    ''' 
    ''' If the function fails, the return value is an error. To get extended error information, call GetLastError. 
    ''' </param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 
    <Command("Escape")>
    Public Declare Function Escape Lib "gdi32" Alias "Escape" (ByVal hdc As Long, ByVal nEscape As Long, ByVal nCount As Long, ByVal lpInData As String, lpOutData As Object) As Long
    <Command("Rectangle")>
    Public Declare Function Rectangle Lib "gdi32" (hdc As Integer, X1 As Integer, Y1 As Integer, X2 As Integer, Y2 As Integer) As Integer
    <Command("Polygon")>
    Public Declare Function Polygon Lib "gdi32" (hdc As Integer, lpPoint As Point, nCount As Integer) As Integer
    <Command("CreateSolidBrush")>
    Public Declare Function CreateSolidBrush Lib "gdi32" (crColor As Integer) As Integer
    <Command("CreatePatternBrush")>
    Public Declare Function CreatePatternBrush Lib "gdi32" (hBitmap As Integer) As Integer
    <Command("CreatePen")>
    Public Declare Function CreatePen Lib "gdi32" (nPenStyle As Integer, nWidth As Integer, crColor As Integer) As Integer
    <Command("GetPixel")>
    Public Declare Function GetPixel Lib "gdi32" (hdc As Integer, x As Integer, y As Integer) As Integer
    <Command("SetPixelV")>
    Public Declare Function SetPixelV Lib "gdi32" (hdc As Integer, x As Integer, y As Integer, crColor As Integer) As Integer
    <Command("CreateCompatibleDC")>
    Public Declare Function CreateCompatibleDC Lib "gdi32" (hdc As Integer) As Integer
    <Command("DeleteDC")>
    Public Declare Function DeleteDC Lib "gdi32" (hdc As Integer) As Integer
    <Command("SaveDC")>
    Public Declare Function SaveDC Lib "gdi32" (hdc As Integer) As Integer
    <Command("RestoreDC")>
    Public Declare Function RestoreDC Lib "gdi32" (hdc As Integer, nSavedDC As Integer) As Integer
    <Command("CreateBitmap")>
    Public Declare Function CreateBitmap Lib "gdi32" (nWidth As Integer, nHeight As Integer, nPlanes As Integer, nBitCount As Integer, lpBits As Object) As Integer
    <Command("CreateCompatibleBitmap")>
    Public Declare Function CreateCompatibleBitmap Lib "gdi32" (hdc As Integer, nWidth As Integer, nHeight As Integer) As Integer
    <Command("CreateBitmapIndirect")>
    Public Declare Function CreateBitmapIndirect Lib "gdi32" (lpBitmap As C_BITMAP) As Integer
    <Command("GetObjectA")>
    Public Declare Function GetObject Lib "gdi32" Alias "GetObjectA" (hObject As Integer, nCount As Integer, lpObject As Object) As Integer
    <Command("SelectObject")>
    Public Declare Function SelectObject Lib "gdi32" (hdc As Integer, hObject As Integer) As Integer
    <Command("DeleteObject")>
    Public Declare Function DeleteObject Lib "gdi32" (hObject As Integer) As Integer
    <Command("GetMapMode")>
    Public Declare Function GetMapMode Lib "gdi32" (hdc As Integer) As Integer
    <Command("SetMapMode")>
    Public Declare Function SetMapMode Lib "gdi32" (hdc As Integer, nMapMode As Integer) As Integer
    <Command("SetBkColor")>
    Public Declare Function SetBkColor Lib "gdi32" (hdc As Integer, crColor As Integer) As Integer
    <Command("BitBlt")>
    Public Declare Function BitBlt Lib "gdi32" (hDestDC As Integer, x As Integer, y As Integer, nWidth As Integer, nHeight As Integer, hSrcDC As Integer, xSrc As Integer, ySrc As Integer, dwRop As Integer) As Integer
    <Command("StretchBlt")>
    Public Declare Function StretchBlt Lib "gdi32" (hdc As Integer, x As Integer, y As Integer, nWidth As Integer, nHeight As Integer, hSrcDC As Integer, xSrc As Integer, ySrc As Integer, nSrcWidth As Integer, nSrcHeight As Integer, dwRop As Integer) As Integer
    <Command("SetTextColor")>
    Public Declare Function SetTextColor Lib "gdi32" (hdc As Integer, crColor As Integer) As Integer
    <Command("SetBkMode")>
    Public Declare Function SetBkMode Lib "gdi32" (hdc As Integer, nBkMode As Integer) As Integer

End Module
