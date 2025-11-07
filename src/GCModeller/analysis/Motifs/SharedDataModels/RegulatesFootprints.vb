#Region "Microsoft.VisualBasic::0c05b1b4211b3404531200cef011b959, analysis\Motifs\SharedDataModels\RegulatesFootprints.vb"

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

'     Class RegulatesFootprints
' 
'         Properties: [Class], Category, DoorId, InitX, InteractionType
'                     MotifFamily, MotifTrace, ORF, Pcc, Regulator
'                     RegulatorTrace, sPcc, StructGenes, tag, Type
'                     WGCNA
' 
'         Function: __posUid, __posUidNonStrict, Equals, ToString, TraceUid
'                   TraceUidStrict
'         Operators: <>, =
' 
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.Framework.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Data.GraphTheory.Network
Imports Microsoft.VisualBasic.Data.GraphTheory.SparseGraph
Imports SMRUCC.genomics.ComponentModel.Loci.Abstract

Namespace DocumentFormat

    ''' <summary>
    ''' 简单描述调控位点和调控因子之间的关系以及该位点在基因组上面的位置
    ''' </summary>
    Public Class RegulatesFootprints : Inherits VirtualFootprints
        Implements IInteraction
        Implements ILocationComponent
        Implements INetworkEdge

#Region "Public Properties & Fields"

        ''' <summary>
        ''' 目标基因所在的操纵子对象的Door数据库之中的编号
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property DoorId As String
        ''' <summary>
        ''' 当前的这个基因是否是其所处的操纵子的第一个基因
        ''' </summary>
        ''' <returns></returns>
        <Column("init.Operon?")> Public Property InitX As Char
        ''' <summary>
        ''' 操纵子里面的结构基因，只有当<see cref="InitX"/>为真的时候这个属性值才不为空
        ''' </summary>
        ''' <returns></returns>
        <Collection("Operon")> Public Property StructGenes As String()

        <Column("ORF ID")> Public Overrides Property ORF As String Implements IInteraction.target
            Get
                Return MyBase.ORF
            End Get
            Set(value As String)
                MyBase.ORF = value
            End Set
        End Property

        Public Property MotifFamily As String

        ''' <summary>
        ''' 这个基因对象所被预测的调控因子
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Regulator As String Implements IInteraction.source
        ''' <summary>
        ''' 所预测的调控因子对目标基因的调控作用的权重的大小，这里的元素的顺序是和<see cref="Regulator"></see>之中的顺序是一一对应的
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Pcc As Double Implements INetworkEdge.value
        Public Property sPcc As Double
        Public Property WGCNA As Double

        ''' <summary>
        ''' A
        ''' </summary>
        ''' <returns></returns>
        Public Property Type As String

        ''' <summary>
        ''' B 目标基因对象的KEGG的代谢途径分类
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property [Class] As String
        ''' <summary>
        ''' C. 目标基因对象的KEGG的代谢途径分类
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Category As String

        Private Property InteractionType As String Implements INetworkEdge.Interaction
            Get
                If InitX.ParseBoolean Then
                    Return $"[Operon] TF Regulation"
                Else
                    Return "TF Regulation"
                End If
            End Get
            Set(value As String)
                ' NULL
            End Set
        End Property

        ''' <summary>
        ''' Trace.Regulator.(调控因子的匹配来源)
        ''' </summary>
        ''' <returns></returns>
        <Column("Trace.Regulator")> Public Overridable Property RegulatorTrace As String
        ''' <summary>
        ''' Trace.Site.(Motif的匹配来源)
        ''' </summary>
        ''' <returns></returns>
        <Column("Trace.Site")> Public Overridable Property MotifTrace As String
        ''' <summary>
        ''' 自定义的一个标签数据
        ''' </summary>
        ''' <returns></returns>
        Public Property tag As String
#End Region

        Public Overrides Function ToString() As String
            Return String.Format("({0},{1})  {2}:  {3}", Starts, Ends, MotifFamily, Sequence)
        End Function

        Public Overrides Function Equals(obj As Object) As Boolean
            If Not TypeOf obj Is RegulatesFootprints Then
                Return False
            Else
                Return IEqualsAPI.Equals(Me, DirectCast(obj, RegulatesFootprints))
            End If
        End Function

        Public Overloads Shared Operator =(a As RegulatesFootprints, b As RegulatesFootprints) As Boolean
            Return a.Equals(b)
        End Operator

        Public Overloads Shared Operator <>(a As RegulatesFootprints, b As RegulatesFootprints) As Boolean
            Return Not a.Equals(b)
        End Operator

        ''' <summary>
        ''' 非严格的
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        Public Shared Function TraceUid(x As RegulatesFootprints) As String
            Return $"{x.ORF}-{x.Regulator}-{x.MotifFamily}-{ __posUidNonStrict(x.Starts, x.Ends)}"
        End Function

        ''' <summary>
        ''' 严格的
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        Public Shared Function TraceUidStrict(x As RegulatesFootprints) As String
            Return $"{x.ORF}-{x.Regulator}-{x.MotifFamily}-{__posUid(x.Starts, x.Ends)}"
        End Function

        Private Shared Function __posUid(x As Integer, y As Integer) As String
            Dim loci As Integer() = {x, y}
            Dim xx As String = CStr(loci.Min)
            Dim yy As String = CStr(loci.Max)

            If xx.Length > 1 Then
                xx = Mid(xx, 1, xx.Length - 1)
            Else
                xx = ""
            End If

            If yy.Length > 1 Then
                yy = Mid(yy, 1, yy.Length - 1)
            Else
                yy = ""
            End If

            Return xx & "-" & yy
        End Function

        Private Shared Function __posUidNonStrict(x As Integer, y As Integer) As String
            Dim loci As Integer() = {x, y}
            x = loci.Min
            y = loci.Max
            Dim d As Integer = y - x
            d /= 2
            Dim s As String = x + d
            Return Mid(s, 1, s.Length - 2)
        End Function
    End Class
End Namespace
