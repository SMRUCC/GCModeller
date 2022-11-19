#Region "Microsoft.VisualBasic::820a9e128112e8472b564dfbb1073cea, GCModeller\core\Bio.Assembly\Assembly\MetaCyc\File\AttributeValue DataFile\DataFiles\Parser\FileReader.vb"

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

    '   Total Lines: 81
    '    Code Lines: 47
    ' Comment Lines: 23
    '   Blank Lines: 11
    '     File Size: 3.12 KB


    '     Module FileReader
    ' 
    '         Function: GetData, GetDbProperty, TabularParser, TryParse
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Linq

Namespace Assembly.MetaCyc.File

    ''' <summary>
    ''' Database file reader of the metacyc database.
    ''' (MetaCyc数据库中的数据库文件的读取模块)
    ''' </summary>
    ''' <remarks></remarks>
    Public Module FileReader

        ''' <summary>
        ''' Try parse the data file.
        ''' </summary>
        ''' <param name="path"></param>
        ''' <param name="prop"></param>
        ''' <param name="objs"></param>
        ''' <returns>Returns error message</returns>
        Public Function TryParse(path As String, ByRef prop As [Property], ByRef objs As ObjectModel()) As String
            If Not path.FileExists Then
                prop = New [Property]
                objs = New ObjectModel() {}
                Return $"{path.ToFileURL} is not found on your file system!"
            End If

            Dim lines As String() = path.ReadAllLines

            prop = GetDbProperty(lines)
            objs = GetData(lines) _
                .Select(Function(array) ObjectModel.ModelParser(array)) _
                .ToArray

            Return ""
        End Function

        Public Function TabularParser(path As String, ByRef prop As [Property], ByRef lines As String(), ByRef first As String) As String
            If Not path.FileExists Then
                prop = New [Property]
                first = ""
                lines = New String() {}
                Return $"{path.ToFileURL} is not found on your file system!"
            End If

            lines = path.ReadAllLines
            first = lines.First
            prop = GetDbProperty(lines)
            lines = GetData(lines).ToArray.ToVector

            Return ""
        End Function

        ''' <summary>
        ''' 从数据库文件中的注释行获取属性值
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetDbProperty(buffer As String()) As [Property]
            Dim Comments As String() = (From Line As String In buffer
                                        Where Not String.IsNullOrEmpty(Line) AndAlso Line.Chars(Scan0) = "#"c
                                        Select Line).ToArray   '获取文件头部说明性的数据库文件属性
            Return [Property].CreateFrom(Comments)
        End Function

        ''' <summary>
        ''' Get the data text line
        ''' (获取非注释的文本行)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Iterator Function GetData(buffer As String()) As IEnumerable(Of String())
            Dim LQuery As String() = (From Line As String In buffer
                                      Where Not String.IsNullOrEmpty(Line) AndAlso Line.Chars(Scan0) <> "#"c
                                      Select Line).ToArray 'Select the text line that not is a comment line
            For Each block As String() In LQuery.Split("//")
                Yield block
            Next
        End Function
    End Module
End Namespace
