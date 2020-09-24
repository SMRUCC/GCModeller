#Region "Microsoft.VisualBasic::aca5117bf437d85aa8a0fefb06e51e05, engine\GCModeller\EngineSystem\ObjectModels\Feature\TransUnit.vb"

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

    '     Class TransUnit
    ' 
    '         Properties: ExpressionActivity, Identifier, MotifSites, ProductHandlers, Products
    '                     TypeId
    ' 
    '         Function: CreateObject, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.GCModeller.Assembly

Namespace EngineSystem.ObjectModels.Feature

    ''' <summary>
    ''' 转录单元为DNA分子上面的一个功能位点，信息存储组织功能
    ''' </summary>
    ''' <remarks></remarks>
    Public Class TransUnit : Inherits EngineSystem.ObjectModels.Feature.BiomacromoleculeFeature
        Implements ObjectModels.Feature.BiomacromoleculeFeature.ITemplate

        ''' <summary>
        ''' 指向代谢组中的代谢物的句柄值集合{geneID, productID}
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlElement> Public Property ProductHandlers As KeyValuePair(Of String, String)()
        <DumpNode> Public Property Products As String Implements Feature.BiomacromoleculeFeature.ITemplate.Products
        <DumpNode> <XmlAttribute> Public Property ExpressionActivity As Double Implements BiomacromoleculeFeature.ITemplate.Quantity
        <DumpNode> <XmlAttribute> Public Overrides Property Identifier As String Implements BiomacromoleculeFeature.ITemplate.locusId

        ''' <summary>
        ''' 调控因子对转录单元的调控作用是从这里开始的
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property MotifSites As SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Feature.MotifSite(Of ObjectModels.Module.CentralDogmaInstance.Transcription)()

        Protected Friend FeatureBaseType As GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME.TranscriptUnit

        Public Overrides Function ToString() As String
            If Not ProductHandlers.IsNullOrEmpty Then
                Dim sBuilder As StringBuilder = New StringBuilder(1024)

                For Each Gene In ProductHandlers
                    sBuilder.AppendFormat("{0}(p: {1}), ", Gene.Key, Gene.Value)
                Next
                Call sBuilder.Remove(sBuilder.Length - 2, 2)

                Return String.Format("({0}){1} [{2}]", Identifier, FeatureBaseType.Name, sBuilder.ToString)
            Else
                Return String.Format("({0}){1}", Identifier, FeatureBaseType.Name)
            End If
        End Function

        ''' <summary>
        ''' 这个方法已经按照操纵子进行展开了
        ''' </summary>
        ''' <param name="TranscriptUnitModelObject"></param>
        ''' <param name="CellSystem"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function CreateObject(TranscriptUnitModelObject As GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME.TranscriptUnit,
                                            CellSystem As EngineSystem.ObjectModels.SubSystem.CellSystem) As TransUnit

            Dim TransUnit As TransUnit = New TransUnit With {.FeatureBaseType = TranscriptUnitModelObject, .Identifier = TranscriptUnitModelObject.Identifier}

            If TranscriptUnitModelObject.GeneCluster.IsNullOrEmpty Then
                Call CellSystem.SystemLogging.WriteLine(String.Format("[EXCEPTION] null gene object list in the transunit, ""{0}"", ignored this object!", TransUnit.Identifier))
                TransUnit.ProductHandlers = New KeyValuePair(Of String, String)() {}
            Else
                TransUnit.ProductHandlers = (From Handle As KeyValuePair
                                             In TranscriptUnitModelObject.GeneCluster
                                             Let Gene As GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME.GeneObject =
                                                        CellSystem.DataModel.BacteriaGenome.Genes.GetItem(Handle.Key)
                                             Let ProductValue = Gene.TranscriptProduct
                                             Select New KeyValuePair(Of String, String)(Gene.Identifier, ProductValue)).ToArray
            End If

            Return TransUnit
        End Function

        Public Overrides ReadOnly Property TypeId As ObjectModel.TypeIds
            Get
                Return TypeIds.FeatureTranscriptionUnit
            End Get
        End Property
    End Class
End Namespace
