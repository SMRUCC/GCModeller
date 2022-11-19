#Region "Microsoft.VisualBasic::fe881c8c3cc6eff0591145c42284a9e5, GCModeller\core\Bio.Annotation\IDMapping.vb"

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

    '   Total Lines: 73
    '    Code Lines: 51
    ' Comment Lines: 11
    '   Blank Lines: 11
    '     File Size: 2.64 KB


    ' Module IDMapping
    ' 
    '     Function: createUnifyIdIndex, Mapping, populateIdMaps, UnifyIdMapping
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

    ''' <summary>
    ''' mapping any symbol id to a unify unique symbol id <see cref="Ptf.ProteinAnnotation.geneId"/>
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="proteins"></param>
    ''' <param name="data"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function Mapping(Of T As {INamedValue, Class})(proteins As Ptf.PtfFile, data As IEnumerable(Of T)) As IEnumerable(Of T)
        Dim unifyIdIndex As Dictionary(Of String, String) = proteins.createUnifyIdIndex

        For Each protein In data
            If unifyIdIndex.ContainsKey(protein.Key) Then
                protein.Key = unifyIdIndex(protein.Key)
            End If

            Yield protein
        Next
    End Function

    Public Iterator Function UnifyIdMapping(proteins As Ptf.PtfFile, geneIDs As IEnumerable(Of String)) As IEnumerable(Of String)
        Dim unifyIdIndex As Dictionary(Of String, String) = proteins.createUnifyIdIndex

        For Each geneId As String In geneIDs
            If unifyIdIndex.ContainsKey(geneId) Then
                Yield unifyIdIndex(geneId)
            Else
                Yield geneId
            End If
        Next
    End Function

    <Extension>
    Private Function createUnifyIdIndex(proteins As Ptf.PtfFile) As Dictionary(Of String, String)
        Return proteins _
            .AsEnumerable _
            .Select(AddressOf IDMapping.populateIdMaps) _
            .IteratesALL _
            .GroupBy(Function(a) a.id) _
            .ToDictionary(Function(a) a.Key,
                          Function(a)
                              Return a.First.geneId
                          End Function)
    End Function

    Private Iterator Function populateIdMaps(prot As Ptf.ProteinAnnotation) As IEnumerable(Of (id$, geneId$))
        ' locus_id map to unify protein id symbol
        If Not prot.locus_id.StringEmpty Then
            Yield (prot.locus_id, prot.geneId)
        End If

        For Each attr In prot.attributes
            For Each id As String In attr.Value
                If HeaderFormats.HasVersionNumber(id) Then
                    Yield (HeaderFormats.TrimAccessionVersion(id), prot.geneId)
                End If

                Yield (id, prot.geneId)
            Next
        Next
    End Function

End Module
