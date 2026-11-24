Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Data.BioCyc
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2

''' <summary>
''' 汇总基因组之上的各类调控元件(启动子、终止子以及DNA结合位点)以及蛋白质序列上面的
''' 结构域与功能位点，并将这些数据转换为虚拟细胞模型之中的结构对象
''' </summary>
Public Class FeatureCompiler

    ReadOnly biocyc As Workspace
    ReadOnly operons As Index(Of String)

    Sub New(biocyc As Workspace, operons As IEnumerable(Of String))
        Me.biocyc = biocyc
        Me.operons = operons.Indexing
    End Sub

    ''' <summary>
    ''' 启动子(转录起始位点)
    ''' </summary>
    ''' <returns></returns>
    Public Function CreatePromoters() As Promoter()
        Return biocyc.promoters.features _
            .Select(Function(p)
                        Return New Promoter With {
                            .id = p.uniqueId,
                            .operonId = findOperon(p.componentOf),
                            .left = CInt(p.left),
                            .right = CInt(p.right),
                            .strand = p.direction,
                            .tss = CInt(p.absolutePlus1Position),
                            .sigmaFactor = p.sigmaFactor.SafeQuery.FirstOrDefault,
                            .minus10Left = CInt(p.minus10Left),
                            .minus10Right = CInt(p.minus10Right),
                            .minus35Left = CInt(p.minus35Left),
                            .minus35Right = CInt(p.minus35Right),
                            .note = p.comment
                        }
                    End Function) _
            .ToArray
    End Function

    ''' <summary>
    ''' 转录终止子
    ''' </summary>
    ''' <returns></returns>
    Public Function CreateTerminators() As Terminator()
        Return biocyc.terminators.features _
            .Select(Function(t)
                        Return New Terminator With {
                            .id = t.uniqueId,
                            .operonId = findOperon(t.componentOf),
                            .left = CInt(t.left),
                            .right = CInt(t.right),
                            .position = CInt(t.positionOfTermination),
                            .note = t.comment
                        }
                    End Function) _
            .ToArray
    End Function

    ''' <summary>
    ''' DNA结合位点
    ''' </summary>
    ''' <returns></returns>
    Public Function CreateBindingSites() As BindingSite()
        Return biocyc.dnabindsites.features _
            .Select(Function(site)
                        Return New BindingSite With {
                            .id = site.uniqueId,
                            .operonId = findOperon(site.componentOf),
                            .left = CInt(site.left),
                            .right = CInt(site.right),
                            .strand = site.direction,
                            .siteType = site.types.SafeQuery.FirstOrDefault,
                            .sequence = site.comment
                        }
                    End Function) _
            .ToArray
    End Function

    ''' <summary>
    ''' 蛋白质的结构域以及功能位点
    ''' </summary>
    ''' <returns></returns>
    Public Function CreateProteinFeatures() As ProteinFeature()
        Return biocyc.proteinFeatures.features _
            .Select(Function(f)
                        Return New ProteinFeature With {
                            .id = f.uniqueId,
                            .protein = f.featureOf.SafeQuery.FirstOrDefault,
                            .featureType = f.featureType,
                            .left = CInt(f.left),
                            .right = CInt(f.right),
                            .residue = f.residueType,
                            .note = f.comment
                        }
                    End Function) _
            .ToArray
    End Function

    ''' <summary>
    ''' 将启动子以及终止子对象挂载到对应的转录单元之上
    ''' </summary>
    ''' <param name="units"></param>
    ''' <param name="promoters"></param>
    ''' <param name="terminators"></param>
    Public Shared Sub Attach(units As IEnumerable(Of TranscriptUnit), promoters As Promoter(), terminators As Terminator())
        Dim promoterIndex As Dictionary(Of String, Promoter) = promoters _
            .SafeQuery _
            .Where(Function(p) Not p.operonId.StringEmpty(, True)) _
            .GroupBy(Function(p) p.operonId) _
            .ToDictionary(Function(g) g.Key, Function(g) g.First)
        Dim terminatorIndex As Dictionary(Of String, Terminator) = terminators _
            .SafeQuery _
            .Where(Function(t) Not t.operonId.StringEmpty(, True)) _
            .GroupBy(Function(t) t.operonId) _
            .ToDictionary(Function(g) g.Key, Function(g) g.First)

        For Each unit As TranscriptUnit In units.SafeQuery
            unit.promoter = promoterIndex.TryGetValue(unit.id)
            unit.terminator = terminatorIndex.TryGetValue(unit.id)

            If Not unit.promoter Is Nothing Then
                unit.direction = unit.promoter.strand
            End If
        Next
    End Sub

    Private Function findOperon(components As String()) As String
        Return components _
            .SafeQuery _
            .Where(Function(id) id Like operons) _
            .FirstOrDefault
    End Function

    Private Function findOperon(component As String) As String
        If component.StringEmpty(, True) Then
            Return Nothing
        End If

        If component Like operons Then
            Return component
        Else
            Return Nothing
        End If
    End Function

End Class
