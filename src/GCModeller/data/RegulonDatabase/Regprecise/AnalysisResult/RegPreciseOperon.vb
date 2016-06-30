Imports LANS.SystemsBiology.ComponentModel.Loci
Imports Microsoft.VisualBasic.Serialization
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports System.Web.Script.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Regprecise

    ''' <summary>
    ''' Operon regulon model that reconstructed from the RegPrecise database.
    ''' (使用RegPrecise数据库重构出来的Regulon数据)
    ''' </summary>
    Public Class RegPreciseOperon

        ''' <summary>
        ''' Mapping from <see cref="TF_trace"/> by using protein ortholog analysis, such as ``bbh`` method.
        ''' </summary>
        ''' <returns></returns>
        Public Property Regulators As String()
        ''' <summary>
        ''' Active the regulation from <see cref="TF_trace"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property Effector As String
        Public Property Pathway As String
        ''' <summary>
        ''' Operon regulator TF
        ''' </summary>
        ''' <returns></returns>
        Public Property TF_trace As String
        Public Property BiologicalProcess As String
        Public Property source As String
        ''' <summary>
        ''' Operon members
        ''' </summary>
        ''' <returns></returns>
        Public Property Operon As String()
        Public Property Strand As String
        Public Property bbh As String()
            Get
                Return __bbh
            End Get
            Set(value As String())
                __bbh = value

                If __bbh Is Nothing Then
                    _bbhUID = ""
                Else
                    _bbhUID = String.Join(", ", value.OrderBy(Function(x) x).ToArray)
                End If
            End Set
        End Property

        Dim __bbh As String()

        ''' <summary>
        ''' Using for the CORN analysis or distinct
        ''' </summary>
        ''' <returns></returns>
        <ScriptIgnore>
        <Ignored>
        <XmlIgnore>
        Public ReadOnly Property bbhUID As String

        Sub New()
        End Sub

        ''' <summary>
        ''' Copy regulon definition from <see cref="Regulator"/>
        ''' </summary>
        ''' <param name="regulon"></param>
        ''' <param name="TF"></param>
        ''' <param name="members"></param>
        ''' <param name="cstrand"></param>
        ''' <param name="bbhHits"></param>
        Sub New(regulon As Regulator, TF As String(), members As String(), cstrand As String, bbhHits As String())
            Operon = members
            TF_trace = regulon.LocusId
            Regulators = TF
            Effector = regulon.Effector
            Pathway = regulon.Pathway
            BiologicalProcess = regulon.BiologicalProcess
            source = regulon.Regulog.Key
            Strand = cstrand
            bbh = bbhHits
        End Sub

        ''' <summary>
        ''' Copy regulon definition from <see cref="Regulator"/>
        ''' </summary>
        ''' <param name="regulon"></param>
        ''' <param name="TF"></param>
        ''' <param name="members"></param>
        ''' <param name="strand"></param>
        ''' <param name="bbhHits"></param>
        Sub New(regulon As Regulator, TF As String(), members As String(), strand As Strands, bbhHits As String())
            Call Me.New(regulon,
                        TF, members,
                        If(strand = Strands.Forward, "+", "-"),
                        bbhHits)
        End Sub

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace