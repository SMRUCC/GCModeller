Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Data.BioCyc
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2

''' <summary>
''' 负责构建虚拟细胞模型之中的文献证据索引表。
''' 
''' 模型之中的各个数据对象使用文献的PubMed编号或者BioCyc的 ``PUB-xxxx`` 编号引用证据表，
''' 证据表本身只保存标题、作者以及年份等元数据，不保存文献的正文内容
''' </summary>
Public Class EvidenceCompiler

    ReadOnly biocyc As Workspace

    ''' <summary>
    ''' PubMed编号或者BioCyc的PUB编号到证据编号的映射
    ''' </summary>
    ReadOnly index As New Dictionary(Of String, String)

    Sub New(biocyc As Workspace)
        Me.biocyc = biocyc
    End Sub

    ''' <summary>
    ''' 构建证据索引表
    ''' </summary>
    ''' <returns></returns>
    Public Function CreateEvidence() As Evidence()
        Dim list As New List(Of Evidence)
        Dim ids As New HashSet(Of String)

        For Each pub As pubs In biocyc.pubs.features
            Dim id As String = If(pub.uniqueId, If(pub.pubmedId.StringEmpty(, True), Nothing, "PUB-" & pub.pubmedId))

            If id.StringEmpty(, True) Then
                Continue For
            End If
            If Not ids.Add(id) Then
                Continue For
            End If

            Call list.Add(New Evidence With {
                .id = id,
                .title = pub.title,
                .authors = pub.authors,
                .doi = pub.doiId,
                .pubmed = pub.pubmedId,
                .year = pub.year,
                .source = pub.source,
                .evidenceType = "reference"
            })

            If Not pub.pubmedId.StringEmpty(, True) Then
                index(pub.pubmedId) = id
            End If
            Call index.TryAdd(id, id)
        Next

        Return list.ToArray
    End Function

    ''' <summary>
    ''' 将BioCyc的引用字符串(``14529615:EV-COMP-AINF:3515430497:paley``)转换为
    ''' 模型文件之中的证据编号
    ''' </summary>
    ''' <param name="citations"></param>
    ''' <returns></returns>
    Public Function GetEvidenceIds(citations As String()) As String()
        Dim result As New List(Of String)

        For Each citation As String In citations.SafeQuery
            Dim token As String = citation.Split(":"c).First.Trim

            If token.StringEmpty(, True) Then
                Continue For
            End If

            Dim id As String = index.TryGetValue(token)

            If id.StringEmpty(, True) Then
                id = "PUB-" & token
            End If
            If Not result.Contains(id) Then
                Call result.Add(id)
            End If
        Next

        Return result.ToArray
    End Function

End Class
