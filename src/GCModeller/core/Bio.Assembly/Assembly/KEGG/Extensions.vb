#Region "Microsoft.VisualBasic::c091fbaf08e01fce06e6d26fa2e25269, Bio.Assembly\Assembly\KEGG\Extensions.vb"

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

    '     Module Extensions
    ' 
    '         Function: DirectGetChEBI, GetIDpairedList, GetPathwayBrite, Glycan2CompoundId, IDlistStrings
    '                   LevelAKOStatics, RemarksTable, SingleID, (+2 Overloads) TheSameAs, ValidateEntryFormat
    '         Interface IKEGGRemarks
    ' 
    '             Properties: Remarks
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry

Namespace Assembly.KEGG

    <HideModuleName>
    Public Module Extensions

        ''' <summary>
        ''' + C compound
        ''' + D drug
        ''' + G glycan
        ''' </summary>
        Public Const KEGGCompoundIDPatterns$ = "[CDG]\d{5}"

        <Extension>
        Public Function Glycan2CompoundId(compounds As IEnumerable(Of bGetObject.Compound)) As Dictionary(Of String, String())
            Return compounds _
                .Where(Function(c) c.entry.StartsWith("G")) _
                .Select(Function(glycan)
                            Return New NamedValue(Of String()) With {
                                .Name = glycan.entry,
                                .Value = bGetObject.Glycan.GetCompoundId(glycan)
                            }
                        End Function) _
                .Where(Function(glycan) Not glycan.Value.IsNullOrEmpty) _
                .GroupBy(Function(glycan) glycan.Name) _
                .ToDictionary(Function(glycan) glycan.Key,
                              Function(group)
                                  Return group _
                                      .Select(Function(name) name.Value) _
                                      .IteratesALL _
                                      .Distinct _
                                      .ToArray
                              End Function)
        End Function

        ''' <summary>
        ''' 这个主要是应用于ID mapping操作的拓展函数
        ''' </summary>
        ''' <param name="text$">
        ''' Example as the text data in the kegg drugs or kegg disease object:
        ''' 
        ''' ```
        ''' E2A-PBX1 (translocation) [HSA:6929 5087] [KO:K09063 K09355]
        ''' ```
        ''' </param>
        ''' <returns></returns>
        <Extension>
        Public Function GetIDpairedList(text$) As Dictionary(Of String, String())
            Dim ids$() = Regex _
                .Matches(text, "\[.+?\]", RegexICSng) _
                .ToArray(Function(s) s.GetStackValue("[", "]"))

            ' 可能还会存在多重数据，所以在这里不能够直接生成字典
            Dim table As New Dictionary(Of String, List(Of String))

            For Each id As String In ids
                Dim k = id.GetTagValue(":")

                If Not table.ContainsKey(k.Name) Then
                    table(k.Name) = New List(Of String)
                End If

                Call table(k.Name).AddRange(k.Value.Split)
            Next

            Dim out As Dictionary(Of String, String()) = table _
                .ToDictionary(Function(k) k.Key,
                              Function(k)
                                  Return k.Value.ToArray
                              End Function)
            Return out
        End Function

        ''' <summary>
        ''' Example as: ``[ChEBI] 16810``
        ''' </summary>
        ''' <param name="cpd"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function DirectGetChEBI(cpd As DBGET.bGetObject.Compound) As String()
            Return cpd.DbLinks _
                      .Where(Function(s)
                                 Return s.DBName.TextEquals("ChEBI")
                             End Function) _
                      .Select(Function(l) l.Entry) _
                      .ToArray
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="tag$"></param>
        ''' <param name="list$"></param>
        ''' <returns>
        ''' Example as:
        ''' 
        ''' ```
        ''' HSA:6929 5087
        ''' ```
        ''' </returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function IDlistStrings(tag$, list$()) As String
            Return $"{tag}:{list.JoinBy(" ")}"
        End Function

        ''' <summary>
        ''' 检测判断所输入的字符串是否是符合格式要求的？
        ''' </summary>
        ''' <param name="s$"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function ValidateEntryFormat(s$) As Boolean
            Return s.MatchPattern("[a-z]+\d+")
        End Function

        ''' <summary>
        ''' Example as:
        ''' 
        ''' ```
        ''' Same as: C00001
        ''' ```
        ''' </summary>
        ''' <param name="s$"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension> Public Function TheSameAs(s$()) As String
            Dim tags As NamedValue(Of String) = s _
                .Select(Function(l) l.GetTagValue(":", trim:=True)) _
                .Where(Function(v) v.Name.TextEquals("Same as")) _
                .FirstOrDefault
            Return tags.Value
        End Function

        ''' <summary>
        ''' 将Remarks数据转换为字典对象
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="o"></param>
        ''' <returns></returns>
        <Extension>
        Public Function RemarksTable(Of T As IKEGGRemarks)(o As T) As Dictionary(Of String, String)
            If Not o.Remarks.IsNullOrEmpty Then
                Return o.Remarks _
                    .Select(Function(l) l.GetTagValue(":", trim:=True)) _
                    .ToDictionary(Function(tag) tag.Name,
                                  Function(tag) tag.Value)
            Else
                Return New Dictionary(Of String, String)
            End If
        End Function

        Public Function SingleID(theSameAs As String) As String
            Dim tokens = Strings.Trim(theSameAs).StringSplit("\s+")
            Dim CID As String = tokens _
                .Where(Function(id) id.IsPattern("C\d+")) _
                .FirstOrDefault

            If CID.StringEmpty Then
                Return tokens.FirstOrDefault
            Else
                Return CID
            End If
        End Function

        ''' <summary>
        ''' 得到和这个药物同义的KEGG代谢物编号, 返回来的字符串可能会包含有多个ID编号
        ''' 例如C\d+和G\d+可能会同时出现
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="o"></param>
        ''' <returns></returns>
        <Extension>
        Public Function TheSameAs(Of T As IKEGGRemarks)(o As T) As String
            If Not o.Remarks.IsNullOrEmpty Then
                Return o.Remarks.TheSameAs
            Else
                Return ""
            End If
        End Function

        Public Interface IKEGGRemarks
            Property Remarks As String()
        End Interface

        ''' <summary>
        ''' 这个函数会自动将物种的KEGG前缀去除掉，从而能够直接匹配字典之中的键名
        ''' </summary>
        ''' <param name="table"></param>
        ''' <param name="ID$"></param>
        ''' <returns></returns>
        <Extension>
        Public Function GetPathwayBrite(table As Dictionary(Of String, Pathway), ID$) As Pathway
            ID = Regex.Match(ID, "\d+").Value
            Return table.TryGetValue(ID)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="mappings">``{geneID -> KO}`` mapping data collection.</param>
        ''' <param name="keepsZERO">默认不保存计数为零的分类</param>
        ''' <returns>这个函数所返回去的数据一般是用作于绘图操作的</returns>
        <Extension>
        Public Function LevelAKOStatics(mappings As IEnumerable(Of NamedValue(Of String)),
                                        Optional ByRef KO_counts As KOCatalog() = Nothing,
                                        Optional keepsZERO As Boolean = False) _
                                        As Dictionary(Of String, NamedValue(Of Integer)())
            Dim brites As htext = htext.ko00001
            Dim KOTable As Dictionary(Of String, BriteHText) = brites.GetEntryDictionary
            Dim out As New Dictionary(Of String, NamedValue(Of Integer)())

            KO_counts = mappings _
                .GroupBy(Function(gene) gene.Value) _
                .Select(Function(x)
                            ' 对每一个KO进行数量上的统计分析
                            If KOTable.ContainsKey(x.Key) Then
                                Return New KOCatalog With {
                                    .Catalog = x.Key,
                                    .IDs = x.Select(Function(gene) gene.Name).ToArray,
                                    .Description = KOTable(.Catalog).description,
                                    .Class = KOTable(.Catalog).class
                                }
                            Else
                                Return New KOCatalog With {
                                    .Catalog = x.Key,
                                    .IDs = x.Select(Function(gene) gene.Name).ToArray,
                                    .Description = "No hits in KEGG KO database",
                                    .Class = "Unclassified"
                                }
                            End If
                        End Function) _
                .ToArray

            For Each [class] As BriteHText In brites.Hierarchical.categoryItems
                Dim profile As New List(Of NamedValue(Of Integer))

                For Each levelACatalog As BriteHText In [class].categoryItems
                    ' 在这里统计levelA的分布情况
                    Dim KO As Index(Of String) = levelACatalog _
                        .GetEntries _
                        .Where(Function(s) Not s.StringEmpty) _
                        .Indexing
                    profile += New NamedValue(Of Integer) With {
                        .Name = levelACatalog.classLabel,
                        .Description = levelACatalog.description,
                        .Value = KO_counts _
                            .Where(Function(tag) KO(tag.Catalog) > -1) _
                            .Count
                    }
                Next

                If keepsZERO Then
                    out([class].classLabel) = profile
                Else
                    out([class].classLabel) = profile _
                        .Where(Function(x) x.Value > 0) _
                        .ToArray
                End If
            Next

            Return out
        End Function
    End Module
End Namespace
