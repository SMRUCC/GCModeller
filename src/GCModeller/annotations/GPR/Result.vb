#Region "Microsoft.VisualBasic::62e37f88702812f44ca8784833814789, annotations\GPR\Result.vb"

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

    '   Total Lines: 100
    '    Code Lines: 76 (76.00%)
    ' Comment Lines: 8 (8.00%)
    '    - Xml Docs: 87.50%
    ' 
    '   Blank Lines: 16 (16.00%)
    '     File Size: 3.35 KB


    ' Class ScoredReaction
    ' 
    '     Properties: Id, Score, Unmapped
    ' 
    '     Function: ToString
    ' 
    ' Class GeneAssociation
    ' 
    '     Properties: GeneId, GPRLinks, MeanScore, MedianScore, Reactions
    '                 TopGPRLinks
    ' 
    '     Function: ToString
    ' 
    ' Class Genome
    ' 
    '     Properties: MetabolicNetwork
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: GetGeneReactions, GetGenesForReaction
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Math.Statistics.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ContextModel
Imports SMRUCC.genomics.MetabolicModel

Public Class ScoredReaction : Implements INamedValue

    Public Property Id As String Implements INamedValue.Key
    Public Property Score As Double
    Public Property Unmapped As Boolean = False

    Public Overrides Function ToString() As String
        Return $"{Id}: {Score}"
    End Function
End Class

Public Class GeneAssociation : Implements INamedValue

    Public Property GeneId As String Implements INamedValue.Key
    Public Property Reactions As Dictionary(Of String, ScoredReaction)

    Public ReadOnly Property GPRLinks As Integer
        Get
            Return Reactions.Count
        End Get
    End Property

    Public ReadOnly Property MeanScore As Double
        Get
            If Reactions.Count = 0 Then
                Return 0
            Else
                Return Reactions.Values.Average(Function(a) a.Score)
            End If
        End Get
    End Property

    Public ReadOnly Property MedianScore As Double
        Get
            If Reactions.Count = 0 Then
                Return 0
            Else
                Return Reactions.Values.Select(Function(a) a.Score).Median
            End If
        End Get
    End Property

    Public ReadOnly Property TopGPRLinks As String()
        Get
            Dim cutoff As Double = MeanScore
            Dim mapped As IEnumerable(Of ScoredReaction) = From r As ScoredReaction
                                                           In Reactions.Values
                                                           Where Not r.Unmapped
            Return mapped _
                .Where(Function(r) r.Score >= cutoff) _
                .Select(Function(r) r.Id) _
                .ToArray
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return $"{GeneId} - [{Reactions.Count}]{Reactions.Keys.GetJson}"
    End Function

End Class

Public Class Genome : Inherits GenomeContext(Of GeneTable)

    Public Property MetabolicNetwork As New Dictionary(Of String, GeneAssociation)

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="genome"></param>
    ''' <param name="name"></param>
    ''' <remarks>
    ''' impute <paramref name="genome"/> context data has been sorted by left in asc order
    ''' </remarks>
    Public Sub New(genome As IEnumerable(Of GeneTable), Optional name As String = "unnamed")
        MyBase.New(genome, name)
    End Sub

    Public Function GetGeneReactions(coGene As String) As IEnumerable(Of MetabolicReaction)
        If MetabolicNetwork.ContainsKey(coGene) Then
            Return MetabolicNetwork(coGene)
        Else
            Return {}
        End If
    End Function

    Public Iterator Function GetGenesForReaction(id As String) As IEnumerable(Of GeneTable)
        For Each gene As GeneAssociation In MetabolicNetwork.Values
            If gene.Reactions.ContainsKey(id) Then
                Yield Me(gene.GeneId)
            End If
        Next
    End Function
End Class
