#Region "Microsoft.VisualBasic::17b8a4b9727b2880e08eef3ae0aacefa, ..\core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\Pathway\Pathway.vb"

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

Imports System.Text.RegularExpressions
Imports System.Threading
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Terminal

Namespace Assembly.KEGG.DBGET.bGetObject

    ''' <summary>
    ''' The kegg pathway annotation data.(这个代谢途径模型是针对某一个物种而言的)
    ''' </summary>
    ''' <remarks></remarks>
    <XmlRoot("KEGG.PathwayBrief", Namespace:="http://www.genome.jp/kegg/pathway.html")>
    Public Class Pathway : Inherits ComponentModel.PathwayBrief

        ''' <summary>
        ''' The name value of this pathway object
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Name As String

        ''' <summary>
        ''' The module entry collection data in this pathway.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Modules As KeyValuePair()
        ''' <summary>
        ''' The kegg compound entry collection data in this pathway.
        ''' (可以通过这个代谢物的列表得到可以出现在当前的这个代谢途径之中的所有的非酶促反应过程，
        ''' 将整个基因组里面的化合物合并起来则可以得到整个细胞内可能存在的非酶促反应过程) 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Compound As KeyValuePair()
        Public Property Drugs As KeyValuePair()
        Public Property OtherDBs As KeyValuePair()
        Public Property PathwayMap As KeyValuePair

        Public Property Genes As KeyValuePair()
            Get
                Return _genes
            End Get
            Set(value As KeyValuePair())
                _genes = value

                If Not value.IsNullOrEmpty Then
                    _geneTable = value.ToDictionary(
                        Function(x) x.Key.Split(":"c).Last)
                Else
                    _geneTable = New Dictionary(Of String, KeyValuePair)
                End If
            End Set
        End Property

        Public Overrides ReadOnly Property BriteId As String
            Get
                Dim id As String = Regex.Match(EntryId, "\d+").Value
                If String.IsNullOrEmpty(id) Then
                    Return EntryId
                Else
                    Return id
                End If
            End Get
        End Property

        Public Property Disease As KeyValuePair()
        Public Property Organism As KeyValuePair
        Public Property KOpathway As KeyValuePair()

        ''' <summary>
        ''' Reference list of this biological pathway
        ''' </summary>
        ''' <returns></returns>
        Public Property References As Reference()

        Dim _genes As KeyValuePair()
        Dim _geneTable As New Dictionary(Of String, KeyValuePair)

        Const SEARCH_URL As String = "http://www.kegg.jp/kegg-bin/search_pathway_text?map={0}&keyword=&mode=1&viewImage=false"
        Const PATHWAY_DBGET As String = "http://www.genome.jp/dbget-bin/www_bget?pathway:{0}{1}"

        Public Function IsContainsCompound(KEGGCompound As String) As Boolean
            If Compound.IsNullOrEmpty Then
                Return False
            End If

            Dim thisLinq = LinqAPI.DefaultFirst(Of KeyValuePair)() <=
 _
                From comp As KeyValuePair
                In Compound
                Where String.Equals(comp.Key, KEGGCompound)
                Select comp

            Return Not [Class](Of KeyValuePair).IsNullOrEmpty Like thisLinq
        End Function

        Public Function IsContainsGeneObject(GeneId As String) As Boolean
            Return _geneTable.ContainsKey(GeneId)
        End Function

        ''' <summary>
        ''' Is current pathway object contains the specific module information?(当前的代谢途径对象是否包含有目标模块信息.)
        ''' </summary>
        ''' <param name="ModuleId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function IsContainsModule(ModuleId As String) As Boolean
            If Modules.IsNullOrEmpty Then
                Return False
            End If
            Dim LQuery As KeyValuePair = LinqAPI.DefaultFirst(Of KeyValuePair) <=
 _
                From [mod] As KeyValuePair
                In Modules
                Where String.Equals([mod].Key, ModuleId)
                Select [mod]

            Return Not LQuery Is Nothing
        End Function

        ''' <summary>
        ''' Downloads all of the available pathway information for the target species genome.(下载目标基因组对象之中的所有可用的代谢途径信息)
        ''' </summary>
        ''' <param name="sp">The brief code of the target genome species in KEGG database.(目标基因组在KEGG数据库之中的简写编号.)</param>
        ''' <param name="EXPORT"></param>
        ''' <returns>函数返回下载失败的代谢途径的编号代码</returns>
        ''' <remarks></remarks>
        Public Shared Function Download(sp$, EXPORT As String, Optional briefFile$ = "") As String()
            Dim source As BriteHEntry.Pathway() = __source(briefFile)
            Dim failures As New List(Of String)

            Using progress As New ProgressBar("Download KEGG pathway data for " & sp,, CLS:=True)
                Dim tick As New ProgressProvider(source.Length)
                Dim ETA$

                For Each entry As KEGG.DBGET.BriteHEntry.Pathway In source
                    Dim EntryID As String = String.Format("{0}{1}", sp, entry.Entry.Key)
                    Dim saveDIR As String = BriteHEntry.Pathway.CombineDIR(entry, EXPORT)

                    Dim xml As String = String.Format("{0}/{1}.xml", saveDIR, EntryID)
                    Dim png As String = String.Format("{0}/{1}.png", saveDIR, EntryID)

                    If xml.FileExists AndAlso png.FileExists Then
                        Continue For
                    End If

                    Dim pathway As Pathway = DownloadPage(sp, entry.Entry.Key)

                    If pathway Is Nothing Then
                        Call $"[{sp}] {entry.ToString} is not exists in the KEGG!".__DEBUG_ECHO
                        failures += EntryID
                    Else
                        Call pathway.GetXml.SaveTo(xml)
                        Call DownloadPathwayMap(sp, entry.Entry.Key, EXPORT:=saveDIR)
                        Call Thread.Sleep(1000)
                    End If
Exit_LOOP:
                    ETA = "ETA:= " & tick.ETA(progress.ElapsedMilliseconds).FormatTime
                    Call progress.SetProgress(tick.StepProgress, ETA)
                Next
            End Using

            Return failures
        End Function

        Private Shared Function __source(path$) As BriteHEntry.Pathway()
            If path.FileLength = 0 Then
                Return BriteHEntry.Pathway.LoadFromResource
            Else
                Return BriteHEntry.Pathway.LoadData(path)
            End If
        End Function

        ''' <summary>
        ''' 下载pathway的图片
        ''' </summary>
        ''' <param name="sp$"></param>
        ''' <param name="entry$"></param>
        ''' <param name="EXPORT$"></param>
        ''' <returns></returns>
        Public Shared Function DownloadPathwayMap(sp$, entry$, EXPORT$) As Boolean
            Dim url As String = $"http://www.genome.jp/kegg/pathway/{sp}/{sp}{entry}.png"
            Dim path$ = String.Format("{0}/{1}{2}.png", EXPORT, sp, entry)
            Return url.DownloadFile(save:=path)
        End Function

        Public Shared Function DownloadPage(sp As String, Entry As String) As Pathway
            Return DownloadPage(url:=String.Format(PATHWAY_DBGET, sp, Entry))
        End Function

        ''' <summary>
        ''' 从某一个页面url或者文件路径所指向的网页文件之中解析出模型数据
        ''' </summary>
        ''' <param name="url"></param>
        ''' <returns></returns>
        Public Shared Function DownloadPage(url As String) As Pathway
#If Not DEBUG Then
            Try
#End If
                Return url.PageParser
#If Not DEBUG Then
            Catch ex As Exception
                ex = New Exception(url, ex)
                Call ex.PrintException
                Throw ex
            End Try
#End If
        End Function

        Public Shared Function GetCompoundCollection(source As IEnumerable(Of Pathway)) As String()
            Dim out As New List(Of String)

            For Each pwy As Pathway In source
                If pwy.Compound.IsNullOrEmpty Then
                    Continue For
                End If

                out += From met As KeyValuePair
                       In pwy.Compound
                       Select met.Key
            Next

            Return LinqAPI.Exec(Of String) <=
 _
                From sId As String
                In out
                Where Not String.IsNullOrEmpty(sId)
                Select sId
                Distinct
                Order By sId Ascending

        End Function

        ''' <summary>
        ''' Imports KEGG compounds from pathways model.
        ''' </summary>
        ''' <param name="ImportsDIR"></param>
        ''' <returns></returns>
        Public Shared Function GetCompoundCollection(ImportsDIR As String) As String()
            Dim LQuery As IEnumerable(Of Pathway) =
 _
                From xml As String
                In ls - l - r - wildcards("*.xml") <= ImportsDIR
                Select xml.LoadXml(Of Pathway)()

            Return GetCompoundCollection(LQuery)
        End Function

        ''' <summary>
        ''' 获取这个代谢途径之中的所有的基因。这个是安全的函数，当出现空值的基因集合的时候函数会返回一个空集合而非空值
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function GetPathwayGenes() As String()
            If Genes.IsNullOrEmpty Then
                Call $"{EntryId} gene set is Null!".__DEBUG_ECHO
                Return {}
            End If

            Dim LQuery As String() = Genes _
                .Select(Function(g) g.Key.Split(":"c).Last) _
                .ToArray
            Return LQuery
        End Function
    End Class
End Namespace
