#Region "Microsoft.VisualBasic::19051ee7ecae52b8c0e4dc3f68709705, DataMySql\CEG\GeneCluster.vb"

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

    '     Class GeneCluster
    ' 
    '         Properties: CEGId, GeneClusters
    ' 
    '         Function: GenerateDatabase, ToString
    ' 
    '     Class CEGAssembly
    ' 
    '         Properties: GeneClusters, Updates
    ' 
    '         Function: InstallDatabase
    ' 
    '     Class ClusterGene
    ' 
    '         Properties: Annotation
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Namespace CEG

    Public Class GeneCluster

        <XmlAttribute> Public Property CEGId As String
        <XmlElement("ClusterGene")> Public Property GeneClusters As ClusterGene()
            Get
                Return __geneClusters.Values.ToArray
            End Get
            Set(value As ClusterGene())
                If Not value.IsNullOrEmpty Then _
                    __geneClusters = value.ToDictionary(Function(ClusterGene) ClusterGene.GId)
            End Set
        End Property

        Dim __geneClusters As Dictionary(Of String, ClusterGene)

        Default Public ReadOnly Property ClusterGene(GId As String) As ClusterGene
            Get
                Dim GeneObject As ClusterGene = Nothing
                Call __geneClusters.TryGetValue(GId, GeneObject)
                Return GeneObject
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return CEGId
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Db">CEG数据库的数据文件夹</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GenerateDatabase(Db As String) As GeneCluster()
            Dim CEGCore As CEG.ClusterGene() = (From Gr In (From ClusterGene As CEG.ClusterGene In (Db & "/ceg_core.csv").LoadCsv(Of CEG.ClusterGene)(False) Where Not String.Equals(ClusterGene.GId, "-") Select ClusterGene Group ClusterGene By ClusterGene.GId Into Group).ToArray Select Gr.Group.First).ToArray
            Dim AnnoSrc = (From anno As CEG.Annotation In CEG.Annotation.LoadDocument(Path:=Db & "/annotation.csv") Where Not String.Equals(anno.GId, "-") Select anno Group anno By anno.GId Into Group).ToArray
            Dim Annotations As Dictionary(Of String, CEG.Annotation) = AnnoSrc.ToDictionary(Function(GeneObject) GeneObject.GId, elementSelector:=Function(GeneObject) GeneObject.Group.First)
            Dim Groups = From GeneObject As ClusterGene
                         In CEGCore
                         Select GeneObject
                         Group By GeneObject.AccessNum Into Group
            Dim ClusterBase As GeneCluster() =
                LinqAPI.Exec(Of GeneCluster) <= From Cluster
                                                In Groups
                                                Let m = Cluster.Group.ToArray
                                                Select New GeneCluster With {
                                                    .CEGId = Cluster.AccessNum,
                                                    .GeneClusters = m
                                                }
            Dim setValue = New SetValue(Of ClusterGene) <= NameOf(CEG.ClusterGene.Annotation)
            Dim ApplyingAnnotationLQuery As ClusterGene() =
                LinqAPI.Exec(Of ClusterGene) <= From gene As ClusterGene
                                                In CEGCore
                                                Let Annotation As CEG.Annotation = Annotations(gene.GId)
                                                Select setValue(gene, Annotation)
            Return ClusterBase
        End Function
    End Class

    <XmlType("CEG.Assembly", Namespace:="http://code.google.com/p/genome-in-code/cefg.uestc.edu.cn/ceg/")>
    Public Class CEGAssembly

        ''' <summary>
        ''' 本数据库文件最后所更新的时间
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property Updates As Date
        ''' <summary>
        ''' 基因簇数据
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlElement> Public Property GeneClusters As GeneCluster()

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="CEG">从网络上所下载的CEG数据库文件夹</param>
        ''' <param name="InstallLoc">数据库Xml文件所安装的文件位置</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function InstallDatabase(CEG As String, InstallLoc As String) As Boolean
            Dim Assembly = GeneCluster.GenerateDatabase(CEG)
            Dim CEGAsm As CEGAssembly = New CEGAssembly With {.Updates = Now, .GeneClusters = Assembly}
            Return CEGAsm.GetXml.SaveTo(InstallLoc)
        End Function
    End Class

    Public Class ClusterGene : Inherits CEG.Core
        Public Property Annotation As CEG.Annotation

        Public Overrides Function ToString() As String
            Return Me.GId & "  --> " & Annotation.ToString
        End Function
    End Class
End Namespace
