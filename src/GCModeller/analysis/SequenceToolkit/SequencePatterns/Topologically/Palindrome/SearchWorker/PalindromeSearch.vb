#Region "Microsoft.VisualBasic::5f4f4654d7e68ccd0a2d684d2a6c93f0, GCModeller\analysis\SequenceToolkit\SequencePatterns\Topologically\Palindrome\SearchWorker\PalindromeSearch.vb"

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

    '   Total Lines: 31
    '    Code Lines: 14
    ' Comment Lines: 13
    '   Blank Lines: 4
    '     File Size: 1.04 KB


    '     Class PalindromeSearch
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Sub: DoSearch
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language
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
        ''' <param name="seq"></param>
        ''' <param name="Min"></param>
        ''' <param name="Max"></param>
        Sub New(seq As IPolymerSequenceModel, min As Integer, max As Integer)
            Call MyBase.New(seq, min, max)
        End Sub

        ''' <summary>
        ''' 片段在反向链找得到自己的反向片段
        ''' </summary>
        ''' <param name="seed"></param>
        Protected Overrides Sub DoSearch(seed As Seed)
            Dim Sites As PalindromeLoci() = Palindrome.CreatePalindrome(seed.sequence, seq).TrimNull
            Call m_resultSet.Add(Sites)
        End Sub
    End Class
End Namespace
