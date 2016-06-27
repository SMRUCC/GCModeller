Imports LANS.SystemsBiology.Assembly.Expasy.Database
Imports LANS.SystemsBiology.ComponentModel.EquaionModel
Imports LANS.SystemsBiology.ComponentModel.EquaionModel.DefaultTypes
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic

Namespace Assembly.MetaCyc.Schema

    Public Class TransportReaction : Inherits MetaCyc.File.DataFiles.Slots.Reaction
        Implements IEquation(Of CompoundSpecies)

        Public Shared ReadOnly Property PrefixTransportReactionTypes As String() = {
            "TR-11", "TR-12", "TR-13", "TR-14", "TR-19", "TR-20", "TR-21",
            "Transport-Reactions"
        }

        Public Class CompoundSpecies : Inherits MetaCyc.Schema.PropertyAttributes
            Implements ComponentModel.EquaionModel.ICompoundSpecies

            Dim _Compartment As String

            Public ReadOnly Property Compartment As String
                Get
                    Return _Compartment
                End Get
            End Property

            Public Property Identifier As String Implements sIdEnumerable.Identifier
                Get
                    Return MyBase.PropertyValue
                End Get
                Set(value As String)
                    MyBase.PropertyValue = value
                End Set
            End Property

            ''' <summary>
            ''' <see cref="TransportReaction">转运反应对象</see>的<see cref="TransportReaction.Left">左</see><see cref="TransportReaction.Right">右</see>两边的原始属性值
            ''' </summary>
            ''' <param name="strValue"></param>
            ''' <remarks></remarks>
            Sub New(strValue As String)
                Call MyBase.New(strValue)
                _Compartment = GetAttributeValue("COMPARTMENT")
                MyBase.PropertyValue = MetaCyc.File.DataFiles.Reactions.Trim(Identifier)
            End Sub

            Public Overrides Function ToString() As String
                If String.IsNullOrEmpty(_Compartment) Then
                    Return Identifier
                Else
                    Return String.Format("[{0}] {1}", _Compartment, Identifier)
                End If
            End Function

            Public Function GetAttributeValue(Key As String) As String
                Dim LQuery = (From item In _Attributes Where String.Equals(Key, item.Key) Select item.Value).ToArray
                If LQuery.IsNullOrEmpty Then
                    Return ""
                Else
                    Return LQuery.First
                End If
            End Function

            Public Property StoiChiometry As Double Implements ComponentModel.EquaionModel.ICompoundSpecies.StoiChiometry
        End Class

#Region "Shadows Property"

        Dim _ECNumber As MetaCyc.Schema.PropertyAttributes
        Dim _Left, _Right As CompoundSpecies()

        Public Shadows Property ECNumber As MetaCyc.Schema.PropertyAttributes
            Get
                Return _ECNumber
            End Get
            Set(value As MetaCyc.Schema.PropertyAttributes)
                _ECNumber = value
                MyBase.ECNumber = _ECNumber.PropertyValue
            End Set
        End Property
        Public Shadows Property LEFT As CompoundSpecies() Implements IEquation(Of CompoundSpecies).Reactants
            Get
                Return _Left
            End Get
            Set(value As CompoundSpecies())
                _Left = value
                MyBase.Left = (From item In _Left Select item.Identifier).ToList
            End Set
        End Property
        Public Shadows Property RIGHT As CompoundSpecies() Implements IEquation(Of CompoundSpecies).Products
            Get
                Return _Right
            End Get
            Set(value As CompoundSpecies())
                _Right = value
                MyBase.Right = (From item In _Right Select item.Identifier).ToList
            End Set
        End Property

        Public Shadows Property Reversible As Boolean = False Implements IEquation(Of CompoundSpecies).Reversible
#End Region

        Public Function CreateEquatopnExpression() As String
            Dim Model As New Equation With {
                .Reactants = (From item In _Left
                              Let GetUniqueId = Function() As String
                                                    If String.IsNullOrEmpty(item.Compartment) Then
                                                        Return item.Identifier
                                                    Else
                                                        Return String.Format("{0} [^COMPARTMENT - {1}]", item.Identifier, item.Compartment)
                                                    End If
                                                End Function
                              Select New CompoundSpecieReference With {.Identifier = GetUniqueId(), .StoiChiometry = item.StoiChiometry}).ToArray,
                .Products = (From x As CompoundSpecies In _Right
                              Select New CompoundSpecieReference With {
                                  .Identifier = GetUniqueId(x),
                                  .StoiChiometry = x.StoiChiometry}).ToArray,
                                  .Reversible = Reversible
            }
            Return LANS.SystemsBiology.ComponentModel.EquaionModel.EquationBuilder.ToString(Model)
        End Function

        Private Shared Function GetUniqueId(x As CompoundSpecies) As String
            If String.IsNullOrEmpty(x.Compartment) Then
                Return x.Identifier
            Else
                Return String.Format("{0} [^COMPARTMENT - {1}]", x.Identifier, x.Compartment)
            End If
        End Function

        Public Function GetSubstrates() As CompoundSpecies()
            Dim List As List(Of CompoundSpecies) = New List(Of CompoundSpecies)
            Call List.AddRange(LEFT)
            Call List.AddRange(RIGHT)

            Return List.ToArray
        End Function

        Sub New(Reaction As MetaCyc.File.DataFiles.Slots.Reaction)
            Call Reaction.CopyTo(Me)

            MyBase.Left = Reaction.Left
            MyBase.Right = Reaction.Right
            Me.LEFT = (From strValue As String In MyBase.Left Select New CompoundSpecies(strValue)).ToArray
            Me.RIGHT = (From strValue As String In MyBase.Right Select New CompoundSpecies(strValue)).ToArray
            Me.ECNumber = New PropertyAttributes(Reaction.ECNumber)
            MyBase.ECNumber = Reaction.ECNumber
            MyBase.EnzymaticReaction = Reaction.EnzymaticReaction
        End Sub

        Public Shared Function GetTransportReactionExpasyEntries(Transports As TransportReaction(), Expasy As NomenclatureDB) As KeyValuePair(Of String, String())()
            Dim ECNumberLQuery = (From item In Transports
                                  Let ec = item.ECNumber.PropertyValue.Replace("EC-", "").Trim
                                  Where Not String.IsNullOrEmpty(ec)
                                  Select ec
                                  Distinct).ToArray
            Dim LQuery = (From Id As String In ECNumberLQuery
                          Let entries = Expasy.GetSwissProtEntries(ECNumber:=Id, Strict:=False)
                          Where Not entries.IsNullOrEmpty
                          Select New KeyValuePair(Of String, String())(Id, entries)).ToArray
            Return LQuery
        End Function
    End Class
End Namespace