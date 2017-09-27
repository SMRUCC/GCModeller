Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Assembly.Uniprot.Web

Public Module Mappings

    ''' <summary>
    ''' 对于还没有参考基因组的物种蛋白组实验而言，实验数据的基因号可能会是用户自己定义的基因号，
    ''' 则需要使用这个函数将用户的基因号替换为所注释到的UniprotKB编号，方便进行后续的实验数据的分析
    ''' </summary>
    ''' <param name="DEGgenes"></param>
    ''' <param name="tsv$"></param>
    ''' <returns></returns>
    <Extension>
    Public Function UserCustomMaps(DEGgenes As IEnumerable(Of EntityObject), tsv$) As EntityObject()
        Dim DEPgenes = DEGgenes.ToArray

        With tsv.MappingReader Or New Dictionary(Of String, String())().AsDefault
            If .Count > 0 Then

                ' 将用户基因号转换为uniprot编号
                For Each gene In DEPgenes
                    If .ContainsKey(gene.ID) Then
                        gene.ID = .ref(gene.ID).First
                    End If
                Next
            End If
        End With

        Return DEPgenes
    End Function
End Module
