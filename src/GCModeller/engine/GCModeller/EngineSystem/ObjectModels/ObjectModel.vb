#Region "Microsoft.VisualBasic::50bbaca85cded4b7aa1b01d47c5bc4f8, ..\GCModeller\engine\GCModeller\EngineSystem\ObjectModels\ObjectModel.vb"

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

Imports System.Xml.Serialization
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.RuntimeObjects
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Serialization

Namespace EngineSystem.ObjectModels

    Public MustInherit Class ObjectModel : Inherits RuntimeObject
        Implements IAddressOf, INamedValue

        ''' <summary>
        ''' Guid/MetaCyc UniqueId String.(Guid或者MetaCyc数据库里面的UniqueId字符串)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DumpNode> <XmlAttribute>
        Public Overridable Property Identifier As String Implements INamedValue.Key

        ''' <summary>
        ''' The index handle of this object instance in the collection of the metabolite compounds in this system model.
        ''' (该对象在当前子系统模型对象实例中的代谢物列表中的索引号)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DumpNode> <XmlAttribute>
        Public Overridable Property Handle As Integer Implements IAddressOf.Address

        Public MustOverride ReadOnly Property TypeId As TypeIds

        Public Overrides Function ToString() As String
            Return Identifier
        End Function

        Public Enum TypeIds
            MetabolismFlux
            EnzymaticFlux
            EntityCompound
            EntityReactionModifier
            EntityRegulator
            EntityTranscript

            ExpressionConstraintFlux
            ActiveTransportationFlux
            PassiveTransportationFlux
            BasalExpression
            CentralDogma
            Pathway

            FeatureMetabolismEnzyme
            FeatureGene
            FeatureTranscriptionUnit
            FeatureMotifSite

            EventTranslation
            EventTranscription
        End Enum
    End Class
End Namespace
