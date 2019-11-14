#Region "Microsoft.VisualBasic::18ec3d031eba9dd51111a8d4dc6f33fc, analysis\SyntenySignature\bacterials-fingerprint\Regulation\SignatureBuilder.vb"

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

    '     Class SignatureBuilder
    ' 
    '         Properties: Title
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: GetSequenceData, ToString, VF2TFreg
    ' 
    '         Sub: __orderGenome
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints
Imports SMRUCC.genomics.SequenceModel

Namespace RegulationSignature

    ''' <summary>
    ''' 本模块将Regprecise注释结果上面的每一个位点数据转换为一段简并的DNA序列数据用来抽象整个基因组的调控网络特征
    ''' 由于位点和下游基因构成了一条边，所以可以从整个Regprecise注释数据之中得到一个网络
    ''' 由于本全基因组调控特征可以用于表示整个基因组的调控网络，所以使用本方法将原有的三维的网络数据降维至二维DNA序列，可以很方便的使用blastn程序进行调控网络的相似度的比对工作
    ''' </summary>
    ''' <remarks>
    ''' 采用冗余的形式来构建序列特征，这样通过blastn就可以同时比对调控网络和代谢网络，将两个三维网络降维至两个二维序列所形成的网络来进行相互比较
    ''' </remarks>
    Public Class SignatureBuilder : Inherits ISequenceBuilder

#Region "三大 特征区域"

        Protected Friend ReadOnly TFHash As Dictionary(Of String, GeneObject) = New Dictionary(Of String, GeneObject)
        Protected Friend ReadOnly KOHash As Dictionary(Of String, GeneObject) = New Dictionary(Of String, GeneObject)
        Protected Friend OtherHash As Dictionary(Of String, GeneObject)
#End Region

        Public Property Title As String

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="VirtualFootprints">调控特征</param>
        ''' <param name="PTT">全基因组</param>
        ''' <param name="KEGG_Pathways">代谢网络信息</param>
        ''' <remarks></remarks>
        Sub New(VirtualFootprints As IEnumerable(Of PredictedRegulationFootprint),
                PTT As PTT,
                KEGG_Pathways As IEnumerable(Of bGetObject.Pathway),
                COG As IEnumerable(Of IFeatureDigest))

            Dim COGHash = COG.ToDictionary(Function(Gene) Gene.Key)
            Dim GenomeHash As Dictionary(Of String, GeneObject) =
                PTT.GeneObjects.ToDictionary(Function(Gene) Gene.Synonym,
                                             Function(Gene)
                                                 Return New GeneObject With {
                                                .COG = If(COGHash.ContainsKey(Gene.Synonym), COGHash(Gene.Synonym).Feature, "-"),
                                                .KO = New List(Of String),
                                                .GeneID = New GeneID With {
                                                    .GeneTagID = Gene.Synonym,
                                                    .GeneName = Gene.Gene,
                                                    .ClassType = GeneID.ClassTypes.Hypothetical
                                                 }
                                             }
                                             End Function)
            For Each GeneEntry In GenomeHash
                If String.IsNullOrEmpty(GeneEntry.Value.COG) Then
                    GeneEntry.Value.COG = "-"
                End If
                If String.IsNullOrEmpty(GeneEntry.Value.GeneID.GeneName) OrElse String.Equals(GeneEntry.Value.GeneID.GeneName, "-") Then
                    GeneEntry.Value.GeneID.GeneName = GeneEntry.Value.COG
                End If
            Next

            Dim GenomeHashShaodows = GenomeHash.ToDictionary(Function(obj) obj.Key, elementSelector:=Function(obj) obj.Value)

            For Each TF In VF2TFreg(VirtualFootprints)
                Dim Gene = GenomeHashShaodows(TF.Key)
                Gene.Regulations = TF.Value.Value
                Gene.GeneID.ClassType = GeneID.ClassTypes.TF
                Gene.TFFamily = TF.Value.Key

                If String.Equals(Gene.GeneID.GeneName, "-") OrElse String.IsNullOrEmpty(Gene.GeneID.GeneName) Then
                    Gene.GeneID.GeneName = Gene.TFFamily
                    Gene.COG = Gene.GeneID.GeneName
                End If

                Call TFHash.Add(TF.Key, Gene)
                Call GenomeHashShaodows.Remove(TF.Key)
            Next

            For Each Pathway In KEGG_Pathways

                If Pathway.genes.IsNullOrEmpty Then
                    Continue For
                End If

                For Each gene As NamedValue In Pathway.genes
                    Dim GeneObject As GeneObject

                    If Not GenomeHashShaodows.ContainsKey(gene.name) Then
                        If TFHash.ContainsKey(gene.name) Then
                            GeneObject = TFHash(gene.name)
                            GeneObject.GeneID.ClassType = GeneID.ClassTypes.Hybrids
                        Else
                            Call Console.WriteLine("Unable to found the information of gene " & gene.name)
                            Continue For
                        End If
                    Else
                        GeneObject = GenomeHashShaodows(gene.name)
                        GeneObject.GeneID.ClassType = GeneID.ClassTypes.KO
                    End If

                    If GeneObject Is Nothing Then
                        Continue For
                    End If

                    Call GeneObject.KO.Add(Pathway.EntryId)
                    If Not KOHash.ContainsKey(gene.name) Then Call KOHash.Add(gene.name, GeneObject)
                    Call GenomeHashShaodows.Remove(gene.name)
                Next
            Next

            For Each gene As GeneObject In KOHash.Values
                gene.KO = (From KO_ID As String In gene.KO Select KO_ID Distinct Order By KO_ID Ascending).AsList
                If String.IsNullOrEmpty(gene.GeneID.GeneName) OrElse String.Equals(gene.GeneID.GeneName, "-") Then
                    gene.GeneID.GeneName = gene.KO.Last
                    gene.COG = gene.GeneID.GeneName
                End If
            Next

            Title = PTT.Title
            OtherHash = GenomeHashShaodows

            For Each gene As GeneObject In GenomeHash.Values
                If String.IsNullOrEmpty(gene.TFFamily) Then
                    gene.TFFamily = "-"
                End If
            Next

            '修剪KO编号，去掉前缀物种简写编号，使该编号可以在不同的基因组之间编码一致

            For Each gene As GeneObject In GenomeHash.Values
                If gene.KO.IsNullOrEmpty Then
                    Continue For
                End If

                Dim LQuery = (From KO_ID As String
                                  In gene.KO
                              Let ClassID As String = Regex.Match(KO_ID, "\d+").Value
                              Where Not String.IsNullOrEmpty(ClassID)
                              Select ClassID).ToArray
                gene.KO = LQuery.AsList
            Next

        End Sub

        Private Shared Function VF2TFreg(VirtualFootprints As Generic.IEnumerable(Of PredictedRegulationFootprint)) As Dictionary(Of String, KeyValuePair(Of String, String()))

            'Dim LQuery = (From vf As SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints.PredictedRegulationFootprint
            '                  In VirtualFootprints
            '              Select GeneID = vf.ORF, TFs = vf.Regulators, vf.MotifFamily).ToArray
            'Dim TFreg = (From orf In LQuery
            '             Select (From TF In orf.TFs
            '                     Select TF, orf.GeneID, orf.MotifFamily).ToArray).ToArray.MatrixToList
            'Dim GroupTF = (From tf In TFreg
            '               Select tf
            '               Group tf By tf.TF Into Group).ToArray
            'Return GroupTF.ToDictionary(Function(obj) obj.TF,
            '                            elementSelector:=Function(obj) New KeyValuePair(Of String, String())(
            '                                                                       (From TFGroup In (From Gene In obj.Group
            '                                                                                         Select Gene.MotifFamily
            '                                                                                         Group MotifFamily By MotifFamily Into Group).ToArray
            '                                                                        Select TFGroup
            '                                                                        Order By TFGroup.Group.Count Descending).First.MotifFamily,
            '                                                                       (From Gene In obj.Group Select Gene.GeneID).ToArray.Distinct.ToArray)) '所调控的基因会被去重复
        End Function

        ''' <summary>
        ''' 从这里得到简并的调控网络特征序列
        ''' 简并序列的特征，总体功能分区为三个部分：调控区，未知功能区域，代谢网络区域
        ''' 在调控网络之中的每一个节点之中又划分为上面的下游基因的三个功能区域：调控区，未知功能区域，代谢网络区域
        ''' 按照这三个功能区域的划分是由于两个基因组特征比较必须要有相同的位置元素
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' 特征编码的规则如下：
        ''' 特征编码得到的序列主要由3部分构成：
        ''' 第一大部分为调控因子
        ''' 第二大部分为没有功能被注释出来的基因
        ''' 第三大部分为KEGG Pathway的注释结果
        ''' </remarks>
        Public Overrides Function ToString() As String

            Call __orderGenome()
            '在调控关系部分的构建过程之中，假若为了压缩数据，采用指针的话，则势必会在不同基因组之间进行相互比较的时候产生差异，所以这里直接将原来的基因号转换为这里的基因统一编号

            Dim sBuilder As StringBuilder = New StringBuilder
            Call sBuilder.Append(String.Join("", (From Gene In TFHash Select Gene.Value.TFGenerateSequence(Me)).ToArray))
            '  Call sBuilder.Append(Seperator)
            Call sBuilder.Append(String.Join("", (From Gene In OtherHash Select Gene.Value.KOGenerateSequence("-")).ToArray))
            '   Call sBuilder.Append(Seperator)
            Call sBuilder.Append(String.Join("", (From Gene In KOOrders Select Gene.Value.KOGenerateSequence(Gene.Key)).ToArray))

            Return sBuilder.ToString
        End Function

        Dim KOOrders As List(Of KeyValuePair(Of String, GeneObject)) = New List(Of KeyValuePair(Of String, GeneObject))

        ''' <summary>
        ''' 对基因组之中原件进行重新排序处理，两个基因组之间的对象应该尽量按照共有的特征进行排序
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub __orderGenome()

            Dim TFList = (From Group In (From Tf In TFHash.Values.ToArray
                                         Select TfTag = Tf.TFFamily.ToLower, Tf
                                         Group By TfTag Into Group).ToArray
                          Select Group.TfTag, COG_Orders = (From Tf In Group.Group
                                                            Select Tf
                                                            Order By Tf.Tf.COG Ascending).ToArray).ToArray '先按照家族排序，在每一个家族之中再按照COG排序，这样子使两个基因组之间的TF的逻辑位置之上顺序尽量保持相互一致
            Call TFHash.Clear()
            For Each TFFamily In TFList
                For Each TF In TFFamily.COG_Orders
                    Call TFHash.Add(TF.Tf.GeneID.GeneTagID, TF.Tf)
                Next
            Next


            ' 排序KEGG Pathway部分的数据
            Dim DisAssembly = (From KO_Gene In (From Gene In KOHash Select (From KO_ID As String
                                                                            In Gene.Value.KO
                                                                            Select KO_ID, GeneObject = Gene).ToArray).Unlist
                               Select KO_Gene
                               Group KO_Gene By KO_Gene.KO_ID Into Group).ToArray

            For Each KO_Gene In DisAssembly
                Dim OrderByName = (From Gene In KO_Gene.Group Select Gene Order By Gene.GeneObject.Value.GeneID.GeneName Ascending).ToArray

                For Each g In OrderByName
                    Dim ID As String = g.KO_ID & g.GeneObject.Value.GeneID.GeneName
                    Call KOOrders.Add(New KeyValuePair(Of String, GeneObject)(ID, g.GeneObject.Value))
                Next
            Next


            '未知功能的区域使用GeneName排序
            Dim GeneList = (From Gene In OtherHash.Values Select Gene Order By Gene.GeneID.GeneName Ascending).ToArray
            OtherHash = GeneList.ToDictionary(Function(Gene) Gene.GeneID.GeneTagID)
        End Sub

        '  Const Seperator As String = "CCCTTCCC"

        Public Overrides Function GetSequenceData() As String
            Return ToString()
        End Function
    End Class
End Namespace
