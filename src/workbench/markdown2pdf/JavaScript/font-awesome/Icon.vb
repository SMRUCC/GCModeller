#Region "Microsoft.VisualBasic::e24067b9265d9e0486625f22b802ffbb, markdown2pdf\JavaScript\font-awesome\Icon.vb"

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

    ' Class Icon
    ' 
    '     Properties: Color, Name, Preview, Style
    ' 
    '     Constructor: (+2 Overloads) Sub New
    '     Function: getClassName, (+2 Overloads) ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Imaging

''' <summary>
''' fontawesome ver5
''' </summary>
Public Class Icon

    Public Property Style As Styles
    Public Property Name As String
    Public Property Color As Color

    Public ReadOnly Property Preview As String
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return ToString()
        End Get
    End Property

    Sub New(icon As icons, Optional style As Styles = Styles.Solid, Optional color As Color = Nothing)
        Call Me.New(icon.Description, style, color)
    End Sub

    Sub New(name$, Optional style As Styles = Styles.Solid, Optional color As Color = Nothing)
        Me.Name = name
        Me.Style = style
        Me.Color = color
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function ToString() As String
        If Color.IsEmpty Then
            Return (<i class=<%= getClassName() %>></i>).ToString
        Else
            Return ToString(style:=$"color:{Color.ToHtmlColor};")
        End If
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Function getClassName() As String
        Return Style.Description & " " & Strings.LCase(Name)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="style">With CSS style values</param>
    ''' <returns></returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overloads Function ToString(style As String) As String
        Return (<i class=<%= getClassName() %> style=<%= style %>></i>).ToString
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Operator &(html$, fa As Icon) As String
        Return html & fa.ToString
    End Operator

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Operator &(fa As Icon, html$) As String
        Return fa.ToString & html
    End Operator
End Class
