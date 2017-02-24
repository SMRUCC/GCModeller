#Region "Microsoft.VisualBasic::0e61f9fb6b5a167cf51d0ed62452923c, ..\GCModeller\analysis\SequenceToolkit\SequencePatterns\Topologically\Exactly\RepeatsSearchs.vb"

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

Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.Seeding
Imports SMRUCC.genomics.SequenceModel

Namespace Topologically

    Public Class RepeatsSearcher : Inherits SearchWorker

        Public ReadOnly Property MinAppeared As Integer
        ''' <summary>
        ''' 片段按照长度的数量上的分布情况
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property CountStatics As New IO.File
        Public ReadOnly Property ResultSet As New List(Of Repeats)

        Sub New(Sequence As IPolymerSequenceModel,
                <Parameter("Min.Len", "The minimum length of the repeat sequence loci.")> Min As Integer,
                <Parameter("Max.Len", "The maximum length of the repeat sequence loci.")> Max As Integer,
                MinAppeared As Integer)
            Call MyBase.New(Sequence, Min, Max)
            Call CountStatics.AppendLine(New String() {"Length", "Matches", "Export"})

            Me.MinAppeared = MinAppeared
        End Sub

        Protected Overrides Sub DoSearch(seed As Seed)
            If Pattern.FindLocation(seq.SequenceData, seed.Sequence).Length < MinAppeared Then
                Return
            End If

            Dim repeats As Repeats = GenerateRepeats(seq.SequenceData, seed.Sequence, MinAppeared)
            If Not repeats Is Nothing Then
                Call ResultSet.AddRange(repeats)
            End If
        End Sub
    End Class
End Namespace
