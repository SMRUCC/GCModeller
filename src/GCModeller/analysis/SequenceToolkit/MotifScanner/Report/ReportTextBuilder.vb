#Region "Microsoft.VisualBasic::98254997e675c99d86d346409fca0725, GCModeller\analysis\SequenceToolkit\MotifScanner\Report\ReportTextBuilder.vb"

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

    '   Total Lines: 36
    '    Code Lines: 28
    ' Comment Lines: 0
    '   Blank Lines: 8
    '     File Size: 1.41 KB


    ' Module ReportTextBuilder
    ' 
    '     Sub: (+2 Overloads) BuildSearchReport
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices

Public Module ReportTextBuilder

    <Extension>
    Public Sub BuildSearchReport(result As IEnumerable(Of GeneReport), report As TextWriter)
        For Each gene As GeneReport In result
            Call gene.BuildSearchReport(report)
        Next
    End Sub

    <Extension>
    Public Sub BuildSearchReport(gene As GeneReport, report As TextWriter)
        Call report.WriteLine(">" & gene.locus_tag)
        Call report.WriteLine(" Length of sequence -            " & gene.length)
        Call report.WriteLine(" Threshold for promoters -       " & gene.threshold)
        Call report.WriteLine(" Number of predicted promoters - " & gene.tfBindingSites.Length)
        Call report.WriteLine($" Promoter Pos:     {gene.promoterPos} LDF-  {gene.promoterPosLDF}")

        For Each com In gene.components
            Call report.WriteLine(com.ToString)
        Next

        Call report.WriteLine()
        Call report.WriteLine(" Oligonucleotides from known TF binding sites:")
        Call report.WriteLine()

        Call report.WriteLine($" For promoter at     {gene.promoterPos}:")

        For Each site In gene.tfBindingSites
            Call report.WriteLine($"        {site.regulator}:  {site.oligonucleotides} at position      {site.position} Score -  {site.score}")
        Next
    End Sub

End Module
