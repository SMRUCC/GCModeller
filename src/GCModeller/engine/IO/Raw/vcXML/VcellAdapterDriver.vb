' ============================================================
' VcellAdapterDriver.vb - 模拟引擎 → vcXML 2.0 的适配驱动
' ============================================================
' 两种用法：
'   1. 旧构造（兼容 R# vcellkit）：
'        New VcellAdapterDriver(file, models, fluxBaseline)
'      内部生成「单物种 sp1 + 1×1×1 空间 + 空培养基」的默认元数据。
'   2. 新构造（完整实验描述）：
'        New VcellAdapterDriver(file, experiment, species, space, medium, layout, perturbations)
'      调用方填充 vcExperiment / vcSpecies / vcGrowthSpace / vcMedium / vcLayout。
'
' 多物种的组学 id 列表通过 SpeciesOmics 提供（species id → OmicsTuple(Of String())）；
' 未提供时回退到旧的合并列表（单物种视图）。
' ============================================================

Imports System.Xml
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.GCModeller.ModellingEngine.BootstrapLoader.Engine
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Definitions
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Engine
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular

Namespace vcXML

    ''' <summary>
    ''' 把模拟结果写入 vcXML 2.0 的适配器。
    ''' </summary>
    Public Class VcellAdapterDriver : Implements IOmicsDataAdapter, IDisposable

        Public ReadOnly Property mass As OmicsTuple(Of String()) Implements IOmicsDataAdapter.mass
        Public ReadOnly Property flux As OmicsTuple(Of String())

        ''' <summary>每个物种的组学 id 列表（species id → 三个组学）</summary>
        Public Property SpeciesOmics As Dictionary(Of String, OmicsTuple(Of String()))

        Public Property Experiment As vcExperiment
        Public Property Species As vcSpecies()
        Public Property GrowthSpace As vcGrowthSpace
        Public Property CultureMedium As vcMedium
        Public Property Layout As vcLayout
        Public Property Perturbations As vcPerturbation()

        Dim fs As Writer
        Dim perturbationList As New List(Of vcPerturbation)
        Dim currentTick As Integer = 0
        Dim currentTime As Double = 0
        Dim m_args As FluxBaseline
        Dim m_initialized As Boolean = False

        ''' <summary>
        ''' 兼容旧版：由 <see cref="OmicsDataAdapter"/> 推导合并的 id 列表，
        ''' 使用单物种、单体素的默认布局。
        ''' </summary>
        Sub New(file As String, model As CellularModule(), args As FluxBaseline)
            mass = OmicsDataAdapter.GetMassTuples(model)
            flux = OmicsDataAdapter.GetFluxTuples(model)

            Experiment = New vcExperiment With {
                .engine = "BootstrapLoader",
                .seed = "auto",
                .timeUnit = VcOntology.UnitMinute,
                .timeUnitName = "minute",
                .cvParams = {
                    VcOntology.Param(VcOntology.Simulation, "virtual cell simulation", ""),
                    VcOntology.Param(VcOntology.BoundaryDerivation, "boundary derivation", "transport-reaction")
                }
            }
            Species = {
                New vcSpecies With {
                    .id = "sp1",
                    .name = $"consortium({If(model Is Nothing, 0, model.Length)})",
                    .taxonomy = "",
                    .mrna = mass.transcriptome.Length,
                    .protein = mass.proteome.Length,
                    .metabolite = mass.metabolome.Length,
                    .cvParams = {
                        VcOntology.Param(VcOntology.Species, "species", $"consortium({If(model Is Nothing, 0, model.Length)})")
                    }
                }
            }
            GrowthSpace = New vcGrowthSpace With {
                .shape = New vcShape With {
                    .width = 1,
                    .height = 1,
                    .depth = 1,
                    .voxelSize = 1,
                    .occupied = 1,
                    .total = 1
                },
                .boundary = New vcBoundary With {.type = "cube"}
            }
            CultureMedium = Nothing
            Perturbations = New vcPerturbation() {}
            Layout = createDefaultLayout()
            SpeciesOmics = New Dictionary(Of String, OmicsTuple(Of String())) From {
                {"sp1", mass}
            }

            m_args = args
            fs = New Writer(file, New XmlWriterSettings With {.Indent = True, .OmitXmlDeclaration = True})

            Call Initialize(SpeciesOmics)
        End Sub

        ''' <summary>
        ''' 完整实验描述构造。
        ''' </summary>
        Sub New(file As String,
                experiment As vcExperiment,
                species As vcSpecies(),
                space As vcGrowthSpace,
                medium As vcMedium,
                layout As vcLayout,
                Optional perturbations As vcPerturbation() = Nothing)

            Me.mass = Nothing
            Me.flux = Nothing
            Me.Experiment = experiment
            Me.Species = If(species, New vcSpecies() {})
            Me.GrowthSpace = space
            Me.CultureMedium = medium
            Me.Layout = If(layout, createDefaultLayout())
            Me.Perturbations = If(perturbations, New vcPerturbation() {})
            Me.SpeciesOmics = New Dictionary(Of String, OmicsTuple(Of String()))()

            m_args = Nothing
            fs = New Writer(file, New XmlWriterSettings With {.Indent = True, .OmitXmlDeclaration = True})
        End Sub

        ''' <summary>
        ''' 初始化文件头并写出 dataEntity 段（新构造必须显式调用一次）。
        ''' </summary>
        ''' <param name="omics">每个物种的组学 id 列表；Nothing 表示沿用已有值</param>
        Public Sub Initialize(Optional omics As Dictionary(Of String, OmicsTuple(Of String())) = Nothing)
            If m_initialized Then
                Return
            End If
            If omics IsNot Nothing Then
                SpeciesOmics = omics
            End If

            Call fs.writeInit(Me, m_args)
            Call fs.beginDataEntity()
            Call writeEntities()
            Call fs.endDataEntityAndBeginRaw()

            m_initialized = True
        End Sub

        ''' <summary>默认布局：单物种、单点体素、三个组学轴</summary>
        Private Shared Function createDefaultLayout() As vcLayout
            Return New vcLayout With {
                .frameMode = "sparse",
                .blockOrder = "voxel,molecule",
                .cvParams = {
                    VcOntology.Param(VcOntology.SpatiotemporalLayout, "spatiotemporal layout", "")
                },
                .axes = {
                    New vcAxis With {
                        .name = "species",
                        .count = 1,
                        .items = {New vcAxisItem With {.index = 0, .reference = "sp1"}}
                    },
                    New vcAxis With {
                        .name = "voxel",
                        .count = 1,
                        .items = {New vcAxisItem With {.index = 0, .x = 0, .y = 0, .z = 0}}
                    },
                    New vcAxis With {
                        .name = "group",
                        .count = 3,
                        .items = {
                            New vcAxisItem With {.index = 0, .value = "transcriptome"},
                            New vcAxisItem With {.index = 1, .value = "proteome"},
                            New vcAxisItem With {.index = 2, .value = "metabolome"}
                        }
                    }
                }
            }
        End Function

        ''' <summary>
        ''' 追加一个虚拟扰动（必须在第一次快照之前调用）。
        ''' </summary>
        Public Sub AddPerturbation(perturbation As vcPerturbation)
            If perturbation Is Nothing Then
                Return
            End If

            Call perturbationList.Add(perturbation)
            Perturbations = perturbationList.ToArray
        End Sub

        ''' <summary>
        ''' 写出指定物种在某一帧的某组学数据（稀疏：一个体素一条）。
        ''' 
        ''' <paramref name="ids"/> 是该 (species, group) 的实体 id 列表，长度必须与 values 一致。
        ''' </summary>
        Public Sub WriteSpeciesFrame(tick As Integer, frameTime As Double, moduleName As String, contentType As String,
                                     speciesId As String, voxelIndex As Integer, ids As String(), values As Double())

            If values.IsNullOrEmpty Then
                Return
            End If

            Call fs.addFrame(tick, frameTime, moduleName, contentType, speciesId,
                             voxelIndex, 1, If(ids Is Nothing, values.Length, ids.Length), values)
        End Sub

        ''' <summary>推进时间轴（供扰动匹配使用）</summary>
        Public Sub Advance(tick As Integer, frameTime As Double)
            currentTick = tick
            currentTime = frameTime
        End Sub

        ''' <summary>当前帧生效的扰动</summary>
        Public Function ActivePerturbations() As vcPerturbation()
            Return Perturbations _
                .Where(Function(p) p.IsActive(currentTick)) _
                .ToArray
        End Function

        ''' <summary>
        ''' 写出一个物种的完整组学快照（三个 module 各一条帧）。
        ''' </summary>
        Private Sub writeSpeciesSnapshot(speciesId As String, omics As OmicsTuple(Of String()),
                                         massData As Dictionary(Of String, Double),
                                         fluxData As Dictionary(Of String, Double),
                                         tick As Integer, frameTime As Double)

            If omics Is Nothing Then
                Return
            End If

            ' mass profile（MassSnapshot 与 FluxSnapshot 分开回调，另一侧可能为 Nothing）
            If massData IsNot Nothing Then
                Call fs.addFrame(tick, frameTime, "transcriptome", "mass_profile", speciesId, 0, 1, omics.transcriptome.Length,
                                 massData.Takes(omics.transcriptome))
                Call fs.addFrame(tick, frameTime, "proteome", "mass_profile", speciesId, 0, 1, omics.proteome.Length,
                                 massData.Takes(omics.proteome))
                Call fs.addFrame(tick, frameTime, "metabolome", "mass_profile", speciesId, 0, 1, omics.metabolome.Length,
                                 massData.Takes(omics.metabolome))
            End If

            ' flux profile
            If fluxData IsNot Nothing Then
                Call fs.addFrame(tick, frameTime, "transcriptome", "activity", speciesId, 0, 1, omics.transcriptome.Length,
                                 fluxData.Takes(omics.transcriptome))
                Call fs.addFrame(tick, frameTime, "proteome", "activity", speciesId, 0, 1, omics.proteome.Length,
                                 fluxData.Takes(omics.proteome))
                Call fs.addFrame(tick, frameTime, "metabolome", "flux_size", speciesId, 0, 1, omics.metabolome.Length,
                                 fluxData.Takes(omics.metabolome))
            End If
        End Sub

        Public Sub MassSnapshot(iteration As Integer, data As Dictionary(Of String, Double)) Implements IOmicsDataAdapter.MassSnapshot
            Call Advance(iteration, iteration)
            Call writeSnapshot(iteration, data, Nothing)
        End Sub

        Public Sub FluxSnapshot(iteration As Integer, data As Dictionary(Of String, Double)) Implements IOmicsDataAdapter.FluxSnapshot
            Call writeSnapshot(iteration, Nothing, data)
        End Sub

        Private Sub writeSnapshot(tick As Integer, massData As Dictionary(Of String, Double),
                                  fluxData As Dictionary(Of String, Double))

            ' 多物种：按 SpeciesOmics 分别写；否则回退到合并列表（旧行为）
            If Not SpeciesOmics.IsNullOrEmpty Then
                For Each sp As vcSpecies In Species
                    Dim omics As OmicsTuple(Of String()) = Nothing

                    If SpeciesOmics.TryGetValue(sp.id, omics) Then
                        Call writeSpeciesSnapshot(sp.id, omics, massData, fluxData, tick, tick)
                    End If
                Next

                Return
            End If

            If mass Is Nothing Then
                Return
            End If

            Call writeSpeciesSnapshot("", mass, massData, fluxData, tick, tick)
        End Sub

        Public Sub ForwardRegulation(iteration As Integer, data As Dictionary(Of String, Double)) Implements IOmicsDataAdapter.ForwardRegulation
        End Sub

        Public Sub ReverseRegulation(iteration As Integer, data As Dictionary(Of String, Double)) Implements IOmicsDataAdapter.ReverseRegulation
        End Sub

        ''' <summary>初始化 dataEntity 段（必须在写帧之前调用一次）</summary>
        Friend Sub writeEntities()
            Dim list As vcSpecies() = If(Species, New vcSpecies() {})

            If list.Length = 0 Then
                Call writeHeaderFor("", mass)
                Return
            End If

            For Each sp As vcSpecies In list
                Dim omics As OmicsTuple(Of String()) = Nothing

                If Not SpeciesOmics.IsNullOrEmpty AndAlso SpeciesOmics.TryGetValue(sp.id, omics) Then
                    Call writeHeaderFor(sp.id, omics)
                Else
                    Call writeHeaderFor(sp.id, mass)
                End If
            Next
        End Sub

        Private Sub writeHeaderFor(speciesId As String, omics As OmicsTuple(Of String()))
            If omics Is Nothing Then
                Return
            End If

            Call fs.writeHeader("transcriptome", "mass_profile", speciesId, omics.transcriptome)
            Call fs.writeHeader("proteome", "mass_profile", speciesId, omics.proteome)
            Call fs.writeHeader("metabolome", "mass_profile", speciesId, omics.metabolome)

            If flux Is Nothing Then
                Return
            End If

            Call fs.writeHeader("transcriptome", "activity", speciesId, flux.transcriptome)
            Call fs.writeHeader("proteome", "activity", speciesId, flux.proteome)
            Call fs.writeHeader("metabolome", "flux_size", speciesId, flux.metabolome)
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    Call fs.Dispose()
                End If
            End If

            disposedValue = True
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)
        End Sub
#End Region

    End Class

End Namespace
