Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Parser
Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace IO.Models

    Public Module TextParser

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function ParseXref(str As IEnumerable(Of String)) As Dictionary(Of String, String())
            Return str.SafeQuery _
                .Select(Function(si) si.GetTagValue(":", trim:=True)) _
                .GroupBy(Function(xr) xr.Name) _
                .ToDictionary(Function(xr) xr.Key,
                              Function(xr)
                                  Return xr.Select(Function(a) a.Value.StringReplace("[{].+[}]", "").Trim) _
                                      .Distinct _
                                      .ToArray
                              End Function)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function ParsePropertyValues(str As IEnumerable(Of String)) As Dictionary(Of String, NamedValue())
            Return str.SafeQuery _
                .Select(Function(si)
                            Dim tokens As String() = DelimiterParser.GetTokens(si)
                            Dim property_name As String = tokens(0)
                            Dim value As String = tokens(1)
                            Dim type As String = tokens(2)

                            Return (property_name, New NamedValue(type, value))
                        End Function) _
                .GroupBy(Function(pr) pr.property_name) _
                .ToDictionary(Function(pr) pr.Key,
                              Function(pr)
                                  Return pr.Select(Function(a) a.Item2).ToArray
                              End Function)
        End Function
    End Module
End Namespace