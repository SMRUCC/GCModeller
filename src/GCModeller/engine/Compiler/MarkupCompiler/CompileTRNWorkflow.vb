#Region "Microsoft.VisualBasic::ba74be84523a615c66a7d2411d4380df, engine\Compiler\MarkupCompiler\CompileTRNWorkflow.vb"

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

    '     Class CompileTRNWorkflow
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: getIdMapper, getTFregulations, processingName
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Process

Namespace MarkupCompiler

    Public Class CompileTRNWorkflow : Inherits CompilerWorkflow

        Public Sub New(compiler As v2MarkupCompiler)
            MyBase.New(compiler)
        End Sub

        Private Shared Function processingName(ByRef name As String) As String
            name = LCase(name).Trim(" "c, ASCII.TAB, ASCII.CR, ASCII.LF, "-"c, ";"c)
            Return name
        End Function

        Private Function getIdMapper() As Func(Of String, String)
            Dim allCompounds '= compiler.KEGG.GetCompounds.Compounds
            ' lower name -> cid mapping
            Dim mapperIndex As New Dictionary(Of String, String)
            Dim invalidNames As New Index(Of String)

            For Each keggCompound In allCompounds _
                .Select(Function(c) c.Entity) _
                .OrderBy(Function(c)
                             ' Return c.entry.Match("\d+").DoCall(AddressOf Integer.Parse)
                         End Function)

                For Each name As String In keggCompound.commonNames
                    mapperIndex(processingName(name)) = keggCompound.entry
                Next
            Next

            Return Function(name)
                       Dim rawName As String = name

                       If mapperIndex.ContainsKey(processingName(name)) Then
                           Return mapperIndex(name)
                       Else
                           If Not name = "" Then
                               If Not rawName Like invalidNames Then
                                   Call compiler.CompileLogging.WriteLine($"no mapped kegg compound id for name: {name}!", NameOf(getIdMapper))
                                   Call invalidNames.Add(rawName)
                               End If
                           End If

                           Return Nothing
                       End If
                   End Function
        End Function

        Friend Iterator Function getTFregulations() As IEnumerable(Of transcription)
            Dim centralDogmas '= compiler.model.Genotype.centralDogmas.ToDictionary(Function(d) d.geneID)
            Dim getId As Func(Of String, String) = getIdMapper()

            Call compiler.CompileLogging.WriteLine("create transcripting regulation network")

            'For Each reg As RegulationFootprint In compiler.regulations
            '    Dim process As CentralDogma = centralDogmas.TryGetValue(reg.regulated)

            '    If process.geneID.StringEmpty Then
            '        Call compiler.CompileLogging.WriteLine($"{reg.ToString}: {reg.regulated} process not found!", type:=MSG_TYPES.WRN)
            '    End If

            '    If reg.motif Is Nothing Then
            '        reg.motif = New NucleotideLocation
            '    End If

            '    Yield New transcription With {
            '        .biological_process = reg.biological_process,
            '        .effector = reg.effector _
            '            .StringSplit("\s*;\s*") _
            '            .Select(getId) _
            '            .Where(Function(cid) Not cid.StringEmpty) _
            '            .ToArray,
            '        .mode = reg.mode,
            '        .regulator = reg.regulator,
            '        .motif = New Motif With {
            '            .family = reg.family,
            '            .left = reg.motif.left,
            '            .right = reg.motif.right,
            '            .strand = reg.motif.Strand.GetBriefCode,
            '            .sequence = reg.sequenceData,
            '            .distance = reg.distance
            '        },
            '        .centralDogma = process.ToString
            '    }
            'Next
        End Function
    End Class
End Namespace
