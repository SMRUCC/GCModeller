#Region "Microsoft.VisualBasic::5cc1b77be2c66807e467c3b758a6a21a, ..\GCModeller\analysis\SequenceToolkit\SequencePatterns\Topologically\Palindrome\SearchWorker\PalindromeSearch.vb"

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

Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.Seeding
Imports SMRUCC.genomics.SequenceModel

Namespace Topologically

    ''' <summary>
    ''' 片段在反向链找得到自己的反向片段
    ''' </summary>
    Public Class PalindromeSearch : Inherits MirrorPalindrome

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Sequence"></param>
        ''' <param name="Min"></param>
        ''' <param name="Max"></param>
        Sub New(Sequence As IPolymerSequenceModel,
                <Parameter("Min.Len", "The minimum length of the repeat sequence loci.")> Min As Integer,
                <Parameter("Max.Len", "The maximum length of the repeat sequence loci.")> Max As Integer)
            Call MyBase.New(Sequence, Min, Max)
        End Sub

        ''' <summary>
        ''' 片段在反向链找得到自己的反向片段
        ''' </summary>
        ''' <param name="seed"></param>
        Protected Overrides Sub DoSearch(seed As Seed)
            Dim Sites As PalindromeLoci() = Palindrome.CreatePalindrome(seed.Sequence, seq.SequenceData).TrimNull
            Call _resultSet.Add(Sites)
        End Sub
    End Class
End Namespace
