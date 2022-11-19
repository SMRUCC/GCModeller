#Region "Microsoft.VisualBasic::b2aff7f4e3113cec45b52fa6d194f2d0, GCModeller\core\Bio.Assembly\Assembly\MetaCyc\File\AttributeValue DataFile\DataFiles\Slots\Promoter.vb"

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

    '   Total Lines: 122
    '    Code Lines: 33
    ' Comment Lines: 79
    '   Blank Lines: 10
    '     File Size: 5.62 KB


    '     Class Promoter
    ' 
    '         Properties: AbsolutePlus1Pos, BindsSigmaFactor, ComponentOf, Direction, Minus10Left
    '                     Minus10Right, Minus35Left, Minus35Right, Table
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Reflection
Imports SMRUCC.genomics.Assembly.MetaCyc.Schema.Reflection
Imports Microsoft.VisualBasic

Namespace Assembly.MetaCyc.File.DataFiles.Slots

    ''' <summary>
    ''' Frames in this class define transcription start sites.
    ''' (本对象定义了一个转录起始位点)
    ''' </summary>
    ''' <remarks>
    ''' 启动子（promoter）是基因的一个组成部分，在遗传学中是指一段能使基因进行转录的脱氧核糖核酸（DNA）序列。
    ''' 启动子可以被RNA聚合酶辨认，并开始转录。在核糖核酸（RNA）合成中，启动子可以和决定转录的开始的转录因子
    ''' 产成相互作用，控制基因表达（转录）的起始时间和表达的程度，包含核心启动子区域和调控区域,就像“开关”，
    ''' 决定基因的活动，继而控制细胞开始生产哪一种蛋白质。
    ''' 启动子本身并无编译功能，但它拥有对基因翻译氨基酸的指挥作用，就像一面旗帜，其核心部分是非编码区上游的
    ''' RNA聚合酶结合位点，指挥聚合酶的合成，这种酶指导RNA的复制合成。因此该段位的启动子发生突变（变异），
    ''' 将对基因的表达有着毁灭性作用。
    ''' </remarks>
    Public Class Promoter : Inherits [Object]

        ''' <summary>
        ''' The absolute base pair position of the transcription start site on the DNA strand.
        ''' (本转录起始位点在DNA链上面的碱基位置)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField(name:="ABSOLUTE-PLUS-1-POS")> Public Property AbsolutePlus1Pos As String

        ''' <summary>
        ''' This slot links to the one or more sigma factors that can bind to a promoter, thereby
        ''' initiating transcription.
        ''' (本属性链接至1至多个可以与本启动子相结合的Sigma因子，然后启动转录过程)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExternalKey("proteins,protligandcplxes", "", ExternalKey.Directions.Out)> <MetaCycField()> Public Property BindsSigmaFactor As String

        ''' <summary>
        ''' This slot links to the transcription-unit(s) to which the promoter belongs.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExternalKey("transunit", "", ExternalKey.Directions.In)> <MetaCycField(type:=MetaCycField.Types.TStr)> Public Property ComponentOf As List(Of String)

        ''' <summary>
        ''' These slots list chromosomal coordinates of the left and right ends of the -35 and -10 boxes
        ''' associated with the promoter.
        ''' (本属性列举了-35和-10区的位置)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField(name:="MINUS-35-LEFT")> Public Property Minus35Left As String

        ''' <summary>
        ''' These slots list chromosomal coordinates of the left and right ends of the -35 and -10 boxes
        ''' associated with the promoter.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField(name:="MINUS-35-RIGHT")> Public Property Minus35Right As String
        ''' <summary>
        ''' These slots list chromosomal coordinates of the left and right ends of the -35 and -10 boxes
        ''' associated with the promoter.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField(name:="MINUS-10-LEFT")> Public Property Minus10Left As String
        ''' <summary>
        ''' These slots list chromosomal coordinates of the left and right ends of the -35 and -10 boxes
        ''' associated with the promoter.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField(name:="MINUS-10-RIGHT")> Public Property Minus10Right As String

        ''' <summary>
        ''' -1 这个启动子序列是位于互补链的;
        ''' 0 无法判断;
        ''' 1 这个启动子序列是位于正链的.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>
        ''' 左端小于右端，在正链，反之在互补链
        ''' </remarks>
        Public ReadOnly Property Direction As Integer
            Get
                Dim _10 = Val(Minus10Left), TSS As Long = Val(AbsolutePlus1Pos)
                If TSS = 0 Then
                    Return 0
                ElseIf TSS > _10 AndAlso _10 > 0 Then
                    Return 1
                ElseIf TSS < _10 Then
                    Return -1
                Else
                    Return 0
                End If
            End Get
        End Property

        Public Overrides ReadOnly Property Table As [Object].Tables
            Get
                Return Tables.promoters
            End Get
        End Property

        'Public Shared Shadows Widening Operator CType(e As MetaCyc.File.AttributeValue.Object) As Promoter
        '    Dim Promoter As Promoter = New Promoter
        '    Call MetaCyc.File.DataFiles.Slots.[Object].TypeCast(Of Promoter) _
        '        (MetaCyc.File.AttributeValue.Object.Format(Promoters.AttributeList, e), Promoter)
        '    Return Promoter
        'End Operator
    End Class
End Namespace
