#Region "Microsoft.VisualBasic::c0f057af1b8f93c619bf8e27fb4ae8ab, R#\seqtoolkit\Annotations\genomics.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xie (genetics@smrucc.org)
'       xieguigang (xie.guigang@live.com)
' 
' Copyright (c) 2018 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
' 
' 
' This program is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 3 of the License, or
' (at your option) any later version.
' 
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
' 
' You should have received a copy of the GNU General Public License
' along with this program. If not, see <http://www.gnu.org/licenses/>.



' /********************************************************************************/

' Summaries:

' Module genomics
' 
'     Function: asTable, genes, getUpstream, readGtf
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
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
    Public Function readGtf(file As String) As GeneBrief()
        Return Gtf.ParseFile(file)
    End Function

    <ExportAPI("as.tabular")>
    Public Function asTable(genes As GeneBrief(), Optional title$ = "n/a", Optional size% = 0, Optional format$ = "PTT|GFF|GTF", Optional env As Environment = Nothing) As Object
        Select Case Strings.UCase(format).Split("|"c).FirstOrDefault
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
    <RApiReturn(GetType(GeneBrief))>
    Public Function genes(<RRawVectorArgument> genome As Object, Optional env As Environment = Nothing) As Object
        If genome Is Nothing Then
            Return {}
        End If

        If TypeOf genome Is PTT Then
            Return DirectCast(genome, PTT).GeneObjects
        ElseIf TypeOf genome Is GBFF.File Then
            Return DirectCast(genome, GBFF.File).EnumerateGeneFeatures(ORF:=False).FeatureGenes.ToArray
        Else
            Return Internal.debug.stop($"Invalid genome context model: {genome.GetType.FullName}!", env)
        End If
    End Function

End Module

