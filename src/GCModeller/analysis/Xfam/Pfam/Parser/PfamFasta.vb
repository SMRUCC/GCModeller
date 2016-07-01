Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports System.Text.RegularExpressions
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports System.Reflection
Imports SMRUCC.genomics.SequenceModel

Namespace PfamFastaComponentModels

    Public Class PfamFasta : Inherits PfamCommon
        Implements I_PolymerSequenceModel
        Implements IAbstractFastaToken

        Public Property Location As SMRUCC.genomics.ComponentModel.Loci.Location

        Public Shared Function CreateObject(FastaObject As FastaToken) As PfamFasta
            Dim FastaData = ParseHeadTitle(FastaObject.Title)
            FastaData.SequenceData = FastaObject.SequenceData
            Return FastaData
        End Function

        Const NULL_ERROR As String = "NULL_ERROR"

        Public Shared Function ParseHeadTitle(strValue As String) As PfamFasta
            Dim DataToken As String() = strValue.Split

            If DataToken.IsNullOrEmpty OrElse
                DataToken.GetElementCounts = 0 OrElse
                DataToken.Length < 2 Then

                Call $"NULL title tokens!!!  ----->   ""{strValue}""".__DEBUG_ECHO
                Return __internalCreateNULL
            Else
                Return __createObject(DataToken)
            End If
        End Function

        Const REGEX_PFAM_ENTRY As String = "PF(am)?\d+\.\d+;.+?;"

        ''' <summary>
        ''' 本方法仅仅解析出Pfam编号以及Pfam结构域的名称
        ''' </summary>
        ''' <param name="strValue"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function ParseEntry(strValue As String) As PfamFasta
            Dim s As String = Regex.Match(strValue, REGEX_PFAM_ENTRY, RegexOptions.IgnoreCase).Value
            If String.IsNullOrEmpty(s) Then
                Call $"NULL_ERROR: {strValue}   @{MethodBase.GetCurrentMethod.Name}".__DEBUG_ECHO
                Return __internalCreateNULL
            End If

            Dim Tokens As String() = Strings.Split(s, ";")
            Return New PfamFasta With {
                .PfamId = Tokens(0).Split("."c).First,
                .PfamCommonName = Tokens(1)
            }
        End Function

        Private Shared ReadOnly Property __internalCreateNULL As PfamFasta
            Get
                Return New PfamFasta With {
                    .ChainId = NULL_ERROR,
                    .Location = New SMRUCC.genomics.ComponentModel.Loci.Location(0, 0),
                    .PfamCommonName = NULL_ERROR,
                    .PfamId = NULL_ERROR,
                    .PfamIdAsub = NULL_ERROR,
                    .SequenceData = NULL_ERROR,
                    .Uniprot = NULL_ERROR,
                    .UniqueId = NULL_ERROR
                }
            End Get
        End Property

        Private Shared Function __createObject(data As String()) As PfamFasta
            Dim PfamFasta As PfamFasta = New PfamFasta

            Dim P1 As String = data(PfamCommon.P1)
            Dim P2 As String = data(PfamCommon.P2)
            Dim P3 As String = data(PfamCommon.P3)

            data = P1.Split(CChar("/"))
            PfamFasta.UniqueId = data.First
            PfamFasta.Location = ComponentModel.Loci.Location.CreateObject(data.Last, "-")

            data = P2.Split(CChar("."))
            PfamFasta.Uniprot = data.First
            PfamFasta.ChainId = data.Last

            data = P3.Split(CChar(";"))
            PfamFasta.PfamCommonName = data(1)
            data = data.First.Split(CChar("."))
            PfamFasta.PfamId = data.First
            PfamFasta.PfamIdAsub = data.Last

            Return PfamFasta
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("[{0}] {1}", PfamId, SequenceData)
        End Function

        Public Shared Function CreateCsvArchive(data As Generic.IEnumerable(Of PfamFasta)) As PfamCsvRow()
            Return (From FastaObject As PfamFasta
                    In data.AsParallel
                    Select PfamCsvRow.CreateObject(FastaObject)).ToArray
        End Function

        Public ReadOnly Property Title As String Implements SequenceModel.FASTA.IAbstractFastaToken.Title
            Get
                Return String.Format("{0}/{1}-{2} {3}.{4} {5}.{6};{7};", UniqueId, Location.Left, Location.Right, Uniprot, ChainId, PfamId, PfamIdAsub, PfamCommonName)
            End Get
        End Property

        Public Property Attributes As String() Implements IAbstractFastaToken.Attributes
    End Class
End Namespace