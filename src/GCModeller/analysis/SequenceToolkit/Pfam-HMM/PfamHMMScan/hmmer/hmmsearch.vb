#Region "Microsoft.VisualBasic::2ff02a3009bd5c862744243ba3054240, analysis\SequenceToolkit\Pfam-HMM\PfamHMMScan\hmmer\hmmsearch.vb"

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

    ' Class hmmsearch
    ' 
    '     Properties: HMM, Queries, source, version
    ' 
    '     Function: GetProfiles, ToString
    ' 
    ' Class PfamQuery
    ' 
    '     Properties: Accession, alignments, describ, hits, MLen
    '                 Query, uncertain
    ' 
    '     Function: ToString
    ' 
    ' Class Score
    ' 
    '     Properties: Best, describ, exp, Full, locus
    '                 N
    ' 
    '     Function: ToString
    ' 
    ' Class AlignmentHit
    ' 
    '     Properties: hits, IsMatched, locus, QueryTag
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' # hmmsearch :: search profile(s) against a sequence database
''' </summary>
Public Class hmmsearch

    Public Const Pfam As String = NameOf(Pfam)

    ''' <summary>
    ''' # hmmsearch :: search profile(s) against a sequence database
    ''' # HMMER 3.1b1 (May 2013); http://hmmer.org/
    ''' # Copyright (C) 2013 Howard Hughes Medical Institute.
    ''' # Freely distributed under the GNU General Public License (GPLv3).
    ''' </summary>
    ''' <returns></returns>
    Public Property version As String
    ''' <summary>
    ''' query HMM file
    ''' </summary>
    ''' <returns></returns>
    Public Property HMM As String
    ''' <summary>
    ''' target sequence database
    ''' </summary>
    ''' <returns></returns>
    Public Property source As String
    Public Property Queries As PfamQuery()

    Public Function GetProfiles() As Dictionary(Of String, AlignmentHit())
        Dim LQuery As IEnumerable(Of AlignmentHit) =
            LinqAPI.MakeList(Of AlignmentHit) <= From x As PfamQuery
                                                 In Queries.AsParallel
                                                 Select x.alignments
        Dim Groups = From x As AlignmentHit
                     In LQuery
                     Let tag As String = x.locus.Split.First
                     Select o = x,
                         tag
                     Group By tag Into Group
        Dim hash As Dictionary(Of String, AlignmentHit()) =
            Groups.ToDictionary(Function(x) x.tag,
                                Function(x)
                                    Return x.Group.Select(Function(o) o.o).ToArray
                                End Function)
        Return hash
    End Function

    Public Overrides Function ToString() As String
        Return New With {version, HMM, source}.GetJson
    End Function
End Class

Public Class PfamQuery
    Public Property Query As String
    Public Property MLen As Integer
    Public Property Accession As String
    Public Property describ As String
    Public Property hits As Score()
    Public Property uncertain As Score()
    Public Property alignments As AlignmentHit()

    Public Overrides Function ToString() As String
        Return Query
    End Function
End Class

Public Class Score
    Public Property Full As hmmscan.Score
    Public Property Best As hmmscan.Score
    Public Property exp As Double
    Public Property N As Integer
    Public Property locus As String
    Public Property describ As String

    Public Overrides Function ToString() As String
        Return locus
    End Function
End Class

Public Class AlignmentHit : Implements IMatched

    Public Property locus As String
    Public Property hits As hmmscan.Align()
    ''' <summary>
    ''' Tag data for the Pfam HMM model
    ''' </summary>
    ''' <returns></returns>
    Public Property QueryTag As String

    Public ReadOnly Property IsMatched As Boolean Implements IMatched.IsMatched
        Get
            Return Not (From x As hmmscan.Align
                        In hits.SafeQuery
                        Where DirectCast(x, IMatched).IsMatched
                        Select x).FirstOrDefault Is Nothing
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return locus
    End Function
End Class
