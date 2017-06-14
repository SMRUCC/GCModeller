Imports System.Runtime.CompilerServices
Imports gene = Microsoft.VisualBasic.Data.csv.IO.EntityObject

Public Module DEGProfiling

    ''' <summary>
    ''' 生成DEG的颜色
    ''' </summary>
    ''' <param name="genes"></param>
    ''' <param name="isDEP"></param>
    ''' <param name="threshold"></param>
    ''' <param name="logFC$"></param>
    ''' <param name="IDMapping"></param>
    ''' <param name="upColor$"></param>
    ''' <param name="downColor$"></param>
    ''' <returns></returns>
    <Extension>
    Public Function ColorsProfiling(genes As IEnumerable(Of gene),
                                    isDEP As Func(Of gene, Boolean),
                                    threshold As (up#, down#),
                                    logFC$,
                                    Optional IDMapping As Dictionary(Of String, String) = Nothing,
                                    Optional upColor$ = "red",
                                    Optional downColor$ = "blue") As Dictionary(Of String, String)

        Dim DEGs As gene() = genes.Where(isDEP).ToArray
        Dim mapID As Func(Of String, String)

        If IDMapping.IsNullOrEmpty Then
            mapID = Function(id) id
        Else
            mapID = Function(id)
                        If IDMapping.ContainsKey(id) Then
                            Return IDMapping(id)
                        Else
                            Return id
                        End If
                    End Function
        End If

        Dim profiles As New Dictionary(Of String, String)

        For Each gene As gene In DEGs
            Dim FC# = Val(gene(logFC))

            If FC >= threshold.up Then
                profiles(mapID(gene.ID)) = upColor
            ElseIf FC <= threshold.down Then
                profiles(mapID(gene.ID)) = downColor
            Else
                ' 不添加
            End If
        Next

        Return profiles
    End Function
End Module
