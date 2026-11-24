Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices.Development
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Data.BioCyc
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.GCModeller.CompilerServices
Imports SMRUCC.genomics.Metagenomics

Public Class v2Compiler : Inherits Compiler(Of VirtualCell)

    Friend ReadOnly biocyc As Workspace
    Friend ReadOnly cellular_id As String

    ''' <summary>
    ''' 区室编译器：负责BioCyc的CCO编号到模型区室编号的映射
    ''' </summary>
    Friend ReadOnly compartment As CompartmentCompiler
    ''' <summary>
    ''' 文献证据编译器：负责构建模型之中的证据索引表
    ''' </summary>
    Friend ReadOnly evidence As EvidenceCompiler

    ReadOnly ccmap As New Dictionary(Of String, String)

    Sub New(biocyc As Workspace)
        Me.biocyc = biocyc
        Me.cellular_id = biocyc.species.uniqueId
        Me.compartment = New CompartmentCompiler(Me.cellular_id)
        Me.evidence = New EvidenceCompiler(biocyc)
    End Sub

    Public Function MapCompartId(id As String) As String
        Return compartment.MapCompartId(id)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function MapCompartId(id As String()) As String()
        Return id.SafeQuery.Select(AddressOf MapCompartId).ToArray
    End Function

    Protected Overrides Function PreCompile(args As CommandLine) As Integer
        Dim info As New StringBuilder
        Dim spec = biocyc.species

        Using writer As New StringWriter(info)
            Call CLITools.AppSummary(GetType(v2Compiler).Assembly.FromAssembly, "", "", writer)
        End Using

        m_compiledModel = New VirtualCell With {
            .taxonomy = New Taxonomy With {
                .scientificName = If(spec.synonyms.FirstOrDefault, spec.commonName),
                .ncbi_taxid = spec.NCBITaxonomyId,
                .species = spec.commonName
            },
            .cellular_id = spec.uniqueId,
            .properties = New [Property] With {
                .authors = spec.PGDBAuthors,
                .comment = spec.comment,
                .compiled = Now,
                .name = spec.commonName,
                .specieId = spec.NCBITaxonomyId,
                .title = If(spec.synonyms.FirstOrDefault, spec.commonName),
                .publications = evidence.GetEvidenceIds(spec.citations),
                .DBLinks = spec.dbLinks
            },
            .genome = New Genome,
            .dataSource = $"BioCyc {If(spec.PGDBName, spec.uniqueId)} (tier {If(spec.PGDBTIER, "?")})",
            .traits = New Traits With {
                .oxygenRequirement = spec.types.SafeQuery.FirstOrDefault
            }
        }
        m_logging.WriteLine(info.ToString)

        Return 0
    End Function

    Protected Overrides Function CompileImpl(args As CommandLine) As Integer
        Dim genomeCompiler As New GenomeCompiler(Me)
        Dim replicons As replicon() = genomeCompiler.CreateReplicons()
        Dim metabolism As MetabolismStructure = New ReactionNetworkCompiler(Me).BuildModel()
        Dim operons As TranscriptUnit() = replicons _
            .Select(Function(r) r.operons) _
            .IteratesALL _
            .ToArray

        ' 调控元件：启动子、终止子、DNA结合位点以及蛋白质的结构域
        Dim featureCompiler As New FeatureCompiler(biocyc, operons.Select(Function(o) o.id))
        Dim promoters As Promoter() = featureCompiler.CreatePromoters()
        Dim terminators As Terminator() = featureCompiler.CreateTerminators()
        Dim bindingSites As BindingSite() = featureCompiler.CreateBindingSites()
        Dim proteinFeatures As ProteinFeature() = featureCompiler.CreateProteinFeatures()

        Call FeatureCompiler.Attach(operons, promoters, terminators)

        Dim regulonList As Regulon() = createRegulons(operons)
        Dim regulations As transcription() = New RegulationCompiler(biocyc, operons, bindingSites, regulonList) _
            .CreateRegulations() _
            .ToArray
        Dim proteins As protein() = New ProteinCompiler(biocyc).CreateProteins().ToArray

        Call attachProteinFeatures(proteins, proteinFeatures)

        m_compiledModel.metabolismStructure = metabolism
        m_compiledModel.genome = New Genome With {
            .replicons = replicons,
            .proteins = proteins,
            .regulations = regulations,
            .regulons = regulonList,
            .promoters = promoters,
            .terminators = terminators,
            .bindingSites = bindingSites,
            .proteinFeatures = proteinFeatures,
            .gcContent = replicons.Select(Function(r) r.gcContent * r.length).DefaultIfEmpty(0).Sum /
                replicons.Select(Function(r) r.length).DefaultIfEmpty(1).Sum,
            .genomeSize = replicons.Select(Function(r) r.length).DefaultIfEmpty(0).Sum
        }
        m_compiledModel.compartments = compartment.Compartments
        m_compiledModel.physiology = createPhysiology()
        m_compiledModel.evidence = createEvidence(m_compiledModel)
        m_compiledModel.classTerms = New ClassCompiler(biocyc).CreateClasses()

        Call joinMissingCompounds(metabolism)

        m_logging.WriteLine($"genes: {m_compiledModel.genome.GetAllGenes.Count}")
        m_logging.WriteLine($"reactions: {metabolism.reactions.size}")

        Return 0
    End Function

    ''' <summary>
    ''' 由调控因子以及其所调控的转录单元构建调控组
    ''' </summary>
    ''' <param name="operons"></param>
    ''' <returns></returns>
    Private Function createRegulons(operons As TranscriptUnit()) As Regulon()
        Dim list As New List(Of Regulon)
        Dim index As New Dictionary(Of String, Regulon)

        ' 首先使用数据库之中明确定义的调控组
        For Each regulon As regulons In biocyc.regulons.features
            If regulon.regulator.StringEmpty(, True) Then
                Continue For
            End If
            If Not regulon.types.SafeQuery.Any(Function(t) t.ToLowerInvariant.Contains("regulon")) Then
                Continue For
            End If
            If index.ContainsKey(regulon.uniqueId) Then
                Continue For
            End If

            Dim members As String() = regulon.regulatedEntity _
                .SafeQuery _
                .Where(Function(id) operons.Any(Function(o) o.id = id)) _
                .ToArray

            Dim item As New Regulon With {
                .id = regulon.uniqueId,
                .name = regulon.commonName,
                .regulator = regulon.regulator,
                .members = members,
                .note = regulon.comment
            }

            index.Add(item.id, item)
            Call list.Add(item)
        Next

        ' 然后由调控关系补充没有得到定义的调控组
        For Each reg As regulation In biocyc.regulation.features
            If reg.types.SafeQuery.Any(Function(t) t = "Transcription-Factor-Binding") Then
                Continue For
            End If

            If reg.regulator.StringEmpty(, True) Then
                Continue For
            End If
            If index.ContainsKey(reg.regulator) Then
                Continue For
            End If

            Dim item As New Regulon With {
                .id = reg.regulator,
                .name = reg.regulator,
                .regulator = reg.regulator,
                .members = operons _
                    .Where(Function(o) o.genes.SafeQuery.Any(Function(g) g.locus_tag = reg.regulatedEntity)) _
                    .Select(Function(o) o.id) _
                    .ToArray
            }

            index.Add(item.id, item)
            Call list.Add(item)
        Next

        Return list.ToArray
    End Function

    ''' <summary>
    ''' 构建模型文件之中的文献证据索引表。
    ''' 
    ''' 数据库之中的文献记录数量巨大，但是模型之中实际引用到的文献只占其中很少的一部分，
    ''' 因此这里仅仅输出被模型数据对象实际引用到的文献记录，避免模型文件的体积过度膨胀
    ''' </summary>
    ''' <param name="model"></param>
    ''' <returns></returns>
    Private Function createEvidence(model As VirtualCell) As Evidence()
        Dim used As New HashSet(Of String)

        Call addEvidence(used, model.properties?.publications)
        Call addEvidence(used, model.genome.GetAllGenes().Select(Function(g) g.evidence).IteratesALL)
        Call addEvidence(used, model.genome.proteins.SafeQuery.Select(Function(p) p.evidence).IteratesALL)
        Call addEvidence(used, model.genome.regulations.SafeQuery.Select(Function(r) r.evidence).IteratesALL)
        Call addEvidence(used, model.metabolismStructure.compounds.SafeQuery.Select(Function(c) c.evidence).IteratesALL)
        Call addEvidence(used, model.metabolismStructure.reactions.SafeQuery.Select(Function(r) r.evidence).IteratesALL)

        Return evidence _
            .CreateEvidence() _
            .Where(Function(e) used.Contains(e.id)) _
            .ToArray
    End Function

    Private Shared Sub addEvidence([set] As HashSet(Of String), values As IEnumerable(Of String))
        For Each id As String In values.SafeQuery
            If Not id.StringEmpty(, True) Then
                Call [set].Add(id)
            End If
        Next
    End Sub

    Private Function createPhysiology() As Physiology
        Dim spec = biocyc.species

        Return New Physiology With {
            .temperature = 37,
            .pH = 7.2,
            .oxygenRequirement = If(spec.types.SafeQuery.FirstOrDefault, "facultative"),
            .medium = spec.comment
        }
    End Function

    Private Shared Sub attachProteinFeatures(proteins As protein(), features As ProteinFeature())
        Dim index As Dictionary(Of String, ProteinFeature()) = features _
            .Where(Function(f) Not f.protein.StringEmpty(, True)) _
            .GroupBy(Function(f) f.protein) _
            .ToDictionary(Function(g) g.Key, Function(g) g.ToArray)

        For Each prot As protein In proteins.SafeQuery
            prot.domains = index.TryGetValue(prot.protein_id)
        Next
    End Sub

    ''' <summary>
    ''' 将酶的动力学参数所引用到但是没有在 ``compounds.dat`` 之中定义的化合物补齐到模型之中
    ''' </summary>
    ''' <param name="metabolism"></param>
    Private Shared Sub joinMissingCompounds(metabolism As MetabolismStructure)
        Dim usedIndex As Index(Of String) = metabolism.compounds _
            .SafeQuery _
            .Select(Function(c) c.ID) _
            .Distinct _
            .Indexing
        Dim missing = metabolism.enzymes _
            .SafeQuery _
            .Select(Function(enz) enz.catalysis) _
            .IteratesALL _
            .Select(Function(cat) cat.parameter) _
            .IteratesALL _
            .Select(Function(par) par.target) _
            .Where(Function(id) Not id.StringEmpty(, True)) _
            .Distinct _
            .Where(Function(id) Not id Like usedIndex) _
            .Select(Function(id)
                        Return New Compound With {
                            .ID = id,
                            .db_xrefs = Nothing,
                            .name = id
                        }
                    End Function) _
            .ToArray

        metabolism.compounds = metabolism.compounds _
            .SafeQuery _
            .JoinIterates(missing) _
            .ToArray
    End Sub
End Class
