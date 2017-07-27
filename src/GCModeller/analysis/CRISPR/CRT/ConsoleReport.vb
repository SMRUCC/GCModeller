#Region "Microsoft.VisualBasic::d7628f6886253de51c1cb2e329cfe04c, ..\GCModeller\analysis\CRISPR\CRT\ConsoleReport.vb"

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

Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData

<Package("CRISPR.Searchs.Reports")>
Public Module ConsoleReport

    <ExportAPI("Prints")>
    <Extension> Public Function Print(dat As IEnumerable(Of SearchingModel.CRISPR)) As String
        If dat.IsNullOrEmpty Then
            Return ""
        Else
            Return dat.__print
        End If
    End Function

    <Extension> Private Function __print(dat As IEnumerable(Of SearchingModel.CRISPR)) As String
        Dim positionMaxLength As Integer = Len((From c In dat Select c.Repeats).IteratesALL.Max.ToString)
        Dim RepeatsMaxLength As Integer = (From c In dat Select c.RepeatLength).Max
        Dim SpacersMaxLength As Integer = (From c In dat Select c.AverageSpacerLength).Max
        Dim sBuilder As New StringBuilder(2048)
        Dim i As int = 0

        positionMaxLength = If(positionMaxLength > 9, positionMaxLength, 9)

        For Each site As SearchingModel.CRISPR In dat
            Call sBuilder.AppendLine($"  ------> CRISPR {++i}")
            Call sBuilder.__print(site, positionMaxLength, RepeatsMaxLength, SpacersMaxLength)
        Next

        Return sBuilder.ToString
    End Function

    <Extension> Private Sub __print(sb As StringBuilder, site As SearchingModel.CRISPR, positionMaxLength%, RepeatsMaxLength%, SpacersMaxLength%)
        Call sb.AppendLine("Position" & New String(" "c, positionMaxLength - 8) & vbTab & "Repeats" & New String(" "c, RepeatsMaxLength) & vbTab & "Space Sequence")
        Call sb.AppendLine(New String("-"c, positionMaxLength) & vbTab & New String("-"c, RepeatsMaxLength) & vbTab & New String("-"c, SpacersMaxLength))

        For j As Integer = 0 To site.NumberOfRepeats - 1
            Call sb.Append((site.RepeatAt(j) + 1) & vbTab & vbTab & site.RepeatStringAt(j) & vbTab)

            ' print spacer
            ' because there are no spacers after the last repeat, we stop early (m < crisprIndexVector.size() - 1)
            If j < site.NumberOfSpacers() Then
                Dim Spacer As String = site.SpacerStringAt(j)

                Call sb.Append(Spacer & New String(" "c, SpacersMaxLength - Len(Spacer)))
                Call sb.AppendLine(String.Format(vbTab & "[ {0}, {1} ]", site.RepeatStringAt(j).Length, site.SpacerStringAt(j).Length))
            End If
        Next

        Call sb.Append(vbLf & New String("-"c, positionMaxLength) & vbTab)
        Call sb.Append(New String("-"c, RepeatsMaxLength) & vbTab)
        Call sb.AppendLine(New String("-"c, SpacersMaxLength))
    End Sub
End Module
