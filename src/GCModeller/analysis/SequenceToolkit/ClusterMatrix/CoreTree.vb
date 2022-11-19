#Region "Microsoft.VisualBasic::8d3f52979d0a93c213a20c45a6c6235f, GCModeller\analysis\SequenceToolkit\ClusterMatrix\CoreTree.vb"

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

    '   Total Lines: 117
    '    Code Lines: 87
    ' Comment Lines: 11
    '   Blank Lines: 19
    '     File Size: 4.10 KB


    ' Module CoreTree
    ' 
    '     Function: BuildTree
    ' 
    ' Class Cluster
    ' 
    '     Properties: conservative, genomes, Members, NumOfMembers, Representive
    ' 
    '     Function: ToString
    ' 
    ' Class ProteinComparer
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: Compare
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.BinaryTree
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH

Public Module CoreTree

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="localblast">多个基因组之间的比对结果</param>
    ''' <returns></returns>
    <Extension>
    Public Function BuildTree(proteins As IEnumerable(Of String), localblast As IEnumerable(Of BestHit)) As Cluster()
        Dim comparer As New ProteinComparer(localblast)
        Dim tree As New AVLTree(Of String, String)(comparer, Function(name) name)

        For Each proteinName As String In proteins
            Call tree.Add(proteinName, proteinName, valueReplace:=False)
        Next

        ' 返回同源性的cluster
        Dim clusters As Cluster() = tree.GetAllNodes _
            .Select(Function(node)
                        Dim rep$ = node.Key
                        Dim members = DirectCast(node!values, IEnumerable(Of String)) _
                            .Distinct _
                            .ToArray

                        Return New Cluster With {
                            .Representive = rep,
                            .Members = members
                        }
                    End Function) _
            .ToArray

        Return clusters
    End Function
End Module

Public Class Cluster

    Public Property Representive As String
    Public Property Members As String()

    Public ReadOnly Property NumOfMembers As Integer
        Get
            Return Members?.Length
        End Get
    End Property

    Public ReadOnly Property conservative As Integer
        Get
            Return genomes?.Length
        End Get
    End Property

    Public Property genomes As String()

    Public Overrides Function ToString() As String
        Return Representive
    End Function

End Class

Public Class ProteinComparer : Implements IComparer(Of String)

    ''' <summary>
    ''' [subj => [query => subject]]
    ''' </summary>
    Dim blastIndex As Dictionary(Of String, Dictionary(Of String, BestHit))

    Sub New(localblast As IEnumerable(Of BestHit))
        blastIndex = localblast _
            .GroupBy(Function(hit) hit.HitName) _
            .ToDictionary(Function(db) db.Key,
                          Function(db)
                              ' 如果存在重复，则直接取identify最高的结果
                              ' 在这里，hitname都是一致的
                              Return db _
                                .GroupBy(Function(hit) hit.QueryName) _
                                .ToDictionary(Function(s)
                                                  Return s.Key
                                              End Function,
                                              Function(g)
                                                  If g.Count > 1 Then
                                                      Return g _
                                                        .OrderByDescending(Function(hit) hit.identities) _
                                                        .First
                                                  Else
                                                      Return g.First
                                                  End If
                                              End Function)
                          End Function)
    End Sub

    Public Function Compare(x As String, y As String) As Integer Implements IComparer(Of String).Compare
        If blastIndex.ContainsKey(x) Then
            Dim qvs = blastIndex(x)

            If qvs.ContainsKey(y) Then
                Dim besthit As BestHit = qvs(y)

                If besthit.identities > 0.6 Then
                    Return 0
                ElseIf besthit.identities > 0.3 Then
                    Return 1
                Else
                    Return -1
                End If
            Else
                Return 1
            End If
        Else
            Return -1
        End If
    End Function
End Class
