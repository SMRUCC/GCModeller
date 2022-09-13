#Region "Microsoft.VisualBasic::89305fa9d44d117be96106d05e2396b5, GCModeller\analysis\Microarray\DEGProfiling.vb"

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

    '   Total Lines: 85
    '    Code Lines: 59
    ' Comment Lines: 14
    '   Blank Lines: 12
    '     File Size: 3.32 KB


    ' Module DEGProfiling
    ' 
    '     Function: ColorsProfiling, createTable, GetDEGs
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports gene = Microsoft.VisualBasic.Data.csv.IO.EntityObject

Public Module DEGProfiling

    ' 2017-9-15
    '
    ' 原来在这个模块之中的每一个和DEP判断相关的函数都具有一个threshold参数用来判断是否上下调
    ' 但是这个判断已经和isDEP函数判断重复了，所以现在将这些threshold参数都去除掉

    <Extension>
    Public Function GetDEGs(genes As IEnumerable(Of gene),
                            isDEP As Func(Of gene, Boolean),
                            log2FC$) As (UP As Dictionary(Of String, Double), DOWN As Dictionary(Of String, Double))

        Dim DEGs As gene() = genes.Where(isDEP).ToArray
        Dim up = DEGs.Where(Function(gene) Val(gene(log2FC)) > 0).createTable(log2FC)
        Dim down = DEGs.Where(Function(gene) Val(gene(log2FC)) < 0).createTable(log2FC)

        Return (up, down)
    End Function

    <Extension>
    Private Function createTable(DEGs As IEnumerable(Of gene), logFC$) As Dictionary(Of String, Double)
        Return DEGs _
            .GroupBy(Function(gene) gene.ID) _
            .ToDictionary(Function(gene) gene.Key,
                          Function(g)
                              Return Aggregate gene As gene
                                     In g
                                     Let log2FC As Double = Val(gene(logFC))
                                     Into Average(log2FC)
                          End Function)
    End Function

    ''' <summary>
    ''' 生成DEG的颜色
    ''' </summary>
    ''' <param name="genes"></param>
    ''' <param name="isDEP"></param>
    ''' <param name="log2FC$"></param>
    ''' <param name="IDMapping"></param>
    ''' <param name="upColor$"></param>
    ''' <param name="downColor$"></param>
    ''' <returns></returns>
    <Extension>
    Public Function ColorsProfiling(genes As IEnumerable(Of gene),
                                    isDEP As Func(Of gene, Boolean),
                                    log2FC$,
                                    Optional IDMapping As Dictionary(Of String, String()) = Nothing,
                                    Optional upColor$ = "red",
                                    Optional downColor$ = "blue") As Dictionary(Of String, String)

        Dim mapID As Func(Of String, String())

        If IDMapping.IsNullOrEmpty Then
            mapID = Function(id) {id}
        Else
            mapID = Function(id)
                        If IDMapping.ContainsKey(id) Then
                            Return IDMapping(id)
                        Else
                            Return {id}
                        End If
                    End Function
        End If

        Dim profiles As New Dictionary(Of String, String)

        With genes.GetDEGs(isDEP, log2FC)
            For Each gene As String In .UP.Keys
                For Each ID In mapID(gene)
                    profiles(ID) = upColor
                Next
            Next
            For Each gene As String In .DOWN.Keys
                For Each ID In mapID(gene)
                    profiles(ID) = downColor
                Next
            Next
        End With

        Return profiles
    End Function
End Module
