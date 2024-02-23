
Imports kegg_api.Html
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Emit.Delegates
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.XML
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Invokes
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization

<Package("parser")>
Public Module parser

    ''' <summary>
    ''' Request kegg map data model
    ''' </summary>
    ''' <param name="fs">
    ''' the cache filesystem, could be a local directory or a
    ''' filesystem object that implements the interface 
    ''' <see cref="IFileSystemEnvironment"/>.
    ''' </param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("kegg_map")>
    <RApiReturn(GetType(Map))>
    Public Function ParseHTML(id As String,
                              Optional fs As Object = "./.cache/",
                              Optional env As Environment = Nothing) As Object
        Dim web As WebQuery

        If fs Is Nothing Then
            Return Internal.debug.stop("the cache filesystem could not be nothing!", env)
        ElseIf TypeOf fs Is String Then
            web = New WebQuery(CLRVector.asCharacter(fs).First)
        ElseIf fs.GetType.ImplementInterface(Of IFileSystemEnvironment) Then
            Static cache As New Dictionary(Of UInteger, WebQuery)

            web = cache.ComputeIfAbsent(
                key:=CUInt(fs.GetHashCode),
                lazyValue:=Function()
                               Return New WebQuery(DirectCast(fs, IFileSystemEnvironment))
                           End Function)
        Else
            Return Message.InCompatibleType(GetType(IFileSystemEnvironment), fs.GetType, env)
        End If

        Dim ref As New Pathway With {.entry = New NamedValue(id, $"https://www.kegg.jp/pathway/{id}")}
        Dim map As Map = web.Query(Of Map)(ref, cacheType:=".html")

        Return map
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="cache">
    ''' the cache filesystem object, the parameter value could be a folder 
    ''' path on local filesystem or a clr object that implements the interface 
    ''' <see cref="IFileSystemEnvironment"/>.
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("fetch_kegg_maps")>
    Public Function getMaps(cache As Object, Optional env As Environment = Nothing) As Object
        Dim q As WebQuery
        Dim br08901 As EntityObject()
        Dim println = env.WriteLineHandler
        Dim REnv = env.globalEnvironment.Rscript

        ' load library module
        ' and then cast data object type
        REnv.Imports({"brite"}, baseDll:="kegg_kit.dll")
        br08901 = REnv.Evaluate("brite.as.table(__kegg__)", ("__kegg__", htext.br08901))

        '               class                   category subcategory    order    entry                                           name
        ' ----------------------------------------------------------------------------------------------------------------------------
        ' <mode>     <string>                   <string>    <string> <string> <string>                                       <string>
        ' [1, ]  "Metabolism" "Global and overview maps"        NULL     NULL  "01100"                           "Metabolic pathways"
        ' [2, ]  "Metabolism" "Global and overview maps"        NULL     NULL  "01110"        "Biosynthesis of secondary metabolites"
        ' [3, ]  "Metabolism" "Global and overview maps"        NULL     NULL  "01120" "Microbial metabolism in diverse environments"
        ' [4, ]  "Metabolism" "Global and overview maps"        NULL     NULL  "01200"                            "Carbon metabolism"
        ' [5, ]  "Metabolism" "Global and overview maps"        NULL     NULL  "01210"              "2-Oxocarboxylic acid metabolism"
        ' [6, ]  "Metabolism" "Global and overview maps"        NULL     NULL  "01212"                        "Fatty acid metabolism"
        ' [7, ]  "Metabolism" "Global and overview maps"        NULL     NULL  "01230"                  "Biosynthesis of amino acids"
        ' [8, ]  "Metabolism" "Global and overview maps"        NULL     NULL  "01232"                        "Nucleotide metabolism"
        ' [9, ]  "Metabolism" "Global and overview maps"        NULL     NULL  "01250"            "Biosynthesis of nucleotide sugars"
        ' [10, ] "Metabolism" "Global and overview maps"        NULL     NULL  "01240"                    "Biosynthesis of cofactors"
        ' [11, ] "Metabolism" "Global and overview maps"        NULL     NULL  "01220"            "Degradation of aromatic compounds"
        ' [12, ] "Metabolism"  "Carbohydrate metabolism"        NULL     NULL  "00010"                 "Glycolysis / Gluconeogenesis"
        ' [13, ] "Metabolism"  "Carbohydrate metabolism"        NULL     NULL  "00020"                    "Citrate cycle (TCA cycle)"
        '
        '  [ reached 'max' / getOption("max.print") -- omitted 549 rows ]

        Call base.print(REnv.Evaluate("as.data.frame(__kegg__)", ("__kegg__", br08901)), New list(("max.print", 13)), env)

        If cache Is Nothing Then
            Return Internal.debug.stop("the required of the cache pool can not be nothing!", env)
        End If

        If TypeOf cache Is String Then
            q = New WebQuery(CStr(cache))
        ElseIf cache.GetType.ImplementInterface(Of IFileSystemEnvironment) Then
            q = New WebQuery(DirectCast(cache, IFileSystemEnvironment))
        Else
            Return Message.InCompatibleType(GetType(IFileSystemEnvironment), cache.GetType, env)
        End If

        For Each row As EntityObject In Tqdm.Wrap(br08901)
            Dim refer As New Pathway With {
                .category = row!category,
                .[class] = row!class,
                .entry = New NamedValue With {.name = row.ID, .text = row!name}
            }
            Dim map As Map = q.Query(Of Map)(refer, cacheType:=".html")
            Dim folderName = New String() {
                refer.class.NormalizePathString(alphabetOnly:=False),
                refer.category.NormalizePathString(alphabetOnly:=False)
            }.JoinBy("/")
            Dim path As String = $"/KEGG_maps/{folderName}/map{row.ID}.xml"

            Call q.FileSystem.WriteText(map.GetXml, path)
            Call q.FileSystem.Flush()
            Call println($"[{row!name}] {path}")
        Next

        Return True
    End Function
End Module
