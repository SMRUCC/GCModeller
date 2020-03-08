
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.NCBI
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Interop
Imports featureLocation = SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES.Location
Imports gbffFeature = SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES.Feature

<Package("annotation.genbank_kit", Category:=APICategories.UtilityTools, Publisher:="xie.guigang@gcmodeller.org")>
Module genbankKit

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
End Module
