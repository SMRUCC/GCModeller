Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.NCBI
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.ContextModel
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("annotation.genomics", Category:=APICategories.ResearchTools, Publisher:="xie.guigang@gcmodeller.org")>
Module genomics

    <ExportAPI("read.gtf")>
    Public Function readGtf(file As String) As PTT
        Return Gtf.ParseFile(file)
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
