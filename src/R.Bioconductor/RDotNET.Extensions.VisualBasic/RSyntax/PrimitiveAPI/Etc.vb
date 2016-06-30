Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports RDotNET.Extensions.VisualBasic.RBase.Vectors

Namespace RBase

    <PackageNamespace("RBase.Read")>
    Public Module Read

        <ExportAPI("Read.Table")>
        Public Function Table(File As String, Optional Header As Boolean = False) As DocumentStream.DataFrame
            Throw New NotImplementedException
        End Function
    End Module

    <PackageNamespace("RBase.Is")>
    Public Module [Is]

        ''' <summary>
        ''' is.finite and is.infinite return a vector of the same length as x, indicating which elements are finite (not infinite and not missing) or infinite.
        ''' </summary>
        ''' <param name="x">R object to be tested: the default methods handle atomic vectors.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("is.finite")>
        Public Function Finite(x As Vector) As BooleanVector
            Return New BooleanVector((From n In x Select Not Double.IsInfinity(n)).ToArray)
        End Function

        <ExportAPI("is.NAN")>
        Public Function NAN(x As Vector) As BooleanVector
            Return New BooleanVector((From n In x Select Double.IsNaN(x)).ToArray)
        End Function

    End Module

    Namespace Machine

        <PackageNamespace("RBase.Machine.Double")>
        Public Module [Double]

            Public ReadOnly Property Eps As Double
                Get
                    Return Double.Epsilon
                End Get
            End Property
        End Module
    End Namespace
End Namespace