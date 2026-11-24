Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2

''' <summary>
''' 由代谢网络之中的跨膜反应构建结构化的转运过程模型。
''' 
''' BioCyc数据库的 ``transporters.col`` 表格文件仅仅提供了经过人工整理的转运子清单，
''' 其中并不包含模型内部所使用的反应编号；因此这里以反应方程之中“同一种化合物同时出现在
''' 反应两侧”这一判据来识别转运反应，从而保证每一个转运过程都可以被追溯到具体的反应对象
''' </summary>
Public Class TransportationCompiler

    ReadOnly compartment As CompartmentCompiler

    Sub New(compartment As CompartmentCompiler)
        Me.compartment = compartment
    End Sub

    ''' <summary>
    ''' 从代谢反应列表之中构建转运过程集合
    ''' </summary>
    ''' <param name="reactions"></param>
    ''' <returns></returns>
    Public Function CreateTransportations(reactions As IEnumerable(Of Reaction)) As Transportation()
        Return reactions _
            .SafeQuery _
            .Where(Function(rxn) Not rxn Is Nothing AndAlso rxn.CheckTransportation()) _
            .Select(AddressOf createTransportation) _
            .ToArray
    End Function

    Private Function createTransportation(rxn As Reaction) As Transportation
        Dim membranes As New List(Of String)

        For Each id As String In rxn.compartment.SafeQuery
            Dim mapped As String = compartment.MapCompartId(id)
            Dim matched As Compartment = compartment.Compartments _
                .Where(Function(c) c.id = mapped) _
                .FirstOrDefault

            If matched Is Nothing Then
                Continue For
            End If
            If matched.kind = CompartmentKind.innerMembrane OrElse matched.kind = CompartmentKind.outerMembrane Then
                Call membranes.Add(mapped)
            End If
        Next

        Return New Transportation With {
            .guid = rxn.ID,
            .name = rxn.name,
            .membrane = membranes.Distinct.ToArray,
            .enzymes = rxn.enzymes,
            .substrates = rxn.substrate,
            .products = rxn.product,
            .reaction = {rxn.ID},
            .couplingType = getCouplingType(rxn)
        }
    End Function

    Private Shared Function getCouplingType(rxn As Reaction) As String
        Dim names As String() = rxn.substrate _
            .SafeQuery _
            .Select(Function(c) c.compound) _
            .Where(Function(id) Not id.StringEmpty(, True)) _
            .Select(Function(id) id.ToUpperInvariant) _
            .ToArray

        If names.Any(Function(id) id Like "*ATP*") Then
            Return "ATP"
        ElseIf names.Any(Function(id) id = "H+" OrElse id Like "*PROTON*") Then
            Return "symport"
        Else
            Return "facilitated-diffusion"
        End If
    End Function

End Class
