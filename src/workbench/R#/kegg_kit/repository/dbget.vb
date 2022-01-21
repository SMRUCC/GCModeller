
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Organism
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.Rsharp.Runtime

<Package("dbget")>
Public Module dbget

    <ExportAPI("fetch.pathwayMaps")>
    Public Function DownloadPathwayMaps(sp As String,
                                        Optional export As String = "./",
                                        Optional isKGML As Boolean = False,
                                        Optional env As Environment = Nothing) As Boolean

        Dim infoJSON$ = $"{export}/kegg.json"

        If Not env.globalEnvironment.debugMode Then
            If infoJSON.LoadJSON(Of OrganismInfo)(throwEx:=False) Is Nothing Then
                Call env.globalEnvironment.options.setOption("dbget.cache", export)
                Call dbget _
                    .ShowOrganism(code:=sp, env:=env) _
                    .GetJson _
                    .SaveTo(infoJSON)
            End If
        End If

        With infoJSON.LoadJSON(Of OrganismInfo)
            Dim assembly$ = .DataSource _
                            .Where(Function(d)
                                       Return InStr(d.text, "https://www.ncbi.nlm.nih.gov/assembly/", CompareMethod.Text) > 0
                                   End Function) _
                            .First _
                            .name

            ' 在这里写入两个空文件是为了方便进行标记
            Call "".SaveTo($"{export}/{ .FullName}.txt")
            Call "".SaveTo($"{export}/{assembly}.txt")
            ' 这个文件方便程序进行信息的读取操作
            Call { .FullName, assembly}.FlushAllLines($"{export}/index.txt")
        End With

        If isKGML AndAlso export.StringEmpty Then
            export &= ".KGML/"

            Return MapDownloader _
                .DownloadsKGML(sp, export) _
                .SaveTo(export & "/failures.txt")
        Else
            Return LinkDB.Pathways _
                .Downloads(sp, export, cache:=export & "/.kegg/") _
                .SaveTo(export & "/failures.txt")
        End If
    End Function

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
