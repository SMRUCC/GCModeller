#Region "Microsoft.VisualBasic::5d11d8cd2f6c6492bf32db16a2d0dd43, GCModeller\analysis\Microarray\Data\DataSimulation.vb"

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

    '   Total Lines: 70
    '    Code Lines: 48
    ' Comment Lines: 11
    '   Blank Lines: 11
    '     File Size: 2.78 KB


    ' Module DataSimulation
    ' 
    '     Function: PopulateSimulateData
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language.C
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Annotation.Ptf
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports randf = Microsoft.VisualBasic.Math.RandomExtensions

Public Module DataSimulation

    ''' <summary>
    ''' DO HTS data simulation for test analysis program
    ''' </summary>
    ''' <param name="proteins">基因组注释数据</param>
    ''' <param name="range">表达范围</param>
    ''' <param name="profiles">[KOmap => log2(foldchange)]</param>
    ''' <param name="tagName">
    ''' the label formatter, ``%s`` for <paramref name="replication"/> n.
    ''' </param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function PopulateSimulateData(proteins As PtfFile,
                                                  range As DoubleRange,
                                                  profiles As Dictionary(Of String, Double),
                                                  Optional replication As Integer = 6,
                                                  Optional tagName As String = "sample_%s") As IEnumerable(Of DataSet)

        Dim KOMapIndex As New Dictionary(Of String, Double)

        For Each level_A In htext.ko00001.Hierarchical.categoryItems
            For Each level_B In level_A.categoryItems
                For Each map In level_B.categoryItems
                    ' default is no changes
                    Dim foldchange As Double = profiles.TryGetValue(map.entryID, default:=1)

                    For Each ko In map.categoryItems
                        KOMapIndex(ko.entryID) += foldchange
                    Next
                Next
            Next
        Next

        Dim sampleNames As String() = replication _
            .Sequence _
            .Select(Function(i) sprintf(tagName, i + 1)) _
            .ToArray

        For Each protein As ProteinAnnotation In proteins.AsEnumerable
            Dim ko As String = AnnotationReader.KO(protein)
            Dim scaleRange As DoubleRange = {0.9 * range.Min, 1.2 * range.Max}

            If Not ko.StringEmpty Then
                If KOMapIndex(ko) < 1 Then
                    scaleRange = {KOMapIndex(ko) * range.Min, range.Min}
                Else
                    scaleRange = {range.Max, KOMapIndex(ko) * range.Max}
                End If
            End If

            Dim data As New DataSet With {.ID = protein.geneId}

            For i As Integer = 1 To replication
                data(sampleNames(i - 1)) = randf.GetRandomValue(scaleRange)
            Next

            Yield data
        Next
    End Function
End Module
