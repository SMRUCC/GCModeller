Imports System.Xml.Serialization
Imports LANS.SystemsBiology.ComponentModel.EquaionModel
Imports LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.RuntimeObjects
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace EngineSystem.ObjectModels.Module.EquationModel

    ''' <summary>
    ''' 在一个代谢反应对象之中对代谢组底物对象的引用
    ''' </summary>
    ''' <remarks></remarks>
    Public Class CompoundSpecieReference : Inherits RuntimeObject
        Implements sIdEnumerable
        Implements ICompoundSpecies

        Public Property EntityCompound As Entity.Compound

        ''' <summary>
        ''' Guid/MetaCyc UniqueId String.(Guid或者MetaCyc数据库里面的UniqueId字符串)
        ''' </summary>
        ''' <remarks></remarks>
        <DumpNode> <XmlAttribute> Public Property Identifier As String Implements sIdEnumerable.Identifier
        ''' <summary>
        ''' 化学计量数
        ''' </summary>
        ''' <remarks></remarks>
        <DumpNode> <XmlAttribute> Public Property Stoichiometry As Double Implements ICompoundSpecies.StoiChiometry

        Public Sub New()
        End Sub

        <DumpNode> Protected Friend _ConstraintTestValue As Double

        Protected Friend Function ConstraintTest(FluxValue As Double) As Double
            Me._ConstraintTestValue = (Me.EntityCompound.DataSource.Value + Stoichiometry * FluxValue) / Stoichiometry
            Return Me._ConstraintTestValue
        End Function

        Public Overrides Function ToString() As String
            Return EntityCompound.ToString
        End Function

        Public Function Flowing(Flux As Double) As Integer
            If EntityCompound Is Nothing Then
                MsgBox(1)
            End If
            EntityCompound.Quantity = EntityCompound.DataSource.Value + Flux / Stoichiometry
            Return 0
        End Function

        Public Shared Operator /(Compound As CompoundSpecieReference, value As Double) As Double
            Return Compound.EntityCompound.Quantity / value
        End Operator

        Public Shared Function CreateObject(Model As Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference,
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