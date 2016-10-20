Imports System.Reflection

Namespace Runtime.HybridsScripting

    Public Module EnvironmentParser

        Public Function [Imports](assm As System.Type) As EntryPoint
            Dim attributes As Object() = assm.GetCustomAttributes(LanguageEntryPoint.TypeInfo, True)

            If attributes.IsNullOrEmpty Then
                Return Nothing
            End If

            Dim InitEntry As MethodInfo = GetEntry(assm, InterfaceTypes.EntryPointInit)
            Dim Evaluate As MethodInfo = GetEntry(assm, InterfaceTypes.Evaluate)
            Dim SetValue As MethodInfo = GetEntry(assm, InterfaceTypes.SetValue)
            Dim DataConvertors = GetEntries(Of DataTransform)(assm)
            Dim ConservedString As MethodInfo = (From cMethod As KeyValuePair(Of DataTransform, MethodInfo)
                                                                   In DataConvertors
                                                 Where cMethod.Key.ReservedStringTLTR = True
                                                 Select cMethod.Value).FirstOrDefault
            If Evaluate Is Nothing Then
                Return Nothing
            Else
                Return New EntryPoint With {
                    .DeclaredAssemblyType = assm,
                    .ConservedString = ConservedString,
                    .Language = DirectCast(attributes(0), LanguageEntryPoint),
                    .Init = InitEntry,
                    .Evaluate = Evaluate,
                    .TypeFullName = assm.FullName,
                    .SetValue = SetValue,
                    .DataConvertors = New SortedDictionary(Of Char, MethodInfo)(
                        DataConvertors.ToDictionary(Function(item) item.Key.TypeChar,
                                                    Function(item) item.Value))
                }
            End If
        End Function

        Private Function GetEntries(Of TEntryType As EntryInterface)(TypeInfo As Type) As KeyValuePair(Of TEntryType, MethodInfo)()
            Dim EntryType As Type = GetType(TEntryType)
            Dim LQuery = (From LoadHandle As MethodInfo
                          In TypeInfo.GetMethods(BindingFlags.Public Or BindingFlags.Static)
                          Let attributes As Object() = LoadHandle.GetCustomAttributes(EntryType, False)
                          Where Not attributes.IsNullOrEmpty
                          Select (From attr As Object
                                  In attributes
                                  Let Entry As TEntryType = DirectCast(attr, TEntryType)
                                  Select New KeyValuePair(Of TEntryType, MethodInfo)(Entry, LoadHandle)).ToArray).ToArray
            Return LQuery.ToVector
        End Function

        Private Function GetEntry(TypeInfo As System.Type, EntryType As InterfaceTypes) As MethodInfo
            Dim LQuery = (From LoadHandle As MethodInfo
                          In TypeInfo.GetMethods(BindingFlags.Public Or BindingFlags.Static)
                          Let attributes As Object() = LoadHandle.GetCustomAttributes(EntryInterface.TypeInfo, False)
                          Where Not attributes.IsNullOrEmpty AndAlso
                              DirectCast(attributes(0), EntryInterface).InterfaceType = EntryType
                          Select LoadHandle).FirstOrDefault
            Return LQuery
        End Function
    End Module
End Namespace