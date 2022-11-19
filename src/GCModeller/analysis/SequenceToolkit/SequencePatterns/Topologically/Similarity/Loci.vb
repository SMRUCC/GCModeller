#Region "Microsoft.VisualBasic::fcd3127bfc107652115014ede62c19d6, GCModeller\analysis\SequenceToolkit\SequencePatterns\Topologically\Similarity\Loci.vb"

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

    '   Total Lines: 51
    '    Code Lines: 22
    ' Comment Lines: 22
    '   Blank Lines: 7
    '     File Size: 1.56 KB


    '     Class ReversedLociMatchedResult
    ' 
    '         Properties: isRevMatched
    ' 
    '         Function: GenerateFromBase
    ' 
    '     Class LociMatchedResult
    ' 
    '         Properties: Location, Loci, Matched, Similarity
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace Topologically.SimilarityMatches

    Public Class ReversedLociMatchedResult : Inherits LociMatchedResult

        ''' <summary>
        ''' Is reverse matched?
        ''' </summary>
        ''' <returns></returns>
        Public Property isRevMatched As String

        Public Shared Function GenerateFromBase(loci As LociMatchedResult) As ReversedLociMatchedResult
            Dim rev As New ReversedLociMatchedResult With {
               .Location = loci.Location,
               .Similarity = loci.Similarity,
               .Matched = loci.Loci,
               .Loci = NucleicAcid.Complement(New String(loci.Loci.ToArray.Reverse.ToArray)),
               .isRevMatched = loci.Matched
            }

            Return rev
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
