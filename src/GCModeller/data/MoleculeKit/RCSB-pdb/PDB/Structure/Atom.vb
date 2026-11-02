Namespace Structures

    ''' <summary>
    ''' The unified molecular atom model for the PDB/SDF structure readers.
    ''' </summary>
    ''' <remarks>
    ''' This class is the single merged atom model of this library: it only carries the
    ''' general physico-chemical fields (coordinates, element, partial charge, residue and
    ''' chain annotations). Domain specific fields (e.g. the AutoDock Vina atom typing) should
    ''' be placed on a derived type instead of polluting this base model.
    '''
    ''' Both of the legacy keyword level atom models
    ''' (<see cref="Keywords.AtomUnit"/> for ``ATOM`` records and
    ''' <see cref="Keywords.HETATM.HETATMRecord"/> for ``HETATM`` records) are inherited from
    ''' this class, so that the fixed-column parsing logic can be shared in one place.
    ''' </remarks>
    Public Class Atom

        ''' <summary>
        ''' The X axis coordinate value in angstrom.
        ''' </summary>
        ''' <returns></returns>
        Public Property X As Double
        ''' <summary>
        ''' The Y axis coordinate value in angstrom.
        ''' </summary>
        ''' <returns></returns>
        Public Property Y As Double
        ''' <summary>
        ''' The Z axis coordinate value in angstrom.
        ''' </summary>
        ''' <returns></returns>
        Public Property Z As Double

        ''' <summary>
        ''' The normalized element symbol, e.g. ``C``/``N``/``O``/``Cl``/``FE``.
        ''' </summary>
        ''' <returns></returns>
        Public Property Element As String = "C"

        ''' <summary>
        ''' The partial charge of current atom site.
        ''' </summary>
        ''' <returns></returns>
        Public Property Charge As Double

        ''' <summary>
        ''' The chain identifier (PDB column 22).
        ''' </summary>
        ''' <returns></returns>
        Public Property ChainID As String = " "

        ''' <summary>
        ''' The residue name (PDB column 18-20), in upper case.
        ''' </summary>
        ''' <returns></returns>
        Public Property ResName As String = ""

        ''' <summary>
        ''' The residue sequence number (PDB column 23-26).
        ''' </summary>
        ''' <returns></returns>
        Public Property ResSeq As Integer = 0

        ''' <summary>
        ''' The atom name (PDB column 13-16).
        ''' </summary>
        ''' <returns></returns>
        Public Property AtomName As String = ""

        ''' <summary>
        ''' The atom serial number (PDB column 7-11).
        ''' </summary>
        ''' <returns></returns>
        Public Property Serial As Integer = 0

        ''' <summary>
        ''' The alternate location indicator (PDB column 17).
        ''' </summary>
        ''' <returns></returns>
        Public Property AltLoc As String = ""

        ''' <summary>
        ''' The occupancy value (PDB column 55-60).
        ''' </summary>
        ''' <returns></returns>
        Public Property Occupancy As Double = 1.0

        ''' <summary>
        ''' The temperature factor, aka B-factor (PDB column 61-66).
        ''' </summary>
        ''' <returns></returns>
        Public Property TempFactor As Double = 0.0

        ''' <summary>
        ''' Is current atom comes from a ``HETATM`` record? (``False`` means the ``ATOM`` record)
        ''' </summary>
        ''' <returns></returns>
        Public Property IsHet As Boolean = False

        ''' <summary>
        ''' Is current atom a water molecule? (residue name is ``HOH`` or ``WAT``)
        ''' </summary>
        ''' <returns></returns>
        Public Property IsWater As Boolean = False

        ''' <summary>
        ''' Get/set the atom spatial position as a point tuple in 3D space.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' This property is a value type view of the <see cref="X"/>, <see cref="Y"/> and
        ''' <see cref="Z"/> fields, which is kept for the backward compatibility with the
        ''' legacy <see cref="Keywords.AtomUnit.Location"/> property.
        ''' </remarks>
        Public Property Location As Keywords.Point3D
            Get
                Return New Keywords.Point3D(X, Y, Z)
            End Get
            Set(value As Keywords.Point3D)
                X = value.X
                Y = value.Y
                Z = value.Z
            End Set
        End Property

        ''' <summary>
        ''' Copy all of the general field values from another atom instance.
        ''' </summary>
        ''' <param name="atom">The atom model that provides the source field values.</param>
        Public Overridable Sub CopyFrom(atom As Atom)
            If atom Is Nothing Then
                Return
            End If

            X = atom.X
            Y = atom.Y
            Z = atom.Z
            Element = atom.Element
            Charge = atom.Charge
            ChainID = atom.ChainID
            ResName = atom.ResName
            ResSeq = atom.ResSeq
            AtomName = atom.AtomName
            Serial = atom.Serial
            AltLoc = atom.AltLoc
            Occupancy = atom.Occupancy
            TempFactor = atom.TempFactor
            IsHet = atom.IsHet
            IsWater = atom.IsWater
        End Sub

        ''' <summary>
        ''' Copy all of the general field values of current atom into another atom instance.
        ''' </summary>
        ''' <param name="atom">The atom model that accepts the field values.</param>
        Public Sub CopyTo(atom As Atom)
            If atom Is Nothing Then
                Return
            End If

            atom.CopyFrom(Me)
        End Sub

        Public Overrides Function ToString() As String
            Return $"[{Serial}] {AtomName}@{ResName}{ResSeq}:{ChainID} ({Element}) {Location.ToString}"
        End Function

    End Class
End Namespace
