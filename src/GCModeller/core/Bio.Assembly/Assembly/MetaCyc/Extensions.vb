#Region "Microsoft.VisualBasic::1bdbba134a5b40937ca3c44a63b10253, GCModeller\core\Bio.Assembly\Assembly\MetaCyc\Extensions.vb"

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

    '   Total Lines: 70
    '    Code Lines: 51
    ' Comment Lines: 6
    '   Blank Lines: 13
    '     File Size: 2.44 KB


    '     Module Extensions
    ' 
    '         Properties: Directions
    ' 
    '         Function: ContactLines, GetAttributeList
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles
Imports SMRUCC.genomics.Assembly.MetaCyc.Schema.Metabolism

Namespace Assembly.MetaCyc

    Public Module Extensions

        Public ReadOnly Property Directions As IReadOnlyDictionary(Of String, ReactionDirections) =
            New Dictionary(Of String, ReactionDirections) From {
 _
            {"LEFT-TO-RIGHT", ReactionDirections.LeftToRight},
            {"REVERSIBLE", ReactionDirections.Reversible},
            {"RIGHT-TO-LEFT", ReactionDirections.RightToLeft}
        }

        <Extension> Public Function GetAttributeList(Of T As Slots.Object)(data As DataFile(Of T)) As String()
            Return (From s As String
                    In data.AttributeList
                    Where Not s.StringEmpty
                    Select s
                    Distinct
                    Order By s.Length Descending).ToArray
        End Function

        ''' <summary>
        ''' 不会保留PGDB中的断行
        ''' </summary>
        ''' <param name="source"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Extension> Public Function ContactLines(source As String()) As String
            Dim sbr As StringBuilder = New StringBuilder(1024)
            Dim breaks As StringBuilder = New StringBuilder(512)

            For ind As Integer = 0 To source.Length - 1
                If ind = source.Length - 1 Then
                    sbr.AppendLine(source(ind))
                    Exit For
                End If

                If String.Compare(source(ind + 1).Chars(0), "/") <> 0 Then
                    sbr.AppendLine(source(ind))
                    Continue For
                End If

                breaks.Clear()
                breaks.Append(source(ind))
                ind += 1

                Do While String.Compare(source(ind).Chars(0), "/") = 0
                    breaks.Append(" ")
                    breaks.Append(source(ind).Substring(1))
                    ind += 1

                    If ind >= source.Length - 1 Then
                        Exit Do
                    End If
                Loop

                sbr.AppendLine(breaks.ToString)
            Next

            Call sbr.Replace("  ", " ")

            Return sbr.ToString
        End Function
    End Module
End Namespace
