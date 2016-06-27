Imports LANS.SystemsBiology.ComponentModel.EquaionModel
Imports LANS.SystemsBiology.ComponentModel.EquaionModel.DefaultTypes
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace Assembly.MetaCyc.Schema

    Public Class EquationEquals

        Protected Friend _CompoundMapping As EffectorMap()

        Public ReadOnly Property CompoundMapping As EffectorMap()
            Get
                Return _CompoundMapping
            End Get
        End Property

        Sub New(MetaCycCompounds As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Compounds, CompoundSpecies As ICompoundObject())
            _CompoundMapping = New MetaCyc.Schema.CompoundsMapping(MetaCycCompounds).EffectorMapping(CompoundSpecies)
            _CompoundMapping = (From item In _CompoundMapping Where Not String.IsNullOrEmpty(item.MetaCycId) Select item).ToArray
        End Sub

        Sub New(MetaCycCompoundModels As Generic.IEnumerable(Of ICompoundObject), CompoundSpecies As ICompoundObject())
            _CompoundMapping = New MetaCyc.Schema.CompoundsMapping(MetaCycCompoundModels.ToArray).EffectorMapping(CompoundSpecies)
            _CompoundMapping = (From item In _CompoundMapping Where Not String.IsNullOrEmpty(item.MetaCycId) Select item).ToArray
        End Sub

        Public Overloads Function Equals(Expression As String, MetaCycEquation As Metabolism.Reaction, Explicit As Boolean) As Boolean
            Dim Equation1 = EquationBuilder.CreateObject(Of CompoundSpecieReference, Equation)(Expression)

            If SideEquals(Equation1.Reactants, MetaCycEquation.Reactants, Explicit) AndAlso
                SideEquals(Equation1.Products, MetaCycEquation.Products, Explicit) Then
                Return True
            Else
                Dim f As Boolean = SideEquals(Equation1.Reactants, MetaCycEquation.Products, Explicit) AndAlso
                    SideEquals(Equation1.Products, MetaCycEquation.Reactants, Explicit)
                Return f
            End If
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Expression1">使用其他代谢物作为标识符的方程表达式</param>
        ''' <param name="Expression2">使用MetaCyc代谢物作为标识的方程表达式</param>
        ''' <param name="Explicit">是否严格等价，假若参数值为假，则仅考虑标识符等价即可</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function Equals(Expression1 As String, Expression2 As String, Explicit As Boolean) As Boolean
            Dim Equation1 = EquationBuilder.CreateObject(Of CompoundSpecieReference, Equation)(Expression1)
            Dim Equation2 = EquationBuilder.CreateObject(Of CompoundSpecieReference, Equation)(Expression2)

            If SideEquals(Equation1.Reactants, Equation2.Reactants, Explicit) AndAlso
                SideEquals(Equation1.Products, Equation2.Products, Explicit) Then
                Return True
            Else
                Dim f As Boolean = SideEquals(Equation1.Reactants, Equation2.Products, Explicit) AndAlso
                    SideEquals(Equation1.Products, Equation2.Reactants, Explicit)
                Return f
            End If
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Side1"></param>
        ''' <param name="Side2">MetaCyc ID</param>
        ''' <param name="Explicit"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function SideEquals(Side1 As CompoundSpecieReference(), Side2 As ICompoundSpecies(), Explicit As Boolean) As Boolean

            If Side1.Length <> Side2.Length Then
                Return False
            End If

            Dim LQuery = (From item In Side1 Let MAP = _CompoundMapping.GetItem(item.Identifier)
                          Where (Not MAP Is Nothing) AndAlso Not String.IsNullOrEmpty(MAP.MetaCycId)
                          Let value = New KeyValuePair(Of CompoundSpecieReference, EffectorMap)(item, MAP)
                          Select value).ToArray

            If LQuery.Length < Side2.Length Then
                Return False
            Else
                If Explicit Then
                    Return SideEqualsExplicit(LQuery, Side2)
                Else
                    Return SideEqualsNOTExplicit(LQuery, Side2)
                End If
            End If
        End Function

        Private Function SideEqualsExplicit(Side1 As KeyValuePair(Of CompoundSpecieReference, EffectorMap)(), Side2 As ICompoundSpecies()) As Boolean
            For Each item In Side1
                Dim MetaCycItem = Side2.GetItem(item.Value.MetaCycId)
                If MetaCycItem Is Nothing Then
                    Return False
                Else
                    If MetaCycItem.StoiChiometry <> item.Key.StoiChiometry Then
                        Return False
                    End If
                End If
            Next
            Return True
        End Function

        Private Function SideEqualsNOTExplicit(Side1 As KeyValuePair(Of CompoundSpecieReference, EffectorMap)(), Side2 As ICompoundSpecies()) As Boolean
            For Each item In Side1
                Dim MetaCycItem = Side2.GetItem(item.Value.MetaCycId)
                If MetaCycItem Is Nothing Then
                    Return False
                End If
            Next
            Return True
        End Function
    End Class
End Namespace