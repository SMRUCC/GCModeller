#Region "Microsoft.VisualBasic::ffcde4d324d1cd720a042e0d9037aac9, G:/GCModeller/src/GCModeller/analysis/Motifs/PrimerDesigner//Restriction_enzyme/TranslatePatterns.vb"

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

    '   Total Lines: 78
    '    Code Lines: 57
    ' Comment Lines: 6
    '   Blank Lines: 15
    '     File Size: 2.63 KB


    '     Module TranslatePatterns
    ' 
    '         Function: (+3 Overloads) TranslateRegular
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace Restriction_enzyme

    Public Module TranslatePatterns

        ReadOnly debase As New DegenerateBasesExtensions

        ''' <summary>
        ''' translate the recongnition site string as regular expression motif pattern data.
        ''' </summary>
        ''' <param name="enzyme"></param>
        ''' <returns></returns>
        <Extension>
        Public Function TranslateRegular(enzyme As Enzyme, Optional direction As Strands = Strands.Forward) As MotifPattern
            Dim site As String = enzyme.Recognition(direction)
            Dim regular As String = TranslateRegular(site)
            Dim motif As New MotifPattern With {
                .Id = enzyme.Enzyme,
                .Motif = enzyme.Enzyme,
                .Width = site.Length,
                .Expression = regular
            }

            Return motif
        End Function

        Private Function TranslateRegular(site As String) As String
            Dim sb As New StringBuilder
            Dim tmp As New List(Of Char)

            For Each c As Char In site
                If debase.DegenerateBases.ContainsKey(c) Then
                    If tmp.Any Then
                        Call sb.Append(TranslateRegular(tmp))
                    End If

                    Call tmp.Add(c)
                ElseIf Char.IsDigit(c) Then
                    ' is number/digit
                    Call tmp.Add(c)
                Else
                    If tmp.Any Then
                        Call sb.Append(TranslateRegular(tmp))
                    End If

                    Call sb.Append(c)
                End If
            Next

            If tmp.Any Then
                Call sb.Append(TranslateRegular(tmp))
            End If

            Return sb.ToString
        End Function

        Private Function TranslateRegular(ByRef tmp As List(Of Char)) As String
            Dim r As String

            If Char.IsDigit(tmp.First) Then
                r = $"{{{tmp.CharString}}}"
            ElseIf tmp.Count = 1 Then
                r = "[" & debase.DegenerateBases(tmp(0)).JoinBy("") & "]"
            Else
                r = $"[{debase.DegenerateBases(tmp(0)).JoinBy("")}]{{{tmp.Skip(1).CharString}}}"
            End If

            Call tmp.Clear()

            Return r
        End Function
    End Module
End Namespace
