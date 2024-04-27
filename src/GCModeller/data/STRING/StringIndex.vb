#Region "Microsoft.VisualBasic::890c7249cd3a60c1e46e01536c3fe640, G:/GCModeller/src/GCModeller/data/STRING//StringIndex.vb"

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

    '   Total Lines: 79
    '    Code Lines: 53
    ' Comment Lines: 16
    '   Blank Lines: 10
    '     File Size: 2.83 KB


    ' Class StringIndex
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: Intersect, QueryLinkDetails, RemoveTaxonomyIdPrefix
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Math.DataFrame
Imports SMRUCC.genomics.Data.STRING.StringDB.Tsv

''' <summary>
''' network index for the <see cref="linksDetail"/>
''' </summary>
Public Class StringIndex

    ''' <summary>
    ''' protein1 => [protein2 => linkdata]
    ''' </summary>
    ReadOnly matrix As Dictionary(Of String, Dictionary(Of String, linksDetail))

    Sub New(links As IEnumerable(Of linksDetail))
        matrix = links _
            .GroupBy(Function(l) l.protein1) _
            .ToDictionary(Function(prot1) prot1.Key,
                          Function(group)
                              Return group.ToDictionary(Function(l) l.protein2)
                          End Function)
    End Sub

    Public Function QueryLinkDetails(prot1 As String, prot2 As String, Optional ignoreDirection As Boolean = True) As linksDetail
        If matrix.ContainsKey(prot1) Then
            If matrix(prot1).ContainsKey(prot2) Then
                Return matrix(prot1)(prot2)
            Else
                Return Nothing
            End If
        ElseIf ignoreDirection AndAlso matrix.ContainsKey(prot2) Then
            If matrix(prot2).ContainsKey(prot1) Then
                Return matrix(prot2)(prot1)
            Else
                Return Nothing
            End If
        Else
            Return Nothing
        End If
    End Function

    Public Shared Iterator Function RemoveTaxonomyIdPrefix(links As IEnumerable(Of linksDetail)) As IEnumerable(Of linksDetail)
        For Each link As linksDetail In links
            link.protein1 = link.protein1.Split("."c)(1)
            link.protein2 = link.protein2.Split("."c)(1)

            Yield link
        Next
    End Function

    ''' <summary>
    ''' filter correlation matrix result that: only the protein
    ''' id tuple that exists in the string-db will create a 
    ''' correlation graph
    ''' </summary>
    ''' <param name="cor"></param>
    ''' <returns></returns>
    Public Function Intersect(cor As CorrelationMatrix) As CorrelationMatrix
        ' set sign to zero if there is no string-db links
        Dim sign As Double()() = cor.Sign
        Dim geneIds As String() = cor.keys

        ' matrix[j,i]
        For j As Integer = 0 To geneIds.Length - 1
            Dim prot2 As String = geneIds(j)

            For i As Integer = 0 To geneIds.Length - 1
                If QueryLinkDetails(geneIds(i), prot2, ignoreDirection:=True) Is Nothing Then
                    sign(j)(i) = 0.0
                ElseIf i = j Then
                    ' removes self loop
                    sign(j)(i) = 0.0
                End If
            Next
        Next

        Return sign * cor
    End Function
End Class

