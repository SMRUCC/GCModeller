Imports System.Runtime.InteropServices
Imports RDotNet.Graphics.Internals

Namespace Graphics
    Public Class DeviceDescription
        Inherits SafeHandle

        Public Sub New()
            MyBase.New(IntPtr.Zero, True)
            Dim pointer = Marshal.AllocHGlobal(Marshal.SizeOf(GetType(DevDesc)))
            Marshal.StructureToPtr(New DevDesc(), pointer, True)
            SetHandle(pointer)
            SetDefaultParameter()
        End Sub

        Public Overrides ReadOnly Property IsInvalid As Boolean
            Get
                Return handle = IntPtr.Zero
            End Get
        End Property

        Public Property Bounds As Rectangle
            Get
                Dim dd = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return New Rectangle(dd.left, dd.bottom, dd.right - dd.left, dd.top - dd.bottom)
            End Get
            Set(ByVal value As Rectangle)
                WriteDouble("left", value.Left)
                WriteDouble("right", value.Right)
                WriteDouble("bottom", value.Bottom)
                WriteDouble("top", value.Top)
            End Set
        End Property

        Public Property ClipBounds As Rectangle
            Get
                Dim dd = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return New Rectangle(dd.clipLeft, dd.clipBottom, dd.clipRight - dd.clipLeft, dd.clipTop - dd.clipBottom)
            End Get
            Set(ByVal value As Rectangle)
                WriteDouble("clipLeft", value.Left)
                WriteDouble("clipRight", value.Right)
                WriteDouble("clipBottom", value.Bottom)
                WriteDouble("clipTop", value.Top)
            End Set
        End Property

        Public Property CharOffsetX As Double
            Get
                Dim dd = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.xCharOffset
            End Get
            Set(ByVal value As Double)
                WriteDouble("xCharOffset", value)
            End Set
        End Property

        Public Property CharOffsetY As Double
            Get
                Dim dd = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.yCharOffset
            End Get
            Set(ByVal value As Double)
                WriteDouble("yCharOffset", value)
            End Set
        End Property

        Public Property LineBiasY As Double
            Get
                Dim dd = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.yLineBias
            End Get
            Set(ByVal value As Double)
                WriteDouble("yLineBias", value)
            End Set
        End Property

        Public Property InchesPerRasterX As Double
            Get
                Dim dd = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.ipr(0)
            End Get
            Set(ByVal value As Double)
                WriteDoubleArray("ipr", 0, value)
            End Set
        End Property

        Public Property InchesPerRasterY As Double
            Get
                Dim dd = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.ipr(1)
            End Get
            Set(ByVal value As Double)
                WriteDoubleArray("ipr", 1, value)
            End Set
        End Property

        Public Property CharacterSizeInRasterX As Double
            Get
                Dim dd = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.cra(0)
            End Get
            Set(ByVal value As Double)
                WriteDoubleArray("cra", 0, value)
            End Set
        End Property

        Public Property CharacterSizeInRasterY As Double
            Get
                Dim dd = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.cra(1)
            End Get
            Set(ByVal value As Double)
                WriteDoubleArray("cra", 1, value)
            End Set
        End Property

        Public Property Gamma As Double
            Get
                Dim dd = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.gamma
            End Get
            Set(ByVal value As Double)
                WriteDouble("gamma", value)
            End Set
        End Property

        Public Property IsGammaModifiable As Boolean
            Get
                Dim dd = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.canChangeGamma
            End Get
            Set(ByVal value As Boolean)
                WriteBoolean("canChangeGamma", value)
            End Set
        End Property

        Public Property IsClippable As Boolean
            Get
                Dim dd = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.canClip
            End Get
            Set(ByVal value As Boolean)
                WriteBoolean("canClip", value)
            End Set
        End Property

        Public Property Adjustment As Adjustment
            Get
                Dim dd = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.canHAdj
            End Get
            Set(ByVal value As Adjustment)
                WriteInt32Enum("canHAdj", value)
            End Set
        End Property

        Public Property StartFontSize As Double
            Get
                Dim dd = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.startps
            End Get
            Set(ByVal value As Double)
                WriteDouble("startps", value)
            End Set
        End Property

        Public Property StartForeground As Color
            Get
                Dim dd = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.startcol
            End Get
            Set(ByVal value As Color)
                WriteColor("startcol", value)
            End Set
        End Property

        Public Property StartBackground As Color
            Get
                Dim dd = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.startfill
            End Get
            Set(ByVal value As Color)
                WriteColor("startfill", value)
            End Set
        End Property

        Public Property StartLineType As LineType
            Get
                Dim dd = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.startlty
            End Get
            Set(ByVal value As LineType)
                WriteInt32Enum("startlty", value)
            End Set
        End Property

        Public Property StartFont As Integer
            Get
                Dim dd = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.startfont
            End Get
            Set(ByVal value As Integer)
                WriteInt32("startfont", value)
            End Set
        End Property

        Public Property StartGamma As Double
            Get
                Dim dd = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.startgamma
            End Get
            Set(ByVal value As Double)
                WriteDouble("startgamma", value)
            End Set
        End Property

        Public Property DisplayListOn As Boolean
            Get
                Dim dd = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.displayListOn
            End Get
            Set(ByVal value As Boolean)
                WriteBoolean("displayListOn", value)
            End Set
        End Property

        Public Property IsTextRotatedInContour As Boolean
            Get
                Dim dd = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.useRotatedTextInContour
            End Get
            Set(ByVal value As Boolean)
                WriteBoolean("useRotatedTextInContour", value)
            End Set
        End Property

        Protected Property DeviceSpecific As IntPtr
            Get
                Dim dd = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.deviceSpecific
            End Get
            Set(ByVal value As IntPtr)
                WriteIntPtr("deviceSpecific", value)
            End Set
        End Property

        Private Sub SetDefaultParameter()
            StartForeground = Black
            StartBackground = White
            StartLineType = LineType.Solid
            StartFont = 1
            StartFontSize = 14.0
            StartGamma = 1.0
            CharOffsetX = 0.4900
            CharOffsetY = 0.3333
            CharacterSizeInRasterX = StartFontSize * 0.9
            CharacterSizeInRasterY = StartFontSize * 1.2
            InchesPerRasterX = 1.0 / 72.0
            InchesPerRasterY = 1.0 / 72.0
            LineBiasY = 0.20
            IsClippable = True
            Adjustment = Adjustment.None
            IsGammaModifiable = False
            DisplayListOn = False
        End Sub

        Private Sub WriteDouble(ByVal fieldName As String, ByVal value As Double)
            Dim bytes = BitConverter.GetBytes(value)
            Dim offset = Marshal.OffsetOf(GetType(DevDesc), fieldName).ToInt32()
            Marshal.Copy(bytes, 0, IntPtr.Add(handle, offset), bytes.Length)
        End Sub

        Private Sub WriteDoubleArray(ByVal fieldName As String, ByVal index As Integer, ByVal value As Double)
            Dim bytes = BitConverter.GetBytes(value)
            Dim offset = Marshal.OffsetOf((GetType(RDotNet.Graphics.Internals.DevDesc)), (fieldName)).ToInt32() + Marshal.SizeOf(GetType(Double)) * index

            Marshal.Copy(bytes, 0, IntPtr.Add(handle, offset), bytes.Length)
        End Sub

        Private Sub WriteBoolean(ByVal fieldName As String, ByVal value As Boolean)
            Dim bytes = BitConverter.GetBytes(Convert.ToInt32(value))
            Dim offset = Marshal.OffsetOf(GetType(DevDesc), fieldName).ToInt32()
            Marshal.Copy(bytes, 0, IntPtr.Add(handle, offset), bytes.Length)
        End Sub

        Private Sub WriteInt32Enum(ByVal fieldName As String, ByVal value As [Enum])
            Dim bytes = BitConverter.GetBytes(Convert.ToInt32(value))
            Dim offset = Marshal.OffsetOf(GetType(DevDesc), fieldName).ToInt32()
            Marshal.Copy(bytes, 0, IntPtr.Add(handle, offset), bytes.Length)
        End Sub

        Private Sub WriteColor(ByVal fieldName As String, ByVal value As Color)
            Dim bytes = BitConverter.GetBytes(value.GetHashCode())
            Dim offset = Marshal.OffsetOf(GetType(DevDesc), fieldName).ToInt32()
            Marshal.Copy(bytes, 0, IntPtr.Add(handle, offset), bytes.Length)
        End Sub

        Private Sub WriteInt32(ByVal fieldName As String, ByVal value As Integer)
            Dim offset = Marshal.OffsetOf(GetType(DevDesc), fieldName).ToInt32()
            Marshal.WriteInt32(handle, offset, value)
        End Sub

        Private Sub WriteIntPtr(ByVal fieldName As String, ByVal value As IntPtr)
            Dim offset = Marshal.OffsetOf(GetType(DevDesc), fieldName).ToInt32()
            Marshal.WriteIntPtr(handle, offset, value)
        End Sub

        Protected Overrides Function ReleaseHandle() As Boolean
            Marshal.FreeHGlobal(handle)
            Return True
        End Function

        Friend Sub SetMethod(ByVal fieldName As String, ByVal d As [Delegate])
            Dim pointer = Marshal.GetFunctionPointerForDelegate(d)
            WriteIntPtr(fieldName, pointer)
        End Sub
    End Class
End Namespace
