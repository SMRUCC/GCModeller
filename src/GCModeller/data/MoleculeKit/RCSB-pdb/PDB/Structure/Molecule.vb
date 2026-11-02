Namespace Structures

    ''' <summary>
    ''' A molecule (or a collection of protein chains) topology model.
    ''' </summary>
    ''' <typeparam name="T">
    ''' The atom model type, must be derived from <see cref="Atom"/> and exposes a public
    ''' parameter-less constructor. Making the container generic keeps the domain specific
    ''' atom types (e.g. the AutoDock Vina atom model) strongly typed, so that the caller
    ''' does not need any down-casting operation.
    ''' </typeparam>
    Public Class Molecule(Of T As {Atom, New})

        ''' <summary>
        ''' The atom sites of current molecule.
        ''' </summary>
        ''' <returns></returns>
        Public Property Atoms As New List(Of T)()
        ''' <summary>
        ''' The covalent bonds of current molecule.
        ''' </summary>
        ''' <returns></returns>
        Public Property Bonds As New List(Of Bond)()
        ''' <summary>
        ''' The molecule title or the source file basename.
        ''' </summary>
        ''' <returns></returns>
        Public Property Id As String = ""

        ''' <summary>
        ''' Get the atom count of current molecule.
        ''' </summary>
        ''' <returns></returns>
        Public Function AtomCount() As Integer
            Return Atoms.Count
        End Function

        ''' <summary>
        ''' Create a shallow clone of current molecule: the atom instances are shared
        ''' with the source molecule, but the bond list is a new list.
        ''' </summary>
        ''' <param name="id">
        ''' The molecule id of the cloned result, nothing then keeps the original id value.
        ''' </param>
        ''' <returns></returns>
        Public Function Clone(Optional id As String = Nothing) As Molecule(Of T)
            Dim copy As New Molecule(Of T) With {
                .Id = If(id Is Nothing, Me.Id, id)
            }

            copy.Atoms.AddRange(Atoms)
            copy.Bonds.AddRange(Bonds)

            Return copy
        End Function

        Public Overrides Function ToString() As String
            Return $"[{Id}] {Atoms.Count} atoms, {Bonds.Count} bonds"
        End Function

    End Class
End Namespace
