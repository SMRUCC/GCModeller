#Region "Microsoft.VisualBasic::7346d12be237486d5297b88d01fa6f6f, analysis\ProteinTools\Protease\PeptideCutter.vb"

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

    ' Module PeptideCutter
    ' 
    '     Function: RunTest
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.GCModeller.Workbench.SeqFeature
Imports r = System.Text.RegularExpressions.Regex

Public Module PeptideCutter

    <Extension>
    Public Iterator Function RunTest(seq As IPolymerSequenceModel, proteases As IEnumerable(Of CleavageRule)) As IEnumerable(Of Site)
        Dim windows = seq.SequenceData _
            .ToUpper _
            .Select(Function(c) c.ToString) _
            .SlideWindows(winSize:=6)

        For Each enzyme As CleavageRule In proteases
            Dim rule As SeqValue(Of String)() = enzyme _
                .GetRules _
                .SeqIterator _
                .ToArray
            Dim match As Boolean

            For Each window As SlideWindow(Of String) In windows
                match = True

                For Each index As SeqValue(Of String) In rule
                    If Not r.Match(window(index), index.value).Success Then
                        match = False
                        Exit For
                    End If
                Next

                If match Then
                    Yield New Site With {
                        .Name = enzyme.Protease,
                        .Left = window.Left + 1
                    }
                End If
            Next
        Next
    End Function
End Module
