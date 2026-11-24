' ============================================================================
' PhenotypeDatasetBuilder.vb
'
' 从真实数据构建训练集：
'   K:/cultral_medium/pfam/<Species>/Pfam.csv      -> Dictionary(Of String, PfamString())
'   ncbi_species_summary_filtered.tsv              -> TraitAnnotation
'   phenotype_traits_and_types.csv                 -> PhenotypeTrait（含 data_type）
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports Microsoft.VisualBasic.Data.Framework
Imports SMRUCC.genomics.ProteinModel

Namespace Traitar.Modules

    ''' <summary>真实数据的装配入口</summary>
    Public Module PhenotypeDatasetBuilder

        ''' <summary>
        ''' 读取每个物种目录下的 <c>Pfam.csv</c>（列：ProteinId,Length,Description,Pfam-string,Domain.Ids）。
        ''' </summary>
        ''' <param name="dir">包含物种子目录的根目录</param>
        Public Function LoadPfamSets(dir As String) As Dictionary(Of String, PfamString())
            Dim sets As New Dictionary(Of String, PfamString())(StringComparer.Ordinal)

            If Not Directory.Exists(dir) Then
                Return sets
            End If

            For Each folder As String In Directory.GetDirectories(dir)
                Dim csv As String = Path.Combine(folder, "Pfam.csv")

                If File.Exists(csv) Then
                    sets(Path.GetFileName(folder)) = csv _
                        .LoadCsv(Of PfamString)(mute:=True) _
                        .ToArray()
                End If
            Next

            Return sets
        End Function

        ''' <summary>
        ''' 读取表型定义表、表型标注表与 Pfam 蛋白质组注释，构建训练集。
        ''' </summary>
        ''' <param name="pfamDir">Pfam 物种目录</param>
        ''' <param name="traitTable">phenotype_traits_and_types.csv</param>
        ''' <param name="annotations">ncbi_species_summary_filtered.tsv</param>
        ''' <param name="minGenomes">Pfam 词表剪枝阈值</param>
        ''' <param name="encoding">Pfam 编码方式</param>
        Public Function FromFiles(pfamDir As String,
                                  traitTable As String,
                                  annotations As String,
                                  Optional minGenomes As Integer = 1,
                                  Optional encoding As PfamEncoding = PfamEncoding.NormalizedCount) As TraitTrainingSet

            Dim traits As PhenotypeTrait() = PhenotypeTraits.LoadTable(traitTable)
            Dim data As TraitAnnotation() = TraitAnnotation.ParseTable(annotations).ToArray()
            Dim pfams As Dictionary(Of String, PfamString()) = LoadPfamSets(pfamDir)
            Dim embedding As PfamEmbedding = TraitProblemBuilder.CreateEmbedding(pfams, minGenomes, encoding)

            Return TraitProblemBuilder.Build(data, pfams, traits, embedding)
        End Function

    End Module
End Namespace
