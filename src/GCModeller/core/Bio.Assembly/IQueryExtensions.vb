Imports System.Runtime.CompilerServices
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Text.Similarity

''' <summary>
''' Extensions for object query in the GCModeller biological components
''' </summary>
Public Module IQueryExtensions

    ''' <summary>
    ''' 函数会优先按照<paramref name="geneName"/>进行查询，假若查找不到结果，才会使用<paramref name="products"/>列表进行模糊匹配
    ''' </summary>
    ''' <param name="PTT"></param>
    ''' <param name="geneName"></param>
    ''' <param name="products"></param>
    ''' <param name="cut">字符串匹配相似度的最小的阈值</param>
    ''' <returns></returns>
    <ExportAPI("Get.Gene")>
    <Extension>
    Public Function MatchGene(PTT As PTT, geneName As String, products As IEnumerable(Of String), Optional cut As Double = 0.8) As GeneBrief
        Dim gene As GeneBrief = PTT.GetGeneByName(geneName)

        If gene Is Nothing Then
            For Each desc As String In products
                gene = PTT.GetGeneByDescription(
                    Function(productValue) _
                        FuzzyMatchString.Equals(desc, productValue, cut:=cut)).FirstOrDefault

                If Not gene Is Nothing Then
                    Exit For
                End If
            Next
        End If

        Return gene
    End Function
End Module
