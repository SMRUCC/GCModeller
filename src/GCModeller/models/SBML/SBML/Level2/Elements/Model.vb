Imports System.Text
Imports Microsoft.VisualBasic

Namespace Level2.Elements

    Public Class Model : Inherits Components.ModelBase

        Public Property listOfUnitDefinitions As List(Of Elements.unitDefinition)
        Public Property listOfCompartments As List(Of SBML.Components.Compartment)

        ''' <summary>
        ''' 在当前的这个SBML文件之中所定义的代谢物对象的列表 
        ''' </summary>
        ''' <remarks></remarks>
        Public Property listOfSpecies As List(Of Elements.Specie)
        Public Property listOfReactions As List(Of Elements.Reaction)

        Public Sub [AddHandler]()
            For i As Integer = 0 To listOfReactions.Count - 1
                listOfReactions(i).Handle = i
            Next
        End Sub
    End Class
End Namespace