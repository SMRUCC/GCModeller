#Region "Microsoft.VisualBasic::2771700778ea487de1afc427d4c351d8, CLI_tools\RNA-seq\CLI\WGCNA.vb"

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

    ' Module CLI
    ' 
    '     Function: FromWGCNA, GroupN
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.FileIO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics.Analysis.RNA_Seq
Imports SMRUCC.genomics.Analysis.RNA_Seq.dataExprMAT
Imports SMRUCC.genomics.Analysis.RNA_Seq.RTools.WGCNA

Partial Module CLI

    <ExportAPI("/WGCNA",
               Info:="Generates the cytoscape network model from WGCNA analysis.",
               Usage:="/WGCNA /data <dataExpr.csv> /anno <annotations.csv> [/out <DIR.Out> /mods <color.list> /from.DESeq /id.map <GeneId>]")>
    <ArgumentAttribute("/mods", True,
                   Description:="Each color in this parameter value is stands for a co expression module, and this parameter controls of the module output filtering, using | character as the seperator for each module color.")>
    <ArgumentAttribute("/out", True, Description:="Export directory of the WGCNA data, if this parameter value is not presents in the arguments, then the current work directory will be used.")>
    <ArgumentAttribute("/data", False,
                   Description:="A sets of RNA-seq RPKM expression data sets, the first row in the csv table should be the experiments or conditions, and first column in the table should be the id of the genes and each cell in the table should be the RPKM expression value of each gene in each condition.
                   The data format of the table it would be like:
                   GeneId, condi1, cond12, condi3, ....
                   locus1, x, xx, x,
                   locus2, y, yy, yyy,
                   locus3, ,z, zz, zzz,
                   ......

    xyz Is the RPKM of the genes")>
    <ArgumentAttribute("/anno", False, Description:="A table of the gene name annotation, the table should be in formats of
    Id, gene_symbol
    locus1, geneName
    locus2, geneName
    ....")>
    <ArgumentAttribute("/From.Deseq", True,
                   Description:="Is the /data matrix if comes from the DESeq analysis result output?
                   If is true, then the expression value will be extract from the original matrix file and save a new file named DESeq.dataExpr0.Csv in the out directory,
                   and last using this extracted data as the source of the WGCNA R script.")>
    Public Function FromWGCNA(args As CommandLine) As Integer
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
            WGCNA = RTools.RunScript.CallInvoke(
                dataExpr,
                annoCsv,
                outDIR,
                GeneIdLabel:=lbMap)
        Else
            WGCNA = RTools.RunScript.CallInvoke(
                dataExpr,
                annoCsv,
                outDIR,
                modules:=mods,
                GeneIdLabel:=lbMap)
        End If

        Return (Not String.IsNullOrEmpty(WGCNA)).CLICode
    End Function

    <ExportAPI("/Group.n", Usage:="/Group.n /in <dataset.csv> [/locus_map <locus> /out <out.csv>]")>
    Public Function GroupN(args As CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim locusMap As String = args.GetValue("/locus_map", "locus")
        Dim out As Integer = args.OpenHandle("/out", inFile.TrimSuffix & "-Groups.n.csv")
        Dim ds = EntityObject.LoadDataSet(inFile, locusMap)
        Dim st = (From x In ds Select x Group x By x.ID Into Count) _
            .Select(Function(item) New NamedValue(Of Integer)(item.ID, item.Count)) _
            .ToArray

        Return st.AsIOStream >> out
    End Function
End Module
