#Region "Microsoft.VisualBasic::81471160ebdfc8a6064e4986e8a0a12a, phenotype_kit\sampleInfo.vb"

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

    ' Module DEGSample
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: guessSampleGroups, print, ReadSampleInfo, ScanForSampleInfo, WriteSampleInfo
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner
Imports SMRUCC.Rsharp.Runtime.Internal.ConsolePrinter
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports REnv = SMRUCC.Rsharp.Runtime

''' <summary>
''' GCModeller DEG experiment analysis designer toolkit
''' </summary>
<Package("sampleInfo", Category:=APICategories.ResearchTools)>
Module DEGSample

    Sub New()
        Call printer.AttachConsoleFormatter(Of SampleInfo)(AddressOf print)
    End Sub

    Private Function print(sample As SampleInfo) As String
        Return $" ({sample.sample_info}) {sample.sample_name}"
    End Function

    <ExportAPI("guess.sample_groups")>
    Public Function guessSampleGroups(sample_names As Array) As list
        Return REnv.asVector(Of String)(sample_names) _
            .AsObjectEnumerator(Of String) _
            .GuessPossibleGroups _
            .ToDictionary(Function(group) group.name,
                          Function(group)
                              Return CObj(group.ToArray)
                          End Function) _
            .DoCall(Function(list)
                        Return New list With {
                            .slots = list
                        }
                    End Function)
    End Function

    <ExportAPI("read.sampleinfo")>
    Public Function ReadSampleInfo(file As String, Optional tsv As Boolean = False, Optional exclude_groups As String() = Nothing) As SampleInfo()
        Dim firstLine As String() = New RowObject(file.ReadFirstLine, tsv).ToArray
        Dim nameMaps As New NameMapping(New Dictionary(Of String, String) From {
            {firstLine(Scan0), NameOf(SampleInfo.ID)}
        })
        Dim samples As SampleInfo()

        If tsv Then
            samples = file.LoadTsv(Of SampleInfo)(nameMaps:=nameMaps).ToArray
        Else
            samples = file.LoadCsv(Of SampleInfo)(maps:=nameMaps).ToArray
        End If

        If Not exclude_groups Is Nothing Then
            With New Index(Of String)(exclude_groups)
                samples = samples _
                    .Where(Function(sample)
                               Return .IndexOf(sample.sample_info) = -1
                           End Function) _
                    .ToArray
            End With
        End If

        Return samples
    End Function

    <ExportAPI("write.sampleinfo")>
    Public Function WriteSampleInfo(sampleinfo As SampleInfo(), file$) As Boolean
        Return sampleinfo.SaveTo(file)
    End Function

    ''' <summary>
    ''' Create sampleInfo table from text files
    ''' </summary>
    ''' <param name="dir"></param>
    ''' <returns></returns>
    <ExportAPI("sampleinfo.text.groups")>
    Public Function ScanForSampleInfo(dir As String) As SampleInfo()
        Dim sampleInfo As New List(Of SampleInfo)
        Dim samplelist As String()
        Dim groupName$
        Dim index As i32 = 1

        For Each file As String In ls - l - r - "*.txt" <= dir
            groupName = file.BaseName
            samplelist = file.ReadAllLines
            sampleInfo += samplelist _
                .Select(Function(id)
                            Return New SampleInfo With {
                                .ID = id,
                                .sample_name = id,
                                .sample_info = groupName,
                                .injectionOrder = ++index
                            }
                        End Function)
        Next

        Return sampleInfo
    End Function
End Module
