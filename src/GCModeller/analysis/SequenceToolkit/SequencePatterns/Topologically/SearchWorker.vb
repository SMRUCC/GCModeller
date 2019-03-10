﻿#Region "Microsoft.VisualBasic::2af27d16a453a877152b5e3b554c37fa, analysis\SequenceToolkit\SequencePatterns\Topologically\SearchWorker.vb"

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

    '     Class SearchWorker
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Sub: DoSearch
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.Seeding
Imports SMRUCC.genomics.SequenceModel

Namespace Topologically

    Public MustInherit Class SearchWorker

        Protected ReadOnly seedBox As SeedBox
        Protected ReadOnly seq As IPolymerSequenceModel
        Protected min, max As Integer

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Sequence"></param>
        ''' <param name="Min"></param>
        ''' <param name="Max"></param>
        Sub New(Sequence As IPolymerSequenceModel,
                <Parameter("Min.Len", "The minimum length of the repeat sequence loci.")> Min As Integer,
                <Parameter("Max.Len", "The maximum length of the repeat sequence loci.")> Max As Integer)

            Me.seedBox = New SeedBox(Sequence)
            Me.min = Min
            Me.max = Max
            Me.seq = Sequence
        End Sub

        ''' <summary>
        ''' 使用种子进行序列位点的搜索
        ''' </summary>
        Public Sub DoSearch()
            For Each seeds As Seed() In seedBox.PopulateSeeds(min, max)
                For Each seed As Seed In seeds
                    Call DoSearch(seed)
                Next
            Next
        End Sub

        Protected MustOverride Sub DoSearch(seed As Seed)
    End Class
End Namespace
