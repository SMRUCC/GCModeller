#Region "Microsoft.VisualBasic::7b78b4f993ec5aa36a7a85a5b1257f61, ..\GCModeller\core\Bio.Assembly\Assembly\NCBI\Database\GenBank\GBK\GbkWriter.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
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

Imports System.Runtime.CompilerServices
Imports System.Text
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Linq.Extensions

Namespace Assembly.NCBI.GenBank.GBFF

    <PackageNamespace("NCBI.Genbank.WriteStream", Publisher:="GCModeller", Url:="http://gcmodeller.org")>
    Public Module GbkWriter

        <ExportAPI("Doc.Create"), Extension>
        Public Function CreateDoc(gb As GenBank.GBFF.File) As String
            Dim gbBuilder As StringBuilder = New StringBuilder(1024)

            On Error Resume Next

            Call gbBuilder.AppendLine(ToString(gb.Locus))
            Call gbBuilder.AppendLine(ToString(gb.Definition))
            Call gbBuilder.AppendLine(ToString(gb.Accession))
            Call gbBuilder.AppendLine(ToString(gb.Version))
            Call gbBuilder.AppendLine(ToString(gb.DbLink))
            Call gbBuilder.AppendLine(ToString(gb.Keywords))
            Call gbBuilder.AppendLine(ToString(gb.Source))
            Call gbBuilder.Append(ToString(gb.Reference))
            Call gbBuilder.Append(ToString(gb.Features))
            Call gbBuilder.Append(ToString(gb.Origin))
            Call gbBuilder.AppendLine("//")

            Dim Tokens As String() = gbBuilder.ToString.lTokens
            Dim doc As String = String.Join(vbLf, Tokens)

            Return doc
        End Function

        <ExportAPI("ToString")>
        Public Function ToString(link As Keywords.DBLINK) As String
            Return link.ToString
        End Function

        Const __INDEX As String = "         "
        ReadOnly __INDEX_LEN As Integer = __INDEX.Length

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
                Call sbr.AppendLine(String.Join(" ", buffer.ToArray(Function(x) New String(x))))
            Next

            Return sbr.ToString
        End Function

        <ExportAPI("ToString")>
        Public Function ToString(features As Keywords.FEATURES.FEATURES) As String
            Dim sbr As StringBuilder = New StringBuilder(1024)
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
            Call sbr.Append($"     {feature.KeyName}{New String(" ", __lenBlank - 6 - feature.KeyName.Length)} {feature.Location.ToString}")
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
            Call sbr.Append($"            " & source.OrganismHierarchy.Categorys.JoinBy("; "))

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

        <ExportAPI("Write.GBK")>
        Public Function WriteGbk(gb As GenBank.GBFF.File, saveGb As String, Optional encoding As System.Text.Encoding = Nothing) As Boolean
            Dim doc As String = gb.CreateDoc()
            Return doc.SaveTo(saveGb, encoding)
        End Function
    End Module
End Namespace
