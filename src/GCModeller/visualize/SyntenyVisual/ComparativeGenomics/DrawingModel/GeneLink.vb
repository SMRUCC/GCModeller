Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization

Namespace ComparativeGenomics

    ''' <summary>
    ''' 两个基因组之间的相互共同的基因
    ''' </summary>
    ''' <remarks></remarks>
    Public Class GeneLink

        <XmlAttribute> Public Property genome1 As String
        <XmlAttribute> Public Property genome2 As String
        <XmlAttribute> Public Property Score As Double

        <XmlElement>
        Public Property Color As Color
        <XmlText>
        Public Property annotation As String

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return String.Format("{0} === {1};   //{2}", genome1, genome2, annotation)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overloads Function Equals(id1 As String, id2 As String) As Boolean
            Dim test1 As Boolean =
                String.Equals(id1, genome1, StringComparison.OrdinalIgnoreCase) AndAlso
                String.Equals(id2, genome2, StringComparison.OrdinalIgnoreCase)
            Dim test2 As Boolean =
                String.Equals(id2, genome1, StringComparison.OrdinalIgnoreCase) AndAlso
                String.Equals(id1, genome2, StringComparison.OrdinalIgnoreCase)

            Return test1 OrElse test2
        End Function
    End Class
End Namespace