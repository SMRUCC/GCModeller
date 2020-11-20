#Region "Microsoft.VisualBasic::4ba2e2c392d6b66f506fb8d7cfc128fc, core\Bio.Annotation\IDMapping.vb"

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

    ' Module IDMapping
    ' 
    '     Function: Mapping
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.SequenceModel.FASTA

''' <summary>
''' GCModeller id mapping services based on the <see cref="Ptf.ProteinAnnotation"/>
''' </summary>
Public Module IDMapping

    <Extension>
    Public Iterator Function Mapping(Of T As {INamedValue, Class})(proteins As Ptf.PtfFile, data As IEnumerable(Of T)) As IEnumerable(Of T)
        Dim unifyIdIndex As Dictionary(Of String, String) = proteins _
            .AsEnumerable _
            .Select(Function(pro)
                        Return pro.attributes _
                            .Select(Iterator Function(a) As IEnumerable(Of (id$, geneId$))
                                        For Each id As String In a.Value
                                            If HeaderFormats.HasVersionNumber(id) Then
                                                Yield (HeaderFormats.TrimAccessionVersion(id), pro.geneId)
                                            End If

                                            Yield (id, pro.geneId)
                                        Next
                                    End Function)
                    End Function) _
            .IteratesALL _
            .IteratesALL _
            .GroupBy(Function(a) a.id) _
            .ToDictionary(Function(a) a.Key,
                          Function(a)
                              Return a.First.geneId
                          End Function)

        For Each protein In data
            If unifyIdIndex.ContainsKey(protein.Key) Then
                protein.Key = unifyIdIndex(protein.Key)
            End If

            Yield protein
        Next
    End Function

End Module

