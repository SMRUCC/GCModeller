' ============================================================
' vcXML 2.0 往返测试
' ============================================================
' 覆盖：
'   * cvList / experiment / species / growthSpace / cultureMedium /
'     perturbations / layout 的写入与读回；
'   * 多物种 × 多体素（sparse）帧的写入、索引与切片；
'   * getFrameVector(offset) 与 getFrameVector(offset, voxelIndex) 的数值一致性；
'   * 1.x 文件被明确拒绝（NotSupportedException）。
' ============================================================

Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Engine
Imports SMRUCC.genomics.GCModeller.ModellingEngine.IO.vcXML

Module Program

    Private failures As Integer = 0

    Private Const SpeciesCount As Integer = 2
    Private Const VoxelCount As Integer = 8
    Private Const TickCount As Integer = 5
    Private Const RnaCount As Integer = 10
    Private Const ProteinCount As Integer = 8
    Private Const MetaboliteCount As Integer = 12

    Sub Main()
        Dim dir As String = Path.Combine(Path.GetTempPath(), "vcxml2_" & Guid.NewGuid().ToString("N"))

        Call Directory.CreateDirectory(dir)

        Try
            Dim file As String = Path.Combine(dir, "test.vcxml")

            Call writeTestFile(file)
            Call readAndVerify(file)
            Call verifyLegacyRejected(dir)
        Catch ex As Exception
            failures += 1
            Call Console.WriteLine($"[FAIL] unhandled: {ex}")
        Finally
            Try
                Call Directory.Delete(dir, recursive:=True)
            Catch
            End Try
        End Try

        Call Console.WriteLine()
        Call Console.WriteLine($"[vcXML.test] failures={failures}")

        Environment.ExitCode = If(failures = 0, 0, 1)
    End Sub

    ' ---------------------------------------------------------------
    ' 写入
    ' ---------------------------------------------------------------
    Private Sub writeTestFile(file As String)
        Dim species As vcSpecies() = {
            New vcSpecies With {
                .id = "sp1", .name = "Species A", .taxonomy = "taxA", .strain = "A1",
                .mrna = RnaCount, .protein = ProteinCount, .metabolite = MetaboliteCount,
                .cvParams = {VcOntology.Param(VcOntology.Species, "species", "Species A")}
            },
            New vcSpecies With {
                .id = "sp2", .name = "Species B", .taxonomy = "taxB", .strain = "B1",
                .mrna = RnaCount, .protein = ProteinCount, .metabolite = MetaboliteCount,
                .cvParams = {VcOntology.Param(VcOntology.Species, "species", "Species B")}
            }
        }

        Dim voxels As vcAxisItem() = New vcAxisItem(VoxelCount - 1) {}

        For i As Integer = 0 To VoxelCount - 1
            voxels(i) = New vcAxisItem With {
                .index = i,
                .x = i Mod 2,
                .y = (i \ 2) Mod 2,
                .z = i \ 4
            }
        Next

        Dim layout As New vcLayout With {
            .frameMode = "sparse",
            .blockOrder = "voxel,molecule",
            .cvParams = {VcOntology.Param(VcOntology.SpatiotemporalLayout, "spatiotemporal layout", "")},
            .axes = {
                New vcAxis With {
                    .name = "species", .count = SpeciesCount,
                    .items = {
                        New vcAxisItem With {.index = 0, .reference = "sp1"},
                        New vcAxisItem With {.index = 1, .reference = "sp2"}
                    }
                },
                New vcAxis With {.name = "voxel", .count = VoxelCount, .items = voxels},
                New vcAxis With {
                    .name = "group", .count = 3,
                    .items = {
                        New vcAxisItem With {.index = 0, .value = "transcriptome"},
                        New vcAxisItem With {.index = 1, .value = "proteome"},
                        New vcAxisItem With {.index = 2, .value = "metabolome"}
                    }
                }
            }
        }

        Dim experiment As New vcExperiment With {
            .engine = "Cella",
            .seed = "2024",
            .totalTime = TickCount,
            .timeStep = 1,
            .frameCount = TickCount,
            .cvParams = {
                VcOntology.Param(VcOntology.Simulation, "virtual cell simulation", ""),
                VcOntology.Number(VcOntology.TotalTime, "total simulation time", TickCount,
                                  VcOntology.UnitMinute, "minute"),
                VcOntology.Flag(VcOntology.PassiveDiffusion, "passive diffusion", True)
            },
            .userParams = {
                New userParam With {.name = "commandLine", .value = "/run --model test"}
            }
        }

        Dim space As New vcGrowthSpace With {
            .cvParams = {VcOntology.Param(VcOntology.VoxelGrid, "voxel grid", "")},
            .shape = New vcShape With {
                .width = 2, .height = 2, .depth = 2,
                .voxelSize = 1, .occupied = VoxelCount, .total = VoxelCount
            },
            .boundary = New vcBoundary With {.type = "cube"}
        }

        Dim medium As New vcMedium With {
            .name = "M9",
            .ph = 7.0,
            .temperature = 37,
            .components = New vcMediumComponentList With {
                .count = 2,
                .items = {
                    New vcMediumComponent With {
                        .id = "glucose", .concentration = 10,
                        .cvParams = {
                            VcOntology.Number(VcOntology.InitialConcentration, "initial concentration",
                                              10, VcOntology.UnitMillimolePerLitre, "millimole per litre")
                        }
                    },
                    New vcMediumComponent With {.id = "oxygen", .concentration = 1}
                }
            },
            .feeding = New vcFeeding With {.mode = "batch"},
            .diffusion = New vcDiffusion With {.coefficient = 1.0}
        }

        Dim perturbations As vcPerturbation() = {
            New vcPerturbation With {
                .id = "p1",
                .type = "gene-knockout",
                .startTick = 2,
                .endTick = 4,
                .cvParams = {VcOntology.Param(VcOntology.GeneKnockout, "gene knockout", "gene1")},
                .targets = {New vcPerturbationTarget With {.species = "sp1", .molecule = "gene1", .factor = 0}}
            }
        }

        Dim rnaIds As String() = makeIds("mRNA", RnaCount)
        Dim proteinIds As String() = makeIds("protein", ProteinCount)
        Dim metaboliteIds As String() = makeIds("metabolite", MetaboliteCount)
        Dim omics As New Dictionary(Of String, OmicsTuple(Of String()))()

        For Each sp As vcSpecies In species
            omics(sp.id) = New OmicsTuple(Of String())(rnaIds, proteinIds, metaboliteIds)
        Next

        Using driver As New VcellAdapterDriver(file, experiment, species, space, medium, layout, perturbations)
            Call driver.Initialize(omics)

            For tick As Integer = 1 To TickCount
                For Each sp As vcSpecies In species
                    Dim speciesIndex As Integer = If(sp.id = "sp1", 0, 1)

                    For v As Integer = 0 To VoxelCount - 1
                        Call driver.WriteSpeciesFrame(tick, tick, "transcriptome", "mass_profile",
                                                      sp.id, v, rnaIds,
                                                      makeValues(RnaCount, tick, v, speciesIndex))
                        Call driver.WriteSpeciesFrame(tick, tick, "proteome", "mass_profile",
                                                      sp.id, v, proteinIds,
                                                      makeValues(ProteinCount, tick, v, speciesIndex))
                        Call driver.WriteSpeciesFrame(tick, tick, "metabolome", "mass_profile",
                                                      sp.id, v, metaboliteIds,
                                                      makeValues(MetaboliteCount, tick, v, speciesIndex))
                    Next
                Next
            Next
        End Using
    End Sub

    ' ---------------------------------------------------------------
    ' 读回与断言
    ' ---------------------------------------------------------------
    Private Sub readAndVerify(file As String)
        Using reader As New Reader(file)
            Call check(reader.CVList IsNot Nothing AndAlso reader.CVList.count = 3, "cvList count = 3")
            Call check(reader.Experiment IsNot Nothing AndAlso reader.Experiment.seed = "2024", "experiment seed")
            Call check(reader.Experiment.GetParam(VcOntology.PassiveDiffusion) IsNot Nothing, "experiment cvParam")
            Call check(reader.Species.Length = SpeciesCount, "species count")
            Call check(reader.GrowthSpace.shape.width = 2 AndAlso reader.GrowthSpace.shape.depth = 2, "growth space shape")
            Call check(reader.CultureMedium.components.count = 2, "medium components")
            Call check(reader.Perturbations.Length = 1, "perturbations count")
            Call check(reader.Perturbations(0).IsActive(3) AndAlso Not reader.Perturbations(0).IsActive(1),
                       "perturbation time window")
            Call check(reader.Layout IsNot Nothing AndAlso reader.Layout.VoxelAxis.count = VoxelCount, "layout voxel axis")
            Call check(reader.Layout.SpeciesAxis.count = SpeciesCount, "layout species axis")
            Call check(Not String.IsNullOrEmpty(reader.Checksum), "checksum")

            Dim expectedFrames As Integer = SpeciesCount * 3 * TickCount * VoxelCount

            Call check(reader.allFrames.Length = expectedFrames,
                       $"allFrames = {expectedFrames} (actual {reader.allFrames.Length})")

            Dim frames As List(Of XML.offset) = reader.getStreamIndex("transcriptome")("mass_profile")

            Call check(frames.Count = SpeciesCount * TickCount * VoxelCount,
                       "transcriptome/mass_profile frame count")

            Dim rnaIds As String() = reader.getStreamEntities("transcriptome", "mass_profile", "sp1")

            Call check(rnaIds.Length = RnaCount, "entity count for sp1/transcriptome")

            ' 逐帧数值比对（抽 12 条，覆盖两个物种）
            Dim sample As XML.offset() = frames _
                .OrderBy(Function(o) o.tick) _
                .ThenBy(Function(o) o.species) _
                .ThenBy(Function(o) o.voxelStart) _
                .Take(12) _
                .ToArray
            Dim mismatches As Integer = 0

            For Each offset As XML.offset In sample
                Dim speciesIndex As Integer = If(offset.species = "sp1", 0, 1)
                Dim expected As Double() = makeValues(RnaCount, offset.tick, offset.voxelStart, speciesIndex)
                Dim actual As Double() = reader.getFrameVector(offset.offset)

                If actual.Length <> expected.Length Then
                    mismatches += 1
                    Continue For
                End If

                For i As Integer = 0 To expected.Length - 1
                    If Math.Abs(actual(i) - expected(i)) > 1.0E-12 Then
                        mismatches += 1
                        Exit For
                    End If
                Next
            Next

            Call check(mismatches = 0, $"frame values round-trip (mismatches={mismatches})")

            ' 切片：单个体素的块应与原值一致
            Dim one As XML.offset = sample(0)
            Dim part As Double() = reader.getFrameVector(one, one.voxelStart)
            Dim expectedOne As Double() = makeValues(RnaCount, one.tick, one.voxelStart,
                                                     If(one.species = "sp1", 0, 1))

            Call check(part.Length = RnaCount, "voxel slice length")
            Call check(Math.Abs(part(0) - expectedOne(0)) < 1.0E-12, "voxel slice value")

            Dim singleValue As Double = reader.getFrameVector(one, one.voxelStart, 3)

            Call check(Math.Abs(singleValue - expectedOne(3)) < 1.0E-12, "single entity value")
        End Using
    End Sub

    ' ---------------------------------------------------------------
    ' 1.x 拒绝
    ' ---------------------------------------------------------------
    Private Sub verifyLegacyRejected(dir As String)
        Dim legacy As String = Path.Combine(dir, "legacy.vcxml")

        Call File.WriteAllText(legacy,
            "<?xml version=""1.0"" encoding=""utf8""?>" & vbCrLf &
            "<vcXML xmlns=""https://biocad.gcmodeller.org/XML/schema_revision/vcellXML_1.10.33"">" & vbCrLf &
            "</vcXML>" & vbCrLf &
            "<indexOffset>0</indexOffset>" & vbCrLf)

        Try
            Using reader As New Reader(legacy)
            End Using

            Call check(False, "1.x file should throw NotSupportedException")
        Catch ex As NotSupportedException
            Call check(True, "1.x file rejected with NotSupportedException")
        Catch ex As Exception
            Call check(False, $"1.x file threw {ex.GetType().Name} instead of NotSupportedException")
        End Try
    End Sub

    ' ---------------------------------------------------------------
    ' 辅助
    ' ---------------------------------------------------------------
    Private Function makeIds(prefix As String, count As Integer) As String()
        Dim ids As String() = New String(count - 1) {}

        For i As Integer = 0 To count - 1
            ids(i) = $"{prefix}_{i:D3}"
        Next

        Return ids
    End Function

    ''' <summary>可预测的数值：tick × 1000 + voxel × 100 + entity + species × 10</summary>
    Private Function makeValues(count As Integer, tick As Integer, voxel As Integer, speciesIndex As Integer) As Double()
        Dim values As Double() = New Double(count - 1) {}

        For i As Integer = 0 To count - 1
            values(i) = tick * 1000.0 + voxel * 100.0 + i + speciesIndex * 10.0
        Next

        Return values
    End Function

    Private Sub check(condition As Boolean, name As String)
        If condition Then
            Call Console.WriteLine($"[ok]   {name}")
        Else
            failures += 1
            Call Console.WriteLine($"[FAIL] {name}")
        End If
    End Sub

End Module
