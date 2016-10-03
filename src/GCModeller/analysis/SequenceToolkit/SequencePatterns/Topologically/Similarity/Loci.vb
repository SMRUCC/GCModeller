#Region "Microsoft.VisualBasic::0777cfaeb307c3e3ea0e79c5ce893706, ..\GCModeller\analysis\SequenceToolkit\SequencePatterns\Topologically\Similarity\Loci.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace Topologically.SimilarityMatches

    Public Class ReversedLociMatchedResult : Inherits LociMatchedResult

        Public Property ReversedMatched As String

        Public Shared Function GenerateFromBase(Loci As LociMatchedResult) As ReversedLociMatchedResult
            Dim RevLoci As ReversedLociMatchedResult = New ReversedLociMatchedResult
            RevLoci.Location = Loci.Location
            RevLoci.Similarity = Loci.Similarity
            RevLoci.Matched = Loci.Loci
            RevLoci.Loci = NucleicAcid.Complement(New String(Loci.Loci.ToArray.Reverse.ToArray))
            RevLoci.ReversedMatched = Loci.Matched

            Return RevLoci
        End Function
    End Class

    Public Class LociMatchedResult

        ''' <summary>
        ''' 原来的序列
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Loci As String
        ''' <summary>
        ''' 模糊匹配上的序列
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Matched As String
        ''' <summary>
        ''' 相似度
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Similarity As Double
        Public Property Location As Integer()
    End Class
End Namespace
