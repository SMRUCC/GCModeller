Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Data.BioCyc
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2

''' <summary>
''' 由 ``pathways.dat`` 构建虚拟细胞模型之中的代谢通路以及通路的功能分类信息
''' </summary>
Public Class PathwayCompiler

    ReadOnly biocyc As Workspace

    Sub New(biocyc As Workspace)
        Me.biocyc = biocyc
    End Sub

    ''' <summary>
    ''' 按照BioCyc之中的一级分类对代谢通路进行分组
    ''' </summary>
    ''' <returns></returns>
    Public Function CreatePathwayMaps() As FunctionalCategory()
        Dim groups = biocyc.pathways.features _
            .GroupBy(Function(p) If(p.types.SafeQuery.FirstOrDefault, "unclassified")) _
            .ToArray

        Return groups _
            .Select(Function(category)
                        Return New FunctionalCategory With {
                            .category = category.Key,
                            .pathways = category.Select(AddressOf createPathway).ToArray
                        }
                    End Function) _
            .ToArray
    End Function

    Private Shared Function createPathway(pwy As pathways) As Pathway
        Return New Pathway With {
            .ID = pwy.uniqueId,
            .name = pwy.commonName,
            .note = pwy.comment,
            .category = pwy.types.SafeQuery.FirstOrDefault,
            .reactions = pwy.reactionList,
            .superPathways = pwy.superPathways,
            .subPathways = pwy.subPathways,
            .dbLinks = pwy.dbLinks,
            .enzymesNotUsed = pwy.enzymesNotUsed,
            .classId = pwy.types.SafeQuery.FirstOrDefault
        }
    End Function

    ''' <summary>
    ''' 代谢通路编号到通路所属分类的映射
    ''' </summary>
    ''' <returns></returns>
    Public Function GetPathwayCategory() As Dictionary(Of String, String)
        Return biocyc.pathways.features _
            .Where(Function(p) Not p.uniqueId.StringEmpty(, True)) _
            .GroupBy(Function(p) p.uniqueId) _
            .ToDictionary(Function(g) g.Key,
                          Function(g)
                              Return If(g.First.types.SafeQuery.FirstOrDefault, "unclassified")
                          End Function)
    End Function

End Class
