#Region "Microsoft.VisualBasic::dca1228058ca935dcec64b6238371709, engine\GCModeller\EngineSystem\ObjectModels\Entity\ReactionModifier.vb"

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

    '     Class ReactionModifier
    ' 
    '         Properties: DataSource, InhibitionType, Quantity, TypeId
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: GetRegulationFluxValue
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Serialization
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.Services

Namespace EngineSystem.ObjectModels.Entity

    ''' <summary>
    ''' 对生化反应过程其调控作用的小分子化合物，其调控的对象包括反应过程以及酶分子对象
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ReactionModifier : Inherits Compound

        Protected Friend RegulatorBaseType As GCML_Documents.XmlElements.SignalTransductions.Regulator

        <DumpNode> Public ReadOnly Property InhibitionType As Boolean
            Get
                Return RegulatorBaseType.Activation
            End Get
        End Property

        <DumpNode> Public Overrides Property Quantity As Double
            Get
                Return MyBase.EntityBaseType.Quantity
            End Get
            Set(value As Double)
                MyBase.EntityBaseType.Quantity = value
            End Set
        End Property

        Public Overrides ReadOnly Property DataSource As DataSource
            Get
                Return New DataSource(Handle, EntityBaseType.DataSource.value)
            End Get
        End Property

        <DumpNode> Public b = 2
        <DumpNode> Public m As Double = 2

        Sub New(Regulator As GCML_Documents.XmlElements.SignalTransductions.Regulator, Metabolite As Entity.Compound)
            Me.RegulatorBaseType = Regulator
            MyBase.EntityBaseType = Metabolite
            MyBase.Identifier = Regulator.Identifier
            Me.ModelBaseElement = Metabolite.ModelBaseElement
        End Sub

        Protected Friend Sub New()
        End Sub

        Public Shared Function GetRegulationFluxValue(Regulator As ReactionModifier) As Double
            Dim v As Double = 1

            If Regulator.InhibitionType Then
                v = 1 / (1 + (Regulator.Quantity / Regulator.b) ^ Regulator.m)
            Else
                Dim n = (Regulator.Quantity / Regulator.b) ^ Regulator.m
                v = n / (1 + n)
            End If

            Return v
        End Function

        Public Overrides ReadOnly Property TypeId As ObjectModel.TypeIds
            Get
                Return TypeIds.EntityReactionModifier
            End Get
        End Property
    End Class
End Namespace
