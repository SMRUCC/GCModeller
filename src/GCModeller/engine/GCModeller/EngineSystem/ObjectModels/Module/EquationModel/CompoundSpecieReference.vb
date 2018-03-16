#Region "Microsoft.VisualBasic::3bcc7b823202ba961875f910add4f82a, engine\GCModeller\EngineSystem\ObjectModels\Module\EquationModel\CompoundSpecieReference.vb"

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

    '     Class CompoundSpecieReference
    ' 
    '         Properties: EntityCompound, Identifier, Stoichiometry
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: ConstraintTest, (+2 Overloads) CreateObject, Flowing, ToString
    '         Operators: /
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Serialization
Imports SMRUCC.genomics.ComponentModel.EquaionModel
Imports SMRUCC.genomics.GCModeller.Assembly
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.RuntimeObjects

Namespace EngineSystem.ObjectModels.Module.EquationModel

    ''' <summary>
    ''' 在一个代谢反应对象之中对代谢组底物对象的引用
    ''' </summary>
    ''' <remarks></remarks>
    Public Class CompoundSpecieReference : Inherits RuntimeObject
        Implements INamedValue
        Implements ICompoundSpecies

        Public Property EntityCompound As Entity.Compound

        ''' <summary>
        ''' Guid/MetaCyc UniqueId String.(Guid或者MetaCyc数据库里面的UniqueId字符串)
        ''' </summary>
        ''' <remarks></remarks>
        <DumpNode> <XmlAttribute> Public Property Identifier As String Implements INamedValue.Key
        ''' <summary>
        ''' 化学计量数
        ''' </summary>
        ''' <remarks></remarks>
        <DumpNode> <XmlAttribute> Public Property Stoichiometry As Double Implements ICompoundSpecies.StoiChiometry

        Public Sub New()
        End Sub

        <DumpNode> Protected Friend _ConstraintTestValue As Double

        Protected Friend Function ConstraintTest(FluxValue As Double) As Double
            Me._ConstraintTestValue = (Me.EntityCompound.DataSource.value + Stoichiometry * FluxValue) / Stoichiometry
            Return Me._ConstraintTestValue
        End Function

        Public Overrides Function ToString() As String
            Return EntityCompound.ToString
        End Function

        Public Function Flowing(Flux As Double) As Integer
            If EntityCompound Is Nothing Then
                MsgBox(1)
            End If
            EntityCompound.Quantity = EntityCompound.DataSource.value + Flux / Stoichiometry
            Return 0
        End Function

        Public Shared Operator /(Compound As CompoundSpecieReference, value As Double) As Double
            Return Compound.EntityCompound.Quantity / value
        End Operator

        Public Shared Function CreateObject(Model As GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference,
                                            Metabolites As Entity.Compound()) _
            As CompoundSpecieReference

            Dim CreatedObject As CompoundSpecieReference = New CompoundSpecieReference
            CreatedObject.Identifier = Model.Identifier
            CreatedObject.Stoichiometry = Model.StoiChiometry
            CreatedObject.EntityCompound = Metabolites.GetItem(Model.Identifier)
            Return CreatedObject
        End Function

        Public Shared Function CreateObject(MetaboliteEntity As Entity.Compound, Stoichiometry As Integer) As CompoundSpecieReference
            Return New CompoundSpecieReference With {.Identifier = MetaboliteEntity.Identifier, .Stoichiometry = Stoichiometry, .EntityCompound = MetaboliteEntity}
        End Function
    End Class
End Namespace
