Imports RDotNet.Extensions.VisualBasic.Services.ScriptBuilder
Imports RDotNet.Extensions.VisualBasic.Services.ScriptBuilder.RTypes
Imports RDotNet.Extensions.VisualBasic

Namespace igraph

    ''' <summary>
    ''' make_lattice is a flexible function, it can create lattices of arbitrary dimensions, periodic or unperiodic ones. It has two forms. 
    ''' In the first form you only supply dimvector, but not length and dim. In the second form you omit dimvector and supply length and dim.
    ''' </summary>
    <RFunc("make_lattice")> Public Class make_lattice : Inherits igraph

        ''' <summary>
        ''' A vector giving the size of the lattice in each dimension.
        ''' </summary>
        ''' <returns></returns>
        Public Property dimvector As RExpression = NULL
        ''' <summary>
        ''' Integer constant, for regular lattices, the size of the lattice in each dimension.
        ''' </summary>
        ''' <returns></returns>
        Public Property length As RExpression = NULL
        ''' <summary>
        ''' Integer constant, the dimension of the lattice.
        ''' </summary>
        ''' <returns></returns>
        Public Property [dim] As RExpression = NULL
        ''' <summary>
        ''' The distance within which (inclusive) the neighbors on the lattice will be connected. This parameter is not used right now.
        ''' </summary>
        ''' <returns></returns>
        Public Property nei As Integer = 1
        ''' <summary>
        ''' Whether to create a directed lattice.
        ''' </summary>
        ''' <returns></returns>
        Public Property directed As Boolean = False
        ''' <summary>
        ''' Logical, if TRUE directed lattices will be mutually connected.
        ''' </summary>
        ''' <returns></returns>
        Public Property mutual As Boolean = False
        ''' <summary>
        ''' Logical, if TRUE the lattice or ring will be circular.
        ''' </summary>
        ''' <returns></returns>
        Public Property circular As Boolean = False
    End Class

    ''' <summary>
    ''' Create a lattice graph
    ''' </summary>
    <RFunc("lattice")> Public Class lattice : Inherits make_lattice

    End Class
End Namespace