#Region "Microsoft.VisualBasic::d93832c720d160e43e06a74ebd3691f4, GCModeller\analysis\SequenceToolkit\SequencePatterns.Abstract\IScanner.vb"

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

    '   Total Lines: 106
    '    Code Lines: 72
    ' Comment Lines: 15
    '   Blank Lines: 19
    '     File Size: 3.61 KB


    '     Class IScanner
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: __complement, Complement, (+2 Overloads) FindLocation, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif.Patterns
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace Motif

    Public MustInherit Class IScanner

        Protected ReadOnly nt As String

        Sub New(nt As IPolymerSequenceModel)
            Me.nt = nt.SequenceData.ToUpper
        End Sub

        Public Overrides Function ToString() As String
            Return $"{nt.Length}bp...."
        End Function

        Public MustOverride Function Scan(pattern As String) As SimpleSegment()

        ''' <summary>
        ''' Found out all of the loci site on the target sequence.
        ''' </summary>
        ''' <param name="seq"></param>
        ''' <param name="loci"></param>
        ''' <returns></returns>
        Public Shared Function FindLocation(seq As IPolymerSequenceModel, loci As String) As Integer()
            Return FindLocation(seq.SequenceData, loci).ToArray
        End Function

        ''' <summary>
        ''' Found out all of the loci site on the target sequence.
        ''' (使用字符串查找得到目标位点在序列之上的所有的位置集合)
        ''' </summary>
        ''' <param name="seq"></param>
        ''' <param name="Loci"></param>
        ''' <returns></returns>
        ''' <remarks>这个位置查找函数是OK的</remarks>
        Public Shared Iterator Function FindLocation(seq$, loci$) As IEnumerable(Of Integer)
            Dim pI32% = 1

            Do While True
                ' 这里需要进行迭代查找，即在上一个位置之后查找，否则会出现无限的重复查找
#If Not NET48 Then
                pI32 = Strings.InStr(pI32, String1:=seq, String2:=loci)
#Else
                pI32 = Strings.InStr(Start:=pI32, String1:=seq, String2:=loci)
#End If
                If pI32 > 0 Then
                    Yield pI32
                Else
                    Exit Do
                End If

                pI32 += 1
            Loop
        End Function

        Public Shared Function Complement(pattern As String) As String
            Dim tokens As String() = PatternParser.SimpleTokens(pattern)

            For i As Integer = 0 To tokens.Length - 1
                Dim s As String = tokens(i)
                If s.Length = 1 Then
                    s = CStr(__complement(s.First))
                Else
                    s = s.GetStackValue("[", "]")
                    Dim temp As New List(Of Char)

                    For Each c As Char In s
                        temp += __complement(c)
                    Next

                    s = New String("["c + temp + "]"c)
                End If

                tokens(i) = s
            Next

            tokens = tokens.Reverse.ToArray
            pattern = String.Join("", tokens)

            Return pattern
        End Function

        Private Shared Function __complement(c As Char) As Char
            Select Case c
                Case "A"c
                    Return "T"c
                Case "G"c
                    Return "C"c
                Case "C"c
                    Return "G"c
                Case "T"c
                    Return "A"c
                Case "N"c, "."c
                    Return "."c
                Case Else
                    Throw New Exception("Illegal characters in the pattern expression!")
            End Select
        End Function

    End Class
End Namespace
