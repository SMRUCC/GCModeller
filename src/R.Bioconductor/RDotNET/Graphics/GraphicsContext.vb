#Region "Microsoft.VisualBasic::5e4cae40ea3e26eb26da4e4c51010951, RDotNET\Graphics\GraphicsContext.vb"

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

    '     Class GraphicsContext
    ' 
    '         Properties: Background, CharacterExpansion, FontFace, FontFamily, FontSizeInPoints
    '                     Foreground, Gamma, IsInvalid, LineEnd, LineHeight
    '                     LineJoin, LineMitre, LineType, LineWidth
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetContext, ReleaseHandle
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports RDotNet.Graphics.Internals
Imports System.Runtime.InteropServices

Namespace Graphics

    Public Class GraphicsContext
        Inherits SafeHandle
        Friend Sub New(pointer As IntPtr)
            MyBase.New(IntPtr.Zero, True)
            SetHandle(pointer)
        End Sub

        Public Overrides ReadOnly Property IsInvalid() As Boolean
            Get
                Return handle = IntPtr.Zero
            End Get
        End Property

        Public ReadOnly Property Foreground() As Color
            Get
                Return GetContext().col
            End Get
        End Property

        Public ReadOnly Property Background() As Color
            Get
                Return GetContext().fill
            End Get
        End Property

        Public ReadOnly Property Gamma() As Double
            Get
                Return GetContext().gamma
            End Get
        End Property

        Public ReadOnly Property LineWidth() As Double
            Get
                Return GetContext().lwd
            End Get
        End Property

        Public ReadOnly Property LineType() As LineType
            Get
                Return GetContext().lty
            End Get
        End Property

        Public ReadOnly Property LineEnd() As LineEnd
            Get
                Return GetContext().lend
            End Get
        End Property

        Public ReadOnly Property LineJoin() As LineJoin
            Get
                Return GetContext().ljoin
            End Get
        End Property

        Public ReadOnly Property LineMitre() As Double
            Get
                Return GetContext().lmitre
            End Get
        End Property

        Public ReadOnly Property CharacterExpansion() As Double
            Get
                Return GetContext().cex
            End Get
        End Property

        Public ReadOnly Property FontSizeInPoints() As Double
            Get
                Return GetContext().ps
            End Get
        End Property

        Public ReadOnly Property LineHeight() As Double
            Get
                Return GetContext().lineheight
            End Get
        End Property

        Public ReadOnly Property FontFace() As FontFace
            Get
                Return GetContext().fontface
            End Get
        End Property

        Public ReadOnly Property FontFamily() As String
            Get
                Return GetContext().fontfamily
            End Get
        End Property

        Protected Overrides Function ReleaseHandle() As Boolean
            Return True
        End Function

        Private Function GetContext() As GEcontext
            Return CType(Marshal.PtrToStructure(handle, GetType(GEcontext)), GEcontext)
        End Function
    End Class
End Namespace
