#Region "Microsoft.VisualBasic::6b1972db78f25bc53af95c86a89f1444, RDotNET\Graphics\DeviceDescription.vb"

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

    '     Class DeviceDescription
    ' 
    '         Properties: Adjustment, Bounds, CharacterSizeInRasterX, CharacterSizeInRasterY, CharOffsetX
    '                     CharOffsetY, ClipBounds, DeviceSpecific, DisplayListOn, Gamma
    '                     InchesPerRasterX, InchesPerRasterY, IsClippable, IsGammaModifiable, IsInvalid
    '                     IsTextRotatedInContour, LineBiasY, StartBackground, StartFont, StartFontSize
    '                     StartForeground, StartGamma, StartLineType
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: ReleaseHandle
    ' 
    '         Sub: SetDefaultParameter, SetMethod, WriteBoolean, WriteColor, WriteDouble
    '              WriteDoubleArray, WriteInt32, WriteInt32Enum, WriteIntPtr
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports RDotNet.Graphics.Internals
Imports System.Runtime.InteropServices

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

        Public Overrides ReadOnly Property IsInvalid() As Boolean
            Get
                Return handle = IntPtr.Zero
            End Get
        End Property

        Public Property Bounds() As Rectangle
            Get
                Dim dd = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return New Rectangle(dd.left, dd.bottom, dd.right - dd.left, dd.top - dd.bottom)
            End Get
            Set
                WriteDouble("left", value.Left)
                WriteDouble("right", value.Right)
                WriteDouble("bottom", value.Bottom)
                WriteDouble("top", value.Top)
            End Set
        End Property

        Public Property ClipBounds() As Rectangle
            Get
                Dim dd = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return New Rectangle(dd.clipLeft, dd.clipBottom, dd.clipRight - dd.clipLeft, dd.clipTop - dd.clipBottom)
            End Get
            Set
                WriteDouble("clipLeft", value.Left)
                WriteDouble("clipRight", value.Right)
                WriteDouble("clipBottom", value.Bottom)
                WriteDouble("clipTop", value.Top)
            End Set
        End Property

        Public Property CharOffsetX() As Double
            Get
                Dim dd = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.xCharOffset
            End Get
            Set
                WriteDouble("xCharOffset", value)
            End Set
        End Property

        Public Property CharOffsetY() As Double
            Get
                Dim dd = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.yCharOffset
            End Get
            Set
                WriteDouble("yCharOffset", value)
            End Set
        End Property

        Public Property LineBiasY() As Double
            Get
                Dim dd = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.yLineBias
            End Get
            Set
                WriteDouble("yLineBias", value)
            End Set
        End Property

        Public Property InchesPerRasterX() As Double
            Get
                Dim dd = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.ipr(0)
            End Get
            Set
                WriteDoubleArray("ipr", 0, value)
            End Set
        End Property

        Public Property InchesPerRasterY() As Double
            Get
                Dim dd = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.ipr(1)
            End Get
            Set
                WriteDoubleArray("ipr", 1, value)
            End Set
        End Property

        Public Property CharacterSizeInRasterX() As Double
            Get
                Dim dd = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.cra(0)
            End Get
            Set
                WriteDoubleArray("cra", 0, value)
            End Set
        End Property

        Public Property CharacterSizeInRasterY() As Double
            Get
                Dim dd = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.cra(1)
            End Get
            Set
                WriteDoubleArray("cra", 1, value)
            End Set
        End Property

        Public Property Gamma() As Double
            Get
                Dim dd = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.gamma
            End Get
            Set
                WriteDouble("gamma", value)
            End Set
        End Property

        Public Property IsGammaModifiable() As Boolean
            Get
                Dim dd = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.canChangeGamma
            End Get
            Set
                WriteBoolean("canChangeGamma", value)
            End Set
        End Property

        Public Property IsClippable() As Boolean
            Get
                Dim dd = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.canClip
            End Get
            Set
                WriteBoolean("canClip", value)
            End Set
        End Property

        Public Property Adjustment() As Adjustment
            Get
                Dim dd = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.canHAdj
            End Get
            Set
                WriteInt32Enum("canHAdj", value)
            End Set
        End Property

        Public Property StartFontSize() As Double
            Get
                Dim dd = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.startps
            End Get
            Set
                WriteDouble("startps", value)
            End Set
        End Property

        Public Property StartForeground() As Color
            Get
                Dim dd = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.startcol
            End Get
            Set
                WriteColor("startcol", value)
            End Set
        End Property

        Public Property StartBackground() As Color
            Get
                Dim dd = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.startfill
            End Get
            Set
                WriteColor("startfill", value)
            End Set
        End Property

        Public Property StartLineType() As LineType
            Get
                Dim dd = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.startlty
            End Get
            Set
                WriteInt32Enum("startlty", value)
            End Set
        End Property

        Public Property StartFont() As Integer
            Get
                Dim dd = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.startfont
            End Get
            Set
                WriteInt32("startfont", value)
            End Set
        End Property

        Public Property StartGamma() As Double
            Get
                Dim dd = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.startgamma
            End Get
            Set
                WriteDouble("startgamma", value)
            End Set
        End Property

        Public Property DisplayListOn() As Boolean
            Get
                Dim dd = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.displayListOn
            End Get
            Set
                WriteBoolean("displayListOn", value)
            End Set
        End Property

        Public Property IsTextRotatedInContour() As Boolean
            Get
                Dim dd = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.useRotatedTextInContour
            End Get
            Set
                WriteBoolean("useRotatedTextInContour", value)
            End Set
        End Property

        Protected Property DeviceSpecific() As IntPtr
            Get
                Dim dd = CType(Marshal.PtrToStructure(handle, GetType(DevDesc)), DevDesc)
                Return dd.deviceSpecific
            End Get
            Set
                WriteIntPtr("deviceSpecific", value)
            End Set
        End Property

        Private Sub SetDefaultParameter()
            StartForeground = Colors.Black
            StartBackground = Colors.White
            StartLineType = LineType.Solid
            StartFont = 1
            StartFontSize = 14.0
            StartGamma = 1.0
            CharOffsetX = 0.49
            CharOffsetY = 0.3333
            CharacterSizeInRasterX = StartFontSize * 0.9
            CharacterSizeInRasterY = StartFontSize * 1.2
            InchesPerRasterX = 1.0 / 72.0
            InchesPerRasterY = 1.0 / 72.0
            LineBiasY = 0.2
            IsClippable = True
            Adjustment = Adjustment.None
            IsGammaModifiable = False
            DisplayListOn = False
        End Sub

        Private Sub WriteDouble(fieldName As String, value As Double)
            Dim bytes = BitConverter.GetBytes(value)
            Dim offset = Marshal.OffsetOf(GetType(DevDesc), fieldName).ToInt32()
            Marshal.Copy(bytes, 0, IntPtr.Add(handle, offset), bytes.Length)
        End Sub

        Private Sub WriteDoubleArray(fieldName As String, index As Integer, value As Double)
            Dim bytes = BitConverter.GetBytes(value)
            Dim offset = Marshal.OffsetOf(GetType(DevDesc), fieldName).ToInt32() + 8 * index
            Marshal.Copy(bytes, 0, IntPtr.Add(handle, offset), bytes.Length)
        End Sub

        Private Sub WriteBoolean(fieldName As String, value As Boolean)
            Dim bytes = BitConverter.GetBytes(Convert.ToInt32(value))
            Dim offset = Marshal.OffsetOf(GetType(DevDesc), fieldName).ToInt32()
            Marshal.Copy(bytes, 0, IntPtr.Add(handle, offset), bytes.Length)
        End Sub

        Private Sub WriteInt32Enum(fieldName As String, value As [Enum])
            Dim bytes = BitConverter.GetBytes(Convert.ToInt32(value))
            Dim offset = Marshal.OffsetOf(GetType(DevDesc), fieldName).ToInt32()
            Marshal.Copy(bytes, 0, IntPtr.Add(handle, offset), bytes.Length)
        End Sub

        Private Sub WriteColor(fieldName As String, value As Color)
            Dim bytes = BitConverter.GetBytes(value.GetHashCode())
            Dim offset = Marshal.OffsetOf(GetType(DevDesc), fieldName).ToInt32()
            Marshal.Copy(bytes, 0, IntPtr.Add(handle, offset), bytes.Length)
        End Sub

        Private Sub WriteInt32(fieldName As String, value As Integer)
            Dim offset = Marshal.OffsetOf(GetType(DevDesc), fieldName).ToInt32()
            Marshal.WriteInt32(handle, offset, value)
        End Sub

        Private Sub WriteIntPtr(fieldName As String, value As IntPtr)
            Dim offset = Marshal.OffsetOf(GetType(DevDesc), fieldName).ToInt32()
            Marshal.WriteIntPtr(handle, offset, value)
        End Sub

        Protected Overrides Function ReleaseHandle() As Boolean
            Marshal.FreeHGlobal(handle)
            Return True
        End Function

        Friend Sub SetMethod(fieldName As String, d As [Delegate])
            Dim pointer = Marshal.GetFunctionPointerForDelegate(d)
            WriteIntPtr(fieldName, pointer)
        End Sub
    End Class
End Namespace
