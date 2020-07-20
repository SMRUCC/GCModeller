#Region "Microsoft.VisualBasic::55fc98ab0e77f17d4089d1d8bca085f7, Bio.Assembly\Assembly\NCBI\Database\GenBank\TabularFormat\FeatureBriefs\PTT\PTT.vb"

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

    '     Class PTT
    ' 
    '         Properties: forwards, GeneIDList, GeneObjects, GetsGeneDatas, NumOfProducts
    '                     reversed, Size, Title
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: Copy, ExistsLocusId, GetEnumerator, GetEnumerator1, GetEnumerator2
    '                   GetGeneByDescription, GetGeneByName, (+2 Overloads) GetObject, GetRelatedGenes, GetStrandGene
    '                   Load, OrderByGeneID, (+2 Overloads) Save, SaveXml, ToDictionary
    '                   TryGetGeneObjectValue, TryGetGenesId
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ApplicationServices.Terminal
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.ContextModel

Namespace Assembly.NCBI.GenBank.TabularFormat

    ''' <summary>
    ''' The brief information of a genome.(基因组的摘要信息)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class PTT : Implements ISaveHandle
        Implements IReadOnlyCollection(Of GeneBrief)
        Implements IReadOnlyDictionary(Of String, GeneBrief)
        Implements IGenomicsContextProvider(Of GeneBrief)

        ''' <summary>
        ''' The gene brief information collection data in this genome.(这个基因组之中的所有的基因摘要数据)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property GeneObjects As GeneBrief() Implements IGenomicsContextProvider(Of GeneBrief).AllFeatures
            Get
                Return _innerList
            End Get
            Set(value As GeneBrief())
                _innerList = value

                Dim isNullLocus As String() = (From g As GeneBrief
                                               In value
                                               Select g.Synonym
                                               Distinct).ToArray

                If isNullLocus.Length = 1 AndAlso
                    String.Equals(isNullLocus.First, "-") Then
                    For Each g As GeneBrief In value
                        g.Synonym = g.Gene
                    Next
                End If

                __innerTable = value.ToDictionary(Function(g) g.Synonym)

                _forwards = (From gene As GeneBrief In value Where gene.Location.Strand = Strands.Forward Select gene).ToArray
                _reversed = (From gene As GeneBrief In value Where gene.Location.Strand = Strands.Reverse Select gene).ToArray
                _contextModel = New GenomeContextProvider(Of GeneBrief)(Me)
            End Set
        End Property

        Dim _contextModel As GenomeContextProvider(Of GeneBrief)

        Public ReadOnly Property forwards As GeneBrief()
        Public ReadOnly Property reversed As GeneBrief()

        ''' <summary>
        ''' 基因组的标题
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Title As String

        ''' <summary>
        ''' The genome original sequence length.(基因组序列的长度)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Size As Integer

        ''' <summary>
        ''' {<see cref="ComponentModels.GeneBrief.Synonym"/>, <see cref="ComponentModels.GeneBrief"/>}
        ''' </summary>
        Dim __innerTable As Dictionary(Of String, GeneBrief)
        Protected Friend _innerList As GeneBrief()

        Public Function ToDictionary() As Dictionary(Of String, GeneBrief)
            Return __innerTable
        End Function

        Public Function OrderByGeneID() As PTT
            Me.GeneObjects = (From g As GeneBrief
                              In Me.GeneObjects
                              Select g
                              Order By g.Synonym Ascending).ToArray
            Return Me
        End Function

        ''' <summary>
        ''' 查看某一个位点之上有哪些基因，假若需要查看某一个位点附近有哪些基因的话，则可以使用
        ''' <see cref="GetRelatedGenes"></see>方法进行查找
        ''' </summary>
        ''' <param name="site">基因组序列之上的一个碱基位点</param>
        ''' <param name="ComplementStrand">该目标基因是否位于互补链之上</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetObject(site As Integer, ComplementStrand As Boolean) As GeneBrief()
            Dim gs As GeneBrief()

            If ComplementStrand Then
                gs = (From gene As GeneBrief In _innerList
                      Where gene.Location.Strand = Strands.Reverse
                      Select gene).ToArray
            Else
                gs = (From gene As GeneBrief In _innerList
                      Where gene.Location.Strand = Strands.Forward
                      Select gene).ToArray
            End If

            Dim LQuery = (From g As GeneBrief In gs Where g.Location.IsInside(site) Select g).ToArray
            Return LQuery
        End Function

        Public Function GetObject(site As Integer, direction As Strands) As GeneBrief()
            Dim Data As GeneBrief()

            If direction = Strands.Reverse Then
                Data = (From gene In _innerList Where gene.Location.Strand = Strands.Reverse Select gene).ToArray
            ElseIf direction = Strands.Forward Then
                Data = (From ItemGene In _innerList Where ItemGene.Location.Strand = Strands.Forward Select ItemGene).ToArray
            Else
                Data = _innerList
            End If

            Dim LQuery = (From item In Data Where item.Location.IsInside(site) Select item).ToArray
            Return LQuery
        End Function

        ''' <summary>
        ''' Find gene by using genen name.
        ''' </summary>
        ''' <param name="Name">基因名称，而非基因号</param>
        ''' <returns></returns>
        Public Function GetGeneByName(Name As String) As GeneBrief Implements IGenomicsContextProvider(Of GeneBrief).GetByName
            ' 先假设这个Name值为基因号
            Dim found As GeneBrief = Me(Name)

            If Not found Is Nothing Then
                Return found
            End If

            Return LinqAPI.DefaultFirst(Of GeneBrief) <=
 _
                From gene As GeneBrief
                In Me._innerList  ' 尝试进行字符串大小写不敏感的非严格匹配
                Where String.Equals(gene.Gene, Name, StringComparison.OrdinalIgnoreCase)
                Select gene

        End Function

        ''' <summary>
        ''' String fuzzy match on the gene ``product`` value
        ''' </summary>
        ''' <param name="matches"></param>
        ''' <returns></returns>
        Public Iterator Function GetGeneByDescription(matches As Func(Of String, Boolean)) As IEnumerable(Of GeneBrief)
            For Each result As GeneBrief In From gene As GeneBrief
                                            In Me._innerList
                                            Where matches(gene.Product)
                                            Select gene
                Yield result
            Next
        End Function

        Sub New()
        End Sub

        Sub New(genes As IEnumerable(Of GeneBrief), Optional title As String = "", Optional ntLen As Integer = 0)
            Me.GeneObjects = genes.ToArray
            title = title
            Size = ntLen
        End Sub

        Public Overloads Function Copy(lstId As String()) As PTT
            Return New PTT With {
                .Size = Size,
                .Title = Title,
                .GeneObjects =
                    LinqAPI.Exec(Of GeneBrief) <= From g As GeneBrief
                                                  In _innerList.AsParallel
                                                  Where Array.IndexOf(lstId, g.Synonym) > -1
                                                  Select g
            }
        End Function

#Region "IO Operations"

        Public Function Save(path As String, encoding As Encoding) As Boolean Implements ISaveHandle.Save
            Using file As New StreamWriter(path.Open(FileMode.OpenOrCreate, doClear:=True), encoding)
                Call WriteDocument(file)
                Return True
            End Using
        End Function

        ''' <summary>
        ''' Save this ptt document as an xml document.(保存Ptt数据库为Xml文件)
        ''' </summary>
        ''' <param name="Path"></param>
        ''' <param name="encoding"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SaveXml(Path As String, Optional encoding As Encoding = Nothing) As Boolean
            Return Me.GetXml.SaveTo(Path, encoding)
        End Function

        ''' <summary>
        ''' Load a ptt format text file, the ptt file is usually can be found at Ncbi FTP server for each species genome data.
        ''' </summary>
        ''' <param name="path">Text file format ptt file path</param>
        ''' <returns></returns>
        ''' <param name="fillBlank">Fill blank gene name.</param>
        ''' <remarks></remarks>
        Public Shared Function Load(path As String, Optional fillBlank As Boolean = False) As PTT
            If Not path.FileExists Then
                Call xConsole.WriteLine($"^r {path.ToFileURL} is not exists on your file system!^!")
                Return Nothing
            End If

            Try
                Return PTTFileReader.Read(path, fillBlank)
            Catch ex As Exception
                Dim exMsg As String = $"[PTT_LOADDER_ERROR]  -----> PTT database file ""{path.ToFileURL}"""
                ex = New Exception(exMsg, ex)
                Return App.LogException(ex)
            End Try
        End Function

        Public Overloads Shared Widening Operator CType(FilePath As String) As PTT
            Return PTT.Load(FilePath)
        End Operator
#End Region

#Region "Implements IReadOnlyCollection(Of GeneBrief)"

        Public Iterator Function GetEnumerator() As IEnumerator(Of GeneBrief) Implements IEnumerable(Of GeneBrief).GetEnumerator
            For Each item As GeneBrief In GeneObjects
                Yield item
            Next
        End Function

        ''' <summary>
        ''' The number of gene product counts in this genome data: <see cref="GeneObjects"/>.Length .(当前的基因组数据之中的基因对象的数目)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property NumOfProducts As Integer Implements IReadOnlyCollection(Of GeneBrief).Count, IReadOnlyCollection(Of KeyValuePair(Of String, GeneBrief)).Count
            Get
                Return GeneObjects.Length
            End Get
        End Property

        Private Iterator Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
#End Region

#Region "Implements IReadOnlyDictionary(Of String, GeneBrief)"

        Public Iterator Function GetEnumerator2() As IEnumerator(Of KeyValuePair(Of String, GeneBrief)) Implements IEnumerable(Of KeyValuePair(Of String, GeneBrief)).GetEnumerator
            For Each Item As KeyValuePair(Of String, GeneBrief) In __innerTable
                Yield Item
            Next
        End Function

        ''' <summary>
        ''' Does the target locus_tag exists in this genome brief data model.
        ''' (通过基因的locus_tag, <see cref="GeneBrief.Synonym"/>来获取某一个基因对象是否存在)
        ''' </summary>
        ''' <param name="locusId"><see cref="GeneBrief.Synonym"/></param>
        ''' <returns></returns>
        Public Function ExistsLocusId(locusId As String) As Boolean Implements IReadOnlyDictionary(Of String, GeneBrief).ContainsKey
            Return __innerTable.ContainsKey(locusId)
        End Function

        ''' <summary>
        ''' 通过基因的Locus_Tag, <see cref="ComponentModels.GeneBrief.Synonym"/>来获取某一个基因对象，不存在则返回空值
        ''' </summary>
        ''' <param name="locusId"><see cref="ComponentModels.GeneBrief.Synonym"/></param>
        ''' <returns></returns>
        Default Public ReadOnly Property GeneObject(locusId As String) As GeneBrief _
            Implements _
            IReadOnlyDictionary(Of String, GeneBrief).Item,
            IGenomicsContextProvider(Of GeneBrief).Feature

            Get
                If __innerTable.ContainsKey(locusId) Then
                    Return __innerTable(locusId)
                Else
                    Return Nothing
                End If
            End Get
        End Property

        ''' <summary>
        ''' Gets a list of all of the gene object in this ptt document.(所有基因的基因号<see cref="ComponentModels.GeneBrief.Synonym"/>列表)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property GeneIDList As IEnumerable(Of String) Implements IReadOnlyDictionary(Of String, GeneBrief).Keys
            Get
                Return __innerTable.Keys
            End Get
        End Property

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="GeneID"><see cref="ComponentModels.GeneBrief.Synonym"/></param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        Public Function TryGetGeneObjectValue(GeneID As String, ByRef value As GeneBrief) As Boolean Implements IReadOnlyDictionary(Of String, GeneBrief).TryGetValue
            Return __innerTable.TryGetValue(GeneID, value)
        End Function

        Public ReadOnly Property GetsGeneDatas As IEnumerable(Of GeneBrief) Implements IReadOnlyDictionary(Of String, GeneBrief).Values
            Get
                Return __innerTable.Values
            End Get
        End Property
#End Region

        ''' <summary>
        ''' Gets the strand specified genes.
        ''' </summary>
        ''' <param name="strand"><see cref="ComponentModel.Loci.Strands.Forward"/>/<see cref="ComponentModel.Loci.Strands.Reverse"/>,
        ''' <see cref="ComponentModel.Loci.Strands.Unknown"/> will be treated as reversed.</param>
        ''' <returns></returns>
        Public Function GetStrandGene(strand As Strands) As GeneBrief() Implements IGenomicsContextProvider(Of GeneBrief).GetStrandFeatures
            If strand = Strands.Forward Then
                Return forwards
            Else
                Return reversed
            End If
        End Function

        ''' <summary>
        ''' 由于GFF文件之中是按照GeneName来进行标识的，有些时候希望全部使用基因号进行标识，所以通过这个函数将基因名称统一转换为基因号
        ''' </summary>
        ''' <param name="tag">Tag data matches in fields:
        ''' <see cref="ComponentModels.GeneBrief.Synonym"/>,
        ''' <see cref="ComponentModels.GeneBrief.Gene"/>,
        ''' <see cref="ComponentModels.GeneBrief.PID"/>
        ''' </param>
        ''' <returns></returns>
        Public Function TryGetGenesId(tag As String) As String
            Dim LQuery = (From gene As GeneBrief
                          In Me.GeneObjects.AsParallel
                          Where String.Equals(gene.Synonym, tag) OrElse
                              String.Equals(gene.Gene, tag) OrElse
                              String.Equals(gene.PID, tag)
                          Select gene).FirstOrDefault
            If LQuery Is Nothing Then
                Return ""
            Else
                Return LQuery.Synonym
            End If
        End Function

        Public Overloads Shared Narrowing Operator CType(obj As PTT) As GeneBrief()
            Return obj.GeneObjects
        End Operator

        ''' <summary>
        ''' 获取某一个位点在位置上有相关联系的基因
        ''' </summary>
        ''' <param name="loci"></param>
        ''' <param name="unstrand"></param>
        ''' <param name="ATGDist"></param>
        ''' <returns></returns>
        Public Function GetRelatedGenes(loci As NucleotideLocation,
                                        Optional unstrand As Boolean = False,
                                        Optional ATGDist As Integer = 500) As Relationship(Of GeneBrief)() Implements IGenomicsContextProvider(Of GeneBrief).GetRelatedGenes

            Dim relates As Relationship(Of GeneBrief)() =
                _contextModel.GetAroundRelated(loci, Not unstrand, ATGDist)
            Return relates
        End Function

        Public Function Save(path As String, Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
            Return Save(path, encoding.CodePage)
        End Function
    End Class
End Namespace
