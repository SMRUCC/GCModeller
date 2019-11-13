#Region "Microsoft.VisualBasic::065194760bcd962dec4849b4dc7b28bf, engine\IO\GCTabular\CsvTabularData\MotifSite.vb"

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

    '     Class MotifSite
    ' 
    '         Properties: Internal_GUID, MotifName, ORF, Position, Regulators
    '                     TU_MODEL_GUID
    ' 
    '         Function: ToString
    ' 
    '     Class Regulator
    ' 
    '         Properties: Effectors, Pcc, ProteinId, RegulatesMotif, TCS_RR
    ' 
    '         Function: get_PCs, get_TargetGeneId, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports SMRUCC.genomics.Assembly

Namespace FileStream

    ''' <summary>
    ''' DNA链或者mRNA链上面的一个调控位点
    ''' </summary>
    ''' <remarks></remarks>
    Public Class MotifSite : Implements INamedValue, IReadOnlyId

        Public Property Regulators As List(Of String)
        Public Property MotifName As String Implements INamedValue.Key
        ''' <summary>
        ''' 与所处的ORF上面的ATG为标准的
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Position As Integer
        Public Property ORF As String

        Public Property TU_MODEL_GUID As String

        ''' <summary>
        ''' 请使用本属性来唯一标识Motif对象
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Internal_GUID As String Implements IReadOnlyId.Identity
            Get
                Return String.Format("{0}-{1}-{2}", ORF, MotifName, Position)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("{0} -> {1}", String.Join("; ", Regulators), MotifName)
        End Function
    End Class

    Public Class Regulator : Implements INamedValue

        ''' <summary>
        ''' <see cref="TranscriptUnit">目标转录单元对象</see>的<see cref="MotifSite.Internal_GUID"></see>
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property RegulatesMotif As String
        ''' <summary>
        ''' The unique identifier of the target regulator object.(目标调控因子对象的唯一标识符，基因号)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ProteinId As String Implements INamedValue.Key
        Public Property Pcc As Double
        ''' <summary>
        ''' 请使用本属性来判断是否为蛋白质复合物，为<see cref="Metabolite.Identifier"></see>属性值
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <CollectionAttribute("Effectors")> Public Property Effectors As String()
        <Column("TCS_RR?")> Public Property TCS_RR As Boolean

        ''' <summary>
        ''' 获取被本调控因子所调控的目标基因
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function get_TargetGeneId() As String
            Return Me.RegulatesMotif.Split(CChar("-")).First
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("{0}  ----> {1}", ProteinId, RegulatesMotif)
        End Function

        ' ''' <summary>
        ' ''' 在OCS部分的构建有活性的调控因子的时候，由于效应物可能会有多种，故而需要本方法展开Effector.在进行<see cref="ProteinId"></see>的替换之后，将原有的数据进行移除
        ' ''' </summary>
        ' ''' <returns></returns>
        ' ''' <remarks></remarks>
        'Public Function Clone() As Regulator
        '    Return DirectCast(Me.MemberwiseClone, Regulator)
        'End Function

        ''' <summary>
        ''' 假若本调控因子有效应物的话，则返回一个列表，否则为空，{PC, {ProteinId, Effector}}
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function get_PCs() As KeyValuePair(Of String, String())()
            If Effectors.IsNullOrEmpty Then
                Return Nothing
            Else
                Return (From s As String In Effectors Let PC As String = String.Format("[{0}][{1}]", Me.ProteinId, s) Select New KeyValuePair(Of String, String())(PC, {ProteinId, s})).ToArray
            End If
        End Function
    End Class
End Namespace
