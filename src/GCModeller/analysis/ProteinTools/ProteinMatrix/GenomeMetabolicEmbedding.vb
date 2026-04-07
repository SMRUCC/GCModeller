#Region "Microsoft.VisualBasic::d73b0b91b3f80232a1ed75c507faa318, analysis\ProteinTools\ProteinMatrix\GenomeMetabolicEmbedding.vb"

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
    '    Code Lines: 40 (72.73%)
    ' Comment Lines: 4 (7.27%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 11 (20.00%)
    '     File Size: 1.79 KB


    ' Class GenomeMetabolicEmbedding
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: AddGenomes, OneHotVectorizer, TfidfVectorizer
    ' 
    '     Sub: Add
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Data.NLP
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Pipeline

Public Class GenomeMetabolicEmbedding

    ReadOnly vec As New TFIDF
    ReadOnly taxonomy As New Dictionary(Of String, String)
    ReadOnly hierarchical As Boolean = False

    Sub New(Optional hierarchical As Boolean = False)
        Me.hierarchical = hierarchical
    End Sub

    Public Sub Add(genome As GenomeVector)
        If genome.assembly_id = "n/a" Then
            Return
        End If

        If hierarchical Then
            Call vec.Add(genome.assembly_id, genome.GetHierarchicalECNumberTerms)
        Else
            Call vec.Add(genome.assembly_id, genome.terms)
        End If

        Call taxonomy.Add(genome.assembly_id, genome.taxonomy)
    End Sub

    Public Function AddGenomes(seqs As IEnumerable(Of GenomeVector)) As GenomeMetabolicEmbedding
        For Each annotation As GenomeVector In seqs
            Call Add(annotation)
        Next

        Return Me
    End Function

    Public Function TfidfVectorizer(Optional normalize As Boolean = False) As DataFrame
        Call $"Make metabolic embedding with: ".info
        Call $"  * {vec.N} genomes".debug
        Call $"  * {vec.Words.Length} total enzyme terms".debug
        Call VBDebugger.EchoLine("")

        Dim df As DataFrame = vec.TfidfVectorizer(normalize)
        Call df.add("taxonomy", From id As String In df.rownames Select taxonomy(id))
        Return df
    End Function

    ''' <summary>
    ''' n-gram One-hot(Bag-of-n-grams)
    ''' </summary>
    ''' <returns></returns>
    Public Function OneHotVectorizer() As DataFrame
        Return vec.OneHotVectorizer
    End Function
End Class

