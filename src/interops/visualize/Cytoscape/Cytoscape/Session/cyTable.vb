Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Session

    Public Class cyTable

        Public Property CyCSVVersion As String
        Public Property fields As cyField()

        Public ReadOnly Property [Dim] As Size
            Get
                Return New Size(fields.Length, fields(Scan0).data.Length)
            End Get
        End Property

        Default Public ReadOnly Property getField(name As String) As cyField
            Get
                Return fields.FirstOrDefault(Function(a) a.name = name)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return fields.Keys().GetJson
        End Function

        Public Shared Function LoadTable(path As String) As cyTable
            Dim table = File.Load(path)
            Dim version As String = table.Rows(Scan0)(1)
            Dim getFields As cyField() =
                Iterator Function() As IEnumerable(Of cyField)
                    For Each col In table.Columns()
                        Dim name As String = col(1)
                        Dim type As String = col(2)
                        Dim mutable As String = col(3)
                        Dim note As String = col(4)
                        Dim data As String() = col.Skip(5).ToArray

                        Yield New cyField With {
                            .name = name,
                            .type = type,
                            .mutable = mutable = NameOf(mutable),
                            .note = note,
                            .data = data
                        }
                    Next
                End Function().ToArray

            Return New cyTable With {
                .CyCSVVersion = version,
                .fields = getFields
            }
        End Function
    End Class

    Public Class cyField : Implements INamedValue

        Public Property name As String Implements INamedValue.Key
        Public Property type As String
        Public Property mutable As Boolean
        Public Property note As String
        Public Property data As String()

        Default Public ReadOnly Property Value(index As Integer) As String
            Get
                Return data(index)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Dim prefix = If(mutable, "Dim", "Const")

            Return $"{prefix} {name} As {type}[{note}] = {data.GetJson.Substring(0, 60)}..."
        End Function
    End Class
End Namespace


