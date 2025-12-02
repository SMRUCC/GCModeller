#Region "Microsoft.VisualBasic::76696fd48b143a291de8674abc406ddc, core\Bio.Assembly\Assembly\NCBI\Database\GenBank\ExportServices\GBFFFeatureDumps.vb"

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


    ' Code Statistics:

    '   Total Lines: 181
    '    Code Lines: 142 (78.45%)
    ' Comment Lines: 19 (10.50%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 20 (11.05%)
    '     File Size: 8.01 KB


    '     Module GBFFFeatureDumps
    ' 
    '         Function: FeatureDumps, GbffToPTT, GetmRNASequence, InternalDump3UTRs, InternalDump5UTRs
    '                   InternalDumpCDS, InternalDumpMiscFeature, InternalDumpRegulatory
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel

Namespace Assembly.NCBI.GenBank

    Public Module GBFFFeatureDumps

        ''' <summary>
        ''' Dump feature sites information data into a tabular dataframe.
        ''' </summary>
        ''' <param name="gb"></param>
        ''' <param name="features"></param>
        ''' <param name="dumpAll"></param>
        ''' <returns></returns>
        <ExportAPI("Features.Dump")>
        <Extension>
        Public Function FeatureDumps(gb As GBFF.File,
                                     Optional features As String() = Nothing,
                                     Optional dumpAll As Boolean = False) As GeneTable()
            If dumpAll Then
                Dim fs As Feature() =
                    LinqAPI.Exec(Of Feature) <= From x As Feature
                                                In gb.Features
                                                Where x.ContainsKey("gene")
                                                Select x
                Return fs.InternalDumpCDS
            End If

            If features Is Nothing Then features = {"5'UTR", "CDS", "regulatory", "misc_feature", "3'UTR"}

            Dim result As New List(Of GeneTable)

            For Each feature As String In features
                Dim fs As Feature() = gb.Features.ListFeatures(feature).ToArray
                result += _dumpMethods(feature)(fs)
            Next

            Return result.ToArray
        End Function

#Region "Dump Methods"

        ''' <summary>
        ''' Method delegates for feature dumping
        ''' </summary>
        ReadOnly _dumpMethods As New Dictionary(Of String, Func(Of Feature(), GeneTable())) From {
                {"5'UTR", AddressOf InternalDump5UTRs},
                {"3'UTR", AddressOf InternalDump3UTRs},
                {"CDS", AddressOf InternalDumpCDS},
                {"regulatory", AddressOf InternalDumpRegulatory},
                {"misc_feature", AddressOf InternalDumpMiscFeature}
        }

        Private Function InternalDumpMiscFeature(features As Feature()) As GeneTable()
            Dim dump As GeneTable() =
                LinqAPI.Exec(Of Feature, GeneTable)(features) <=
                    Function(feature As Feature) New GeneTable With {
                        .COG = "misc_feature",
                        .function = feature("note"),
                        .commonName = feature("note"),
                        .Location = feature.Location.ContiguousRegion,
                        .locus_id = feature("locus_tag"),
                        .geneName = feature("gene") & "_mics_feature",
                        .translation = feature("translation"),
                        .ProteinId = feature("protein_id"),
                        .CDS = feature.SequenceData
                    }
            Return dump
        End Function

        Private Function InternalDumpRegulatory(features As Feature()) As GeneTable()
            Dim dump As GeneTable() = features.Select(
                Function(feature) New GeneTable With {
                    .COG = "regulatory",
                    .function = feature("regulatory_class"),
                    .commonName = feature("note"),
                    .Location = feature.Location.ContiguousRegion,
                    .locus_id = feature("locus_tag"),
                    .geneName = feature("gene") & "_regulatory",
                    .translation = feature("translation"),
                    .ProteinId = feature("protein_id"),
                    .CDS = feature.SequenceData
               }).ToArray
            Return dump
        End Function

        <Extension>
        Private Function InternalDumpCDS(features As Feature()) As GeneTable()
            Dim dump As GeneTable() = features.Select(
                Function(feature) New GeneTable With {
                    .COG = "CDS",
                    .function = feature("function"),
                    .commonName = feature("note"),
                    .Location = feature.Location.ContiguousRegion,
                    .locus_id = feature("locus_tag"),
                    .geneName = feature("gene"),
                    .translation = feature("translation"),
                    .ProteinId = feature("protein_id"),
                    .CDS = feature.SequenceData
               }).ToArray
            Return dump
        End Function

        <Extension> Private Function InternalDump5UTRs(features As Feature()) As GeneTable()
            Dim dump As GeneTable() = features.Select(
                Function(feature) New GeneTable With {
                    .COG = "5'UTR",
                    .function = feature("function"),
                    .commonName = feature("note"),
                    .Location = feature.Location.ContiguousRegion,
                    .locus_id = $"5'UTR_{feature.Location.ContiguousRegion.left}..{feature.Location.ContiguousRegion.right}",
                    .geneName = $"5'UTR_{feature.Location.ContiguousRegion.left}..{feature.Location.ContiguousRegion.right}",
                    .CDS = feature.SequenceData
                }).ToArray
            Return dump
        End Function

        <Extension> Private Function InternalDump3UTRs(features As Feature()) As GeneTable()
            Dim dump As GeneTable() = features.Select(
                Function(feature) New GeneTable With {
                    .COG = "3'UTR",
                    .function = feature("function"),
                    .commonName = feature("note"),
                    .Location = feature.Location.ContiguousRegion,
                    .locus_id = $"3'UTR_{feature.Location.ContiguousRegion.left}..{feature.Location.ContiguousRegion.right}",
                    .geneName = $"3'UTR_{feature.Location.ContiguousRegion.left}..{feature.Location.ContiguousRegion.right}",
                    .CDS = feature.SequenceData
                }).ToArray
            Return dump
        End Function
#End Region

        <Extension>
        Public Function GbffToPTT(contextInfo As IEnumerable(Of GeneTable), size%, Optional title$ = "Unknown") As PTT
            Dim genes As GeneBrief() = contextInfo _
                .Select(Function(context) GeneBrief.CreateObject(g:=context)) _
                .ToArray
            Dim description As New PTT With {
                .GeneObjects = genes,
                .Size = size,
                .Title = title
            }

            Return description
        End Function

        ''' <summary>
        ''' get mRNA sequence
        ''' </summary>
        ''' <returns>
        ''' the DNA sequence of the target <paramref name="mRNA"/> gene.
        ''' </returns>
        ''' <remarks>
        ''' Cut and join genomics DNA sequence for the given mRNA features.
        ''' </remarks>
        <Extension>
        Public Function GetmRNASequence(gb As GBFF.File, mRNA As Feature) As String
            Dim locs = mRNA.Location

            If locs.Locations.IsNullOrEmpty Then
                Return mRNA.SequenceData
            Else
                Dim extrons As New List(Of String)

                For Each loc As NucleotideLocation In locs.JoinLocations
                    Call extrons.Add(gb.Origin.CutSequenceLinear(loc).SequenceData)
                Next

                Return extrons.JoinBy("")
            End If
        End Function
    End Module
End Namespace
