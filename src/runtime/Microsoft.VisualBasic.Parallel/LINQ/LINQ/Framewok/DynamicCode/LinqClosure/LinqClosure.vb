Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Text

Namespace Framework.DynamicCode

    Public Module LinqClosure

        <Extension>
        Public Function [GetType](assm As Assembly) As Type
            Dim types As Type() = assm.GetTypes
            Dim Linq As Type = (From type As Type In types
                                Where String.Equals(type.Name, LinqClosure.Linq, StringComparison.Ordinal)
                                Select type).FirstOrDefault
            Return Linq
        End Function

        <Extension>
        Public Function GetProjectAbstract(assm As Assembly) As IProject
            Return AddressOf New __project(assm).Project
        End Function

        Private Class __project
            ReadOnly __typeINFO As Type
            ReadOnly __project As MethodInfo

            Sub New(assm As Assembly)
                __typeINFO = LinqClosure.[GetType](assm)
                __project = __typeINFO.GetMethod(LinqClosure.Project)
            End Sub

            Public Function Project(x As Object) As LinqValue
                Dim value As Object = __project.Invoke(Nothing, {x})
                Return DirectCast(value, LinqValue)
            End Function

            Public Overrides Function ToString() As String
                Return $"{__typeINFO.FullName}::{__project.ToString}"
            End Function
        End Class

        Const Linq As String = "Linq"
        Const Project As String = "Project"

        Public Function BuildClosure(x As String, type As Type, preLetClosures As IEnumerable(Of String), afterLetClosures As IEnumerable(Of String), projects As IEnumerable(Of String), Optional where As String = "") As String
            Return BuildClosure(x, type.FullName, preLetClosures, afterLetClosures, projects, where)
        End Function

        Public Function BuildClosure(x As String, type As String, preLetClosures As IEnumerable(Of String), afterLetClosures As IEnumerable(Of String), projects As IEnumerable(Of String), Optional where As String = "") As String
            Dim lBuilder As New StringBuilder()

            Call lBuilder.AppendLine("Namespace ___linqClosure")
            Call lBuilder.AppendLine()
            Call lBuilder.AppendLine($"  Public Class {LinqClosure.Linq}")
            Call lBuilder.AppendLine()

            Call lBuilder.AppendLine($"     Public Shared Function {Project}({x} As {type}) As {GetType(LinqValue).FullName}")
            Call lBuilder.AppendLine()

            If Not preLetClosures Is Nothing Then
                For Each line As String In preLetClosures
                    Call lBuilder.AppendLine("          Dim " & line)
                Next
                Call lBuilder.AppendLine()
            End If

            If Not String.IsNullOrEmpty(where) Then
                Call lBuilder.AppendLine($"          If Not {where} Then")
                Call lBuilder.AppendLine($"             Return {GetType(LinqValue).FullName}.Unavailable()")
                Call lBuilder.AppendLine("          End If")
                Call lBuilder.AppendLine()
            End If

            If Not afterLetClosures Is Nothing Then
                For Each line As String In afterLetClosures
                    Call lBuilder.AppendLine("          Dim " & line)
                Next
                Call lBuilder.AppendLine()
            End If

            Call lBuilder.AppendLine($"          Dim {LinqClosure.obj} As Object = " & __getProjects(projects))
            Call lBuilder.AppendLine($"          Return New {GetType(LinqValue).FullName}({LinqClosure.obj})")

            Call lBuilder.AppendLine("      End Function")
            Call lBuilder.AppendLine()

            Call lBuilder.AppendLine("  End Class")
            Call lBuilder.AppendLine("End Namespace")

            Return lBuilder.ToString
        End Function

        ''' <summary>
        ''' 名字复杂一些，避免在进行动态编译的时候的命名冲突
        ''' </summary>
        Const obj As String = "____x__EF53E4E8_897E_4282_AD81_2D6462990FCC__obj"

        Private Function __getProjects(source As IEnumerable(Of String)) As String
            Dim projects As String() = source.ToArray

            If projects.Length = 1 Then
                Return projects(Scan0)
            Else
                Dim anym As StringBuilder = New StringBuilder("New With {" & vbCrLf)
                Call anym.AppendLine(String.Join(", " & vbCrLf, projects.ToArray(Function(s) vbTab & __property(s))))
                Call anym.AppendLine(vbTab & "}")

                Return anym.ToString
            End If
        End Function

        Private Function __property(prop As String) As String
            Dim pos As Integer = InStr(prop, "=")

            If pos = 0 Then
                Return $".{prop} = {prop}"
            Else
                Dim name As String = Mid(prop, 1, pos - 1).Trim
                Dim value As String = Mid(prop, pos + 1).Trim
                Return $".{name} = {value}"
            End If
        End Function

        Public Delegate Function IProject(x As Object) As LinqValue
    End Module

    ''' <summary>
    ''' Example
    ''' </summary>
    Friend NotInheritable Class Linq

        ''' <summary>
        ''' From X As &lt;Type> In $source Let a = &lt;Expression> let b = &lt;Expression> Where &lt;Expression> Let c = &lt;Expression> Select X,a,b,c
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        Public Shared Function Project(x As Object) As LinqValue
            Dim a '= <Expression>
            Dim b '= <Expression> 

            If Not True Then
                Return LinqValue.Unavailable()
            End If

            Dim c '= <Expression> 

#Disable Warning
            Dim obj As Object = New With {.x = x, .A = a, .b = b, .c = c}
#Enable Warning

            Return New LinqValue(obj)
        End Function
    End Class
End Namespace