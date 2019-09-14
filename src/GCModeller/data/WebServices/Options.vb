#Region "Microsoft.VisualBasic::fc8796a67f8436aadaa9194ec254877a, WebServices\Options.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    '     Class OptionValueCollection
    ' 
    '         Properties: Count, ICollection_IsSynchronized, ICollection_SyncRoot, IList_IsFixedSize, IList_Item
    '                     IsReadOnly
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: Contains, GetEnumerator, IEnumerable_GetEnumerator, IList_Add, IList_Contains
    '                   IList_IndexOf, IndexOf, Remove, ToArray, ToList
    '                   ToString
    ' 
    '         Sub: Add, AssertValid, Clear, CopyTo, ICollection_CopyTo
    '              IList_Insert, IList_Remove, IList_RemoveAt, Insert, RemoveAt
    ' 
    '     Class OptionContext
    ' 
    '         Properties: [Option], OptionIndex, OptionName, OptionSet, OptionValues
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Enum OptionValueType
    ' 
    '         [Optional], None, Required
    ' 
    '  
    ' 
    ' 
    ' 
    '     Class [Option]
    ' 
    '         Properties: Description, MaxValueCount, Names, OptionValueType, Prototype
    '                     ValueSeparators
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '         Function: GetNames, GetValueSeparators, Parse, ParsePrototype, ToString
    ' 
    '         Sub: AddSeparators, Invoke
    ' 
    '     Class OptionException
    ' 
    '         Properties: OptionName
    ' 
    '         Constructor: (+4 Overloads) Sub New
    '         Sub: GetObjectData
    ' 
    '     Delegate Sub
    ' 
    ' 
    '     Class OptionSet
    ' 
    '         Properties: MessageLocalizer
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '         Function: (+9 Overloads) Add, CreateOptionContext, GetArgumentName, GetDescription, GetKeyForItem
    '                   GetLineEnd, GetLines, GetNextOptionIndex, GetOptionForName, GetOptionParts
    '                   (+3 Overloads) Parse, ParseBool, ParseBundledValue, Unprocessed, WriteOptionPrototype
    ' 
    '         Sub: AddImpl, InsertItem, Invoke, ParseValue, RemoveItem
    '              SetItem, Write, WriteOptionDescriptions
    '         Class ActionOption
    ' 
    '             Constructor: (+1 Overloads) Sub New
    '             Sub: OnParseComplete
    ' 
    '         Class ActionOption
    ' 
    '             Constructor: (+1 Overloads) Sub New
    '             Sub: OnParseComplete
    ' 
    '         Class ActionOption
    ' 
    '             Constructor: (+1 Overloads) Sub New
    '             Sub: OnParseComplete
    ' 
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

'
' Options.cs
'
' Authors:
'  Jonathan Pryor <jpryor@novell.com>
'
' Copyright (C) 2008 Novell (http://www.novell.com)
'
' Permission is hereby granted, free of charge, to any person obtaining
' a copy of this software and associated documentation files (the
' "Software"), to deal in the Software without restriction, including
' without limitation the rights to use, copy, modify, merge, publish,
' distribute, sublicense, and/or sell copies of the Software, and to
' permit persons to whom the Software is furnished to do so, subject to
' the following conditions:
'
' The above copyright notice and this permission notice shall be
' included in all copies or substantial portions of the Software.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
' EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
' MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
' NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
' LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
' OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
' WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
'

' Compile With:
'   gmcs -debug+ -r:System.Core Options.cs -o:NDesk.Options.dll
'   gmcs -debug+ -d:LINQ -r:System.Core Options.cs -o:NDesk.Options.dll
'
' The LINQ version just changes the implementation of
' OptionSet.Parse(IEnumerable<string>), and confers no semantic changes.

'
' A Getopt::Long-inspired option parsing library for C#.
'
' NDesk.Options.OptionSet is built upon a key/value table, where the
' key is a option format string and the value is a delegate that is
' invoked when the format string is matched.
'
' Option format strings:
'  Regex-like BNF Grammar:
'    name: .+
'    type: [=:]
'    sep: ( [^{}]+ | '{' .+ '}' )?
'    aliases: ( name type sep ) ( '|' name type sep )*
'
' Each '|'-delimited name is an alias for the associated action.  If the
' format string ends in a '=', it has a required value.  If the format
' string ends in a ':', it has an optional value.  If neither '=' or ':'
' is present, no value is supported.  `=' or `:' need only be defined on one
' alias, but if they are provided on more than one they must be consistent.
'
' Each alias portion may also end with a "key/value separator", which is used
' to split option values if the option accepts > 1 value.  If not specified,
' it defaults to '=' and ':'.  If specified, it can be any character except
' '{' and '}' OR the *string* between '{' and '}'.  If no separator should be
' used (i.e. the separate values should be distinct arguments), then "{}"
' should be used as the separator.
'
' Options are extracted either from the current option by looking for
' the option name followed by an '=' or ':', or is taken from the
' following option IFF:
'  - The current option does not contain a '=' or a ':'
'  - The current option requires a value (i.e. not a Option type of ':')
'
' The `name' used in the option format string does NOT include any leading
' option indicator, such as '-', '--', or '/'.  All three of these are
' permitted/required on any named option.
'
' Option bundling is permitted so long as:
'   - '-' is used to start the option group
'   - all of the bundled options are a single character
'   - at most one of the bundled options accepts a value, and the value
'     provided starts from the next character to the end of the string.
'
' This allows specifying '-a -b -c' as '-abc', and specifying '-D name=value'
' as '-Dname=value'.
'
' Option processing is disabled by specifying "--".  All options after "--"
' are returned by OptionSet.Parse() unchanged and unprocessed.
'
' Unprocessed options are returned from OptionSet.Parse().
'
' Examples:
'  int verbose = 0;
'  OptionSet p = new OptionSet ()
'    .Add ("v", v => ++verbose)
'    .Add ("name=|value=", v => Console.WriteLine (v));
'  p.Parse (new string[]{"-v", "--v", "/v", "-name=A", "/name", "B", "extra"});
'
' The above would parse the argument string array, and would invoke the
' lambda expression three times, setting `verbose' to 3 when complete.
' It would also print out "A" and "B" to standard output.
' The returned array would contain the string "extra".
'
' C# 3.0 collection initializers are supported and encouraged:
'  var p = new OptionSet () {
'    { "h|?|help", v => ShowHelp () },
'  };
'
' System.ComponentModel.TypeConverter is also supported, allowing the use of
' custom data types in the callback type; TypeConverter.ConvertFromString()
' is used to convert the value option to an instance of the specified
' type:
'
'  var p = new OptionSet () {
'    { "foo=", (Foo f) => Console.WriteLine (f.ToString ()) },
'  };
'
' Random other tidbits:
'  - Boolean options (those w/o '=' or ':' in the option format string)
'    are explicitly enabled if they are followed with '+', and explicitly
'    disabled if they are followed with '-':
'      string a = null;
'      var p = new OptionSet () {
'        { "a", s => a = s },
'      };
'      p.Parse (new string[]{"-a"});   // sets v != null
'      p.Parse (new string[]{"-a+"});  // sets v != null
'      p.Parse (new string[]{"-a-"});  // sets v == null
'

Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Globalization
Imports System.IO
Imports System.Runtime.Serialization
Imports System.Security.Permissions
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Language

#If LINQ Then
Imports System.Linq
#End If

#If TEST Then
Imports NDesk.Options
#End If

Namespace Options

    Public Class OptionValueCollection
        Implements IList
        Implements IList(Of String)

        Private values As New List(Of String)()
        Private c As OptionContext

        Friend Sub New(c As OptionContext)
            Me.c = c
        End Sub

#Region "ICollection"
        Private Sub ICollection_CopyTo(array As Array, index As Integer) Implements ICollection.CopyTo
            TryCast(values, ICollection).CopyTo(array, index)
        End Sub
        Private ReadOnly Property ICollection_IsSynchronized() As Boolean Implements ICollection.IsSynchronized
            Get
                Return TryCast(values, ICollection).IsSynchronized
            End Get
        End Property
        Private ReadOnly Property ICollection_SyncRoot() As Object Implements ICollection.SyncRoot
            Get
                Return TryCast(values, ICollection).SyncRoot
            End Get
        End Property
#End Region

#Region "ICollection<T>"
        Public Sub Add(item As String) Implements ICollection(Of String).Add
            values.Add(item)
        End Sub
        Public Sub Clear() Implements IList.Clear, ICollection(Of String).Clear
            values.Clear()
        End Sub
        Public Function Contains(item As String) As Boolean Implements ICollection(Of String).Contains
            Return values.Contains(item)
        End Function
        Public Sub CopyTo(array As String(), arrayIndex As Integer) Implements ICollection(Of String).CopyTo
            values.CopyTo(array, arrayIndex)
        End Sub
        Public Function Remove(item As String) As Boolean Implements ICollection(Of String).Remove
            Return values.Remove(item)
        End Function
        Public ReadOnly Property Count() As Integer Implements ICollection.Count, ICollection(Of String).Count
            Get
                Return values.Count
            End Get
        End Property
        Public ReadOnly Property IsReadOnly() As Boolean Implements IList.IsReadOnly, ICollection(Of String).IsReadOnly
            Get
                Return False
            End Get
        End Property
#End Region

#Region "IEnumerable"
        Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Return values.GetEnumerator()
        End Function
#End Region

#Region "IEnumerable<T>"
        Public Function GetEnumerator() As IEnumerator(Of String) Implements IEnumerable(Of String).GetEnumerator
            Return values.GetEnumerator()
        End Function
#End Region

#Region "IList"
        Private Function IList_Add(value As Object) As Integer Implements IList.Add
            Return TryCast(values, IList).Add(value)
        End Function

        Private Function IList_Contains(value As Object) As Boolean Implements IList.Contains
            Return TryCast(values, IList).Contains(value)
        End Function
        Private Function IList_IndexOf(value As Object) As Integer Implements IList.IndexOf
            Return TryCast(values, IList).IndexOf(value)
        End Function
        Private Sub IList_Insert(index As Integer, value As Object) Implements IList.Insert
            TryCast(values, IList).Insert(index, value)
        End Sub
        Private Sub IList_Remove(value As Object) Implements IList.Remove
            TryCast(values, IList).Remove(value)
        End Sub
        Private Sub IList_RemoveAt(index As Integer) Implements IList.RemoveAt
            TryCast(values, IList).RemoveAt(index)
        End Sub
        Private ReadOnly Property IList_IsFixedSize() As Boolean Implements IList.IsFixedSize
            Get
                Return False
            End Get
        End Property

        Property IList_Item(index As Integer) As Object Implements IList.Item
            Get
                Return Me(index)
            End Get
            Set
                TryCast(values, IList)(index) = Value
            End Set
        End Property
#End Region

#Region "IList<T>"
        Public Function IndexOf(item As String) As Integer Implements IList(Of String).IndexOf
            Return values.IndexOf(item)
        End Function
        Public Sub Insert(index As Integer, item As String) Implements IList(Of String).Insert
            values.Insert(index, item)
        End Sub
        Public Sub RemoveAt(index As Integer) Implements IList(Of String).RemoveAt
            values.RemoveAt(index)
        End Sub

        Private Sub AssertValid(index As Integer)
            If c.[Option] Is Nothing Then
                Throw New InvalidOperationException("OptionContext.Option is null.")
            End If
            If index >= c.[Option].MaxValueCount Then
                Throw New ArgumentOutOfRangeException("index")
            End If
            If c.[Option].OptionValueType = OptionValueType.Required AndAlso index >= values.Count Then
                Throw New OptionException(String.Format(c.OptionSet.MessageLocalizer("Missing required value for option '{0}'."), c.OptionName), c.OptionName)
            End If
        End Sub

        Default Public Property Item(index As Integer) As String Implements IList(Of String).Item
            Get
                AssertValid(index)
                Return If(index >= values.Count, Nothing, values(index))
            End Get
            Set
                values(index) = Value
            End Set
        End Property
#End Region

        Public Function ToList() As List(Of String)
            Return New List(Of String)(values)
        End Function

        Public Function ToArray() As String()
            Return values.ToArray()
        End Function

        Public Overrides Function ToString() As String
            Return String.Join(", ", values.ToArray())
        End Function

    End Class

    Public Class OptionContext
        Private m_option As [Option]
        Private name As String
        Private index As Integer
        Private [set] As OptionSet
        Private c As OptionValueCollection

        Public Sub New([set] As OptionSet)
            Me.[set] = [set]
            Me.c = New OptionValueCollection(Me)
        End Sub

        Public Property [Option]() As [Option]
            Get
                Return m_option
            End Get
            Set
                m_option = Value
            End Set
        End Property

        Public Property OptionName() As String
            Get
                Return name
            End Get
            Set
                name = Value
            End Set
        End Property

        Public Property OptionIndex() As Integer
            Get
                Return index
            End Get
            Set
                index = Value
            End Set
        End Property

        Public ReadOnly Property OptionSet() As OptionSet
            Get
                Return [set]
            End Get
        End Property

        Public ReadOnly Property OptionValues() As OptionValueCollection
            Get
                Return c
            End Get
        End Property
    End Class

    Public Enum OptionValueType
        None
        [Optional]
        Required
    End Enum

    Public MustInherit Class [Option]
        Private m_prototype As String, m_description As String
        Private m_names As String()
        Private type As OptionValueType
        Private count As Integer
        Private separators As String()

        Protected Sub New(prototype As String, description As String)
            Me.New(prototype, description, 1)
        End Sub

        Protected Sub New(prototype As String, description As String, maxValueCount As Integer)
            If prototype Is Nothing Then
                Throw New ArgumentNullException("prototype")
            End If
            If prototype.Length = 0 Then
                Throw New ArgumentException("Cannot be the empty string.", "prototype")
            End If
            If maxValueCount < 0 Then
                Throw New ArgumentOutOfRangeException("maxValueCount")
            End If

            Me.m_prototype = prototype
            Me.m_names = prototype.Split("|"c)
            Me.m_description = description
            Me.count = maxValueCount
            Me.type = ParsePrototype()

            If Me.count = 0 AndAlso type <> OptionValueType.None Then
                Throw New ArgumentException("Cannot provide maxValueCount of 0 for OptionValueType.Required or " & "OptionValueType.Optional.", "maxValueCount")
            End If
            If Me.type = OptionValueType.None AndAlso maxValueCount > 1 Then
                Throw New ArgumentException(String.Format("Cannot provide maxValueCount of {0} for OptionValueType.None.", maxValueCount), "maxValueCount")
            End If
            If Array.IndexOf(m_names, "<>") >= 0 AndAlso ((m_names.Length = 1 AndAlso Me.type <> OptionValueType.None) OrElse (m_names.Length > 1 AndAlso Me.MaxValueCount > 1)) Then
                Throw New ArgumentException("The default option handler '<>' cannot require values.", "prototype")
            End If
        End Sub

        Public ReadOnly Property Prototype() As String
            Get
                Return m_prototype
            End Get
        End Property
        Public ReadOnly Property Description() As String
            Get
                Return m_description
            End Get
        End Property
        Public ReadOnly Property OptionValueType() As OptionValueType
            Get
                Return type
            End Get
        End Property
        Public ReadOnly Property MaxValueCount() As Integer
            Get
                Return count
            End Get
        End Property

        Public Function GetNames() As String()
            Return DirectCast(m_names.Clone(), String())
        End Function

        Public Function GetValueSeparators() As String()
            If separators Is Nothing Then
                Return New String(-1) {}
            End If
            Return DirectCast(separators.Clone(), String())
        End Function

        Protected Shared Function Parse(Of T)(value As String, c As OptionContext) As T
            Dim conv As TypeConverter = TypeDescriptor.GetConverter(GetType(T))
            Dim obj As T = Nothing
            Try
                If value IsNot Nothing Then
                    obj = DirectCast(conv.ConvertFromString(value), T)
                End If
            Catch e As Exception
                Throw New OptionException(String.Format(c.OptionSet.MessageLocalizer("Could not convert string `{0}' to type {1} for option `{2}'."), value, GetType(T).Name, c.OptionName), c.OptionName, e)
            End Try
            Return obj
        End Function

        Friend ReadOnly Property Names() As String()
            Get
                Return m_names
            End Get
        End Property
        Friend ReadOnly Property ValueSeparators() As String()
            Get
                Return separators
            End Get
        End Property

        Shared ReadOnly NameTerminator As Char() = New Char() {"="c, ":"c}

        Private Function ParsePrototype() As OptionValueType
            Dim type As Char = ControlChars.NullChar
            Dim seps As New List(Of String)()
            For i As Integer = 0 To m_names.Length - 1
                Dim name As String = m_names(i)
                If name.Length = 0 Then
                    Throw New ArgumentException("Empty option names are not supported.", "prototype")
                End If

                Dim [end] As Integer = name.IndexOfAny(NameTerminator)
                If [end] = -1 Then
                    Continue For
                End If
                m_names(i) = name.Substring(0, [end])
                If type = ControlChars.NullChar OrElse type = name([end]) Then
                    type = name([end])
                Else
                    Throw New ArgumentException(String.Format("Conflicting option types: '{0}' vs. '{1}'.", type, name([end])), "prototype")
                End If
                AddSeparators(name, [end], seps)
            Next

            If type = ControlChars.NullChar Then
                Return OptionValueType.None
            End If

            If count <= 1 AndAlso seps.Count <> 0 Then
                Throw New ArgumentException(String.Format("Cannot provide key/value separators for Options taking {0} value(s).", count), "prototype")
            End If
            If count > 1 Then
                If seps.Count = 0 Then
                    Me.separators = New String() {":", "="}
                ElseIf seps.Count = 1 AndAlso seps(0).Length = 0 Then
                    Me.separators = Nothing
                Else
                    Me.separators = seps.ToArray()
                End If
            End If

            Return If(type = "="c, OptionValueType.Required, OptionValueType.[Optional])
        End Function

        Private Shared Sub AddSeparators(name As String, [end] As Integer, seps As ICollection(Of String))
            Dim start As Integer = -1
            For i As Integer = [end] + 1 To name.Length - 1
                Select Case name(i)
                    Case "{"c
                        If start <> -1 Then
                            Throw New ArgumentException(String.Format("Ill-formed name/value separator found in ""{0}"".", name), "prototype")
                        End If
                        start = i + 1
                        Exit Select
                    Case "}"c
                        If start = -1 Then
                            Throw New ArgumentException(String.Format("Ill-formed name/value separator found in ""{0}"".", name), "prototype")
                        End If
                        seps.Add(name.Substring(start, i - start))
                        start = -1
                        Exit Select
                    Case Else
                        If start = -1 Then
                            seps.Add(name(i).ToString())
                        End If
                        Exit Select
                End Select
            Next
            If start <> -1 Then
                Throw New ArgumentException(String.Format("Ill-formed name/value separator found in ""{0}"".", name), "prototype")
            End If
        End Sub

        Public Sub Invoke(c As OptionContext)
            OnParseComplete(c)
            c.OptionName = Nothing
            c.[Option] = Nothing
            c.OptionValues.Clear()
        End Sub

        Protected MustOverride Sub OnParseComplete(c As OptionContext)

        Public Overrides Function ToString() As String
            Return Prototype
        End Function
    End Class

    <Serializable>
    Public Class OptionException
        Inherits Exception
        Private [option] As String

        Public Sub New()
        End Sub

        Public Sub New(message As String, optionName As String)
            MyBase.New(message)
            Me.[option] = optionName
        End Sub

        Public Sub New(message As String, optionName As String, innerException As Exception)
            MyBase.New(message, innerException)
            Me.[option] = optionName
        End Sub

        Protected Sub New(info As SerializationInfo, context As StreamingContext)
            MyBase.New(info, context)
            Me.[option] = info.GetString("OptionName")
        End Sub

        Public ReadOnly Property OptionName() As String
            Get
                Return Me.[option]
            End Get
        End Property

        <SecurityPermission(SecurityAction.LinkDemand, SerializationFormatter:=True)>
        Public Overrides Sub GetObjectData(info As SerializationInfo, context As StreamingContext)
            MyBase.GetObjectData(info, context)
            info.AddValue("OptionName", [option])
        End Sub
    End Class

    Public Delegate Sub OptionAction(Of TKey, TValue)(key As TKey, value As TValue)

    Public Class OptionSet
        Inherits KeyedCollection(Of String, [Option])
        Public Sub New()
            Me.New(Function(f As String) f)
        End Sub

        Public Sub New(localizer As Converter(Of String, String))
            Me.localizer = localizer
        End Sub

        Private localizer As Converter(Of String, String)

        Public ReadOnly Property MessageLocalizer() As Converter(Of String, String)
            Get
                Return localizer
            End Get
        End Property

        Protected Overrides Function GetKeyForItem(item As [Option]) As String
            If item Is Nothing Then
                Throw New ArgumentNullException("option")
            End If
            If item.Names IsNot Nothing AndAlso item.Names.Length > 0 Then
                Return item.Names(0)
            End If
            ' This should never happen, as it's invalid for Option to be
            ' constructed w/o any names.
            Throw New InvalidOperationException("Option has no names!")
        End Function

        <Obsolete("Use KeyedCollection.this[string]")>
        Protected Function GetOptionForName([option] As String) As [Option]
            If [option] Is Nothing Then
                Throw New ArgumentNullException("option")
            End If
            Try
                Return MyBase.Item([option])
            Catch generatedExceptionName As KeyNotFoundException
                Return Nothing
            End Try
        End Function

        Protected Overrides Sub InsertItem(index As Integer, item As [Option])
            MyBase.InsertItem(index, item)
            AddImpl(item)
        End Sub

        Protected Overrides Sub RemoveItem(index As Integer)
            MyBase.RemoveItem(index)
            Dim p As [Option] = Items(index)
            ' KeyedCollection.RemoveItem() handles the 0th item
            For i As Integer = 1 To p.Names.Length - 1
                Dictionary.Remove(p.Names(i))
            Next
        End Sub

        Protected Overrides Sub SetItem(index As Integer, item As [Option])
            MyBase.SetItem(index, item)
            RemoveItem(index)
            AddImpl(item)
        End Sub

        Private Sub AddImpl([option] As [Option])
            If [option] Is Nothing Then
                Throw New ArgumentNullException("option")
            End If
            Dim added As New List(Of String)([option].Names.Length)
            Try
                ' KeyedCollection.InsertItem/SetItem handle the 0th name.
                For i As Integer = 1 To [option].Names.Length - 1
                    Dictionary.Add([option].Names(i), [option])
                    added.Add([option].Names(i))
                Next
            Catch generatedExceptionName As Exception
                For Each name As String In added
                    Dictionary.Remove(name)
                Next
                Throw
            End Try
        End Sub

        Public Overloads Function Add([option] As [Option]) As OptionSet
            MyBase.Add([option])
            Return Me
        End Function

        Private NotInheritable Class ActionOption
            Inherits [Option]
            Private action As Action(Of OptionValueCollection)

            Public Sub New(prototype As String, description As String, count As Integer, action As Action(Of OptionValueCollection))
                MyBase.New(prototype, description, count)
                If action Is Nothing Then
                    Throw New ArgumentNullException("action")
                End If
                Me.action = action
            End Sub

            Protected Overrides Sub OnParseComplete(c As OptionContext)
                action(c.OptionValues)
            End Sub
        End Class

        Public Overloads Function Add(prototype As String, action As Action(Of String)) As OptionSet
            Return Add(prototype, Nothing, action)
        End Function

        Public Overloads Function Add(prototype As String, description As String, action As Action(Of String)) As OptionSet
            If action Is Nothing Then
                Throw New ArgumentNullException("action")
            End If
            Dim p As [Option] = New ActionOption(prototype, description, 1, Sub(v As OptionValueCollection) action(v(0)))
            MyBase.Add(p)
            Return Me
        End Function

        Public Overloads Function Add(prototype As String, action As OptionAction(Of String, String)) As OptionSet
            Return Add(prototype, Nothing, action)
        End Function

        Public Overloads Function Add(prototype As String, description As String, action As OptionAction(Of String, String)) As OptionSet
            If action Is Nothing Then
                Throw New ArgumentNullException("action")
            End If
            Dim p As [Option] = New ActionOption(prototype, description, 2, Sub(v As OptionValueCollection) action(v(0), v(1)))
            MyBase.Add(p)
            Return Me
        End Function

        Private NotInheritable Class ActionOption(Of T)
            Inherits [Option]
            Private action As Action(Of T)

            Public Sub New(prototype As String, description As String, action As Action(Of T))
                MyBase.New(prototype, description, 1)
                If action Is Nothing Then
                    Throw New ArgumentNullException("action")
                End If
                Me.action = action
            End Sub

            Protected Overrides Sub OnParseComplete(c As OptionContext)
                action(Parse(Of T)(c.OptionValues(0), c))
            End Sub
        End Class

        Private NotInheritable Class ActionOption(Of TKey, TValue)
            Inherits [Option]
            Private action As OptionAction(Of TKey, TValue)

            Public Sub New(prototype As String, description As String, action As OptionAction(Of TKey, TValue))
                MyBase.New(prototype, description, 2)
                If action Is Nothing Then
                    Throw New ArgumentNullException("action")
                End If
                Me.action = action
            End Sub

            Protected Overrides Sub OnParseComplete(c As OptionContext)
                action(Parse(Of TKey)(c.OptionValues(0), c), Parse(Of TValue)(c.OptionValues(1), c))
            End Sub
        End Class

        Public Overloads Function Add(Of T)(prototype As String, action As Action(Of T)) As OptionSet
            Return Add(prototype, Nothing, action)
        End Function

        Public Overloads Function Add(Of T)(prototype As String, description As String, action As Action(Of T)) As OptionSet
            Return Add(New ActionOption(Of T)(prototype, description, action))
        End Function

        Public Overloads Function Add(Of TKey, TValue)(prototype As String, action As OptionAction(Of TKey, TValue)) As OptionSet
            Return Add(prototype, Nothing, action)
        End Function

        Public Overloads Function Add(Of TKey, TValue)(prototype As String, description As String, action As OptionAction(Of TKey, TValue)) As OptionSet
            Return Add(New ActionOption(Of TKey, TValue)(prototype, description, action))
        End Function

        Protected Overridable Function CreateOptionContext() As OptionContext
            Return New OptionContext(Me)
        End Function

#If LINQ Then
		Public Function Parse(arguments As IEnumerable(Of String)) As List(Of String)
			Dim process As Boolean = True
			Dim c As OptionContext = CreateOptionContext()
			c.OptionIndex = -1
			Dim def = GetOptionForName("<>")
			Dim unprocessed__1 = From argument In arguments Where If(System.Threading.Interlocked.Increment(c.OptionIndex) >= 0 AndAlso (process OrElse def IsNot Nothing), If(process, If(argument = "--", (InlineAssignHelper(process, False)), If(Not Parse(argument, c), If(def IsNot Nothing, Unprocessed(Nothing, def, c, argument), True), False)), If(def IsNot Nothing, Unprocessed(Nothing, def, c, argument), True)), True)argument
			Dim r As List(Of String) = unprocessed__1.AsList()
			If c.[Option] IsNot Nothing Then
				c.[Option].Invoke(c)
			End If
			Return r
		End Function
#Else
        Public Function Parse(arguments As IEnumerable(Of String)) As List(Of String)
            Dim c As OptionContext = CreateOptionContext()
            c.OptionIndex = -1
            Dim process As Boolean = True
            Dim unprocessed__1 As New List(Of String)()
            Dim def As [Option] = If(Contains("<>"), Me("<>"), Nothing)
            For Each argument As String In arguments
                c.OptionIndex += 1
                If argument = "--" Then
                    process = False
                    Continue For
                End If
                If Not process Then
                    Unprocessed(unprocessed__1, def, c, argument)
                    Continue For
                End If
                If Not Parse(argument, c) Then
                    Unprocessed(unprocessed__1, def, c, argument)
                End If
            Next
            If c.[Option] IsNot Nothing Then
                c.[Option].Invoke(c)
            End If
            Return unprocessed__1
        End Function
#End If

        Private Shared Function Unprocessed(extra As ICollection(Of String), def As [Option], c As OptionContext, argument As String) As Boolean
            If def Is Nothing Then
                extra.Add(argument)
                Return False
            End If
            c.OptionValues.Add(argument)
            c.[Option] = def
            c.[Option].Invoke(c)
            Return False
        End Function

        Private ReadOnly ValueOption As New Regex("^(?<flag>--|-|/)(?<name>[^:=]+)((?<sep>[:=])(?<value>.*))?$")

        Protected Function GetOptionParts(argument As String, ByRef flag As String, ByRef name As String, ByRef sep As String, ByRef value As String) As Boolean
            If argument Is Nothing Then
                Throw New ArgumentNullException("argument")
            End If

            flag = Nothing
            name = Nothing
            sep = Nothing
            value = Nothing

            Dim m As Match = ValueOption.Match(argument)
            If Not m.Success Then
                Return False
            End If
            flag = m.Groups("flag").Value
            name = m.Groups("name").Value
            If m.Groups("sep").Success AndAlso m.Groups("value").Success Then
                sep = m.Groups("sep").Value
                value = m.Groups("value").Value
            End If
            Return True
        End Function

        Protected Overridable Function Parse(argument As String, c As OptionContext) As Boolean
            If c.[Option] IsNot Nothing Then
                ParseValue(argument, c)
                Return True
            End If

#Disable Warning

            Dim f As String, n As String, s As String, v As String
            If Not GetOptionParts(argument, f, n, s, v) Then
                Return False
            End If

#Enable Warning

            Dim p As [Option]
            If Contains(n) Then
                p = Me(n)
                c.OptionName = f & n
                c.[Option] = p
                Select Case p.OptionValueType
                    Case OptionValueType.None
                        c.OptionValues.Add(n)
                        c.[Option].Invoke(c)
                        Exit Select
                    Case OptionValueType.[Optional], OptionValueType.Required
                        ParseValue(v, c)
                        Exit Select
                End Select
                Return True
            End If
            ' no match; is it a bool option?
            If ParseBool(argument, n, c) Then
                Return True
            End If
            ' is it a bundled option?
            If ParseBundledValue(f, String.Concat(n & s & v), c) Then
                Return True
            End If

            Return False
        End Function

        Private Sub ParseValue([option] As String, c As OptionContext)
            If [option] IsNot Nothing Then
                For Each o As String In If(c.[Option].ValueSeparators IsNot Nothing, [option].Split(c.[Option].ValueSeparators, StringSplitOptions.None), New String() {[option]})
                    c.OptionValues.Add(o)
                Next
            End If
            If c.OptionValues.Count = c.[Option].MaxValueCount OrElse c.[Option].OptionValueType = OptionValueType.[Optional] Then
                c.[Option].Invoke(c)
            ElseIf c.OptionValues.Count > c.[Option].MaxValueCount Then
                Throw New OptionException(localizer(String.Format("Error: Found {0} option values when expecting {1}.", c.OptionValues.Count, c.[Option].MaxValueCount)), c.OptionName)
            End If
        End Sub

        Private Function ParseBool([option] As String, n As String, c As OptionContext) As Boolean
            Dim p As [Option]
            Dim rn As New Value(Of String)

            If n.Length >= 1 AndAlso (n(n.Length - 1) = "+"c OrElse n(n.Length - 1) = "-"c) AndAlso Contains(rn = n.Substring(0, n.Length - 1)) Then
                p = Me(+rn)
                Dim v As String = If(n(n.Length - 1) = "+"c, [option], Nothing)
                c.OptionName = [option]
                c.[Option] = p
                c.OptionValues.Add(v)
                p.Invoke(c)
                Return True
            Else
                Return False
            End If
        End Function

        Private Function ParseBundledValue(f As String, n As String, c As OptionContext) As Boolean
            If f <> "-" Then
                Return False
            End If
            For i As Integer = 0 To n.Length - 1
                Dim p As [Option]
                Dim opt As String = f & n(i).ToString()
                Dim rn As String = n(i).ToString()
                If Not Contains(rn) Then
                    If i = 0 Then
                        Return False
                    End If
                    Throw New OptionException(String.Format(localizer("Cannot bundle unregistered option '{0}'."), opt), opt)
                End If
                p = Me(rn)
                Select Case p.OptionValueType
                    Case OptionValueType.None
                        Invoke(c, opt, n, p)
                        Exit Select
                    Case OptionValueType.[Optional], OptionValueType.Required
                        If True Then
                            Dim v As String = n.Substring(i + 1)
                            c.[Option] = p
                            c.OptionName = opt
                            ParseValue(If(v.Length <> 0, v, Nothing), c)
                            Return True
                        End If
                    Case Else
                        Throw New InvalidOperationException("Unknown OptionValueType: " & Convert.ToString(p.OptionValueType))
                End Select
            Next
            Return True
        End Function

        Private Shared Sub Invoke(c As OptionContext, name As String, value As String, [option] As [Option])
            c.OptionName = name
            c.[Option] = [option]
            c.OptionValues.Add(value)
            [option].Invoke(c)
        End Sub

        Private Const OptionWidth As Integer = 29

        Public Sub WriteOptionDescriptions(o As TextWriter)
            For Each p As [Option] In Me
                Dim written As Integer = 0
                If Not WriteOptionPrototype(o, p, written) Then
                    Continue For
                End If

                If written < OptionWidth Then
                    o.Write(New String(" "c, OptionWidth - written))
                Else
                    o.WriteLine()
                    o.Write(New String(" "c, OptionWidth))
                End If

                Dim lines As List(Of String) = GetLines(localizer(GetDescription(p.Description)))
                o.WriteLine(lines(0))
                Dim prefix As New String(" "c, OptionWidth + 2)
                For i As Integer = 1 To lines.Count - 1
                    o.Write(prefix)
                    o.WriteLine(lines(i))
                Next
            Next
        End Sub

        Private Function WriteOptionPrototype(o As TextWriter, p As [Option], ByRef written As Integer) As Boolean
            Dim names As String() = p.Names

            Dim i As Integer = GetNextOptionIndex(names, 0)
            If i = names.Length Then
                Return False
            End If

            If names(i).Length = 1 Then
                Write(o, written, "  -")
                Write(o, written, names(0))
            Else
                Write(o, written, "      --")
                Write(o, written, names(0))
            End If

            i = GetNextOptionIndex(names, i + 1)
            While i < names.Length
                Write(o, written, ", ")
                Write(o, written, If(names(i).Length = 1, "-", "--"))
                Write(o, written, names(i))
                i = GetNextOptionIndex(names, i + 1)
            End While

            If p.OptionValueType = OptionValueType.[Optional] OrElse p.OptionValueType = OptionValueType.Required Then
                If p.OptionValueType = OptionValueType.[Optional] Then
                    Write(o, written, localizer("["))
                End If
                Write(o, written, localizer("=" & GetArgumentName(0, p.MaxValueCount, p.Description)))
                Dim sep As String = If(p.ValueSeparators IsNot Nothing AndAlso p.ValueSeparators.Length > 0, p.ValueSeparators(0), " ")
                For c As Integer = 1 To p.MaxValueCount - 1
                    Write(o, written, localizer(sep & GetArgumentName(c, p.MaxValueCount, p.Description)))
                Next
                If p.OptionValueType = OptionValueType.[Optional] Then
                    Write(o, written, localizer("]"))
                End If
            End If
            Return True
        End Function

        Private Shared Function GetNextOptionIndex(names As String(), i As Integer) As Integer
            While i < names.Length AndAlso names(i) = "<>"
                i += 1
            End While
            Return i
        End Function

        Private Shared Sub Write(o As TextWriter, ByRef n As Integer, s As String)
            n += s.Length
            o.Write(s)
        End Sub

        Private Shared Function GetArgumentName(index As Integer, maxIndex As Integer, description As String) As String
            If description Is Nothing Then
                Return If(maxIndex = 1, "VALUE", "VALUE" & (index + 1))
            End If
            Dim nameStart As String()
            If maxIndex = 1 Then
                nameStart = New String() {"{0:", "{"}
            Else
                nameStart = New String() {"{" & index & ":"}
            End If
            For i As Integer = 0 To nameStart.Length - 1
                Dim start As Integer, j As Integer = 0
                Do
                    start = description.IndexOf(nameStart(i), j)
                Loop While If(start >= 0 AndAlso j <> 0, description(System.Math.Max(System.Threading.Interlocked.Increment(j), j - 1) - 1) = "{"c, False)
                If start = -1 Then
                    Continue For
                End If
                Dim [end] As Integer = description.IndexOf("}", start)
                If [end] = -1 Then
                    Continue For
                End If
                Return description.Substring(start + nameStart(i).Length, [end] - start - nameStart(i).Length)
            Next
            Return If(maxIndex = 1, "VALUE", "VALUE" & (index + 1))
        End Function

        Private Shared Function GetDescription(description As String) As String
            If description Is Nothing Then
                Return String.Empty
            End If
            Dim sb As New StringBuilder(description.Length)
            Dim start As Integer = -1
            For i As Integer = 0 To description.Length - 1
                Select Case description(i)
                    Case "{"c
                        If i = start Then
                            sb.Append("{"c)
                            start = -1
                        ElseIf start < 0 Then
                            start = i + 1
                        End If
                        Exit Select
                    Case "}"c
                        If start < 0 Then
                            If (i + 1) = description.Length OrElse description(i + 1) <> "}"c Then
                                Throw New InvalidOperationException("Invalid option description: " & description)
                            End If
                            i += 1
                            sb.Append("}")
                        Else
                            sb.Append(description.Substring(start, i - start))
                            start = -1
                        End If
                        Exit Select
                    Case ":"c
                        If start < 0 Then
                            GoTo case_default
                        End If
                        start = i + 1
                        Exit Select
                    Case Else
                        If start < 0 Then
case_default:               sb.Append(description(i))
                        End If
                        Exit Select
                End Select
            Next
            Return sb.ToString()
        End Function

        Private Shared Function GetLines(description As String) As List(Of String)
            Dim lines As New List(Of String)()
            If String.IsNullOrEmpty(description) Then
                lines.Add(String.Empty)
                Return lines
            End If
            Dim length As Integer = 80 - OptionWidth - 2
            Dim start As Integer = 0, [end] As Integer
            Do
                [end] = GetLineEnd(start, length, description)
                Dim cont As Boolean = False
                If [end] < description.Length Then
                    Dim c As Char = description([end])
                    If c = "-"c OrElse (Char.IsWhiteSpace(c) AndAlso c <> ControlChars.Lf) Then
                        [end] += 1
                    ElseIf c <> ControlChars.Lf Then
                        cont = True
                        [end] -= 1
                    End If
                End If
                lines.Add(description.Substring(start, [end] - start))
                If cont Then
                    lines(lines.Count - 1) += "-"
                End If
                start = [end]
                If start < description.Length AndAlso description(start) = ControlChars.Lf Then
                    start += 1
                End If
            Loop While [end] < description.Length
            Return lines
        End Function

        Private Shared Function GetLineEnd(start As Integer, length As Integer, description As String) As Integer
            Dim [end] As Integer = Math.Min(start + length, description.Length)
            Dim sep As Integer = -1
            For i As Integer = start To [end] - 1
                Select Case description(i)
                    Case " "c, ControlChars.Tab, ControlChars.VerticalTab, "-"c, ","c, "."c,
                        ";"c
                        sep = i
                        Exit Select
                    Case ControlChars.Lf
                        Return i
                End Select
            Next
            If sep = -1 OrElse [end] = description.Length Then
                Return [end]
            End If
            Return sep
        End Function
    End Class
End Namespace
