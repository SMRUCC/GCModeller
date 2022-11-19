#Region "Microsoft.VisualBasic::eaaa6046f2bfcb54245f441955433e74, GCModeller\core\Bio.Assembly\Assembly\NCBI\Database\GenBank\GBK\GbkWriter.vb"

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

    '   Total Lines: 218
    '    Code Lines: 144
    ' Comment Lines: 33
    '   Blank Lines: 41
    '     File Size: 8.55 KB


    '     Module GbkWriter
    ' 
    '         Function: __qualifierFormats, CreateDoc, (+12 Overloads) ToString, (+2 Overloads) WriteGenbank
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES

Namespace Assembly.NCBI.GenBank.GBFF

    ''' <summary>
    ''' 将数据写入现有的genbank文件或者创建新的genbank文件
    ''' </summary>
    <Package("NCBI.Genbank.WriteStream", Publisher:="GCModeller", Url:="http://gcmodeller.org")>
    Public Module GbkWriter

        ''' <summary>
        ''' 将genbank对象模型转换为文本文档数据以进行数据保存
        ''' </summary>
        ''' <param name="gb"></param>
        ''' <returns></returns>
        <ExportAPI("Doc.Create"), Extension> Public Function CreateDoc(gb As GenBank.GBFF.File) As String
            Dim writer As New StringBuilder(1024)

            On Error Resume Next

            ' 头部的元数据
            Call writer.AppendLine(ToString(gb.Locus))
            Call writer.AppendLine(ToString(gb.Definition))
            Call writer.AppendLine(ToString(gb.Accession))
            Call writer.AppendLine(ToString(gb.Version))
            Call writer.AppendLine(ToString(gb.DbLinks))
            Call writer.AppendLine(ToString(gb.Keywords))
            Call writer.AppendLine(ToString(gb.Source))
            Call writer.Append(ToString(gb.Reference))

            ' 位点注释数据
            Call writer.Append(ToString(gb.Features))

            ' 基因组序列数据
            Call writer.Append(ToString(gb.Origin))
            Call writer.AppendLine("//")

            Dim lines As String() = writer.ToString.LineTokens
            Dim text As String = String.Join(vbLf, lines)

            Return text
        End Function

        <ExportAPI("ToString")>
        Public Function ToString(link As Keywords.DBLINK) As String
            Return link.ToString
        End Function

        Const __INDEX As String = "         "
        ReadOnly __INDEX_LEN As Integer = __INDEX.Length

        ''' <summary>
        ''' 保存基因组序列数据
        ''' </summary>
        ''' <param name="origin"></param>
        ''' <returns></returns>
        <ExportAPI("ToString")>
        Public Function ToString(origin As Keywords.ORIGIN) As String
            Dim sbr As StringBuilder = New StringBuilder
            Dim array As Char() = origin.SequenceData.ToLower.Replace("-", "n").ToArray
            Dim lines As Char()() = array.Split(60)

            Call sbr.AppendLine("ORIGIN")

            For i As Integer = 0 To lines.Length - 1
                Dim line = lines(i)
                Dim index As String = CStr(i * 60 + 1)
                Dim buffer = line.Split(10)
                Dim s As String = New String(" ", __INDEX_LEN - index.Length) & index & " "

                Call sbr.Append(s)
                Call sbr.AppendLine(String.Join(" ", buffer.Select(Function(x) New String(x)).ToArray))
            Next

            Return sbr.ToString
        End Function

        ''' <summary>
        ''' 生成基因组位点注释数据
        ''' </summary>
        ''' <param name="features"></param>
        ''' <returns></returns>
        <ExportAPI("ToString")>
        Public Function ToString(features As Keywords.FEATURES.FEATURES) As String
            Dim sbr As New StringBuilder(1024)
            Call sbr.AppendLine("FEATURES             Location/Qualifiers")

            For Each feature In features._innerList
                Call sbr.Append(ToString(feature))
            Next

            Return sbr.ToString
        End Function

        Const __BLANK__ As String = "                     "
        ReadOnly __lenBlank As Integer = __BLANK__.Length

        ''' <summary>
        ''' 用于生成gbk文档里面的一部分文档节点
        ''' </summary>
        ''' <param name="feature"></param>
        ''' <returns></returns>
        <ExportAPI("ToString")>
        Public Function ToString(feature As Feature) As String
            Dim sbr As StringBuilder = New StringBuilder
            Dim blank As New String(" ", __lenBlank - 6 - feature.KeyName.Length)

            Call sbr.Append($"     {feature.KeyName}{blank} {feature.Location.ToString}")
            Call sbr.AppendLine()

            For Each q In feature.PairedValues
                Call sbr.Append(__qualifierFormats($"/{q.Name}=""{q.Value}"""))
            Next

            Return sbr.ToString
        End Function

        Private Function __qualifierFormats(s As String) As String
            Dim sbr As StringBuilder = New StringBuilder
            Dim array = s.ToArray.Split(58)

            For Each line As Char() In array
                Call sbr.Append(__BLANK__ & New String(line))
                Call sbr.AppendLine()
            Next

            Return sbr.ToString
        End Function

        <ExportAPI("ToString")>
        Public Function ToString(acc As Keywords.ACCESSION) As String
            Return $"ACCESSION   {acc.AccessionId}"
        End Function

        <ExportAPI("ToString")>
        Public Function ToString(version As Keywords.VERSION) As String
            Return $"VERSION     {version.AccessionID}.{version.Ver}  GI:{version.GI}"
        End Function

        <ExportAPI("ToString")>
        Public Function ToString(keyword As Keywords.KEYWORDS) As String
            Dim list As String = If(keyword.KeyWordList.IsNullOrEmpty, ".", keyword.KeyWordList.JoinBy(", "))
            Return $"KEYWORDS    " & list
        End Function

        <ExportAPI("ToString")>
        Public Function ToString(source As Keywords.SOURCE) As String
            Dim sbr As StringBuilder = New StringBuilder
            Call sbr.AppendLine($"SOURCE      {source.SpeciesName}")
            Call sbr.AppendLine($"  ORGANISM  {source.OrganismHierarchy.SpeciesName}")
            Call sbr.Append($"            " & source.OrganismHierarchy.Lineage.JoinBy("; "))

            Return sbr.ToString
        End Function

        <ExportAPI("ToString")>
        Public Function ToString(locus As GBFF.Keywords.LOCUS) As String
            Return $"LOCUS       {locus.AccessionID}             {locus.Length} bp    {locus.Type}     {locus.Molecular} BCT {locus.UpdateTime}"
        End Function

        <ExportAPI("ToString")>
        Public Function ToString(def As GBFF.Keywords.DEFINITION) As String
            Dim sbr As StringBuilder = New StringBuilder
            Call sbr.Append("DEFINITION  ")
            Call sbr.Append(def.Value)
            Return sbr.ToString
        End Function

        <ExportAPI("ToString")>
        Public Function ToString(i As Integer, ref As Keywords.RefInfo) As String
            Dim sbr As StringBuilder = New StringBuilder
            Call sbr.AppendLine($"REFERENCE   {i}  (base {ref.BaseLocation.Left} to {ref.BaseLocation.Right})")
            Call sbr.AppendLine($"  AUTHORS   {ref.Authors.JoinBy(", ")}")
            Call sbr.AppendLine($"  TITLE     {ref.Title}")
            Call sbr.AppendLine($"  JOURNAL   {ref.Journal}")
            Call sbr.Append($"   PUBMED   {ref.Pubmed}")

            Return sbr.ToString
        End Function

        <ExportAPI("ToString")>
        Public Function ToString(ref As Keywords.REFERENCE) As String
            Dim sbr As StringBuilder = New StringBuilder(1024)

            For i As Integer = 0 To ref.ReferenceList.Length - 1
                Call sbr.AppendLine(ToString(i + 1, ref.ReferenceList(i)))
            Next

            Return sbr.ToString
        End Function

        ''' <summary>
        ''' 使用这个函数将更新之后的genbank对象写入文件之中
        ''' </summary>
        ''' <param name="gb"></param>
        ''' <param name="path$">``*.gb``，所需要进行保存的genbank数据库的文件路径</param>
        ''' <param name="encoding">文本编码</param>
        ''' <returns></returns>
        <ExportAPI("Write.GBK")>
        <Extension>
        Public Function WriteGenbank(gb As GenBank.GBFF.File, path$, Optional encoding As Encoding = Nothing) As Boolean
            Dim doc As String = gb.CreateDoc()
            Return doc.SaveTo(path, encoding)
        End Function

        <ExportAPI("Write.GBK")>
        <Extension>
        Public Function WriteGenbank(gb As GenBank.GBFF.File, path$, Optional encoding As Encodings = Encodings.ASCII) As Boolean
            Return gb.WriteGenbank(path, encoding.CodePage)
        End Function
    End Module
End Namespace
