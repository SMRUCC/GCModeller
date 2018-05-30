Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Data.GeneOntology.DAG
Imports SMRUCC.genomics.Data.GeneOntology.OBO

''' <summary>
''' 进行富集计算分析所需要的基因组背景模型的导入模块
''' </summary>
Public Module [Imports]

    Public Delegate Function GetClusterTerms(geneID As String) As String()

    <Extension>
    Public Function KEGGClusters(maps As IEnumerable(Of Map)) As GetClusterTerms
        Dim clusters = maps.Select(Function(m)
                                       Return m.Areas.Select(Function(a) a.IDVector) _
                                                     .IteratesALL _
                                                     .Where(Function(id) id.IsPattern("K\d+")) _
                                                     .Distinct _
                                                     .Select(Function(id)
                                                                 Return (mapID:=m.ID, KO:=id)
                                                             End Function)
                                   End Function) _
                           .IteratesALL _
                           .GroupBy(Function(ko) ko.KO) _
                           .ToDictionary(Function(g) g.Key,
                                         Function(g)
                                             Return g.Select(Function(t) t.mapID) _
                                                     .Distinct _
                                                     .ToArray
                                         End Function)
        Return Function(id)
                   If clusters.ContainsKey(id) Then
                       Return clusters(id)
                   Else
                       Return {}
                   End If
               End Function
    End Function

    <Extension>
    Public Function GOClusters(GO_terms As IEnumerable(Of Term)) As GetClusterTerms
        Dim tree As New Graph(GO_terms)
        Dim parentPopulator = Iterator Function(termID As String) As IEnumerable(Of String)
                                  Dim chains = tree.Family(termID).ToArray

                                  For Each route In chains
                                      For Each node In route.Route
                                          Yield node.GO_term.id
                                      Next
                                  Next
                              End Function

        Return Function(termID)
                   Return parentPopulator(termID).ToArray
               End Function
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="db">从UniProt数据库之中下载的基因组Xml文件的迭代器</param>
    ''' <param name="genomeName$"></param>
    ''' <param name="define">Functional cluster define function</param>
    ''' <returns></returns>
    <Extension>
    Public Function ImportsUniProt(db As IEnumerable(Of entry),
                                   getTerm As Func(Of entry, String()),
                                   define As GetClusterTerms,
                                   Optional genomeName$ = Nothing) As Genome

        Dim clusters As New Dictionary(Of String, List(Of String))

        For Each protein As entry In db
            Dim terms = getTerm(protein)
            Dim clusterNames$()

            If terms.IsNullOrEmpty Then
                clusterNames = {"NA"}
            Else
                clusterNames = terms.Select(Function(geneID) define(geneID)) _
                                    .IteratesALL _
                                    .ToArray
            End If

            For Each clusterID As String In clusterNames
                If Not clusters.ContainsKey(clusterID) Then
                    clusters.Add(clusterID, New List(Of String))
                End If

                clusters(clusterID) += protein.accessions
            Next
        Next

        Return New Genome With {
            .Name = genomeName,
            .Clusters = clusters _
                .Select(Function(c)
                            Dim geneIDs$() = c.Value _
                                              .Distinct _
                                              .ToArray

                            Return New Cluster With {
                                .Name = c.Key,
                                .Members = geneIDs
                            }
                        End Function) _
                .ToArray
        }
    End Function
End Module
