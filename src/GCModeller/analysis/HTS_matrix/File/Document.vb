#Region "Microsoft.VisualBasic::0743e2db27b2b6ae69cc67311c898d1d, analysis\HTS_matrix\Document.vb"

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

    ' Module Document
    ' 
    '     Function: LoadMatrixDocument, SaveMatrix
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Text

Public Module Document

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="excludes"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' 因为矩阵文档是由数字构成的，所以在这里不再使用csv文件解析器来完成，直接通过分隔符进行解析来获取最好的解析性能
    ''' </remarks>
    Public Function LoadMatrixDocument(file As String, excludes As Index(Of String)) As Matrix
        Dim text As String() = file.LineIterators(strictFile:=True).ToArray
        Dim sampleIds As String() = text(Scan0) _
            .Split(ASCII.TAB, ","c) _
            .Skip(1) _
            .Select(Function(s) s.Trim(""""c, " "c)) _
            .ToArray
        Dim takeIndex As Integer()

        If excludes Is Nothing Then
            takeIndex = sampleIds.Sequence.ToArray
        Else
            takeIndex = sampleIds _
                .Select(Function(name, i) (i, Not name Like excludes)) _
                .Where(Function(a) a.Item2 = True) _
                .Select(Function(a) a.i) _
                .ToArray
        End If

        Dim matrix As DataFrameRow() = text _
            .Skip(1) _
            .loadGeneMatrix(excludes, takeIndex) _
            .ToArray

        Return New Matrix With {
            .expression = matrix,
            .sampleID = takeIndex _
                .Select(Function(i) sampleIds(i)) _
                .ToArray,
            .tag = file.FileName
        }
    End Function

    <Extension>
    Public Function SaveMatrix(mat As Matrix, file As String, Optional idcolName As String = "geneID") As Boolean
        Using table As StreamWriter = file.OpenWriter(Encodings.UTF8WithoutBOM)
            Dim line As String = {idcolName}.Join(mat.sampleID).JoinBy(",")

            Call table.WriteLine(line)

            For Each gene As DataFrameRow In mat.expression
                line = New String() {$"""{gene.geneID}"""} _
                    .Join(gene.experiments.Select(Function(d) d.ToString)) _
                    .JoinBy(",")

                Call table.WriteLine(line)
            Next
        End Using

        Return 0
    End Function

End Module
