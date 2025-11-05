#Region "Microsoft.VisualBasic::a47f4f7002876fe97b350252f7ca9fda, visualize\SyntenyVisual\DeviceModel.vb"

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

    ' Class DeviceModel
    ' 
    '     Properties: Colors, DefaultColor, Margin, Meta, Orders
    '                 penWidth, PTT, Size, style, Titles
    ' 
    '     Function: GetColors, GetTitles, Template
    ' 
    ' Class Title
    ' 
    '     Properties: key, Title
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.FileIO
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Serialization.JSON

Public Class DeviceModel

    Public Property Margin As Size
    Public Property Size As Size
    Public Property Meta As String
    Public Property penWidth As Integer

    ''' <summary>
    ''' {基因组的名称, PTT的文件路径}
    ''' </summary>
    ''' <returns></returns>
    Public Property PTT As Dictionary(Of String, String)
    Public Property Orders As String()
    Public Property Titles As String
    ''' <summary>
    ''' <see cref="ColorMap"/>
    ''' </summary>
    ''' <returns></returns>
    Public Property Colors As String
    Public Property DefaultColor As String
    Public Property style As LineStyles

    Public Function GetColors(DIR As Directory) As ColorMgr
        Dim path As String

        If String.IsNullOrEmpty(Colors) Then
            path = DIR.GetFullPath("./colors.Csv")
        Else
            path = DIR.GetFullPath(Colors)
        End If

        Dim [default] As Color = DefaultColor.ToColor

        If Not path.FileExists Then
            Return New ColorMgr({}, [default])
        Else
            Return New ColorMgr(path.LoadCsv(Of ColorMap), [default])
        End If
    End Function

    Public Function GetTitles(DIR As Directory) As Title()
        Dim path As String

        If String.IsNullOrEmpty(Titles) Then
            path = DIR.GetFullPath("./titles.Csv")
        Else
            path = DIR.GetFullPath(Titles)
        End If

        If Not path.FileExists Then
            Return Nothing
        Else
            Return path.LoadCsv(Of Title)
        End If
    End Function

    Public Shared Function Template() As DeviceModel
        Return New DeviceModel With {
            .Size = New Size(1920, 1600),
            .Meta = "./bbh.Xml",
            .PTT = New Dictionary(Of String, String) From {
                {"xcb", "./Xanthomonas_campestris_8004_uid15.PTT"},
                {"xor", "./Xanthomonas_oryzae_oryzicola_BLS256_uid16740.PTT"}
            },
            .Margin = New Size(25, 25),
            .Orders = {"xcb", "xor"},
            .penWidth = 3,
            .DefaultColor = NameOf(Color.LightSkyBlue),
            .style = LineStyles.Polyline
        }
    End Function
End Class

''' <summary>
''' 由于这里是通过html来控制标题的显示格式的，所以在这里需要注意将文本里面的&lt;起始转义为&amp;lt;
''' </summary>
Public Class Title : Implements INamedValue

    Public Property key As String Implements INamedValue.Key
    ''' <summary>
    ''' 用来控制标题格式的html文本
    ''' </summary>
    ''' <returns></returns>
    Public Property Title As String

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class
