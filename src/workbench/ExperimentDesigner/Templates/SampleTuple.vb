Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Data.csv

''' <summary>
''' Using for paired sample T-test
''' </summary>
<Template(ExperimentDesigner)>
Public Class SampleTuple

    <XmlAttribute> Public Property Sample1 As String
    <XmlAttribute> Public Property Sample2 As String

    ''' <summary>
    ''' Using this 
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Label As String
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return Sample1 & "/" & Sample2
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return $"{Sample1} vs {Sample2}"
    End Function
End Class
