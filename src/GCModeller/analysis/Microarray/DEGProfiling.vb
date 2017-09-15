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
