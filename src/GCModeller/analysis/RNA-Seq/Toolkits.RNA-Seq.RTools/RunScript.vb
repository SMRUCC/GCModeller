#Region "Microsoft.VisualBasic::ad5f706f4cae9dca148d8da97802be82, analysis\RNA-Seq\Toolkits.RNA-Seq.RTools\RunScript.vb"

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

    ' Module RunScript
    ' 
    '     Function: CallInvoke
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text
Imports RDotNET.Extensions.VisualBasic

Public Module RunScript

    Const DEFAULT_COLORS As String = "yellow|blue|grey|pink|red|black|turquoise|midnightblue|brown|magenta|purple|cyan|greenyellow|green|tan|salmon"

    ''' <summary>
    ''' Applying the WGCNA analysis on your transcriptome data.
    ''' </summary>
    ''' <param name="dataExpr">
    ''' The text encoding of this document should be ASCII, or the data reading in the R will be failed!
    ''' (转录组数据的csv文件的位置，请注意！，数据文件都必须是ASCII编码的)
    ''' </param>
    ''' <param name="GeneIdLabel">使用这个参数来修改Id映射</param>
    ''' <returns>
    ''' 函数返回的是最终的WGCNA导出到Cytoscape的网络模型文件的文件路径，假若脚本执行失败，则返回空字符串
    ''' </returns>
    ''' 
    <ExportAPI("WGCNA.Analysis")>
    Public Function CallInvoke(<Parameter("dataExpr.csv",
                                          "The csv document file path for the WGCNA data source. Which the first column in the document should be the genes locus_tag and the first row is the experiment title list.")>
                               dataExpr As String,
                               <Parameter("annotations.csv",
                                          "The csv document file path for the gene annotation data source, this file should contains at least two columns which one of the column should named Id for the genes' locus_tag and named gene_symbol for the gene name.")>
                               annotations As String,
                               <Parameter("DIR.Export",
                                          "Export the saved data and image plots to this directory, default location is current directory.")>
                               Optional outDIR As String = "./",
                               Optional GeneIdLabel As String = "GeneId",
                               <Parameter("list.mod", "Module was represents in colors, using | as seperator.")>
                               Optional modules As String = DEFAULT_COLORS) As String

        Dim WGCNA As New StringBuilder(Encoding.UTF8.GetString(My.Resources.WGCNA))
        outDIR = outDIR.GetDirectoryFullPath
        Call WGCNA.Replace("[dataExpr]", dataExpr.GetFullPath)
        Call WGCNA.Replace("[WORK]", outDIR)
        Call WGCNA.Replace("[GeneId_LABEL]", GeneIdLabel)
        Call WGCNA.Replace("[TOMsave]", BaseName(dataExpr) & ".TOMsave")
        Call WGCNA.Replace("[Annotations.csv]", annotations.GetFullPath)

        Dim mods As String() = modules.ToLower.Trim.Split("|"c).Select(Function(sCl) $"""{sCl}""").ToArray
        Call WGCNA.Replace("[list.MODs]", String.Join(", ", mods))
        Call WGCNA.SaveTo($"{outDIR}/{BaseName(dataExpr)}.WGCNACallInvoke.R", System.Text.Encoding.ASCII)

        Call dataExpr.TransEncoding(Encodings.ASCII)
        Call annotations.TransEncoding(Encodings.ASCII)

#If DEBUG Then
            Call My.Computer.FileSystem.CurrentDirectory.__DEBUG_ECHO
#End If
        Dim STD As String() = R.WriteLine(WGCNA.ToString)
        Dim Cytoscape As String = outDIR & "/CytoscapeEdges.txt"

        Call STD.SaveTo(outDIR & "/WGCNA.STD.txt")

        If Not Cytoscape.FileExists Then
            Return ""
        Else
            Return Cytoscape
        End If
    End Function
End Module
