#Region "Microsoft.VisualBasic::ad680aaa9ee39f2003c31cff062ba176, core\Bio.Assembly\ComponentModel\PeriodicTable.vb"

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

    '     Module PeriodicTable
    ' 
    '         Properties: PeriodicTable
    ' 
    '         Function: MolecularWeightCalculate
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions

Namespace ComponentModel

    ''' <summary>
    ''' 元素周期表
    ''' </summary>
    ''' <remarks></remarks>
    Public Module PeriodicTable

        Public ReadOnly Property PeriodicTable As IReadOnlyDictionary(Of String, Double) =
            New Dictionary(Of String, Double) From {
                {"H", 1.008}, {"He", 4.003},
                {"Li", 6.941}, {"Be", 9.012}, {"B", 10.81}, {"C", 12.01}, {"N", 14.01}, {"O", 16.0}, {"F", 19.0}, {"Ne", 20.18},
                {"Na", 22.99}, {"Mg", 24.31}, {"Al", 26.98}, {"Si", 28.09}, {"P", 30.97}, {"S", 32.07}, {"Cl", 35.45}, {"Ar", 39.95},
                {"K", 39.1}, {"Ca", 40.08}, {"Sc", 44.96}, {"Ti", 47.88}, {"V", 50.94}, {"Cr", 52.0}, {"Mn", 54.94}, {"Fe", 55.85}, {"Co", 58.93}, {"Ni", 58.69}, {"Cu", 63.55}, {"Zn", 65.39}, {"Ga", 69.72}, {"Ge", 72.59}, {"As", 74.92}, {"Se", 78.96}, {"Br", 79.9}, {"Kr", 83.8},
                {"Rb", 85.47}, {"Sr", 87.62}, {"Y", 88.91}, {"Zr", 91.22}, {"Nb", 92.91}, {"Mo", 95.94}, {"Tc", (97.91)}, {"Ru", 101.1}, {"Rh", 102.9}, {"Pd", 106.4}, {"Ag", 107.9}, {"Cd", 112.4}, {"In", 114.8}, {"Sn", 118.7}, {"Sb", 121.8}, {"Te", 127.6}, {"I", 126.7}, {"Xe", 131.3},
                {"Cs", 132.9}, {"Ba", 137.3}, {"Hf", 178.5}, {"Ta", 180.9}, {"W", 183.9}, {"Re", 186.2}, {"Os", 190.2}, {"Ir", 192.2}, {"Pt", 195.1}, {"Au", 197.0}, {"Hg", 200.6}, {"Tl", 204.4}, {"Pb", 207.2}, {"Bi", 209.0}, {"Po", (209.0)}, {"At", (210.0)}, {"Rn", (222.0)},
                {"Fr", (223.0)}, {"Ra", 226.0}, {"Rf", (265.1)}, {"Db", (268.1)}, {"Sg", (271.1)}, {"Bh", (270.1)}, {"Hs", (277.2)}, {"Mt", (276.2)}, {"Ds", (281.2)}, {"Rg", (280.2)}, {"Cn", (285.2)}, {"Uut", (284.2)}, {"Fl", (289.2)}, {"Uup", (288.2)}, {"Lv", (293.2)}, {"Uus", (294.2)}, {"Uuo", (294.2)},
                {"La", 138.9}, {"Ce", 140.1}, {"Pr", 140.9}, {"Nd", 144.2}, {"Pm", (144.9)}, {"Sm", 150.4}, {"Eu", 152.0}, {"Gd", 157.3}, {"Tb", 158.9}, {"Dy", 162.5}, {"Ho", 164.9}, {"Er", 167.3}, {"Tm", 168.9}, {"Yb", 173.0}, {"Lu", 175.0},
                {"Ac", (227.0)}, {"Th", 232.0}, {"Pa", 231.0}, {"U", 238.0}, {"Np", 237.1}, {"Pu", 244.1}, {"Am", (243.1)}, {"Cm", 247.1}, {"Bk", (247.1)}, {"Cf", (252.1)}, {"Es", (252.1)}, {"Fm", (257.1)}, {"Md", (258.1)}, {"No", (259.1)}, {"Lr", (262.1)}}

        Const REGEX_ATOM As String = "[A-Z][a-z]*\d*"

        ''' <summary>
        ''' 尝试通过化学方程式来计算分子质量
        ''' </summary>
        ''' <param name="Formula"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function MolecularWeightCalculate(Formula As String) As Double
            If Regex.Match(Formula, "\(.+\)n").Success Then
                Formula = Mid(Formula, 2, Len(Formula) - 3)
                Dim value = MolecularWeightCalculate(Formula) * 20 '假设大分子链至少长度为20
                Return value
            ElseIf Regex.Match(Formula, ".+\(.+\)n").Success Then
                Dim PartA As String = Regex.Match(Formula, "\(.+\)n").Value
                Dim PartB As String = Formula.Replace(PartA, "")
                Dim value = MolecularWeightCalculate(PartA) + MolecularWeightCalculate(PartB)
                Return value
            End If

            Dim Atoms As String() = Regex.Matches(Formula, REGEX_ATOM).ToArray
            Dim sum As Double = 0

            For Each Atom As String In Atoms
                Dim Counts As Integer = Val(Regex.Match(Atom, "\d+").Value)
                If Counts = 0 Then
                    Counts = 1 '当只有一个原子的时候，仅有符号而没有相对应的数字
                Else
                    Atom = Atom.Replace(Counts, "")
                End If
                Dim AtomWeight As Double = 0
                Try
                    AtomWeight = PeriodicTable(Atom)
                Catch ex As Exception
                    Call $"Atom ""{Atom}"" is not found!  [{Formula}]".__DEBUG_ECHO
                    Return 0  '出错了，无法进行计算
                End Try

                sum += Counts * AtomWeight
            Next

            Return sum
        End Function
    End Module
End Namespace
