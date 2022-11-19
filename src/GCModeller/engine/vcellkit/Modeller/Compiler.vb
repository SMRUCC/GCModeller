#Region "Microsoft.VisualBasic::b29df7f87b0792d8396261e43cca0763, GCModeller\engine\vcellkit\Modeller\Compiler.vb"

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

    '   Total Lines: 175
    '    Code Lines: 112
    ' Comment Lines: 48
    '   Blank Lines: 15
    '     File Size: 8.14 KB


    ' Module Compiler
    ' 
    '     Function: AssemblingGenomeInformation, AssemblingMetabolicNetwork, AssemblingRegulationNetwork, compileBiocyc, kegg
    '               load_geneKOMapping, ToMarkup
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Data.BioCyc
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.GCModeller.Compiler
Imports SMRUCC.genomics.GCModeller.Compiler.MarkupCompiler
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports gccWorkflow = SMRUCC.genomics.GCModeller.Compiler.Workflow
Imports Rdataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe

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
            Return Internal.debug.stop("No gene_id to KO mapping data provided!", env)
        End If

        If TypeOf data Is Rdataframe Then
            If Not DirectCast(data, Rdataframe).columns.ContainsKey(geneIDcol) Then
                Return Internal.debug.stop($"No geneId column data which is named: '{geneIDcol}'!", env)
            ElseIf Not DirectCast(data, Rdataframe).columns.ContainsKey(KOcol) Then
                Return Internal.debug.stop($"NO KEGG id column data which is named: '{KOcol}'!", env)
            End If

            Dim geneID As String() = asVector(Of String)(DirectCast(data, Rdataframe).getColumnVector(geneIDcol))
            Dim KO As String() = asVector(Of String)(DirectCast(data, Rdataframe).getColumnVector(KOcol))

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

        Return Internal.debug.stop(New NotImplementedException(data.GetType.FullName), env)
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

    <ExportAPI("compile.biocyc")>
    Public Function compileBiocyc(biocyc As Workspace, genomes As Dictionary(Of String, GBFF.File), Optional logfile As String = "./gcc.log") As VirtualCell
        Using compiler As New BioCyc.v2Compiler(genomes.First.Value, biocyc)
            Return compiler.Compile($"compile --log {logfile.CLIPath}")
        End Using
    End Function
End Module
