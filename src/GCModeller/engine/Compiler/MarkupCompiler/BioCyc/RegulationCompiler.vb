#Region "Microsoft.VisualBasic::3e8f3593444396affd2b601e0e8e86af, engine\Compiler\MarkupCompiler\BioCyc\RegulationCompiler.vb"

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

    '   Total Lines: 55
    '    Code Lines: 47 (85.45%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 8 (14.55%)
    '     File Size: 2.17 KB


    '     Class RegulationCompiler
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: CreateRegulations
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Data.BioCyc
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2

Namespace MarkupCompiler.BioCyc

    Public Class RegulationCompiler

        ReadOnly biocyc As Workspace

        Sub New(biocyc As Workspace)
            Me.biocyc = biocyc
        End Sub

        Public Iterator Function CreateRegulations(operons As IEnumerable(Of TranscriptUnit)) As IEnumerable(Of transcription)
            Dim site_index As Dictionary(Of String, TranscriptUnit()) = operons _
                .Where(Function(o) Not o.sites.IsNullOrEmpty) _
                .Select(Function(o)
                            Return o.sites.Select(Function(id) (id, o))
                        End Function) _
                .IteratesALL _
                .GroupBy(Function(t) t.id) _
                .ToDictionary(Function(t) t.Key,
                              Function(t)
                                  Return t.Select(Function(o) o.Item2).ToArray
                              End Function)

            For Each reg As regulation In biocyc.regulation.features
                Dim transcripts = site_index.TryGetValue(reg.regulatedEntity)

                If reg.types(0) <> "Transcription-Factor-Binding" Then
                    Continue For
                End If

                Yield New transcription With {
                    .regulator = reg.regulator,
                    .mode = reg.mode,
                    .centralDogma = transcripts _
                        .SafeQuery _
                        .Select(Function(t) t.id) _
                        .ToArray,
                    .note = reg.comment,
                    .targets = transcripts _
                        .SafeQuery _
                        .Select(Function(t)
                                    Return t.genes.Select(Function(g) g.locus_tag)
                                End Function) _
                        .IteratesALL _
                        .Distinct _
                        .ToArray
                }
            Next
        End Function
    End Class
End Namespace
