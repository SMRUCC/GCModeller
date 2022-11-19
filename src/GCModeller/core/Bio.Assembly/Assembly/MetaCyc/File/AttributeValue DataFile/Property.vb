#Region "Microsoft.VisualBasic::5c334bee80851a7a6a3ea2bfe6e2cfb3, GCModeller\core\Bio.Assembly\Assembly\MetaCyc\File\AttributeValue DataFile\Property.vb"

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

    '   Total Lines: 86
    '    Code Lines: 62
    ' Comment Lines: 12
    '   Blank Lines: 12
    '     File Size: 3.46 KB


    '     Class [Property]
    ' 
    '         Properties: Authors, Database, FileName, GeneratedDate, Species
    '                     Version
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: CreateFrom, Generate, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Text.RegularExpressions

Namespace Assembly.MetaCyc.File

    ''' <summary>
    ''' The summary database property text content in the attribute-value data file head line. 
    ''' (在键值对的数据文件头部的说明性的数据库文件属性)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class [Property]

        Public Property Authors As String()
        Public Property Species As String
        Public Property Database As String
        Public Property Version As String
        Public Property FileName As String
        Public Property GeneratedDate As String

        Public Function Generate() As String
            Dim sBuilder As StringBuilder = New StringBuilder(1024)
            sBuilder.AppendLine("# Authors:")
            For Each Author As String In Authors
                sBuilder.AppendLine("#    " & Author)
            Next
            sBuilder.AppendLine("# Species: " & Species)
            sBuilder.AppendLine("# Database: " & Database)
            sBuilder.AppendLine("# Version: " & Version)
            sBuilder.AppendLine("# File Name: " & FileName)
            sBuilder.AppendLine("# Date and time generated: " & GeneratedDate)

            Return sBuilder.ToString
        End Function

        ''' <summary>
        ''' 从文本行中创建
        ''' </summary>
        ''' <param name="data"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CreateFrom(data As String()) As [Property]
            Dim this As New [Property]
            Dim e As String = ContactLines(data)
            Dim Match As Match
            Dim p As Integer, [Next] As Integer
            Dim chunkBuffer() As String

            Match = Regex.Match(e, "# Database: .+", RegexOptions.Multiline)
            If Match.Success Then this.Database = Match.Value.Substring(12).Trim
            Match = Regex.Match(e, "# Version: .+", RegexOptions.Multiline)
            If Match.Success Then this.Version = Match.Value.Substring(11).Trim
            Match = Regex.Match(e, "# Species: .+", RegexOptions.Multiline)
            If Match.Success Then this.Species = Match.Value.Substring(11).Trim
            Match = Regex.Match(e, "# File Name: .+", RegexOptions.Multiline)
            If Match.Success Then this.FileName = Match.Value.Substring(13).Trim
            Match = Regex.Match(e, "# Date and time generated: .+", RegexOptions.Multiline)
            If Match.Success Then this.GeneratedDate = Match.Value.Substring(27).Trim

            [Next] = Array.IndexOf(data, "# Authors:") + 1
            For i As Integer = [Next] To data.Length - 1
                If String.Compare(data(i), "#") Then
                    p += 1
                Else
                    ReDim chunkBuffer(p - 1)
                    Call Array.ConstrainedCopy(data, [Next], chunkBuffer, Scan0, p)
                    For j As Integer = 0 To chunkBuffer.Length - 1
                        chunkBuffer(j) = chunkBuffer(j).Substring(1).Trim
                    Next
                    this.Authors = chunkBuffer

                    Exit For
                End If
            Next

            Return this
        End Function

        Friend Sub New()
            'Do NOTHING
        End Sub

        Public Overrides Function ToString() As String
            Return String.Format("{0} --> {1}", Species, FileName)
        End Function
    End Class
End Namespace
