#Region "Microsoft.VisualBasic::b6c2c5d2ddae7c29d58ffb04c9160d6b, core\Bio.Assembly\Metagenomics\IGenomeObject.vb"

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

    '   Total Lines: 87
    '    Code Lines: 55 (63.22%)
    ' Comment Lines: 16 (18.39%)
    '    - Xml Docs: 93.75%
    ' 
    '   Blank Lines: 16 (18.39%)
    '     File Size: 2.93 KB


    '     Interface IGenomeObject
    ' 
    '         Properties: genome_name, ncbi_taxid
    ' 
    '     Class GenomeNameIndex
    ' 
    '         Properties: Size
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GenericEnumerator, GetBestMatch, LoadDatabase, Query
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Linq

Namespace Metagenomics

    Public Interface IGenomeObject

        Property genome_name As String
        Property ncbi_taxid As UInteger

    End Interface

    ''' <summary>
    ''' in-memory name search
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    Public Class GenomeNameIndex(Of T As IGenomeObject) : Implements Enumeration(Of T)

        ReadOnly qgram As QGramIndex
        ReadOnly pool As New List(Of T)

        Default Public ReadOnly Property Assembly(q As FindResult) As T
            Get
                Return pool(q.index)
            End Get
        End Property

        ''' <summary>
        ''' size of the data pool
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Size As Integer
            Get
                Return pool.Count
            End Get
        End Property

        Sub New(Optional qgram As Integer = 6)
            Me.qgram = New QGramIndex(qgram)
        End Sub

        Public Function LoadDatabase(data As IEnumerable(Of T)) As GenomeNameIndex(Of T)
            For Each genome As T In data.SafeQuery
                Call pool.Add(genome)
                Call qgram.AddString(genome.genome_name)
            Next

            Return Me
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="name"></param>
        ''' <param name="cutoff"></param>
        ''' <returns>
        ''' the query output result has already been sorted with <see cref="FindResult.similarity"/> in decreasing order.
        ''' </returns>
        Public Iterator Function Query(name As String, Optional cutoff As Double = 0.8) As IEnumerable(Of (genome As T, match As FindResult))
            Dim offsets As FindResult() = qgram.FindSimilar(name, cutoff) _
                .OrderByDescending(Function(a) a.similarity) _
                .ToArray

            For i As Integer = 0 To offsets.Length - 1
                Yield (pool(offsets(i).index), offsets(i))
            Next
        End Function

        Public Function GetBestMatch(name As String, Optional cutoff As Double = 0.8, Optional ByRef match As FindResult = Nothing) As T
            match = qgram.FindSimilar(name, cutoff) _
                .OrderByDescending(Function(a) a.similarity) _
                .FirstOrDefault

            If match IsNot Nothing Then
                Return pool(match.index)
            Else
                Return Nothing
            End If
        End Function

        Public Iterator Function GenericEnumerator() As IEnumerator(Of T) Implements Enumeration(Of T).GenericEnumerator
            For Each obj As T In pool
                Yield obj
            Next
        End Function
    End Class
End Namespace
