#Region "Microsoft.VisualBasic::41b9381f334bd35e1312d4ee9f520fce, seqtoolkit\Annotations\context.vb"

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

' Module context
' 
'     Constructor: (+1 Overloads) Sub New
' 
'     Function: context, contextSummary, getNtLocation, isForward, location
'               offsetLocation, relationship
' 
'     Sub: Main
' 
' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal
Imports SMRUCC.Rsharp.Runtime.Internal.ConsolePrinter
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports IContext = SMRUCC.genomics.ContextModel.Context

<Package("annotation.genomics_context", Category:=APICategories.ResearchTools)>
Module context

    Sub New()
        Call Main()
    End Sub

    <RInitialize>
    Private Sub Main()
        Call generic.add("summary", GetType(IContext), AddressOf contextSummary)

        Call printer.AttachConsoleFormatter(Of NucleotideLocation)(Function(o) o.ToString)
    End Sub

    Private Function contextSummary(x As IContext, args As list, env As Environment) As Object
        Dim sb As New StringBuilder
        Dim nt As FastaSeq = args.getValue(Of FastaSeq)("nt", env)

        Call sb.AppendLine($"summary of {x.tag}:")
        Call sb.AppendLine($"current feature: {x.feature.ToString}")
        Call sb.AppendLine($"distance: {x.distance} bp")
        Call sb.AppendLine()
        Call sb.AppendLine($"upstream location in given distance: {x.upstream.ToString}")
        If Not nt Is Nothing Then
            Call sb.AppendLine(nt.CutSequenceLinear(x.upstream).SequenceData)
        End If
        Call sb.AppendLine($"downstream location in given distance: {x.downstream.ToString}")
        If Not nt Is Nothing Then
            Call sb.AppendLine(nt.CutSequenceLinear(x.downstream).SequenceData)
        End If
        Call sb.AppendLine($"complement strand location of current: {x.antisense.ToString}")
        If Not nt Is Nothing Then
            Call sb.AppendLine(nt.CutSequenceLinear(x.antisense).SequenceData)
        End If

        Return sb.ToString
    End Function

    ''' <summary>
    ''' filter genes by given strand direction
    ''' </summary>
    ''' <param name="genes"></param>
    ''' <param name="strand"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("strand.filter")>
    Public Function strandFilter(<RRawVectorArgument> genes As Object,
                                 Optional strand As Object = "+",
                                 Optional env As Environment = Nothing) As Object

        Dim strVal As Strands = getStrand(strand)
        Dim geneObjects As pipeline = pipeline.TryCreatePipeline(Of IGeneBrief)(genes, env)

        If geneObjects.isError Then
            Return geneObjects.getError
        End If

        Return geneObjects _
            .populates(Of IGeneBrief)(env) _
            .Where(Function(gene)
                       If strVal = Strands.Unknown Then
                           Return True
                       Else
                           Return gene.Location.Strand = strVal
                       End If
                   End Function) _
            .DoCall(AddressOf vector.asVector)
    End Function

    Private Function getStrand(strand As Object) As Strands
        Dim strVal As Strands

        If strand Is Nothing Then
            strVal = Strands.Unknown
        ElseIf TypeOf strand Is Strands Then
            strVal = strand
        Else
            strVal = Scripting.ToString(strand).GetStrand
        End If

        Return strVal
    End Function

    ''' <summary>
    ''' create a new nucleotide location object
    ''' </summary>
    ''' <param name="left"></param>
    ''' <param name="right"></param>
    ''' <param name="strand"></param>
    ''' <returns></returns>
    <ExportAPI("location")>
    Public Function location(left As Integer, right As Integer, Optional strand As Object = Nothing) As Object
        Return New NucleotideLocation(left, right, getStrand(strand))
    End Function

    ''' <summary>
    ''' the given nucleotide location is in forward direction
    ''' </summary>
    ''' <param name="loci"></param>
    ''' <returns></returns>
    <ExportAPI("is.forward")>
    Public Function isForward(loci As NucleotideLocation) As Boolean
        Return loci.Strand = Strands.Forward
    End Function

    ''' <summary>
    ''' do offset of the given location
    ''' </summary>
    ''' <param name="loci"></param>
    ''' <param name="offset"></param>
    ''' <returns></returns>
    <ExportAPI("offset")>
    Public Function offsetLocation(loci As NucleotideLocation, offset As Integer) As NucleotideLocation
        Return loci + offset
    End Function

    ''' <summary>
    ''' Create a new context model of a specific genomics feature site.
    ''' </summary>
    ''' <param name="loci"></param>
    ''' <param name="distance"></param>
    ''' <param name="note"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("context")>
    <RApiReturn(GetType(IContext))>
    Public Function context(loci As Object, distance As Integer, Optional note As String = Nothing, Optional env As Environment = Nothing) As Object
        Dim pos As NucleotideLocation

        If loci Is Nothing Then
            Return Nothing
        ElseIf TypeOf loci Is IGeneBrief Then
            pos = DirectCast(loci, IGeneBrief).Location
            note = If(note, loci.ToString)
        ElseIf TypeOf loci Is Contig Then
            pos = DirectCast(loci, Contig).MappingLocation
            note = If(note, loci.ToString)
        ElseIf TypeOf loci Is NucleotideLocation Then
            pos = loci
        Else
            Return debug.stop(New InvalidCastException(loci.GetType.FullName), env)
        End If

        Return New IContext(pos, distance, note)
    End Function

    Public Function getNtLocation(x As Object) As NucleotideLocation
        If x Is Nothing Then
            Return Nothing
        ElseIf TypeOf x Is IGeneBrief Then
            Return DirectCast(x, IGeneBrief).Location
        ElseIf TypeOf x Is Contig Then
            Return DirectCast(x, Contig).MappingLocation
        ElseIf TypeOf x Is NucleotideLocation Then
            Return x
        Else
            Return Nothing
        End If
    End Function

    ''' <summary>
    ''' get the segment relationship of two location
    ''' </summary>
    ''' <param name="a"></param>
    ''' <param name="b"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("relationship")>
    <RApiReturn(GetType(SegmentRelationships))>
    Public Function relationship(a As Object, b As Object, Optional env As Environment = Nothing) As Object
        Dim loci1 As NucleotideLocation = getNtLocation(a)
        Dim loci2 As NucleotideLocation = getNtLocation(b)

        If loci1 Is Nothing AndAlso Not a Is Nothing Then
            Return debug.stop(New InvalidCastException(a.GetType.FullName), env)
        ElseIf loci2 Is Nothing AndAlso Not b Is Nothing Then
            Return debug.stop(New InvalidCastException(b.GetType.FullName), env)
        End If

        If loci1 Is Nothing OrElse loci2 Is Nothing Then
            Return SegmentRelationships.Blank
        Else
            Return LocusExtensions.GetRelationship(loci1, loci2)
        End If
    End Function

End Module
