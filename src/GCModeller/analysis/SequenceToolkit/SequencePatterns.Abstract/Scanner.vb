#Region "Microsoft.VisualBasic::5b196b79fc8cd9f747b7ab54a2801140, GCModeller\analysis\SequenceToolkit\SequencePatterns.Abstract\Scanner.vb"

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

    '   Total Lines: 47
    '    Code Lines: 38
    ' Comment Lines: 3
    '   Blank Lines: 6
    '     File Size: 2.02 KB


    '     Class Scanner
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: (+2 Overloads) Scan
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace Motif

    ''' <summary>
    ''' 使用正则表达式扫描序列得到可能的motif位点
    ''' </summary>
    Public Class Scanner : Inherits IScanner

        Sub New(nt As IPolymerSequenceModel)
            Call MyBase.New(nt)
        End Sub

        Public Overrides Function Scan(pattern As String) As SimpleSegment()
            Return (Scan(nt, pattern, "+"c).AsList + Scan(nt, Complement(pattern), "-"c)).OrderBy(Function(x) x.Start).ToArray
        End Function

        Public Overloads Shared Function Scan(nt As String, pattern As String, strand As Char) As SimpleSegment()
            Dim ms$() = Regex.Matches(nt, pattern) _
                .ToArray _
                .Distinct _
                .ToArray
            Dim locis = LinqAPI.Exec(Of SimpleSegment) _
                                                       _
                () <= From m As String
                      In ms
                      Let pos As Integer() = FindLocation(nt, m)
                      Let rc As String = New String(NucleicAcid.Complement(m).Reverse.ToArray)
                      Select LinqAPI.Exec(Of Integer, SimpleSegment)(pos) _
                                                                          _
                          <= Function(ind As Integer) As SimpleSegment
                                 Return New SimpleSegment With {
                                     .Ends = ind + m.Length,
                                     .Start = ind,
                                     .SequenceData = m,
                                     .strand = strand.ToString  ' .Complement = rc
                                 }
                             End Function

            Return locis
        End Function
    End Class
End Namespace
