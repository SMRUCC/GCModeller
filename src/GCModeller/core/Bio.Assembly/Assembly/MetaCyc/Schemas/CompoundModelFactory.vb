Imports LANS.SystemsBiology.Assembly
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace Assembly.MetaCyc.Schema

    Public Module CompoundModelFactory

        Public Class GeneralCompoundModel : Implements ICompoundObject

            Public Property Identifier As String Implements sIdEnumerable.Identifier
            Public Property CommonNames As String() Implements ICompoundObject.CommonNames

            Public Property CHEBI As String() Implements ICompoundObject.CHEBI
            Public Property KEGGCompound As String Implements ICompoundObject.locusId
            Public Property PUBCHEM As String Implements ICompoundObject.PUBCHEM

            Public Function GenerateModels(data As Generic.IEnumerable(Of LANS.SystemsBiology.Assembly.KEGG.DBGET.bGetObject.Compound)) As GeneralCompoundModel()
                Dim LQuery = (From item In data
                              Select New GeneralCompoundModel With {
                                  .Identifier = item.Entry,
                                  ._CHEBI = item.CHEBI,
                                  .CommonNames = item.CommonNames,
                                  .KEGGCompound = item.Entry,
                                  ._PUBCHEM = item.PUBCHEM}).ToArray
                Return LQuery
            End Function

            Public Function GenerateModels(data As Generic.IEnumerable(Of MetaCyc.File.DataFiles.Slots.Compound)) As GeneralCompoundModel()
                Dim LQuery = (From item In data
                              Select New GeneralCompoundModel With {
                                  .Identifier = item.Identifier,
                                  ._PUBCHEM = item.PUBCHEM,
                                  ._CHEBI = item.CHEBI,
                                  .CommonNames = item.Names,
                                  .KEGGCompound = item.KEGGCompound}).ToArray
                Return LQuery
            End Function
        End Class


    End Module
End Namespace