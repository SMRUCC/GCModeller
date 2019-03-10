#Region "Microsoft.VisualBasic::a10a59aca90c083ec29c0c01e966ca3a, analysis\SyntenySignature\bacterials-fingerprint\Regulation\GeneObject.vb"

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

    '     Class GeneObject
    ' 
    '         Properties: COG, GeneID, KO, Regulations, TFFamily
    '                     TFMotifs, TSS, TTS
    ' 
    '         Function: KOGenerateSequence, TFGenerateSequence, ToString
    ' 
    '         Sub: __buildRegulations
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic

Namespace RegulationSignature

    Public Class GeneObject

        ''' <summary>
        ''' 放在ATG的前面
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property GeneID As GeneID

#Region "基因蛋白质序列内容特征"

        Public Property COG As String
        ''' <summary>
        ''' KEGG Pathway的EntryID列表
        ''' </summary>
        ''' <returns></returns>
        Public Property KO As List(Of String)
        ''' <summary>
        ''' 受本调控因子所调控的下游基因的基因号列表，这个属性不是直接被序列化的，而是首先在字典之中根据这个值查找出<see cref="GeneID"></see>然后在附加<see cref="GeneID"></see>的序列化的值
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Regulations As String()

        ''' <summary>
        ''' 假若这个基因还是调控因子的话，则这个属性值不会为空并且<see cref="Regulations"></see>也不为空
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property TFFamily As String
#End Region

#Region "调控元件"

        ''' <summary>
        ''' {MotifFamilyName, ATG relative Distance}
        ''' </summary>
        ''' <returns></returns>
        Public Property TFMotifs As KeyValuePair(Of String, Integer)()
        Public Property TSS As Integer()
        Public Property TTS As Integer()

#End Region

        ''' <summary>
        ''' 调试的时候使用这个接口进行可视化
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function ToString() As String
            Return String.Format("[{0}] {1}  {2}", GeneID.GeneTagID, GeneID.GeneName, COG)
        End Function

        ''' <summary>
        ''' 使用这个属性获取得到特征序列
        ''' </summary>
        ''' <returns></returns>
        ''' <param name="Builder">应用于调控因子上面</param>
        ''' <remarks></remarks>
        Public Overloads Function TFGenerateSequence(Builder As SignatureBuilder) As String
            Dim sBuilder As StringBuilder = New StringBuilder

#Region "首先生成调控因子自身的标识符以及摘要信息数据"

            '  Call sBuilder.Append(Seperator)
            Call sBuilder.Append(GeneID.ToString)
            '   Call sBuilder.Append(Seperator)
            Call sBuilder.Append(GenerateCode(COG))
            '  Call sBuilder.Append(Seperator)
            Call sBuilder.Append(GenerateCode(String.Join("+", KO.ToArray)))
            ' Call sBuilder.Append(Seperator)
            Call sBuilder.Append(GenerateCode(TFFamily))
            '  Call sBuilder.Append(Seperator)
#End Region

            If Regulations.IsNullOrEmpty Then
                '非调控因子
                Call sBuilder.Append(GenerateCode("-"))
            Else
                Call __buildRegulations(Builder, sBuilder)
            End If

            '     Call sBuilder.Append(Seperator)

            Return sBuilder.ToString
        End Function

        ''' <summary>
        ''' 构建调控缩影
        ''' </summary>
        ''' <param name="Builder">分别抽取三段数据构建调控缩影</param>
        ''' <param name="sBuilder"></param>
        ''' <remarks>由于<paramref name="builder"/>里面的字典中的各个元素都是已经排好序的，所以在这里不可以再使用并行拓展，以防止破环排好序的顺序</remarks>
        Private Sub __buildRegulations(Builder As SignatureBuilder, ByRef sBuilder As StringBuilder)

            '调控因子
            Dim Genes = (From Gene In Builder.TFHash.Values Where Array.IndexOf(Regulations, Gene.GeneID.GeneTagID) > -1 Select Gene).ToArray
            Dim str As String = String.Join("", (From Gene In Genes Select s_ID = Gene.GeneID.ToString).ToArray)
            Call sBuilder.Append(str)

            '无功能注释
            Genes = (From Gene In Builder.OtherHash.Values Where Array.IndexOf(Regulations, Gene.GeneID.GeneTagID) > -1 Select Gene).ToArray
            str = String.Join("", (From Gene In Genes Select s_ID = Gene.GeneID.ToString).ToArray)
            Call sBuilder.Append(str)

            'KEGG Pathways
            Genes = (From Gene In Builder.KOHash.Values Where Array.IndexOf(Regulations, Gene.GeneID.GeneTagID) > -1 Select Gene).ToArray
            str = String.Join("", (From Gene In Genes Select s_ID = Gene.GeneID.ToString).ToArray)
            Call sBuilder.Append(str)

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="KO_ID">对于OtherHash，这个参数值使用-空值</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function KOGenerateSequence(KO_ID As String) As String
            Dim sBuilder As StringBuilder = New StringBuilder
            '  Call sBuilder.Append(Seperator)
            Call sBuilder.Append(GeneID.ToString)
            '   Call sBuilder.Append(Seperator)
            Call sBuilder.Append(GenerateCode(COG))
            '  Call sBuilder.Append(Seperator)
            Call sBuilder.Append(GenerateCode(KO_ID))
            ' Call sBuilder.Append(Seperator)
            '  Call sBuilder.Append(Seperator)

            Return sBuilder.ToString
        End Function

        '   Const Seperator As String = "AAAGTAAA"

    End Class

End Namespace
