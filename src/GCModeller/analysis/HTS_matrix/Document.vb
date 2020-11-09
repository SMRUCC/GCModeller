#Region "Microsoft.VisualBasic::5ca02a0a31d09900f0e6d0f79d14d053, analysis\HTS_matrix\Document.vb"

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
    '     Function: LoadMatrixDocument
    ' 
    ' /********************************************************************************/

#End Region

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
        Dim text As String() = file.LineIterators.ToArray
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
            .Select(Function(line)
                        Dim tokens = line.Split(ASCII.TAB, ","c)
                        Dim data As Double() = tokens _
                            .Skip(1) _
                            .Select(AddressOf Val) _
                            .ToArray

                        If Not excludes Is Nothing Then
                            data = takeIndex _
                                .Select(Function(i) data(i)) _
                                .ToArray
                        End If

                        Return New DataFrameRow With {
                            .experiments = data,
                            .geneID = tokens(Scan0).Trim(""""c, " "c)
                        }
                    End Function) _
            .ToArray

        Return New Matrix With {
            .expression = matrix,
            .sampleID = takeIndex _
                .Select(Function(i) sampleIds(i)) _
                .ToArray
        }
    End Function
End Module
