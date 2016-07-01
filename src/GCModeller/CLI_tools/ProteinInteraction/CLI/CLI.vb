#Region "Microsoft.VisualBasic::d9f02fc4009934cd13c7f3038c12d73e, ..\GCModeller\CLI_tools\ProteinInteraction\CLI\CLI.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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

Imports SMRUCC.genomics.AnalysisTools.ProteinTools.Interactions.SwissTCS
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic

<PackageNamespace("Protein.Interactions.Tools", Category:=APICategories.CLI_MAN,
                  Description:="Tools for analysis the protein interaction relationship.",
                  Publisher:="xie.guigang@gcmodeller.org",
                  Url:="http://gcmodeller.org")>
Public Module CLI

    <ExportAPI("--interact.TCS", Usage:="--interact.TCS /door <door.opr> /MiST2 <mist2.xml> /swiss <tcs.csv.DIR> /out <out.DIR>")>
    Public Function TCSParser(args As CommandLine.CommandLine) As Integer
        Dim MiST2 = args("/mist2").LoadXml(Of SMRUCC.genomics.Assembly.MiST2.MiST2)  ' 主要是从这个模块之中获取TCS的基因定义
        Dim Door = SMRUCC.genomics.Assembly.Door.Load(args("/door"))
        Dim cTkDIR As String = args("/swiss")
        Dim outDIR As String = args.GetValue("/out", App.CurrentDirectory)
        Dim CrossTalks = FileIO.FileSystem.GetFiles(cTkDIR, FileIO.SearchOption.SearchAllSubDirectories, "*.csv") _
            .ToArray(Function(csv) csv.LoadCsv(Of CrossTalks)).MatrixToList

        For Each rep As Assembly.MiST2.Replicon In MiST2.MajorModules

            Dim lstHisk As String() = rep.TwoComponent.get_HisKinase
            Dim lstRR As String() = rep.TwoComponent.GetRR

            For Each HisK As String In lstHisk
                Dim lstChunk As New List(Of CrossTalks)

                For Each RR As String In lstRR

                    Dim p As Double = CrossTalks.CrossTalk(HisK, RR)

                    If Door.SameOperon(HisK, RR) Then  ' 同源的？？？
                        If Not p > 0 Then
                            p = 1
                        End If

                        Call lstChunk.Add(New CrossTalks With {.Kinase = HisK, .Regulator = RR, .Probability = p})
                    Else
                        If p > 0 Then
                            Call lstChunk.Add(New CrossTalks With {.Kinase = HisK, .Regulator = RR, .Probability = p})
                        End If
                    End If
                Next

                If Not lstChunk.IsNullOrEmpty Then
                    Call lstChunk.SaveTo(outDIR & $"/{HisK}.csv")
                End If
            Next
        Next

        Return 0
    End Function
End Module

