#Region "Microsoft.VisualBasic::b70ea160354626df7a8a5296f9ca3afc, GCModeller\visualize\ChromosomeMap\FootPrintMap\ColorProfiles.vb"

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


    ' Code Statistics:

    '   Total Lines: 70
    '    Code Lines: 44
    ' Comment Lines: 12
    '   Blank Lines: 14
    '     File Size: 2.47 KB


    '     Class DrawingDevice
    ' 
    '         Properties: Color, GraphicDevice, Image
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: ToString
    ' 
    '         Sub: (+2 Overloads) Dispose
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing

Namespace ComponentModel

    Public Class DrawingDevice : Implements IDisposable

        Dim _GraphicDevice As Graphics, _ImageData As Bitmap
        Dim _ColorProfiles As ColorProfiles

        Public ReadOnly Property GraphicDevice As Graphics
            Get
                Return _GraphicDevice
            End Get
        End Property

        Public ReadOnly Property Image As Bitmap
            Get
                Return _ImageData
            End Get
        End Property

        Public ReadOnly Property Color(Name As String) As Color
            Get
                Return _ColorProfiles(Name)
            End Get
        End Property

        Sub New(Width As Integer, Height As Integer, ColorProfiles As Generic.IEnumerable(Of String), Optional DefaultColor As Color = Nothing)
            _ColorProfiles = New ColorProfiles(ColorProfiles, DefaultColor)
            _ImageData = New Bitmap(Width, Height)
            _GraphicDevice = Graphics.FromImage(_ImageData)
        End Sub

        Public Overrides Function ToString() As String
            Return String.Format("<{0}, {1}>", _ImageData.Width, _ImageData.Height)
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose( disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose( disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            System.GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace
