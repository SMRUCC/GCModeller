Imports RDotNet.Graphics.Internals
Imports System.Runtime.InteropServices

Namespace Graphics

    Public Class DeviceDescription
        Inherits SafeHandle
        Public Sub New()
            MyBase.New(IntPtr.Zero, True)
            Dim pointer As System.IntPtr = Marshal.AllocHGlobal(Marshal.SizeOf(GetType(DevDesc)))
            Marshal.StructureToPtr(New DevDesc(), pointer, True)
            SetHandle(pointer)
            SetDefaultParameter()
        End Sub

        Public Overrides ReadOnly Property IsInvalid() As Boolean
            Get
                Return handle = IntPtr.Zero
            End Get
        End Property

        Public Property Bounds() As Rectangle
            Get
                Dim dd As DevDesc = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return New Rectangle(dd.left, dd.bottom, dd.right - dd.left, dd.top - dd.bottom)
            End Get
            Set(value As Rectangle)
                WriteDouble("left", Value.Left)
                WriteDouble("right", Value.Right)
                WriteDouble("bottom", Value.Bottom)
                WriteDouble("top", Value.Top)
            End Set
        End Property

        Public Property ClipBounds() As Rectangle
            Get
                Dim dd As DevDesc = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return New Rectangle(dd.clipLeft, dd.clipBottom, dd.clipRight - dd.clipLeft, dd.clipTop - dd.clipBottom)
            End Get
            Set(value As Rectangle)
                WriteDouble("clipLeft", Value.Left)
                WriteDouble("clipRight", Value.Right)
                WriteDouble("clipBottom", Value.Bottom)
                WriteDouble("clipTop", Value.Top)
            End Set
        End Property

        Public Property CharOffsetX() As Double
            Get
                Dim dd As DevDesc = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.xCharOffset
            End Get
            Set(value As Double)
                WriteDouble("xCharOffset", Value)
            End Set
        End Property

        Public Property CharOffsetY() As Double
            Get
                Dim dd As DevDesc = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.yCharOffset
            End Get
            Set(value As Double)
                WriteDouble("yCharOffset", Value)
            End Set
        End Property

        Public Property LineBiasY() As Double
            Get
                Dim dd As DevDesc = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.yLineBias
            End Get
            Set(value As Double)
                WriteDouble("yLineBias", Value)
            End Set
        End Property

        Public Property InchesPerRasterX() As Double
            Get
                Dim dd As DevDesc = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.ipr(0)
            End Get
            Set(value As Double)
                WriteDoubleArray("ipr", 0, Value)
            End Set
        End Property

        Public Property InchesPerRasterY() As Double
            Get
                Dim dd As DevDesc = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.ipr(1)
            End Get
            Set(value As Double)
                WriteDoubleArray("ipr", 1, Value)
            End Set
        End Property

        Public Property CharacterSizeInRasterX() As Double
            Get
                Dim dd As DevDesc = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.cra(0)
            End Get
            Set(value As Double)
                WriteDoubleArray("cra", 0, Value)
            End Set
        End Property

        Public Property CharacterSizeInRasterY() As Double
            Get
                Dim dd As DevDesc = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.cra(1)
            End Get
            Set(value As Double)
                WriteDoubleArray("cra", 1, Value)
            End Set
        End Property

        Public Property Gamma() As Double
            Get
                Dim dd As DevDesc = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.gamma
            End Get
            Set(value As Double)
                WriteDouble("gamma", Value)
            End Set
        End Property

        Public Property IsGammaModifiable() As Boolean
            Get
                Dim dd As DevDesc = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.canChangeGamma
            End Get
            Set(value As Boolean)
                WriteBoolean("canChangeGamma", Value)
            End Set
        End Property

        Public Property IsClippable() As Boolean
            Get
                Dim dd As DevDesc = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.canClip
            End Get
            Set(value As Boolean)
                WriteBoolean("canClip", Value)
            End Set
        End Property

        Public Property Adjustment() As Adjustment
            Get
                Dim dd As DevDesc = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.canHAdj
            End Get
            Set(value As Adjustment)
                WriteInt32Enum("canHAdj", Value)
            End Set
        End Property

        Public Property StartFontSize() As Double
            Get
                Dim dd As DevDesc = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.startps
            End Get
            Set(value As Double)
                WriteDouble("startps", Value)
            End Set
        End Property

        Public Property StartForeground() As Color
            Get
                Dim dd As DevDesc = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.startcol
            End Get
            Set(value As Color)
                WriteColor("startcol", Value)
            End Set
        End Property

        Public Property StartBackground() As Color
            Get
                Dim dd As DevDesc = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.startfill
            End Get
            Set(value As Color)
                WriteColor("startfill", Value)
            End Set
        End Property

        Public Property StartLineType() As LineType
            Get
                Dim dd As DevDesc = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.startlty
            End Get
            Set(value As LineType)
                WriteInt32Enum("startlty", Value)
            End Set
        End Property

        Public Property StartFont() As Integer
            Get
                Dim dd As DevDesc = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.startfont
            End Get
            Set(value As Integer)
                WriteInt32("startfont", Value)
            End Set
        End Property

        Public Property StartGamma() As Double
            Get
                Dim dd As DevDesc = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.startgamma
            End Get
            Set(value As Double)
                WriteDouble("startgamma", Value)
            End Set
        End Property

        Public Property DisplayListOn() As Boolean
            Get
                Dim dd As DevDesc = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.displayListOn
            End Get
            Set(value As Boolean)
                WriteBoolean("displayListOn", Value)
            End Set
        End Property

        Public Property IsTextRotatedInContour() As Boolean
            Get
                Dim dd As DevDesc = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.useRotatedTextInContour
            End Get
            Set(value As Boolean)
                WriteBoolean("useRotatedTextInContour", Value)
            End Set
        End Property

        Protected Property DeviceSpecific() As IntPtr
            Get
                Dim dd As DevDesc = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.deviceSpecific
            End Get
            Set(value As IntPtr)
                WriteIntPtr("deviceSpecific", Value)
            End Set
        End Property

        Private Sub SetDefaultParameter()
            StartForeground = Colors.Black
            StartBackground = Colors.White
            StartLineType = LineType.Solid
            StartFont = 1
            StartFontSize = 12.0
            StartGamma = 1.0
            CharOffsetX = 0.49
            CharOffsetY = 0.3333
            CharacterSizeInRasterX = 9.0
            CharacterSizeInRasterY = 12.0
            InchesPerRasterX = 1.0 / 72.0
            InchesPerRasterY = 1.0 / 72.0
            LineBiasY = 0.2
            IsClippable = True
            Adjustment = Adjustment.All
            IsGammaModifiable = False
            DisplayListOn = False
        End Sub

        Private Sub WriteDouble(fieldName As String, value As Double)
            Dim bytes As Byte() = BitConverter.GetBytes(value)
            Dim offset As Integer = Marshal.OffsetOf(GetType(DevDesc), fieldName).ToInt32()
            Marshal.Copy(bytes, 0, IntPtr.Add(handle, offset), bytes.Length)
        End Sub

        Private Sub WriteDoubleArray(fieldName As String, index As Integer, value As Double)
            Dim bytes As Byte() = BitConverter.GetBytes(value)
            Dim offset As Integer = Marshal.OffsetOf(GetType(DevDesc), fieldName).ToInt32() + 8 * index
            Marshal.Copy(bytes, 0, IntPtr.Add(handle, offset), bytes.Length)
        End Sub

        Private Sub WriteBoolean(fieldName As String, value As Boolean)
            Dim bytes As Byte() = BitConverter.GetBytes(Convert.ToInt32(value))
            Dim offset As Integer = Marshal.OffsetOf(GetType(DevDesc), fieldName).ToInt32()
            Marshal.Copy(bytes, 0, IntPtr.Add(handle, offset), bytes.Length)
        End Sub

        Private Sub WriteInt32Enum(fieldName As String, value As [Enum])
            Dim bytes As Byte() = BitConverter.GetBytes(Convert.ToInt32(value))
            Dim offset As Integer = Marshal.OffsetOf(GetType(DevDesc), fieldName).ToInt32()
            Marshal.Copy(bytes, 0, IntPtr.Add(handle, offset), bytes.Length)
        End Sub

        Private Sub WriteColor(fieldName As String, value As Color)
            Dim bytes As Byte() = BitConverter.GetBytes(value.GetHashCode())
            Dim offset As Integer = Marshal.OffsetOf(GetType(DevDesc), fieldName).ToInt32()
            Marshal.Copy(bytes, 0, IntPtr.Add(handle, offset), bytes.Length)
        End Sub

        Private Sub WriteInt32(fieldName As String, value As Integer)
            Dim offset As Integer = Marshal.OffsetOf(GetType(DevDesc), fieldName).ToInt32()
            Marshal.WriteInt32(handle, offset, value)
        End Sub

        Private Sub WriteIntPtr(fieldName As String, value As IntPtr)
            Dim offset As Integer = Marshal.OffsetOf(GetType(DevDesc), fieldName).ToInt32()
            Marshal.WriteIntPtr(handle, offset, value)
        End Sub

        Protected Overrides Function ReleaseHandle() As Boolean
            Marshal.FreeHGlobal(handle)
            Return True
        End Function

        Friend Sub SetMethod(fieldName As String, d As [Delegate])
            WriteIntPtr(fieldName, Marshal.GetFunctionPointerForDelegate(d))
        End Sub
    End Class
End Namespace