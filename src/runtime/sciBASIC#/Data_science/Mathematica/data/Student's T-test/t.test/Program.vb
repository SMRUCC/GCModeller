#Region "Microsoft.VisualBasic::36ba2ab99112b3987e5604d0e8a9094b, Data_science\Mathematica\data\Student's T-test\t.test\Program.vb"

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

    ' Module Program
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Math.Statistics.Hypothesis
Imports Microsoft.VisualBasic.Serialization.JSON

Module Program

    Sub Main()
        Call oneSample()


        Dim a#() = {115, 108, 108, 119, 105, 101, 120, 115, 104, 100.9}
        Dim b#() = {185, 169, 173, 173, 188, 186, 175, 174, 179, 180}

        With t.Test(a, b)
            Call $"alternative hypothesis: { .Valid}".__DEBUG_ECHO
            Call .GetJson(True).__DEBUG_ECHO
        End With

        Dim x#() = {0, 1, 1, 1}

        ' ttest([0,1,1,1], {mu: 1}).valid() // true
        With t.Test(x, mu:=1)
            Call $"alternative hypothesis: { .Valid}".__DEBUG_ECHO
            Call .GetJson(True).__DEBUG_ECHO
        End With

        ' ttest([0,1,1,1], [1,2,2,2], {mu: -1}).valid() // true
        Call Console.Write(t.Test({0, 1, 1, 1}, {1, 2, 2, 2}, mu:=-1).ToString)

        a = {6846523.253, 6840877.665, 5806323.704}
        b = {3056565.388, 1831431.105, 2933659.497}

        Call t.Test(a, b).GetJson(indent:=True).__DEBUG_ECHO
        Call t.Test(a, b, varEqual:=False).GetJson(indent:=True).__DEBUG_ECHO

        Pause()
    End Sub

    Sub oneSample()
        With t.Test({1, 2, 2, 2, 4}, mu:=2, alpha:=0.05, alternative:=Hypothesis.TwoSided)
            '    valid: true,
            '    freedom: 4,

            '    pValue: 0.703999999999999737099187768763,
            '    testValue: 0.408248290463863405808098150374,

            '    confidence: [
            '      0.839825238683489017077477001294,
            '      3.560174761316511560238495803787
            '    ]
            Call Console.WriteLine(.ToString)
        End With

        With t.Test({1, 2, 2, 2, 4}, mu:=2, alpha:=0.05, alternative:=Hypothesis.Less)
            '    valid: true,
            '    freedom: 4,

            '    pValue: 0.648000000000000131450406115619,
            '    testValue: 0.408248290463863405808098150374,

            '    confidence: [
            '      -Infinity,
            '      3.244387367258481980059059424093
            '    ]

            Call Console.WriteLine(.ToString)
        End With

        With t.Test({1, 2, 2, 2, 4}, mu:=2, alpha:=0.05, alternative:=Hypothesis.Greater)
            '    valid: true,
            '    freedom: 4,

            '    pValue: 0.351999999999999868549593884381,
            '    testValue: 0.408248290463863405808098150374,

            '    confidence: [
            '      1.155612632741518375212308455957,
            '      Infinity
            '    ]
            Call Console.WriteLine(.ToString)
        End With

        Pause()
    End Sub

End Module
