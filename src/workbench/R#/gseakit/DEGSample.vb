#Region "Microsoft.VisualBasic::c8ce4394270fb4c6b5c0e10853d6045f, R#\gseakit\DEGSample.vb"

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
    '     Function: print, ReadSampleInfo, ScanForSampleInfo, WriteSampleInfo
    ' 
    ' /********************************************************************************/

#End Region


Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
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
<Package("gseakit.DEG_sample", Category:=APICategories.ResearchTools)>
Module DEGSample

    Sub New()
        Call printer.AttachConsoleFormatter(Of SampleInfo)(AddressOf print)
    End Sub

    Private Function print(sample As SampleInfo) As String
        Return $" ({sample.sample_group}) {sample.sample_name}"
    End Function

    <ExportAPI("guess.sample_groups")>
    Public Function guessSampleGroups(sample_names As Array) As List
        Return REnv.asVector(Of String)(sample_names) _
            .AsObjectEnumerator(Of String) _
            .GuessPossibleGroups _
            .ToDictionary(Function(group) group.name,
                          Function(group)
                              Return CObj(group.ToArray)
                          End Function) _
            .DoCall(Function(list)
                        Return New List With {
                            .slots = list
                        }
                    End Function)
    End Function

    <ExportAPI("read.sampleinfo")>
    Public Function ReadSampleInfo(file As String) As SampleInfo()
        Return file.LoadCsv(Of SampleInfo)
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
                                .sample_group = groupName,
                                .Order = ++index
                            }
                        End Function)
        Next

        Return sampleInfo
    End Function
End Module

