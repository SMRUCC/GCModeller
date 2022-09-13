#Region "Microsoft.VisualBasic::252ff3ecb8dd9fb2cff5f8c198dd4ac4, GCModeller\core\Bio.Annotation\PTF\Document\PtfParser.vb"

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

    '   Total Lines: 76
    '    Code Lines: 58
    ' Comment Lines: 10
    '   Blank Lines: 8
    '     File Size: 3.05 KB


    '     Module PtfParser
    ' 
    '         Function: IterateAnnotations, ParseAnnotation, ParseDocument
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Values
Imports Microsoft.VisualBasic.Text

Namespace Ptf.Document

    Friend Module PtfParser

        ''' <summary>
        ''' 读取小文件
        ''' </summary>
        ''' <param name="file"></param>
        ''' <returns></returns>
        Public Function ParseDocument(file As StreamReader) As PtfFile
            Dim lines As String() = file.ReadToEnd.LineTokens
            Dim headers As String() = lines.TakeWhile(Function(a) a.StartsWith("#")).ToArray
            Dim attributes As Dictionary(Of String, String()) = headers _
                .Select(Function(line)
                            Return line.Trim("#"c, " "c).GetTagValue("=")
                        End Function) _
                .GroupBy(Function(a) a.Name) _
                .ToDictionary(Function(a) a.Key,
                              Function(a)
                                  Return a.Select(Function(p) p.Value).ToArray
                              End Function)

            Return New PtfFile With {
                .attributes = attributes,
                .proteins = lines _
                    .Skip(headers.Length) _
                    .Where(Function(line) Not line.StringEmpty) _
                    .Select(AddressOf ParseAnnotation) _
                    .ToArray
            }
        End Function

        ''' <summary>
        ''' 这个函数主要是用于读取大文件
        ''' </summary>
        ''' <returns></returns>
        Public Iterator Function IterateAnnotations(file As Stream) As IEnumerable(Of ProteinAnnotation)
            Using reader As New StreamReader(file, Encoding.UTF8, False, App.BufferSize)
                Dim line As Value(Of String) = ""

                Do While Not reader.EndOfStream
                    If (line = reader.ReadLine) = String.Empty OrElse line.StartsWith("#") Then
                        ' 跳过文件头
                    Else
                        Yield ParseAnnotation(line)
                    End If
                Loop
            End Using
        End Function

        Public Function ParseAnnotation(line As String) As ProteinAnnotation
            Dim tokens As String() = line.Split(ASCII.TAB)
            Dim attrs As Dictionary(Of String, String()) = tokens(4) _
                .StringSplit(";\s+") _
                .Select(Function(t) t.GetTagValue(":", trim:=True)) _
                .ToDictionary(Function(a) a.Name,
                              Function(a)
                                  Return a.Value.Split(","c)
                              End Function)

            Return New ProteinAnnotation With {
                .geneId = tokens(Scan0),
                .locus_id = tokens(1),
                .geneName = tokens(2),
                .description = tokens(3),
                .attributes = attrs
            }
        End Function
    End Module
End Namespace
