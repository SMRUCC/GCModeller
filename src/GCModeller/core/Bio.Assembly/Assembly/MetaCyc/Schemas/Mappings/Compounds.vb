Imports System.Text.RegularExpressions
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem
Imports LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles

Namespace Assembly.MetaCyc.Schema

    Public Interface ICompoundObject : Inherits sIdEnumerable
        Property CommonNames As String()
        Property PUBCHEM As String
        Property CHEBI As String()
        Property locusId As String
    End Interface

    ''' <summary>
    ''' Regprecise Effector与MetaCyc Compounds Mapping
    ''' </summary>
    ''' <remarks></remarks>
    Public Class EffectorMap : Implements sIdEnumerable, ICompoundObject

        Public Property Effector As String Implements sIdEnumerable.Identifier
        ''' <summary>
        ''' <see cref="ICompoundObject.CommonNames"></see>
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property EffectorAlias As String() Implements ICompoundObject.CommonNames
        Public Property MetaCycId As String
        Public Property CommonName As String
        Public Property Synonym As String

        Sub New()
        End Sub

        Public Overrides Function ToString() As String
            Return String.Format("{0} --> {1}", Effector, MetaCycId)
        End Function

        Public Property CHEBI As String() Implements ICompoundObject.CHEBI
        Public Property KEGGCompound As String Implements ICompoundObject.locusId
        Public Property PUBCHEM As String Implements ICompoundObject.PUBCHEM

        Public Shared Function GenerateMap(CompoundSpecies As ICompoundObject()) As EffectorMap()
            Dim Effectors = (From cps As ICompoundObject In CompoundSpecies Select cps).ToArray
            Return Effectors
        End Function

        Private Shared Function __newMap(cps As ICompoundObject) As EffectorMap
            Return New EffectorMap With {
                .Effector = cps.Identifier,
                .EffectorAlias = cps.CommonNames,
                ._CHEBI = cps.CHEBI,
                .KEGGCompound = cps.Identifier,
                ._PUBCHEM = cps.PUBCHEM
            }
        End Function
    End Class
End Namespace