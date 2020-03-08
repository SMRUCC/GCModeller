Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.NCBI
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.ContextModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Interop
Imports featureLocation = SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES.Location
Imports gbffFeature = SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES.Feature

<Package("annotation.genomics", Category:=APICategories.ResearchTools, Publisher:="xie.guigang@gcmodeller.org")>
Module genomics

    <ExportAPI("read.gtf")>
    Public Function readGtf(file As String) As GeneBrief()
        Return Gtf.ParseFile(file)
    End Function

    <ExportAPI("as.tabular")>
    Public Function asTable(genes As GeneBrief(), Optional title$ = "n/a", Optional size% = 0, Optional format$ = "PTT", Optional env As Environment = Nothing) As Object
        Select Case UCase(format)
            Case "PTT"
                Return New PTT(genes, title, size)
            Case "GFF"
                Return Internal.debug.stop(New NotImplementedException, env)
            Case "GTF"
                Return Internal.debug.stop(New NotImplementedException, env)
            Case Else
                Return Internal.debug.stop($"unsupported table format: '{format}'!", env)
        End Select
    End Function

    <ExportAPI("read.genbank")>
    Public Function readGenbank(file As String,
                                Optional repliconTable As Boolean = False,
                                Optional env As Environment = Nothing) As Object

        If Not file.FileExists(True) Then
            Return Internal.debug.stop($"invalid file resource: '{file}'!", env)
        End If

        If repliconTable Then
            Return GenBank.loadRepliconTable(file)
        Else
            Return GBFF.File.Load(file)
        End If
    End Function

    <ExportAPI("write.genbank")>
    Public Function writeGenbank(gb As GBFF.File, file$, Optional env As Environment = Nothing) As Object
        If gb Is Nothing Then
            Return Internal.debug.stop("write data is nothing!", env)
        Else
            Return gb.Save(file)
        End If
    End Function

    <ExportAPI("as.genbank")>
    <RApiReturn(GetType(GBFF.File))>
    Public Function asGenbank(<RRawVectorArgument> x As Object, Optional env As Environment = Nothing) As Object
        If x Is Nothing Then
            env.AddMessage("The genome information data object is nothing!", MSG_TYPES.WRN)
            Return Nothing
        End If

        If TypeOf x Is PTT Then
            Return DirectCast(x, PTT).CreateGenbankObject
        Else
            Return Internal.debug.stop(New NotImplementedException(x.GetType.FullName), env)
        End If
    End Function

    <ExportAPI("add.origin.fasta")>
    Public Function addNtOrigin(gb As GBFF.File, nt As FastaSeq) As GBFF.File
        gb.Origin = New ORIGIN With {
            .Headers = nt.Headers,
            .SequenceData = nt.SequenceData
        }
        gb.Features.SetSourceFeature(New gbffFeature With {
            .KeyName = "source",
            .Location = New featureLocation With {
                .Complement = False,
                .Locations = {
                    New RegionSegment With {.Left = 1, .Right = nt.Length}
                }
            }
        })

        Return gb
    End Function

    <ExportAPI("add.protein.fasta")>
    Public Function addproteinSeq(gb As GBFF.File, <RRawVectorArgument> proteins As Object, Optional env As Environment = Nothing) As Object
        Dim seqs = GetFastaSeq(proteins)

        If seqs Is Nothing Then
            Return Internal.debug.stop("no protein sequence data provided!", env)
        End If

        Dim seqTable = seqs.ToDictionary(Function(fa) fa.Title.Split.First)
        Dim geneId As String
        Dim prot As FastaSeq

        For Each feature In gb.Features.Where(Function(a) a.KeyName = "CDS")
            geneId = feature.Query(FeatureQualifiers.locus_tag)
            prot = seqTable.TryGetValue(geneId)

            If prot Is Nothing Then
                env.AddMessage($"missing protein sequence for '{geneId}'...", MSG_TYPES.WRN)
                Continue For
            End If

            feature.SetValue(FeatureQualifiers.translation, prot.SequenceData)
            feature.SetValue(FeatureQualifiers.product, prot.Title)
        Next

        Return gb
    End Function

    <ExportAPI("upstream")>
    Public Function getUpstream(<RRawVectorArgument>
                                context As GeneBrief(),
                                Optional length% = 200,
                                Optional isRelativeOffset As Boolean = False) As NucleotideLocation()
        Return context _
            .Select(Function(gene)
                        Dim loci As NucleotideLocation = gene.Location

                        If isRelativeOffset Then
                            If loci.Strand = Strands.Forward Then
                                loci = New NucleotideLocation(loci.left - length, loci.left, Strands.Forward) With {
                                    .tag = loci.ToString & $"|offset=-{length}"
                                }
                            Else
                                loci = New NucleotideLocation(loci.right, loci.right + length, Strands.Reverse) With {
                                    .tag = loci.ToString & $"|offset=+{length}"
                                }
                            End If
                        Else
                            If loci.Strand = Strands.Forward Then
                                loci = loci - length
                            Else
                                loci = loci + length
                            End If
                        End If

                        Return loci
                    End Function) _
            .ToArray
    End Function

    <ExportAPI("genome.genes")>
    <RApiReturn(GetType(GeneBrief()))>
    Public Function genes(<RRawVectorArgument> genome As Object, Optional env As Environment = Nothing) As Object
        If genome Is Nothing Then
            Return {}
        End If

        If TypeOf genome Is PTT Then
            Return DirectCast(genome, PTT).GeneObjects
        Else
            Return Internal.debug.stop($"Invalid genome context model: {genome.GetType.FullName}!", env)
        End If
    End Function

End Module
