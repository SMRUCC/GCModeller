Imports System.Text
Imports System.Xml.Serialization
Imports SMRUCC.genomics.Assembly.SBML.Specifics.MetaCyc
Imports SMRUCC.genomics.ComponentModel.EquaionModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace Level2.Elements

    ''' <summary>
    ''' 生化反应对象对代谢物的引用类型
    ''' </summary>
    ''' <remarks></remarks>
    <XmlType("speciesReference", Namespace:="http://www.sbml.org/sbml/level2")>
    Public Class speciesReference : Implements ICompoundSpecies

        ''' <summary>
        ''' Unique-Id for the metabolite.(目标参加本反应的代谢物对象的Unique-ID属性)
        ''' </summary>
        ''' <remarks></remarks>
        <Escaped> <XmlAttribute()>
        Public Property species As String Implements sIdEnumerable.Identifier
        ''' <summary>
        ''' (化学计量数)
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute()>
        Public Property stoichiometry As Double Implements ICompoundSpecies.StoiChiometry

        Sub New()
        End Sub

        Sub New(x As ICompoundSpecies)
            species = x.Identifier
            stoichiometry = x.StoiChiometry
        End Sub

        Public Overrides Function ToString() As String
            Return String.Format("species={0}; stoichiometry={1}", species, stoichiometry)
        End Function

        Public Overloads Function Equals(b As speciesReference, strict As Boolean) As Boolean
            Return Equivalence.Equals(Me, b, strict)
        End Function

        Public Function CopyData() As speciesReference
            Return New speciesReference With {
                .species = Me.species,
                .stoichiometry = Me.stoichiometry
            }
        End Function
    End Class
End Namespace