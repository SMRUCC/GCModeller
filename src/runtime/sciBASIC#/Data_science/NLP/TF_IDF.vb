Imports System.IO
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language

Namespace org.ikit.nlp


    ''' <summary>
    ''' TF-IDF algorithm in Java. An explanation of this algorithm can be found at
    ''' 
    '''      http://en.wikipedia.org/wiki/Tf*idf
    ''' 
    ''' @author Bodong Chen <bodong.chen>
    ''' </summary>
    ''' <remarks>
    ''' **TF-IDF (Term Frequency-Inverse Document Frequency)** is a statistical measure that evaluates how relevant
    ''' a word is to a document in a collection of documents. It is one of the most popular methods used for feature 
    ''' extraction in text data. The TF-IDF score for a word increases proportionally to the number of times a word 
    ''' appears in the document but is offset by the frequency of the word in the collection of documents.
    ''' **Components of TF-IDF:**
    ''' 1. **Term Frequency (TF):** 
    '''   - Measures how frequently a term occurs in a document. 
    '''   - Calculated as the number of times a term appears in a document divided by the total number of terms in the document.
    '''   - Formula: \( \text{TF}(t,d) = \frac{\text{Number of times term t appears in document d}}{\text{Total number of terms in document d}} \)
    ''' 2. **Inverse Document Frequency (IDF):**
    '''   - Measures how important a term is to a collection of documents.
    '''  - Calculated as the logarithm of the total number of documents divided by the number of documents containing the term.
    '''   - Formula: \( \text{IDF}(t,D) = \log\left(\frac{\text{Total number of documents}}{\text{Number of documents containing term t}}\right) \)
    ''' 3. **TF-IDF Score:**
    '''  - Combines TF and IDF to compute the TF-IDF score for a term in a document.
    '''   - Formula: \( \text{TF-IDF}(t,d,D) = \text{TF}(t,d) \times \text{IDF}(t,D) \)
    ''' **Interpretation:**
    ''' - A high TF-IDF score indicates that the term is highly relevant to the document and rare across all documents.
    ''' - A low TF-IDF score indicates that the term is either not very relevant to the document or very common across all documents.
    ''' **Applications:**
    ''' - **Information Retrieval:** Used to rank documents based on the relevance of search queries.
    ''' - **Text Mining:** Helps in identifying important terms and features in text data.
    ''' - **Machine Learning:** Used as a feature vector for text classification, clustering, and other NLP tasks.
    ''' **Advantages:**
    ''' - Simple and intuitive.
    ''' - Effective in identifying important terms in a document.
    ''' - Widely used and well-understood in the field of text analysis.
    ''' **Disadvantages:**
    ''' - Does not consider the semantic meaning of words.
    ''' - Can be biased towards longer documents.
    ''' - May not perform well with very rare or very common terms.
    ''' **Example:**
    ''' Consider a collection of documents about animals. The term "animal" might appear frequently in all documents, 
    ''' resulting in a high TF but a low IDF, and thus a lower TF-IDF score. Conversely, a specific term like "giraffe"
    ''' might appear less frequently but only in a few documents, resulting in a higher TF-IDF score, indicating its 
    ''' relevance to those specific documents.
    ''' 
    ''' In summary, TF-IDF is a powerful and widely-used technique for text analysis that helps in identifying the 
    ''' importance of terms within a document relative to a collection of documents.
    ''' </remarks>
    Public Class TF_IDF

        Private stopwords As ISet(Of String) ' a common set of English stopwords

        Private docs As IList(Of IList(Of String)) ' documents as bags of words, with stopwords removed
        Private numDocs As Integer

        Private terms As List(Of String) ' unique terms
        Private numTerms As Integer

        Private termFreq As Integer()() ' tf matrix
        Private termWeight As Double()() ' tf-idf matrix
        Private docFreq As Integer() ' terms' frequency in all documents

        ''' <summary>
        ''' Constructor </summary>
        ''' <paramname="documents"> documents represented as strings </param>
        Public Sub New(documents As String())

            stopwords = loadStopWords("stoplist.txt")

            docs = parseDocuments(documents)
            numDocs = docs.Count

            terms = generateTerms(docs)
            numTerms = terms.Count

            docFreq = New Integer(numTerms - 1) {}
            termFreq = RectangularArray.Matrix(Of Integer)(numTerms, numDocs)
            termWeight = RectangularArray.Matrix(Of Double)(numTerms, numDocs)

            countTermOccurrence()
            generateTermWeight()
        End Sub

        ''' <summary>
        ''' Load stopwords from a file </summary>
        ''' <paramname="filename"> </param>
        ''' <returns>  </returns>
        Private Function loadStopWords(filename As String) As ISet(Of String)
            Dim stoplist As ISet(Of String) = New HashSet(Of String)()

            Try
                Dim [in] As Stream = Nothing '= this.GetType().getResourceAsStream(filename);
                Dim br As StreamReader = New StreamReader([in])
                Dim line As Value(Of String) = ""
                While Not (line = br.ReadLine()) Is Nothing
                    stoplist.Add(line)
                End While
            Catch e As IOException
                Console.WriteLine(e.ToString())
                Console.Write(e.StackTrace)
            End Try

            Return stoplist
        End Function

        ''' <summary>
        ''' Parse documents into bags of words </summary>
        ''' <paramname="docs"> documents in strings </param>
        ''' <returns> a list of documents represented by bags of words </returns>
        Private Function parseDocuments(docs As String()) As IList(Of IList(Of String))
            Dim parsedDocs As IList(Of IList(Of String)) = New List(Of IList(Of String))()

            For Each doc In docs
                Dim words As String() = doc.StringReplace("\p{Punct}", "").ToLower().StringSplit("\s", True)
                Dim wordList As IList(Of String) = New List(Of String)()
                For Each wordi In words
                    Dim word = wordi.Trim()
                    If word.Length > 0 AndAlso Not stopwords.Contains(word) Then
                        wordList.Add(word)
                    End If
                Next
                parsedDocs.Add(wordList)
            Next

            Return parsedDocs
        End Function

        ''' <summary>
        ''' Generate terms from a list of documents </summary>
        ''' <paramname="docs"> </param>
        ''' <returns>  </returns>
        Private Function generateTerms(docs As IList(Of IList(Of String))) As List(Of String)
            Dim uniqueTerms As List(Of String) = New List(Of String)()
            For Each doc In docs
                For Each word In doc
                    If Not uniqueTerms.Contains(word) Then
                        uniqueTerms.Add(word)
                    End If
                Next
            Next
            Return uniqueTerms
        End Function

        ''' <summary>
        ''' Count term occurrence
        ''' and occurrence of each term in the whole corpus
        ''' </summary>
        Private Sub countTermOccurrence()
            For i = 0 To docs.Count - 1
                Dim doc = docs(i)
                Dim tfMap = countTermOccurrenceInOneDoc(doc)
                For Each entry In tfMap
                    Dim word = entry.Key
                    Dim wordFreq = entry.Value
                    Dim termIndex = terms.IndexOf(word)

                    termFreq(termIndex)(i) = wordFreq
                    docFreq(termIndex) += 1
                Next
            Next
        End Sub

        ''' <summary>
        ''' Count term frequency in a document </summary>
        ''' <paramname="doc"> a document as a bag of words </param>
        ''' <returns> a map of term occurrence; key - term; value - occurrence. </returns>
        Private Function countTermOccurrenceInOneDoc(doc As IList(Of String)) As Dictionary(Of String, Integer)

            Dim tfMap As Dictionary(Of String, Integer) = New Dictionary(Of String, Integer)()

            For Each word In doc
                Dim count = 0
                For Each str As String In doc
                    If str.Equals(word) Then
                        count += 1
                    End If
                Next

                tfMap(word) = count
            Next

            Return tfMap
        End Function

        ''' <summary>
        ''' Calculate term weight based on tf*idf algorithm
        ''' There are different choices in calculating tf and idf.
        ''' So you may want to change it to fit your own needs.
        ''' </summary>
        Private Sub generateTermWeight()
            For i = 0 To numTerms - 1
                For j = 0 To numDocs - 1
                    Dim tf = getTFMeasure(i, j)
                    Dim idf = getIDFMeasure(i)
                    termWeight(i)(j) = tf * idf
                Next
            Next
        End Sub

        Private Function getTFMeasure(term As Integer, doc As Integer) As Double
            Dim freq = termFreq(term)(doc)
            Return System.Math.Sqrt(freq)
        End Function

        Private Function getIDFMeasure(term As Integer) As Double
            Dim df = docFreq(term)
            Return 1.0R + System.Math.Log(numDocs / (1.0R + df))
        End Function

        ''' <summary>
        ''' Get similarity score between two documents </summary>
        ''' <paramname="doc_i"> index of one document </param>
        ''' <paramname="doc_j"> index of another document </param>
        ''' <returns> similarity score </returns>
        Public Overridable Function getSimilarity(doc_i As Integer, doc_j As Integer) As Double
            Dim vector1 = getDocumentVector(doc_i)
            Dim vector2 = getDocumentVector(doc_j)
            Return computeCosineSimilarity(vector1, vector2)
        End Function

        ''' <summary>
        ''' Compile a vector for a document </summary>
        ''' <paramname="docIndex"> index of a document </param>
        ''' <returns> the vector representation of the document </returns>
        Private Function getDocumentVector(docIndex As Integer) As Double()
            Dim v = New Double(numTerms - 1) {}
            For i = 0 To numTerms - 1
                v(i) = termWeight(i)(docIndex)
            Next
            Return v
        End Function

        ''' <summary>
        ''' Calculate cosine similarity between two vectors </summary>
        ''' <paramname="vector1"> a vector </param>
        ''' <paramname="vector2"> another vector </param>
        ''' <returns> cosine similarity score </returns>
        Public Shared Function computeCosineSimilarity(vector1 As Double(), vector2 As Double()) As Double
            If vector1.Length <> vector2.Length Then
                Console.WriteLine("Different vector length.")
            End If

            Dim denom = vectorLength(vector1) * vectorLength(vector2)
            If denom = 0.0R Then
                Return 0.0R
            Else
                Return innerProduct(vector1, vector2) / denom
            End If
        End Function

        ''' <summary>
        ''' Calculate inner product of two vectors </summary>
        ''' <paramname="vector1"> a vector </param>
        ''' <paramname="vector2"> another vector </param>
        ''' <returns> inner production of two vectors </returns>
        Public Shared Function innerProduct(vector1 As Double(), vector2 As Double()) As Double
            Dim result = 0.0R
            For i = 0 To vector1.Length - 1
                result += vector1(i) * vector2(i)
            Next
            Return result
        End Function

        ''' <summary>
        ''' Calculate vector length </summary>
        ''' <paramname="vector"> a vector </param>
        ''' <returns> vector length </returns>
        Public Shared Function vectorLength(vector As Double()) As Double
            Dim sum = 0.0R
            For Each d In vector
                sum += d * d
            Next
            Return System.Math.Sqrt(sum)
        End Function


        ''' <summary>
        ''' Testing </summary>
        ''' <paramname="args">  </param>
        Public Shared Sub Main(args As String())
            Dim docs = New String() {"knowledge building needs innovative environments are better at helping their inhabitants explore the adjacent possible", "As a basis for evaluating explanations, creative knowledge building weight of evidence is a poor substitute for the first two criteria listed above.", "A public idea database makes every passing idea visible to everyone else in the organization and do creative work.", "questioning and various disturbances initiate cycles of innovation and creative organization knowledge.", "We need some way to ensure knowledge to spread among environments that any notes that are dropped are dropped."}

            Dim tfIdf As TF_IDF = New TF_IDF(docs)
            For i = 0 To tfIdf.docs.Count - 1
                Console.Write(i + 1.ToString() & vbTab)
                For j = 0 To tfIdf.docs.Count - 1
                    Console.Write(tfIdf.getSimilarity(i, j).ToString() & vbTab)
                Next
                Console.WriteLine()
            Next
        End Sub
    End Class

End Namespace
