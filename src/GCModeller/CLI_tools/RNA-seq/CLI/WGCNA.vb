Imports LANS.SystemsBiology.Toolkits.RNA_Seq
Imports LANS.SystemsBiology.Toolkits.RNA_Seq.dataExprMAT
Imports LANS.SystemsBiology.Toolkits.RNA_Seq.RTools.WGCNA
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Linq.Extensions

Partial Module CLI

    <ExportAPI("/WGCNA",
               Info:="Generates the cytoscape network model from WGCNA analysis.",
               Usage:="/WGCNA /data <dataExpr.csv> /anno <annotations.csv> [/out <DIR.Out> /mods <color.list> /from.DESeq /id.map <GeneId>]")>
    <ParameterInfo("/mods", True,
                   Description:="Each color in this parameter value is stands for a co expression module, and this parameter controls of the module output filtering, using | character as the seperator for each module color.")>
    <ParameterInfo("/out", True, Description:="Export directory of the WGCNA data, if this parameter value is not presents in the arguments, then the current work directory will be used.")>
    <ParameterInfo("/data", False,
                   Description:="A sets of RNA-seq RPKM expression data sets, the first row in the csv table should be the experiments or conditions, and first column in the table should be the id of the genes and each cell in the table should be the RPKM expression value of each gene in each condition.
                   The data format of the table it would be like:
                   GeneId, condi1, cond12, condi3, ....
                   locus1, x, xx, x,
                   locus2, y, yy, yyy,
                   locus3, ,z, zz, zzz,
                   ......

    xyz Is the RPKM of the genes")>
    <ParameterInfo("/anno", False, Description:="A table of the gene name annotation, the table should be in formats of
    Id, gene_symbol
    locus1, geneName
    locus2, geneName
    ....")>
    <ParameterInfo("/From.Deseq", True,
                   Description:="Is the /data matrix if comes from the DESeq analysis result output?
                   If is true, then the expression value will be extract from the original matrix file and save a new file named DESeq.dataExpr0.Csv in the out directory,
                   and last using this extracted data as the source of the WGCNA R script.")>
    Public Function FromWGCNA(args As CommandLine.CommandLine) As Integer
        Dim dataExpr As String = args("/data")
        Dim annoCsv As String = args("/anno")
        Dim outDIR As String = args.GetValue("/out", "./")
        Dim mods As String = args("/mods")
        Dim WGCNA As String
        Dim lbMap As String = args.GetValue("/id.map", "GeneId")

        If args.GetBoolean("/from.deseq") Then
            Dim dataExpr0 = dataExpr.LoadCsv(Of RTools.DESeq2.ResultData)
            Dim MAT As ExprMAT() = dataExprMAT.API.ToSamples(dataExpr0)
            Dim setValue = New SetValue(Of ExprMAT) <= NameOf(ExprMAT.dataExpr0)
            MAT = (From x As ExprMAT In MAT Select setValue(x, x.dataExpr0.RemoveLeft(""))).ToArray ' 这个空标题列数据是index不是想要的表达数据，删除掉
            dataExpr = outDIR & "/DeSeq.dataExpr0.Csv"
            lbMap = NameOf(ExprMAT.LocusId)
            Call MAT.SaveTo(dataExpr)
        End If

        If String.IsNullOrEmpty(mods) Then
            WGCNA = RTools.WGCNA.CallInvoke(
                dataExpr,
                annoCsv,
                outDIR,
                GeneIdLabel:=lbMap)
        Else
            WGCNA = RTools.WGCNA.CallInvoke(
                dataExpr,
                annoCsv,
                outDIR,
                modules:=mods,
                GeneIdLabel:=lbMap)
        End If

        Return (Not String.IsNullOrEmpty(WGCNA)).CLICode
    End Function

    <ExportAPI("/Group.n", Usage:="/Group.n /in <dataset.csv> [/locus_map <locus> /out <out.csv>]")>
    Public Function GroupN(args As CommandLine.CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim locusMap As String = args.GetValue("/locus_map", "locus")
        Dim out As Int = args.OpenHandle("/out", inFile.TrimFileExt & "-Groups.n.csv")
        Dim ds = DocumentStream.EntityObject.LoadDataSet(inFile, locusMap)
        Dim st = (From x In ds Select x Group x By x.Identifier Into Count).ToArray
        Return st > out
    End Function
End Module
