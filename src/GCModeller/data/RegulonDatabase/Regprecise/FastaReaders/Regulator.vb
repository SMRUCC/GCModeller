Imports System.Text.RegularExpressions
Imports SMRUCC.genomics.Assembly.KEGG.Archives
Imports SMRUCC.genomics.SequenceModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace Regprecise.FastaReaders

    ''' <summary>
    ''' 调控因子的蛋白质序列
    ''' > xcb:XC_1184|Family|Regulates|Regulog|Definition
    ''' </summary>
    Public Class Regulator : Inherits FASTA.FastaToken
        Implements sIdEnumerable

        ''' <summary>
        ''' &lt;(KEGG)species_code>:&lt;locusTag>
        ''' </summary>
        ''' <returns></returns>
        Public Property KEGG As String Implements sIdEnumerable.Identifier
            Get
                Return _kegg
            End Get
            Set(value As String)
                _kegg = value
                Dim Tokens As String() = _kegg.Split(":"c)
                _SpeciesCode = Tokens(Scan0)
                _LocusTag = Tokens(1)
            End Set
        End Property
        Public Property Sites As String()
        Public Property Family As String
        Public Property Regulog As String
        Public Property Definition As String

        Public ReadOnly Property SpeciesCode As String
        Public ReadOnly Property LocusTag As String

        Dim _kegg As String

        ''' <summary>
        ''' $"{<see cref="KEGG"/>}|{<see cref="Family"/>}|{<see cref="String.Join"/>(";", <see cref="Sites"/>)}|{<see cref="Regulog"/>}|{<see cref="Definition"/>}"
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Return $"{KEGG}|{Family}|{String.Join(";", Sites)}|{Regulog}|{Definition}"
        End Function

        Public Shared Function LoadDocument(FastaObject As FASTA.FastaToken) As Regulator
            Dim attributes As String() = FastaObject.Attributes
            Dim RegpreciseRegulator As Regulator =
                New Regulator With {
                    .KEGG = attributes(Scan0),
                    .Attributes = attributes,
                    .Family = attributes(1),
                    .Sites = Strings.Split(attributes(2), ";"),
                    .Regulog = attributes(3),
                    .SequenceData = FastaObject.SequenceData.ToUpper,
                    .Definition = attributes(4)
            }

            Return RegpreciseRegulator
        End Function

        Public Shared Function LoadDocument(FastaFile As FASTA.FastaFile) As Regulator()
            Dim LQuery As Regulator() = (From FastaObject As FASTA.FastaToken
                                         In FastaFile.AsParallel
                                         Select Regulator.LoadDocument(FastaObject)).ToArray
            Return LQuery
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="fasta">文件的路径</param>
        ''' <returns></returns>
        Public Shared Function LoadDocument(fasta As String) As Regulator()
            Dim File As FASTA.FastaFile = SequenceModel.FASTA.FastaFile.Read(fasta)
            Dim regulators As Regulator() = LoadDocument(File)
            Return regulators
        End Function

        Protected Friend Shared Function NullDictionary(uniqueId As String) As Regprecise.FastaReaders.Regulator
            Return Nothing
        End Function

        ''' <summary>
        ''' 在KEGG数据库之中所注释的家族分类
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property KEGGFamily As String
            Get
                Try
                    Return __KEGGFamily()
                Catch ex As Exception
                    Return Family
                End Try
            End Get
        End Property

        Private Function __KEGGFamily() As String
            Return SequenceDump.KEGGFamily(Definition, [default]:=Me.Family)
        End Function
    End Class
End Namespace