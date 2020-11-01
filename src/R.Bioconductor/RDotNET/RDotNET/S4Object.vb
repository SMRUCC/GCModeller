Imports RDotNet.Diagnostics
Imports RDotNet.Internals
Imports System
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Linq
Imports System.Security.Permissions


    ''' <summary>
    ''' An S4 object
    ''' </summary>
    <DebuggerDisplay("SlotCount = {SlotCount}; RObjectType = {Type}")>
    <DebuggerTypeProxy(GetType(S4ObjectDebugView))>
    <SecurityPermission(SecurityAction.Demand, Flags:=SecurityPermissionFlag.UnmanagedCode)>
    Public Class S4Object
        Inherits SymbolicExpression
        ''' <summary>
        ''' Function .slotNames
        ''' </summary>
        ''' <remarks>
        ''' slotNames, when used on the class representation object, returns the slot names of
        ''' instances of the class, rather than the slot names of the class object itself. '.slotNames' is what we want.
        ''' </remarks>
        Private Shared dotSlotNamesFunc As [Function] = Nothing

        ''' <summary>
        ''' Create a new S4 object
        ''' </summary>
        ''' <param name="engine">R engine</param>
        ''' <param name="pointer">pointer to native S4 SEXP</param>
        Protected Friend Sub New(ByVal engine As REngine, ByVal pointer As IntPtr)
            MyBase.New(engine, pointer)
            If dotSlotNamesFunc Is Nothing Then dotSlotNamesFunc = MyBase.Engine.Evaluate("invisible(.slotNames)").AsFunction()
        End Sub

        ''' <summary>
        ''' Gets/sets the value of a slot
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        Default Public Property Item(ByVal name As String) As SymbolicExpression
            Get
                checkSlotName(name)
                Dim slotValue As IntPtr

                Using s = New ProtectedPointer(Engine, GetFunction(Of Rf_mkString)()(InternalString.NativeUtf8FromString(name)))
                    slotValue = GetFunction(Of R_do_slot)()(DangerousGetHandle(), s)
                End Using

                Return New SymbolicExpression(Engine, slotValue)
            End Get
            Set(ByVal value As SymbolicExpression)
                checkSlotName(name)

                Using s = New ProtectedPointer(Engine, GetFunction(Of Rf_mkString)()(InternalString.NativeUtf8FromString(name)))

                    Using New ProtectedPointer(Me)
                        GetFunction(Of R_do_slot_assign)()(DangerousGetHandle(), s, value.DangerousGetHandle())
                    End Using
                End Using
            End Set
        End Property

        Private Sub checkSlotName(ByVal name As String)
            If Not SlotNames.Contains(name) Then Throw New ArgumentException(String.Format("Invalid slot name '{0}'", name), "name")
        End Sub

        ''' <summary>
        ''' Is a slot name valid.
        ''' </summary>
        ''' <param name="slotName">the name of the slot</param>
        ''' <returns>whether a slot name is present in the object</returns>
        Public Function HasSlot(ByVal slotName As String) As Boolean
            Using s = New ProtectedPointer(Engine, GetFunction(Of Rf_mkString)()(InternalString.NativeUtf8FromString(slotName)))
                Return GetFunction(Of R_has_slot)()(DangerousGetHandle(), s)
            End Using
        End Function

        Private slotNamesField As String() = Nothing

        ''' <summary>
        ''' Gets the slot names for this object. The values are cached once retrieved the first time.
        ''' Note this is equivalent to the function '.slotNames' in R, not 'slotNames'
        ''' </summary>
        Public ReadOnly Property SlotNames As String()
            Get
                If slotNamesField Is Nothing Then slotNamesField = dotSlotNamesFunc.Invoke(Me).AsCharacter().ToArray()
                Return CType(slotNamesField.Clone(), String())
            End Get
        End Property

        ''' <summary>
        ''' Gets the number of slot names
        ''' </summary>
        Public ReadOnly Property SlotCount As Integer
            Get
                Return SlotNames.Length
            End Get
        End Property

        ''' <summary>
        ''' Gets the class representation.
        ''' </summary>
        ''' <returns>The class representation of the S4 class.</returns>
        Public Function GetClassDefinition() As S4Object
            Dim classSymbol = Engine.GetPredefinedSymbol("R_ClassSymbol")
            Dim className = GetAttribute(classSymbol).AsCharacter().First()
            Dim definition = Engine.GetFunction(Of R_getClassDef)()(className)
            Return New S4Object(Engine, definition)
        End Function

        ''' <summary>
        ''' Gets slot names and types.
        ''' </summary>
        ''' <returns>Slot names.</returns>
        Public Function GetSlotTypes() As IDictionary(Of String, String)
            Dim definition = GetClassDefinition()
            Dim slots = definition("slots")
            Dim namesSymbol = Engine.GetPredefinedSymbol("R_NamesSymbol")
            Return slots.GetAttribute(namesSymbol).AsCharacter().Zip(slots.AsCharacter(), Function(name, type) New With {
                name,
                type
            }).ToDictionary(Function(t) t.Name, Function(t) t.Type)
        End Function
    End Class

