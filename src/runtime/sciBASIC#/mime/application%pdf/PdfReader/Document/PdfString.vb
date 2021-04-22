﻿Imports System

Namespace PdfReader
    Public Class PdfString
        Inherits PdfObject

        Public ReadOnly Property StrVal As String
            Get
                Return Value
            End Get
        End Property

        Public Sub New(ByVal parent As PdfObject, ByVal str As ParseString)
            MyBase.New(parent, str)
        End Sub

        Public Overrides Function ToString() As String
            Return Value
        End Function

        Public Overrides Sub Visit(ByVal visitor As IPdfObjectVisitor)
            visitor.Visit(Me)
        End Sub

        Public ReadOnly Property ParseString As ParseString
            Get
                Return TryCast(ParseObject, ParseString)
            End Get
        End Property

        Public ReadOnly Property Value As String
            Get
                Return Decrypt.DecodeString(Me)
            End Get
        End Property

        Public ReadOnly Property ValueAsBytes As Byte()
            Get
                Return Decrypt.DecodeStringAsBytes(Me)
            End Get
        End Property

        Public ReadOnly Property ValueAsDateTime As Date
            Get

                Try
                    Dim str = Value

                    If Not Equals(str, Nothing) AndAlso str.Length >= 4 Then
                        Dim index = 0
                        Dim length = str.Length

                        ' The 'D:' prefix is optional
                        If str(index) = "D"c AndAlso str(index + 1) = ":"c Then index += 2

                        ' Year is mandatory, all the others are optional
                        Dim YYYY = Integer.Parse(str.Substring(index, 4))
                        Dim MM = If(index + 4 < length, Integer.Parse(str.Substring(index + 4, 2)), 1)
                        Dim DD = If(index + 6 < length, Integer.Parse(str.Substring(index + 6, 2)), 1)
                        Dim HH = If(index + 7 < length, Integer.Parse(str.Substring(index + 8, 2)), 0)
                        Dim lMm = If(index + 10 < length, Integer.Parse(str.Substring(index + 10, 2)), 0)
                        Dim SS = If(index + 12 < length, Integer.Parse(str.Substring(index + 12, 2)), 0)
                        Dim O = If(index + 14 < length, str(index + 14), "Z"c)
                        Dim OHH = If(index + 15 < length, Integer.Parse(str.Substring(index + 15, 2)), 0)
                        Dim OSS = If(index + 18 < length, Integer.Parse(str.Substring(index + 18, 2)), 0)
                        Return New DateTime(YYYY, MM, DD, HH, lMm, SS, DateTimeKind.Utc)
                    Else
                        Throw New ApplicationException($"String '{Value}' cannot be converted to a date.")
                    End If

                Catch
                    Throw New ApplicationException($"String '{Value}' cannot be converted to a date.")
                End Try
            End Get
        End Property
    End Class
End Namespace
