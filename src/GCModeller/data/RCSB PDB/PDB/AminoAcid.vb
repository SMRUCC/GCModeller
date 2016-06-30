
''' <summary>
''' 氨基酸残基
''' </summary>
''' <remarks></remarks>
Public Class AminoAcid

    Public Property Index As Integer
    Public Property AA_ID As String
    Public Property Atoms As Keywords.AtomUnit()

    ''' <summary>
    ''' 中心的碳原子
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Carbon As Keywords.AtomUnit
        Get
            Dim CLQuery = (From Atom In Atoms Where String.Equals(Atom.Atom, "C") Select Atom).FirstOrDefault
            If CLQuery Is Nothing Then
                Return Atoms.First
            Else
                Return CLQuery
            End If
        End Get
    End Property

    Public Shared Function SequenceGenerator(Atoms As Keywords.Atom) As AminoAcid()
        Dim Resource = (From Atom In Atoms Select Atom Group Atom By Atom.AA_IDX Into Group).ToArray
        Dim LQuery = (From item In Resource
                      Select AA = New AminoAcid With {
                          .Index = item.AA_IDX,
                          .AA_ID = item.Group.First.AA_ID,
                          .Atoms = item.Group.ToArray
                      }
                      Order By AA.Index Ascending).ToArray
        Return LQuery
    End Function
End Class
