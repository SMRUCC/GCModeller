#Region "Microsoft.VisualBasic::2ad81c001879f215f78673811532374a, R#\kegg_kit\repository\dbget.vb"

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

    '   Total Lines: 66
    '    Code Lines: 19
    ' Comment Lines: 37
    '   Blank Lines: 10
    '     File Size: 2.77 KB


    ' Module dbget
    ' 
    '     Function: ShowOrganism
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Organism
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("dbget")>
Public Module dbget

    ''' <summary>
    ''' get kegg map from the kegg web server
    ''' </summary>
    ''' <param name="id"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("getMap")>
    <RApiReturn(GetType(Map))>
    Public Function getMap(id As String, Optional env As Environment = Nothing) As Object
        Return Map.ParseFromUrl($"https://www.kegg.jp/pathway/{id}")
    End Function

    '<ExportAPI("fetch.pathwayMaps")>
    'Public Function DownloadPathwayMaps(sp As String,
    '                                    Optional export As String = "./",
    '                                    Optional isKGML As Boolean = False,
    '                                    Optional env As Environment = Nothing) As Boolean

    '    Dim infoJSON$ = $"{export}/kegg.json"

    '    If infoJSON.LoadJSON(Of OrganismInfo)(throwEx:=False) Is Nothing Then
    '        Call env.globalEnvironment.options.setOption("dbget.cache", export)
    '        Call dbget _
    '            .ShowOrganism(code:=sp, env:=env) _
    '            .GetJson _
    '            .SaveTo(infoJSON)
    '    End If

    '    With infoJSON.LoadJSON(Of OrganismInfo)
    '        Dim assembly$ = .DataSource _
    '                        .Where(Function(d)
    '                                   Return InStr(d.text, "https://www.ncbi.nlm.nih.gov/assembly/", CompareMethod.Text) > 0
    '                               End Function) _
    '                        .First _
    '                        .name

    '        ' 在这里写入两个空文件是为了方便进行标记
    '        Call "".SaveTo($"{export}/{ .FullName}.txt")
    '        Call "".SaveTo($"{export}/{assembly}.txt")
    '        ' 这个文件方便程序进行信息的读取操作
    '        Call { .FullName, assembly}.FlushAllLines($"{export}/index.txt")
    '    End With

    '    If isKGML AndAlso export.StringEmpty Then
    '        export &= ".KGML/"

    '        Return MapDownloader _
    '            .DownloadsKGML(sp, export) _
    '            .SaveTo(export & "/failures.txt")
    '    Else
    '        Return LinkDB.Pathways _
    '            .Downloads(sp, export, cache:=export & "/.kegg/") _
    '            .SaveTo(export & "/failures.txt")
    '    End If
    'End Function

    <ExportAPI("show_organism")>
    Public Function ShowOrganism(code As String, Optional env As Environment = Nothing) As OrganismInfo
        Dim dbgetCache As String = env.globalEnvironment.options.getOption("dbget.cache", [default]:="./")
        Dim organism As OrganismInfo = OrganismInfo.ShowOrganism(
            code:=code,
            cache:=$"{dbgetCache}/.kegg/show_organism/"
        )

        Return organism
    End Function
End Module
