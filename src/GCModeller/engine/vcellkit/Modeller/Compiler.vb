﻿#Region "Microsoft.VisualBasic::ad10afa8c30f74480bbdc81b2d0ad6fd, engine\vcellkit\Modeller\Compiler.vb"

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

'   Total Lines: 179
'    Code Lines: 115 (64.25%)
' Comment Lines: 48 (26.82%)
'    - Xml Docs: 100.00%
' 
'   Blank Lines: 16 (8.94%)
'     File Size: 8.46 KB


' Module Compiler
' 
'     Function: AssemblingGenomeInformation, AssemblingMetabolicNetwork, AssemblingRegulationNetwork, compileBiocyc, kegg
'               load_geneKOMapping, ToMarkup
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.Data.BioCyc
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.GCModeller.Compiler
Imports SMRUCC.genomics.GCModeller.Compiler.MarkupCompiler
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH
Imports SMRUCC.Rsharp.Interpreter.ExecuteEngine
Imports SMRUCC.Rsharp.Interpreter.ExecuteEngine.ExpressionSymbols.DataSets
Imports SMRUCC.Rsharp.Interpreter.ExecuteEngine.ExpressionSymbols.Operators
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports gccWorkflow = SMRUCC.genomics.GCModeller.Compiler.Workflow
Imports Rdataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

''' <summary>
''' The GCModeller virtual cell model creator
''' </summary>
<Package("compiler")>
Module Compiler

    ''' <summary>
    ''' create kegg repository arguments
    ''' </summary>
    ''' <param name="compounds">A directory path which the folder contains the compounds data models.</param>
    ''' <param name="maps">A directory path which the folder contains the kegg reference map data.</param>
    ''' <param name="reactions"></param>
    ''' <param name="glycan2Cpd"></param>
    ''' <returns></returns>
    <ExportAPI("kegg")>
    Public Function kegg(compounds$, maps$, reactions$, glycan2Cpd As Dictionary(Of String, String())) As RepositoryArguments
        Return New RepositoryArguments With {
            .KEGGCompounds = compounds,
            .KEGGPathway = maps,
            .KEGGReactions = reactions,
            .Glycan2Cpd = glycan2Cpd _
                .ToDictionary(Function(t) t.Key,
                              Function(t)
                                  Return t.Value(Scan0)
                              End Function)
        }
    End Function

    ''' <summary>
    ''' create a list that map gene id to KO id.
    ''' </summary>
    ''' <param name="data">any kind of dataframe dataset.</param>
    ''' <param name="KOcol">the column name for get KO term id</param>
    ''' <param name="geneIDcol">the column name for get gene id</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("geneKO.maps")>
    <RApiReturn(GetType(list))>
    Public Function load_geneKOMapping(<RRawVectorArgument> data As Object,
                                       Optional KOcol$ = "KO",
                                       Optional geneIDcol$ = "ID",
                                       Optional env As Environment = Nothing) As Object
        If data Is Nothing Then
            Return RInternal.debug.stop("No gene_id to KO mapping data provided!", env)
        End If

        If TypeOf data Is Rdataframe Then
            If Not DirectCast(data, Rdataframe).columns.ContainsKey(geneIDcol) Then
                Return RInternal.debug.stop($"No geneId column data which is named: '{geneIDcol}'!", env)
            ElseIf Not DirectCast(data, Rdataframe).columns.ContainsKey(KOcol) Then
                Return RInternal.debug.stop($"NO KEGG id column data which is named: '{KOcol}'!", env)
            End If

            Dim geneID As String() = CLRVector.asCharacter(DirectCast(data, Rdataframe).getColumnVector(geneIDcol))
            Dim KO As String() = CLRVector.asCharacter(DirectCast(data, Rdataframe).getColumnVector(KOcol))

            Return geneID _
                .SeqIterator _
                .ToDictionary(Function(id) id.value,
                              Function(i)
                                  Return KO(i)
                              End Function)
        ElseIf TypeOf data Is EntityObject() Then
            Return DirectCast(data, EntityObject()) _
                .ToDictionary(Function(protein) protein.ID,
                              Function(protein)
                                  Return protein(KOcol)
                              End Function)
        ElseIf TypeOf data Is BiDirectionalBesthit() Then
            Return DirectCast(data, BiDirectionalBesthit()) _
                .ToDictionary(Function(protein) protein.QueryName,
                              Function(protein)
                                  Return protein.term
                              End Function)
        ElseIf TypeOf data Is pipeline AndAlso DirectCast(data, pipeline).elementType Like GetType(BiDirectionalBesthit) Then
            Return DirectCast(data, pipeline) _
                .populates(Of BiDirectionalBesthit)(env) _
                .ToDictionary(Function(protein) protein.QueryName,
                              Function(protein)
                                  Return protein.term
                              End Function)
        End If

        Return RInternal.debug.stop(New NotImplementedException(data.GetType.FullName), env)
    End Function

    ''' <summary>
    ''' create genome framework for the virtual cell model
    ''' </summary>
    ''' <param name="replicons"></param>
    ''' <param name="geneKO"></param>
    ''' <param name="lociAsLocus_tag"></param>
    ''' <returns></returns>
    <ExportAPI("assembling.genome")>
    Public Function AssemblingGenomeInformation(replicons As Dictionary(Of String, GBFF.File),
                                                geneKO As Dictionary(Of String, String),
                                                Optional lociAsLocus_tag As Boolean = False) As CellularModule

        Return gccWorkflow.AssemblingGenomeInformation(replicons, geneKO, lociAsLocus_tag)
    End Function

    ''' <summary>
    ''' add metabolism network information for the virtual cell model
    ''' </summary>
    ''' <param name="cell"></param>
    ''' <param name="geneKO"></param>
    ''' <param name="repo"></param>
    ''' <returns></returns>
    <ExportAPI("assembling.metabolic")>
    Public Function AssemblingMetabolicNetwork(cell As CellularModule,
                                               geneKO As Dictionary(Of String, String),
                                               repo As RepositoryArguments) As CellularModule

        Return gccWorkflow.AssemblingMetabolicNetwork(cell, geneKO, repo)
    End Function

    ''' <summary>
    ''' add transcription regulation network information for the virtual cell model
    ''' </summary>
    ''' <param name="model"></param>
    ''' <param name="regulations"></param>
    ''' <returns></returns>
    <ExportAPI("assembling.TRN")>
    Public Function AssemblingRegulationNetwork(model As CellularModule, regulations As RegulationFootprint()) As CellularModule
        Return gccWorkflow.AssemblingRegulationNetwork(model, regulations)
    End Function

    ''' <summary>
    ''' Save the virtual cell data model in XML file format.
    ''' </summary>
    ''' <param name="model"></param>
    ''' <param name="genomes"></param>
    ''' <param name="KEGG"></param>
    ''' <param name="regulations"></param>
    ''' <param name="lociAsLocus_tag"></param>
    ''' <returns></returns>
    <ExportAPI("vcell.markup")>
    Public Function ToMarkup(model As CellularModule,
                             genomes As Dictionary(Of String, GBFF.File),
                             KEGG As RepositoryArguments,
                             regulations As RegulationFootprint(),
                             Optional lociAsLocus_tag As Boolean = False,
                             Optional logfile As String = Nothing) As VirtualCell

        Using compiler As New v2KEGGCompiler(model, genomes, KEGG, regulations, lociAsLocus_tag)
            Return compiler.Compile($"compile --log {logfile.CLIPath}")
        End Using
    End Function

    ''' <summary>
    ''' Build a specific organism metacyc database as virtual cell model 
    ''' </summary>
    ''' <param name="biocyc"></param>
    ''' <param name="genomes"></param>
    ''' <param name="logfile"></param>
    ''' <returns></returns>
    <ExportAPI("compile.biocyc")>
    Public Function compileBiocyc(biocyc As Workspace, genomes As Dictionary(Of String, GBFF.File),
                                  Optional logfile As String = "./gcc.log") As VirtualCell

        Using compiler As New BioCyc.v2Compiler(genomes.First.Value, biocyc)
            Return compiler.Compile($"compile --log {logfile.CLIPath}")
        End Using
    End Function

    <ExportAPI("compile_network")>
    Public Function compile_network(<RListObjectArgument> <RLazyExpression> x As list, Optional env As Environment = Nothing) As Object
        Dim network As New List(Of Reaction)

        For Each expr As Expression In x.data.OfType(Of Expression)
            If Not TypeOf expr Is BinaryExpression Then
                Continue For
            End If

            Call network.Add(buildReaction(expr))
        Next

        Return New VirtualCell With {
            .metabolismStructure = New MetabolismStructure With {
                .reactions = New ReactionGroup With {
                    .none_enzymatic = network.ToArray
                },
                .compounds = network _
                    .Select(Function(r) r.AsEnumerable) _
                    .IteratesALL _
                    .GroupBy(Function(c) c.compound) _
                    .Select(Function(id) New Compound(id.Key)) _
                    .ToArray
            }
        }
    End Function

    Private Function buildReaction(eq As BinaryExpression) As Reaction
        Dim left = getFactors(eq.left).ToArray
        Dim right = getFactors(eq.right).ToArray

        Return New Reaction With {
            .bounds = {10, 10},
            .substrate = left,
            .product = right,
            .ID = .equation,
            .name = .ID
        }
    End Function

    Private Iterator Function getFactors(side As Expression) As IEnumerable(Of CompoundFactor)
        If TypeOf side Is BinaryExpression Then
            Dim bin = DirectCast(side, BinaryExpression)

            If bin.operator = "+" Then
                ' A + B
                For Each xi As CompoundFactor In getFactors(bin.left).JoinIterates(getFactors(bin.right))
                    Yield xi
                Next
            ElseIf bin.operator = "*" Then
                ' 2 * A
                Dim factor = CLRVector.asNumeric(bin.left.Evaluate(Nothing))
                Dim id As String = DirectCast(bin.right, SymbolReference).symbol

                Yield New CompoundFactor(factor(0), id)
            Else
                Throw New SyntaxErrorException
            End If
        ElseIf TypeOf side Is SymbolReference Then
            Dim x As SymbolReference = DirectCast(side, SymbolReference)
            Yield New CompoundFactor(1, x.symbol)
        Else
            Throw New InvalidDataException
        End If
    End Function
End Module
