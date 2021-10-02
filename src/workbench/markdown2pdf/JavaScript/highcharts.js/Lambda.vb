#Region "Microsoft.VisualBasic::3b1f5d9783c0e93ba59731f35f4ca85b, markdown2pdf\JavaScript\highcharts.js\Lambda.vb"

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

    ' Class LambdaWriter
    ' 
    '     Function: CanConvert, ReadJson, StripLambda
    ' 
    '     Sub: WriteJson
    ' 
    ' Class Lambda
    ' 
    '     Properties: [function], args
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Reflection
Imports System.Text
Imports Newtonsoft.Json
Imports r = System.Text.RegularExpressions.Regex

''' <summary>
''' 
''' </summary>
''' <remarks>
''' https://stackoverflow.com/questions/11934487/custom-json-serialization-of-class
''' </remarks>
Public Class LambdaWriter : Inherits JsonConverter

    Public Overrides Sub WriteJson(writer As JsonWriter, value As Object, serializer As JsonSerializer)
        If value Is Nothing Then
            Call serializer.Serialize(writer, Nothing)
        End If

        Dim lambda$ = Nothing
        Dim properties = value _
            .GetType _
            .GetProperties _
            .Where(Function(p)
                       Return p.PropertyType Is GetType(Lambda)
                   End Function)

        Call writer.WriteStartObject()

        For Each [property] As PropertyInfo In properties
            ' write property name
            Call writer.WritePropertyName([property].Name)
            ' let the serializer serialize the value itself
            ' (so this converter will work with any other type, Not just int)
            Call lambda.SetValue(TryCast([property].GetValue(value, Nothing), Lambda)?.ToString())
            Call serializer.Serialize(writer, lambda)
        Next

        Call writer.WriteEndObject()
    End Sub

    Public Overrides Function ReadJson(reader As JsonReader, objectType As Type, existingValue As Object, serializer As JsonSerializer) As Object
        Throw New NotImplementedException()
    End Function

    Public Overrides Function CanConvert(objectType As Type) As Boolean
        If objectType Is GetType(Lambda) Then
            Return True
        Else
            Return False
        End If
    End Function

    Const LambdaPattern$ = Lambda.DeliStart & ".+?" & Lambda.DeliEnds

    Public Shared Function StripLambda(json As String) As String
        Dim matches = r.Matches(json, LambdaPattern, RegexICSng).ToArray
        Dim out As New StringBuilder(json)
        Dim replaceValue$

        For Each match As String In matches
            replaceValue = match _
                .Replace(Lambda.DeliStart, "") _
                .Replace(Lambda.DeliEnds, "") _
                .Replace("\r", vbCr) _
                .Replace("\n", vbLf)
            match = $"""{match}"""

            out.Replace(match, replaceValue)
        Next

        Return out.ToString
    End Function
End Class

Public Class Lambda

    Public Property args As String()
    Public Property [function] As String

    Friend Const DeliStart$ = ";<<<"
    Friend Const DeliEnds$ = ";>>>>"

    Public Overrides Function ToString() As String
        Return $"{DeliStart} function({args.JoinBy(", ")}) {{
    {[function]}
}} {DeliEnds}"
    End Function
End Class
